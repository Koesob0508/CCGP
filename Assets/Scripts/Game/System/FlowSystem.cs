using CCGP.AspectContainer;
using CCGP.Shared;

namespace CCGP.Server
{
    public class FlowSystem : Aspect, IActivatable
    {
        public void Activate()
        {
            this.AddObserver(OnPerformGameStart, Global.PerformNotification<GameStartAction>(), Container);
            this.AddObserver(OnPerformRoundStart, Global.PerformNotification<StartRoundAction>(), Container);
            this.AddObserver(OnPerformTurnStart, Global.PerformNotification<TurnStartAction>(), Container);
            this.AddObserver(OnPerformTurnEnd, Global.PerformNotification<TurnEndAction>(), Container);
            this.AddObserver(OnPerformRoundEnd, Global.PerformNotification<RoundEndAction>(), Container);
            this.AddObserver(OnPerformGameEnd, Global.PerformNotification<GameEndAction>(), Container);

            this.AddObserver(OnReceivedTryEndTurn, Global.MessageNotification(GameCommand.TryEndTurn), Container);
        }

        public void Deactivate()
        {
            this.RemoveObserver(OnPerformGameStart, Global.PerformNotification<GameStartAction>(), Container);
            this.RemoveObserver(OnPerformRoundStart, Global.PerformNotification<StartRoundAction>(), Container);
            this.RemoveObserver(OnPerformTurnStart, Global.PerformNotification<TurnStartAction>(), Container);
            this.RemoveObserver(OnPerformTurnEnd, Global.PerformNotification<TurnEndAction>(), Container);
            this.RemoveObserver(OnPerformRoundEnd, Global.PerformNotification<RoundEndAction>(), Container);
            this.RemoveObserver(OnPerformGameEnd, Global.PerformNotification<GameEndAction>(), Container);

            this.RemoveObserver(OnReceivedTryEndTurn, Global.MessageNotification(GameCommand.TryEndTurn), Container);
        }

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

        public void OnPerformGameStart(object sender, object args)
        {
            LogUtility.Log<FlowSystem>("게임 시작");
            var action = new StartRoundAction();
            Container.AddReaction(action);
        }

        public void OnPerformGameEnd(object sender, object args)
        {
            LogUtility.Log<FlowSystem>("게임 종료");
        }

        public void OnPerformRoundStart(object sender, object args)
        {
            LogUtility.Log<FlowSystem>("라운드 시작");

            var match = Container.GetMatch();
            var action = new TurnStartAction(match.FirstPlayerIndex);
            Container.AddReaction(action);
        }

        public void OnPerformRoundEnd(object sender, object args)
        {
            LogUtility.Log<FlowSystem>("라운드 종료");

            var gameEndAction = new GameEndAction();
            Container.AddReaction(gameEndAction);
        }

        public void OnPerformTurnStart(object sender, object args)
        {
            var action = args as TurnStartAction;

            LogUtility.Log<FlowSystem>($"{action.TargetPlayerIndex}번째 플레이어 턴 시작");

            var match = Container.GetMatch();
            match.CurrentPlayerIndex = action.TargetPlayerIndex;
        }

        public void OnPerformTurnEnd(object sender, object args)
        {
            var match = Container.GetMatch();
            var action = args as TurnEndAction;

            LogUtility.Log<FlowSystem>($"{action.TargetPlayerIndex}번째 플레이어 턴 종료");

            // 임시 라운드 종료 조건
            var roundEnd = true;
            foreach (var player in match.Players)
            {
                if (player.IsOpened == false)
                {
                    roundEnd = false;
                }
            }

            // if (match.Opened.Contains(false))
            if (roundEnd)
            {
                var roundEndAction = new RoundEndAction();
                Container.AddReaction(roundEndAction);
            }
            else
            {
                var turnStartAction = new TurnStartAction(action.NextPlayerIndex);
                Container.AddReaction(turnStartAction);
            }
        }

        private void OnReceivedTryEndTurn(object sender, object args)
        {
            LogUtility.Log<FlowSystem>("Received OnTryEndTurn", colorName: ColorCodes.Logic);

            var sData = args as SerializedData;
            var sPlayer = sData.Get<SerializedPlayer>();

            EndTurn(sPlayer.Index);
        }
    }
}