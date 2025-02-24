using System.Collections.Generic;

namespace CCGP.Server
{
    public class OpenCardsAction : GameAction
    {
        public Player Player;
        public List<Card> Cards;
        public OpenCardsAction(Player player)
        {
            Player = player;
        }
    }
}