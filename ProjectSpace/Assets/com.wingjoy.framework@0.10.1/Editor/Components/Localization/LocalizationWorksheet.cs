using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Wingjoy.Framework.Runtime.Localization;

namespace Wingjoy.Framework.Editor.Localization
{
    public class LocalizationWorksheet
    {
        /// <summary>
        /// 源语言
        /// </summary>
        public Language SourceLanguage;

        /// <summary>
        /// 表单
        /// </summary>
        public Dictionary<Language, LocalizationXml> Value;

        public LocalizationWorksheet(Language sourceLanguage)
        {
            Value = new Dictionary<Language, LocalizationXml>();
            SourceLanguage = sourceLanguage;
            Value.Add(sourceLanguage, new LocalizationXml(sourceLanguage));
        }

        /// <summary>
        /// 是否拥有某语言
        /// </summary>
        /// <param name="language">语言</param>
        /// <returns>是否</returns>
        public bool HasLanguage(Language language)
        {
            return Value.ContainsKey(language);
        }

        /// <summary>
        /// 获取源语言xml
        /// </summary>
        /// <returns>源语言xml</returns>
        public LocalizationXml GetSourceLanguageLocalizationXml()
        {
            return GetLocalizationXml(SourceLanguage);
        }

        /// <summary>
        /// 获取指定语言的xml
        /// </summary>
        /// <param name="language">语言</param>
        /// <returns>xml</returns>
        public LocalizationXml GetLocalizationXml(Language language)
        {
            Value.TryGetValue(language, out var xml);
            return xml;
        }

        /// <summary>
        /// 添加本地化数据
        /// </summary>
        /// <param name="localizationXml">数据</param>
        /// <param name="mergeType">合并方式</param>
        public LocalizationXml AddLocalizationXml(LocalizationXml localizationXml, MergeType mergeType)
        {
            if (Value.TryGetValue(localizationXml.DictionaryLanguage, out var xml))
            {
                var keyList = localizationXml.KeyValue.Keys.ToList();
                foreach (var key in keyList)
                {
                    var newValue = localizationXml.KeyValue[key];
                    if (newValue.Content == LocalizationSetting.NoKeyValue)
                    {
                        continue;
                    }

                    switch (mergeType)
                    {
                        case MergeType.Replace:
                            xml.Replace(key, newValue.Content);
                            break;
                        case MergeType.Append:
                            xml.Append(key, newValue.Content);
                            break;
                        case MergeType.Add:
                            xml.Add(key, newValue.Content);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(mergeType), mergeType, null);
                    }
                }
            }
            else
            {
                Value.Add(localizationXml.DictionaryLanguage, localizationXml);
            }
            
            return GetLocalizationXml(localizationXml.DictionaryLanguage);
        }

        /// <summary>
        /// 转换数据
        /// </summary>
        /// <param name="excelPackage">Excel</param>
        /// <param name="sheetName">表名</param>
        public void ParseWorksheet(ExcelPackage excelPackage, string sheetName)
        {
            var any = excelPackage.Workbook.Worksheets.Any((excelWorksheet => excelWorksheet.Name == sheetName));
            if (any)
            {
                var worksheet = excelPackage.Workbook.Worksheets[sheetName];

                if (worksheet.Dimension == null)
                {
                    return;
                }
                
                var endColumn = worksheet.Dimension.End.Column;
                var endRow = worksheet.Dimension.End.Row;

                for (int col = 2; col <= endColumn; col++)
                {
                    var o = worksheet.Cells[1, col].Value;
                    if(o == null)
                        continue;
                    string languageStr = o.ToString();
                    if (Enum.TryParse<Language>(languageStr, out var language))
                    {
                        LocalizationXml localizationXml = new LocalizationXml(language);
                        for (int row = 2; row <= endRow; row++)
                        {
                            var key = worksheet.Cells[row, 1].Value.ToString();
                            var value = worksheet.Cells[row, col].Value;
                            if (value != null)
                            {
                                localizationXml.Append(key, value.ToString());
                            }
                            else
                            {
                                localizationXml.Append(key, LocalizationSetting.NoKeyValue);
                            }
                        }

                        AddLocalizationXml(localizationXml, MergeType.Append);
                    }
                }
            }
            
            ResetStatus();
        }

        /// <summary>
        /// 生成Excel
        /// </summary>
        /// <param name="sheetName">表名</param>
        /// <returns>Excel</returns>
        public ExcelPackage GenerateWorksheet(string sheetName)
        {
            ExcelPackage package = new ExcelPackage();
            var sourceLanguageXml = GetLocalizationXml(SourceLanguage);
            if (sourceLanguageXml == null)
            {
                Debug.LogError("没有源语言");
                return package;
            }

            var excelWorksheet = package.Workbook.Worksheets.Add(sheetName);
            excelWorksheet.DefaultColWidth = 100;
            var excelStyle = excelWorksheet.Row(1).Style;
            excelStyle.Font.Bold = true;

            //生成表头
            excelWorksheet.Cells[1, 1].Value = "LocalizationKey";
            excelWorksheet.Column(1).Width = 60;
            excelWorksheet.Cells[1, 2].Value = SourceLanguage;


            var keyList = sourceLanguageXml.KeyValue.Keys.ToList();
            for (var index = 0; index < keyList.Count; index++)
            {
                var key = keyList[index];
                var excelWorksheetCell = excelWorksheet.Cells[2 + index, 1];
                excelWorksheetCell.Value = key;
                excelWorksheetCell.Style.Locked = true;
            }

            var languages = Value.Keys.ToList();
            languages.Remove(SourceLanguage);
            languages.Insert(0, SourceLanguage);
            
            for (var languageIndex = 0; languageIndex < languages.Count; languageIndex++)
            {
                var language = languages[languageIndex];
                excelWorksheet.Cells[1, 2 + languageIndex].Value = language;
                var localizationXml = Value[language];
                for (var keyIndex = 0; keyIndex < keyList.Count; keyIndex++)
                {
                    var key = keyList[keyIndex];
                    if (localizationXml.TryGetValue(key, out var value))
                    {
                        var excelWorksheetCell = excelWorksheet.Cells[2 + keyIndex, 2 + languageIndex];
                        excelWorksheetCell.Value = value.Content;

                        if (value.Status == LocalizationXml.Status.None)
                        {

                        }
                        else
                        {
                            excelWorksheetCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            switch (value.Status)
                            {
                                case LocalizationXml.Status.None:
                                    break;
                                case LocalizationXml.Status.NewAdd:
                                    excelWorksheetCell.Style.Fill.BackgroundColor.Indexed = 42;
                                    break;
                                case LocalizationXml.Status.Modify:
                                    excelWorksheetCell.Style.Fill.BackgroundColor.Indexed = 51;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }
                    }
                }
            }

            return package;
        }

        /// <summary>
        /// 重置状态
        /// </summary>
        public void ResetStatus()
        {
            foreach (var keyValuePair in Value)
            {
                foreach (var valuePair in keyValuePair.Value.KeyValue)
                {
                    valuePair.Value.Status = LocalizationXml.Status.None;
                }
            }
        }
        
        /// <summary>
        /// 比对，生成状态数据
        /// </summary>
        /// <param name="other">其他表</param>
        public void Compare(LocalizationWorksheet other)
        {
            foreach (var keyValuePair in Value)
            {
                var thisLocalizationXml = keyValuePair.Value;
                var otherLocalizationXml = other.GetLocalizationXml(keyValuePair.Key);
                if (otherLocalizationXml == null)
                {
                    continue;
                }
                
                foreach (var valuePair in thisLocalizationXml.KeyValue)
                {
                    valuePair.Value.Status = LocalizationXml.Status.None;
                    
                    if (otherLocalizationXml.TryGetValue(valuePair.Key, out var otherValue))
                    {
                        if (otherValue.Content != valuePair.Value.Content)
                        {
                            valuePair.Value.Status = LocalizationXml.Status.Modify;
                        }
                    }
                    else
                    {
                        valuePair.Value.Status = LocalizationXml.Status.NewAdd;
                    }
                }
            }
        }

        /// <summary>
        /// 读取Excel文件
        /// </summary>
        /// <param name="sourceLanguage">源语言</param>
        /// <param name="defaultAsset">資源文件</param>
        /// <returns>Excel文件</returns>
        public static LocalizationWorksheet ReadLocalizationWorksheet(Language sourceLanguage, DefaultAsset defaultAsset)
        {
            FileInfo fileInfo = new FileInfo(AssetDatabase.GetAssetPath(defaultAsset));
            return ReadLocalizationWorksheet(sourceLanguage, fileInfo);
        }

        /// <summary>
        /// 读取Excel文件
        /// </summary>
        /// <param name="sourceLanguage">源语言</param>
        /// <param name="fileInfo">文件信息</param>
        /// <returns>Excel文件</returns>
        public static LocalizationWorksheet ReadLocalizationWorksheet(Language sourceLanguage, FileInfo fileInfo)
        {
            LocalizationWorksheet localizationWorksheet = new LocalizationWorksheet(sourceLanguage);
            try
            {
                using (ExcelPackage package = new ExcelPackage(fileInfo))
                {
                    localizationWorksheet.ParseWorksheet(package, "Sheet1");
                }

                return localizationWorksheet;
            }
            catch (IOException e)
            {
                EditorUtility.DisplayDialog("警告", "请先关闭当前使用的Excel", "确定");
                throw;
            }
        }
        
        
        public enum MergeType
        {
            /// <summary>
            /// 使用新数据替换
            /// </summary>
            Replace,
            /// <summary>
            /// 使用新数据替换并追加
            /// </summary>
            Append,
            /// <summary>
            /// 直接添加缺少的数据
            /// </summary>
            Add,
        }
    }
}