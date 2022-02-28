using System;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Wingjoy.Framework.Runtime.Localization;
using WingjoyUtility.Runtime;

namespace Wingjoy.Framework.Editor.Localization
{
    [GlobalConfig("WingjoyData/Framework/Localization")]
    public class TranslationDatabase : GlobalConfig<TranslationDatabase>
    {
        private string m_SearchKey;
        private TranslationResultDatabaseXML m_TranslationResultDatabase;
        private TranslationResultDatabaseXML m_TempTranslationResultDatabase;
        private Dictionary<TranslationResultXML,bool> m_CurrentResultXml = new Dictionary<TranslationResultXML, bool>();

        private static readonly string m_TranslationDatabasePath =
            "WingjoyData/Framework/Localization";

        private readonly string m_XmlPath = m_TranslationDatabasePath + "/TranslationDatabase.xml";
        private readonly string m_OutPutPath = m_TranslationDatabasePath + "/OutPutTranslationDatabase.xml";
        private readonly string m_TempXmlPath = m_TranslationDatabasePath + "/TempTranslationDatabase.xml";

        /// <summary>
        /// 是否被修改过
        /// </summary>
        private bool m_IsDirty;

        /// <summary>
        /// 当前页
        /// </summary>
        private int m_CurrentPage;

        /// <summary>
        /// 最大页
        /// </summary>
        private int m_MaxPage;

        /// <summary>
        /// 每页数量
        /// </summary>
        private const int NumberOfPage = 10;
        /// <summary>
        /// 是否加载
        /// </summary>
        private bool m_IsLoad;

        /// <summary>
        /// 绘制
        /// </summary>
        [OnInspectorGUI]
        public void OnInspectorGUI()
        {
            SirenixEditorGUI.BeginIndentedVertical();
            SirenixEditorGUI.Title(
                string.Format("请输入想要搜索的关键字  数据库中共有{0}条记录", m_TranslationResultDatabase?.Database?.Count), "",
                TextAlignment.Left, true);

            //搜索栏
            SirenixEditorGUI.BeginIndentedHorizontal();
            Rect rect1 = GUILayoutUtility.GetRect(0.0f, 10, (GUILayoutOption[]) GUILayoutOptions.ExpandWidth(true));
            var searchField = SirenixEditorGUI.SearchField(rect1, m_SearchKey);
            SirenixEditorGUI.EndIndentedHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (searchField != m_SearchKey)
            {
                m_SearchKey = searchField;
                m_CurrentPage = 1;

                Dictionary<TranslationResultXML, bool> translationResultDic = new Dictionary<TranslationResultXML, bool>();

                if (!string.IsNullOrEmpty(m_SearchKey))
                {
                    //var startDateTime = DateTime.Now;
                    foreach (var translationResultXml in m_TranslationResultDatabase?.GetResult(m_SearchKey))
                    {
                        if (m_CurrentResultXml.ContainsKey(translationResultXml))
                        {
                            translationResultDic.Add(translationResultXml, m_CurrentResultXml[translationResultXml]);
                        }
                        else
                        {
                            translationResultDic.Add(translationResultXml, true);
                        }
                    }
                    //Debug.Log("搜索耗时 {0}",(DateTime.Now - startDateTime).ToString("g"));
                }

                m_CurrentResultXml = translationResultDic;
            }

            if (m_CurrentResultXml != null && m_CurrentResultXml.Count > 0)
            {
                EditorGUILayout.PrefixLabel(string.Format("搜索到{0}条", m_CurrentResultXml.Count), SirenixGUIStyles.BoldLabelCentered);
                EditorGUILayout.Space();
                m_MaxPage = Mathf.CeilToInt(m_CurrentResultXml.Count * 1f / NumberOfPage);
                SirenixEditorGUI.BeginHorizontalToolbar();
                if (SirenixEditorGUI.ToolbarButton("<"))
                {
                    m_CurrentPage--;
                    m_CurrentPage = Mathf.Clamp(m_CurrentPage, 1, m_MaxPage);
                }
                if (SirenixEditorGUI.ToolbarButton(">"))
                {
                    m_CurrentPage++;
                    m_CurrentPage = Mathf.Clamp(m_CurrentPage, 1, m_MaxPage);
                }
                SirenixEditorGUI.Title(RuntimeUtilities.Text.Format("当前页{0}/共{1}页", m_CurrentPage, m_MaxPage), "",
                    TextAlignment.Left, false);
                SirenixEditorGUI.EndHorizontalToolbar();

                var translationResultXmls = m_CurrentResultXml.Keys.ToList();

                for (var index = (m_CurrentPage - 1) * NumberOfPage;
                    index < m_CurrentPage * NumberOfPage && index < translationResultXmls.Count;
                    index++)
                {
                    var translationResultXml = translationResultXmls[index];
                    m_CurrentResultXml[translationResultXml] =
                        EditorGUILayout.BeginFoldoutHeaderGroup(m_CurrentResultXml[translationResultXml],
                            translationResultXml.Original);

                    if (m_CurrentResultXml[translationResultXml])
                    {
                        var keyCollection = translationResultXml.Result.Keys.ToList();

                        SirenixEditorGUI.BeginBox();
                        foreach (var language in keyCollection)
                        {
                            SirenixEditorGUI.BeginIndentedHorizontal();
                            EditorGUILayout.PrefixLabel(language.ToString());
                            var delayedTextField =
                                SirenixEditorFields.DelayedTextField(translationResultXml.Result[language]);
                            if (delayedTextField != translationResultXml.Result[language])
                            {
                                m_IsDirty = true;
                            }

                            translationResultXml.Result[language] = delayedTextField;
                            SirenixEditorGUI.EndIndentedHorizontal();
                        }

                        SirenixEditorGUI.EndBox();
                    }

                    EditorGUILayout.EndFoldoutHeaderGroup();

                    EditorGUILayout.Space();
                }
            }
            else
            {
                SirenixEditorGUI.MessageBox("没有找到匹配结果", MessageType.Info);
            }

            //搜索结果
            SirenixEditorGUI.EndIndentedVertical();

            EditorGUILayout.Space();
        }

        /// <summary>
        /// 加载
        /// </summary>
        [Button(ButtonSizes.Large)]
        private void Load()
        {
            if (EditorUtility.DisplayDialog("加载", "加载数据将丢失未保存的数据，确定要加载吗", "确定", "取消"))
            {
                LoadInternal();
            }
        }

        /// <summary>
        /// 加载
        /// </summary>
        private void LoadInternal()
        {
            EditorUtility.DisplayProgressBar("加载中", "正在加载缓存库", 0);

            m_TranslationResultDatabase = new TranslationResultDatabaseXML();
            var path = Application.dataPath + "/" + m_XmlPath;
            if (File.Exists(path))
            {
                var allText = File.ReadAllText(path);
                m_TranslationResultDatabase.Load(allText);
            }
            else
            {
                m_TranslationResultDatabase.GenerateXmlDocument().Save("Assets/" + m_XmlPath);
                AssetDatabase.Refresh();
            }

            m_TempTranslationResultDatabase = new TranslationResultDatabaseXML();
            path = Application.dataPath + "/" + m_TempXmlPath;
            if (File.Exists(path))
            {
                var allText = File.ReadAllText(path);
                m_TempTranslationResultDatabase.Load(allText);
            }
            else
            {
                m_TempTranslationResultDatabase.GenerateXmlDocument().Save("Assets/" + m_TempXmlPath);
                AssetDatabase.Refresh();
            }

            Debug.Log("加载翻译文本缓存");
            m_IsLoad = true;
            EditorUtility.ClearProgressBar();
        }

        private void OnEnable()
        {
            CompilationPipeline.compilationStarted += o =>
            {
                if (m_IsDirty)
                {
                    Save();
                }
            };

            m_IsDirty = false;
            m_IsLoad = false;
            if (m_CurrentResultXml == null)
            {
                //Load();
                m_CurrentResultXml = new Dictionary<TranslationResultXML, bool>();
            }
        }

        /// <summary>
        /// 使能检查
        /// </summary>
        public void EnableCheck()
        {
            if (!m_IsLoad)
            {
                LoadInternal();
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        [Button(ButtonSizes.Large)]
        public void Save()
        {
            if (m_TranslationResultDatabase != null)
            {
                m_TranslationResultDatabase.GenerateXmlDocument().Save("Assets/" + m_XmlPath);
                AssetDatabase.Refresh();
                Debug.Log("保存成功");
                m_IsDirty = false;
            }

            if (m_TempTranslationResultDatabase != null)
            {
                m_TempTranslationResultDatabase.GenerateXmlDocument().Save("Assets/" + m_TempXmlPath);
                AssetDatabase.Refresh();
                Debug.Log("保存成功");
                m_IsDirty = false;
            }
        }

        [Button(ButtonSizes.Large)]
        public void Sub()
        {
            if (EditorUtility.DisplayDialog("减法", "确定要实行减法并导出内容吗？", "确定", "取消"))
            {
                TranslationResultDatabaseXML outPut = new TranslationResultDatabaseXML();
                foreach (var translationResultXml in m_TranslationResultDatabase.Database)
                {
                    outPut.Database.Add(translationResultXml.Key, translationResultXml.Value);
                }

                List<string> noContainKey = new List<string>();
                foreach (var translationResultXml in outPut.Database)
                {
                    if (m_TempTranslationResultDatabase.Database.ContainsKey(translationResultXml.Key))
                        continue;
                    noContainKey.Add(translationResultXml.Key);
                }

                foreach (var s in noContainKey)
                {
                    outPut.Database.Remove(s);
                }

                outPut.GenerateXmlDocument().Save("Assets/" + m_OutPutPath);
            }
        }

        [Button(ButtonSizes.Large)]
        public void Merge()
        {
            if (EditorUtility.DisplayDialog("减法", "使用Output中的内容进行矫正？", "确定", "取消"))
            {
                var outputDatabase = new TranslationResultDatabaseXML();
                var path = Application.dataPath + "/" + m_OutPutPath;
                if (File.Exists(path))
                {
                    var allText = File.ReadAllText(path);
                    try
                    {
                        outputDatabase.Load(allText);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                        throw;
                    }
                }
                else
                {
                    outputDatabase.GenerateXmlDocument().Save("Assets/" + m_OutPutPath);
                    AssetDatabase.Refresh();
                }

                foreach (var translationResultXml in outputDatabase.Database)
                {
                    if (m_TranslationResultDatabase.Database.ContainsKey(translationResultXml.Key))
                    {
                        m_TranslationResultDatabase.Database[translationResultXml.Key] = translationResultXml.Value;
                    }
                    else
                    {
                        m_TranslationResultDatabase.Database.Add(translationResultXml.Key, translationResultXml.Value);
                    }
                }
            }
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="original">原文</param>
        /// <param name="language">语言</param>
        /// <param name="result">翻译结果</param>
        public void Insert(string original, Language language, string result)
        {
            EnableCheck();

            m_IsDirty = true;
            m_TempTranslationResultDatabase.Append(original, language, result);
            m_TranslationResultDatabase.Append(original, language, result);
            Debug.LogFormat("Insert {0}", result);
        }

        /// <summary>
        /// 检索
        /// </summary>
        /// <param name="original">原文</param>
        /// <param name="language">语言</param>
        /// <returns>翻译结果</returns>
        public string Select(string original, Language language)
        {
            EnableCheck();

            //Debug.Log("Select {0}", original);
            var @select = m_TranslationResultDatabase.Select(original, language);
            m_TempTranslationResultDatabase.Append(original, language, @select);
            return @select;
        }

        [Button(ButtonSizes.Large)]
        public void CheckDailyWorkContent(string start,int aimCount)
        {
            var outputDatabase = new TranslationResultDatabaseXML();
            var path = Application.dataPath + "/" + m_OutPutPath;
            if (File.Exists(path))
            {
                var allText = File.ReadAllText(path);
                try
                {
                    outputDatabase.Load(allText);
                    var list = outputDatabase.Database.Keys.ToList();
                    var findIndex = list.FindIndex((s => s.Equals(start)));
                    var range = list.GetRange(findIndex, list.Count - findIndex);
                    int currentCount = 0;
                    int lineCount = 0;
                    Debug.Log($"从{findIndex}开始");
                    foreach (var s in range)
                    {
                        Debug.Log(s);
                        lineCount++;
                        currentCount += s.Length;
                        if (currentCount > aimCount)
                        {
                            Debug.Log($"到{list.IndexOf(s)}结束 共{lineCount}行");
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    throw;
                }
            }
        }
    }
}