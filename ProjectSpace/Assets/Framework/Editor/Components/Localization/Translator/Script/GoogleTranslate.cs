using System.Text;
using Framework.Runtime.Localization;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LitJson;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Framework.Editor.Localization
{
    public class GoogleTranslate: Translator
    {
        [Serializable]
        public class Translations
        {
            /// <summary>
            /// 
            /// </summary>
            public string translatedText { get; set; }
        }

        [Serializable]
        public class Data
        {
            /// <summary>
            /// 
            /// </summary>
            public List<Translations> translations { get; set; }
        }

        [Serializable]
        public class ReturnResult
        {
            /// <summary>
            /// 
            /// </summary>
            public Data data { get; set; }
        }


        /// <summary>
        /// 翻译工具支持的语言
        /// </summary>
        [ReadOnly]
        public List<LanguageShorthand> SupportLanguages;

        /// <summary>
        /// 支持的语言数
        /// </summary>
        public override int SupportLanguageCount
        {
            get
            {
                return SupportLanguages.Count;
            } 
        }

        /// <summary>
        /// 翻译工具名称
        /// </summary>
        public override string Name
        {
            get
            {
                return "谷歌翻译";
            }
        }

        private static Translator s_Instance;

        public static Translator Instance
        {
            get
            {
                Type type = typeof(GoogleTranslate);
                if (s_Instance == null)
                {
                    var findAssets = AssetDatabase.FindAssets("t:" + type.FullName);
                    if (findAssets.Length > 0)
                    {
                        s_Instance = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(findAssets.First()), type) as Translator;
                    }

                    if (s_Instance == null)
                    {
                        s_Instance = Create(type) as Translator;
                    }
                }
                return s_Instance;
            }
        }


        /// <summary>
        /// Api密钥
        /// </summary>
        public string ApiKey;

        // 当对象已启用并处于活动状态时调用此函数
        private void OnEnable()
        {
            if (SupportLanguages == null)
            {
                BuildSupportLanguage();
            }
        }

        /// <summary>
        /// 翻译
        /// </summary>
        /// <param name="sourceLanguage">源语言</param>
        /// <param name="targetLanguage">目标语言</param>
        /// <param name="query">翻译查询文本内容</param>
        /// <returns>翻译结果</returns>
        public override string Translate(Language sourceLanguage, Language targetLanguage, string query)
        {
            try
            {
                string sCode = FindLanguageCode(sourceLanguage);
                string tCode = FindLanguageCode(targetLanguage);

                if (string.IsNullOrEmpty(sCode))
                {
                    EditorUtility.DisplayDialog("警告", "谷歌翻译不支持当前源语言","确定");
                    return null;
                }

                if (string.IsNullOrEmpty(tCode))
                {
                    EditorUtility.DisplayDialog("警告", "谷歌翻译不支持当前目标语言", "确定");
                    return null;
                }

                if (tCode == "auto")
                {
                    EditorUtility.DisplayDialog("警告", "目标语言不能使用auto", "确定");
                    return null;
                }

                if (sCode == tCode)
                {
                    //TranslationDatabase.Instance.Insert(query, targetLanguage, query);
                    return query;
                }
                else
                {
                    string value = string.Empty;
                    //切分换行
                    var splits = query.Split('\n');
                    for (var index = 0; index < splits.Length; index++)
                    {
                        var s = splits[index];
                        if (string.IsNullOrEmpty(s))
                        {
                        }
                        else
                        {
                            var escapeUrl = UnityWebRequest.EscapeURL(s, Encoding.UTF8);
                            string url = string.Format("https://translation.googleapis.com/language/translate/v2?target={0}&source={1}&key={2}&q={3}",
                                tCode, sCode, ApiKey, escapeUrl);

                            // Make request
                            using (WebClient wc = new WebClient())
                            {
                                wc.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
                                wc.Headers.Add(HttpRequestHeader.AcceptCharset, "UTF-8");
                                wc.Encoding = System.Text.Encoding.UTF8;
                                value += UnTransferred(ResolveTranslate(wc.DownloadString(new Uri(url))));
                            }
                        }

                        if (index + 1 < splits.Length)
                        {
                            value += "\n";
                        }
                    }

                    TranslationDatabase.Instance.Insert(query, targetLanguage, value);
                    return value;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            Debug.LogError("翻译出错");
            return null;
        }

        /// <summary>
        /// 异步翻译
        /// </summary>
        /// <param name="sourceLanguage">源语言</param>
        /// <param name="targetLanguage">目标语言</param>
        /// <param name="query">翻译查询文本内容</param>
        /// <returns>翻译结果</returns>
        public override Task<string> TranslateAsync(Language sourceLanguage, Language targetLanguage, string query)
        {
            var task = Task.Run((() =>
            {
                try
                {
                    string sCode = FindLanguageCode(sourceLanguage);
                    string tCode = FindLanguageCode(targetLanguage);

                    if (string.IsNullOrEmpty(sCode))
                    {
                        EditorUtility.DisplayDialog("警告", "百度翻译不支持当前源语言", "确定");
                        return null;
                    }

                    if (string.IsNullOrEmpty(tCode))
                    {
                        EditorUtility.DisplayDialog("警告", "百度翻译不支持当前目标语言", "确定");
                        return null;
                    }

                    if (tCode == "auto")
                    {
                        EditorUtility.DisplayDialog("警告", "目标语言不能使用auto", "确定");
                        return null;
                    }

                    if (sCode == tCode)
                    {
                        //TranslationDatabase.Instance.Insert(query, targetLanguage, query);
                        return query;
                    }
                    else
                    {
                        string url = string.Format("https://translation.googleapis.com/language/translate/v2?target={0}&source={1}&key={2}&q={3}",
                            tCode, sCode, ApiKey, WWW.EscapeURL(query));

                        // Make request
                        using (WebClient wc = new WebClient())
                        {
                            wc.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
                            wc.Headers.Add(HttpRequestHeader.AcceptCharset, "UTF-8");
                            wc.Encoding = System.Text.Encoding.UTF8;
                            var resolveTranslate = ResolveTranslate(wc.DownloadString(new Uri(url)));
                            TranslationDatabase.Instance.Insert(query, targetLanguage, resolveTranslate);
                            return resolveTranslate;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }

                Debug.LogError("翻译出错" + query);
                return null;
            }));

            return task;
        }

        /// <summary>
        /// 处理获得的翻译结果
        /// </summary>
        /// <param name="content">回传内容</param>
        /// <returns>翻译结果</returns>
        public string ResolveTranslate(string content)
        {
            Debug.Log(content);
            var jsonData = (ReturnResult)JsonMapper.ToObject(content,typeof(ReturnResult));
            string translatedText = string.Empty;
            foreach (var result in jsonData.data.translations)
            {
                translatedText += result.translatedText;
            }
            return translatedText;
        }

        /// <summary>
        /// 在支持的语言中搜索语言代码
        /// </summary>
        /// <param name="language">语言</param>
        /// <returns>语言代码</returns>
        public string FindLanguageCode(Language language)
        {
            return SupportLanguages.Find((l => l.Language == language))?.Code;
        }

        /// <summary>
        /// 建立支持语言库
        /// </summary>
        [Button(ButtonSizes.Large)]
        public void BuildSupportLanguage()
        {
            SupportLanguages = new List<LanguageShorthand>()
            {
                new LanguageShorthand("自动检测",Language.Unspecified, "auto"),
                new LanguageShorthand("南非荷兰语",Language.Afrikaans, "af"),
                //new LanguageShorthand("",Language.Albanian, "sq"),
                new LanguageShorthand("阿拉伯",Language.Arabic, "ar"),
                //new LanguageShorthand("",Language.Armenian, "hy"),
                //new LanguageShorthand("",Language.Azerbaijani, "az"),
                new LanguageShorthand("巴斯克",Language.Basque, "eu"),
                new LanguageShorthand("白俄罗斯",Language.Belarusian, "be"),
                //new LanguageShorthand("",Language.Bengali, "bn"),
                new LanguageShorthand("保加利亚语",Language.Bulgarian, "bg"),
                new LanguageShorthand("加泰罗尼亚",Language.Catalan, "ca"),
                new LanguageShorthand("简体中文",Language.ChineseSimplified, "zh-CN"),
                new LanguageShorthand("繁体中文",Language.ChineseTraditional, "zh-TW"),
                new LanguageShorthand("塞尔维亚 - 克罗地亚语",Language.SerboCroatian, "hr"),
                new LanguageShorthand("捷克",Language.Czech, "cs"),
                new LanguageShorthand("丹麦",Language.Danish, "da"),
                new LanguageShorthand("荷兰人",Language.Dutch, "nl"),
                new LanguageShorthand("英语",Language.English, "en"),
                //new LanguageShorthand("",Language.Esperanto, "eo"),
                new LanguageShorthand("爱沙尼亚语",Language.Estonian, "et"),
                //new LanguageShorthand("",Language.Filipino, "tl"),
                new LanguageShorthand("芬兰",Language.Finnish, "fi"),
                new LanguageShorthand("法国",Language.French, "fr"),
                //new LanguageShorthand("",Language.Galician, "gl"),
                new LanguageShorthand("德语",Language.German, "de"),
                //new LanguageShorthand("",Language.Georgian, "ka"),
                new LanguageShorthand("希腊语",Language.Greek, "el"),
                //new LanguageShorthand("",Language.Haitian Creole, "ht"),
                new LanguageShorthand("希伯来语",Language.Hebrew, "iw"),
                //new LanguageShorthand("",Language.Hindi, "hi"),
                new LanguageShorthand("匈牙利",Language.Hungarian, "hu"),
                new LanguageShorthand("冰岛的",Language.Icelandic, "is"),
                new LanguageShorthand("印度尼西亚",Language.Indonesian, "id"),
                //new LanguageShorthand("",Language.Irish, "ga"),
                new LanguageShorthand("意大利",Language.Italian, "it"),
                new LanguageShorthand("日本",Language.Japanese, "ja"),
                new LanguageShorthand("韩语",Language.Korean, "ko"),
                //new LanguageShorthand("",Language.Lao, "lo"),
                //new LanguageShorthand("",Language.Latin, "la"),
                new LanguageShorthand("拉脱维亚",Language.Latvian, "lv"),
                new LanguageShorthand("立陶宛",Language.Lithuanian, "lt"),
                //new LanguageShorthand("",Language.Macedonian, "mk"),
                //new LanguageShorthand("",Language.Malay, "ms"),
                //new LanguageShorthand("",Language.Maltese, "mt"),
                new LanguageShorthand("挪威",Language.Norwegian, "no"),
                //new LanguageShorthand("",Language.Persian, "fa"),
                new LanguageShorthand("波兰",Language.Polish, "pl"),
                new LanguageShorthand("葡萄牙语",Language.PortuguesePortugal, "pt"),
                new LanguageShorthand("罗马尼亚",Language.Romanian, "ro"),
                new LanguageShorthand("俄语",Language.Russian, "ru"),
                //new LanguageShorthand("",Language.Serbian, "sr"),
                new LanguageShorthand("斯洛伐克",Language.Slovak, "sk"),
                new LanguageShorthand("斯洛文尼亚",Language.Slovenian, "sl"),
                new LanguageShorthand("西班牙语",Language.Spanish, "es"),
                //new LanguageShorthand("",Language.Swahili, "sw"),
                new LanguageShorthand("瑞典",Language.Swedish, "sv"),
                //new LanguageShorthand("",Language.Tamil, "ta"),
                //new LanguageShorthand("",Language.Telugu, "te"),
                new LanguageShorthand("泰语",Language.Thai, "th"),
                new LanguageShorthand("土耳其",Language.Turkish, "tr"),
                new LanguageShorthand("乌克兰",Language.Ukrainian, "uk"),
                //new LanguageShorthand("",Language.Urdu, "ur"),
                new LanguageShorthand("越南",Language.Vietnamese, "vi"),
                //new LanguageShorthand("",Language.Welsh, "cy"),
                //new LanguageShorthand("",Language.Yiddish, "yi"),
            };
        }
    }
}
#endif