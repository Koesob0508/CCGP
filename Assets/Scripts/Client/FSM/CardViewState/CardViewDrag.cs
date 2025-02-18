using CCGP.Shared;
using UnityEngine.EventSystems;

namespace CCGP.Client
{
    public class CardViewDrag : BaseCardViewState
    {
        public CardViewDrag(CardView handler, BaseStateMachine fsm) : base(handler, fsm) { }

        #region State Operations

        public override void OnInitialize() { }
        public override void OnEnter()
        {
            Handler.Input.OnDrag += OnDrag;
            Handler.Input.OnEndDrag += OnEndDrag;
        }

        public override void OnExit()
        {
            Handler.Input.OnDrag -= OnDrag;
            Handler.Input.OnEndDrag -= OnEndDrag;
        }

        #endregion

        #region Pointer Operations

        private void OnDrag(PointerEventData eventData)
        {
            Handler.transform.position = eventData.position;
        }

        private void OnEndDrag(PointerEventData eventData)
        {
            // Cancel을 내야하는데
            this.PostNotification(Global.MessageNotification(GameCommand.SelectTile), new SerializedTile() { Name = "NULL", Space = Space.None, AgentIndex = -1 });
            FSM.PopState();
        }

        #endregion
    }
}