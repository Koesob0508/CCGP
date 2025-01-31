using CCGP.AspectContainer;

namespace CCGP.Server
{
    public class OpenMarketSystem : Aspect, IObserve
    {
        public void Awake()
        {
            this.AddObserver(OnPrepareTurnEnd, Global.PrepareNotification<TurnEndAction>(), Entity);
        }

        public void Sleep()
        {
            this.RemoveObserver(OnPrepareTurnEnd, Global.PrepareNotification<TurnEndAction>(), Entity);
        }

        private void OnPrepareTurnEnd(object sender, object args)
        {
            var match = Entity.GetMatch();
            var action = args as TurnEndAction;

            match.Opened[action.TargetPlayerIndex] = true;
        }
    }
}