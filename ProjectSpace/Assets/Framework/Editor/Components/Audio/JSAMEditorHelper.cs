using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace JSAM.JSAMEditor
{
    public class JSAMEditorHelper
    {
        public static string TimeToString(float time)
        {
            time *= 1000;
            int minutes = (int)time / 60000;
            int seconds = (int)time / 1000 - 60 * minutes;
            int milliseconds = (int)time - minutes * 60000 - 1000 * seconds;
            return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        }
        
        public static void SmartFolderField(SerializedProperty folderProp)
        {
            EditorGUILayout.BeginHorizontal();
            string filePath = folderProp.stringValue;
            if (filePath == string.Empty) filePath = Application.dataPath;
            GUIContent blontent = new GUIContent(folderProp.displayName, folderProp.tooltip);
            EditorGUI.BeginChangeCheck();
            filePath = EditorGUILayout.DelayedTextField(blontent, filePath);
            if (EditorGUI.EndChangeCheck())
            {
                // If the user presses "cancel"
                if (filePath.Equals(string.Empty))
                {
                    return;
                }
                // or specifies something outside of this folder, reset filePath and don't proceed
                else if (!filePath.Contains("Assets"))
                {
                    EditorUtility.DisplayDialog("Folder Browsing Error!", "AudioManager is a Unity editor tool and can only " +
                        "function inside the project's Assets folder. Please choose a different folder.", "OK");
                    return;
                }
                else
                {
                    // Fix path to be usable for AssetDatabase.FindAssets
                    filePath = filePath.Remove(0, filePath.IndexOf("Assets"));
                    if (filePath[filePath.Length - 1] == '/') filePath = filePath.Remove(filePath.Length - 1, 1);
                }
            }
            SmartBrowseButton(folderProp);
            EditorGUILayout.EndHorizontal();
        }
        
        public static void SmartBrowseButton(SerializedProperty folderProp)
        {
            GUIContent buttonContent = new GUIContent("Browse", "Designate a new folder");
            if (GUILayout.Button(buttonContent, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.MaxWidth(55) }))
            {
                string filePath = folderProp.stringValue;
                filePath = EditorUtility.OpenFolderPanel("Specify a new folder", filePath, string.Empty);

                // If the user presses "cancel"
                if (filePath.Equals(string.Empty))
                {
                    return;
                }
                // or specifies something outside of this folder, reset filePath and don't proceed
                else if (!filePath.Contains("Assets"))
                {
                    EditorUtility.DisplayDialog("Folder Browsing Error!", "AudioManager is a Unity editor tool and can only " +
                        "function inside the project's Assets folder. Please choose a different folder.", "OK");
                    return;
                }
                else if (filePath.Contains(Application.dataPath))
                {
                    // Fix path to be usable for AssetDatabase.FindAssets
                    filePath = filePath.Remove(0, filePath.IndexOf("Assets"));
                }

                folderProp.stringValue = filePath;
            }
        }

        public static List<T> ImportAssetsOrFoldersAtPath<T>(string filePath) where T : Object
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(filePath);
            if (!AssetDatabase.IsValidFolder(filePath))
            {
                if (asset != null)
                {
                    return new List<T> { asset };
                }
            }
            else
            {
                List<T> imports = new List<T>();
                List<string> importTarget = new List<string>(Directory.GetDirectories(filePath));
                importTarget.AddRange(Directory.GetFiles(filePath));
                for (int i = 0; i < importTarget.Count; i++)
                {
                    imports.AddRange(ImportAssetsOrFoldersAtPath<T>(importTarget[i]));
                }
                return imports;
            }

            return new List<T>();
        }

        static Color guiColor;
        public static void BeginColourChange(Color color)
        {
            guiColor = GUI.color;
            GUI.color = color;
        }

        public static void EndColourChange() => GUI.color = guiColor;

        public static GUIStyle ApplyRichTextToStyle(GUIStyle referenceStyle)
        {
            var style = new GUIStyle(referenceStyle);
            style.richText = true;
            return style;
        }

        public static GUIStyle ApplyTextColorToStyle(GUIStyle referenceStyle, Color color)
        {
            var style = new GUIStyle(referenceStyle);
            style.normal.textColor = color;
            return style;
        }

        public static GUIStyle ApplyTextAnchorToStyle(GUIStyle referenceStyle, TextAnchor anchor)
        {
            var style = new GUIStyle(referenceStyle);
            style.alignment = anchor;
            return style;
        }

        public static GUIStyle ApplyFontSizeToStyle(GUIStyle referenceStyle, int fontSize)
        {
            var style = new GUIStyle(referenceStyle);
            style.fontSize = fontSize;
            return style;
        }

        public static GUIStyle ApplyBoldTextToStyle(GUIStyle referenceStyle)
        {
            var style = new GUIStyle(referenceStyle);
            style.fontStyle = FontStyle.Bold;
            return style;
        }
    }
}