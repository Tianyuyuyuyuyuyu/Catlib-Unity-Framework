using UnityEngine;

namespace Framework.Utility.Runtime
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        protected static T mInstance = null;

        public static T Instance()
        {
            if (mInstance == null)
            {
                mInstance = FindObjectOfType<T>();

                if (FindObjectsOfType<T>().Length > 1)
                {
                    Debug.LogError(typeof(T).Name + "More than 1!");
                    return mInstance;
                }

                if (mInstance == null)
                {
                    if (Application.isPlaying)
                    {
                        string instanceName = typeof(T).Name;
                        GameObject instanceGO = GameObject.Find(instanceName);

                        if (instanceGO == null)
                            instanceGO = new GameObject(instanceName);

                        mInstance = instanceGO.AddComponent<T>();
                        DontDestroyOnLoad(instanceGO);  //保证实例不会被释放
                    }
                }
            }

            return mInstance;
        }

        /// <summary>
        /// 是否有实例
        /// </summary>
        /// <returns>是否</returns>
        public static bool HasInstance()
        {
            if (mInstance == null)
            {
                mInstance = FindObjectOfType<T>();
            }
            return mInstance != null;
        }

        protected virtual void OnDestroy()
        {
            mInstance = null;
        }
    }
}