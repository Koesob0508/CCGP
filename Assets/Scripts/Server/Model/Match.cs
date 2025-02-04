using System.Collections.Generic;

namespace CCGP.Server
{
    public class Match
    {
        public uint ID { get; set; }
        public Board Board;
        public List<Player> Players;
        public int FirstPlayerIndex;
        public int CurrentPlayerIndex;
        public List<bool> Opened;

        public Match(uint id, Board board, List<Player> players)
        {
            ID = id;
            Board = board;
            Players = players;
            Opened = new(players.Count);
            Opened.AddRange(new bool[players.Count]);
        }
    }
}