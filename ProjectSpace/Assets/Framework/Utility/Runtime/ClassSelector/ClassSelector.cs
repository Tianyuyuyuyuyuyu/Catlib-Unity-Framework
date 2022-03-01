#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace Framework.Utility.Runtime
{
    public class ClassSelector<T> : OdinSelector<Type> where T : class
    {
        public ClassSelector()
        {
        }

        public ClassSelector(Action<T> confirmed)
        {
            this.SelectionConfirmed += (enumerable =>
            {
                var firstOrDefault = enumerable.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    if (firstOrDefault.IsSubclassOf(typeof(ScriptableObject)))
                    {
                        string savePath = EditorUtility.SaveFilePanel("创建文件", "Assets", $"New{firstOrDefault.GetNiceName()}", "asset");
                        if (savePath != "") // Make sure user didn't press "Cancel"
                        {
                            AssetDatabase.Refresh();
                            var asset = ScriptableObject.CreateInstance(firstOrDefault);
                            savePath = savePath.Remove(0, savePath.IndexOf("Assets/"));
                            RuntimeUtilities.AssetDatabaseHelper.CreateAssetSafe(asset, savePath);
                            confirmed?.Invoke(asset as T);
                        }
                    }
                    else
                    {
                        confirmed?.Invoke(Activator.CreateInstance(firstOrDefault) as T);
                    }
                }
            });
        }

        protected override void BuildSelectionTree(OdinMenuTree tree)
        {
            IEnumerable<Type> objectTypes = AssemblyUtilities.GetTypes(AssemblyTypeFlags.CustomTypes)
                .Where(x => x.IsClass && !x.IsAbstract && x.InheritsFrom(typeof(T)) && x.GetCustomAttribute<SkipClassSelectorAttribute>() == null);

            tree.Selection.SupportsMultiSelect = false;
            tree.Config.DrawSearchToolbar = true;
            tree.AddRange(objectTypes, x =>
                {
                    var classSelectorLabelAttribute = x.GetCustomAttribute<ClassSelectorLabelAttribute>();
                    if (classSelectorLabelAttribute != null)
                    {
                        return classSelectorLabelAttribute.Label;
                    }

                    var labelTextAttribute = x.GetCustomAttribute<LabelTextAttribute>();
                    if (labelTextAttribute != null)
                    {
                        return labelTextAttribute.Text;
                    }

                    return x.GetNiceName();
                })
                .AddThumbnailIcons();
        }

        /// <summary>
        /// 显示
        /// </summary>
        public void Show()
        {
            if (SelectionTree.EnumerateTree().Count() == 1)
            {
                // If there is only one scriptable object to choose from in the selector, then 
                // we'll automatically select it and confirm the selection. 
                SelectionTree.EnumerateTree().First().Select();
                SelectionTree.Selection.ConfirmSelection();
            }
            else
            {
                // Else, we'll open up the selector in a popup and let the user choose.
                ShowInPopup(200);
            }
        }
    }

    public class ClassSelector : OdinSelector<Type>
    {
        /// <summary>
        /// 基类
        /// </summary>
        private Type m_BaseType;

        public ClassSelector()
        {
        }

        public ClassSelector(Type type, Action<ScriptableObject> confirmed)
        {
            m_BaseType = type;
            this.SelectionConfirmed += (enumerable =>
            {
                var firstOrDefault = enumerable.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    if (firstOrDefault.IsSubclassOf(typeof(ScriptableObject)))
                    {
                        var savePath = EditorUtility.SaveFilePanel("创建文件", "Assets", $"New{firstOrDefault.GetNiceName()}", "asset");

                        //保证用户没有点击取消
                        if (string.IsNullOrEmpty(savePath) == false)
                        {
                            AssetDatabase.Refresh();
                            var asset = ScriptableObject.CreateInstance(firstOrDefault);
                            savePath = savePath.Remove(0, savePath.IndexOf("Assets/"));
                            RuntimeUtilities.AssetDatabaseHelper.CreateAssetSafe(asset, savePath);
                            confirmed?.Invoke(asset);
                        }
                    }
                }
            });
        }

        protected override void BuildSelectionTree(OdinMenuTree tree)
        {
            IEnumerable<Type> objectTypes = AssemblyUtilities.GetTypes(AssemblyTypeFlags.CustomTypes)
                .Where(x => x.IsClass && !x.IsAbstract && x.InheritsFrom(m_BaseType) && x.GetCustomAttribute<SkipClassSelectorAttribute>() == null);

            tree.Selection.SupportsMultiSelect = false;
            tree.Config.DrawSearchToolbar = true;
            tree.AddRange(objectTypes, x =>
                {
                    var classSelectorLabelAttribute = x.GetCustomAttribute<ClassSelectorLabelAttribute>();
                    if (classSelectorLabelAttribute != null)
                    {
                        return classSelectorLabelAttribute.Label;
                    }

                    var labelTextAttribute = x.GetCustomAttribute<LabelTextAttribute>();
                    if (labelTextAttribute != null)
                    {
                        return labelTextAttribute.Text;
                    }

                    return x.GetNiceName();
                })
                .AddThumbnailIcons();
        }

        /// <summary>
        /// 显示
        /// </summary>
        public void Show()
        {
            if (SelectionTree.EnumerateTree().Count() == 1)
            {
                // If there is only one scriptable object to choose from in the selector, then 
                // we'll automatically select it and confirm the selection. 
                SelectionTree.EnumerateTree().First().Select();
                SelectionTree.Selection.ConfirmSelection();
            }
            else
            {
                // Else, we'll open up the selector in a popup and let the user choose.
                ShowInPopup(200);
            }
        }
    }
}
#endif