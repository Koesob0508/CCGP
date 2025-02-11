using CCGP.Shared;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CCGP.Client
{
    public class CardViewIdle : BaseCardViewState
    {
        Vector3 DefaultSize { get; }
        public CardViewIdle(CardView handler, BaseStateMachine fsm) : base(handler, fsm)
        {
            DefaultSize = Handler.HoverImage.transform.localScale;
        }

        #region State Operations

        public override void OnEnter()
        {
            Handler.Input.OnPointerEnter -= OnPointerEnter;
            Handler.Input.OnPointerEnter += OnPointerEnter;

            Handler.HoverImage.transform.localScale = DefaultSize;
        }

        public override void OnExit()
        {
            Handler.Input.OnPointerEnter -= OnPointerEnter;
        }

        #endregion

        #region Pointer Operations

        private void OnPointerEnter(PointerEventData eventData)
        {
            if(FSM.IsCurrent(this))
            {
                FSM.PushState<CardViewHover>();
            }
        }

        #endregion
    }
}