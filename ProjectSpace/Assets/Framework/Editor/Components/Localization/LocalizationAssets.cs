using OfficeOpenXml;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Framework.Runtime.Localization;
using Framework.Utility.Runtime;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Object = UnityEngine.Object;

namespace Framework.Editor.Localization
{
    [GlobalConfig("FrameworkData/FrameworkMono/Localization")]
    public class LocalizationAssets : GlobalConfig<LocalizationAssets>
    {
        [LabelText("源集合")]
        public List<Object> Source;

        /// <summary>
        /// 配置表excel文件
        /// </summary>
        public DefaultAsset ConfigExcel;

        /// <summary>
        /// 引号正则
        /// </summary>
        private Regex m_Regex = new Regex("\"[^\"]*\"");

        /// <summary>
        /// 获取需要搜索的资产文件
        /// </summary>
        /// <param name="newPath">新路径</param>
        /// <returns>资产字典</returns>
        public Dictionary<string, string> GetAssetFiles(string newPath = "")
        {
            Dictionary<string, string> searchFile = new Dictionary<string, string>();

            foreach (var o in Source)
            {
                var assetPath = AssetDatabase.GetAssetPath(o);
                var path = Application.dataPath.Replace("Assets", string.Empty) + assetPath;

                if (searchFile.ContainsKey(path))
                {
                    Debug.Log("All ready Contain");
                    continue;
                }

                if (File.Exists(path))
                {
                    searchFile.Add(path, Path.Combine(newPath, "{0}", Path.GetFileName(path)));
                }
                else if (Directory.Exists(path))
                {
                    var dictionary = SearchFile(path, path, newPath);
                    foreach (var keyValuePair in dictionary)
                    {
                        searchFile.Add(keyValuePair.Key, keyValuePair.Value);
                    }
                }
                else
                {
                    Debug.LogError(o.name + "既不是文件也不是文件夹");
                }
            }

            Dictionary<string, string> finallyDictionary = new Dictionary<string, string>();

            foreach (var keyValuePair in searchFile)
            {
                var replace = keyValuePair.Key.Replace("/", "\\");
                if (replace.EndsWith(".meta"))
                {
                    //Debug.Log("Skip meta file");
                    continue;
                }

                if (finallyDictionary.ContainsKey(replace))
                {
                    Debug.Log($"Already have {replace}");
                    continue;
                }

                finallyDictionary.Add(replace, keyValuePair.Value);
            }

            return finallyDictionary;
        }

        [Button("=>Excel", ButtonSizes.Large)]
        public void ExportToExcel()
        {
            FileInfo nowExcelFile = new FileInfo(AssetDatabase.GetAssetPath(ConfigExcel));
            var nowLocalizationWorksheet = LocalizationWorksheet.ReadLocalizationWorksheet(LocalizationSetting.Instance.SourceLanguage,nowExcelFile);

            var sourceLanguageLocalizationXml = nowLocalizationWorksheet.GetSourceLanguageLocalizationXml();
            var assetFiles = GetAssetFiles();
            var keyValuePairs = assetFiles.ToList();
            for (var i = 0; i < keyValuePairs.Count; i++)
            {
                var keyValuePair = keyValuePairs[i];
                var fileName = Path.GetFileName(keyValuePair.Key);
                var readAllText = File.ReadAllText(keyValuePair.Key);

                // Setup the input
                var input = new StringReader(readAllText);

                // Load the stream
                var yaml = new YamlStream();
                yaml.Load(input);

                var allString = GetAllString(yaml);
                EditorUtility.DisplayProgressBar("搜索", $"搜索文件中{i}/{keyValuePairs.Count}", (i * 1f / keyValuePairs.Count));
                for (var index = 0; index < allString.Count; index++)
                {
                    var s = allString[index];
                    if (HasChinese(s))
                    {
                        sourceLanguageLocalizationXml.Append($"[{fileName}.{RuntimeUtilities.MD5.Encrypt(s)}]", s);
                    }
                }
            }

            EditorUtility.ClearProgressBar();
            
            var supportLanguages = LocalizationSetting.Instance.SupportLanguages;
            foreach (var supportLanguage in supportLanguages)
            {
                if (!nowLocalizationWorksheet.HasLanguage(supportLanguage))
                {
                    nowLocalizationWorksheet.AddLocalizationXml(new LocalizationXml(supportLanguage), LocalizationWorksheet.MergeType.Add);
                }
            }

            FileInfo newFile = new FileInfo(AssetDatabase.GetAssetPath(ConfigExcel));
            var generateWorksheet = nowLocalizationWorksheet.GenerateWorksheet("Sheet1");
            generateWorksheet.SaveAs(newFile);
            generateWorksheet.Dispose();
        }

        [Button("Excel=>Asset", ButtonSizes.Large)]
        public void Localization()
        {
            var folder = Path.GetDirectoryName("Assets/Config/Loc");
            var saveFolderPanel = EditorUtility.SaveFolderPanel("Select Folder", folder, "Loc");
            if (saveFolderPanel == "")
                return;

            LocalizationWorksheet nowLocalizationWorksheet = LocalizationWorksheet.ReadLocalizationWorksheet(LocalizationSetting.Instance.SourceLanguage,ConfigExcel);

            var assetFiles = GetAssetFiles();
            var filePaths = assetFiles.Keys.ToList();
            for (var i = 0; i < filePaths.Count; i++)
            {
                var path = filePaths[i];
                var fileName = Path.GetFileName(path);
                var readAllText = File.ReadAllText(path, Encoding.UTF8);
#if true
                foreach (var localizationXml in nowLocalizationWorksheet.Value)
                {
                    // Setup the input
                    var input = new StringReader(readAllText);

                    // Load the stream
                    var yaml = new YamlStream();
                    yaml.Load(input);

                    ReplaceAllString(yaml, fileName, localizationXml);
                    
                    var newFilePath = $"{saveFolderPanel}/{localizationXml.Key}/{fileName}";
                    var directoryName = Path.GetDirectoryName(newFilePath);
                    if (!Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    using (TextWriter writer = File.CreateText(newFilePath))
                    {
                        writer.Write(@"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 ");
                        yaml.Save(writer, false);
                    }

                    //特殊处理
                    //...
                    //---
                    //改为
                    //...
                    //--- !u!114
                    readAllText = File.ReadAllText(newFilePath, Encoding.UTF8);
                    readAllText = readAllText.Replace(@"...
---", @"--- !u!114");
                    readAllText.Substring(0, readAllText.Length - 3);
                    File.WriteAllText(newFilePath, readAllText);
                    if (EditorUtility.DisplayCancelableProgressBar($"{fileName}-{localizationXml.Key}", $"{i}/{filePaths.Count}", i * 1f / filePaths.Count))
                    {
                        break;
                    }

                }


#else
                foreach (var localizationXml in nowLocalizationWorksheet.Value)
                {
                    var fileContent = readAllText;
                    var matchCollection = m_Regex.Matches(fileContent);
                    List<string> slices = new List<string>();
                    int startIndex = 0;
                    foreach (Match o in matchCollection)
                    {
                        var count = o.Index - startIndex;
                        var substring = fileContent.Substring(startIndex, count);
                        slices.Add(substring);
                        var content = Regex.Unescape(o.Value);
                        if (HasChinese(content))
                        {
                            var replace = content.Replace("\"", "");
                            var localizationKey = $"[{fileName}.{RuntimeUtilities.MD5.Encrypt(replace)}]";
                            if (localizationXml.Value.TryGetString(localizationKey, out var str))
                            {
                                if (localizationXml.Key == Language.English)
                                {
                                    //英语特殊处理
                                    //str = str.Replace("'", "''");
                                    str = str.Replace("\"", "\\\"");
                                    //content = content.Replace($"\"{replace}\"", $"'{str}'");
                                }

                                content = content.Replace(replace, str);
                            }
                        }


                        slices.Add(ChineseToUnicode(content, true));
                        // if (localizationXml.Key == Language.English)
                        // {
                        //     
                        // }
                        // else
                        // {
                        //     slices.Add(ChineseToUnicode(content, true));
                        // }

                        startIndex = o.Index + o.Length;
                    }

                    var finallySliceLength = fileContent.Length - startIndex;
                    var finallySubstring = fileContent.Substring(startIndex, finallySliceLength);
                    slices.Add(finallySubstring);

                    string newFileContent = string.Empty;
                    foreach (var s in slices)
                    {
                        newFileContent += s;
                    }

                    var newFilePath = $"{saveFolderPanel}/{localizationXml.Key}/{fileName}";
                    var directoryName = Path.GetDirectoryName(newFilePath);
                    if (!Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    File.WriteAllText(newFilePath, newFileContent);

                    if (EditorUtility.DisplayCancelableProgressBar($"{fileName}-{localizationXml.Key}", $"{index}/{filePaths.Count}", index * 1f / filePaths.Count))
                    {
                        break;
                    }
                }
#endif
                if (EditorUtility.DisplayCancelableProgressBar(fileName, $"{i}/{filePaths.Count}", i * 1f / filePaths.Count))
                {
                    break;
                }
            }

            EditorUtility.ClearProgressBar();

            AssetDatabase.Refresh();
        }

        public void ReplaceAllString(YamlStream yamlStream, string fileName, KeyValuePair<Language, LocalizationXml> localizationXml)
        {
            //List<string> values = new List<string>();
            foreach (var yamlDocument in yamlStream.Documents)
            {
                var yamlNodes = yamlDocument.AllNodes.ToList();
                for (var index = 0; index < yamlNodes.Count; index++)
                {
                    var yamlNode = yamlNodes[index];
                    if (yamlNode.NodeType == YamlNodeType.Scalar)
                    {
                        var node = (YamlScalarNode)yamlNode;
                        if (HasChinese(node.Value))
                        {
                            var replace = node.Value.Replace("\"", "");
                            var localizationKey = $"[{fileName}.{RuntimeUtilities.MD5.Encrypt(replace)}]";
                            if (localizationXml.Value.TryGetString(localizationKey, out var str))
                            {
                                if (localizationXml.Key == Language.English)
                                {
                                    //英语特殊处理
                                    //str = str.Replace("'", "''");
                                    str = str.Replace("\"", "\\\"");
                                    //content = content.Replace($"\"{replace}\"", $"'{str}'");
                                }

                                node.Value = node.Value.Replace(replace, str);
                            }
                        }
                    }
                    EditorUtility.DisplayProgressBar("替换", $"Ymal{index}/{yamlNodes.Count}", index * 1f / yamlNodes.Count);
                }
            }
        }

        public static string ArrayToString(char[] chars)
        {
            string s = string.Empty;

            for (int i = 0; i < chars.Length && chars[i] != 0; i++)
            {
                s += chars[i];
            }

            return s;
        }

        /// <summary>
        /// 本地化协程
        /// </summary>
        // IEnumerator LocalizationCo()
        // {
        //     var saveFilePanel = EditorUtility.SaveFilePanel("Save object as", Path.GetDirectoryName("Assets"), name, "asset");
        //     var newPath = saveFilePanel.Replace(name + ".asset", "");
        //
        //     var finallyDictionary = GetAssetFiles(newPath);
        //     
        //     if (finallyDictionary.Count <= 0)
        //     {
        //         Debug.LogError("没有找到文件");
        //         yield break;
        //     }
        //
        //     var files = finallyDictionary.ToList();
        //
        //     for (var i = 0; i < files.Count; i++)
        //     {
        //         var keyValuePair = files[i];
        //         var fileName = Path.GetFileName(keyValuePair.Key);
        //         var allLines = File.ReadAllLines(keyValuePair.Key);
        //         //遍历语言
        //         foreach (var supportLanguage in LocalizationSetting.Instance.SupportLanguages)
        //         {
        //             string[] copy = new string[allLines.Length];
        //             allLines.CopyTo(copy, 0);
        //
        //             if (supportLanguage != LocalizationSetting.Instance.SourceLanguage)
        //             {
        //                 //遍历资源文件的每一行
        //                 for (var index = 0; index < copy.Length; index++)
        //                 {
        //                     var line = copy[index];
        //                     var matchCollection = m_Regex.Matches(line);
        //
        //                     //遍历每一行中的双引号内容
        //                     foreach (Match match in matchCollection)
        //                     {
        //                         var matchValue = match.Value;
        //
        //                         var unescape = Regex.Unescape(matchValue).Trim('\"');
        //
        //                         if (HasChinese(unescape))
        //                         {
        //                             var @select = TranslationDatabase.Instance.Select(unescape, supportLanguage);
        //                             if (string.IsNullOrEmpty(@select))
        //                             {
        //                                 yield return new EditorWaitForSeconds(TranslatorOverview.Instance
        //                                     .DefaultTranslator.WaitInterval);
        //                                 @select = TranslatorOverview.Instance.DefaultTranslator.Translate(
        //                                     LocalizationSetting.Instance.SourceLanguage, supportLanguage,
        //                                     unescape);
        //                             }
        //
        //                             if (string.IsNullOrEmpty(@select))
        //                             {
        //                                 Debug.LogError(unescape + "翻译出错");
        //                             }
        //                             else
        //                             {
        //                                 var replace = @select.Replace("\"", "\\\"");
        //                                 var s = ChineseToUnicode(replace);
        //                                 copy[index] = line.Replace(matchValue,
        //                                     "\"" + s + "\"");
        //                             }
        //                         }
        //                     }
        //
        //                     if (index % 10000 == 0)
        //                     {
        //                         if (EditorUtility.DisplayCancelableProgressBar(
        //                             "翻译中 " + supportLanguage + " " + i + "/" + files.Count,
        //                             fileName + (index * 1f / copy.Length).ToString("P"), index * 1f / copy.Length))
        //                         {
        //                             TranslationDatabase.Instance.Save();
        //                             EditorUtility.ClearProgressBar();
        //                             yield break;
        //                         }
        //                     }
        //                 }
        //             }
        //
        //             var localizationPath = string.Format(keyValuePair.Value, supportLanguage);
        //             var directoryName = Path.GetDirectoryName(localizationPath);
        //             if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
        //             {
        //                 Directory.CreateDirectory(directoryName);
        //             }
        //             File.WriteAllLines(localizationPath, copy);
        //         }
        //     }
        //
        //     TranslationDatabase.Instance.Save();
        //     EditorUtility.ClearProgressBar();
        // }
        public string ChineseToUnicode(string chinese, bool withoutWrap)
        {
            string outStr = "";
            if (!string.IsNullOrEmpty(chinese))
            {
                for (int i = 0; i < chinese.Length; i++)
                {
                    var l = (int) chinese[i];

                    if (l >= 127)
                    {
                        outStr += $"\\u{l:X4}";
                    }
                    else if (withoutWrap && l == 10)
                    {
                        //必须双斜杠，否则会出现Unity提示文件可能存在合并冲突的错误
                        outStr += "\\n";
                    }
                    else
                    {
                        outStr += chinese[i].ToString();
                    }
                }
            }

            return outStr;
        }

        /// <summary>
        /// 字符串中是否有中文
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>是否有中文</returns>
        private bool HasChinese(string str)
        {
            return Regex.IsMatch(str, @"[\u4e00-\u9fa5]"); //中文正则
        }

        /// <summary>
        /// 获取所有的字符串
        /// </summary>
        /// <param name="yamlStream">Yaml流</param>
        /// <returns>字符串列表</returns>
        public List<string> GetAllString(YamlStream yamlStream)
        {
            List<string> values = new List<string>();
            foreach (var yamlDocument in yamlStream.Documents)
            {
                var yamlNodes = yamlDocument.AllNodes.ToList();
                for (var index = 0; index < yamlNodes.Count; index++)
                {
                    var yamlNode = yamlNodes[index];
                    if (yamlNode.NodeType == YamlNodeType.Scalar)
                    {
                        var node = (YamlScalarNode) yamlNode;
                        values.Add(node.Value);
                    }
                    EditorUtility.DisplayProgressBar("搜索", $"Ymal{index}/{yamlNodes.Count}", index * 1f / yamlNodes.Count);
                }
            }

            return values;
        }

        public void SeekForString(List<string> values, YamlNode yamlNode)
        {
            var yamlNodeAnchor = yamlNode.Anchor;
            var yamlNodeTag = yamlNode.Tag;
        }

        /// <summary>
        /// 搜索文件并生成相应的新路径
        /// </summary>
        /// <param name="sourceDirectory">源文件夹</param>
        /// <param name="upperLayerDirectory">上层文件夹</param>
        /// <param name="newPath">新保存路径</param>
        /// <returns>文件路径集</returns>
        public Dictionary<string, string> SearchFile(string sourceDirectory, string upperLayerDirectory, string newPath)
        {
            Dictionary<string, string> tempDictionary = new Dictionary<string, string>();
            var files = Directory.GetFiles(upperLayerDirectory).ToList();
            foreach (var file in files)
            {
                tempDictionary.Add(file, newPath + "{0}" + file.Replace(sourceDirectory, ""));
            }

            var directories = Directory.GetDirectories(upperLayerDirectory);
            foreach (var directory in directories)
            {
                var searchFile = SearchFile(sourceDirectory, directory, newPath);
                foreach (var keyValuePair in searchFile)
                {
                    tempDictionary.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }

            return tempDictionary;
        }
    }

    public class SimpleTextLocalization
    {
        private Dictionary<SystemLanguage, string> m_LocalizatioDictionary;

        /// <summary>
        /// 追加语言
        /// </summary>
        /// <param name="language">语言</param>
        /// <param name="value">本地化内容</param>
        public void AppendLanguage(SystemLanguage language, string value)
        {
            m_LocalizatioDictionary.Add(language, value);
        }

        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="language">语言</param>
        /// <returns>是否</returns>
        public bool Contains(SystemLanguage language)
        {
            return m_LocalizatioDictionary.ContainsKey(language);
        }
    }
}