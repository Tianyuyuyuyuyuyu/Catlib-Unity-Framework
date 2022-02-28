using System;
using System.Collections.Generic;
using UnityEngine;

namespace Wingjoy.Framework.Runtime.HotFix
{
    public abstract class WingjoyFrameworkHotFixComponent
    {
        /// <summary>
        /// 存放所有的组件
        /// </summary>
        private static readonly Dictionary<Type, WingjoyFrameworkHotFixComponent> WingjoyFrameworkComponentsDic = new Dictionary<Type, WingjoyFrameworkHotFixComponent>();

        /// <summary>
        /// 获取游戏框架组件。
        /// </summary>
        /// <typeparam name="T">要获取的游戏框架组件类型。</typeparam>
        /// <returns>要获取的游戏框架组件。</returns>
        public static T GetFrameworkComponent<T>() where T : WingjoyFrameworkHotFixComponent
        {
            return (T) GetFrameworkComponent(typeof(T));
        }

        /// <summary>
        /// 获取游戏框架组件。
        /// </summary>
        /// <param name="type">要获取的游戏框架组件类型。</param>
        /// <returns>要获取的游戏框架组件。</returns>
        public static WingjoyFrameworkHotFixComponent GetFrameworkComponent(Type type)
        {
            if (WingjoyFrameworkComponentsDic.TryGetValue(type, out var component))
            {
                return component;
            }

            return null;
        }

        /// <summary>
        /// 注册游戏框架组件。
        /// </summary>
        /// <param name="wingjoyFrameworkHotFixComponent">要注册的游戏框架组件。</param>
        internal static void RegisterComponent(WingjoyFrameworkHotFixComponent wingjoyFrameworkHotFixComponent)
        {
            if (wingjoyFrameworkHotFixComponent == null)
            {
                Debug.LogError("Wingjoy HotFix Framework component is invalid.");
                return;
            }

            Type type = wingjoyFrameworkHotFixComponent.GetType();


            if (WingjoyFrameworkComponentsDic.ContainsKey(type))
            {
                Debug.LogErrorFormat("Wingjoy HotFix Framework component type '{0}' is already exist.", type.FullName);
                return;
            }
            else
            {
                WingjoyFrameworkComponentsDic.Add(type, wingjoyFrameworkHotFixComponent);
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
            foreach (var wingjoyFrameworkComponent in WingjoyFrameworkComponentsDic)
            {
                Debug.Log(wingjoyFrameworkComponent.Key);
                wingjoyFrameworkComponent.Value.Launcher();
            }
        }

        protected WingjoyFrameworkHotFixComponent()
        {
            RegisterComponent(this);
            WingjoyFramework.OnUpdate += Update;
            WingjoyFramework.OnFrameworkDestroy += OnDestroy;
        }

        /// <summary>
        /// 只在游戏框架组件初次启动完毕后回调
        /// </summary>
        public virtual void Launcher()
        {
        }
    }
}