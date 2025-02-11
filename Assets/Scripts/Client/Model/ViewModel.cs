using CCGP.Shared;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CCGP.Client
{
    public class CardViewModel
    {
        public string GUID { get; }
        public int OwnerIndex { get; }

        public string ID { get; }
        public string Name { get; }
        public int Cost { get; }
        public int Persuasion { get; }
        public Space Space { get; }

        public Zone Zone { get; }

        public CardViewModel() { }

        public CardViewModel(SerializedCard sCard)
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

    public class PlayerViewModel
    {
        public ulong ID { get; }
        public int Index { get; }
        public string LobbyID { get; }
        public uint AgentCount { get; }

        private readonly List<CardViewModel> _leader = new();
        public ReadOnlyCollection<CardViewModel> Leader => _leader.AsReadOnly();

        private readonly List<CardViewModel> _deck = new();
        public ReadOnlyCollection<CardViewModel> Deck => _deck.AsReadOnly();

        private readonly List<CardViewModel> _hand = new();
        public ReadOnlyCollection<CardViewModel> Hand => _hand.AsReadOnly();

        private readonly List<CardViewModel> _graveyard = new();
        public ReadOnlyCollection<CardViewModel> Graveyard => _graveyard.AsReadOnly();

        private readonly List<CardViewModel> _agent = new();
        public ReadOnlyCollection<CardViewModel> Agent => _agent.AsReadOnly();

        private readonly List<CardViewModel> _open = new();
        public ReadOnlyCollection<CardViewModel> Open => _open.AsReadOnly();

        public PlayerViewModel() { }

        public PlayerViewModel(SerializedPlayer sPlayer)
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
                _leader.Add(new CardViewModel(card));
            }

            foreach (var card in sPlayer.Deck)
            {
                _deck.Add(new CardViewModel(card));
            }

            foreach (var card in sPlayer.Hand)
            {
                _hand.Add(new CardViewModel(card));
            }

            foreach (var card in sPlayer.Graveyard)
            {
                _graveyard.Add(new CardViewModel(card));
            }

            foreach (var card in sPlayer.Agent)
            {
                _agent.Add(new CardViewModel(card));
            }

            foreach (var card in sPlayer.Open)
            {
                _open.Add(new CardViewModel(card));
            }
        }
    }

    public class TileViewModel
    {
        public string Name { get; }
        public Space Space { get; }
        public int AgentIndex { get;}

        public TileViewModel() { }
        public TileViewModel(SerializedTile sTile)
        {
            Name = sTile.Name;
            Space = sTile.Space;
            AgentIndex = sTile.AgentIndex;
        }
    }

    public class BoardViewModel
    {
        private readonly List<TileViewModel> _tiles;
        public ReadOnlyCollection<TileViewModel> Tiles => _tiles.AsReadOnly();
        public BoardViewModel() { }

        public BoardViewModel(SerializedBoard sBoard)
        {
            _tiles = new();
            foreach(var tile in sBoard.Tiles)
            {
                _tiles.Add(new TileViewModel(tile));
            }
        }
    }

    public class MatchViewModel
    {
        public int YourIndex { get; }
        public BoardViewModel Board { get; }
        private readonly List<PlayerViewModel> _players;
        public ReadOnlyCollection<PlayerViewModel> Players => _players.AsReadOnly();
        public int FirstPlayerIndex { get; }
        public int CurrentPlayerIndex { get; }
        private readonly List<bool> _opened;
        public ReadOnlyCollection<bool> Opened => _opened.AsReadOnly();
        public MatchViewModel() { }
        public MatchViewModel(SerializedMatch sMatch)
        {
            YourIndex = sMatch.YourIndex;

            Board = new BoardViewModel(sMatch.Board);
            
            _players = new();
            foreach(var player in sMatch.Players)
            {
                _players.Add(new PlayerViewModel(player));
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