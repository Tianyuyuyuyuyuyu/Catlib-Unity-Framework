using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.UI;
using Wingjoy.Framework.Runtime.UI;

namespace Wingjoy.Framework.Runtime.HotFix.UI
{
    [UIFormBase]
    public abstract class UIFormBase : UIBase, IDisposable
    {
        /// <summary>
        /// 画布
        /// </summary>
        private Canvas m_Canvas;

        /// <summary>
        /// 画布句柄
        /// </summary>
        private HandleUIForm m_HandleUIForm;

        /// <summary>
        /// 窗体键值
        /// </summary>
        [NonSerialized]
        public string FormKey;

        /// <summary>
        /// 适配安全区域
        /// </summary>
        [BoxGroup("窗体参数")]
        [SerializeField]
        private bool m_NeedMask;

        /// <summary>
        /// 锁住后将不能被隐藏
        /// </summary>
        private bool m_Lock;

        public Canvas Canvas => m_Canvas;

        public HandleUIForm HandleUIForm
        {
            get => m_HandleUIForm;
            set => m_HandleUIForm = value;
        }

        public bool NeedMask
        {
            get => m_NeedMask;
            set => m_NeedMask = value;
        }

        public bool Lock
        {
            get => m_Lock;
            set => m_Lock = value;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="userData">自定义数据</param>
        public override void OnInit(object userData)
        {
            base.OnInit(userData);
            
            m_Canvas = GameObject.GetComponent<Canvas>();
            m_Canvas.overrideSorting = true;
            
            if (GameObject.GetComponent<GraphicRaycaster>() == null)
            {
                GameObject.AddComponent<GraphicRaycaster>();    
            }

            InitSubUIGroup(userData);
        }
        
        public override void OnUpdate()
        {
            base.OnUpdate();
            UpdateSubUIGroup();
        }

        /// <summary>
        /// 打开回调
        /// </summary>
        /// <param name="userData">自定义数据</param>
        public virtual void OnOpen(object userData)
        {
            SetActive(true);
            OpenSubUIGroup(userData);
        }

        /// <summary>
        /// 重新打开回调
        /// </summary>
        /// <param name="userData">自定义数据</param>
        public virtual void OnReopen(object userData)
        {
            OnOpen(userData);
        }

        /// <summary>
        /// 刷新UIFormBase
        /// </summary>
        /// <param name="userData">自定义数据</param>
        public virtual void Refresh(object userData)
        {
            
        }

        /// <summary>
        /// 设置显影
        /// </summary>
        /// <param name="active">显影</param>
        public virtual void SetActive(bool active)
        {
            GameObject.SetActive(active);
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="userData">自定义数据</param>
        /// <param name="closeComplete">关闭成功</param>
        public virtual void OnClose(object userData, Action closeComplete)
        {
            SetActive(false);
            closeComplete?.Invoke();
        }

        /// <summary>
        /// UI排序变更
        /// </summary>
        /// <param name="order">排序顺序</param>
        public void SortOrderChange(int order)
        {
            var canvasSortingOrder = order * 100;
            m_Canvas.sortingOrder = canvasSortingOrder;
            OnSortOrderChanged(canvasSortingOrder);
        }

        /// <summary>
        /// UI排序变化
        /// </summary>
        /// <param name="sortOrder">排序顺序</param>
        public virtual void OnSortOrderChanged(int sortOrder)
        {

        }

        /// <summary>
        /// 销毁释放
        /// </summary>
        public virtual void OnDispose()
        {
        }

        /// <summary>
        /// 销毁逻辑
        /// </summary>
        public void Dispose()
        {
            Release();
            OnDispose();
        }
    }
}