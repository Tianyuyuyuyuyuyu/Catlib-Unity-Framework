#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace WingjoyUtility.Runtime
{
    public static partial class RuntimeUtilities
    {
        /// <summary>
        /// 字符相关的实用函数。
        /// </summary>
        public static class AssetDatabaseHelper
        {
            public static void SaveAssets()
            {
#if UNITY_EDITOR
                AssetDatabase.SaveAssets();
#endif
            }

#if UNITY_EDITOR
            /// <summary>
            /// January 20th 2021, don't you ever forget you dingus
            /// </summary>
            /// <param name="asset"></param>
            /// <param name="path"></param>
            /// <returns>False if the operation was unsuccessful or was cancelled, 
            /// True if an asset was created.</returns>
            public static bool CreateAssetSafe(Object asset, string path)
            {
                if (AssetDatabase.IsValidFolder(path))
                {
                    Debug.LogError("Error! Attempted to write an asset over a folder!");
                    return false;
                }

                string folderPath = path.Substring(0, path.LastIndexOf("/"));
                if (GenerateFolderStructureAt(folderPath))
                {
                    AssetDatabase.CreateAsset(asset, path);
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Generates the folder structure to a specified path if it doesn't already exist. 
            /// Will perform the check itself first
            /// </summary>
            /// <param name="folderPath">The FOLDER path, this should NOT include any file names</param>
            /// <param name="ask">Asks if you want to generate the folder structure</param>
            /// <returns>False if the user cancels the operation, 
            /// True if there was no need to generate anything or if the operation was successful</returns>
            public static bool GenerateFolderStructureAt(string folderPath, bool ask = true)
            {
                // Convert slashes so we can use the Equals operator together with other file-system operations
                folderPath = folderPath.Replace("/", "\\");
                if (!AssetDatabase.IsValidFolder(folderPath))
                {
                    string existingPath = "Assets";
                    string unknownPath = folderPath.Remove(0, existingPath.Length + 1);
                    // Remove the "Assets/" at the start of the path name
                    string folderName = (unknownPath.Contains("\\")) ? unknownPath.Substring(0, (unknownPath.IndexOf("\\"))) : unknownPath;
                    do
                    {
                        string newPath = System.IO.Path.Combine(existingPath, folderName);
                        // Begin checking down the file path to see if it's valid
                        if (!AssetDatabase.IsValidFolder(newPath))
                        {
                            bool createFolder = true;
                            if (ask)
                            {
                                createFolder = EditorUtility.DisplayDialog("Path does not exist!", "The folder " +
                                                                                                   "\"" +
                                                                                                   newPath +
                                                                                                   "\" does not exist! Would you like to create this folder?", "Yes", "No");
                            }

                            if (createFolder)
                            {
                                AssetDatabase.CreateFolder(existingPath, folderName);
                            }
                            else return false;
                        }

                        existingPath = newPath;
                        // Full path still doesn't exist
                        if (!existingPath.Equals(folderPath))
                        {
                            unknownPath = unknownPath.Remove(0, folderName.Length + 1);
                            folderName = (unknownPath.Contains("\\")) ? unknownPath.Substring(0, (unknownPath.IndexOf("\\"))) : unknownPath;
                        }
                    } while (!AssetDatabase.IsValidFolder(folderPath));
                }

                return true;
            }

            public static void OpenSmartSaveFileDialog<T>(string defaultName = "New Object", string startingPath = "Assets") where T : ScriptableObject
            {
                string savePath = EditorUtility.SaveFilePanel("Designate save path", startingPath, defaultName, "asset");
                if (savePath != "") // Make sure user didn't press "Cancel"
                {
                    var asset = ScriptableObject.CreateInstance<T>();
                    savePath = savePath.Remove(0, savePath.IndexOf("Assets/"));
                    CreateAssetSafe(asset, savePath);
                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = asset;
                }
            }
#endif
        }
    }
}