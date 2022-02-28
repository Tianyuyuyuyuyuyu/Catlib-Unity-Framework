using System;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace WinjoyFramework.Editor.QuickGuide
{
    public class FrameworkGuideWindows : OdinEditorWindow
    {
        /// <summary>
        /// 是否初始化样式
        /// </summary>
        private bool m_HasInitStyle;

        private static GUIStyle s_titleStyle;

        private Vector2 m_ScrollPosition = Vector2.zero;

        private static Texture s_webIcon; //= EditorGUIUtility.IconContent( "BuildSettings.Web.Small" ).image;

        //btn样式
        private static GUIContent s_wikiBtnContent; // = new GUIContent( " 中文Wiki", webIcon );

        private string m_FrameUpdateNote = "正在获取...";

        [MenuItem("Wingjoy/Guide", false, -1)]
        public static void Open()
        {
            var win = GetWindow<FrameworkGuideWindows>("WingjoyFramework Guide");
            win.minSize = win.maxSize = new Vector2(400, 400);
            win.Show();
        }

        protected override void Initialize()
        {
            base.Initialize();
            CheckUpdateLog();
        }

        public void InitStyle()
        {
            if (!m_HasInitStyle)
            {
                m_HasInitStyle = true;

                s_titleStyle = new GUIStyle("BoldLabel")
                {
                    margin = new RectOffset(4, 4, 4, 4), padding = new RectOffset(2, 2, 2, 2), fontSize = 13
                };


                //初始化各种样式
                s_webIcon = EditorGUIUtility.IconContent("BuildSettings.Web.Small").image;
                s_wikiBtnContent = new GUIContent(" 框架Wiki", s_webIcon);
            }
        }

        protected override void OnGUI()
        {
            InitStyle();
            if (GUILayout.Button(s_wikiBtnContent, GUILayout.Height(30)))
            {
                Application.OpenURL("http://gitealocal.wingjoy.cn/SVNBuildGroup/WingjoyFramework/wiki");
            }

            SirenixEditorGUI.Title("热更", string.Empty, TextAlignment.Left, true);

            if (GUILayout.Button("导入热更依赖", GUILayout.Height(50)))
            {
                AssetDatabase.ImportPackage("Packages/com.wingjoy.framework/Asset/UnityPackage/ILRuntime.unitypackage", true);
                PlayerSettings.assemblyVersionValidation = false;
            }

            GUILayout.Space(10);
            SirenixEditorGUI.Title("资源管理", string.Empty, TextAlignment.Left, true);

            var existAddressables = UnityPackageUtility.HasPackageInstalled("com.unity.addressables", new Version(1, 17, 0));
            var existAddressablesCn = UnityPackageUtility.HasPackageInstalled("com.unity.addressables.cn", new Version(1, 17, 0));
            if (!existAddressables && !existAddressablesCn)
            {
                GUILayout.Label("未导入addressables");
            }
            else
            {
                GUILayout.Label("addressables 已导入");
            }

            GUILayout.Space(10);
            SirenixEditorGUI.Title("更新日志", string.Empty, TextAlignment.Left, true);
            m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition, "ProgressBarBack",
                GUILayout.Height(200), GUILayout.ExpandWidth(true));
            GUILayout.Label(m_FrameUpdateNote, "WordWrappedMiniLabel", GUILayout.ExpandHeight(true));
            GUILayout.EndScrollView();
        }

        public void CheckUpdateLog()
        {
            var path = AssetDatabase.GUIDToAssetPath("ea150cce1419ff849a9c92a9a8953856");
            var localNote = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            m_FrameUpdateNote = localNote.text;
        }
    }
}