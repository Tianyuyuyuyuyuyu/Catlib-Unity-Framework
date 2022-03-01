using System;

namespace Framework.Runtime.HotFix
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentConfigAttribute : Attribute
    {
        /// <summary>
        /// 配置类型
        /// </summary>
        private Type m_ConfigType;

        public Type ConfigType => m_ConfigType;

        public ComponentConfigAttribute(Type configType)
        {
            m_ConfigType = configType;
        }
    }
}