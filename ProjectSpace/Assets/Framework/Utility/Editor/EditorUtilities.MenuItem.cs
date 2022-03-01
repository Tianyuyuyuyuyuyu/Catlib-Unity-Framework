using System.IO;
using Framework.Utility.Editor.Windows;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Utility.Editor
{
    public static partial class EditorUtilities
    {
        public static class MenuItem
        {
            [UnityEditor.MenuItem("Tools/Create Asset")]
            public static void CreateAsset()
            {
                var createAssetEditorWindow = EditorWindow.GetWindow<CreateAssetEditorWindow>();
                createAssetEditorWindow.Show();
                var position = createAssetEditorWindow.position;
                position.width = 400;
                position.height = 200;
                createAssetEditorWindow.position = position;
            }
            
            [UnityEditor.MenuItem("Tools/Path/persistentDataPath")]
            public static void OpenPersistentDataPathDirectory()
            {
                string output = Application.persistentDataPath;
                if (!Directory.Exists(output))
                {
                    Directory.CreateDirectory(output);
                }
                output = output.Replace("/", "\\");
                System.Diagnostics.Process.Start("explorer.exe", output);
            }

            [UnityEditor.MenuItem("Tools/Path/dataPath")]
            public static void OpenDataPathDirectory()
            {
                string output = Application.dataPath;
                if (!Directory.Exists(output))
                {
                    Directory.CreateDirectory(output);
                }
                output = output.Replace("/", "\\");
                System.Diagnostics.Process.Start("explorer.exe", output);
            }

            [UnityEditor.MenuItem("Tools/Path/streamingAssetsPath")]
            public static void OpenStreamingAssetsPathDirectory()
            {
                string output = Application.streamingAssetsPath;
                if (!Directory.Exists(output))
                {
                    Directory.CreateDirectory(output);
                }
                output = output.Replace("/", "\\");
                System.Diagnostics.Process.Start("explorer.exe", output);
            }
        }
        
        [UnityEditor.MenuItem("Tools/OpenAssetStore")]
        public static void OpenAssetStore()
        {
            AssetStore.Open("");
        }
        
        [UnityEditor.MenuItem("Tools/CaptureScreenShot")]
        public static void CaptureScreenShot()
        {
            ScreenCapture.CaptureScreenshot(Application.dataPath + "/ScreenShot.png", 0);
        }

        [UnityEditor.MenuItem("Tools/ClearRaycastTarget")]
        public static void ClearRaycastTarget()
        {
            foreach (var gameObject in Selection.gameObjects)
            {
                gameObject.GetComponent<MaskableGraphic>().raycastTarget = false;
            }
        }
    }
}
