using CCGP.AspectContainer;
using CCGP.Shared;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;

namespace CCGP.Client
{
    public class ClientMessageSystem : Aspect, IActivatable
    {
        private Dictionary<ushort, Action<ulong, SerializedData>> Handlers;

        public void Activate()
        {
            Handlers = new();

            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("ToClientGame", OnReceivedMessage);
        }

        public void Deactivate()
        {

        }

        public void RegisterHandler(ushort type, Action<ulong, SerializedData> callback)
        {
            Handlers.Add(type, callback);
        }

        private void OnReceivedMessage(ulong clientID, FastBufferReader reader)
        {
            reader.ReadValueSafe(out ushort command);
            SerializedData sdata = new SerializedData(reader);
            // LogUtility.Log($"{(GameCommand)command}");
            Container.PostNotification(Global.MessageNotification((GameCommand)command), sdata);
        }

        #region Send Utilities

        private void SendToHost(ushort tag, INetworkSerializable data = null, NetworkDelivery delivery = NetworkDelivery.ReliableSequenced)
        {
            using (var writer = new FastBufferWriter(128, Allocator.Temp, 1024 * 1024))
            {
                writer.WriteValueSafe(tag);
                if (data != null)
                {
                    writer.WriteNetworkSerializable(data);
                }
                NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("ToServerGame", NetworkManager.ServerClientId, writer, delivery);
            }
        }

        #endregion
    }
}
