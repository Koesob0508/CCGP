using CCGP.Server;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

namespace CCGP.Shared
{
    public class SerializedData : INetworkSerializable
    {
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            throw new System.NotImplementedException();
        }
    }
    public class SerializedCard : INetworkSerializable
    {
        public string GUID;
        public int OwnerIndex;

        public string ID;
        public string Name;
        public int Cost;
        public int Persuasion;
        public Space Space;

        public Zone Zone;

        public SerializedCard() { }

        public SerializedCard(Card card)
        {
            GUID = card.GUID;
            OwnerIndex = card.OwnerIndex;
            ID = card.ID;
            Name = card.Name;
            Cost = card.Cost;
            Persuasion = card.Persuasion;
            Space = card.Space;
            Zone = card.Zone;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref GUID);
            serializer.SerializeValue(ref OwnerIndex);
            serializer.SerializeValue(ref ID);
            serializer.SerializeValue(ref Name);
            serializer.SerializeValue(ref Cost);
            serializer.SerializeValue(ref Persuasion);
            serializer.SerializeValue(ref Space);
            serializer.SerializeValue(ref Zone);
        }
    }

    public class SerializedPlayer : INetworkSerializable
    {
        public ulong ID;
        public int Index;
        public uint AgentCount;
        public List<SerializedCard> leader;
        public List<SerializedCard> Deck;
        public List<SerializedCard> Hand;
        public List<SerializedCard> Graveyard;
        public List<SerializedCard> Agent;
        public List<SerializedCard> Open;

        public SerializedPlayer() { }

        public SerializedPlayer(Player player)
        {
            ID = player.ID;
            Index = player.Index;
            AgentCount = player.AgentCount;

            leader = new();
            foreach (var card in player[Zone.Leader])
            {
                leader.Add(new SerializedCard(card));
            }

            Deck = new();
            foreach (var card in player[Zone.Deck])
            {
                Deck.Add(new SerializedCard(card));
            }

            Hand = new();
            foreach (var card in player[Zone.Hand])
            {
                Hand.Add(new SerializedCard(card));
            }

            Graveyard = new();
            foreach (var card in player[Zone.Graveyard])
            {
                Graveyard.Add(new SerializedCard(card));
            }

            Agent = new();
            foreach (var card in player[Zone.Agent])
            {
                Agent.Add(new SerializedCard(card));
            }

            Open = new();
            foreach (var card in player[Zone.Open])
            {
                Open.Add(new SerializedCard(card));
            }
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ID);
            serializer.SerializeValue(ref Index);

            SerializeList(serializer, ref leader);
            SerializeList(serializer, ref Deck);
            SerializeList(serializer, ref Hand);
            SerializeList(serializer, ref Graveyard);
        }

        private void SerializeList<T>(BufferSerializer<T> serializer, ref List<SerializedCard> list) where T : IReaderWriter
        {
            if (serializer.IsWriter)
            {
                int count = list.Count;
                serializer.SerializeValue(ref count);

                foreach (var card in list)
                {
                    SerializedCard sCard = card; // 로그 확인해보면, sCard는 null 아님!
                    serializer.SerializeNetworkSerializable(ref sCard); // sCard null 오류 발생!
                }
            }
            else
            {
                int count = 0;
                serializer.SerializeValue(ref count);

                list = new List<SerializedCard>(count); // 크기에 맞게 새 리스트 생성
                for (int i = 0; i < count; i++)
                {
                    SerializedCard sCard = new SerializedCard();
                    serializer.SerializeNetworkSerializable(ref sCard);
                    list.Add(sCard);
                }
            }
        }
    }

    public class SerializedTile : INetworkSerializable
    {
        public string Name;
        public Space Space;
        public int AgentIndex;
        public SerializedTile() { }

        public SerializedTile(Tile tile)
        {
            Name = tile.Name;
            Space = tile.Space;
            AgentIndex = tile.AgentIndex;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Name);
            serializer.SerializeValue(ref Space);
            serializer.SerializeValue(ref AgentIndex);
        }
    }

    public class SerializedBoard : INetworkSerializable
    {
        public List<SerializedTile> Tiles;
        public SerializedBoard() { }
        public SerializedBoard(Board board)
        {
            Tiles = board.Tiles.Select(tile => new SerializedTile(tile)).ToList();
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if(serializer.IsWriter)
            {
                int count = (Tiles != null) ? Tiles.Count : 0;
                serializer.SerializeValue(ref count);
                for(int i = 0; i < count; i++)
                {
                    SerializedTile tile = Tiles[i];
                    serializer.SerializeNetworkSerializable(ref tile);
                    Tiles[i] = tile;
                }
            }
            else
            {
                int count = 0;
                serializer.SerializeValue(ref count);
                Tiles = new List<SerializedTile>(count);
                for(int i = 0; i < count; i++)
                {
                    SerializedTile tile = new();
                    serializer.SerializeNetworkSerializable(ref tile);
                    Tiles.Add(tile);
                }
            }
        }
    }

    public class SerializedMatch : INetworkSerializable
    {
        public uint ID;
        public SerializedBoard Board;
        public List<SerializedPlayer> Players;
        public int FirstPlayerIndex;
        public int CurrentPlayerIndex;
        public List<bool> Opened;

        public SerializedMatch() { }

        public SerializedMatch(Match match)
        {
            ID = match.ID;
            Board = new SerializedBoard(match.Board);
            Players = new();
            foreach (var player in match.Players)
            {
                Players.Add(new SerializedPlayer(player));
            }
            FirstPlayerIndex = match.FirstPlayerIndex;
            CurrentPlayerIndex = match.CurrentPlayerIndex;
            Opened = match.Opened;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ID);
            serializer.SerializeNetworkSerializable(ref Board);
            serializer.SerializeValue(ref FirstPlayerIndex);
            serializer.SerializeValue(ref CurrentPlayerIndex);

            if (serializer.IsWriter)
            {
                int count = (Players != null) ? Players.Count : 0;
                serializer.SerializeValue(ref count);
                for (int i = 0; i < count; i++)
                {
                    SerializedPlayer player = Players[i];
                    serializer.SerializeNetworkSerializable(ref player);
                    Players[i] = player;
                }
            }
            else
            {
                int count = 0;
                serializer.SerializeValue(ref count);
                Players = new List<SerializedPlayer>(count);
                for (int i = 0; i < count; i++)
                {
                    SerializedPlayer player = new SerializedPlayer();
                    serializer.SerializeNetworkSerializable(ref player);
                    Players.Add(player);
                }
            }

            if (serializer.IsWriter)
            {
                int count = (Opened != null) ? Opened.Count : 0;
                serializer.SerializeValue(ref count);
                for(int i = 0; i < count; i++)
                {
                    bool value = Opened[i];
                    serializer.SerializeValue(ref value);
                }
            }
            else
            {
                int count = 0;
                serializer.SerializeValue(ref count);
                Opened = new List<bool>(count);
                for(int i = 0; i < count; i++)
                {
                    bool value = false;
                    serializer.SerializeValue(ref value);
                    Opened.Add(value);
                }
            }
        }
    }
}