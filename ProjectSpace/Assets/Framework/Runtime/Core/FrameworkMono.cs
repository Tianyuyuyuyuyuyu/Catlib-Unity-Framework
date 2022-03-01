using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
#if UNITY_EDITOR
using Framework.Runtime.HotFix;
#endif

namespace Framework.Runtime.Core
{
    public partial class FrameworkMono : SerializedMonoBehaviour
    {
        /// <summary>
        /// 核心
        /// </summary>
        [Title("框架")]
        [ValueDropdown("GetCoreMainType")]
        [LabelText("主")]
        public Type CoreMainType;
        
        /// <summary>
        /// 核心
        /// </summary>
        [ValueDropdown("GetCoreHotFixType")]
        [LabelText("热更")]
        public Type CoreHotFixType;
        
        /// <summary>
        /// 启动模式
        /// </summary>
        [Title("配置")]
        [LabelText("模式")]
        public LauncherMode Mode;
        
        /// <summary>
        /// 自动注册程序集
        /// </summary>
        [LabelText("自动注册程序集")]
        public bool AutoRegisterAssembly;

        /// <summary>
        /// 手动注册程序集列表
        /// </summary>
        [LabelText("手动注册程序集列表")]
        [HideIf("AutoRegisterAssembly"), ValueDropdown("GetAssemblyList")]
        public List<string> RegisterAssemblyList;

        /// <summary>
        /// 每帧更新
        /// </summary>
        public static Action OnUpdate;
        
        /// <summary>
        /// 框架销毁更新
        /// </summary>
        public static Action OnFrameworkDestroy;

        /// <summary>
        /// MainOnLauncher之前
        /// </summary>
        public static Func<UniTask> BeforeMainOnLauncher;
        
        /// <summary>
        /// HotFixOnLauncher之前
        /// </summary>
        public static Func<UniTask> BeforeHotFixOnLauncher;

#if UNITY_EDITOR
        /// <summary>
        /// 获取核心类型
        /// </summary>
        /// <returns>核心类型列表</returns>
        IEnumerable GetCoreMainType()
        {
            var types = AssemblyUtilities.GetTypes(AssemblyTypeFlags.CustomTypes)
                .Where((x => !x.IsAbstract))
                .Where((x => !x.IsGenericTypeDefinition))
                .Where((x => typeof(CoreMain).IsAssignableFrom(x)));

            return types;
        }
        /// <summary>
        /// 获取核心类型
        /// </summary>
        /// <returns>核心类型列表</returns>
        IEnumerable GetCoreHotFixType()
        {
            var types = AssemblyUtilities.GetTypes(AssemblyTypeFlags.CustomTypes)
                .Where((x => !x.IsAbstract))
                .Where((x => !x.IsGenericTypeDefinition))
                .Where((x => typeof(CoreHotFix).IsAssignableFrom(x)));

            return types;
        }
#endif

        /// <summary>
        /// 获取程序集列表
        /// </summary>
        IEnumerable GetAssemblyList()
        {
            return AppDomain.CurrentDomain.GetAssemblies().Select((assembly => new ValueDropdownItem(assembly.FullName, assembly.FullName)));
        }

        /// <summary>
        /// 启动主工程框架
        /// </summary>
        public async UniTask LauncherMain()
        {
            var coreMain = Activator.CreateInstance(CoreMainType);
            if (coreMain != null)
            {
                CoreMainType.GetMethod("InitComponents").Invoke(coreMain, null);
            }
            
            if (BeforeMainOnLauncher != null)
            {
                foreach (var @delegate in BeforeMainOnLauncher.GetInvocationList())
                {
                    if (@delegate is Func<UniTask> func)
                    {
                        await func.Invoke();
                    }
                }
            }
            
            await FrameworkComponent.OnLauncher();
        }

        /// <summary>
        /// 启动热更工程框架
        /// </summary>
        public async UniTask LauncherHotFix()
        {
            List<Type> types = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (AutoRegisterAssembly || RegisterAssemblyList.Contains(assembly.FullName))
                {
                    types.AddRange(assembly.GetTypes());
                }
            }

            var mainTypes = types.ToArray();
            
            await UniTask.WaitForEndOfFrame();
            
            if (Mode == LauncherMode.HotFix)
            {
                ILRuntimeWrapper.AppDomain.Invoke("Wingjoy.FrameworkMono.Runtime.HotFix.CoreHotFix", "StartGame", null,
                    new object[] {this, CoreHotFixType, mainTypes, ILRuntimeWrapper.GetHotfixTypes().ToArray()});
            }
            else
            {
                Type.GetType("Wingjoy.FrameworkMono.Runtime.HotFix.CoreHotFix").GetMethod("StartGame").Invoke(null, new object[] {this, CoreHotFixType, mainTypes, mainTypes});
            }
            
            await UniTask.WaitForEndOfFrame();
            
            if (BeforeHotFixOnLauncher != null)
            {
                foreach (var @delegate in BeforeHotFixOnLauncher.GetInvocationList())
                {
                    if (@delegate is Func<UniTask> func)
                    {
                        await func.Invoke();
                    }
                }
            }
            
            if (Mode == LauncherMode.HotFix)
            {
                ILRuntimeWrapper.AppDomain.Invoke("Wingjoy.FrameworkMono.Runtime.HotFix.FrameworkHotFixComponent", "OnLauncher", null);
            }
            else
            {
                Type.GetType("Wingjoy.FrameworkMono.Runtime.HotFix.FrameworkHotFixComponent").GetMethod("OnLauncher").Invoke(null, null);
            }
        }
        
        /// <summary>
        /// 启动
        /// </summary>
        [Obsolete("Use LauncherMain and LauncherHotFix")]
        public async UniTask Launcher()
        {
            List<Type> types = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (AutoRegisterAssembly || RegisterAssemblyList.Contains(assembly.FullName))
                {
                    types.AddRange(assembly.GetTypes());
                }
            }

            var mainTypes = types.ToArray();
            
            var coreMain = Activator.CreateInstance(CoreMainType);
            if (coreMain != null)
            {
                CoreMainType.GetMethod("InitComponents").Invoke(coreMain, null);
            }

            if (Mode == LauncherMode.HotFix)
            {
                ILRuntimeWrapper.AppDomain.Invoke("Wingjoy.FrameworkMono.Runtime.HotFix.CoreHotFix", "StartGame", null,
                    new object[] {this, CoreHotFixType, mainTypes, ILRuntimeWrapper.GetHotfixTypes().ToArray()});
            }
            else
            {
                Type.GetType("Wingjoy.FrameworkMono.Runtime.HotFix.CoreHotFix").GetMethod("StartGame").Invoke(null, new object[] {this, CoreHotFixType, mainTypes, mainTypes});
            }

            //初始化完成

            if (BeforeMainOnLauncher != null)
            {
                await BeforeMainOnLauncher.Invoke();    
            }
            
            if (BeforeHotFixOnLauncher != null)
            {
                await BeforeHotFixOnLauncher.Invoke();    
            }

            //开始OnLauncher
            
            if (Mode == LauncherMode.HotFix)
            {
                ILRuntimeWrapper.AppDomain.Invoke("Wingjoy.FrameworkMono.Runtime.HotFix.FrameworkHotFixComponent", "OnLauncher", null);
            }
            else
            {
                Type.GetType("Wingjoy.FrameworkMono.Runtime.HotFix.FrameworkHotFixComponent").GetMethod("OnLauncher").Invoke(null, null);
            }

            await FrameworkComponent.OnLauncher();
        }
        
        private void Update()
        {
            OnUpdate?.Invoke();
        }

        private void OnDestroy()
        {
            OnFrameworkDestroy?.Invoke();
        }
    }
}