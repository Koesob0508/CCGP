namespace CCGP.Server
{
    public class CardPlayAction : GameAction
    {
        public Card Card;

        public CardPlayAction(Player player, Card card)
        {
            Player = player;
            Card = card;
        }
    }
}