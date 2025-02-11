using CCGP.Shared;

namespace CCGP.Client
{
    public abstract class BaseCardViewState : IState
    {
        protected CardView Handler { get; }
        protected BaseStateMachine FSM { get; }

        protected BaseCardViewState(CardView handler, BaseStateMachine fsm)
        {
            Handler = handler;
            FSM = fsm;
        }

        public virtual void OnInitialize() { }
        public virtual void OnEnter() { }
        public virtual void OnUpdate() { }
        public virtual void OnNextState(IState next) { }
        public virtual void OnExit() { }
        public virtual void OnClear() { }
    }
}