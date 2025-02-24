using CCGP.AspectContainer;
using CCGP.Shared;

namespace CCGP.Server
{
    public class FlowSystem : Aspect, IActivatable
    {
        public void Activate()
        {
            this.AddObserver(OnPerformGameStart, Global.PerformNotification<StartGameAction>(), Container);
            this.AddObserver(OnPerformRoundStart, Global.PerformNotification<StartRoundAction>(), Container);
            this.AddObserver(OnPerformTurnStart, Global.PerformNotification<StartTurnAction>(), Container);
            this.AddObserver(OnPerformTurnEnd, Global.PerformNotification<EndTurnAction>(), Container);
            this.AddObserver(OnPerformRoundEnd, Global.PerformNotification<EndRoundAction>(), Container);
            this.AddObserver(OnPerformGameEnd, Global.PerformNotification<EndGameAction>(), Container);

            this.AddObserver(OnReceivedTryEndTurn, Global.MessageNotification(GameCommand.TryEndTurn), Container);
        }

        public void Deactivate()
        {
            this.RemoveObserver(OnPerformGameStart, Global.PerformNotification<StartGameAction>(), Container);
            this.RemoveObserver(OnPerformRoundStart, Global.PerformNotification<StartRoundAction>(), Container);
            this.RemoveObserver(OnPerformTurnStart, Global.PerformNotification<StartTurnAction>(), Container);
            this.RemoveObserver(OnPerformTurnEnd, Global.PerformNotification<EndTurnAction>(), Container);
            this.RemoveObserver(OnPerformRoundEnd, Global.PerformNotification<EndRoundAction>(), Container);
            this.RemoveObserver(OnPerformGameEnd, Global.PerformNotification<EndGameAction>(), Container);

            this.RemoveObserver(OnReceivedTryEndTurn, Global.MessageNotification(GameCommand.TryEndTurn), Container);
        }

        public void StartGame()
        {
            var action = new StartGameAction();
            Container.Perform(action);
        }

        private void OnReceivedTryEndTurn(object sender, object args)
        {
            LogUtility.Log<FlowSystem>("Received OnTryEndTurn", colorName: ColorCodes.Logic);

            var sData = args as SerializedData;
            var sPlayer = sData.Get<SerializedPlayer>();

            EndTurn(sPlayer.Index);
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

            var action = new EndTurnAction(targetPlayerIndex, nextPlayerIndex);
            Container.Perform(action);
        }

        private void EndGame()
        {
            var action = new EndGameAction();
            Container.Perform(action);
        }

        private void EndRound()
        {
            var action = new EndRoundAction();
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
            var action = new StartTurnAction(match.FirstPlayerIndex);
            Container.AddReaction(action);
        }

        public void OnPerformRoundEnd(object sender, object args)
        {
            LogUtility.Log<FlowSystem>("라운드 종료");

            var gameEndAction = new EndGameAction();
            Container.AddReaction(gameEndAction);
        }

        public void OnPerformTurnStart(object sender, object args)
        {
            var action = args as StartTurnAction;

            LogUtility.Log<FlowSystem>($"{action.TargetPlayerIndex}번째 플레이어 턴 시작");

            var match = Container.GetMatch();
            match.CurrentPlayerIndex = action.TargetPlayerIndex;
        }

        public void OnPerformTurnEnd(object sender, object args)
        {
            var match = Container.GetMatch();
            var action = args as EndTurnAction;

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
                var roundEndAction = new EndRoundAction();
                Container.AddReaction(roundEndAction);
            }
            else
            {
                var turnStartAction = new StartTurnAction(action.NextPlayerIndex);
                Container.AddReaction(turnStartAction);
            }
        }


    }
}