using CCGP.AspectContainer;

namespace CCGP.Server
{
    public class OpenMarketSystem : Aspect, IObserve
    {
        public void Awake()
        {
            this.AddObserver(OnPrepareTurnEnd, Global.PrepareNotification<TurnEndAction>(), Container);
        }

        public void Sleep()
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