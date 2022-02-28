using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Wingjoy.Framework.Runtime;
using WingjoyUtility.Runtime;

namespace Wingjoy
{
    public class Launcher : SerializedMonoBehaviour
    {
        /// <summary>
        /// 使用本地DLL
        /// </summary>
        [SerializeField]
        [LabelText("使用工程内代码")]
        private bool m_UseLocal;

        /// <summary>
        /// 绑定类型
        /// </summary>
        [SerializeField]
        [ValueDropdown("GetBindType")]
        [LabelText("ILRuntime绑定")]
        private Type m_BindType;

        /// <summary>
        /// 内建Bundles
        /// </summary>
        private HashSet<string> m_BuildInBundle;

        /// <summary>
        /// 更新进度
        /// </summary>
        private Action<float> m_OnUpdateProgress;

        // Start is called before the first frame update
        public virtual async void Start()
        {
            Debug.Log("开始读取本地Bundle");
            ReadBuiltInBundle();

            Addressables.InternalIdTransformFunc = InternalIdTransformFunc;
            await Addressables.InitializeAsync();

            await OnAddressableInitializeSuccess();

            var wingjoyFramework = GetComponent<WingjoyFramework>();
            await wingjoyFramework.LauncherMain(); //主工程框架启动完毕

            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                //开始热更检测
                var checkForCatalogUpdates = Addressables.CheckForCatalogUpdates(false);
                await checkForCatalogUpdates;

                if (checkForCatalogUpdates.Result.Count > 0)
                {
                    //更新Catalog
                    var updateCatalogs = Addressables.UpdateCatalogs(checkForCatalogUpdates.Result, false);
                    await updateCatalogs;
                    Addressables.Release(updateCatalogs);
                }

                Addressables.Release(checkForCatalogUpdates);    
                
                //获取所有资源位置
                List<object> keys = new List<object>();
                foreach (var resourceLocator in Addressables.ResourceLocators)
                {
                    keys.AddRange(resourceLocator.Keys);
                }

                //验证是否需要下载更新
                var downloadSizeAsync = Addressables.GetDownloadSizeAsync(keys);
                await downloadSizeAsync;

                float size = downloadSizeAsync.Result / 1000f / 1000f;
                if (size > 0)
                {
                    //提示下载
                    var displayNeedDownloadDialog = await DisplayNeedDownloadDialog(size);
                    if (displayNeedDownloadDialog)
                    {
                        //获取是否需要下载本地化文本
                        var locDicDownloadSize = Addressables.GetDownloadSizeAsync(CoreMain.Loc.GetLoadLanguageDictionaryKeys());
                        await locDicDownloadSize;
                        var needReloadLocDic = locDicDownloadSize.Result > 0;
                        Addressables.Release(locDicDownloadSize);
                    
                        var downloadDependenciesAsync = Addressables.DownloadDependenciesAsync(keys, Addressables.MergeMode.Union, false);
                        OnDownloadStart();
                        while (downloadDependenciesAsync.Status == AsyncOperationStatus.None)
                        {
                            var downloadStatus = downloadDependenciesAsync.GetDownloadStatus();
                            OnDownloadProgressUpdate(downloadStatus);
                            await UniTask.WaitForEndOfFrame();
                        }
                        OnDownloadEnd();
                        Addressables.Release(downloadDependenciesAsync);
                    
                        //发现需要重新加载
                        if (needReloadLocDic)
                        {
                            Debug.Log("检测到本地化文本更新，需要重新加载");
                            await CoreMain.Loc.LoadLanguageDictionary();
                        }
                    }
                }
            }
            
            await LoadHotFixCodeAsset();
        }

        /// <summary>
        /// Addressable初始化成功
        /// </summary>
        public virtual async UniTask OnAddressableInitializeSuccess()
        {
            
        }

        /// <summary>
        /// 开始下载时
        /// </summary>
        public virtual void OnDownloadStart()
        {
            
        }

        /// <summary>
        /// 停止下载时
        /// </summary>
        public virtual void OnDownloadEnd()
        {
            
        }

        /// <summary>
        /// 下载进度变更
        /// </summary>
        /// <param name="downloadStatus">下载状态</param>
        public virtual void OnDownloadProgressUpdate(DownloadStatus downloadStatus)
        {
            
        }

        /// <summary>
        /// 读取APK包中的资源列表
        /// </summary>
        public virtual void ReadBuiltInBundle()
        {
            var json = Resources.Load<TextAsset>("BuiltInBundle");
            if (json != null)
            {
                var builtInBundle = JsonUtility.FromJson<BuiltInBundle>(json.text);
                m_BuildInBundle = new HashSet<string>(builtInBundle.BundleNames);
            }
            else
            {
                m_BuildInBundle = new HashSet<string>();
                Debug.LogError("无法读取到本地BuiltInBundle文件");
            }
        }

        /// <summary>
        /// 路径转变函数
        /// </summary>
        /// <param name="location">路径</param>
        /// <returns>转换路径</returns>
        public virtual string InternalIdTransformFunc(IResourceLocation location)
        {
            //判定是否是一个AB包的请求
            if (location.Data is AssetBundleRequestOptions)
            {
                //PrimaryKey是AB包的名字
                //path就是StreamingAssets/aa/RemoteBundle/AB包名.bundle,其中Bundles是自定义文件夹名字,发布应用程序时,复制的目录
                if (m_BuildInBundle.Contains(location.PrimaryKey))
                {
                    var internalIdTransformFunc = Path.Combine(Addressables.RuntimePath, "RemoteBundle", location.PrimaryKey);
                    return internalIdTransformFunc;
                }
            }

            return location.InternalId;
        }

        /// <summary>
        /// 显示是否继续下载对话框
        /// </summary>
        /// <param name="mb">大小</param>
        /// <returns>是否继续下载</returns>
        public virtual async UniTask<bool> DisplayNeedDownloadDialog(float mb)
        {
            Debug.Log($"需要下载{mb}mb大小的文件");
            return true;
        }

        /// <summary>
        /// 加载热更代码
        /// </summary>
        public virtual async UniTask LoadHotFixCodeAsset()
        {
            var wingjoyFramework = GetComponent<WingjoyFramework>();
            if (wingjoyFramework.Mode == LauncherMode.Editor)
            {
                await wingjoyFramework.LauncherHotFix();
            }
            else
            {
                (byte[] dll, byte[] pdb) code = default;
#if UNITY_EDITOR
                if (m_UseLocal)
                {
                    code = LoadProjectAsset();
                }
                else
#endif
                {
                    var asyncOperationHandle = Addressables.LoadAssetAsync<TextAsset>("Assets/Resource/HotFix/hotfix.bytes");
                    await asyncOperationHandle;
                    code.dll = asyncOperationHandle.Result.bytes;
                }

                await LoadHotFixCode(code);
            }
        }

        /// <summary>
        /// 加载热更代码
        /// </summary>
        /// <param name="code">代码</param>
        public virtual async UniTask LoadHotFixCode((byte[] dll, byte[] pdb) code)
        {
            UniTaskCompletionSource uniTaskCompletionSource = new UniTaskCompletionSource();
            if (Activator.CreateInstance(m_BindType) is IBinding iBinding)
            {
                ILRuntimeWrapper.RegisterCrossBindingAdaptorCallback = iBinding.RegisterCrossBindingAdaptor();
                ILRuntimeWrapper.CLRBindingsInitializeCallback = iBinding.InitializeCLRBingding();
                ILRuntimeWrapper.CLRManualBindingsInitializeCallback = iBinding.InitializeCLRManualBingding();
                ILRuntimeWrapper.DelegateHelperRegisterCallback = iBinding.DelegateRegister();
            }

            ILRuntimeWrapper.LoadHotFixAssembly(code.dll, code.pdb, (async () =>
            {
                var wingjoyFramework = GetComponent<WingjoyFramework>();
                await wingjoyFramework.LauncherHotFix();
                uniTaskCompletionSource.TrySetResult();
            }));
        }

#if UNITY_EDITOR

        /// <summary>
        /// 获取绑定类型
        /// </summary>
        /// <returns>绑定类型列表</returns>
        IEnumerable GetBindType()
        {
            var types = AssemblyUtilities.GetTypes(AssemblyTypeFlags.CustomTypes)
                .Where((x => !x.IsAbstract))
                .Where((x => !x.IsGenericTypeDefinition))
                .Where((x => typeof(IBinding).IsAssignableFrom(x)));

            return types;
        }

        /// <summary>
        /// 从工程读取dll与pdb
        /// </summary>
        /// <returns>dll与pdb</returns>
        public (byte[] dll, byte[] pdb) LoadProjectAsset()
        {
            var path = Path.Combine(Application.dataPath + "/Resource/HotFix~",
                EditorUserBuildSettings.activeBuildTarget.ToString());
            var dllPath = Path.Combine(path, "Hotfix/hotfix.dll");
            var dllBytes = File.ReadAllBytes(dllPath);
            var pdbPath = dllPath + ".pdb";

            byte[] pdbBytes = null;
            if (File.Exists(pdbPath))
            {
                pdbBytes = File.ReadAllBytes(pdbPath);
            }

            return (dllBytes, pdbBytes);
        }
#endif
    }
}