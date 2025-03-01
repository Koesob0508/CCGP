using System.Collections.Generic;

namespace CCGP.Server
{
    public class Match
    {
        public Round Round;
        public const int MaxRound = 10;
        public List<Player> Players;
        public Board Board;
        public Imperium Imperium;
        public int FirstPlayerIndex;
        public int CurrentPlayerIndex;
        public List<bool> Opened;

        public Match(List<Player> players, Board board, Round round, Imperium imperium)
        {
            Round = round;
            Players = players;
            Board = board;
            Imperium = imperium;
            Opened = new(players.Count);
            Opened.AddRange(new bool[players.Count]);
        }
    }
}