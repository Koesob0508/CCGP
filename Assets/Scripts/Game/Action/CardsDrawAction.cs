using CCGP.Shared;
using System.Collections.Generic;
using Unity.Netcode;

namespace CCGP.Server
{
    public class CardsDrawAction : GameAction
    {
        public int Amount;
        public List<Card> Cards;

        public CardsDrawAction(Player player, int amount)
        {
            Player = player;
            Amount = amount;
        }
    }
}