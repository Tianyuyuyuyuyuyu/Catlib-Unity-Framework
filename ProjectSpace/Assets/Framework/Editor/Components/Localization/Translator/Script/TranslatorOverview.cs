#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Framework.Editor.Localization
{
    [GlobalConfig("FrameworkData/FrameworkMono/Localization")]
    public class TranslatorOverview:GlobalConfig<TranslatorOverview>
    {
        [FormerlySerializedAs("m_TranslatorWrappers")]
        [TableList(IsReadOnly = true, AlwaysExpanded = true), ShowInInspector]
        private List<TranslatorWrapper> m_TranslatorWrappers;

        [ShowInInspector,LabelText("默认翻译工具")]
        public string DefaultTranslatorName
        {
            get
            {
                return DefaultTranslator?.Name;
            }
        }

        /// <summary>
        /// 默认翻译工具
        /// </summary>
        [HideInInspector]
        public Translator DefaultTranslator;

        /// <summary>
        /// 搜索工程内所有翻译工具
        /// </summary>
        public void SearchAllTranslators()
        {
            List<Translator> translators = AssetDatabase.FindAssets("t:Translator")
                .Select(guid => AssetDatabase.LoadAssetAtPath<Translator>(AssetDatabase.GUIDToAssetPath(guid)))
                .ToList();

            m_TranslatorWrappers = translators.Select((translator => new TranslatorWrapper(translator))).ToList();

            var translatorWrapper = m_TranslatorWrappers.Find((wrapper => wrapper.Translator == DefaultTranslator));
            if (translatorWrapper != null)
            {
                translatorWrapper.IsDefault = true;
            }
        }

        /// <summary>
        /// 重置所有falg
        /// </summary>
        public void ResetAllDefaultFlag()
        {
            foreach (var translatorWrapper in m_TranslatorWrappers)
            {
                translatorWrapper.IsDefault = false;
            }
        }

        public class TranslatorWrapper
        {
            /// <summary>
            /// 翻译工具
            /// </summary>
            [HideInInspector]
            public readonly Translator Translator;

            /// <summary>
            /// 是否为默认翻译工具
            /// </summary>
            [HideInInspector]
            public bool IsDefault;

            [ShowInInspector]
            public string Name
            {
                get
                {
                    return Translator.Name;
                }
            }

            [ShowInInspector,ProgressBar(0, "MaxSupportLanguageCount")]
            public int SupportLanguageCount
            {
                get
                {
                    return Translator.SupportLanguageCount;
                }
            }

            /// <summary>
            ///   初始化 <see cref="T:System.Object" /> 类的新实例。
            /// </summary>
            public TranslatorWrapper(Translator translator)
            {
                Translator = translator;
                IsDefault = false;
            }

            /// <summary>
            /// 最大语言数
            /// </summary>
            /// <returns>数量</returns>
            public int MaxSupportLanguageCount()
            {
                return LocalizationSetting.Instance.LanguageCount;
            }

            /// <summary>
            /// 设为默认
            /// </summary>
            [Button("设为默认"),HideIf("IsDefault")]
            public void SetAsDefault()
            {
                Instance.ResetAllDefaultFlag();
                Instance.DefaultTranslator = Translator;
                IsDefault = true;
            }
        }
    }
}
#endif