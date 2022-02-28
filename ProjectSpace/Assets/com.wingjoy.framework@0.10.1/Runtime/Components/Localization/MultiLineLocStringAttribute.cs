using System;

namespace Wingjoy.Framework.Runtime.Localization
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public class MultiLineLocStringAttribute : Attribute
    {
        /// <summary>
        /// The number of lines for the text box.
        /// </summary>
        public int Lines;

        /// <summary>
        /// Makes a multiline textbox for editing strings.
        /// </summary>
        /// <param name="lines">The number of lines for the text box.</param>
        public MultiLineLocStringAttribute(int lines = 3)
        {
            this.Lines = Math.Max(1, lines);
        }
    }
}