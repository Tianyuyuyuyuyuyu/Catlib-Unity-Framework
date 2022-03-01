using System;
using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace Framework.Utility.Editor.Windows
{
    public class CreateAssetEditorWindow : OdinEditorWindow
    {
        /// <summary>
        /// 选择的类型
        /// </summary>
        [ValueDropdown("GetTypeList")]
        public Type SelectType;

        public IEnumerable GetTypeList()
        {
            var assemblyTypeFlags = AssemblyTypeFlags.All & ~AssemblyTypeFlags.UnityEditorTypes;
            return AssemblyUtilities
                .GetTypes(assemblyTypeFlags)
                .Where((type => type.IsSubclassOf(typeof(ScriptableObject)) && !type.IsSubclassOf(typeof(EditorWindow))))
                .Select((type => new ValueDropdownItem(type.FullName.Replace(".", "/"), type)));
        }

        [Button(ButtonSizes.Large, Name = "生成")]
        public void Generate()
        {
            if (SelectType == null)
                return;

            var dest = EditorUtility.SaveFilePanel("Save object as", Application.dataPath, "New" + SelectType?.Name, "asset");
            if (!string.IsNullOrEmpty(dest))
            {
                var obj = CreateInstance(SelectType);
                AssetDatabase.CreateAsset(obj, $"Assets{dest.Replace(Application.dataPath, "")}");
                AssetDatabase.Refresh();
            }
        }

        // private void OnGUI()
        // {
        //     List<Type> types = new List<Type>();
        //     foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        //     {
        //         var enumerable = assembly.GetTypes();
        //         foreach (var type in enumerable)
        //         {
        //             if (type.IsSubclassOf(typeof(ScriptableObject)))
        //             {
        //                 types.Add(type);
        //             }
        //         }
        //     }
        //
        //     var displayedOptions = types.Select((type => type.FullName));
        //     var groupBy = displayedOptions.GroupBy((content => content.Split('.')[0]));
        //     var guiContents = groupBy.Select((grouping => new GUIContent(grouping.Key))).ToArray();
        //     m_SelectNameSpaceIndex = EditorGUI.Popup(EditorGUILayout.GetControlRect(), new GUIContent("命名空间"), m_SelectNameSpaceIndex, guiContents);
        //     var nameSpace = guiContents[m_SelectNameSpaceIndex];
        //
        //     var typeArray = displayedOptions.Where((s => s.StartsWith(nameSpace.text))).Select((s => new GUIContent(s))).ToArray();
        //     m_SelectTypeIndex = EditorGUI.Popup(EditorGUILayout.GetControlRect(), new GUIContent("类型"), m_SelectTypeIndex, typeArray);
        //     
        //     
        //     if(GUILayout.Button("生成"))
        //     {
        //         var type = typeArray[m_SelectTypeIndex];
        //         foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        //         {
        //             var selectType = assembly.GetType(type.text);
        //             if (selectType != null)
        //             {
        //                 var dest = EditorUtility.SaveFilePanel("Save object as", Application.dataPath, "New" + selectType?.Name, "asset");
        //                 if (!string.IsNullOrEmpty(dest))
        //                 {
        //                     var obj = CreateInstance(selectType);
        //                     AssetDatabase.CreateAsset(obj, $"Assets{dest.Replace(Application.dataPath, "")}");
        //                     AssetDatabase.Refresh();
        //                 }
        //             }
        //         }
        //     }
        // }
    }
}