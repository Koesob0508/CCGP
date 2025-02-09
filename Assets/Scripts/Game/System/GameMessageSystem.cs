using CCGP.AspectContainer;
using CCGP.Shared;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;

namespace CCGP.Server
{
    public class GameMessageSystem : Aspect, IObserve
    {
        private Dictionary<ushort, Action<ulong, SerializedData>> Handlers;

        public void Awake()
        {
            Handlers = new();

            RegisterBaseHandler();
            RegisterObserver();

            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("ToServerGame", OnReceivedMessage);
        }

        public void Sleep()
        {
            NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler("ToServerGame");

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
                LogUtility.LogWarning<GameMessageSystem>($"Unknown command received : {gameCommand}");
            }
        }

        private void RegisterBaseHandler()
        {
        }

        private void RegisterObserver()
        {
            this.AddObserver(OnSendStartGame, Global.PerformNotification<GameStartAction>(), Container);
        }

        private void OnSendStartGame(object sender, object args)
        {
            var game = Container as Game;
            var match = Container.GetMatch();

            foreach (var playerInfo in game.PlayerInfos)
            {
                var sMatch = new SerializedMatch(playerInfo.ClientID, match);
                Send(GameCommand.StartGameToClient, playerInfo.ClientID, sMatch, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }

        #region Send Utilities

        private void Send(ushort tag, ulong clientID, INetworkSerializable data = null, NetworkDelivery delivery = NetworkDelivery.ReliableSequenced)
        {
            using (var writer = new FastBufferWriter(128, Allocator.Temp, 1024 * 1024))
            {
                writer.WriteValueSafe(tag);
                if (data != null)
                {
                    writer.WriteNetworkSerializable(data);
                }
                NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("ToClientGame", clientID, writer, delivery);
            }
        }

        #endregion
    }
}