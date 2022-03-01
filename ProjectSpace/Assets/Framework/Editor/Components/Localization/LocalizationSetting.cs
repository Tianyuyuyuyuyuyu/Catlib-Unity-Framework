using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using Framework.Runtime.Localization;

namespace Framework.Editor.Localization
{
    [GlobalConfig("FrameworkData/Framework/Localization")]
    public class LocalizationSetting : GlobalConfig<LocalizationSetting>
    {
        /// <summary>
        /// 源语言
        /// </summary>
        public Language SourceLanguage;

        /// <summary>
        /// 支持的语言
        /// </summary>
        public List<Language> SupportLanguages = new List<Language>();

        /// <summary>
        /// 本地化语言数
        /// </summary>
        public int LanguageCount
        {
            get { return Enum.GetValues(typeof(SystemLanguage)).Length; }
        }
        
        /// <summary>
        /// 无键值
        /// </summary>
        public static string NoKeyValue = "NoKey";

        // 当对象已启用并处于活动状态时调用此函数
        private void OnEnable()
        {
            // var findObjectOfType = GameObject.FindObjectOfType<LocalizationComponent>();
            // // lin: 远程编译国内版时，有错误的log打印 
            // // NullReferenceException: Object reference not set to an instance of an object
            // if (findObjectOfType != null)
            // {
            //     SupportLanguages = findObjectOfType.SupportLanguages;
            // }
        }

//        private void OnValidate()
//        {
//            var findObjectOfType = GameObject.FindObjectOfType<LocalizationComponent>();
//            SupportLanguages = findObjectOfType.SupportLanguages;
//        }
    }
}