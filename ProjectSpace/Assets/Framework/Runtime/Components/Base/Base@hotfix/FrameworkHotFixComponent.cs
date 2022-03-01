using System;
using System.Collections.Generic;
using Framework.Runtime.Core;
using UnityEngine;

namespace Framework.Runtime.HotFix
{
    public abstract class FrameworkHotFixComponent
    {
        /// <summary>
        /// 存放所有的组件
        /// </summary>
        private static readonly Dictionary<Type, FrameworkHotFixComponent> FrameworkComponentsDic = new Dictionary<Type, FrameworkHotFixComponent>();

        /// <summary>
        /// 获取游戏框架组件。
        /// </summary>
        /// <typeparam name="T">要获取的游戏框架组件类型。</typeparam>
        /// <returns>要获取的游戏框架组件。</returns>
        public static T GetFrameworkComponent<T>() where T : FrameworkHotFixComponent
        {
            return (T) GetFrameworkComponent(typeof(T));
        }

        /// <summary>
        /// 获取游戏框架组件。
        /// </summary>
        /// <param name="type">要获取的游戏框架组件类型。</param>
        /// <returns>要获取的游戏框架组件。</returns>
        public static FrameworkHotFixComponent GetFrameworkComponent(Type type)
        {
            if (FrameworkComponentsDic.TryGetValue(type, out var component))
            {
                return component;
            }

            return null;
        }

        /// <summary>
        /// 注册游戏框架组件。
        /// </summary>
        /// <param name="frameworkHotFixComponent">要注册的游戏框架组件。</param>
        internal static void RegisterComponent(FrameworkHotFixComponent frameworkHotFixComponent)
        {
            if (frameworkHotFixComponent == null)
            {
                Debug.LogError("Wingjoy HotFix FrameworkMono component is invalid.");
                return;
            }

            Type type = frameworkHotFixComponent.GetType();


            if (FrameworkComponentsDic.ContainsKey(type))
            {
                Debug.LogErrorFormat("Wingjoy HotFix FrameworkMono component type '{0}' is already exist.", type.FullName);
                return;
            }
            else
            {
                FrameworkComponentsDic.Add(type, frameworkHotFixComponent);
            }
        }

        /// <summary>
        /// 每帧更新
        /// </summary>
        public virtual void Update()
        {
        }

        /// <summary>
        /// 框架销毁
        /// </summary>
        public virtual void OnDestroy()
        {
            
        }

        /// <summary>
        /// 游戏框架初次启动完毕后
        /// </summary>
        public static void OnLauncher()
        {
            Debug.Log("OnLauncher");
            foreach (var FrameworkComponent in FrameworkComponentsDic)
            {
                Debug.Log(FrameworkComponent.Key);
                FrameworkComponent.Value.Launcher();
            }
        }

        protected FrameworkHotFixComponent()
        {
            RegisterComponent(this);
            FrameworkMono.OnUpdate += Update;
            FrameworkMono.OnFrameworkDestroy += OnDestroy;
        }

        /// <summary>
        /// 只在游戏框架组件初次启动完毕后回调
        /// </summary>
        public virtual void Launcher()
        {
        }
    }
}