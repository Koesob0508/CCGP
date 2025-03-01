using CCGP.AspectContainer;
using CCGP.Shared;

namespace CCGP.Server
{
    public class BoardSystem : Aspect, IActivatable
    {
        private Board Data;

        public void Activate()
        {
            Data = Container.GetMatch().Board;

            this.AddObserver(OnPlayCard, Global.PerformNotification<PlayCardAction>(), Container);
            this.AddObserver(OnEndRound, Global.PerformNotification<EndRoundAction>(), Container);
        }

        public void Deactivate()
        {
            this.RemoveObserver(OnPlayCard, Global.PerformNotification<PlayCardAction>(), Container);
            this.RemoveObserver(OnEndRound, Global.PerformNotification<EndRoundAction>(), Container);

            Data = null;
        }

        private void OnPlayCard(object sender, object args)
        {
            var action = args as PlayCardAction;
            if (action == null) return;

            var card = action.Card;
            action.Card.TryGetAspect(out Target target);
            var tile = target.Selected;

            if (card == null || tile == null) return;
            if (Data == null)
            {
                LogUtility.LogWarning<BoardSystem>("Data is null");
                return;
            }

            var rTile = Data.GetTile(tile);

            if (rTile == null)
            {
                LogUtility.LogWarning<BoardSystem>("Tile is null");
                return;
            }

            rTile.AgentIndex = card.OwnerIndex;
        }

        private void OnEndRound(object sender, object args)
        {
            var action = args as EndRoundAction;
            var match = action.Match;

            foreach (var tile in Data.Tiles)
            {
                tile.AgentIndex = -1;
            }
        }
    }
}