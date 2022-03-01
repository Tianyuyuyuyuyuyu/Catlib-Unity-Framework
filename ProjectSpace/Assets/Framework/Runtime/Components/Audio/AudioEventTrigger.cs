using System;
using Framework.Runtime.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Framework.Runtime.Audio
{
    public class AudioEventTrigger : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerClickHandler,
        IInitializePotentialDragHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IDropHandler,
        IScrollHandler,
        IUpdateSelectedHandler,
        ISelectHandler,
        IDeselectHandler,
        IMoveHandler,
        ISubmitHandler,
        ICancelHandler
    {
        /// <summary>
        /// 音乐组类型
        /// </summary>
        private AudioGroupType m_AudioGroupType;

        /// <summary>
        /// 事件类型
        /// </summary>
        [SerializeField]
        private EventTriggerType m_EventTriggerType = EventTriggerType.PointerClick;

        /// <summary>
        /// 音频文件
        /// </summary>
        [SerializeField]
        private AudioClipObject m_AudioClipObject;

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="eventTriggerType">事件类型</param>
        /// <param name="eventData">事件数据</param>
        /// <param name="function">行为</param>
        public void Execute<T>(EventTriggerType eventTriggerType, BaseEventData eventData, ExecuteEvents.EventFunction<T> function) where T : IEventSystemHandler
        {
            if (eventTriggerType == m_EventTriggerType)
            {
                switch (m_AudioGroupType)
                {
                    case AudioGroupType.Sound:
                        CoreMain.Audio.PlaySound(m_AudioClipObject);
                        break;
                    case AudioGroupType.Music:
                        CoreMain.Audio.PlayMusic(m_AudioClipObject);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Execute(EventTriggerType.PointerEnter, eventData, ExecuteEvents.pointerEnterHandler);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Execute(EventTriggerType.PointerExit, eventData, ExecuteEvents.pointerExitHandler);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Execute(EventTriggerType.PointerDown, eventData, ExecuteEvents.pointerDownHandler);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Execute(EventTriggerType.PointerUp, eventData, ExecuteEvents.pointerUpHandler);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Execute(EventTriggerType.PointerClick, eventData, ExecuteEvents.pointerClickHandler);
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            Execute(EventTriggerType.InitializePotentialDrag, eventData, ExecuteEvents.initializePotentialDrag);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Execute(EventTriggerType.BeginDrag, eventData, ExecuteEvents.beginDragHandler);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Execute(EventTriggerType.Drag, eventData, ExecuteEvents.dragHandler);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Execute(EventTriggerType.EndDrag, eventData, ExecuteEvents.endDragHandler);
        }

        public void OnDrop(PointerEventData eventData)
        {
            Execute(EventTriggerType.Drop, eventData, ExecuteEvents.dropHandler);
        }

        public void OnScroll(PointerEventData eventData)
        {
            Execute(EventTriggerType.Scroll, eventData, ExecuteEvents.scrollHandler);
        }

        public void OnUpdateSelected(BaseEventData eventData)
        {
            Execute(EventTriggerType.UpdateSelected, eventData, ExecuteEvents.updateSelectedHandler);
        }

        public void OnSelect(BaseEventData eventData)
        {
            Execute(EventTriggerType.Select, eventData, ExecuteEvents.selectHandler);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            Execute(EventTriggerType.Deselect, eventData, ExecuteEvents.deselectHandler);
        }

        public void OnMove(AxisEventData eventData)
        {
            Execute(EventTriggerType.Move, eventData, ExecuteEvents.moveHandler);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            Execute(EventTriggerType.Submit, eventData, ExecuteEvents.submitHandler);
        }

        public void OnCancel(BaseEventData eventData)
        {
            Execute(EventTriggerType.Cancel, eventData, ExecuteEvents.cancelHandler);
        }
    }
}