using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Wingjoy.Framework.Runtime
{
    public class WingjoyFrameworkComponent : MonoBehaviour
    {
        /// <summary>
        /// 存放所有的组件
        /// </summary>
        private static readonly Dictionary<Type, WingjoyFrameworkComponent> WingjoyFrameworkComponentsDic = new Dictionary<Type, WingjoyFrameworkComponent>();

        /// <summary>
        /// 获取游戏框架组件。
        /// </summary>
        /// <typeparam name="T">要获取的游戏框架组件类型。</typeparam>
        /// <returns>要获取的游戏框架组件。</returns>
        public static T GetFrameworkComponent<T>() where T : WingjoyFrameworkComponent
        {
            return (T) GetFrameworkComponent(typeof(T));
        }

        /// <summary>
        /// 获取游戏框架组件。
        /// </summary>
        /// <param name="type">要获取的游戏框架组件类型。</param>
        /// <returns>要获取的游戏框架组件。</returns>
        public static WingjoyFrameworkComponent GetFrameworkComponent(Type type)
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
        internal static void RegisterComponent(WingjoyFrameworkComponent wingjoyFrameworkHotFixComponent)
        {
            if (wingjoyFrameworkHotFixComponent == null)
            {
                Debug.LogError("Wingjoy Framework component is invalid.");
                return;
            }

            Type type = wingjoyFrameworkHotFixComponent.GetType();


            if (WingjoyFrameworkComponentsDic.ContainsKey(type))
            {
                Debug.LogErrorFormat("Wingjoy Framework component type '{0}' is already exist.", type.FullName);
                return;
            }
            else
            {
                WingjoyFrameworkComponentsDic.Add(type, wingjoyFrameworkHotFixComponent);
            }
        }

        protected virtual void Awake()
        {
            RegisterComponent(this);
        }

        /// <summary>
        /// 游戏框架初次启动完毕后
        /// </summary>
        public static async UniTask OnLauncher()
        {
            List<UniTask> uniTasks = new List<UniTask>();
            foreach (var wingjoyFrameworkComponent in WingjoyFrameworkComponentsDic)
            {
                Debug.Log(wingjoyFrameworkComponent.Key);
                var uniTask = wingjoyFrameworkComponent.Value.Launcher();
                uniTasks.Add(uniTask);
            }

            await UniTask.WhenAll(uniTasks);
        }

        /// <summary>
        /// 只在游戏框架组件初次启动完毕后回调
        /// </summary>
        public virtual async UniTask Launcher()
        {
        }
    }
}