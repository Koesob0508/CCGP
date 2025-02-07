namespace CCGP.Server
{
    public class CardPlayAction : GameAction
    {
        public Card Card;

        public CardPlayAction(Card card)
        {
            Card = card;
        }
    }
}