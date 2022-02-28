using System;
using Wingjoy.Framework.Runtime.Localization;

namespace Wingjoy.Framework.Editor.Localization
{
    [Serializable]
    public class LanguageShorthand
    {
        public string Name;
        public Language Language;
        public string Code;

        /// <summary>
        ///   初始化 <see cref="T:System.Object" /> 类的新实例。
        /// </summary>
        public LanguageShorthand(string name, Language language, string code)
        {
            Name = name;
            Language = language;
            Code = code;
        }
    }
}
