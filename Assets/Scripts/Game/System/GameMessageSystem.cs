using CCGP.AspectContainer;
using CCGP.Shared;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;

namespace CCGP.Server
{
    public class GameMessageSystem : Aspect, IActivatable
    {
        private Dictionary<ushort, Action<ulong, SerializedData>> Handlers;
        private Game Game => Container as Game;

        public void Activate()
        {
            Handlers = new();

            RegisterBaseHandler();
            RegisterObserver();

            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("ToServerGame", OnReceivedMessage);
        }

        public void Deactivate()
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
            this.AddObserver(OnStartGame, Global.PerformNotification<GameStartAction>(), Container);
            this.AddObserver(OnStartRound, Global.PerformNotification<RoundStartAction>(), Container);
            this.AddObserver(OnDrawCards, Global.PerformNotification<CardsDrawAction>(), Container);
        }

        private void OnStartGame(object sender, object args)
        {
            LogUtility.Log<GameMessageSystem>("OnGameStart Send", colorName: ColorCodes.Server);
            var match = Container.GetMatch();

            foreach (var playerInfo in Game.PlayerInfos)
            {
                var sMatch = new SerializedMatch(playerInfo.ClientID, match);
                Send((ushort)GameCommand.StartGame, playerInfo.ClientID, sMatch, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }

        private void OnStartRound(object sender, object args)
        {
            LogUtility.Log<GameMessageSystem>("OnRoundStart Send", colorName: ColorCodes.Server);

            var match = Container.GetMatch();

            foreach(var playerInfo in Game.PlayerInfos)
            {
                var sMatch = new SerializedMatch(playerInfo.ClientID, match);
                Send((ushort)GameCommand.StartRound, playerInfo.ClientID, sMatch, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }

        private void OnDrawCards(object sender, object args)
        {
            LogUtility.Log<GameMessageSystem>("OnCardDraw Send", colorName: ColorCodes.Server);

            var action = args as CardsDrawAction;
            var sAction = new SerializedCardsDrawAction(action);

            foreach (var playerInfo in Game.PlayerInfos)
            {
                Send((ushort)GameCommand.DrawCards, playerInfo.ClientID, sAction, NetworkDelivery.ReliableFragmentedSequenced);
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