using CCGP.Shared;

namespace CCGP.Client
{
    public class CardViewFSM : BaseStateMachine
    {
        private CardViewDraw DrawState { get; }
        private CardViewIdle IdleState { get; }
        private CardViewHover HoverState { get; }
        private CardViewSelect SelectState { get; }
        private CardViewDrag DragState { get; }

        public CardViewFSM(CardView handler = null) : base(handler)
        {
            DrawState = new CardViewDraw(handler, this);
            IdleState = new CardViewIdle(handler, this);
            HoverState = new CardViewHover(handler, this);
            SelectState = new CardViewSelect(handler, this);
            DragState = new CardViewDrag(handler, this);

            RegisterState(DrawState);
            RegisterState(IdleState);
            RegisterState(HoverState);
            RegisterState(SelectState);
            RegisterState(DragState);

            Initialize();
        }
    }
}