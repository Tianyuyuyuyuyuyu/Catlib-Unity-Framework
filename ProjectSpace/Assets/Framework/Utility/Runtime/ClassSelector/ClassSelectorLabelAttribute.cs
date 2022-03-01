using System;

namespace Framework.Utility.Runtime
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ClassSelectorLabelAttribute: Attribute
    {
        /// <summary>
        /// 文本
        /// </summary>
        private string m_Label;
        public string Label => m_Label;

        public ClassSelectorLabelAttribute(string label)
        {
            m_Label = label;
        }
    }
}