using CCGP.AspectContainer;
using CCGP.Shared;
using Cysharp.Threading.Tasks;
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
            this.AddObserver(OnStartGame, Global.PerformNotification<StartGameAction>(), Container);
            this.AddObserver(OnStartRound, Global.PerformNotification<StartRoundAction>(), Container);
            this.AddObserver(OnStartTurn, Global.PerformNotification<StartTurnAction>(), Container);
            this.AddObserver(OnEndTurn, Global.PerformNotification<EndTurnAction>(), Container);
            this.AddObserver(OnEndRound, Global.PerformNotification<EndRoundAction>(), Container);
            this.AddObserver(OnEndGame, Global.PerformNotification<EndGameAction>(), Container);

            // Not Phase, But Change
            this.AddObserver(OnRoundStartDraw, Global.PerformNotification<RoundStartDrawAction>(), Container);
            this.AddObserver(OnDrawCards, Global.PerformNotification<DrawCardsAction>(), Container);
            this.AddObserver(OnPlayCard, Global.PerformNotification<PlayCardAction>(), Container);
            this.AddObserver(OnGainResources, Global.PerformNotification<GainResourcesAction>(), Container);
            this.AddObserver(OnGenerateCard, Global.PerformNotification<GenerateCardAction>(), Container);
            this.AddObserver(OnOpenCards, Global.PerformNotification<OpenCardsAction>(), Container);

            // Not Phase, Not Change, Just Notify
            this.AddObserver(OnCancelTryPlayCard, Global.CancelNotification<TryPlayCardAction>(), Container);
            this.AddObserver(OnCancelPlayCard, Global.CancelNotification<PlayCardAction>(), Container);
            this.AddObserver(OnShowAvailableTiles, Global.MessageNotification(GameCommand.ShowAvailableTiles), Container);
        }

        private void UnregisterObserver()
        {
            // Phase
            this.RemoveObserver(OnStartGame, Global.PerformNotification<StartGameAction>(), Container);
            this.RemoveObserver(OnStartRound, Global.PerformNotification<StartRoundAction>(), Container);
            this.RemoveObserver(OnStartTurn, Global.PerformNotification<StartTurnAction>(), Container);
            this.RemoveObserver(OnEndTurn, Global.PerformNotification<EndTurnAction>(), Container);
            this.RemoveObserver(OnEndRound, Global.PerformNotification<EndRoundAction>(), Container);
            this.RemoveObserver(OnEndGame, Global.PerformNotification<EndGameAction>(), Container);

            // Not Phase, But Change
            this.RemoveObserver(OnRoundStartDraw, Global.PerformNotification<RoundStartDrawAction>(), Container);
            this.RemoveObserver(OnDrawCards, Global.PerformNotification<DrawCardsAction>(), Container);
            this.RemoveObserver(OnPlayCard, Global.PerformNotification<PlayCardAction>(), Container);
            this.RemoveObserver(OnGainResources, Global.PerformNotification<GainResourcesAction>(), Container);
            this.RemoveObserver(OnGenerateCard, Global.PerformNotification<GenerateCardAction>(), Container);
            this.RemoveObserver(OnOpenCards, Global.PerformNotification<OpenCardsAction>(), Container);

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

        private async void OnStartGame(object sender, object args)
        {
            UpdateData();

            await UniTask.Yield();

            LogUtility.Log<GameMessageSystem>("Send Game Start", colorName: ColorCodes.Server);

            foreach (var playerInfo in Game.PlayerInfos)
            {
                Send((ushort)GameCommand.StartGame, playerInfo.ClientID, null, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }

        private async void OnStartRound(object sender, object args)
        {
            UpdateData();

            await UniTask.Yield();

            LogUtility.Log<GameMessageSystem>("Send Round Start", colorName: ColorCodes.Server);

            foreach (var playerInfo in Game.PlayerInfos)
            {
                Send((ushort)GameCommand.StartRound, playerInfo.ClientID, null, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }

        private async void OnStartTurn(object sender, object args)
        {
            UpdateData();

            await UniTask.Yield();

            LogUtility.Log<GameMessageSystem>("Send Turn Start", colorName: ColorCodes.Server);

            foreach (var playerInfo in Game.PlayerInfos)
            {
                Send((ushort)GameCommand.StartTurn, playerInfo.ClientID, null, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }

        private async void OnEndTurn(object sender, object args)
        {
            UpdateData();

            await UniTask.Yield();

            LogUtility.Log<GameMessageSystem>("Send Turn End", colorName: ColorCodes.Server);

            foreach (var playerInfo in Game.PlayerInfos)
            {
                Send((ushort)GameCommand.EndTurn, playerInfo.ClientID, null, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }

        private async void OnEndRound(object sender, object args)
        {
            UpdateData();

            await UniTask.Yield();

            LogUtility.Log<GameMessageSystem>("Send Round End", colorName: ColorCodes.Logic);

            foreach (var playerInfo in Game.PlayerInfos)
            {
                Send((ushort)GameCommand.EndRound, playerInfo.ClientID, null, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }

        private async void OnEndGame(object sender, object args)
        {
            UpdateData();

            await UniTask.Yield();

            LogUtility.Log<GameMessageSystem>("Send Game End", colorName: ColorCodes.Logic);

            foreach (var playerInfo in Game.PlayerInfos)
            {
                Send((ushort)GameCommand.EndGame, playerInfo.ClientID, null, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }

        #endregion

        #region Not Phase, But Change : Need Update Data
        private async void OnRoundStartDraw(object sender, object args)
        {
            UpdateData();

            await UniTask.Yield();

            LogUtility.Log<GameMessageSystem>("Send Round Start Card Draw", colorName: ColorCodes.Logic);

            var action = args as RoundStartDrawAction;
            var sAction = new SerializedRoundStartDrawAction(action);


            foreach (var playerInfo in Game.PlayerInfos)
            {
                Send((ushort)GameCommand.RoundStartDraw, playerInfo.ClientID, sAction, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }

        private async void OnDrawCards(object sender, object args)
        {
            UpdateData();

            await UniTask.Yield();

            LogUtility.Log<GameMessageSystem>("Send Card Draw", colorName: ColorCodes.Logic);

            var action = args as DrawCardsAction;
            var sAction = new SerializedDrawCardsAction(action);

            foreach (var playerInfo in Game.PlayerInfos)
            {
                Send((ushort)GameCommand.DrawCards, playerInfo.ClientID, sAction, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }

        private async void OnPlayCard(object sender, object args)
        {
            UpdateData();

            await UniTask.Yield();

            LogUtility.Log<GameMessageSystem>("Send Play Card", colorName: ColorCodes.Server);

            var action = args as PlayCardAction;
            var sCard = new SerializedCard(action.Card);

            Send((ushort)GameCommand.PlayCard, action.Player.ID, sCard, NetworkDelivery.ReliableFragmentedSequenced);
        }

        private async void OnGainResources(object sender, object args)
        {
            UpdateData();

            await UniTask.Yield();

            LogUtility.Log<GameMessageSystem>("Send Gain Resource", colorName: ColorCodes.Server);

            foreach (var playerInfo in Game.PlayerInfos)
            {
                Send((ushort)GameCommand.GainResources, playerInfo.ClientID, null, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }

        private async void OnGenerateCard(object sender, object args)
        {
            UpdateData();

            await UniTask.Yield();

            LogUtility.Log<GameMessageSystem>("Send Generate Card", colorName: ColorCodes.Server);

            var action = args as GenerateCardAction;
            var sCard = new SerializedCard(action.Card);

            foreach (var playerInfo in Game.PlayerInfos)
            {
                Send((ushort)GameCommand.GenerateCard, playerInfo.ClientID, sCard, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }

        private async void OnOpenCards(object sender, object args)
        {
            UpdateData();

            await UniTask.Yield();

            LogUtility.Log<GameMessageSystem>("Send Open Cards", colorName: ColorCodes.Server);

            var action = args as OpenCardsAction;
            var sPlayer = new SerializedPlayer(action.Player);

            foreach (var playerInfo in Game.PlayerInfos)
            {
                Send((ushort)GameCommand.GenerateCard, playerInfo.ClientID, sPlayer, NetworkDelivery.ReliableFragmentedSequenced);
            }
        }
        #endregion

        #region Not Phase, Not Change, Just Notify : No Need Update Data

        private void OnShowAvailableTiles(object sender, object args)
        {
            LogUtility.Log<GameMessageSystem>("Send Show Available Tiles", colorName: ColorCodes.Server);

            var pairArgs = args as Tuple<Player, List<Tile>>;
            var player = pairArgs.Item1;
            var tiles = pairArgs.Item2;
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
            else if (player == null)
            {
                LogUtility.Log($"{actionSystem.CurrentAction.GetType().Name}");
                LogUtility.Log("CurrentAction.Player is null");
            }

            var targetID = player.ID;

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