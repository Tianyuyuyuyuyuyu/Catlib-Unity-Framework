using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Wingjoy;
using WingjoyUtility.Runtime;

namespace WinjoyFramework.Editor.Inspector
{
    [CustomEditor(typeof(Launcher))]
    public class LauncherEditor : OdinEditor
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (GUILayout.Button("CopyRemoteBuildToLibrary"))
            {
                var addressableAssetSettings = AddressableAssetSettingsDefaultObject.Settings;
                var profileSettings = addressableAssetSettings.profileSettings;
                var activeProfileId = addressableAssetSettings.activeProfileId;
                var remoteBuildPath = profileSettings.EvaluateString(activeProfileId, profileSettings.GetValueByName(activeProfileId, "RemoteBuildPath"));
                RuntimeUtilities.File.CopyFileOrDirectory(remoteBuildPath, $"{Addressables.RuntimePath}/RemoteBundle", new string[0]);
            }
        }
    }
}