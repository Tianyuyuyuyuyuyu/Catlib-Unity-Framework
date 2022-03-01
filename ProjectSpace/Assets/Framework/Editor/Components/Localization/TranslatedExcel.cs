using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using Framework.Runtime.Localization;
using Framework.Utility.Runtime;

namespace Framework.Editor.Localization
{
    [GlobalConfig("FrameworkData/FrameworkMono/Localization")]
    public class TranslatedExcel : GlobalConfig<TranslatedExcel>
    {
        /// <summary>
        /// 需要翻译的语言
        /// </summary>
        public List<Language> ToLanguages;
        /// <summary>
        /// Excel
        /// </summary>
        public List<DefaultAsset> Excels;
        /// <summary>
        /// 倾向翻译模式
        /// </summary>
        [InfoBox("倾向模式: \n Excel：如果Excel中非Nokey，则使用Excel里的内容\nTranslationDatabase：如果TranslationDatabase里有内容则倾向覆盖Excel里的内容")]
        public PreferMode Mode;
        
        [Button("翻译", ButtonSizes.Large)]
        public void Translate()
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(TranslateCo());
        }

        /// <summary>
        /// 翻译协程
        /// </summary>

        IEnumerator TranslateCo()
        {
            EditorUtility.DisplayProgressBar("翻译中，请稍等", "0%", 0);

            for (var index = 0; index < Excels.Count; index++)
            {
                var defaultAsset = Excels[index];
                LocalizationWorksheet localizationWorksheet = LocalizationWorksheet.ReadLocalizationWorksheet(LocalizationSetting.Instance.SourceLanguage,defaultAsset);
                
                var sourceLanguageLocalizationXml = localizationWorksheet.GetSourceLanguageLocalizationXml();

                foreach (var keyValuePair in localizationWorksheet.Value)
                {
                    if (keyValuePair.Key == localizationWorksheet.SourceLanguage)
                        continue;

                    var localizationXml = keyValuePair.Value;
                    
                    if (!ToLanguages.Contains(localizationXml.DictionaryLanguage))
                    {
                        continue;
                    }

                    foreach (var valuePair in localizationXml.KeyValue)
                    {
                        if (sourceLanguageLocalizationXml.TryGetString(valuePair.Key, out var content))
                        {
                            //更倾向使用Excel里的内容，则有内容就跳过翻译

                            string excelContent = valuePair.Value.Content;
                            string translationDatabaseContent = TranslationDatabase.Instance.Select(content, localizationXml.DictionaryLanguage);

                            string result = string.Empty;
                            switch (Mode)
                            {
                                case PreferMode.Excel:
                                    result = excelContent == LocalizationSetting.NoKeyValue ? translationDatabaseContent : excelContent;
                                    break;
                                case PreferMode.TranslationDatabase:
                                    result = translationDatabaseContent == LocalizationSetting.NoKeyValue ? excelContent : translationDatabaseContent;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                            
                            
                            if (result == LocalizationSetting.NoKeyValue||string.IsNullOrEmpty(result))
                            {
                                result = TranslatorOverview.Instance.DefaultTranslator.Translate(LocalizationSetting.Instance.SourceLanguage, localizationXml.DictionaryLanguage, content);
                            }

                            localizationXml.Replace(valuePair.Key, result);
                        }

                        if (EditorUtility.DisplayCancelableProgressBar("翻译中，请稍等", $"{defaultAsset.name} {localizationXml.DictionaryLanguage} {valuePair.Key}", index*1f / Excels.Count))
                        {
                            EditorUtility.ClearProgressBar();
                            TranslationDatabase.Instance.Save();
                            yield break;
                        }
                    }
                }

                var generateWorksheet = localizationWorksheet.GenerateWorksheet("Sheet1");
                generateWorksheet.SaveAs(new FileInfo(AssetDatabase.GetAssetPath(defaultAsset)));
                generateWorksheet.Dispose();

                yield return RuntimeUtilities.WaitFor.EndOfFrame;
            }

            TranslationDatabase.Instance.Save();
            EditorUtility.ClearProgressBar();
        }

        public enum PreferMode
        {
            Excel,
            TranslationDatabase
        }
    }
}