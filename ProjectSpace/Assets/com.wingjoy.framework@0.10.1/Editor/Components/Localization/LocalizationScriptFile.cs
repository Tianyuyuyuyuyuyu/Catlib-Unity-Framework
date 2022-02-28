using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using Wingjoy.Framework.Runtime.Localization;
using WingjoyUtility.Runtime;

namespace Wingjoy.Framework.Editor.Localization
{
    [GlobalConfig("WingjoyData/Framework/Localization")]
    public class LocalizationScriptFile : GlobalConfig<LocalizationScriptFile>
    {
        /// <summary>
        /// 引号正则
        /// </summary>
        private Regex m_Regex = new Regex("\"[^\"]*\"");
        
        /// <summary>
        /// 脚本文件夹
        /// </summary>
        public DefaultAsset SourceFolder;

        /// <summary>
        /// 场景excel文件
        /// </summary>
        public DefaultAsset ScriptExcel;

        /// <summary>
        /// 目标文件夹
        /// </summary>
        public DefaultAsset XMLFolder;

        /// <summary>
        /// 搜索文件
        /// </summary>
        /// <param name="directory">文件夹路径</param>
        /// <returns>文件路径集</returns>
        public List<string> SearchFile(string directory)
        {
            List<string> files = Directory.GetFiles(directory).ToList();
            var directories = Directory.GetDirectories(directory);
            foreach (var s in directories)
            {
                files.AddRange(SearchFile(s));
            }

            return files;
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
        /// 获得所有文件路径
        /// </summary>
        /// <returns>文件路径列表</returns>
        public List<string> GetFiles()
        {
            var folderPath = AssetDatabase.GetAssetPath(SourceFolder);
            var directoryPath = Application.dataPath.Replace("Assets", string.Empty) + folderPath;

            var searchFile = SearchFile(directoryPath);
            var enumerable =
                searchFile.FindAll((s => !s.Contains("Editor") && !s.Contains("meta") && s.Contains(".cs")));
            return enumerable;
        }

        /// <summary>
        /// 是否需要跳过
        /// </summary>
        /// <param name="str">字符串内容</param>
        /// <returns>需要跳过</returns>
        public bool NeedSkip(string str)
        {
            if (str.Contains("Debug."))
                return true;
            if (str.IndexOf("//") == 0)
                return true;
            if (str.IndexOf("///") == 0)
                return true;
            if (str.StartsWith("[") || str.EndsWith("]"))
                return true;
            if (str.Contains("Exception(\""))
                return true;
            if (str.StartsWith("EditorUtility."))
                return true;
            return false;
        }

        [Button("打印脚本中中文", ButtonSizes.Large)]
        public void SearchChinese()
        {
            EditorUtility.DisplayProgressBar("搜索中，请稍等", "0%", 0);
            var files = GetFiles();
            for (var fileIndex = 0; fileIndex < files.Count; fileIndex++)
            {
                var csharpFile = files[fileIndex];
                var readAllLines = File.ReadAllLines(csharpFile);
                var fileName = Path.GetFileName(csharpFile).Replace(".cs", "");
                for (var lineIndex = 0; lineIndex < readAllLines.Length; lineIndex++)
                {
                    var line = readAllLines[lineIndex];
                    var str = line.Trim();
                    if (NeedSkip(str))
                        continue;

                    var matchCollection = m_Regex.Matches(line);

                    foreach (Match match in matchCollection)
                    {
                        if (HasChinese(match.Value))
                        {
                            Debug.Log($"name:{fileName} line:{lineIndex} content:{match.Value}");
                        }
                    }
                }
            }

            EditorUtility.ClearProgressBar();
        }

        [Button("搜索中文 -> Excel",ButtonSizes.Large)]
        public void SearchScriptLocalizationMethod()
        {
            LocalizationWorksheet nowLocalizationWorksheet = LocalizationWorksheet.ReadLocalizationWorksheet(LocalizationSetting.Instance.SourceLanguage,ScriptExcel);
            
            var nowSourceLocalizationXml = nowLocalizationWorksheet.GetSourceLanguageLocalizationXml();
            var files = GetFiles();

            EditorUtility.DisplayProgressBar("搜索中，请稍等", "0%", 0);
            
            for (var fileIndex = 0; fileIndex < files.Count; fileIndex++)
            {
                var csharpFile = files[fileIndex];
                var readAllLines = File.ReadAllLines(csharpFile);
                var fileName = Path.GetFileName(csharpFile).Replace(".cs", "");
                for (var lineIndex = 0; lineIndex < readAllLines.Length; lineIndex++)
                {
                    var line = readAllLines[lineIndex];
                    var str = line.Trim();
                    if(NeedSkip(str))
                        continue;

                    var matchCollection = m_Regex.Matches(line);

                    foreach (Match match in matchCollection)
                    {
                        if (CheckContainsLocalizationMethod(line, match.Value))
                        {
                            if (HasChinese(match.Value))
                            {
                                var replace = match.Value.Trim('"');
                                nowSourceLocalizationXml.Append($"[{fileName}.{RuntimeUtilities.MD5.Encrypt(replace)}]", replace);
                            }
                            else
                            {
                                var key = match.Value.Trim('"');
                                if (!nowSourceLocalizationXml.ContainsKey(key))
                                {
                                    nowSourceLocalizationXml.Add(key,"");
                                }
                            }
                        }
                    }
                }
            }

            var supportLanguages = LocalizationSetting.Instance.SupportLanguages;
            foreach (var supportLanguage in supportLanguages)
            {
                if (!nowLocalizationWorksheet.HasLanguage(supportLanguage))
                {
                    nowLocalizationWorksheet.AddLocalizationXml(new LocalizationXml(supportLanguage), LocalizationWorksheet.MergeType.Add);
                }
            }

            FileInfo newFile = new FileInfo(AssetDatabase.GetAssetPath(ScriptExcel));
            var generateWorksheet = nowLocalizationWorksheet.GenerateWorksheet("Sheet1");
            generateWorksheet.SaveAs(newFile);
            generateWorksheet.Dispose();
            
            EditorUtility.ClearProgressBar();
        }

        [Button("用Key替换脚本中的中文", ButtonSizes.Large)]
        public void ReplaceChineseInScript()
        {
            LocalizationWorksheet nowLocalizationWorksheet = LocalizationWorksheet.ReadLocalizationWorksheet(LocalizationSetting.Instance.SourceLanguage,ScriptExcel);

            var files = GetFiles();
            
            var sourceLanguageLocalizationXml = nowLocalizationWorksheet.GetSourceLanguageLocalizationXml();
            if (sourceLanguageLocalizationXml != null)
            {
                for (var fileIndex = 0; fileIndex < files.Count; fileIndex++)
                {
                    var csharpFile = files[fileIndex];
                    var readAllLines = File.ReadAllLines(csharpFile);
                    var fileName = Path.GetFileName(csharpFile).Replace(".cs", "");
                    bool isDirty = false;
                    for (var lineIndex = 0; lineIndex < readAllLines.Length; lineIndex++)
                    {
                        var line = readAllLines[lineIndex];
                        var str = line.Trim();
                        if (NeedSkip(str))
                            continue;
                        var matchCollection = m_Regex.Matches(line);
                        
                        foreach (Match match in matchCollection)
                        {
                            if (HasChinese(match.Value))
                            {
                                if (CheckContainsLocalizationMethod(line, match.Value))
                                {
                                    var replace = match.Value.Trim('"');
                                    var key = $"[{fileName}.{RuntimeUtilities.MD5.Encrypt(replace)}]";
                                    line = line.Replace(match.Value, $"\"{key}\"");
                                    isDirty = true;
                                }
                            }
                        }

                        readAllLines[lineIndex] = line;
                    }

                    if (isDirty)
                    {
                        File.WriteAllLines(csharpFile, readAllLines);
                    }
                }
            }
        }

        [Button("Excel -> XML", ButtonSizes.Large)]
        public void ExcelToXml()
        {
            FileInfo newFile = new FileInfo(AssetDatabase.GetAssetPath(ScriptExcel));
            try
            {
                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    LocalizationWorksheet localizationWorksheet = new LocalizationWorksheet(LocalizationSetting.Instance.SourceLanguage);
                    localizationWorksheet.ParseWorksheet(package, "Sheet1");
                    foreach (var keyValuePair in localizationWorksheet.Value)
                    {
                        var assetPath = AssetDatabase.GetAssetPath(XMLFolder) + $"/{keyValuePair.Key}/LocalizationScriptDictionary.xml";
                        var directoryName = Path.GetDirectoryName(assetPath);
                        if (!Directory.Exists(directoryName))
                        {
                            Directory.CreateDirectory(directoryName);
                        }
                        keyValuePair.Value.GenerateLocalizationXml().Save(assetPath);
                    }
                }
            }
            catch (IOException e)
            {
                EditorUtility.DisplayDialog("警告", "请先关闭当前使用的Excel", "确定");
                return;
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 本地化方法
        /// </summary>
        [LabelText("本地化方法")]
        public string LocalizationMethod = "CoreBase.Loc.GS(";

        /// <summary>
        /// 检查该文本是否包含本地化方法
        /// </summary>
        /// <param name="line">该行内容</param>
        /// <param name="matchValue">匹配到的文本</param>
        /// <returns>是否</returns>
        public bool CheckContainsLocalizationMethod(string line, string matchValue)
        {
            var indexOf = line.IndexOf(matchValue, StringComparison.Ordinal);
            if (indexOf >= 0 && indexOf < line.Length)
            {
                var length = LocalizationMethod.Length;
                var startIndex = indexOf - length;
                if (startIndex >= 0)
                {
                    var substring = line.Substring(startIndex, length);
                    if (substring == LocalizationMethod)
                    {
                        //已包含本地化方法
                        return true;
                    }
                }
            }
            else
            {
                Debug.LogErrorFormat("Index out of bounds,{0},{1}", line, matchValue);
            }

            return false;
        }
    }
}