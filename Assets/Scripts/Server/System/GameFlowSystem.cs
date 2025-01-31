using CCGP.AspectContainer;
using CCGP.Shared;
using System.ComponentModel;

namespace CCGP.Server
{
    public class GameFlowSystem : Aspect, IObserve
    {
        public void StartGame()
        {
            var action = new GameStartAction();
            Entity.Perform(action);
        }

        public void EndGame()
        {

        }

        private void StartRound()
        {

        }

        private void EndRound()
        {

        }

        private void StartTurn()
        {

        }

        public void EndTurn()
        {

        }

        public void Awake()
        {
            this.AddObserver(OnPerformGameStart, Global.PerformNotification<GameStartAction>());
            this.AddObserver(OnPerformRoundStart, Global.PerformNotification<RoundStartAction>());
            this.AddObserver(OnPerformTurnStart, Global.PerformNotification<TurnStartAction>());
            this.AddObserver(OnPerformTurnEnd, Global.PerformNotification<TurnEndAction>());
            this.AddObserver(OnPerformRoundEnd, Global.PerformNotification<RoundEndAction>());
            this.AddObserver(OnPerformGameEnd, Global.PerformNotification<GameEndAction>());
        }

        public void Sleep()
        {
            this.RemoveObserver(OnPerformGameStart, Global.PerformNotification<GameStartAction>());
            this.RemoveObserver(OnPerformRoundStart, Global.PerformNotification<RoundStartAction>());
            this.RemoveObserver(OnPerformTurnStart, Global.PerformNotification<TurnStartAction>());
            this.RemoveObserver(OnPerformTurnEnd, Global.PerformNotification<TurnEndAction>());
            this.RemoveObserver(OnPerformRoundEnd, Global.PerformNotification<RoundEndAction>());
            this.RemoveObserver(OnPerformGameEnd, Global.PerformNotification<GameEndAction>());
        }

        public void OnPerformGameStart(object sender, object args)
        {

        }

        public void OnPerformGameEnd(object sender, object args)
        {

        }

        public void OnPerformRoundStart(object sender, object args)
        {

        }
        
        public void OnPerformRoundEnd(object sender, object args)
        {

        }

        public void OnPerformTurnStart(object sender, object args)
        {

        }

        public void OnPerformTurnEnd(object sender, object args)
        {

        }
    }
}