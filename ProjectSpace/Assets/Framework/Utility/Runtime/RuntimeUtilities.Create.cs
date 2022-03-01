using System;

namespace Framework.Utility.Runtime
{
    public static partial class RuntimeUtilities
    {
        /// <summary>
        /// 通用扩展
        /// </summary>
        public static class Create
        {
            /// <summary>
            /// 创建实例
            /// </summary>
            /// <param name="type">实例类型</param>
            /// <returns>实例</returns>
            public static object CreateInstance(Type type)
            {
                if (type == typeof(string))
                {
                    return string.Empty;
                }
                else
                {
                    return Activator.CreateInstance(type);
                }
            }
        }
    }
}