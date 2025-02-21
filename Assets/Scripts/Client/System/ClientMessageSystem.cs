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
            this.AddObserver(OnTryPlayCard, Global.MessageNotification(GameCommand.TryPlayCard));
            this.AddObserver(OnTryCancelPlayCard, Global.MessageNotification(GameCommand.TryCancelPlayCard));
            this.AddObserver(OnSelectTile, Global.MessageNotification(GameCommand.TrySelectTile));
            this.AddObserver(OnTryEndTurn, ClientDialect.EndTurn);
        }

        public void Deactivate()
        {
            NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler("ToClientGame");
            this.RemoveObserver(OnTryPlayCard, Global.MessageNotification(GameCommand.TryPlayCard));
            this.RemoveObserver(OnTryCancelPlayCard, Global.MessageNotification(GameCommand.TryCancelPlayCard));
            this.RemoveObserver(OnSelectTile, Global.MessageNotification(GameCommand.TrySelectTile));
            this.RemoveObserver(OnTryEndTurn, ClientDialect.EndTurn);
        }

        public void RegisterHandler(ushort type, Action<ulong, SerializedData> callback)
        {
            Handlers.Add(type, callback);
        }

        private void OnReceivedMessage(ulong clientID, FastBufferReader reader)
        {
            reader.ReadValueSafe(out ushort command);
            SerializedData sdata = new SerializedData(reader);
            LogUtility.Log<ClientMessageSystem>($"On received {(GameCommand)command}", colorName: ColorCodes.Client);
            Container.PostNotification(Global.MessageNotification((GameCommand)command), sdata);
        }

        #region Client Message

        private void OnTryPlayCard(object sender, object args)
        {
            LogUtility.Log<ClientMessageSystem>($"Send TryPlayCard", colorName: ColorCodes.Client);
            var data = args as SerializedCard;
            SendToHost((ushort)GameCommand.TryPlayCard, data);
        }

        private void OnTryCancelPlayCard(object sender, object args)
        {
            LogUtility.Log<ClientMessageSystem>($"Send TryCancelPlayCard", colorName: ColorCodes.Client);
            SendToHost((ushort)GameCommand.TryCancelPlayCard);
        }

        private void OnSelectTile(object sender, object args)
        {
            LogUtility.Log<ClientMessageSystem>($"Send SelectTile", colorName: ColorCodes.Client);
            var data = args as SerializedTile;
            SendToHost((ushort)GameCommand.TrySelectTile, data);
        }

        private void OnTryEndTurn(object sender, object args)
        {
            LogUtility.Log<ClientMessageSystem>($"Send TryEndTurn", colorName: ColorCodes.Client);
            var data = args as SerializedTurnEndAction;
            SendToHost((ushort)GameCommand.TryEndTurn, data);
        }

        #endregion

        #region Send Utilities

        private void SendToHost(ushort tag, INetworkSerializable data = null, NetworkDelivery delivery = NetworkDelivery.ReliableFragmentedSequenced)
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
