using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using OfficeOpenXml;
#if USE_TMPRO
using TMPro;
#endif
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wingjoy.Framework.Runtime.Localization;
using WingjoyUtility.Editor;
using WingjoyUtility.Runtime;

namespace Wingjoy.Framework.Editor.Localization
{
    [GlobalConfig("WingjoyData/Framework/Localization")]
    public class LocalizationScene : GlobalConfig<LocalizationScene>
    {
        /// <summary>
        /// 要本地化的场景
        /// </summary>
        [ValueDropdown("GetSceneAsset")] 
        public SceneAsset Scene;

        /// <summary>
        /// 目标文件夹
        /// </summary>
        [LabelText("生成到文件夹")] 
        public DefaultAsset TargetFolder;

        /// <summary>
        /// 场景excel文件
        /// </summary>
        public DefaultAsset SceneExcel;

        [Button("搜索结果转Excel", ButtonSizes.Large)]
        public void ExportToExcel()
        {
            LocalizationWorksheet nowLocalizationWorksheet = LocalizationWorksheet.ReadLocalizationWorksheet(LocalizationSetting.Instance.SourceLanguage, SceneExcel);

            LocalizationWorksheet localizationWorksheet = new LocalizationWorksheet(LocalizationSetting.Instance.SourceLanguage);
            LocalizationXml sourceLanguageLocalizationXml = localizationWorksheet.GetSourceLanguageLocalizationXml();
            var sceneByName = SceneManager.GetSceneByName(Scene.name);
            if (!sceneByName.isLoaded)
            {
                if (EditorUtility.DisplayDialog("警告", $"即将打开场景{Scene.name}", "确定", "取消"))
                {
                    sceneByName = EditorSceneManager.OpenScene(AssetDatabase.GetAssetOrScenePath(Scene), OpenSceneMode.Single);
                }
                else
                {
                    return;
                }
            }

            var rootGameObjects = sceneByName.GetRootGameObjects();
            foreach (var rootGameObject in rootGameObjects)
            {
                // var localizedTexts = rootGameObject.GetComponentsInChildren<LocalizedText>(true);
                // foreach (var localizedText in localizedTexts)
                // {
                //     if (localizedText.EnableText)
                //     {
                //         sourceLanguageLocalizationXml.Append(localizedText.LocalizationKey, localizedText.text);
                //     }
                // }

                var componentsInChildren = rootGameObject.GetComponentsInChildren<ILocalizedCom>(true);
                foreach (var localizedCom in componentsInChildren)
                {
                    if (localizedCom.EnableLocalization)
                    {
                        sourceLanguageLocalizationXml.Append(localizedCom.GetLocalizationKey(), localizedCom.GetContent());
                    }
                }
            }

            var languageLocalizationXml = nowLocalizationWorksheet.GetSourceLanguageLocalizationXml();
            foreach (var keyValuePair in languageLocalizationXml.KeyValue)
            {
                sourceLanguageLocalizationXml.Add(keyValuePair.Key, keyValuePair.Value.Content);
            }


            for (var languageIndex = 0; languageIndex < LocalizationSetting.Instance.SupportLanguages.Count; languageIndex++)
            {
                var language = LocalizationSetting.Instance.SupportLanguages[languageIndex];
                var assetPath = AssetDatabase.GetAssetPath(TargetFolder) + $"/{language}/LocalizationSceneDictionary.xml";

                LocalizationXml xml = null;
                var loadAssetAtPath = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
                if (language == LocalizationSetting.Instance.SourceLanguage)
                {
                    if (loadAssetAtPath == null)
                    {
                        xml = sourceLanguageLocalizationXml;
                    }
                }

                if (loadAssetAtPath != null)
                {
                    xml = new LocalizationXml(language);
                    xml.ParseLocalizationXml(loadAssetAtPath.text);
                }

                if (xml != null)
                {
                    localizationWorksheet.AddLocalizationXml(xml, LocalizationWorksheet.MergeType.Replace);
                }
            }

            localizationWorksheet.Compare(nowLocalizationWorksheet);
            
            FileInfo newFile = new FileInfo(AssetDatabase.GetAssetPath(SceneExcel));
            var generateWorksheet = localizationWorksheet.GenerateWorksheet("Sheet1");
            generateWorksheet.SaveAs(newFile);
            generateWorksheet.Dispose();
        }

        [Button("翻译后的Excel转运行时使用的Xml", ButtonSizes.Large)]
        public void ExcelToXml()
        {
            FileInfo newFile = new FileInfo(AssetDatabase.GetAssetPath(SceneExcel));
            try
            {
                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    LocalizationWorksheet localizationWorksheet = new LocalizationWorksheet(LocalizationSetting.Instance.SourceLanguage);
                    localizationWorksheet.ParseWorksheet(package, "Sheet1");
                    foreach (var keyValuePair in localizationWorksheet.Value)
                    {
                        var assetPath = AssetDatabase.GetAssetPath(TargetFolder) + $"/{keyValuePair.Key}/LocalizationSceneDictionary.xml";
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

        // [BoxGroup("搜索"), InfoBox("生成的注释会以#开头"), LabelText("生成注释")]
        // public bool BuildSceneDetail = false;

        //         [BoxGroup("搜索"), Button("搜索场景中的中文", ButtonSizes.Large)]
        //         public void SearchChineseInScene()
        //         {
        //             StringBuilder stringBuilder = new StringBuilder(1024);
        //             EditorUtility.DisplayProgressBar("搜索中，请稍等", "0%", 0);
        //
        //             var sceneByName = SceneManager.GetSceneByName(Scene.name);
        //             var rootGameObjects = sceneByName.GetRootGameObjects();
        //
        //             foreach (var rootGameObject in rootGameObjects)
        //             {
        //                 var textsInChildren = rootGameObject.GetComponentsInChildren<Text>(true);
        //                 foreach (var textCom in textsInChildren)
        //                 {
        //                     if (HasChinese(textCom.text))
        //                     {
        //                         if (BuildSceneDetail)
        //                         {
        //                             stringBuilder.AppendFormat("#{0}", GetTransformPath(textCom.transform, ""));
        //                             stringBuilder.AppendLine();
        //                         }
        //
        //                         var key = Runtime.Utility.MD5.Encrypt(textCom.text);
        //
        //                         stringBuilder.AppendFormat("{0}={1}", key, textCom.text);
        //                         stringBuilder.AppendLine();
        //
        //                         stringBuilder.AppendLine();
        //                     }
        //
        //                     EditorUtility.DisplayProgressBar("搜索中，请稍等", textCom.name, 0);
        //                 }
        //             }
        //
        //             EditorUtility.ClearProgressBar();
        //             string path = Application.dataPath.Replace("Assets", string.Empty) +
        //                           AssetDatabase.GetAssetPath(TargetFolder) + "/ChineseInScene.txt";
        //             File.WriteAllText(path, stringBuilder.ToString());
        //         }
        //
        //         [BoxGroup("替换"), LabelText("已经更正的中文文件")]
        //         public TextAsset SourceFile;
        //
        //         [BoxGroup("替换"), Button("替换场景中的中文", ButtonSizes.Large)]
        //         public void ReplaceChineseInScene()
        //         {
        //             if (!EditorUtility.DisplayDialog("警告", "请确定你的所有要替换文件已经备份！", "我已备份，继续吧", "稍等，让我备份一下"))
        //             {
        //                 return;
        //             }
        //
        //             if (SourceFile == null)
        //             {
        //                 EditorUtility.DisplayDialog("警告", "修改后的文件未配置路径", "确定");
        //                 return;
        //             }
        //
        //             EditorUtility.DisplayProgressBar("替换中，请稍等", "0%", 0);
        //
        //             var assetPath = AssetDatabase.GetAssetPath(SourceFile);
        //             var allLines = File.ReadAllLines(Application.dataPath.Replace("Assets", string.Empty) + assetPath);
        //             Dictionary<string, string> localizationDictionary = new Dictionary<string, string>();
        //             foreach (var allLine in allLines)
        //             {
        //                 if (allLine.IndexOf('#') == 0)
        //                     continue;
        //                 var split = allLine.Split('=');
        //                 if (split.Length != 2)
        //                     continue;
        //
        //                 if (string.IsNullOrEmpty(split[0]) || string.IsNullOrEmpty(split[1]))
        //                 {
        //                     continue;
        //                 }
        //
        //                 if (!localizationDictionary.ContainsKey(split[0]))
        //                     localizationDictionary.Add(split[0], split[1]);
        //             }
        //
        //             var sceneByName = SceneManager.GetSceneByName(Scene.name);
        //             var rootGameObjects = sceneByName.GetRootGameObjects();
        //
        //             foreach (var rootGameObject in rootGameObjects)
        //             {
        //                 var textsInChildren = rootGameObject.GetComponentsInChildren<Text>(true);
        //                 foreach (var textCom in textsInChildren)
        //                 {
        //                     if (HasChinese(textCom.text))
        //                     {
        //                         if (localizationDictionary.TryGetValue(Runtime.Utility.MD5.Encrypt(textCom.text), out var newValue))
        //                         {
        //                             textCom.text = newValue;
        //
        //                             EditorUtility.DisplayProgressBar("替换中，请稍等", string.Format("{0}->{1}", textCom.text, newValue), 0);
        //                         }
        //                     }
        //                 }
        //             }
        //
        //             EditorUtility.ClearProgressBar();
        //             AssetDatabase.SaveAssets();
        //         }
        //
        //         [BoxGroup("辅助"), Button("给物体添加本地化组件(只会给有内容的Text添加)", ButtonSizes.Large)]
        //         public void AppendLocalizationToText()
        //         {
        //             var sceneByName = SceneManager.GetSceneByName(Scene.name);
        //             var rootGameObjects = sceneByName.GetRootGameObjects();
        //
        //             foreach (var rootGameObject in rootGameObjects)
        //             {
        //                 var textsInChildren = rootGameObject.GetComponentsInChildren<Text>(true);
        //                 foreach (var textCom in textsInChildren)
        //                 {
        //                     if (string.IsNullOrEmpty(textCom.text))
        //                     {
        //                         continue;
        //                     }
        //
        //                     var staticLocalizer = textCom.GetComponent<StaticLocalizer>();
        //                     if (staticLocalizer == null)
        //                     {
        //                         staticLocalizer = textCom.gameObject.AddComponent<StaticLocalizer>();
        //                         staticLocalizer.Target = textCom;
        //                     }
        //
        //                     staticLocalizer.Key = GetTransformPath(textCom.transform, null).Replace("Canvas.","");
        //                 }
        //             }
        //         }
        //
        //         /// <summary>
        //         /// 获取路径
        //         /// </summary>
        //         /// <param name="transform">Tran</param>
        //         /// <param name="path">当前路径</param>
        //         /// <returns>路径</returns>
        //         public string GetTransformPath(Transform transform, string path)
        //         {
        //             
        //             if (transform.parent != null)
        //             {
        //                 path = "." + transform.name + path;
        //                 return GetTransformPath(transform.parent, path);
        //             }
        //             else
        //             {
        //                 path = transform.name + path;
        //                 return path;
        //             }
        //         }
        //
        //         [Button(ButtonSizes.Large), BoxGroup("辅助"), LabelText("根据场景中的StaticLoc组件生成本地化XML文件")]
        //         public void GenerateXmlLocalizationFile()
        //         {
        //             EditorCoroutineUtility.StartCoroutineOwnerless(GenerateXmlLocalizationFileCo());
        //         }
        //
        //
        //         /// <summary>
        //         /// 根据翻译生成结果
        //         /// </summary>
        //         /// <returns>结果</returns>
        //         public IEnumerator GenerateXmlLocalizationFileCo()
        //         {
        //             //搜索场景中的可本地化组件 生成key-value
        //             Dictionary<string, string> dictionary = new Dictionary<string, string>();
        //
        //             var sceneByName = SceneManager.GetSceneByName(Scene.name);
        //             var rootGameObjects = sceneByName.GetRootGameObjects();
        //
        //             foreach (var rootGameObject in rootGameObjects)
        //             {
        //                 var componentsInChildren = rootGameObject.GetComponentsInChildren<StaticLocalizer>(true);
        //                 foreach (var componentsInChild in componentsInChildren)
        //                 {
        //                     string key = componentsInChild.Key;
        //                     if (string.IsNullOrEmpty(key))
        //                     {
        //                         Debug.LogError(rootGameObject + "" + componentsInChild.gameObject.name + "的Key值没有设置");
        //                         continue;
        //                     }
        //
        //                     string value = string.Empty;
        //                     if (componentsInChild.Target.GetType() == typeof(Text))
        //                     {
        //                         value = ((Text)componentsInChild.Target).text;
        //                     }
        // #if USE_TMPRO
        //                     else if (componentsInChild.Target.GetType() == typeof(TextMeshProUGUI))
        //                     {
        //                         value = ((TextMeshProUGUI)componentsInChild.Target).text;
        //                     }
        // #endif
        //                     else if (componentsInChild.Target.GetType() == typeof(Image))
        //                     {
        //                         var assetPath = AssetDatabase.GetAssetPath(((Image)componentsInChild.Target).sprite);
        //                         var extension = Path.GetExtension(assetPath);
        //                         //当前暂不支持全路径加载
        //                         if (assetPath.Contains("Assets/Resources/"))
        //                         {
        //                             value = assetPath.Replace("Assets/Resources/", "").Replace(extension, "");
        //                         }
        //                         else
        //                         {
        //                             Debug.LogError("{0} 未在Resources下", assetPath);
        //                         }
        //
        //                         continue;
        //                     }
        //
        //                     if (dictionary.ContainsKey(key))
        //                     {
        //                         Debug.LogError("{0} 已包含 字典中:{1} 当前:{2}", key, dictionary[key], value);
        //                         continue;
        //                     }
        //
        //                     dictionary.Add(key, value);
        //                 }
        //             }
        //
        //             foreach (var supportLanguage in LocalizationSetting.Instance.SupportLanguages)
        //             {
        //                 var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(Runtime.Utility.Text.Format(
        //                     "Assets/Resources/Localization/{0}/StaticDictionary.xml",
        //                     supportLanguage));
        //
        //                 LocalizationXml localizationXml = new LocalizationXml(supportLanguage);
        //
        //                 if (textAsset != null)
        //                 {
        //                     localizationXml.ParseLocalizationXml(textAsset.text);
        //                 }
        //
        //                 foreach (var keyValuePair in dictionary)
        //                 {
        //                     var @select = TranslationDatabase.Instance.Select(keyValuePair.Value, supportLanguage);
        //
        //                     if (string.IsNullOrEmpty(@select))
        //                     {
        //                         if (supportLanguage != LocalizationSetting.Instance.SourceLanguage)
        //                         {
        //                             yield return new EditorWaitForSeconds(TranslatorOverview.Instance.DefaultTranslator.WaitInterval);
        //                         }
        //
        //                         if (!string.IsNullOrEmpty(keyValuePair.Value))
        //                         {
        //                             @select = TranslatorOverview.Instance.DefaultTranslator.Translate(LocalizationSetting.Instance.SourceLanguage, supportLanguage, keyValuePair.Value);
        //                         }
        //                         else
        //                         {
        //                             Debug.LogError(keyValuePair.Key +"为空");
        //                         }
        //                     }
        //
        //                     localizationXml.Append(keyValuePair.Key, @select);
        //                 }
        //
        //                 string path = Runtime.Utility.Text.Format("Assets/Resources/Localization/{0}/StaticDictionary.xml",
        //                     supportLanguage);
        //
        //                 var directoryName =
        //                     Path.Combine(Application.dataPath.Replace("/Assets", ""), Path.GetDirectoryName(path));
        //                 if (!Directory.Exists(directoryName))
        //                 {
        //                     Directory.CreateDirectory(directoryName);
        //                 }
        //
        //                 localizationXml.GenerateLocalizationXml().Save(path);
        //             }
        //
        //             Debug.Log("GenerateXmlLocalizationFile Done!");
        //             TranslationDatabase.Instance.Save();
        //             AssetDatabase.Refresh();
        //         }
        //

        #region tools

        /// <summary>
        /// 获得工程中的场景列表
        /// </summary>
        public IEnumerable GetSceneAsset()
        {
            return RuntimeUtilities.File.SearchFile<SceneAsset>("t:Scene", new[] {"Assets"})
                .Select((asset => new ValueDropdownItem(asset.ToString(), asset)));
        }

        private Regex m_Regex = new Regex("\"[^\"]*\""); //引号正则

        /// <summary>
        /// 字符串中是否有中文
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>是否有中文</returns>
        private bool HasChinese(string str)
        {
            return Regex.IsMatch(str, @"[\u4e00-\u9fa5]"); //中文正则
        }

        #endregion
    }
}