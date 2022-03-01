using System;

namespace Framework.Utility.Runtime
{
    [Serializable]
    public abstract class Singleton<T> : ISingleton where T : Singleton<T>
    {
        protected static T mInstance;

        protected Singleton() { }

        public static T Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = SingletonCreator.CreateSingleton<T>();
                }
                return mInstance;
            }
        }

        public virtual void Dispose()
        {
            mInstance = null;
        }

        public virtual void OnSingletonInit() { }

        /// <summary>
        /// 是否有实例
        /// </summary>
        /// <returns>是否</returns>
        public static bool HasInstance()
        {
            return mInstance != null;
        }
    }
}