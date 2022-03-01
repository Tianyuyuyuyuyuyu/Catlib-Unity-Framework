using System.Collections.Generic;
using Framework.Runtime.Localization;

namespace Framework.Editor.Localization
{
    public class TranslationResultXML
    {
        /// <summary>
        /// 原文
        /// </summary>
        public string Original;

        /// <summary>
        /// 翻译结果
        /// </summary>
        public Dictionary<Language, string> Result;

        /// <summary>
        ///   初始化 <see cref="T:System.Object" /> 类的新实例。
        /// </summary>
        public TranslationResultXML(string original)
        {
            Original = original;
            Result = new Dictionary<Language, string>();
        }

        /// <summary>
        /// 追加
        /// </summary>
        /// <param name="language">语言</param>
        /// <param name="result">翻译结果</param>
        public void Append(Language language, string result)
        {
            if (Result.ContainsKey(language))
            {
                Result[language] = result;
            }
            else
            {
                Result.Add(language,result);
            }
        }
    }
}