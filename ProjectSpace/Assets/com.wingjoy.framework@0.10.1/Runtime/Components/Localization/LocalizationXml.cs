using System.Collections.Generic;
using System.Xml;

namespace Wingjoy.Framework.Runtime.Localization
{
    public class LocalizationXml
    {
        /// <summary>
        /// 语言
        /// </summary>
        public readonly Language DictionaryLanguage;

        /// <summary>
        /// 键值对
        /// </summary>
        public Dictionary<string, Value> KeyValue;

        public LocalizationXml(Language dictionaryLanguage)
        {
            KeyValue = new Dictionary<string, Value>();
            DictionaryLanguage = dictionaryLanguage;
        }

        /// <summary>
        /// 转换数据
        /// </summary>
        /// <param name="xml">XML文本</param>
        public void ParseLocalizationXml(string xml)
        {
            string languageStr = DictionaryLanguage.ToString();

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

                string language = xmlNodeDictionary.Attributes.GetNamedItem("Language").Value;
                if (language != languageStr)
                {
                    continue;
                }

                XmlNodeList xmlNodeStringList = xmlNodeDictionary.ChildNodes;
                for (int j = 0; j < xmlNodeStringList.Count; j++)
                {
                    XmlNode xmlNodeString = xmlNodeStringList.Item(j);
                    if (xmlNodeString.Name != "String")
                    {
                        continue;
                    }

                    var key = xmlNodeString.Attributes.GetNamedItem("Key").Value;
                    var value = xmlNodeString.Attributes.GetNamedItem("Value").Value;

                    if (!KeyValue.ContainsKey(key))
                    {
                        KeyValue.Add(key, new Value(value, Status.NewAdd));
                    }
                }
            }
        }

        /// <summary>
        /// 是否还有键
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否含有</returns>
        public bool ContainsKey(string key)
        {
            return KeyValue.ContainsKey(key);
        }

        /// <summary>
        /// 获取与指定键关联的值。
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>是否拿到</returns>
        public bool TryGetValue(string key, out Value value)
        {
            return KeyValue.TryGetValue(key, out value);
        }

        /// <summary>
        /// 获取与指定键关联的内容。
        /// </summary>
        /// <param name="key">键值</param> 
        /// <param name="value">内容</param>
        /// <returns>是否拿到</returns>
        public bool TryGetString(string key, out string value)
        {
            if(KeyValue.TryGetValue(key, out var v))
            {
                value = v.Content;
                return true;
            }
            else
            {
                value = string.Empty;
                return false;
            }
        }

        /// <summary>
        /// 追加字典（已过翻译的）
        /// </summary>
        /// <param name="dictionary">字典</param>
        public void Append(Dictionary<string, string> dictionary)
        {
            foreach (var keyValuePair in dictionary)
            {
                if (KeyValue.ContainsKey(keyValuePair.Key))
                    continue;
                Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        /// <summary>
        /// 替换键值对中的值(不存在时则追加进字典)
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void Replace(string key, string value)
        {
            if (KeyValue.ContainsKey(key))
            {
                var v = KeyValue[key];
                if (v.Content != value)
                {
                    v.Status = Status.Modify;
                }
                v.Content = value;
            }
        }

        /// <summary>
        /// 追加键值对（已过翻译的）
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void Append(string key, string value)
        {
            if (KeyValue.ContainsKey(key))
            {
                var v = KeyValue[key];
                if (v.Content != value)
                {
                    v.Status = Status.Modify;
                }
                v.Content = value;
            }
            else
            {
                KeyValue.Add(key, new Value(value, Status.NewAdd));
            }
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="value">值</param>
        /// <returns>新增内容</returns>
        public Value Add(string key, string value)
        {
            if (!KeyValue.ContainsKey(key))
            {
                var v = new Value(value, Status.NewAdd);
                KeyValue.Add(key, v);
                return v;
            }

            return null;
        }

        /// <summary>
        /// 生成本地化XML
        /// </summary>
        public XmlDocument GenerateLocalizationXml()
        {
            XmlDocument xmlDocument = new XmlDocument();

            //<?xml version="1.0" encoding="UTF-8"?>
            var xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDocument.AppendChild(xmlDeclaration);

            var xmlRoot = xmlDocument.CreateElement("", "Dictionaries", "");
            var root = xmlDocument.AppendChild(xmlRoot);

            var xmlElement = xmlDocument.CreateElement("Dictionary");
            xmlElement.SetAttribute("Language", DictionaryLanguage.ToString());
            root.AppendChild(xmlElement);

            var selectSingleNode = xmlDocument.SelectSingleNode("Dictionaries");
            var xmlDictionary = selectSingleNode.SelectSingleNode("Dictionary");

            for (int i = xmlDictionary.ChildNodes.Count - 1; i >= 0; i--)
            {
                xmlDictionary.RemoveChild(xmlDictionary.ChildNodes.Item(i));
            }

            foreach (var keyValuePair in KeyValue)
            {
                var element = xmlDocument.CreateElement("String");
                element.SetAttribute("Key", keyValuePair.Key);
                element.SetAttribute("Value", keyValuePair.Value.Content);
                xmlDictionary.AppendChild(element);
            }

            return xmlDocument;
        }
        
        public class Value
        {
            /// <summary>
            /// 内容
            /// </summary>
            public string Content;
            /// <summary>
            /// 状态
            /// </summary>
            public Status Status;

            public Value(string content, Status status)
            {
                Content = content;
                Status = status;
            }
        }
        
        public enum Status
        {
            None,
            NewAdd,
            Modify,
        }
    }
}