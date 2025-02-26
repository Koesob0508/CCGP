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

            this.AddObserver(OnStartRound, Global.PerformNotification<StartRoundAction>(), Container);
        }

        public void Deactivate()
        {
            this.RemoveObserver(OnStartRound, Global.PerformNotification<StartRoundAction>(), Container);

            Data = null;
        }

        private void OnStartRound(object sender, object args)
        {
            var action = args as StartRoundAction;
            if (action == null) return;

            Data.Row.Clear();

            for (int i = 0; i < 5; i++)
            {
                if (TryDrawImperium(out var card))
                {
                    Data.Row.Add(card);
                    card.SetZone(Zone.ImperiumRow);
                }
            }
        }

        private bool TryDrawImperium(out Card card)
        {
            if (Data.Deck.Count == 0)
            {
                card = null;
                return false;
            }

            card = Data.Deck[0];
            Data.Deck.RemoveAt(0);

            return true;
        }
    }
}