using CCGP.AspectContainer;
using CCGP.Shared;

namespace CCGP.Server
{
    public class RoundRewardSystem : Aspect, IActivatable
    {
        private RoundReward Data;
        public void Activate()
        {
            Data = Container.GetMatch().RoundReward;
            this.AddObserver(OnStartRound, Global.PerformNotification<StartRoundAction>(), Container);
        }

        public void Deactivate()
        {
            this.RemoveObserver(OnStartRound, Global.PerformNotification<StartRoundAction>(), Container);
        }

        private void OnStartRound(object sender, object args)
        {

        }
    }
}