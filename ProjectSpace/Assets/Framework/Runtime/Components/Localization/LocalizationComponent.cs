using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using Framework.Runtime.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Framework.Runtime.Definition;
using Framework.Runtime.Setting;
using Framework.Utility.Runtime;

namespace Framework.Runtime.Localization
{
    [DisallowMultipleComponent]
    public class LocalizationComponent : FrameworkComponent
    {
        /// <summary>
        /// 本地化字典
        /// </summary>
        private Dictionary<string, string> m_Dictionary;

        /// <summary>
        /// 当前语言
        /// </summary>
        private Language m_Language;

        /// <summary>
        /// 变更语言流程
        /// </summary>
        private List<UniTask> m_ChangeLanguageTask;

        /// <summary>
        /// 语种变更回调
        /// </summary>
        private event Action<Language> OnLanguageChanged;

        /// <summary>
        /// 所有本地化组件
        /// </summary>
        private List<ILocalizedCom> m_LocalizedComList;

        /// <summary>
        /// 是否使用系统语言作为第一次启动时语言
        /// </summary>
        [LabelWidth(250)]
        public bool UseSystemLanguageForFirstLaunch;

        /// <summary>
        /// 默认支持语言
        /// </summary>
        public Language DefaultSupportLanguage;

        /// <summary>
        /// 支持的语言
        /// </summary>
        public List<Language> SupportLanguages;

        /// <summary>
        /// 获取系统语言。
        /// </summary>
        public Language SystemLanguage
        {
            get
            {
                switch (Application.systemLanguage)
                {
                    case UnityEngine.SystemLanguage.Afrikaans: return Language.Afrikaans;
                    case UnityEngine.SystemLanguage.Arabic: return Language.Arabic;
                    case UnityEngine.SystemLanguage.Basque: return Language.Basque;
                    case UnityEngine.SystemLanguage.Belarusian: return Language.Belarusian;
                    case UnityEngine.SystemLanguage.Bulgarian: return Language.Bulgarian;
                    case UnityEngine.SystemLanguage.Catalan: return Language.Catalan;
                    case UnityEngine.SystemLanguage.Chinese: return Language.ChineseSimplified;
                    case UnityEngine.SystemLanguage.ChineseSimplified: return Language.ChineseSimplified;
                    case UnityEngine.SystemLanguage.ChineseTraditional: return Language.ChineseTraditional;
                    case UnityEngine.SystemLanguage.Czech: return Language.Czech;
                    case UnityEngine.SystemLanguage.Danish: return Language.Danish;
                    case UnityEngine.SystemLanguage.Dutch: return Language.Dutch;
                    case UnityEngine.SystemLanguage.English: return Language.English;
                    case UnityEngine.SystemLanguage.Estonian: return Language.Estonian;
                    case UnityEngine.SystemLanguage.Faroese: return Language.Faroese;
                    case UnityEngine.SystemLanguage.Finnish: return Language.Finnish;
                    case UnityEngine.SystemLanguage.French: return Language.French;
                    case UnityEngine.SystemLanguage.German: return Language.German;
                    case UnityEngine.SystemLanguage.Greek: return Language.Greek;
                    case UnityEngine.SystemLanguage.Hebrew: return Language.Hebrew;
                    case UnityEngine.SystemLanguage.Hungarian: return Language.Hungarian;
                    case UnityEngine.SystemLanguage.Icelandic: return Language.Icelandic;
                    case UnityEngine.SystemLanguage.Indonesian: return Language.Indonesian;
                    case UnityEngine.SystemLanguage.Italian: return Language.Italian;
                    case UnityEngine.SystemLanguage.Japanese: return Language.Japanese;
                    case UnityEngine.SystemLanguage.Korean: return Language.Korean;
                    case UnityEngine.SystemLanguage.Latvian: return Language.Latvian;
                    case UnityEngine.SystemLanguage.Lithuanian: return Language.Lithuanian;
                    case UnityEngine.SystemLanguage.Norwegian: return Language.Norwegian;
                    case UnityEngine.SystemLanguage.Polish: return Language.Polish;
                    case UnityEngine.SystemLanguage.Portuguese: return Language.PortuguesePortugal;
                    case UnityEngine.SystemLanguage.Romanian: return Language.Romanian;
                    case UnityEngine.SystemLanguage.Russian: return Language.Russian;
                    case UnityEngine.SystemLanguage.SerboCroatian: return Language.SerboCroatian;
                    case UnityEngine.SystemLanguage.Slovak: return Language.Slovak;
                    case UnityEngine.SystemLanguage.Slovenian: return Language.Slovenian;
                    case UnityEngine.SystemLanguage.Spanish: return Language.Spanish;
                    case UnityEngine.SystemLanguage.Swedish: return Language.Swedish;
                    case UnityEngine.SystemLanguage.Thai: return Language.Thai;
                    case UnityEngine.SystemLanguage.Turkish: return Language.Turkish;
                    case UnityEngine.SystemLanguage.Ukrainian: return Language.Ukrainian;
                    case UnityEngine.SystemLanguage.Unknown: return Language.Unspecified;
                    case UnityEngine.SystemLanguage.Vietnamese: return Language.Vietnamese;
                    default: return Language.Unspecified;
                }
            }
        }

        /// <summary>
        /// 获取或设置本地化语言。
        /// </summary>
        [ShowInInspector,ReadOnly]
        public Language Language
        {
            get { return m_Language; }
            set
            {
                if (value == Language.Unspecified)
                {
                    Debug.LogError("Language is invalid.");
                }

                m_Language = value;
            }
        }

        /// <summary>
        /// 是否开发模式
        /// </summary>
        public bool Development;


        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            m_ChangeLanguageTask = new List<UniTask>();
            m_LocalizedComList = new List<ILocalizedCom>();
            m_Dictionary = new Dictionary<string, string>();
        }

        public override async UniTask Launcher()
        {
            if (CoreMain.Setting != null)
            {
                string readLanguageString = CoreMain.Setting.GetString(Constant.Setting.Language, Language.Unspecified.ToString());
                Enum.TryParse(readLanguageString, out Language language);
                if (language == Language.Unspecified)
                {
                    Language systemLanguage = SystemLanguage;
                    //如果选择的语言不在支持范围，则使用默认支持语言
                    if (SupportLanguages.Contains(systemLanguage))
                    {
                        if (!UseSystemLanguageForFirstLaunch)
                        {
                            systemLanguage = DefaultSupportLanguage;
                        }
                    }
                    else
                    {
                        systemLanguage = DefaultSupportLanguage;
                    }

                    language = systemLanguage;
                }

                Language = language;

                CoreMain.Setting.SetString(Constant.Setting.Language, language.ToString());

                Debug.LogFormat("Init language settings complete, current language is '{0}'.", Language.ToString());

                RegisterOnLanguageChangedCallback((l =>
                {
                    //Message.SendUIMessage("ChangeLanguage", l);
                }));

                await LoadLanguageDictionary();
            }
        }

        /// <summary>
        /// 增加字典。
        /// </summary>
        /// <param name="key">字典主键。</param>
        /// <param name="value">字典内容。</param>
        /// <returns>是否增加字典成功。</returns>
        public bool AddRawString(string key, string value)
        {
            if (HasRawString(key))
            {
                return false;
            }

            if (value.Contains("\\"))
            {
                value = Regex.Unescape(value);
            }

            m_Dictionary.Add(key, value ?? string.Empty);
            return true;
        }

        /// <summary>
        /// 是否存在字典。
        /// </summary>
        /// <param name="key">字典主键。</param>
        /// <returns>是否存在字典。</returns>
        public bool HasRawString(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("Key is invalid.");
                return true;
            }

            return m_Dictionary.ContainsKey(key);
        }

        /// <summary>
        /// 解析字典。
        /// </summary>
        /// <param name="text">要解析的字典文本。</param>
        /// <returns>是否解析字典成功。</returns>
        public bool ParseDictionary(string text)
        {
            return XmlLocalizationHelper.ParseDictionary(text);
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串。
        /// </summary>
        /// <param name="key">字典主键。</param>
        /// <returns>要获取的字典内容字符串。</returns>
        public string GS(string key)
        {
            if (!m_Dictionary.TryGetValue(key, out var value))
            {
                var format = RuntimeUtilities.Text.Format("<NoKey>{0}", key);
                Debug.LogWarning(format);
                return format;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串。
        /// </summary>
        /// <param name="formatKey">格式键值</param>
        /// <param name="args">参数</param>
        /// <returns>要获取的字典内容字符串。</returns>
        public string GS(string formatKey, params object[] args)
        {
            if (!m_Dictionary.TryGetValue(formatKey, out var value))
            {
                var format = RuntimeUtilities.Text.Format("<NoKey>{0}", formatKey);
                Debug.LogWarning(format);
                return format;
            }
            else
            {
                return RuntimeUtilities.Text.Format(value, args);
            }
        }

        /// <summary>
        /// 修改语言
        /// </summary>
        /// <param name="language">语言</param>
        /// <param name="onFinish">结束回调</param>
        public async void ChangeLanguage(Language language, Action onFinish)
        {
            if (m_Language == language)
            {
                Debug.LogWarning("当前已是该语言");
                onFinish?.Invoke();
                return;
            }

            if (SupportLanguages.Contains(language))
            {
                m_Language = language;
                
                GetFrameworkComponent<SettingComponent>().SetString("Setting.Language", language.ToString());
                
                await LoadLanguageDictionary();
                
                for (var index = m_LocalizedComList.Count - 1; index >= 0; index--)
                {
                    var localizedCom = m_LocalizedComList[index];
                    if(localizedCom == null)
                    {
                        m_LocalizedComList.RemoveAt(index);
                    }
                    else
                    {
                        localizedCom.DoLocalize();
                    }
                }


                foreach (var task in m_ChangeLanguageTask)
                {
                    await task;
                }

                OnLanguageChanged?.Invoke(language);
            }
            else
            {
                Debug.LogError("当前并不支持该语言，若要支持，将语言加入SupportLanguages List中");
            }

            onFinish?.Invoke();
        }

        /// <summary>
        /// 获取加载当前语言本地化关键字列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetLoadLanguageDictionaryKeys()
        {
            List<string> keys = new List<string>();
            keys.Add(Language.ToString());
            keys.Add("LocalizationXml");
            return keys;
        }
        
        /// <summary>
        /// 加载字典
        /// </summary>
        public async UniTask LoadLanguageDictionary()
        {
            m_Dictionary.Clear();
            var keys = GetLoadLanguageDictionaryKeys();
            var asyncOperationHandle = Addressables.LoadAssetsAsync<TextAsset>(keys, null, Addressables.MergeMode.Intersection);
            await asyncOperationHandle;
            if (asyncOperationHandle.Result != null)
            {
                foreach (var textAsset in asyncOperationHandle.Result)
                {
                    switch (textAsset.name)
                    {
                        case "LocalizationSceneDictionary":
                            ParseDictionary(textAsset.text);
                            break;
                        case "LocalizationScriptDictionary":
                            ParseDictionary(textAsset.text);
                            break;
                    }
                }    
            }
            else
            {
                Debug.LogError("标记");
            }
            
            Addressables.Release(asyncOperationHandle);
        }

        /// <summary>
        /// 注册语种变更回调
        /// </summary>
        /// <param name="callback">回调</param>
        public void RegisterOnLanguageChangedCallback(Action<Language> callback)
        {
            OnLanguageChanged += callback;
        }

        /// <summary>
        /// 注册本地化组件
        /// </summary>
        /// <param name="localizedCom">本地化组件</param>
        public void RegisterLocalizedCom(ILocalizedCom localizedCom)
        {
            m_LocalizedComList.Add(localizedCom);
        }

        /// <summary>
        /// 移除本地化组件
        /// </summary>
        /// <param name="localizedCom">本地化组件</param>
        public void RemoveLocalizedCom(ILocalizedCom localizedCom)
        {
            m_LocalizedComList.Remove(localizedCom);
        }

        /// <summary>
        /// 注册变更语言工作流
        /// </summary>
        /// <param name="action">工作流</param>
        public void RegisterChangeLanguageFlowTask(UniTask action)
        {
            m_ChangeLanguageTask.Add(action);
        }

        /// <summary>
        /// 获取语言文本
        /// </summary>
        /// <param name="language">语言</param>
        /// <returns>文本</returns>
        public static string GetLanguageLabel(Language language)
        {
            switch (language)
            {
                case Language.Unspecified:
                    return "Unspecified";
                case Language.ChineseSimplified:
                    return "简体中文";
                case Language.ChineseTraditional:
                    return "繁體中文";
                case Language.English:
                    return "English";
                default:
                    throw new ArgumentOutOfRangeException(nameof(language), language, null);
            }
        }
    }
}