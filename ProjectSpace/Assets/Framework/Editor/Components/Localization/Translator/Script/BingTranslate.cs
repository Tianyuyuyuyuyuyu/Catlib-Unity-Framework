#if UNITY_EDITOR
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Framework.Runtime.Localization;
using Framework.Utility.Runtime;

namespace Framework.Editor.Localization
{
    public class BingTranslate : Translator
    {
        [Serializable]
        public class TransResult
        {
            public string src;
            public string dst;

            /// <summary>
            ///   初始化 <see cref="T:System.Object" /> 类的新实例。
            /// </summary>
            public TransResult()
            {
            }
        }
        [Serializable]
        public class ReturnResult
        {
            public string from;
            public string to;
            public List<TransResult> trans_result;

            /// <summary>
            ///   初始化 <see cref="T:System.Object" /> 类的新实例。
            /// </summary>
            public ReturnResult()
            {
            }
        }

        /// <summary>
        /// AppID
        /// </summary>
        public string AppID;

        /// <summary>
        /// 密钥
        /// </summary>
        public string Key;

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
                return "必应翻译";
            }
        }

        private static Translator s_Instance;

        public static Translator Instance
        {
            get
            {
                Type type = typeof(BaiduTranslate);
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
                    // Generate query url
                    string url = string.Format("http://api.microsofttranslator.com/v2/Http.svc/Translate?appId={3}&from={1}&to={2}&text={0}",
                        WWW.EscapeURL(query), sCode, tCode, AppID);

                    // Make request
                    using (WebClient wc = new WebClient())
                    {
                        wc.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
                        wc.Headers.Add(HttpRequestHeader.AcceptCharset, "UTF-8");
                        wc.Encoding = System.Text.Encoding.UTF8;
                        var resolveTranslate = ResolveTranslate(wc.DownloadString(new Uri(url)));
                        //TranslationDatabase.Instance.Insert(query, targetLanguage, resolveTranslate);
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// 生成MD5码
        /// </summary>
        /// <param name="appid">应用ID</param>
        /// <param name="q">翻译内容</param>
        /// <param name="salt">随机数</param>
        /// <param name="key">密钥</param>
        /// <returns>加密结果</returns>
        private string BuildSign(string appid, string q, string salt, string key)
        {
            return RuntimeUtilities.MD5.Encrypt(appid + q + salt + key);
        }

        /// <summary>
        /// 处理获得的翻译结果
        /// </summary>
        /// <param name="content">回传内容</param>
        /// <returns>翻译结果</returns>
        private string ResolveTranslate(string content)
        {
            Debug.Log(content);
            var jsonData = (ReturnResult)JsonUtility.FromJson(content, typeof(ReturnResult));//(ReturnResult)JsonMapper.ToObject(content, typeof(ReturnResult));

            return jsonData.trans_result.First().dst;
        }

        /// <summary>
        /// 在支持的语言中搜索语言代码
        /// </summary>
        /// <param name="language">语言</param>
        /// <returns>语言代码</returns>SystemLanguage
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
                new LanguageShorthand("阿拉伯语",Language.Arabic, "ara"),
                new LanguageShorthand("保加利亚语",Language.Bulgarian, "bul"),
                new LanguageShorthand("简体中文",Language.ChineseSimplified, "zh"),
                new LanguageShorthand("繁体中文",Language.ChineseTraditional, "cht"),
                new LanguageShorthand("文言文",Language.Cantonese, "wyw"),
                new LanguageShorthand("粤语",Language.ClassicalChinese, "yue"),
                new LanguageShorthand("捷克语",Language.Czech, "cs"),
                new LanguageShorthand("丹麦语",Language.Danish, "dan"),
                new LanguageShorthand("荷兰语",Language.Dutch, "nl"),
                new LanguageShorthand("英语",Language.English, "en"),
                new LanguageShorthand("爱沙尼亚语",Language.Estonian, "est"),
                new LanguageShorthand("芬兰语",Language.Finnish, "fin"),
                new LanguageShorthand("法语",Language.French, "fra"),
                new LanguageShorthand("德语",Language.German, "de"),
                new LanguageShorthand("希腊语",Language.Greek, "el"),
                new LanguageShorthand("匈牙利语",Language.Hungarian, "hu"),
                new LanguageShorthand("意大利语",Language.Italian, "it"),
                new LanguageShorthand("日语",Language.Japanese, "jp"),
                new LanguageShorthand("韩语",Language.Korean, "kor"),
                new LanguageShorthand("波兰语",Language.Polish, "pl"),
                new LanguageShorthand("葡萄牙语",Language.PortuguesePortugal, "pt"),
                new LanguageShorthand("罗马尼亚",Language.Romanian, "rom"),
                new LanguageShorthand("俄语",Language.Russian, "ru"),
                new LanguageShorthand("斯洛文尼亚语",Language.Slovenian, "slo"),
                new LanguageShorthand("西班牙语",Language.Spanish, "spa"),
                new LanguageShorthand("瑞典语",Language.Swedish, "swe"),
                new LanguageShorthand("泰语",Language.Thai, "th"),
                new LanguageShorthand("越南语",Language.Vietnamese, "vie"),
            };
        }
    }
}
#endif