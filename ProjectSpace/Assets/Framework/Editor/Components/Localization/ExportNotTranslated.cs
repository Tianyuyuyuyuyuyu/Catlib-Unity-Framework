using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using Framework.Runtime.Localization;
using Framework.Utility.Runtime;

namespace Framework.Editor.Localization
{
    [GlobalConfig("FrameworkData/Framework/Localization")]
    public class ExportNotTranslated : GlobalConfig<ExportNotTranslated>
    {
        /// <summary>
        /// Excel
        /// </summary>
        public List<DefaultAsset> Excels;

        /// <summary>
        /// 导出
        /// </summary>
        [Button("导出未翻译部分", ButtonSizes.Large)]
        public void ExportNotTranslatedContent()
        {
            var folder = Path.GetDirectoryName("Assets/FrameworkData/Framework/Localization/NotTranslated/");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var saveFolderPanel = EditorUtility.SaveFolderPanel("Select Folder", folder, "");
            if (saveFolderPanel == "")
                return;

            Dictionary<FileInfo, LocalizationWorksheet> worksheets = new Dictionary<FileInfo, LocalizationWorksheet>();
            for (var index = 0; index < Excels.Count; index++)
            {
                var defaultAsset = Excels[index];
                FileInfo newFile = new FileInfo(AssetDatabase.GetAssetPath(defaultAsset));

                var readLocalizationWorksheet = LocalizationWorksheet.ReadLocalizationWorksheet(LocalizationSetting.Instance.SourceLanguage, newFile);

                HashSet<string> notTranslatedKeyHashSet = new HashSet<string>();
                var sourceLanguageLocalizationXml = readLocalizationWorksheet.GetSourceLanguageLocalizationXml();
                foreach (var valuePair in sourceLanguageLocalizationXml.KeyValue)
                {
                    bool hasNotTranslated = false;
                    var localizationKey = valuePair.Key;
                    foreach (var keyValuePair in readLocalizationWorksheet.Value)
                    {
                        if (keyValuePair.Value.TryGetString(localizationKey, out var content))
                        {
                            if (content == LocalizationSetting.NoKeyValue || string.IsNullOrEmpty(content))
                            {
                                hasNotTranslated = true;
                            }
                        }
                    }

                    if (hasNotTranslated)
                    {
                        notTranslatedKeyHashSet.Add(localizationKey);
                    }
                }

                //新建Excel
                LocalizationWorksheet worksheet = new LocalizationWorksheet(LocalizationSetting.Instance.SourceLanguage);
                worksheets.Add(newFile, worksheet);

                foreach (var keyValuePair in readLocalizationWorksheet.Value)
                {
                    var addLocalizationXml = worksheet.AddLocalizationXml(new LocalizationXml(keyValuePair.Key), LocalizationWorksheet.MergeType.Append);
                    foreach (var valuePair in sourceLanguageLocalizationXml.KeyValue)
                    {
                        var localizationKey = valuePair.Key;
                        if (notTranslatedKeyHashSet.Contains(localizationKey))
                        {
                            if (keyValuePair.Value.TryGetString(localizationKey, out var content))
                            {
                                addLocalizationXml.Add(localizationKey, content);
                            }
                        }
                    }
                }
            }

            foreach (var keyValuePair in worksheets)
            {
                var generateWorksheet = keyValuePair.Value.GenerateWorksheet("Sheet1");
                var folderPanel = saveFolderPanel + "/" + keyValuePair.Key.Name;
                FileInfo fileInfo = new FileInfo(folderPanel);
                var stream = fileInfo.Create();
                stream.Dispose();
                generateWorksheet.SaveAs(fileInfo);
            }
        }

        /// <summary>
        /// Excel
        /// </summary>
        public List<DefaultAsset> CompareExcels;

        /// <summary>
        /// 导出
        /// </summary>
        [Button("比对上次翻译并导出", ButtonSizes.Large)]
        public void CompareExport()
        {
            var folder = Path.GetDirectoryName("Assets/FrameworkData/Framework/Localization/CompareResult/");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var saveFolderPanel = EditorUtility.SaveFolderPanel("Select Folder", folder, "");
            if (saveFolderPanel == "")
                return;

            Dictionary<FileInfo, LocalizationWorksheet> worksheets = new Dictionary<FileInfo, LocalizationWorksheet>();
            for (var index = 0; index < Excels.Count; index++)
            {
                var defaultAsset = Excels[index];

                var find = CompareExcels.Find((asset => asset.name == defaultAsset.name));
                if (find != null)
                {
                    FileInfo localFile = new FileInfo(AssetDatabase.GetAssetPath(defaultAsset));
                    var localWorksheet = LocalizationWorksheet.ReadLocalizationWorksheet(LocalizationSetting.Instance.SourceLanguage, localFile);

                    FileInfo otherFile = new FileInfo(AssetDatabase.GetAssetPath(find));
                    var otherWorksheet = LocalizationWorksheet.ReadLocalizationWorksheet(LocalizationSetting.Instance.SourceLanguage, otherFile);

                    localWorksheet.Compare(otherWorksheet);

                    //新建Excel
                    LocalizationWorksheet worksheet = new LocalizationWorksheet(LocalizationSetting.Instance.SourceLanguage);
                    worksheets.Add(localFile, worksheet);

                    foreach (var keyValuePair in localWorksheet.Value)
                    {
                        worksheet.AddLocalizationXml(new LocalizationXml(keyValuePair.Key), LocalizationWorksheet.MergeType.Append);
                    }

                    var sourceLanguageLocalizationXml = localWorksheet.GetSourceLanguageLocalizationXml();
                    foreach (var keyValuePair in sourceLanguageLocalizationXml.KeyValue)
                    {
                        bool notNoneStatus = false;
                        foreach (var localizationXml in localWorksheet.Value)
                        {
                            if (localizationXml.Key == Language.ChineseTraditional)
                                continue;

                            if (localizationXml.Value.TryGetValue(keyValuePair.Key, out var v))
                            {
                                if (v.Status != LocalizationXml.Status.None)
                                {
                                    notNoneStatus = true;
                                }
                            }
                        }

                        if (notNoneStatus)
                        {
                            foreach (var localizationXml in localWorksheet.Value)
                            {
                                if (localizationXml.Key == Language.ChineseTraditional)
                                    continue;
                                if (localizationXml.Value.TryGetValue(keyValuePair.Key, out var v))
                                {
                                    var xml = worksheet.GetLocalizationXml(localizationXml.Key);
                                    var value = xml.Add(keyValuePair.Key, v.Content);
                                    value.Status = v.Status;
                                }
                            }
                        }
                    }
                }
            }

            foreach (var keyValuePair in worksheets)
            {
                var generateWorksheet = keyValuePair.Value.GenerateWorksheet("Sheet1");
                var folderPanel = saveFolderPanel + "/" + keyValuePair.Key.Name;
                FileInfo fileInfo = new FileInfo(folderPanel);
                var stream = fileInfo.Create();
                stream.Dispose();
                generateWorksheet.SaveAs(fileInfo);
            }
        }

        
        /// <summary>
        /// 扩展字符串
        /// </summary>
        public string ExternalChar;
        
        [Button("导出所有文本", ButtonSizes.Large)]
        public void ExportAllText()
        {
            var folder = Path.GetDirectoryName("Assets/FrameworkData/Framework/Localization/Text/");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var saveFolderPanel = EditorUtility.SaveFolderPanel("Select Folder", folder, "");
            if (saveFolderPanel == "")
                return;

            string totalContent = string.Empty;
            StringBuilder stringBuilder = new StringBuilder(1024);
            stringBuilder.Append("abcdefghijklmnopqistuvwxyz");
            stringBuilder.Append("0123456789");
            stringBuilder.Append("ABCDEFGHIJKLMNOPQISTUVWXYZ");
            stringBuilder.Append(ExternalChar);
            for (var index = 0; index < Excels.Count; index++)
            {
                var defaultAsset = Excels[index];
                var readLocalizationWorksheet = LocalizationWorksheet.ReadLocalizationWorksheet(LocalizationSetting.Instance.SourceLanguage, defaultAsset);
                foreach (var keyValuePair in readLocalizationWorksheet.Value)
                {
                    foreach (var valuePair in keyValuePair.Value.KeyValue)
                    {
                        stringBuilder.Append(valuePair.Value.Content);
                    }
                }

                string content = new string(stringBuilder.ToString().Distinct().ToArray());
                totalContent += content;
                stringBuilder.Clear();
            }
            var fileName = $"{saveFolderPanel}/AllText.txt";
            File.WriteAllText(fileName, new string(totalContent.Distinct().ToArray()));
        }
    }
}