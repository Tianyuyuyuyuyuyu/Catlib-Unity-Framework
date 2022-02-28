using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using Wingjoy.Framework.Runtime.Localization;
using WingjoyUtility.Editor;

namespace Wingjoy.Framework.Editor.Localization
{
    [CustomEditor(typeof(LocalizedImage), true)]
    [CanEditMultipleObjects]
    public class LocalizedImageEditor : ImageEditor
    {
        SerializedProperty m_Enable;
        SerializedProperty m_LocalizationKey;
        SerializedProperty m_SelectDisplayLanguage;
        SerializedProperty m_LocalizationData;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            m_Enable = serializedObject.FindProperty("EnableImage");
            m_LocalizationKey = serializedObject.FindProperty("LocalizationKey");
            m_SelectDisplayLanguage = serializedObject.FindProperty("m_SelectDisplayLanguage");
            m_LocalizationData = serializedObject.FindProperty("LocalizationData");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (target is LocalizedImage value)
            {
                serializedObject.Update();

                SirenixEditorGUI.BeginBox("Localization");
                if (true)
                {
                    EditorGUILayout.BeginHorizontal();

                    m_Enable.boolValue = EditorGUILayout.Toggle("", m_Enable.boolValue, GUILayoutOptions.MaxWidth(30));
                    EditorGUILayout.PropertyField(m_LocalizationKey);
                    if (GUILayout.Button("生成本地化key", new GUIStyle(SirenixGUIStyles.Button).Width(110)))
                    {
                        value.GenerateLocalizationKey();
                    }

                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.Space();

                    //LocalizationData
                    // SirenixEditorGUI.BeginBox();
                    // var localizationSetting = LocalizationSetting.Instance;
                    // var instanceSupportLanguages = localizationSetting.SupportLanguages;
                    // var enumValueIndex = (Language)m_SelectDisplayLanguage.enumValueIndex;
                    // if (!instanceSupportLanguages.Contains(enumValueIndex))
                    // {
                    //     m_SelectDisplayLanguage.enumValueIndex = (int) localizationSetting.SourceLanguage;
                    // }

                    // EditorGUILayout.BeginHorizontal();
                    // var valueDropDownControlRect = EditorGUILayout.GetControlRect();
                    // var selectLanguage = SirenixEditorFields.Dropdown(valueDropDownControlRect, new GUIContent(), (Language)m_SelectDisplayLanguage.enumValueIndex, instanceSupportLanguages);
                    // if ((int) selectLanguage != m_SelectDisplayLanguage.enumValueIndex)
                    // {
                    //     m_SelectDisplayLanguage.enumValueIndex = (int)selectLanguage;
                    //     serializedObject.ApplyModifiedProperties();
                    //     Event.current.Use();
                    //     return;
                    // }
                    //
                    // var findIndex = value.LocalizationData.FindIndex((data => data.Language == selectLanguage));
                    // if (findIndex == -1)
                    // {
                    //     if (GUILayout.Button($"生成{selectLanguage}数据"))
                    //     {
                    //         value.LocalizationData.Add(new LocalizedText.TextData()
                    //         {
                    //             Language = selectLanguage,
                    //             FontSize = value.fontSize
                    //         });
                    //     }
                    //     EditorGUILayout.EndHorizontal();
                    // }
                    // else if (findIndex < m_LocalizationData.arraySize)
                    // {
                    //     if (GUILayout.Button($"删除{selectLanguage}数据"))
                    //     {
                    //         if (EditorUtility.DisplayDialog("警告", "确定要删除本地化配置吗", "确定", "取消"))
                    //         {
                    //             value.LocalizationData.RemoveAt(findIndex);
                    //             return;
                    //         }
                    //     }
                    //     EditorGUILayout.EndHorizontal();
                    //
                    //     var arrayElementAtIndex = m_LocalizationData.GetArrayElementAtIndex(findIndex);
                    //     var enableFontSize = arrayElementAtIndex.FindPropertyRelative("EnableFontSize");
                    //     var fontSize = arrayElementAtIndex.FindPropertyRelative("FontSize");
                    //
                    //     //if (Event.current.type == EventType.Layout)
                    //     {
                    //         EditorGUILayout.BeginHorizontal();
                    //         enableFontSize.boolValue = EditorGUILayout.Toggle("", enableFontSize.boolValue, GUILayoutOptions.MaxWidth(30));
                    //         GUI.enabled = enableFontSize.boolValue;
                    //         EditorGUILayout.PropertyField(fontSize);
                    //         GUI.enabled = true;
                    //         EditorGUILayout.EndHorizontal();
                    //     }
                    // }
                    // else
                    // {
                    //     EditorGUILayout.EndHorizontal();
                    // }
                    // SirenixEditorGUI.EndBox();
                }

                SirenixEditorGUI.EndBox();
                //EditorGUILayout.(m_LocalizationData);
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}