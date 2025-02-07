using CCGP.AspectContainer;
using CCGP.Shared;
using System;
using System.Collections.Generic;
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
    }
}