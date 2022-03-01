using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using Framework.Runtime.Localization;

namespace Framework.Editor.Localization
{
    public class TranslationResultDatabaseXML
    {
        public Dictionary<string,TranslationResultXML> Database;

        /// <summary>
        ///   初始化 <see cref="T:System.Object" /> 类的新实例。
        /// </summary>
        public TranslationResultDatabaseXML()
        {
            Database = new Dictionary<string, TranslationResultXML>();
        }

        public List<TranslationResultXML> GetResult(string original)
        {
            return Database.Where((pair => pair.Key.Contains(original))).Select((pair => pair.Value)).ToList();
        }

        public string Select(string original, Language language)
        {
            if (Database.TryGetValue(original, out var xml))
            {
                if (xml.Result.TryGetValue(language, out var result))
                {
                    return result;
                }
            }

            return string.Empty;
        }

        public void Append(string original, Language language, string result)
        {
            TranslationResultXML translationResultXml;
            
            if (Database.ContainsKey(original))
            {
                translationResultXml = Database[original];
            }
            else
            {
                translationResultXml = new TranslationResultXML(original);
                Database.Add(original,translationResultXml);
            }

            translationResultXml.Append(language,result);   
        }

        public void Load(string xml)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);
            XmlNode xmlRoot = xmlDocument.SelectSingleNode("Dictionaries");
            XmlNodeList xmlNodeDictionaryList = xmlRoot.ChildNodes;
            for (int i = 0; i < xmlNodeDictionaryList.Count; i++)
            {
                XmlNode xmlNodeDictionary = xmlNodeDictionaryList.Item(i);
                if (xmlNodeDictionary.Name != "Dictionary")
                {
                    continue;
                }

                string original = xmlNodeDictionary.Attributes.GetNamedItem("Original").Value;
                if (string.IsNullOrEmpty(original))
                {
                    continue;
                }

                TranslationResultXML translationResultXml = new TranslationResultXML(original);

                XmlNodeList xmlNodeStringList = xmlNodeDictionary.ChildNodes;
                for (int j = 0; j < xmlNodeStringList.Count; j++)
                {
                    XmlNode xmlNodeString = xmlNodeStringList.Item(j);
                    if (xmlNodeString.Name != "String")
                    {
                        continue;
                    }

                    //if (xmlNodeString.Attributes.GetNamedItem("Key") == null)
                    //{
                    //    Debug.Log(xmlNodeStringList.Item(j-1).Value);
                    //}
                    var key = xmlNodeString.Attributes.GetNamedItem("Key").Value;
                    var value = xmlNodeString.Attributes.GetNamedItem("Value").Value;

                    if (Enum.TryParse<Language>(key,out var language))
                    {
                        translationResultXml.Append(language,value);
                    }
                    else
                    {
                        Debug.LogError(key +"没有找到匹配的语言");
                    }
                }

                Database.Add(original,translationResultXml);
            }
        }

        public XmlDocument GenerateXmlDocument()
        {
            XmlDocument xmlDocument = new XmlDocument();

            //<?xml version="1.0" encoding="UTF-8"?>
            var xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDocument.AppendChild(xmlDeclaration);

            var xmlRoot = xmlDocument.CreateElement("", "Dictionaries", "");
            var root = xmlDocument.AppendChild(xmlRoot);

            foreach (var translationResultXml in Database)
            {
                var xmlElement = xmlDocument.CreateElement("Dictionary");
                xmlElement.SetAttribute("Original", translationResultXml.Key);
                var appendChild = root.AppendChild(xmlElement);

//                for (int i = xmlDictionary.ChildNodes.Count - 1; i >= 0; i--)
//                {
//                    xmlDictionary.RemoveChild(xmlDictionary.ChildNodes.Item(i));
//                }

                foreach (var keyValuePair in translationResultXml.Value.Result)
                {
                    var element = xmlDocument.CreateElement("String");
                    element.SetAttribute("Key", keyValuePair.Key.ToString());
                    element.SetAttribute("Value", keyValuePair.Value);
                    appendChild.AppendChild(element);
                }
            }

            return xmlDocument;
        }
    }
}
