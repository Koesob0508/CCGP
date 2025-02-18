using CCGP.Shared;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CCGP.Client
{
    public class CardViewIdle : BaseCardViewState
    {
        bool IsFreeze = false;
        Vector3 DefaultSize { get; }
        Vector3 DefaultHoverSize { get; }
        public CardViewIdle(CardView handler, BaseStateMachine fsm) : base(handler, fsm)
        {
            DefaultSize = Handler.transform.localScale;
            DefaultHoverSize = Handler.HoverImage.transform.localScale;
        }

        #region State Operations

        public override void OnEnter()
        {
            this.AddObserver(OnTryPlayCard, Global.MessageNotification(GameCommand.TryPlayCard));
            this.AddObserver(OnPlayCard, Global.MessageNotification(GameCommand.PlayCard));
            this.AddObserver(OnCancelPlayCard, Global.MessageNotification(GameCommand.CancelPlayCard));

            Handler.Input.OnPointerEnter -= OnPointerEnter;
            Handler.Input.OnPointerEnter += OnPointerEnter;

            Handler.transform.localScale = DefaultSize;
            Handler.HoverImage.transform.localScale = DefaultHoverSize;
        }

        public override void OnExit()
        {
            Handler.Input.OnPointerEnter -= OnPointerEnter;

            this.RemoveObserver(OnTryPlayCard, Global.MessageNotification(GameCommand.TryPlayCard));
            this.RemoveObserver(OnPlayCard, Global.MessageNotification(GameCommand.PlayCard));
            this.RemoveObserver(OnCancelPlayCard, Global.MessageNotification(GameCommand.CancelPlayCard));
        }

        #endregion

        #region Pointer Operations

        private void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsFreeze && FSM.IsCurrent(this))
            {
                FSM.PushState<CardViewHover>();
            }
        }

        #endregion

        private void OnTryPlayCard(object sender, object args)
        {
            Freeze();
        }

        private void OnCancelPlayCard(object sender, object args)
        {
            Unfreeze();
        }

        private void OnPlayCard(object sender, object args)
        {
            Unfreeze();
            // FSM.PushState<CardViewPlay>();

            var sData = args as SerializedData;
            var sCard = sData.Get<SerializedCard>();

            if (Handler.Data.ID == sCard.ID)
            {
                Handler.gameObject.SetActive(false);
            }
        }

        private void Freeze()
        {
            IsFreeze = true;
        }

        private void Unfreeze()
        {
            IsFreeze = false;
        }
    }
}