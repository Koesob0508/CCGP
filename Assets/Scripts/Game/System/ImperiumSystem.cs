using CCGP.AspectContainer;
using CCGP.Shared;

namespace CCGP.Server
{
    public class ImperiumSystem : Aspect, IActivatable
    {
        private Imperium Data;

        public void Activate()
        {
            Data = Container.GetMatch().Imperium;

            this.AddObserver(OnPerformStartRound, Global.PerformNotification<StartRoundAction>(), Container);
        }

        public void Deactivate()
        {
            this.RemoveObserver(OnPerformStartRound, Global.PerformNotification<StartRoundAction>(), Container);
        }

        private void OnPerformStartRound(object sender, object args)
        {
            var action = args as StartRoundAction;
            if (action == null) return;

            Data.Row.Clear();

            for (int i = 0; i < 5; i++)
            {
                var card = Data.Deck[0];
                Data.Row.Add(card);
                Data.Deck.RemoveAt(0);
                card.SetZone(Zone.ImperiumRow);
            }
        }
    }
}