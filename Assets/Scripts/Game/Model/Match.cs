using System.Collections.Generic;

namespace CCGP.Server
{
    public class Match
    {
        public Board Board;
        public List<Player> Players;
        public int FirstPlayerIndex;
        public int CurrentPlayerIndex;
        public List<bool> Opened;

        public Match(Board board, List<Player> players)
        {
            Board = board;
            Players = players;
            Opened = new(players.Count);
            Opened.AddRange(new bool[players.Count]);
        }
    }
}