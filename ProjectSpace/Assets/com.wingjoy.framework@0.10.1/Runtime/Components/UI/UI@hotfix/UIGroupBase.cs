using System;
using UnityEngine;

namespace Wingjoy.Framework.Runtime.HotFix.UI
{
    public abstract class UIGroupBase : UIBase
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="userData">自定义数据</param>
        public override void OnInit(object userData)
        {
            base.OnInit(userData);
            InitSubUIGroup(userData);
        }

        /// <summary>
        /// 界面打开
        /// </summary>
        /// <param name="userData">自定义数据</param>
        public virtual void OnFormOpen(object userData)
        {
            
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            UpdateSubUIGroup();
        }

        /// <summary>
        /// 打开
        /// </summary>
        /// <param name="userData">自定义数据</param>
        public virtual void Open(object userData = null)
        {
            GameObject.SetActive(true);
            OnOpen(userData);
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="userData">自定义数据</param>
        /// <param name="closeComplete">关闭成功</param>
        public virtual void Close(object userData = null, Action closeComplete = null)
        {
            GameObject.SetActive(false);
            closeComplete?.Invoke();
            OnClose(userData);
        }

        /// <summary>
        /// 打开回调
        /// </summary>
        /// <param name="userData">自定义数据</param>
        public virtual void OnOpen(object userData)
        {
            
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="userData">自定义数据</param>
        public virtual void OnClose(object userData)
        {
            
        }
    }
}