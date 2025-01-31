using System.Collections.Generic;

namespace CCGP.Server
{
    public class Match
    {
        public uint ID { get; set; }
        public List<Player> Players;
        public int FirstPlayerIndex;
        public int CurrentPlayerIndex;

        public Match(uint iD, List<ulong> players)
        {
            ID = iD;

            for(int i = 0; i < players.Count; ++i)
            {
                Players.Add(new Player(players[i]));
            }
        }
    }
}