using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using Framework.Runtime.Localization;

namespace Framework.Editor.Localization
{
    [GlobalConfig("FrameworkData/Editor/Component/Localization/Tools")]
    public class MergeExcel : GlobalConfig<MergeExcel>
    {
        /// <summary>
        /// Excel
        /// </summary>
        [LabelText("本地文件")]
        public List<DefaultAsset> LocalFile;

        /// <summary>
        /// Excel
        /// </summary>
        [LabelText("外来文件")]
        public List<DefaultAsset> ExternalFile;

        /// <summary>
        /// 导出
        /// </summary>
        [Button("合并", ButtonSizes.Large)]
        public void Merge()
        {
            Dictionary<DefaultAsset, List<DefaultAsset>> dictionary = new Dictionary<DefaultAsset, List<DefaultAsset>>();

            foreach (var file in LocalFile)
            {
                var sameNameFiles = ExternalFile.FindAll((asset => asset.name == file.name));
                dictionary.Add(file, sameNameFiles);
            }

            foreach (var keyValuePair in dictionary)
            {
                var localFile = keyValuePair.Key;
                FileInfo localFileInfo = new FileInfo(AssetDatabase.GetAssetPath(localFile));
                var localLocalizationWorksheet = LocalizationWorksheet.ReadLocalizationWorksheet(LocalizationSetting.Instance.SourceLanguage, localFileInfo);

                foreach (var externalFile in keyValuePair.Value)
                {
                    FileInfo externalFileInfo = new FileInfo(AssetDatabase.GetAssetPath(externalFile));
                    var externalLocalizationWorksheet = LocalizationWorksheet.ReadLocalizationWorksheet(LocalizationSetting.Instance.SourceLanguage, externalFileInfo);

                    foreach (KeyValuePair<Language, LocalizationXml> localizationXml in externalLocalizationWorksheet.Value)
                    {
                        var xml = localLocalizationWorksheet.GetLocalizationXml(localizationXml.Key);
                        if (xml == null)
                        {
                            localLocalizationWorksheet.AddLocalizationXml(localizationXml.Value, LocalizationWorksheet.MergeType.Add);
                        }
                        else
                        {
                            foreach (var valuePair in localizationXml.Value.KeyValue)
                            {
                                xml.Replace(valuePair.Key, valuePair.Value.Content);
                            }
                        }
                    }
                }

                localLocalizationWorksheet.GenerateWorksheet("Original").SaveAs(localFileInfo);
            }
        }
    }
}
