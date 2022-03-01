#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Utility.Runtime
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
            /// 安全创建资产
            /// </summary>
            /// <param name="asset"></param>
            /// <param name="path"></param>
            /// <returns>被取消或创建不成功则为False, 
            /// 创建成功则为True</returns>
            public static bool CreateAssetSafe(Object asset, string path)
            {
                if (AssetDatabase.IsValidFolder(path))
                {
                    Debug.LogError("错误！试图在文件夹上写入资源!");
                    return false;
                }

                var folderPath = path.Substring(0, path.LastIndexOf("/", StringComparison.Ordinal));

                if (!GenerateFolderStructureAt(folderPath))
                {
                    return false;
                }

                AssetDatabase.CreateAsset(asset, path);
                return true;

            }

            /// <summary>
            /// 如果文件夹结构不存在，则将其生成到指定的路径。
            /// 首先执行检查本身
            /// </summary>
            /// <param name="folderPath">文件夹路径，这应该不包括任何文件名</param>
            /// <param name="ask">询问是否要生成文件夹结构</param>
            /// <returns>如果用户取消操作，则为False, 
            /// 不需要生成任何东西，或者操作成功则为True</returns>
            public static bool GenerateFolderStructureAt(string folderPath, bool ask = true)
            {
                //转换斜杠，这样我们就可以将Equals操作符与其他文件系统操作符一起使用
                folderPath = folderPath.Replace("/", "\\");
                if (!AssetDatabase.IsValidFolder(folderPath))
                {
                    var existingPath = "Assets";

                    //移除路径头的"Assets/"
                    var unknownPath = folderPath.Remove(0, existingPath.Length + 1);

                    var folderName = unknownPath;
                    if(unknownPath.Contains("\\"))
                    {
                        folderName = unknownPath.Substring(0, (unknownPath.IndexOf("\\", StringComparison.Ordinal)));
                    }

                    do
                    {
                        var newPath = System.IO.Path.Combine(existingPath, folderName);
                        // 开始检查文件路径，看看它是否有效
                        if (AssetDatabase.IsValidFolder(newPath) == false)
                        {
                            //询问是否创建文件夹
                            if (ask)
                            {
                                if (EditorUtility.DisplayDialog("路径不存在！",
                                        "文件夹 \"" + newPath + "\" 不存在! 你想创建该文件夹吗?", "Yes", "No"))
                                {
                                    AssetDatabase.CreateFolder(existingPath, folderName);
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                AssetDatabase.CreateFolder(existingPath, folderName);
                            }
                        }

                        existingPath = newPath;

                        // 完整路径仍然不存在
                        if (existingPath.Equals(folderPath) == false)
                        {
                            unknownPath = unknownPath.Remove(0, folderName.Length + 1);
                            folderName = unknownPath;
                            if (unknownPath.Contains("\\"))
                            {
                                folderName = unknownPath.Substring(0,
                                    (unknownPath.IndexOf("\\", StringComparison.Ordinal)));
                            }
                        }
                    } while (AssetDatabase.IsValidFolder(folderPath) == false);
                }

                return true;
            }

            /// <summary>
            /// 打开智能保存文件对话框
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="defaultName"></param>
            /// <param name="startingPath"></param>
            public static void OpenSmartSaveFileDialog<T>(string defaultName = "New Object", string startingPath = "Assets") where T : ScriptableObject
            {
                var savePath = EditorUtility.SaveFilePanel("指定保存路径", startingPath, defaultName, "asset");

                //保证用户没有点击取消
                if (string.IsNullOrEmpty(savePath))
                {
                    return;
                }

                //创建实例
                var asset = ScriptableObject.CreateInstance<T>();

                //保存路径，移除"Assets/"
                savePath = savePath.Remove(0, savePath.IndexOf("Assets/", StringComparison.Ordinal));

                //安全创建资产
                CreateAssetSafe(asset, savePath);

                //将 Project 窗口置于前面并聚焦该窗口。
                //通常，系统调用用于创建和选择资源的菜单项之后，会调用此函数
                EditorUtility.FocusProjectWindow();

                Selection.activeObject = asset;
            }
#endif
        }
    }
}