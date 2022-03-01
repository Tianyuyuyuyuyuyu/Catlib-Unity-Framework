using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Runtime.Base
{
    public class ComponentConfig : MonoBehaviour
    {
        /// <summary>
        /// 存放所有的组件
        /// </summary>
        private static readonly Dictionary<string, ComponentConfig> ComponentConfigDic = new Dictionary<string, ComponentConfig>();

        public void Awake()
        {
            RegisterComponent(this);
        }

        /// <summary>
        /// 获取游戏框架组件。
        /// </summary>
        /// <returns>要获取的游戏框架组件。</returns>
        public static T GetComponentConfig<T>() where T : ComponentConfig
        {
            if (ComponentConfigDic.TryGetValue(typeof(T).ToString(), out var component))
            {
                return component as T;
            }

            return null;
        }

        /// <summary>
        /// 获取游戏框架组件。
        /// </summary>
        /// <param name="type">要获取的游戏框架组件类型。</param>
        /// <returns>要获取的游戏框架组件。</returns>
        public static ComponentConfig GetComponentConfig(string type)
        {
            if (ComponentConfigDic.TryGetValue(type, out var component))
            {
                return component;
            }

            return null;
        }

        /// <summary>
        /// 注册游戏框架组件。
        /// </summary>
        /// <param name="FrameworkHotFixComponent">要注册的游戏框架组件。</param>
        internal static void RegisterComponent(ComponentConfig FrameworkHotFixComponent)
        {
            if (FrameworkHotFixComponent == null)
            {
                Debug.LogError("Component Config is invalid.");
                return;
            }

            Type type = FrameworkHotFixComponent.GetType();


            var key = type.ToString();
            if (ComponentConfigDic.ContainsKey(key))
            {
                Debug.LogErrorFormat("Component Config type '{0}' is already exist.", type.FullName);
                return;
            }
            else
            {
                ComponentConfigDic.Add(key, FrameworkHotFixComponent);
            }
        }
    }
}