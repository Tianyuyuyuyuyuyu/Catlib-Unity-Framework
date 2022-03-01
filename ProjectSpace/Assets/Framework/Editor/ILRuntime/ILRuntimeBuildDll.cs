using ILRuntime.Runtime.CLRBinding;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using System;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Framework.Runtime;
using Debug = UnityEngine.Debug;

namespace Framework.Editor.ILRuntime
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class ILRuntimeBuildDll : OdinEditorWindow
    {
        public static string HotFixCodePath=> Application.dataPath + "/Resource/HotFix~";

        [MenuItem("Framework/ILRuntime", false, 54)]
        public static void OpenWindows()
        {
            var window = GetWindow(typeof(ILRuntimeBuildDll), false, "DLL打包工具");
            window.Show();
            //创建BuildDllConfig
            var buildDllConfig = BuildDllConfig.Instance;
        }
        
        [Button(ButtonSizes.Large),LabelText("1.编译dll(Roslyn-Release)")]
        [VerticalGroup("Button")]
        [HorizontalGroup("Button/Build")]
        public void RoslynRelease()
        {
            BindingCallback();
            RoslynBuild(ScriptBiuld_Service.BuildMode.Release);
        }


        [Button(ButtonSizes.Large), LabelText("1.编译dll(Roslyn-Debug)")]
        [HorizontalGroup("Button/Build")]
        public void RoslynDebug()
        {
            BindingCallback();
            RoslynBuild(ScriptBiuld_Service.BuildMode.Debug);
        }
        
        [Button(ButtonSizes.Large),LabelText("2.生成CLRBinding")]
        [HorizontalGroup("Button/Analysis")]
        public static void GenCLRBindingByAnalysis()
        {
            var dllPath = Path.Combine(HotFixCodePath, EditorUserBuildSettings.activeBuildTarget.ToString(), "HotFix/hotfix.dll");
            var dllText = File.ReadAllBytes(dllPath);
            //用新的分析热更dll调用引用来生成绑定代码
            ILRuntimeWrapper.LoadHotFixAssembly(dllText, null);
            BindingCodeGenerator.GenerateBindingCode(ILRuntimeWrapper.AppDomain, "Assets/Scripts/ILRuntime/Binding");
            AssetDatabase.Refresh();
        }

        [Button(ButtonSizes.Large), LabelText("3.生成跨域Adapter")]
        [HorizontalGroup("Button/Adapter")]
        public void GenCrossBindAdapter([Sirenix.OdinInspector.FilePath] string path, Type type, string namespaceStr)
        {
            //由于跨域继承特殊性太多，自动生成无法实现完全无副作用生成，所以这里提供的代码自动生成主要是给大家生成个初始模版，简化大家的工作
            //大多数情况直接使用自动生成的模版即可，如果遇到问题可以手动去修改生成后的文件，因此这里需要大家自行处理是否覆盖的问题
            
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path))
            {
                //第二个参数：命名空间名称
                sw.WriteLine(global::ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.GenerateCrossBindingAdapterCode(type, namespaceStr));
            }
            
            AssetDatabase.Refresh();
        }

        [Button(ButtonSizes.Large), LabelText("安装VS调试插件(需要关闭相关的进程)")]
        [HorizontalGroup("Button/Helper")]
        public void InstallVSDebugger()
        {
            EditorUtility.OpenWithDefaultApp("Assets/Debugger~/ILRuntimeDebuggerLauncher.vsix");
        }

        [Button(ButtonSizes.Large), LabelText("打开ILRuntime中文文档")]
        [HorizontalGroup("Button/Helper")]
        public void OpenDocument()
        {
            Application.OpenURL("https://ourpalm.github.io/ILRuntime/");
        }

        /// <summary>
        /// 编译模式
        /// </summary>
        /// <param name="mode"></param>
        static void RoslynBuild(ScriptBiuld_Service.BuildMode mode)
        {
            //1.build dll
            ScriptBiuld_Service.BuildDll(HotFixCodePath, EditorUserBuildSettings.activeBuildTarget, mode);

            //2.同步到Resource文件夹
            var outpath_AB = Application.dataPath + "/Resource/HotFix/hotfix.bytes";
            var source = Path.Combine(HotFixCodePath, EditorUserBuildSettings.activeBuildTarget.ToString(), "HotFix/hotfix.dll");
            var bytes = File.ReadAllBytes(source);

            var _directoryName = Path.GetDirectoryName(outpath_AB);
            if (Directory.Exists(_directoryName) == false)
            {
                Directory.CreateDirectory(_directoryName);
            }

            File.WriteAllBytes(outpath_AB, bytes);

            //3.生成CLRBinding
            GenCLRBindingByAnalysis();
            AssetDatabase.Refresh();
            Debug.Log("脚本打包完毕");
        }

        /// <summary>
        /// 绑定Framework中初始化ILRutime回调
        /// </summary>
        static void BindingCallback()
        {
            var types = AssemblyUtilities.GetTypes(AssemblyTypeFlags.CustomTypes);

            foreach (var item in types)
            {
                if (item.IsInterface)
                {
                    continue;
                }

                Type[] ins = item.GetInterfaces();
                foreach (Type ty in ins)
                {
                    if (ty == typeof(IBinding))
                    {
                        IBinding v = Activator.CreateInstance(item) as IBinding;
                        ILRuntimeWrapper.DelegateHelperRegisterCallback = v.DelegateRegister();
                        ILRuntimeWrapper.CLRBindingsInitializeCallback = v.InitializeCLRBingding();
                        ILRuntimeWrapper.CLRManualBindingsInitializeCallback = v.InitializeCLRManualBingding();
                        ILRuntimeWrapper.RegisterCrossBindingAdaptorCallback = v.RegisterCrossBindingAdaptor();
                        break;
                    }
                }
            }
        }
    }
}