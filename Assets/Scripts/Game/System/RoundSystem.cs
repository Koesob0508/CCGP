using CCGP.AspectContainer;
using CCGP.Shared;

namespace CCGP.Server
{
    public class RoundSystem : Aspect, IActivatable
    {
        private Round Data;

        public void Activate()
        {
            Data = Container.GetMatch().Round;
            this.AddObserver(OnPerformStartGame, Global.PerformNotification<StartGameAction>(), Container);
            this.AddObserver(OnPerformStartRound, Global.PerformNotification<StartRoundAction>(), Container);
            this.AddObserver(OnPrepareEndRound, Global.PrepareNotification<EndRoundAction>(), Container);
        }

        public void Deactivate()
        {
            this.RemoveObserver(OnPerformStartGame, Global.PerformNotification<StartGameAction>(), Container);
            this.RemoveObserver(OnPerformStartRound, Global.PerformNotification<StartRoundAction>(), Container);
            this.RemoveObserver(OnPrepareEndRound, Global.PrepareNotification<EndRoundAction>(), Container);
        }

        private void OnPerformStartGame(object sender, object args)
        {
            if (TryDrawRoundReward(out var reward))
            {
                Data.CurrentReward = reward;
            }

            LogUtility.Log<RoundSystem>($"현재 {Data.Count} : {Data.CurrentReward.Name} 라운드입니다.", colorName: ColorCodes.Logic);
        }

        private void OnPerformStartRound(object sender, object args)
        {
            Data.Count++;

            // Round Deck에서 한 장 뽑아서 CurrentReward에 놓는다.
            if (TryDrawRoundReward(out var reward))
            {
                Data.CurrentReward = reward;
            }

            LogUtility.Log<RoundSystem>($"현재 {Data.Count} : {Data.CurrentReward.Name} 라운드입니다.", colorName: ColorCodes.Logic);
        }

        private void OnPrepareEndRound(object sender, object args)
        {
            var action = args as EndRoundAction;

            if (Data.Count < Match.MaxRound)
            {
                action.IsEndRound = false;
            }
            else if (Data.Count == Match.MaxRound)
            {
                action.IsEndRound = true;
            }
            else
            {
                LogUtility.LogWarning<RoundSystem>($"{Data.Count} 최고 라운드를 넘어섬", colorName: ColorCodes.Red);
            }
        }

        private bool TryDrawRoundReward(out RoundReward reward)
        {
            if (Data.RewardDeck.Count == 0)
            {
                reward = null;
                return false;
            }

            reward = Data.RewardDeck.Pop();

            return true;
        }
    }
}