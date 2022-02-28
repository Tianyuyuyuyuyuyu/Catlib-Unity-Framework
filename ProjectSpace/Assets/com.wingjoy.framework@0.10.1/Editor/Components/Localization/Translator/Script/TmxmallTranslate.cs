#if UNITY_EDITOR
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Wingjoy.Framework.Runtime.Localization;
using WingjoyUtility.Runtime;
using Random = System.Random;

namespace Wingjoy.Framework.Editor.Localization
{
    public class TmxmallTranslate : Translator
    {
        [Serializable]
        public class TransResult
        {
            public string src;
            public string tgt;
            public string provider;

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
            public string error_code;
            public string error_msg;
            public string from;
            public string to;
            public string text;
            public List<TransResult> mt_set;

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
        public string ClientId;

        /// <summary>
        /// 密钥
        /// </summary>
        public string UserName;

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
                return "Tmxmall翻译";
            }
        }

        private static Translator s_Instance;

        public static Translator Instance
        {
            get
            {
                Type type = typeof(TmxmallTranslate);
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
        /// 随机数生成器
        /// </summary>
        private Random m_Random;

        // 当对象已启用并处于活动状态时调用此函数
        private void OnEnable()
        {
            if (SupportLanguages == null)
            {
                BuildSupportLanguage();
            }
            m_Random = new Random((int)DateTime.Now.Ticks);
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
                    EditorUtility.DisplayDialog("警告", "Tmxmall翻译不支持当前源语言", "确定");
                    return null;
                }

                if (string.IsNullOrEmpty(tCode))
                {
                    EditorUtility.DisplayDialog("警告", "Tmxmall翻译不支持当前目标语言", "确定");
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
                    string salt = m_Random.Next().ToString();
                    // Generate query url
                    string url = string.Format(
                        "http://api.tmxmall.com/v1/http/mttranslate?text={0}&user_name={1}&client_id={2}&from={3}&to={4}&de={5}",
                        WWW.EscapeURL(query), UserName, ClientId, sCode, tCode, "trados");

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
        }

        /// <summary>
        /// 翻译
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
                        string salt = m_Random.Next().ToString();
                        // Generate query url
                        string url = string.Format(
                            "http://api.tmxmall.com/v1/http/mttranslate?text={0}&user_name={1}&client_id={2}&from={3}&to={4}&de={5}",
                            UnityWebRequest.EscapeURL(query),UserName,ClientId,sCode,tCode, "trados");

                        // Make request
                        using (WebClient wc = new WebClient())
                        {
                            wc.Headers.Add("user-agent",
                                "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
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
            Debug.Log(Regex.Unescape(content));
            var jsonData = (ReturnResult)JsonUtility.FromJson(content, typeof(ReturnResult)); //(ReturnResult)JsonMapper.ToObject(content, typeof(ReturnResult));
            string dst = string.Empty;
            foreach (var result in jsonData.mt_set)
            {
                dst += result.tgt;
            }
            return dst;
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
                new LanguageShorthand("简体中文",Language.ChineseSimplified, "zh-CN"),
                new LanguageShorthand("繁体中文",Language.ChineseTraditional, "zh-HK"),
                new LanguageShorthand("英语",Language.English, "en-US"),
                new LanguageShorthand("日语",Language.Japanese, "ja-JP"),
                new LanguageShorthand("韩语",Language.Korean, "ko-KR"),
            };
        }
    }
}
#endif