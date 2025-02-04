using CCGP.AspectContainer;
using CCGP.Shared;
using System.Collections.Generic;

namespace CCGP.Server
{
    public class ServerSystem : Aspect, IObserve
    {
        private NetworkSystem network;
        private Queue<ulong> ReadyPlayers;
        private Dictionary<uint, IContainer> Games;

        public void Awake()
        {
            network = Container.GetNetwork();
            // network.NetworkManager.OnClientConnectedCallback += OnConnect;
        }

        public void Sleep()
        {
            network.NetworkManager.OnClientConnectedCallback -= OnConnect;
            network = null;
        }

        private void OnConnect(ulong clientID)
        {
            if(clientID == network.NetworkManager.LocalClientId)
            {
                OnMyClientConnected();
            }
            else
            {
                OnOtherClientConnected(clientID);
            }
        }

        private void OnMyClientConnected()
        {
            if(!network.NetworkManager.IsHost)
            {
                Sleep();
                return;
            }

            ReadyPlayers.Enqueue(network.NetworkManager.LocalClientId);
            WhenReadyToGame();
        }

        private void OnOtherClientConnected(ulong clientID)
        {
            if (ReadyPlayers.Contains(clientID)) return;

            ReadyPlayers.Enqueue(clientID);
            WhenReadyToGame();
        }

        private void WhenReadyToGame()
        {
            if(ReadyPlayers.Count >= 4)
            {
                var player1 = ReadyPlayers.Dequeue();
                var player2 = ReadyPlayers.Dequeue();
                var player3 = ReadyPlayers.Dequeue();
                var player4 = ReadyPlayers.Dequeue();

                List<ulong> players = new List<ulong>() { player1, player2, player3, player4 };

                var id = (uint)Games.Count;
                var game = GameSystemFactory.Create(id, players);
                game.Awake();
                Games.Add(id, game);
                game.TryGetAspect<FlowSystem>(out var flow);
                flow.StartGame();
            }
        }
    }
}