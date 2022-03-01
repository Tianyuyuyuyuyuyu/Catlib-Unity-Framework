using System;
using System.Reflection;

namespace Framework.Utility.Runtime
{
    public static partial class RuntimeUtilities
    {
        public static class Copy
        {
            /// <summary>
            /// 深拷贝(反射)
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="obj"></param>
            /// <returns></returns>
            public static T DeepCopyByReflect<T>(T obj)
            {
                //如果是字符串或值类型则直接返回
                if (obj is string || obj.GetType().IsValueType) return obj;

                var retVal = Activator.CreateInstance(obj.GetType());
                var fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var field in fields)
                {
                    try
                    {
                        field.SetValue(retVal, DeepCopyByReflect(field.GetValue(obj)));
                    }
                    catch
                    {
                        // ignored
                    }
                }
                return (T)retVal;
            }
        }
    }
}
