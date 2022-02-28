/**
*Copyright(C) 2017 by Wingjoy
*All rights reserved.
*ProductName:  SecondLife2
*Author:       Administrator
*Version:      1.0
*UnityVersion: 2019.4.9f1
*CreateTime:   2021/05/20 11:51:38
*Description:   文档描述待填写
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ILRuntime.Runtime.Enviorment;
using System.IO;
using LitJson;
using System.Threading;
using System;
using System.Linq;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Wingjoy.Framework.Runtime
{
    /// <summary>
    /// ILRuntime控制类
    /// </summary>
    public class ILRuntimeWrapper
    {
        //AppDomain是ILRuntime的入口，最好是在一个单例类中保存，整个游戏全局就一个
        //在正式项目中请全局只创建一个AppDomain
        public static AppDomain AppDomain { get; private set; }

        private static System.IO.MemoryStream fs;
        private static System.IO.MemoryStream p;

        /// <summary>
        /// CLRBingding回调
        /// </summary>
        public static Action<AppDomain> CLRBindingsInitializeCallback;

        /// <summary>
        /// CLRManualBinding回调
        /// </summary>
        public static Action<AppDomain> CLRManualBindingsInitializeCallback;

        /// <summary>
        /// 委托跨域注册回调
        /// </summary>
        public static Action<AppDomain> DelegateHelperRegisterCallback;

        /// <summary>
        /// 适配器注册回调
        /// </summary>
        public static Action<AppDomain> RegisterCrossBindingAdaptorCallback;

        /// <summary>
        /// 加载热更dll
        /// </summary>
        /// <param name="dll">程序集字节数据</param>
        /// <param name="pdb">调试数据库/param>
        public static void LoadHotFixAssembly(byte[] dll, byte[] pdb, Action callback = null)
        {
            AppDomain = new AppDomain();

            fs = new MemoryStream(dll);
            //p = new MemoryStream(pdb);
            try
            {
                AppDomain.LoadAssembly(fs, null, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
            }
            catch
            {
                Debug.LogError("加载热更DLL失败，请确保已经编译过热更DLL");
                return;
            }

            InitializeILRuntime();

            callback?.Invoke();
        }

        /// <summary>
        /// 初始化ILRuntime
        /// </summary>
        private static void InitializeILRuntime()
        {
#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
            //由于Unity的Profiler接口只允许在主线程使用，为了避免出异常，需要告诉ILRuntime主线程的线程ID才能正确将函数运行耗时报告给Profiler
            AppDomain.UnityMainThreadID = Thread.CurrentThread.ManagedThreadId;
#endif

            //注册适配器
            //AdapterRegister.RegisterCrossBindingAdaptor(AppDomain);
            RegisterCrossBindingAdaptorCallback?.Invoke(AppDomain);

            //CLRBinding
            CLRBindingsInitializeCallback?.Invoke(AppDomain);
            CLRManualBindingsInitializeCallback?.Invoke(AppDomain);

            //委托跨域注册
            DelegateHelperRegisterCallback?.Invoke(AppDomain);

            //重定向
            JsonMapper.RegisterILRuntimeCLRRedirection(AppDomain);

            //在函数入口添加代码启动调试服务
            // ILRuntime建议全局只创建一个AppDomain，在函数入口添加代码启动调试服务
            //运行主工程(Unity工程)
            //在热更的VS工程中 点击 -调试 - 附加到ILRuntime调试，注意使用一样的端口
            if (Application.isEditor)
            {
                //热更调试器
                AppDomain.DebugService.StartDebugService(56000);
            }
        }

        public static void Close()
        {
            AppDomain = null;

            if (fs != null)
            {
                fs.Dispose();
            }
        }

        static private List<Type> hotfixTypeList = null;

        /// <summary>
        /// 获取所有的hotfix的类型
        /// </summary>
        /// <returns></returns>
        public static List<Type> GetHotfixTypes()
        {
            if (hotfixTypeList == null)
            {
                hotfixTypeList = new List<Type>();
                var values = AppDomain.LoadedTypes.Values.ToList();
                foreach (var v in values)
                {
                    hotfixTypeList.Add(v.ReflectionType);
                }
            }

            return hotfixTypeList;
        }
    }
}