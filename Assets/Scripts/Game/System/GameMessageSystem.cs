using CCGP.AspectContainer;
using CCGP.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine.PlayerLoop;

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

            UnregisterObserver();

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
            // Phase
            this.AddObserver(OnStartGame, Global.PerformNotification<GameStartAction>(), Container);
            this.AddObserver(OnStartRound, Global.PerformNotification<RoundStartAction>(), Container);
            this.AddObserver(OnStartTurn, Global.PerformNotification<TurnStartAction>(), Container);
            this.AddObserver(OnEndTurn, Global.PerformNotification<TurnEndAction>(), Container);
            this.AddObserver(OnEndRound, Global.PerformNotification<RoundEndAction>(), Container);
            this.AddObserver(OnEndGame, Global.PerformNotification<GameEndAction>(), Container);

            // Not Phase, But Change
            this.AddObserver(OnDrawCards, Global.PerformNotification<DrawCardsAction>(), Container);
            this.AddObserver(OnPlayCard, Global.PerformNotification<PlayCardAction>(), Container);
            this.AddObserver(OnGainResources, Global.PerformNotification<GainResourcesAction>(), Container);
            this.AddObserver(OnGenerateCard, Global.PerformNotification<GenerateCardAction>(), Container);

            // Not Phase, Not Change, Just Notify
            this.AddObserver(OnCancelTryPlayCard, Global.CancelNotification<TryPlayCardAction>(), Container);
            this.AddObserver(OnCancelPlayCard, Global.CancelNotification<PlayCardAction>(), Container);
            this.AddObserver(OnShowAvailableTiles, Global.MessageNotification(GameCommand.ShowAvailableTiles), Container);
        }

        private void UnregisterObserver()
        {
            // Phase
            this.RemoveObserver(OnStartGame, Global.PerformNotification<GameStartAction>(), Container);
            this.RemoveObserver(OnStartRound, Global.PerformNotification<RoundStartAction>(), Container);
            this.RemoveObserver(OnStartTurn, Global.PerformNotification<TurnStartAction>(), Container);
            this.RemoveObserver(OnEndTurn, Global.PerformNotification<TurnEndAction>(), Container);
            this.RemoveObserver(OnEndRound, Global.PerformNotification<RoundEndAction>(), Container);
            this.RemoveObserver(OnEndGame, Global.PerformNotification<GameEndAction>(), Container);

            // Not Phase, But Change
            this.RemoveObserver(OnDrawCards, Global.PerformNotification<DrawCardsAction>(), Container);
            this.RemoveObserver(OnPlayCard, Global.PerformNotification<PlayCardAction>(), Container);
            this.RemoveObserver(OnGainResources, Global.PerformNotification<GainResourcesAction>(), Container);
            this.RemoveObserver(OnGenerateCard, Global.PerformNotification<GenerateCardAction>(), Container);

            // Not Phase, Not Change, Just Notify
            this.RemoveObserver(OnCancelTryPlayCard, Global.CancelNotification<TryPlayCardAction>(), Container);
            this.RemoveObserver(OnCancelPlayCard, Global.CancelNotification<PlayCardAction>(), Container);
            this.RemoveObserver(OnShowAvailableTiles, Global.MessageNotification(GameCommand.ShowAvailableTiles), Container);
        }

        /// <summary>
        /// Send Update Data.
        /// </summary>
        private void UpdateData()
        {
            LogUtility.Log<GameMessageSystem>("Send Update Data", colorName: ColorCodes.Server);

            var match = Container.GetMatch();

            foreach (var playerInfo in Game.PlayerInfos)
            {
                var sMatch = new SerializedMatch(playerInfo.ClientID, match);
                Send((ushort)GameCommand.UpdateData, playerInfo.ClientID, sMatch, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }

        #region Phase Message

        private void OnStartGame(object sender, object args)
        {
            UpdateData();

            LogUtility.Log<GameMessageSystem>("Send Game Start", colorName: ColorCodes.Server);

            foreach (var playerInfo in Game.PlayerInfos)
            {
                Send((ushort)GameCommand.StartGame, playerInfo.ClientID, null, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }

        private void OnStartRound(object sender, object args)
        {
            UpdateData();

            LogUtility.Log<GameMessageSystem>("Send Round Start", colorName: ColorCodes.Server);

            foreach (var playerInfo in Game.PlayerInfos)
            {
                Send((ushort)GameCommand.StartRound, playerInfo.ClientID, null, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }

        private void OnStartTurn(object sender, object args)
        {
            UpdateData();

            LogUtility.Log<GameMessageSystem>("Send Turn Start", colorName: ColorCodes.Server);

            foreach (var playerInfo in Game.PlayerInfos)
            {
                Send((ushort)GameCommand.StartTurn, playerInfo.ClientID, null, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }

        private void OnEndTurn(object sender, object args)
        {
            UpdateData();

            LogUtility.Log<GameMessageSystem>("Send Turn End", colorName: ColorCodes.Server);

            foreach (var playerInfo in Game.PlayerInfos)
            {
                Send((ushort)GameCommand.EndTurn, playerInfo.ClientID, null, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }

        private void OnEndRound(object sender, object args)
        {
            UpdateData();

            LogUtility.Log<GameMessageSystem>("Send Round End", colorName: ColorCodes.Logic);

            foreach (var playerInfo in Game.PlayerInfos)
            {
                Send((ushort)GameCommand.EndRound, playerInfo.ClientID, null, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }

        private void OnEndGame(object sender, object args)
        {
            UpdateData();

            LogUtility.Log<GameMessageSystem>("Send Game End", colorName: ColorCodes.Logic);

            foreach (var playerInfo in Game.PlayerInfos)
            {
                Send((ushort)GameCommand.EndGame, playerInfo.ClientID, null, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }

        #endregion

        #region Not Phase, But Change : Need Update Data
        private void OnDrawCards(object sender, object args)
        {
            UpdateData();

            LogUtility.Log<GameMessageSystem>("Send Card Draw", colorName: ColorCodes.Logic);

            var action = args as DrawCardsAction;
            var sAction = new SerializedCardsDrawAction(action);

            foreach (var playerInfo in Game.PlayerInfos)
            {
                Send((ushort)GameCommand.DrawCards, playerInfo.ClientID, sAction, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }

        private void OnPlayCard(object sender, object args)
        {
            UpdateData();

            LogUtility.Log<GameMessageSystem>("Send Play Card", colorName: ColorCodes.Server);

            var action = args as PlayCardAction;
            var sCard = new SerializedCard(action.Card);

            Send((ushort)GameCommand.PlayCard, action.Player.ID, sCard, NetworkDelivery.ReliableFragmentedSequenced);
        }

        private void OnGainResources(object sender, object args)
        {
            UpdateData();

            LogUtility.Log<GameMessageSystem>("Send Gain Resource", colorName: ColorCodes.Server);

            foreach (var playerInfo in Game.PlayerInfos)
            {
                Send((ushort)GameCommand.GainResources, playerInfo.ClientID, null, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }

        private void OnGenerateCard(object sender, object args)
        {
            UpdateData();

            LogUtility.Log<GameMessageSystem>("Send Generate Card", colorName: ColorCodes.Server);

            var action = args as GenerateCardAction;
            var sCard = new SerializedCard(action.Card);

            foreach (var playerInfo in Game.PlayerInfos)
            {
                Send((ushort)GameCommand.GenerateCard, playerInfo.ClientID, sCard, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }
        #endregion

        #region Not Phase, Not Change, Just Notify : No Need Update Data

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
        private void OnCancelTryPlayCard(object sender, object args)
        {
            LogUtility.Log<GameMessageSystem>("Send Cancel Try Play Card", colorName: ColorCodes.Server);

            var action = args as TryPlayCardAction;
            var sCard = new SerializedCard(action.Card);

            Send((ushort)GameCommand.CancelTryPlayCard, action.Player.ID, sCard, NetworkDelivery.ReliableFragmentedSequenced);
        }

        private void OnCancelPlayCard(object sender, object args)
        {
            LogUtility.Log<GameMessageSystem>("Send Cancel Play Card", colorName: ColorCodes.Server);

            var action = args as PlayCardAction;
            var sCard = new SerializedCard(action.Card);

            Send((ushort)GameCommand.CancelPlayCard, action.Player.ID, sCard, NetworkDelivery.ReliableFragmentedSequenced);
        }

        #endregion

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