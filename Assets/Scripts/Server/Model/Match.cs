using System.Collections.Generic;

namespace CCGP.Server
{
    public class Match
    {
        public uint ID { get; set; }
        public List<Player> Players;
        public int FirstPlayerIndex;
        public int CurrentPlayerIndex;

        public Match(uint id, List<Player> players)
        {
            ID = id;
            Players = players;
        }
    }
}