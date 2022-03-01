using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Framework.Runtime
{
    public class FrameworkComponent : MonoBehaviour
    {
        /// <summary>
        /// 存放所有的组件
        /// </summary>
        private static readonly Dictionary<Type, FrameworkComponent> FrameworkComponentsDic = new Dictionary<Type, FrameworkComponent>();

        /// <summary>
        /// 获取游戏框架组件。
        /// </summary>
        /// <typeparam name="T">要获取的游戏框架组件类型。</typeparam>
        /// <returns>要获取的游戏框架组件。</returns>
        public static T GetFrameworkComponent<T>() where T : FrameworkComponent
        {
            return (T) GetFrameworkComponent(typeof(T));
        }

        /// <summary>
        /// 获取游戏框架组件。
        /// </summary>
        /// <param name="type">要获取的游戏框架组件类型。</param>
        /// <returns>要获取的游戏框架组件。</returns>
        public static FrameworkComponent GetFrameworkComponent(Type type)
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
        internal static void RegisterComponent(FrameworkComponent frameworkHotFixComponent)
        {
            if (frameworkHotFixComponent == null)
            {
                Debug.LogError("Wingjoy FrameworkMono component is invalid.");
                return;
            }

            Type type = frameworkHotFixComponent.GetType();


            if (FrameworkComponentsDic.ContainsKey(type))
            {
                Debug.LogErrorFormat("Wingjoy FrameworkMono component type '{0}' is already exist.", type.FullName);
                return;
            }
            else
            {
                FrameworkComponentsDic.Add(type, frameworkHotFixComponent);
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
            foreach (var frameworkComponent in FrameworkComponentsDic)
            {
                Debug.Log(frameworkComponent.Key);
                var uniTask = frameworkComponent.Value.Launcher();
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