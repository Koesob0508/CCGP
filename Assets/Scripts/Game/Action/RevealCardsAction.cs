using System.Collections.Generic;

namespace CCGP.Server
{
    public class RevealCardsAction : GameAction
    {
        public Player Player;
        public List<Card> Cards;
        public RevealCardsAction(Player player)
        {
            Player = player;
        }
    }
}