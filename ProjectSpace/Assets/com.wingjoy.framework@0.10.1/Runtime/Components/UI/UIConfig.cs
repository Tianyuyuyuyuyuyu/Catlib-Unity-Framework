using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Wingjoy.Framework.Runtime.Base;

namespace Wingjoy.Framework.Runtime.UI
{
    public class UIConfig : ComponentConfig
    {
        /// <summary>
        /// 背景遮罩预制件
        /// </summary>
        [SerializeField]
        private AssetReferenceGameObject m_MaskUIFormPrefab;

        /// <summary>
        /// 根节点
        /// </summary>
        [SerializeField]
        private Transform m_Root;

        /// <summary>
        /// 待机节点
        /// </summary>
        [SerializeField]
        private Transform m_StandBy;

        /// <summary>
        /// 预制件路径
        /// </summary>
        [SerializeField, FolderPath]
        private string m_PrefabPath = "Assets/Prefab/UIForms";

        /// <summary>
        /// 背景遮罩预制件
        /// </summary>
        public AssetReferenceGameObject MaskUiFormPrefab => m_MaskUIFormPrefab;

        /// <summary>
        /// 根节点
        /// </summary>
        public Transform Root => m_Root;

        /// <summary>
        /// 待机节点
        /// </summary>
        public Transform StandBy => m_StandBy;

        /// <summary>
        /// 预制件路径
        /// </summary>
        public string PrefabPath => m_PrefabPath;
    }
}