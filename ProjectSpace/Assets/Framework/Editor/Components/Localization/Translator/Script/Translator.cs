#if UNITY_EDITOR
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Framework.Runtime.Localization;

namespace Framework.Editor.Localization
{
    public abstract class Translator : ScriptableObject
    {
        /// <summary>
        /// 翻译工具名称
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 支持的语言数
        /// </summary>
        public abstract int SupportLanguageCount { get; }

        /// <summary>
        /// 等待时间间隔
        /// </summary>
        public float WaitInterval;

        /// <summary>
        /// 翻译
        /// </summary>
        /// <param name="sourceLanguage">源语言</param>
        /// <param name="targetLanguage">目标语言</param>
        /// <param name="query">翻译查询文本内容</param>
        /// <returns>翻译结果</returns>
        public abstract string Translate(Language sourceLanguage, Language targetLanguage, string query);

        /// <summary>
        /// 异步翻译
        /// </summary>
        /// <param name="sourceLanguage">源语言</param>
        /// <param name="targetLanguage">目标语言</param>
        /// <param name="query">翻译查询文本内容</param>
        /// <returns>翻译结果</returns>
        public abstract Task<string> TranslateAsync(Language sourceLanguage, Language targetLanguage, string query);

        /// <summary>
        /// HTML转义
        /// </summary>
        /// <param name="meaning">原文</param>
        /// <returns>结果</returns>
        public static string UnTransferred(string meaning)
        {
            //转义字符变换成普通字符 
            meaning = meaning.Replace("&lt;", "<");
            meaning = meaning.Replace("&gt;", ">");
            meaning = meaning.Replace("&apos;", "'");
            meaning = meaning.Replace("&quot;", "\"");
            meaning = meaning.Replace("&amp;", "&");

            return meaning;
        }

        public static ScriptableObject Create(Type type)
        {
            var s_Instance = CreateInstance(type);
            var path = "Assets/FrameworkData/FrameworkMono/Localization/Translator/" + type.Name + ".asset";
            var directoryName = Path.GetDirectoryName(path);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            AssetDatabase.CreateAsset(s_Instance, path);
            AssetDatabase.Refresh();
            return s_Instance;
        }
    }
}
#endif