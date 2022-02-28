using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace Wingjoy.Framework.Runtime.Localization
{
    public class LocalizedText : Text, ILocalizedCom
    {
        /// <summary>
        /// 是否使能
        /// </summary>
        public bool EnableLocalization => EnableText;
        /// <summary>
        /// 本地化使能
        /// </summary>
        [SerializeField]
        public bool EnableText = true;
        /// <summary>
        /// 本地化键值
        /// </summary>
        [SerializeField]
        public string LocalizationKey;
        /// <summary>
        /// 选中要显示的语言
        /// </summary>
        [SerializeField]
        private Language m_SelectDisplayLanguage;
        /// <summary>
        /// 原始数据
        /// </summary>
        private TextData m_OriginalTextData;
        /// <summary>
        /// 文本本地化数据
        /// </summary>
        [SerializeField]
        public List<TextData> LocalizationData = new List<TextData>();

        protected override void Awake()
        {
            m_OriginalTextData = new TextData();
            m_OriginalTextData.EnableFontSize = true;
            m_OriginalTextData.FontSize = fontSize;
            if (Application.isPlaying)
            {
                WingjoyFrameworkComponent.GetFrameworkComponent<LocalizationComponent>().RegisterLocalizedCom(this);
                DoLocalize();
            }
        }

        protected override void OnDestroy()
        {
            if (Application.isPlaying)
            {
                WingjoyFrameworkComponent.GetFrameworkComponent<LocalizationComponent>().RemoveLocalizedCom(this);
            }
        }

        /// <summary>
        /// 生成本地化KEY值
        /// </summary>
        [Button(ButtonSizes.Large)]
        public void GenerateLocalizationKey()
        {
            LocalizationKey = GetPath(transform).Replace("/Canvas (Environment)", "").TrimStart('/');
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }


        /// <summary>
        /// 获取Transform路径
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <returns>Transform路径</returns>
        private static string GetPath(Transform transform)
        {
            return transform ? GetPath(transform.parent) + "/" + transform.gameObject.name : "";
        }
        
        [Serializable]
        public struct TextData
        {
            /// <summary>
            /// 语言
            /// </summary>
            public Language Language;
            /// <summary>
            /// 使能字体大小
            /// </summary>
            public bool EnableFontSize;
            /// <summary>
            /// 字体大小
            /// </summary>
            public int FontSize;
        }

        /// <summary>
        /// 本地化键值
        /// </summary>
        public string GetLocalizationKey()
        {
            return LocalizationKey;
        }

        /// <summary>
        /// 本地化内容
        /// </summary>
        public string GetContent()
        {
            return text;
        }

        /// <summary>
        /// 本地化
        /// </summary>
        public void DoLocalize()
        {
            var localizationComponent = WingjoyFrameworkComponent.GetFrameworkComponent<LocalizationComponent>();
            if (EnableText)
            {
                text = localizationComponent.GS(LocalizationKey);
            }

            var textData = LocalizationData.Find((data => data.Language == localizationComponent.Language));
            if (textData.Language != localizationComponent.Language)
            {
                textData = m_OriginalTextData;
            }

            if (textData.EnableFontSize)
            {
                fontSize = textData.FontSize;
            }
        }
    }
}