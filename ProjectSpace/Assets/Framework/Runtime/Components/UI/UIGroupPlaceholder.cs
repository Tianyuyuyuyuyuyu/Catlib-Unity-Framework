using UnityEngine;

namespace Framework.Runtime.UI
{
    public class UIGroupPlaceholder : UIPlaceholder
    {
        /// <summary>
        /// 预制件
        /// </summary>
        [SerializeField]
        private bool m_Prefab;

        public bool Prefab => m_Prefab;

        public override bool NeedShow()
        {
            return m_Prefab;
        }
    }
}