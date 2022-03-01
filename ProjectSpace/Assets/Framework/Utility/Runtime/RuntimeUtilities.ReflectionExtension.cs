using System;
using System.Reflection;
using UnityEngine;

namespace Framework.Utility.Runtime
{
    public static partial class RuntimeUtilities
    {
        /// <summary>
        /// ��ILR�л�ȡAttrbute
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        static public T GetAttributeInILRuntime<T>(this MemberInfo memberInfo) where T : Attribute
        {
#if UNITY_EDITOR
            try
            {
#endif
                var attrs = memberInfo.GetCustomAttributes(false);
                foreach (var attr in attrs)
                {
                    if (attr is T)
                    {
                        return (attr as T);
                    }
                }
#if UNITY_EDITOR
            }
            catch (Exception e)
            {
                Debug.LogError("��ȡ[Attribute]ʧ��:" + memberInfo.Name + "\n ��ע���Attribute����������Ƿ񱨴�!");
            }
#endif
            return null;
        }
    }
}