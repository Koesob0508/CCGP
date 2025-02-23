using CCGP.Server;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;

namespace CCGP.Shared
{
    public class SerializedDrawCardsAction : INetworkSerializable
    {
        public SerializedPlayer Player;
        public uint Amount;
        public List<SerializedCard> Cards;

        public SerializedDrawCardsAction() { }

        public SerializedDrawCardsAction(DrawCardsAction action)
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
            SerializedData.SerializeList(serializer, ref Cards);
        }
    }

    public class SerializedRoundStartDrawAction : INetworkSerializable
    {
        public List<SerializedCard> Cards1;
        public List<SerializedCard> Cards2;
        public List<SerializedCard> Cards3;
        public List<SerializedCard> Cards4;

        public SerializedRoundStartDrawAction() { }
        public SerializedRoundStartDrawAction(RoundStartDrawAction action)
        {
            Cards1 = action[0].Select(card => new SerializedCard(card)).ToList();
            Cards2 = action[1].Select(card => new SerializedCard(card)).ToList();
            Cards3 = action[2].Select(card => new SerializedCard(card)).ToList();
            Cards4 = action[3].Select(card => new SerializedCard(card)).ToList();
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            SerializedData.SerializeList(serializer, ref Cards1);
            SerializedData.SerializeList(serializer, ref Cards2);
            SerializedData.SerializeList(serializer, ref Cards3);
            SerializedData.SerializeList(serializer, ref Cards4);
        }

        public List<SerializedCard> this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return Cards1;
                    case 1:
                        return Cards2;
                    case 2:
                        return Cards3;
                    case 3:
                        return Cards4;
                    default:
                        LogUtility.LogError<SerializedRoundStartDrawAction>($"잘못된 값입니다. : {i}");
                        return null;
                }
            }
        }
    }
}