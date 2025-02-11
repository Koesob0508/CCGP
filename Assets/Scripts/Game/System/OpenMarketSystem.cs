using CCGP.AspectContainer;
using CCGP.Shared;

namespace CCGP.Server
{
    public class OpenMarketSystem : Aspect, IActivatable
    {
        public void Activate()
        {
            this.AddObserver(OnPrepareTurnEnd, Global.PrepareNotification<TurnEndAction>(), Container);
        }

        public void Deactivate()
        {
            this.RemoveObserver(OnPrepareTurnEnd, Global.PrepareNotification<TurnEndAction>(), Container);
        }

        private void OnPrepareTurnEnd(object sender, object args)
        {
            var match = Container.GetMatch();
            var action = args as TurnEndAction;

            match.Opened[action.TargetPlayerIndex] = true;
        }
    }
}