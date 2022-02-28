using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;
using UnityEngine.Build.Pipeline;
using Wingjoy.Framework.Runtime;

namespace WinjoyFramework.Editor.Addressable
{
    [CreateAssetMenu(fileName = "BuildScriptPackedWithBundleListMode.asset", menuName = "Addressables/Content Builders/BuildScriptPackedWithBundleListMode")]
    public class BuildScriptPackedWithBundleListMode : BuildScriptPackedMode
    {
        public override string Name => "BuildScriptPackedWithBundleListMode";


        private AddressableAssetSettings m_Settings;

        /// <summary>
        /// 内建数据
        /// </summary>
        private BuiltInBundle m_BuiltInBundle;
        protected override TResult BuildDataImplementation<TResult>(AddressablesDataBuilderInput builderInput)
        {
            m_Settings = builderInput.AddressableSettings;
            m_BuiltInBundle = new BuiltInBundle();
            var buildDataImplementation = base.BuildDataImplementation<TResult>(builderInput);
            var json = JsonUtility.ToJson(m_BuiltInBundle);
            File.WriteAllText("Assets/Resources/BuiltInBundle.txt", json);
            return buildDataImplementation;
        }

        protected override string ConstructAssetBundleName(AddressableAssetGroup assetGroup, BundledAssetGroupSchema schema, BundleDetails info, string assetBundleName)
        {
            var constructAssetBundleName = base.ConstructAssetBundleName(assetGroup, schema, info, assetBundleName);
            if (!assetGroup.Default && schema.BuildPath.GetName(m_Settings) == AddressableAssetSettings.kRemoteBuildPath)
            {
                m_BuiltInBundle.BundleNames.Add(constructAssetBundleName);
            }

            return constructAssetBundleName;
        }

        public void OnRemoteBuildPathChanged()
        {
            EditorUtility.SetDirty(this);
        }
    }
}