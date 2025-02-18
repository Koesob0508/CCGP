using CCGP.Server;
using System.Collections.Generic;
using Unity.Netcode;

namespace CCGP.Shared
{
    public class SerializedCardsDrawAction : INetworkSerializable
    {
        public SerializedPlayer Player;
        public int Amount;
        public List<SerializedCard> Cards;

        public SerializedCardsDrawAction() { }

        public SerializedCardsDrawAction(CardsDrawAction action)
        {
            Player = new SerializedPlayer(action.Player);
            Amount = action.Amount;
            Cards = new();
            foreach (var card in action.Cards)
            {
                Cards.Add(new SerializedCard(card));
            }
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeNetworkSerializable(ref Player);
            serializer.SerializeValue(ref Amount);
            SerializeList(serializer, ref Cards);
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

    public class SerializedTurnEndAction : INetworkSerializable
    {
        public SerializedPlayer Player;
        public int TargetPlayerIndex;
        public int NextPlayerIndex;

        public SerializedTurnEndAction() { }

        public SerializedTurnEndAction(TurnEndAction action)
        {
            Player = new SerializedPlayer(action.Player);
            TargetPlayerIndex = action.TargetPlayerIndex;
            NextPlayerIndex = action.NextPlayerIndex;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeNetworkSerializable(ref Player);
            serializer.SerializeValue(ref TargetPlayerIndex);
            serializer.SerializeValue(ref NextPlayerIndex);
        }
    }
}