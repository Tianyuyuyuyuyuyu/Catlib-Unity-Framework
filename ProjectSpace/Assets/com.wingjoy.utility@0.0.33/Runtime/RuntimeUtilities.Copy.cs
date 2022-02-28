using System;
using System.Reflection;

namespace WingjoyUtility.Runtime
{
    public static partial class RuntimeUtilities
    {
        public static class Copy
        {
            public static T DeepCopyByReflect<T>(T obj)
            {
                //如果是字符串或值类型则直接返回
                if (obj is string || obj.GetType().IsValueType) return obj;

                object retval = Activator.CreateInstance(obj.GetType());
                FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (FieldInfo field in fields)
                {
                    try { field.SetValue(retval, DeepCopyByReflect(field.GetValue(obj))); }
                    catch { }
                }
                return (T)retval;
            }
        }
    }
}
