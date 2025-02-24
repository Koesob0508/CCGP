using CCGP.AspectContainer;
using CCGP.Shared;
using System;

namespace CCGP.Server
{
    public class MatchSystem : Aspect, IActivatable
    {
        public void Activate()
        {
            this.AddObserver(OnStartGame, Global.PerformNotification<StartGameAction>(), Container);
            this.AddObserver(OnStartRound, Global.PerformNotification<StartRoundAction>(), Container);
            this.AddObserver(OnStartTurn, Global.PerformNotification<StartTurnAction>(), Container);
            this.AddObserver(OnEndTurn, Global.PerformNotification<EndTurnAction>(), Container);
            this.AddObserver(OnEndRound, Global.PerformNotification<EndRoundAction>(), Container);
            this.AddObserver(OnEndGame, Global.PerformNotification<EndGameAction>(), Container);
        }

        public void Deactivate()
        {
            this.RemoveObserver(OnStartGame, Global.PerformNotification<StartGameAction>(), Container);
            this.RemoveObserver(OnStartRound, Global.PerformNotification<StartRoundAction>(), Container);
            this.RemoveObserver(OnStartTurn, Global.PerformNotification<StartTurnAction>(), Container);
            this.RemoveObserver(OnEndTurn, Global.PerformNotification<EndTurnAction>(), Container);
            this.RemoveObserver(OnEndRound, Global.PerformNotification<EndRoundAction>(), Container);
            this.RemoveObserver(OnEndGame, Global.PerformNotification<EndGameAction>(), Container);
        }

        private void OnStartGame(object sender, object args)
        {
            var action = args as StartGameAction;
            var match = Container.GetMatch();

            // match의 FirstPlayerIndex를 무작위로 정한다.
            var random = new Random();
            match.FirstPlayerIndex = random.Next(0, match.Players.Count);
            action.Match = match;

            LogUtility.Log<MatchSystem>($"게임 시작 - 첫 번째 플레이어: {match.FirstPlayerIndex}", colorName: ColorCodes.Logic);
        }

        private void OnStartRound(object sender, object args)
        {
            var action = args as StartRoundAction;
            var match = Container.GetMatch();

            // 어디선가 선 턴 플레이어를 바꿔주긴 해야함
            action.Match = match;

            LogUtility.Log<MatchSystem>($"라운드 시작 - 첫 번째 플레이어: {match.FirstPlayerIndex}", colorName: ColorCodes.Logic);
        }

        private void OnStartTurn(object sender, object args)
        {
            var action = args as StartTurnAction;
            var match = Container.GetMatch();

            action.Match = match;

            LogUtility.Log<MatchSystem>($"턴 시작 (플레이어: {match.CurrentPlayerIndex})", colorName: ColorCodes.Logic);
        }

        private void OnEndTurn(object sender, object args)
        {
            var action = args as EndTurnAction;
            var match = Container.GetMatch();

            action.Match = match;

            LogUtility.Log<MatchSystem>($"턴 종료 (플레이어 : {match.CurrentPlayerIndex})", colorName: ColorCodes.Logic);
        }

        private void OnEndRound(object sender, object args)
        {
            var action = args as EndRoundAction;
            var match = Container.GetMatch();

            action.Match = match;

            LogUtility.Log<MatchSystem>($"라운드 종료", colorName: ColorCodes.Logic);
        }

        private void OnEndGame(object sender, object args)
        {
            var action = args as EndGameAction;
            var match = Container.GetMatch();

            action.Match = match;

            LogUtility.Log<MatchSystem>($"게임 종료", colorName: ColorCodes.Logic);
        }
    }
}