using System;

namespace AdsPlatform.Runtime
{
    public class InitEvent
    {
        /// <summary>
        /// 初始化结果
        /// </summary>
        public event Action<bool> InitResult;

        /// <summary>
        /// 初始化事件结果
        /// </summary>
        /// <param name="obj">是否成功</param>
        public virtual void OnInitResult(bool obj)
        {
            InitResult?.Invoke(obj);
        }
    }
}
