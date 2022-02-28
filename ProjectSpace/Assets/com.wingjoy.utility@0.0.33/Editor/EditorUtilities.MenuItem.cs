﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using WingjoyUtility.Runtime;

namespace WingjoyUtility.Editor
{
    public static partial class EditorUtilities
    {
        public static class MenuItem
        {
            [UnityEditor.MenuItem("Wingjoy/Tools/Create Asset")]
            public static void CreateAsset()
            {
                var createAssetEditorWindow = EditorWindow.GetWindow<CreateAssetEditorWindow>();
                createAssetEditorWindow.Show();
                var position = createAssetEditorWindow.position;
                position.width = 400;
                position.height = 200;
                createAssetEditorWindow.position = position;
            }
            
            // [UnityEditor.MenuItem("Assets/Create/ScriptableObject")]
            // public static void CreateScriptableObject()
            // {
            //     // var assemblyTypeFlags = AssemblyTypeFlags.All & ~AssemblyTypeFlags.UnityEditorTypes;
            //     // var items = AssemblyUtilities
            //     //     .GetTypes(assemblyTypeFlags)
            //     //     .Where((type => type.IsSubclassOf(typeof(ScriptableObject)) && !type.IsSubclassOf(typeof(EditorWindow))));
            //         //.Select((type => new ValueDropdownItem(type.FullName.Replace(".", "/"), type)));
            //         ClassSelector<ScriptableObject> classSelector = new ClassSelector<ScriptableObject>((o =>
            //         {
            //
            //         }));
            //         classSelector.ShowInPopup();
            //     // var createAssetEditorWindow = EditorWindow.GetWindow<CreateAssetEditorWindow>();
            //     // createAssetEditorWindow.Show();
            //     // var position = createAssetEditorWindow.position;
            //     // position.width = 400;
            //     // position.height = 200;
            //     // createAssetEditorWindow.position = position;
            // }

            [UnityEditor.MenuItem("Wingjoy/Tools/Publish UPM %F4")]
            public static void PublishUPM()
            {
                var searchFile = Runtime.RuntimeUtilities.File.SearchFilePath("t:TextAsset", new[] {"Assets"});

                string packageFile = string.Empty;
                string projectName = string.Empty;
                foreach (var path in searchFile)
                {
                    var fileName = Path.GetFileName(path);
                    if (fileName == "package.json")
                    {
                        var directoryName = Path.GetDirectoryName(path);
                        var strings = directoryName.Split('\\');
                        var s = strings.LastOrDefault();
                        if (!string.IsNullOrEmpty(s))
                        {
                            packageFile = path;
                            projectName = s;
                            break;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(projectName))
                {
                    var easyPackageInfo = JsonUtility.FromJson<EasyPackageInfo>(File.ReadAllText(packageFile));

                    if (EditorUtility.DisplayDialog("发布包", $"是否要发布{projectName}\n{easyPackageInfo}","确定","取消"))
                    {
                        string batPath = $"Packages/com.wingjoy.utility/Assets/Publish.bat";
                        if (!File.Exists(batPath))
                        {
                            batPath = $"{Application.dataPath}/Utility/Assets/Publish.bat";
                        }
                        else
                        {
                            batPath = Path.GetFullPath(batPath);
                        }

                        ProcessStartInfo info = new ProcessStartInfo(batPath);
                        info.Arguments = $"{projectName} {easyPackageInfo.version}";

                        Process process = Process.Start(info);
                        process.WaitForExit();
                    }
                }
            }
            
            [UnityEditor.MenuItem("Wingjoy/Tools/Path/persistentDataPath")]
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

            [UnityEditor.MenuItem("Wingjoy/Tools/Path/dataPath")]
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

            [UnityEditor.MenuItem("Wingjoy/Tools/Path/streamingAssetsPath")]
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
        
        [UnityEditor.MenuItem("Wingjoy/Tools/OpenAssetStore")]
        public static void OpenAssetStore()
        {
            AssetStore.Open("");
        }
        
        [UnityEditor.MenuItem("Wingjoy/Tools/CaptureScreenShot")]
        public static void CaptureScreenShot()
        {
            ScreenCapture.CaptureScreenshot(Application.dataPath + "/ScreenShot.png", 0);
        }

        [UnityEditor.MenuItem("Wingjoy/Tools/ClearRaycastTarget")]
        public static void ClearRaycastTarget()
        {
            foreach (var gameObject in Selection.gameObjects)
            {
                gameObject.GetComponent<MaskableGraphic>().raycastTarget = false;
            }
        }
    }
}
