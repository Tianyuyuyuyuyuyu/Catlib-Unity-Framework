using System;
using UnityEngine;
using Framework.Utility.Runtime;

namespace Framework.Runtime.Localization
{
    public static class XmlLocalizationHelper
    {
        /// <summary>
        /// 解析字典。
        /// </summary>
        /// <param name="text">要解析的字典文本。</param>
        /// <returns>是否解析字典成功。</returns>
        public static bool ParseDictionary(string text)
        {
            var localizationComponent = FrameworkComponent.GetFrameworkComponent<LocalizationComponent>();

            try
            {
                LocalizationXml localizationXml = new LocalizationXml(localizationComponent.Language);
                localizationXml.ParseLocalizationXml(text);

                foreach (var keyValuePair in localizationXml.KeyValue)
                {
                    if (!localizationComponent.AddRawString(keyValuePair.Key, keyValuePair.Value.Content))
                    {
                        Debug.LogWarningFormat("Can not add raw string with key '{0}' which may be invalid or duplicate.", keyValuePair.Key);
                        return false;
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                Debug.LogWarningFormat("Can not parse dictionary '{0}' with exception '{1}'.", text, RuntimeUtilities.Text.Format("{0}\n{1}", exception.Message, exception.StackTrace));
                return false;
            }
        }
    }
}
