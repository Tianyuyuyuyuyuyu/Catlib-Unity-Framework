using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;

namespace Framework.Editor.Localization
{
    public class LocalizationEditorWindow : OdinMenuEditorWindow
    {
        [MenuItem("Framework/Editor/Localization")]
        private static void OpenWindow()
        {
            var window = GetWindow<LocalizationEditorWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
        }

        /// <summary>Builds the menu tree.</summary>
        protected override OdinMenuTree BuildMenuTree()
        {
            TranslatorOverview.Instance.SearchAllTranslators();
            OdinMenuTree tree = new OdinMenuTree(supportsMultiSelect: false)
            {
                {"翻译文本", LocalizationText.Instance, EditorIcons.Paperclip},
                {"翻译文件", LocalizationFile.Instance, EditorIcons.File},
                {"翻译资产", LocalizationAssets.Instance, EditorIcons.ShoppingBasket},
                {"脚本本地化", LocalizationScriptFile.Instance, EditorIcons.Folder},
                {"场景本地化", LocalizationScene.Instance, EditorIcons.Airplane},
                {"Excel", null, EditorIcons.FileCabinet},
                {"Excel/导出", ExportNotTranslated.Instance, EditorIcons.FileCabinet},
                {"Excel/合并", MergeExcel.Instance, EditorIcons.FileCabinet},
                {"Excel/翻译", TranslatedExcel.Instance, EditorIcons.FileCabinet},
                {"翻译结果库", TranslationDatabase.Instance, EditorIcons.FileCabinet},
                {"本地化设置", LocalizationSetting.Instance, EditorIcons.SettingsCog},
                {"翻译工具", TranslatorOverview.Instance, EditorIcons.Globe},
                {"翻译工具/谷歌翻译", GoogleTranslate.Instance, EditorIcons.Globe},
                {"翻译工具/百度翻译", BaiduTranslate.Instance, EditorIcons.Globe},
                {"翻译工具/Tmxmall翻译", TmxmallTranslate.Instance, EditorIcons.Globe},
            };

            var customMenuStyle = new OdinMenuStyle
            {
                BorderPadding = 0f,
                AlignTriangleLeft = true,
                TriangleSize = 16f,
                TrianglePadding = 0f,
                Offset = 20f,
                Height = 25,
                IconPadding = 0f,
                IconSize = 20,
                BorderAlpha = 0.323f
            };

            tree.DefaultMenuStyle = customMenuStyle;
            tree.Config.DrawScrollView = true;
            tree.Config.DrawSearchToolbar = true;

            tree.Selection.SelectionChanged += type =>
            {
                if (type == SelectionChangedType.ItemAdded)
                {
                    if (ReferenceEquals(tree.Selection.SelectedValue, TranslationDatabase.Instance))
                    {
                        TranslationDatabase.Instance.EnableCheck();
                    }
                }
            };

            return tree;
        }
    }
}