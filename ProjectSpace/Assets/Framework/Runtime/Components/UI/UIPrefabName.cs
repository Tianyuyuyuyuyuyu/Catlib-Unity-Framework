using System;

namespace Framework.Runtime.UI
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UIPrefabName : Attribute
    {
        /// <summary>
        /// 预制件名称
        /// </summary>
        public string Name;

        public UIPrefabName()
        {
        }

        public UIPrefabName(string name)
        {
            Name = name;
        }
    }
}