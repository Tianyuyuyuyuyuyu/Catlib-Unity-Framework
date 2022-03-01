using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Utility.Runtime
{
    public static partial class RuntimeUtilities
    {
        public class Coroutine : MonoSingleton<Coroutine>
        {
            /// <summary>
            /// 存放协程的字典
            /// </summary>
            public static Dictionary<string, MonoBehaviour> Dictionary = new Dictionary<string, MonoBehaviour>();

            /// <summary>
            /// 执行协程
            /// </summary>
            /// <param name="enumerator">协程内容</param>
            /// <returns>协程</returns>
            public static UnityEngine.Coroutine Run(IEnumerator enumerator)
            {
                return Run(null, enumerator);
            }

            /// <summary>
            /// 执行协程
            /// </summary>
            /// <param name="key">协程键值</param>
            /// <param name="enumerator">协程内容</param>
            /// <returns>协程</returns>
            public static UnityEngine.Coroutine Run(string key, IEnumerator enumerator)
            {
                if (string.IsNullOrEmpty(key))
                {
                    return Instance().StartCoroutine(enumerator);
                }

                if (Dictionary.TryGetValue(key, out var monoBehaviour))
                {
                    return monoBehaviour.StartCoroutine(enumerator);
                }
                else
                {
                    GameObject o = new GameObject(key);
                    o.transform.SetParent(Instance().transform);
                    var addComponent = o.AddComponent<CoroutineHelper>();
                    Dictionary.Add(key, addComponent);
                    return addComponent.StartCoroutine(enumerator);
                }
            }

            /// <summary>
            /// 停用所有协程
            /// </summary>
            public static void StopAll()
            {
                StopAll(null);
            }

            /// <summary>
            /// 停用所有协程
            /// </summary>
            /// <param name="key">协程键值</param>
            public static void StopAll(string key)
            {
                if (string.IsNullOrEmpty(key))
                {
                    Instance().StopAllCoroutines();
                    return;
                }

                if (Dictionary.TryGetValue(key, out var monoBehaviour))
                {
                    monoBehaviour.StopAllCoroutines();
                }
            }

            /// <summary>
            /// 停用所有协程
            /// </summary>
            public static void StopAllCo()
            {
                Instance().StopAllCoroutines();
                foreach (var keyValuePair in Dictionary)
                {
                    keyValuePair.Value.StopAllCoroutines();
                }
            }

            /// <summary>
            /// 下一帧执行
            /// </summary>
            /// <param name="action">执行函数</param>
            public static IEnumerator NextFrameMethod(Action action)
            {
                return DelayFrameMethod(1, action);
            }

            /// <summary>
            /// 延迟指定帧执行
            /// </summary>
            /// <param name="waitFrame">指定帧数</param>
            /// <param name="action">执行函数</param>
            public static IEnumerator DelayFrameMethod(int waitFrame, Action action)
            {
                for (int i = 0; i < waitFrame; i++)
                {
                    yield return null;
                }

                action();
            }

            /// <summary>
            /// 延迟指定帧执行
            /// </summary>
            /// <param name="waitSecond">指定帧数</param>
            /// <param name="action">执行函数</param>
            public static IEnumerator DelaySecondMethod(int waitSecond, Action action)
            {
                for (int i = 0; i < waitSecond; i++)
                {
                    yield return WaitFor.Second_1;
                }

                action();
            }

            /// <summary>
            /// 延迟指定帧执行
            /// </summary>
            /// <param name="waitSecond">指定帧数</param>
            /// <param name="action">执行函数</param>
            public static IEnumerator DelaySecondMethod(float waitSecond, Action action)
            {
                yield return new WaitForSeconds(waitSecond);
                action();
            }
        }
    }

    public static class CoroutineExtension
    {
        public static Coroutine Start(this IEnumerator enumerator)
        {
            return RuntimeUtilities.Coroutine.Run(enumerator);
        }

        public static Coroutine Start(this IEnumerator enumerator, string key)
        {
            return RuntimeUtilities.Coroutine.Run(key, enumerator);
        }
    }
}