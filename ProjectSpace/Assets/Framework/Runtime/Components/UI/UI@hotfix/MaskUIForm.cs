using UnityEngine;
using UnityEngine.UI;

namespace Framework.Runtime.HotFix.UI
{
    public class MaskUIForm : UIFormBase
    {
        /// <summary>
        /// 背景板图片
        /// </summary>
        [SerializeField]
        private Image m_Background;

        /// <summary>
        /// 设置颜色，遮挡使能
        /// </summary>
        /// <param name="color">颜色</param>
        /// <param name="rayCast">遮挡使能</param>
        public void SetColor(Color color, bool rayCast)
        {
            m_Background.color = color;
            m_Background.raycastTarget = rayCast;
        }
    }
}