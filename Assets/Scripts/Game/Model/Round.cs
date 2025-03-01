using System.Collections.Generic;

namespace CCGP.Server
{
    public class Round
    {
        public uint Count;
        public Stack<RoundReward> RewardDeck;
        public RoundReward CurrentReward;
    }
}