using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CCGP.Client
{
    public class MouseInputProvider : MonoBehaviour, IMouseInput
    {
        #region Fields and Properties
        private Vector3 oldPosition;
        public Vector2 MousePosition => Input.mousePosition;
        public DragDirection DragDirection => GetDragDirection();
        #endregion

        #region Unity Callbacks
        //private void Awake()
        //{
        //    if (Camera.main.GetComponent<Physics2DRaycaster>() == null)
        //        throw new Exception(GetType() + " needs an " + typeof(Physics2DRaycaster) + " on the MainCamera");
        //}
        #endregion

        #region IMouseInput
        public Action<PointerEventData> OnPointerEnter { get; set; } = eventData => { };
        public Action<PointerEventData> OnPointerExit { get; set; } = eventData => { };
        public Action<PointerEventData> OnPointerUp { get; set; } = eventData => { };
        public Action<PointerEventData> OnPointerDown { get; set; } = eventData => { };
        public Action<PointerEventData> OnPointerClick { get; set; } = eventData => { };
        public Action<PointerEventData> OnBeginDrag { get; set; } = eventData => { };
        public Action<PointerEventData> OnDrag { get; set; } = eventData => { };
        public Action<PointerEventData> OnEndDrag { get; set; } = eventData => { };
        public Action<PointerEventData> OnDrop { get; set; } = eventData => { };

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            ((IMouseInput)this).OnPointerEnter.Invoke(eventData);
        }
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            ((IMouseInput)this).OnPointerExit.Invoke(eventData);
        }
        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            ((IMouseInput)this).OnPointerUp.Invoke(eventData);
        }
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            ((IMouseInput)this).OnPointerDown.Invoke(eventData);
        }
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            ((IMouseInput)this).OnPointerClick.Invoke(eventData);
        }
        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            ((IMouseInput)this).OnBeginDrag.Invoke(eventData);
        }
        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            ((IMouseInput)this).OnDrag.Invoke(eventData);
        }
        void IDropHandler.OnDrop(PointerEventData eventData)
        {
            ((IMouseInput)this).OnDrop.Invoke(eventData);
        }
        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            ((IMouseInput)this).OnEndDrag.Invoke(eventData);
        }
        #endregion

        #region Utils

        private DragDirection GetDragDirection()
        {
            var currentPosition = Input.mousePosition;
            var normalized = (currentPosition - oldPosition).normalized;

            oldPosition = currentPosition;

            if (normalized.x > 0) return DragDirection.Right;
            if (normalized.x < 0) return DragDirection.Left;
            if (normalized.y > 0) return DragDirection.Up;
            if (normalized.y < 0) return DragDirection.Down;

            return DragDirection.None;
        }

        #endregion
    }
}