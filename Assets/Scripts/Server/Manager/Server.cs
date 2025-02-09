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
        private Dictionary<string, bool> attendances;
        private Dictionary<string, PlayerInfo> playerInfos;

        public void Init()
        {
            playerInfos = new();
            attendances = new();
            NetworkManager.Singleton.OnClientConnectedCallback += OnConnect;
        }

        public void Clear()
        {
            playerInfos = null;
            attendances = null;
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
            LogUtility.Log<Server>("Register this server : Host", ColorCodes.Server);

            try
            {
                Lobby lobby = await LobbyUtility.GetCurrentLobbyAsync();
                if (lobby != null)
                {
                    LogUtility.Log<Server>($"로비 등록 완료. 현재 속한 로비 : {lobby.Name}, 참가자 숫자 : {lobby.Players.Count}", ColorCodes.Server);
                    foreach(var player in lobby.Players)
                    {
                        attendances.Add(player.Id, false);
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
                    LogUtility.Log<Server>("현재 참가한 로비가 없습니다.", ColorCodes.Server);
                }
            }
            catch (System.Exception e)
            {
                LogUtility.LogError($"현재 로비 정보를 가져오는 중 오류 발생 : {e.Message}", ColorCodes.Server);
            }
        }

        private void RegisterClient()
        {
            LogUtility.Log<Server>("Register this server : Client", ColorCodes.Server);

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

            LogUtility.Log<Server>($"ToHost 메시지 수신\n- Lobby ID {sPlayerInfo.LobbyID}\n- Client ID {sPlayerInfo.ClientID}", ColorCodes.Server);

            playerInfos.Add(sPlayerInfo.LobbyID, PlayerInfo.CreatePlayerInfo(sPlayerInfo.LobbyID, sPlayerInfo.ClientID));
            attendances[sPlayerInfo.LobbyID] = true;

            if(CheckAttendance())
            {
                LogUtility.Log<Server>("모든 참가자 접속 완료. 게임 시작", ColorCodes.Server);
                TryGetAspect<Game>(out var game);
                game.PlayerInfos = playerInfos.Values.ToList();
                game.Awake();

                game.TryGetAspect<FlowSystem>(out var flowSystem);
                flowSystem.StartGame();
            }
            else
            {
                LogUtility.Log<Server>("다른 참가자의 접속을 기다려야 합니다.", ColorCodes.Server);
            }
        }

        private bool CheckAttendance()
        {
            if(attendances.Values.Contains(false))
            {
                return false;
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