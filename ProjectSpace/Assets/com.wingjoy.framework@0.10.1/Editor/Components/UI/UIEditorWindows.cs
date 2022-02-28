using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Wingjoy.Framework.Editor
{
    public class UIEditorWindows : OdinMenuEditorWindow
    {
        [MenuItem("Wingjoy/Editor/UIForm")]
        public static void OpenWindow()
        {
            UIEditorWindows window = GetWindow<UIEditorWindows>();
            window.titleContent = new GUIContent("UIEditorWindows");
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree(supportsMultiSelect: true)
            {
                {"UIForm", UIFormCreator.Instance, EditorIcons.Table},
            };

            var customMenuStyle = new OdinMenuStyle
            {
                BorderPadding = 0f,
                AlignTriangleLeft = true,
                TriangleSize = 16f,
                TrianglePadding = 0f,
                Offset = 20f,
                Height = 23,
                IconPadding = 0f,
                BorderAlpha = 0.323f
            };

            tree.DefaultMenuStyle = customMenuStyle;
            tree.Config.DrawScrollView = true;
            tree.Config.DrawSearchToolbar = true;

            return tree;
        }
    }
}
