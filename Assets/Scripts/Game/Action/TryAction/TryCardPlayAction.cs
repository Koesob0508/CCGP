namespace CCGP.Server
{
    public class TryPlayCardAction : GameAction
    {
        public Player Player;
        public Card Card;

        public TryPlayCardAction(Player player, Card card)
        {
            Player = player;
            Card = card;
        }
    }
}