using CCGP.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

namespace CCGP.Shared
{
    public class SerializedData
    {
        private byte[] data;
        private int validLength;

        public SerializedData(FastBufferReader reader)
        {
            // FastBufferReader의 ToArray()로 전체 버퍼를 관리형 배열로 가져옵니다.
            byte[] fullData = reader.ToArray();

            // 남은 데이터 길이 계산
            int remaining = reader.Length - reader.Position;
            validLength = remaining;

            // 현재 읽기 위치부터 남은 데이터만 복사합니다.
            data = new byte[remaining];
            Array.Copy(fullData, reader.Position, data, 0, remaining);
        }

        public T Get<T>() where T : INetworkSerializable, new()
        {
            using (var newReader = new FastBufferReader(data, Unity.Collections.Allocator.Temp, validLength))
            {
                newReader.ReadNetworkSerializable(out T val);

                return val;
            }
        }

        public static void SerializeList<TSerializer, TItem>(BufferSerializer<TSerializer> serializer, ref List<TItem> list)
            where TSerializer : IReaderWriter
            where TItem : INetworkSerializable, new()
        {
            if (serializer.IsWriter)
            {
                int count = list.Count;
                serializer.SerializeValue(ref count);

                foreach (var item in list)
                {
                    TItem serializedItem = item;
                    serializer.SerializeNetworkSerializable(ref serializedItem);
                }
            }
            else
            {
                int count = 0;
                serializer.SerializeValue(ref count);

                list = new List<TItem>(count);
                for (int i = 0; i < count; i++)
                {
                    TItem serializedItem = new TItem();
                    serializer.SerializeNetworkSerializable(ref serializedItem);
                    list.Add(serializedItem);
                }
            }
        }
    }
    public class SerializedPlayerInfo : INetworkSerializable
    {
        public string LobbyID;
        public ulong ClientID;

        public SerializedPlayerInfo() { }

        public SerializedPlayerInfo(PlayerInfo playerInfo)
        {
            LobbyID = playerInfo.LobbyID;
            ClientID = playerInfo.ClientID;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref LobbyID);
            serializer.SerializeValue(ref ClientID);
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
        public ulong ClientID;
        public int Index;
        public string LobbyID;
        public uint BaseAgentCount;
        public uint MentatCount;
        public uint TotalAgentCount;
        public uint UsedAgentCount;
        public uint TurnActionCount;
        public bool IsOpened;
        public uint Lunar;
        public uint Marsion;
        public uint Water;
        public uint EmperorInfluence;
        public uint SpacingGuildInfluence;
        public uint BeneGesseritInfluence;
        public uint FremenInfluence;

        public uint Troop;
        public uint Persuasion;
        public uint BasePersuasion;
        public uint VictoryPoint;

        public List<SerializedCard> Leader;
        public List<SerializedCard> Deck;
        public List<SerializedCard> Hand;
        public List<SerializedCard> Graveyard;
        public List<SerializedCard> Agent;
        public List<SerializedCard> Open;

        public SerializedPlayer() { }

        public SerializedPlayer(Player player)
        {
            ClientID = player.ID;
            Index = player.Index;
            LobbyID = player.PlayerInfo.LobbyID;

            BaseAgentCount = player.BaseAgentCount;
            MentatCount = player.MentatCount;
            TotalAgentCount = player.TotalAgentCount;
            UsedAgentCount = player.UsedAgentCount;
            TurnActionCount = player.TurnActionCount;
            IsOpened = player.IsOpened;

            Lunar = player.Lunar;
            Marsion = player.Marsion;
            Water = player.Water;

            EmperorInfluence = player.EmperorInfluence;
            SpacingGuildInfluence = player.SpacingGuildInfluence;
            BeneGesseritInfluence = player.BeneGesseritInfluence;
            FremenInfluence = player.FremenInfluence;

            Troop = player.Troop;
            Persuasion = player.Persuasion;
            BasePersuasion = player.BasePersuasion;
            VictoryPoint = player.VictoryPoint;

            Leader = new();
            foreach (var card in player[Zone.Leader])
            {
                Leader.Add(new SerializedCard(card));
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
            serializer.SerializeValue(ref ClientID);
            serializer.SerializeValue(ref Index);
            serializer.SerializeValue(ref LobbyID);
            serializer.SerializeValue(ref BaseAgentCount);
            serializer.SerializeValue(ref MentatCount);
            serializer.SerializeValue(ref TotalAgentCount);
            serializer.SerializeValue(ref UsedAgentCount);
            serializer.SerializeValue(ref TurnActionCount);
            serializer.SerializeValue(ref IsOpened);
            serializer.SerializeValue(ref Lunar);
            serializer.SerializeValue(ref Marsion);
            serializer.SerializeValue(ref Water);
            serializer.SerializeValue(ref EmperorInfluence);
            serializer.SerializeValue(ref SpacingGuildInfluence);
            serializer.SerializeValue(ref BeneGesseritInfluence);
            serializer.SerializeValue(ref FremenInfluence);
            serializer.SerializeValue(ref Troop);
            serializer.SerializeValue(ref Persuasion);
            serializer.SerializeValue(ref BasePersuasion);
            serializer.SerializeValue(ref VictoryPoint);

            SerializedData.SerializeList(serializer, ref Leader);
            SerializedData.SerializeList(serializer, ref Deck);
            SerializedData.SerializeList(serializer, ref Hand);
            SerializedData.SerializeList(serializer, ref Graveyard);
            SerializedData.SerializeList(serializer, ref Agent);
            SerializedData.SerializeList(serializer, ref Open);
        }
    }

    public class SerializedTile : INetworkSerializable
    {
        public string Name;
        public Space Space;
        public int AgentIndex;
        public string Description;
        public ConditionType ConditionType;
        public uint ConditionAmount;
        public ResourceType CostType;
        public uint CostAmount;
        public SerializedTile() { }

        public SerializedTile(Tile tile)
        {
            Name = tile.Name;
            Space = tile.Space;
            AgentIndex = tile.AgentIndex;
            Description = tile.Description;

            if (tile.TryGetAspect(out Condition condition))
            {
                ConditionType = condition.Type;
                ConditionAmount = condition.Amount;
            }
            else
            {
                ConditionType = ConditionType.None;
                ConditionAmount = 0;
            }

            if (tile.TryGetAspect(out Cost cost))
            {
                CostType = cost.Type;
                CostAmount = cost.Amount;
            }
            else
            {
                CostType = ResourceType.None;
                CostAmount = 0;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is SerializedTile tile &&
                   Name == tile.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Name);
            serializer.SerializeValue(ref Space);
            serializer.SerializeValue(ref AgentIndex);
            serializer.SerializeValue(ref Description);
            serializer.SerializeValue(ref ConditionType);
            serializer.SerializeValue(ref ConditionAmount);
            serializer.SerializeValue(ref CostType);
            serializer.SerializeValue(ref CostAmount);
        }
    }

    public class SerializedTiles : INetworkSerializable
    {
        public List<SerializedTile> Tiles;

        public SerializedTiles() { }
        public SerializedTiles(List<Tile> tiles)
        {
            Tiles = new();
            foreach (var tile in tiles)
            {
                Tiles.Add(new SerializedTile(tile));
            }
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            SerializedData.SerializeList(serializer, ref Tiles);
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
            SerializedData.SerializeList(serializer, ref Tiles);
        }
    }

    public class SerializedImperium : INetworkSerializable
    {
        public List<SerializedCard> Deck;
        public List<SerializedCard> Row;

        public SerializedImperium() { }

        public SerializedImperium(Imperium imperium)
        {
            Deck = imperium.Deck?.Select(card => new SerializedCard(card)).ToList();
            Row = imperium.Row?.Select(card => new SerializedCard(card)).ToList();
        }
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            SerializedData.SerializeList(serializer, ref Deck);
            SerializedData.SerializeList(serializer, ref Row);
        }
    }

    public class SerializedMatch : INetworkSerializable
    {
        private ulong targetClientID;
        public int YourIndex;
        public List<SerializedPlayer> Players;
        public SerializedBoard Board;
        public SerializedImperium Imperium;
        public int FirstPlayerIndex;
        public int CurrentPlayerIndex;
        public List<bool> Opened;

        public SerializedMatch() { }

        public SerializedMatch(ulong targetClientID, Match match)
        {
            this.targetClientID = targetClientID;

            Players = new();
            Board = new SerializedBoard(match.Board);
            Imperium = new SerializedImperium(match.Imperium);
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
            serializer.SerializeNetworkSerializable(ref Board);
            serializer.SerializeNetworkSerializable(ref Imperium);
            serializer.SerializeValue(ref FirstPlayerIndex);
            serializer.SerializeValue(ref CurrentPlayerIndex);

            if (serializer.IsWriter)
            {
                YourIndex = (int)targetClientID;
                serializer.SerializeValue(ref YourIndex);

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
                serializer.SerializeValue(ref YourIndex);

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
                for (int i = 0; i < count; i++)
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
                for (int i = 0; i < count; i++)
                {
                    bool value = false;
                    serializer.SerializeValue(ref value);
                    Opened.Add(value);
                }
            }
        }
    }
}