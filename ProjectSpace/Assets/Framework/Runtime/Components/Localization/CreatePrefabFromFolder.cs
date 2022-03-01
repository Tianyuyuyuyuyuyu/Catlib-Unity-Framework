using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.IO;
using UnityEditor;
using UnityEngine;
using Framework.Utility.Runtime;

namespace Framework.Runtime.Localization
{
    public class CreatePrefabFromFolder : MonoBehaviour
    {
        /// <summary>
        /// 前缀
        /// </summary>
        public string Prefix = "Assets\\";
        /// <summary>
        /// 文件夹
        /// </summary>
        [FolderPath]
        public List<string> Folder;

#if UNITY_EDITOR
        /// <summary>
        /// 生成预制件
        /// </summary>
        [Button(ButtonSizes.Large)]
        public void GeneratePrefab()
        {
            var searchFilePath = RuntimeUtilities.File.SearchFilePath("t:Prefab", Folder.ToArray());
            foreach (var s in searchFilePath)
            {
                var directoryName = Path.GetDirectoryName(s);
                if (directoryName.StartsWith(Prefix))
                {
                    directoryName = directoryName.Remove(0, Prefix.Length);
                }
                //directoryName = directoryName.Remove(0, Folder.Length);
                CreateTransform(transform, directoryName);
                //var find = transform.Find(directoryName);
                
                var loadAssetAtPath = AssetDatabase.LoadAssetAtPath<GameObject>(s);
                var replace = directoryName.Replace("\\","/");
                PrefabUtility.InstantiatePrefab(loadAssetAtPath, transform.Find(replace)?.transform);
            }

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);
                child.SetParent(null);
            }
        }
#endif
        /// <summary>
        /// 创建父级
        /// </summary>
        /// <param name="parent">父级</param>
        /// <param name="path">路径</param>
        public void CreateTransform(Transform parent, string path)
        {
            var strings = path.Split('\\');
            for (int i = 0; i < strings.Length; )
            {
                var s = strings[i];
                Transform find;
                if (parent == null)
                {
                    find = transform.Find(s)?.transform;
                }
                else
                {
                    find = parent.Find(s);
                }
                
                if (find == null)
                {
                    find = new GameObject(s).transform;
                    find.SetParent(parent);
                }

                if(s.Length == path.Length)
                    return;
                
                var sLength = (s.Length + 1);//+1 是字符‘\\’
                var substring = path.Substring(sLength, path.Length - sLength);
                if (substring.Length == 0)
                {
                    return;
                }
                else
                {
                    CreateTransform(find, substring);
                    break;
                }
            }
        }
    }
}