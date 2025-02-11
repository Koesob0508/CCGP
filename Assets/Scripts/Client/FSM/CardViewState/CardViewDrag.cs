using CCGP.Shared;

namespace CCGP.Client
{
    public class CardViewDrag : BaseCardViewState
    {
        public CardViewDrag(CardView handler, BaseStateMachine fsm) : base(handler, fsm) { }

        #region State Operations

        public override void OnInitialize() { }
        public override void OnEnter()
        {
            
        }
        public override void OnUpdate() { }
        public override void OnNextState(IState next) { }
        public override void OnExit() { }
        public override void OnClear() { }

        #endregion
    }
}