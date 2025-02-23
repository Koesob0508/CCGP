using System.Collections.Generic;

namespace CCGP.Server
{
    public class BatchDrawCardsAction : GameAction
    {
        public List<uint> Amounts;
        public Dictionary<int, List<Card>> Cards;

        public BatchDrawCardsAction(uint amount)
        {
            Amounts = new() { amount, amount, amount, amount };
            Cards = new();
        }
    }
}