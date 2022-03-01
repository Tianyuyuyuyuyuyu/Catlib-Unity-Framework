using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using Framework.Runtime.Localization;

namespace Framework.Editor.Localization
{
    [GlobalConfig("FrameworkData/Framework/Localization")]
    public class LocalizationText : GlobalConfig<LocalizationText>
    {
        public Language SourceLanguage;

        public Language TargetLanguage;

        [Multiline]
        public string Content;

        [Multiline]
        public string Result;

        [Button(ButtonSizes.Large,Name = "翻译")]
        public void StartTranslate()
        {
//            if (string.IsNullOrEmpty(SourceLanguage))
//            {
//                EditorUtility.DisplayDialog("警告", "源语言未选定", "确定");
//                return;
//            }
//            if (string.IsNullOrEmpty(TargetLanguage))
//            {
//                EditorUtility.DisplayDialog("警告", "目标语言未选定", "确定");
//                return;
//            }
            if (string.IsNullOrEmpty(Content))
            {
                EditorUtility.DisplayDialog("警告", "未输入翻译内容", "确定");
                return;
            }

            Result = Regex.Unescape(TranslatorOverview.Instance.DefaultTranslator.Translate(SourceLanguage, TargetLanguage, Content));
        }
    }
}
