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

            this.AddObserver(OnPerformCardPlay, Global.PerformNotification<PlayCardAction>(), Container);
        }

        public void Deactivate()
        {
            this.RemoveObserver(OnPerformCardPlay, Global.PerformNotification<PlayCardAction>(), Container);
            Data = null;
        }

        private void OnPerformCardPlay(object sender, object args)
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
    }
}