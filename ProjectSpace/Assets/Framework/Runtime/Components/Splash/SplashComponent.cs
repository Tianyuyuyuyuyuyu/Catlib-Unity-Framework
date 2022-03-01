using System;
using UnityEngine;

namespace Framework.Runtime.Splash
{
    public class SplashComponent : FrameworkComponent
    {
        /// <summary>
        /// 画布
        /// </summary>
        private Canvas m_Canvas;

        /// <summary>
        /// 隐藏回调
        /// </summary>
        private Action m_OnHide;

        /// <summary>
        /// 动画是否播放完成
        /// </summary>
        private bool m_IsAnimationComplete;

        /// <summary>
        /// 是否已经呼叫隐藏
        /// </summary>
        private bool m_IsCallHide;

        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool IsHide;


        protected void Awake()
        {
            m_Canvas = GetComponent<Canvas>();
            var animator = GetComponent<Animator>();

            if (!false)
            {
                animator.Play("SplashWithoutAnnouncement");
            }
            else
            {
                animator.Play("Splash");
            }
            
            m_Canvas.enabled = true;
        }

        /// <summary>
        /// 完成动画
        /// </summary>
        public void AnimationComplete()
        {
            m_IsAnimationComplete = true;
            if (m_IsCallHide)
            {
                ReallyHide();
            }
        }

        /// <summary>
        /// 隐藏闪屏
        /// </summary>
        public void HideSplash()
        {
            m_IsCallHide = true;
            if (m_IsAnimationComplete)
            {
                ReallyHide();
            }
        }

        /// <summary>
        /// 添加隐藏事件
        /// </summary>
        /// <param name="action">事件</param>
        public void AddHideEvent(Action action)
        {
            if (IsHide)
            {
                action?.Invoke();
                return;
            }

            m_OnHide += action;
        }

        /// <summary>
        /// 真隐藏
        /// </summary>
        private void ReallyHide()
        {
            m_Canvas.enabled = false;
            m_OnHide?.Invoke();
            IsHide = true;
        }
    }
}