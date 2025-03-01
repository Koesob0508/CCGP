using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CCGP.Client
{
    public enum DragDirection
    {
        None,
        Down,
        Left,
        Up,
        Right
    }

    public interface IMouseInput :
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerClickHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IDropHandler
    {
        Vector2 MousePosition { get; }
        DragDirection DragDirection { get; }

        // Enter & Exit
        new Action<PointerEventData> OnPointerEnter { get; set; }
        new Action<PointerEventData> OnPointerExit { get; set; }

        // Click
        new Action<PointerEventData> OnPointerDown { get; set; }
        new Action<PointerEventData> OnPointerUp { get; set; }
        new Action<PointerEventData> OnPointerClick { get; set; }

        // Drag & Drop
        new Action<PointerEventData> OnBeginDrag { get; set; }
        new Action<PointerEventData> OnDrag { get; set; }
        new Action<PointerEventData> OnEndDrag { get; set; }
        new Action<PointerEventData> OnDrop { get; set; }
    }
}