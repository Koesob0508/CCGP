using CCGP.AspectContainer;
using CCGP.Shared;
using System.ComponentModel;

namespace CCGP.Server
{
    public class FlowSystem : Aspect, IObserve
    {
        public void StartGame()
        {
            var action = new GameStartAction();
            Entity.Perform(action);
        }

        public void EndTurn()
        {
            var action = new TurnEndAction();
            Entity.Perform(action);
        }

        private void EndGame()
        {
            var action = new GameEndAction();
            Entity.Perform(action);
        }

        private void EndRound()
        {
            var action = new RoundEndAction();
            Entity.Perform(action);
        }

        private void StartTurn()
        {
            var action = new TurnStartAction();
            Entity.Perform(action);
        }

        public void Awake()
        {
            this.AddObserver(OnPerformGameStart, Global.PerformNotification<GameStartAction>(), Entity);
            this.AddObserver(OnPerformRoundStart, Global.PerformNotification<RoundStartAction>(), Entity);
            this.AddObserver(OnPerformTurnStart, Global.PerformNotification<TurnStartAction>(), Entity);
            this.AddObserver(OnPerformTurnEnd, Global.PerformNotification<TurnEndAction>(), Entity);
            this.AddObserver(OnPerformRoundEnd, Global.PerformNotification<RoundEndAction>(), Entity);
            this.AddObserver(OnPerformGameEnd, Global.PerformNotification<GameEndAction>(), Entity);
        }

        public void Sleep()
        {
            this.RemoveObserver(OnPerformGameStart, Global.PerformNotification<GameStartAction>(), Entity);
            this.RemoveObserver(OnPerformRoundStart, Global.PerformNotification<RoundStartAction>(), Entity);
            this.RemoveObserver(OnPerformTurnStart, Global.PerformNotification<TurnStartAction>(), Entity);
            this.RemoveObserver(OnPerformTurnEnd, Global.PerformNotification<TurnEndAction>(), Entity);
            this.RemoveObserver(OnPerformRoundEnd, Global.PerformNotification<RoundEndAction>(), Entity);
            this.RemoveObserver(OnPerformGameEnd, Global.PerformNotification<GameEndAction>(), Entity);
        }

        public void OnPerformGameStart(object sender, object args)
        {
            Logger.Log<FlowSystem>("게임 시작");
            var action = new RoundStartAction();
            Entity.AddReaction(action);
        }

        public void OnPerformGameEnd(object sender, object args)
        {
            Logger.Log<FlowSystem>("게임 끝");
        }

        public void OnPerformRoundStart(object sender, object args)
        {
            Logger.Log<FlowSystem>("라운드 시작");
        }

        public void OnPerformRoundEnd(object sender, object args)
        {
            Logger.Log<FlowSystem>("라운드 끝");
        }

        public void OnPerformTurnStart(object sender, object args)
        {
            Logger.Log<FlowSystem>("턴 시작");
        }

        public void OnPerformTurnEnd(object sender, object args)
        {
            Logger.Log<FlowSystem>("턴 끝");
        }
    }
}