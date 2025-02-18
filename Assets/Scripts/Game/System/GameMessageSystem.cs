using CCGP.AspectContainer;
using CCGP.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            reader.ReadValueSafe(out ushort command);
            SerializedData sdata = new SerializedData(reader);
            LogUtility.Log<GameMessageSystem>($"On received {(GameCommand)command}", colorName: ColorCodes.Server);
            Container.PostNotification(Global.MessageNotification((GameCommand)command), sdata);
        }

        private void RegisterObserver()
        {
            this.AddObserver(OnStartGame, Global.PerformNotification<GameStartAction>(), Container);
            this.AddObserver(OnStartRound, Global.PerformNotification<RoundStartAction>(), Container);
            this.AddObserver(OnDrawCards, Global.PerformNotification<CardsDrawAction>(), Container);
            this.AddObserver(OnPlayCard, Global.PerformNotification<CardPlayAction>(), Container);
            this.AddObserver(OnCancelPlayCard, Global.CancelNotification<CardPlayAction>(), Container);
            this.AddObserver(OnShowAvailableTiles, Global.MessageNotification(GameCommand.ShowAvailableTiles), Container);
        }

        private void OnStartGame(object sender, object args)
        {
            LogUtility.Log<GameMessageSystem>("Send Game Start", colorName: ColorCodes.Server);
            var match = Container.GetMatch();

            foreach (var playerInfo in Game.PlayerInfos)
            {
                var sMatch = new SerializedMatch(playerInfo.ClientID, match);
                Send((ushort)GameCommand.StartGame, playerInfo.ClientID, sMatch, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }

        private void OnStartRound(object sender, object args)
        {
            LogUtility.Log<GameMessageSystem>("Send Round Start", colorName: ColorCodes.Server);

            var match = Container.GetMatch();

            foreach (var playerInfo in Game.PlayerInfos)
            {
                var sMatch = new SerializedMatch(playerInfo.ClientID, match);
                Send((ushort)GameCommand.StartRound, playerInfo.ClientID, sMatch, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }

        private void OnDrawCards(object sender, object args)
        {
            LogUtility.Log<GameMessageSystem>("Send Card Draw", colorName: ColorCodes.Server);

            var action = args as CardsDrawAction;
            var sAction = new SerializedCardsDrawAction(action);

            foreach (var playerInfo in Game.PlayerInfos)
            {
                Send((ushort)GameCommand.DrawCards, playerInfo.ClientID, sAction, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }

        private void OnShowAvailableTiles(object sender, object args)
        {
            LogUtility.Log<GameMessageSystem>("Send Show Available Tiles", colorName: ColorCodes.Server);

            var tiles = args as List<Tile>;
            var sTiles = new SerializedTiles(tiles);
            Container.TryGetAspect<ActionSystem>(out var actionSystem);
            if (actionSystem == null)
            {
                LogUtility.Log("ActionSystem is null");
            }
            else if (actionSystem.CurrentAction == null)
            {
                LogUtility.Log("CurrentAction is null");
            }
            else if (actionSystem.CurrentAction.Player == null)
            {
                LogUtility.Log($"{actionSystem.CurrentAction.GetType().Name}");
                LogUtility.Log("CurrentAction.Player is null");
            }

            var targetID = actionSystem.CurrentAction.Player.ID;

            Send((ushort)GameCommand.ShowAvailableTiles, targetID, sTiles, NetworkDelivery.ReliableFragmentedSequenced);
        }

        private void OnPlayCard(object sender, object args)
        {
            LogUtility.Log<GameMessageSystem>("Send Play Card", colorName: ColorCodes.Server);

            var action = args as CardPlayAction;
            var sCard = new SerializedCard(action.Card);

            Send((ushort)GameCommand.PlayCard, action.Player.ID, sCard, NetworkDelivery.ReliableFragmentedSequenced);
        }

        private void OnCancelPlayCard(object sender, object args)
        {
            LogUtility.Log<GameMessageSystem>("Send Cancel Play Card", colorName: ColorCodes.Server);

            var action = args as CardPlayAction;
            var sCard = new SerializedCard(action.Card);

            Send((ushort)GameCommand.CancelPlayCard, action.Player.ID, sCard, NetworkDelivery.ReliableFragmentedSequenced);
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