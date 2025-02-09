using CCGP.AspectContainer;
using CCGP.Shared;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;

namespace CCGP.Client
{
    public class ClientMessageSystem : Aspect, IObserve
    {
        private Dictionary<ushort, Action<ulong, SerializedData>> Handlers;

        public void Awake()
        {
            Handlers = new();

            RegisterBaseHandler();
            RegisterObserver();

            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("ToClientGame", OnReceivedMessage);
        }

        public void Sleep()
        {
            NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler("ToClientGame");

            Handlers = null;
        }

        public void RegisterHandler(ushort type, Action<ulong, SerializedData> callback)
        {
            Handlers.Add(type, callback);
        }

        private void OnReceivedMessage(ulong clientID, FastBufferReader reader)
        {
            reader.ReadValueSafe(out ushort gameCommand);
            SerializedData sdata = new SerializedData(reader);

            if (Handlers.TryGetValue(gameCommand, out var handler))
            {
                handler(clientID, sdata);
            }
            else
            {
                LogUtility.LogWarning<ClientMessageSystem>($"Unknown command received : {gameCommand}");
            }
        }

        private void RegisterBaseHandler()
        {
            RegisterHandler(GameCommand.StartGameToClient, OnReceivedStartGame);
        }

        private void RegisterObserver()
        {

        }

        private void OnReceivedStartGame(ulong clientID, SerializedData sData)
        {
            var sMatch = sData.Get<SerializedMatch>();
            var match = new MatchView(sMatch);

            LogUtility.Log<ClientMessageSystem>($"게임 시작, {match.YourIndex} {match.Players[match.YourIndex].LobbyID}", ColorCodes.Client);
            Container.PostNotification("StartGame");
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
