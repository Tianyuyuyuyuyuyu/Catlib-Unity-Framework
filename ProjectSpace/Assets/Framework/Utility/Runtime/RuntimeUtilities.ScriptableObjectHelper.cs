using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Framework.Utility.Runtime
{
    public static partial class RuntimeUtilities
    {
        /// <summary>
        /// 字符相关的实用函数。
        /// </summary>
        public static class ScriptableObjectHelper
        {
            public static T AddScriptableObject<T>(Object assetObj) where T : UnityEngine.ScriptableObject
            {
#if UNITY_EDITOR
                var instance = UnityEngine.ScriptableObject.CreateInstance<T>();
                UnityEditor.AssetDatabase.AddObjectToAsset(instance, assetObj);
                return instance;
#else
            return null;
#endif
            }

            public static object AddScriptableObject(Object assetObj, Type type)
            {
#if UNITY_EDITOR
                var instance = UnityEngine.ScriptableObject.CreateInstance(type);
                UnityEditor.AssetDatabase.AddObjectToAsset(instance, assetObj);
                return instance;
#else
            return null;
#endif
            }
        }
    }
}
