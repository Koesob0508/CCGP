using CCGP.Shared;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CCGP.Client
{
    public class CardView
    {
        public string GUID { get; }
        public int OwnerIndex { get; }

        public string ID { get; }
        public string Name { get; }
        public int Cost { get; }
        public int Persuasion { get; }
        public Space Space { get; }

        public Zone Zone { get; }

        public CardView() { }

        public CardView(SerializedCard sCard)
        {
            GUID = sCard.GUID;
            OwnerIndex = sCard.OwnerIndex;

            ID = sCard.ID;
            Name = sCard.Name;
            Cost = sCard.Cost;
            Persuasion = sCard.Persuasion;
            Space = sCard.Space;

            Zone = sCard.Zone;
        }
    }

    public class PlayerView
    {
        public ulong ID { get; }
        public int Index { get; }
        public string LobbyID { get; }
        public uint AgentCount { get; }

        private readonly List<CardView> _leader = new();
        public ReadOnlyCollection<CardView> Leader => _leader.AsReadOnly();

        private readonly List<CardView> _deck = new();
        public ReadOnlyCollection<CardView> Deck => _deck.AsReadOnly();

        private readonly List<CardView> _hand = new();
        public ReadOnlyCollection<CardView> Hand => _hand.AsReadOnly();

        private readonly List<CardView> _graveyard = new();
        public ReadOnlyCollection<CardView> Graveyard => _graveyard.AsReadOnly();

        private readonly List<CardView> _agent = new();
        public ReadOnlyCollection<CardView> Agent => _agent.AsReadOnly();

        private readonly List<CardView> _open = new();
        public ReadOnlyCollection<CardView> Open => _open.AsReadOnly();

        public PlayerView() { }

        public PlayerView(SerializedPlayer sPlayer)
        {
            ID = sPlayer.ID;
            Index = sPlayer.Index;
            LobbyID = sPlayer.LobbyID;
            AgentCount = sPlayer.AgentCount;

            _leader = new();
            _deck = new();
            _hand = new();
            _graveyard = new();
            _agent = new();
            _open = new();

            foreach (var card in sPlayer.Leader)
            {
                _leader.Add(new CardView(card));
            }

            foreach (var card in sPlayer.Deck)
            {
                _deck.Add(new CardView(card));
            }

            foreach (var card in sPlayer.Hand)
            {
                _hand.Add(new CardView(card));
            }

            foreach (var card in sPlayer.Graveyard)
            {
                _graveyard.Add(new CardView(card));
            }

            foreach (var card in sPlayer.Agent)
            {
                _agent.Add(new CardView(card));
            }

            foreach (var card in sPlayer.Open)
            {
                _open.Add(new CardView(card));
            }
        }
    }

    public class TileView
    {
        public string Name { get; }
        public Space Space { get; }
        public int AgentIndex { get;}

        public TileView() { }
        public TileView(SerializedTile sTile)
        {
            Name = sTile.Name;
            Space = sTile.Space;
            AgentIndex = sTile.AgentIndex;
        }
    }

    public class BoardView
    {
        private readonly List<TileView> _tiles;
        public ReadOnlyCollection<TileView> Tiles => _tiles.AsReadOnly();
        public BoardView() { }

        public BoardView(SerializedBoard sBoard)
        {
            _tiles = new();
            foreach(var tile in sBoard.Tiles)
            {
                _tiles.Add(new TileView(tile));
            }
        }
    }

    public class MatchView
    {
        public int YourIndex { get; }
        public BoardView Board { get; }
        private readonly List<PlayerView> _players;
        public ReadOnlyCollection<PlayerView> Players => _players.AsReadOnly();
        public int FirstPlayerIndex { get; }
        public int CurrentPlayerIndex { get; }
        private readonly List<bool> _opened;
        public ReadOnlyCollection<bool> Opened => _opened.AsReadOnly();
        public MatchView() { }
        public MatchView(SerializedMatch sMatch)
        {
            YourIndex = sMatch.YourIndex;

            Board = new BoardView(sMatch.Board);
            
            _players = new();
            foreach(var player in sMatch.Players)
            {
                _players.Add(new PlayerView(player));
            }
            
            FirstPlayerIndex = sMatch.FirstPlayerIndex;
            CurrentPlayerIndex = sMatch.CurrentPlayerIndex;
            
            _opened = new();
            foreach(var open in sMatch.Opened)
            {
                _opened.Add(open);
            }
        }
    }
}