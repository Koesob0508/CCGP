namespace CCGP.Server
{
    public class PlayCardAction : GameAction
    {
        public Card Card;

        public PlayCardAction(Player player, Card card)
        {
            Player = player;
            Card = card;
        }
    }
}