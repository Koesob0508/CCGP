using System.Collections.Generic;

namespace CCGP.Server
{
    public class OpenCardsAction : GameAction
    {
        public List<Card> Cards;
        public OpenCardsAction(Player player)
        {
            Player = player;
        }
    }
}