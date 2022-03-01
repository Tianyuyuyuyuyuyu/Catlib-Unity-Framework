using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
#endif
using UnityEngine;

namespace Framework.Runtime.UI
{
    public abstract class UIPlaceholder : MonoBehaviour
    {
        /// <summary>
        /// 唯一标识符
        /// </summary>
        [SerializeField, ReadOnly]
        private string m_Guid = System.Guid.NewGuid().ToString();

        /// <summary>
        /// UIGroup类名
        /// </summary>
        [SerializeField, ShowIf("NeedShow")]
        private string m_RuntimeUIGroupType;

        /// <summary>
        /// 唯一标识符
        /// </summary>
        public string Guid => m_Guid;

        /// <summary>
        /// UIGroup类名
        /// </summary>
        public string RuntimeUiGroupType => m_RuntimeUIGroupType;

        /// <summary>
        /// 设置运行时要被替换的UIGroup类型
        /// </summary>
        public void SetRuntimeUIGroupType(string type)
        {
            m_RuntimeUIGroupType = type;
        }

        /// <summary>
        /// 是否需要显示
        /// </summary>
        /// <returns>是否需要显示</returns>
        public virtual bool NeedShow()
        {
            return false;
        }


#if UNITY_EDITOR
        [OnInspectorGUI]
        public void OnInspectorGUI()
        {
            if (!NeedShow())
            {
                SirenixEditorGUI.InfoMessageBox(m_RuntimeUIGroupType, false);                
            }
        }
#endif
    }
}