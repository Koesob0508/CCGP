using CCGP.Shared;
using UnityEngine;

namespace CCGP.Client
{
    public class CardViewDraw : BaseCardViewState
    {
        Vector3 DefaultScale { get; set; }

        public CardViewDraw(CardView handler, BaseStateMachine fsm) : base(handler, fsm) { }

        public override void OnEnter()
        {
            //DefaultScale = Handler.Transform.localScale;
            //Handler.Transform.localScale *= 0.1f;
            //Handler.ScaleTo(DefaultScale, Parameters.ScaleSpeed);

            //Handler.Scale.OnFinishMotion += GoToIdle;
        }

        public override void OnExit()
        {
            //Handler.Scale.OnFinishMotion -= GoToIdle;
        }

        private void GoToIdle()
        {
            FSM.PopState(true);
            FSM.PushState<CardViewIdle>();
        }
    }
}