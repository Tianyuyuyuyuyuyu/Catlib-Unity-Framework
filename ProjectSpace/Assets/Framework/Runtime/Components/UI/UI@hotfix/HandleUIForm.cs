using System;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Framework.Runtime.HotFix.UI
{
    public class HandleUIForm
    {
        /// <summary>
        /// 状态
        /// </summary>
        public UIFromStatus Status;

        /// <summary>
        /// 异步操作句柄
        /// </summary>
        public AsyncOperationHandle AsyncOperationHandle;

        /// <summary>
        /// 窗体
        /// </summary>
        public UIFormBase UIFormBase;

        /// <summary>
        /// 打开方式
        /// </summary>
        public OpenMode OpenMode;

        /// <summary>
        /// 用户数据
        /// </summary>
        public object UserData;
        
        /// <summary>
        /// 窗体
        /// </summary>
        public HandleUIForm SwitchHandleUIForm;

        /// <summary>
        /// 完成事件
        /// </summary>
        public event Action<UIFormBase> Complete;

        /// <summary>
        /// 完成时
        /// </summary>
        /// <param name="obj">窗体数据</param>
        /// <param name="userData">用户自定义数据</param>
        public void OnComplete(UIFormBase obj, object userData)
        {
            UIFormBase = obj;
            UIFormBase.HandleUIForm = this;
            UIFormBase.FormKey = obj.GetType().ToString();
            UIFormBase.OnInit(userData);
            Complete?.Invoke(obj);
            Complete = null;
        }
    }

    public enum UIFromStatus
    {
        None,
        Loading,
        Open,
        Close
    }
}