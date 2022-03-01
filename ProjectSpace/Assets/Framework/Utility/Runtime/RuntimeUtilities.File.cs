using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework.Utility.Runtime
{
    public static partial class RuntimeUtilities
    {
        public static class File
        {
            /// <summary>
            /// 检查文件夹，如果不存在则自动创建
            /// </summary>
            /// <param name="path">路径</param>
            public static void CheckDirectory(string path)
            {
                var directoryName = System.IO.Path.GetDirectoryName(path);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName ?? string.Empty);
                }
            }


            /// <summary>
            /// 拷贝文件或文件夹
            /// </summary>
            /// <param name="srcPath">源地址</param>
            /// <param name="dstPath">目标地址</param>
            /// <param name="excludeExtensions">剔除扩展名</param>
            /// <param name="overwrite">是否覆盖目标文件</param>
            public static void CopyFileOrDirectory(string srcPath, string dstPath, string[] excludeExtensions, bool overwrite = true)
            {
                if (!Directory.Exists(dstPath))
                    Directory.CreateDirectory(dstPath);

                foreach (var file in Directory.GetFiles(srcPath, "*.*", SearchOption.TopDirectoryOnly).Where(path => excludeExtensions == null || !excludeExtensions.Contains(System.IO.Path.GetExtension(path))))
                {
                    System.IO.File.Copy(file, System.IO.Path.Combine(dstPath, System.IO.Path.GetFileName(file) ?? throw new InvalidOperationException()), overwrite);
                }

                foreach (var dir in Directory.GetDirectories(srcPath))
                    CopyFileOrDirectory(dir, System.IO.Path.Combine(dstPath, System.IO.Path.GetFileName(dir) ?? throw new InvalidOperationException()), excludeExtensions, overwrite);
            }

#if UNITY_EDITOR
            public static List<T> SearchFile<T>(string filter, string[] searchInFolders) where T : UnityEngine.Object
            {
                var findAssets = AssetDatabase.FindAssets(filter, searchInFolders);
                List<string> assetPathList = new List<string>();
                foreach (var findAsset in findAssets)
                {
                    assetPathList.Add(AssetDatabase.GUIDToAssetPath(findAsset));
                }

                return assetPathList.Select((AssetDatabase.LoadAssetAtPath<T>)).ToList();
            }

            public static List<string> SearchFilePath(string filter, string[] searchInFolders)
            {
                var findAssets = AssetDatabase.FindAssets(filter, searchInFolders);
                List<string> assetPathList = new List<string>();
                foreach (var findAsset in findAssets)
                {
                    assetPathList.Add(AssetDatabase.GUIDToAssetPath(findAsset));
                }

                return assetPathList;
            }
#endif
        }
    }
}
