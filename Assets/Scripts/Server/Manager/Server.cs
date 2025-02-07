using CCGP.AspectContainer;
using CCGP.Shared;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;

namespace CCGP.Server
{
    public class Server : Container
    {
        private Dictionary<string, PlayerInfo> playerInfos;

        public void Init()
        {
            playerInfos = new();
            NetworkManager.Singleton.OnClientConnectedCallback += OnConnect;
        }

        public void Clear()
        {
            playerInfos = null;
            NetworkManager.Singleton.OnClientConnectedCallback -= OnConnect;
        }

        private void OnConnect(ulong clientID)
        {
            if (clientID != NetworkManager.Singleton.LocalClientId) return;

            OnLocalConnect();
        }

        private void OnLocalConnect()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                RegisterHost();
            }
            else
            {
                RegisterClient();
            }
        }

        private async void RegisterHost()
        {
            try
            {
                Lobby lobby = await LobbyUtility.GetCurrentLobbyAsync();
                if (lobby != null)
                {
                    LogUtility.Log<Server>($"현재 속한 로비 : {lobby.Name}");
                    foreach(var player in lobby.Players)
                    {
                        playerInfos.Add(player.Id, null);
                    }


                    // CustomMessage Awake
                    NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler("ToHost", OnReceivedMessage);

                    // Host에게 Player 정보 보내기
                    var lobbyID = AuthenticationService.Instance.PlayerId;
                    var clientID = NetworkManager.Singleton.LocalClientId;
                    var playerInfo = PlayerInfo.CreatePlayerInfo(lobbyID, clientID);
                    var sPlayerInfo = new SerializedPlayerInfo(playerInfo);

                    using (var writer = new FastBufferWriter(128, Unity.Collections.Allocator.Temp))
                    {
                        writer.WriteNetworkSerializable(sPlayerInfo);
                        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("ToHost", NetworkManager.ServerClientId, writer, NetworkDelivery.ReliableSequenced);
                    }
                }
                else
                {
                    LogUtility.Log<Server>("현재 참가한 로비가 없습니다.");
                }
            }
            catch (System.Exception e)
            {
                LogUtility.LogError($"현재 로비 정보를 가져오는 중 오류 발생 : {e.Message}");
            }
        }

        private void RegisterClient()
        {
            // Host에게 Player 정보 보내기
            var lobbyID = AuthenticationService.Instance.PlayerId;
            var clientID = NetworkManager.Singleton.LocalClientId;
            var playerInfo = PlayerInfo.CreatePlayerInfo(lobbyID, clientID);
            var sPlayerInfo = new SerializedPlayerInfo(playerInfo);

            using (var writer = new FastBufferWriter(128, Unity.Collections.Allocator.Temp))
            {
                writer.WriteNetworkSerializable(sPlayerInfo);
                NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage("ToHost", NetworkManager.ServerClientId, writer, NetworkDelivery.ReliableSequenced);
            }
        }

        private void OnReceivedMessage(ulong clientID, FastBufferReader reader)
        {
            var sData = new SerializedData(reader);
            var sPlayerInfo = sData.Get<SerializedPlayerInfo>();

            LogUtility.Log($"{sPlayerInfo.LobbyID} {sPlayerInfo.ClientID}");

            playerInfos[sPlayerInfo.LobbyID] = PlayerInfo.CreatePlayerInfo(sPlayerInfo.LobbyID, sPlayerInfo.ClientID);

            if(CheckAttendance())
            {
                LogUtility.Log("게임 스타트~");
                TryGetAspect<Game>(out var game);
                game.PlayerInfos = playerInfos.Values.ToList();
                game.Awake();

                game.TryGetAspect<FlowSystem>(out var flowSystem);
                flowSystem.StartGame();
            }
        }

        private bool CheckAttendance()
        {
            foreach(var playerInfo in playerInfos)
            {
                if(playerInfo.Value == null)
                {
                    LogUtility.Log($"{playerInfo.Key} did not set");
                    return false;
                }
            }

            return true;
        }
    }

    public class PlayerInfo
    {
        public string LobbyID;
        public ulong ClientID;

        public static PlayerInfo CreatePlayerInfo(string lobbyID, ulong clientID)
        {
            return new PlayerInfo() { LobbyID = lobbyID, ClientID = clientID };
        }
    }
}