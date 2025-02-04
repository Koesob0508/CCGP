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
            Container.Perform(action);
        }

        public void EndTurn(int targetPlayerIndex)
        {
            var match = Container.GetMatch();
            var nextIndex = (targetPlayerIndex + 1) % match.Players.Count;
            EndTurn(targetPlayerIndex, nextIndex);
        }

        public void EndTurn(int targetPlayerIndex, int nextPlayerIndex)
        {
            var match = Container.GetMatch();

            // 현재 턴이 아닌 사람이 EndTurn을 호출했을 경우 return
            if (targetPlayerIndex != match.CurrentPlayerIndex) return;

            var action = new TurnEndAction(targetPlayerIndex, nextPlayerIndex);
            Container.Perform(action);
        }

        private void EndGame()
        {
            var action = new GameEndAction();
            Container.Perform(action);
        }

        private void EndRound()
        {
            var action = new RoundEndAction();
            Container.Perform(action);
        }

        private void StartTurn()
        {
            //var action = new TurnStartAction();
            //Entity.Perform(action);
        }

        public void Awake()
        {
            this.AddObserver(OnPerformGameStart, Global.PerformNotification<GameStartAction>(), Container);
            this.AddObserver(OnPerformRoundStart, Global.PerformNotification<RoundStartAction>(), Container);
            this.AddObserver(OnPerformTurnStart, Global.PerformNotification<TurnStartAction>(), Container);
            this.AddObserver(OnPerformTurnEnd, Global.PerformNotification<TurnEndAction>(), Container);
            this.AddObserver(OnPerformRoundEnd, Global.PerformNotification<RoundEndAction>(), Container);
            this.AddObserver(OnPerformGameEnd, Global.PerformNotification<GameEndAction>(), Container);
        }

        public void Sleep()
        {
            this.RemoveObserver(OnPerformGameStart, Global.PerformNotification<GameStartAction>(), Container);
            this.RemoveObserver(OnPerformRoundStart, Global.PerformNotification<RoundStartAction>(), Container);
            this.RemoveObserver(OnPerformTurnStart, Global.PerformNotification<TurnStartAction>(), Container);
            this.RemoveObserver(OnPerformTurnEnd, Global.PerformNotification<TurnEndAction>(), Container);
            this.RemoveObserver(OnPerformRoundEnd, Global.PerformNotification<RoundEndAction>(), Container);
            this.RemoveObserver(OnPerformGameEnd, Global.PerformNotification<GameEndAction>(), Container);
        }

        public void OnPerformGameStart(object sender, object args)
        {
            Logger.Log<FlowSystem>("게임 시작");
            var action = new RoundStartAction();
            Container.AddReaction(action);
        }

        public void OnPerformGameEnd(object sender, object args)
        {
            Logger.Log<FlowSystem>("게임 종료");
        }

        public void OnPerformRoundStart(object sender, object args)
        {
            Logger.Log<FlowSystem>("라운드 시작");

            var match = Container.GetMatch();
            var action = new TurnStartAction(match.FirstPlayerIndex);
            Container.AddReaction(action);
        }

        public void OnPerformRoundEnd(object sender, object args)
        {
            Logger.Log<FlowSystem>("라운드 종료");

            var gameEndAction = new GameEndAction();
            Container.AddReaction(gameEndAction);
        }

        public void OnPerformTurnStart(object sender, object args)
        {
            var action = args as TurnStartAction;
            
            Logger.Log<FlowSystem>($"{action.TargetPlayerIndex}번째 플레이어 턴 시작");

            var match = Container.GetMatch();
            match.CurrentPlayerIndex = action.TargetPlayerIndex;
        }

        public void OnPerformTurnEnd(object sender, object args)
        {
            var match = Container.GetMatch();
            var action = args as TurnEndAction;

            Logger.Log<FlowSystem>($"{action.TargetPlayerIndex}번째 플레이어 턴 종료");

            if (match.Opened.Contains(false))
            {
                var turnStartAction = new TurnStartAction(action.NextPlayerIndex);
                Container.AddReaction(turnStartAction);
            }
            else
            {
                var roundEndAction = new RoundEndAction();
                Container.AddReaction(roundEndAction);
            }
        }
    }
}