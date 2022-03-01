namespace Framework.Runtime.UI
{
    public class UIObject<T> : WingjoyMonoBehaviour
    {
        /// <summary>
        /// 值
        /// </summary>
        private T m_Value;

        /// <summary>
        /// 值
        /// </summary>
        protected T Value => m_Value;

        /// <summary>
        /// 初始化UI物体
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="userData">自定义数据</param>
        public virtual void Init(T value, object userData = null)
        {
            m_Value = value;
            Refresh();
        }

        /// <summary>
        /// 刷新UI物体
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="userData">自定义数据</param>
        public virtual void UpdateData(T value, object userData = null)
        {
            m_Value = value;
            Refresh();
        }

        /// <summary>
        /// 刷新物体
        /// </summary>
        public virtual void Refresh()
        {

        }
    }
}