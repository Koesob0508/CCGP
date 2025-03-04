using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using TMPro;
using CCGP.Shared;
using ACode.UGS;
using Unity.Netcode;

namespace CCGP.Client
{
    public class LobbyViewManager : MonoBehaviour
    {
        private GameView View;

        [Header("Pannel Settings")]
        public GameObject Canvas_Lobby;
        public GameObject Panel_LobbyList;          // 로비 목록이 보이는 패널
        public GameObject Panel_LobbyRoom;          // 로비에 입장했을 때 보이는 패널

        [Header("Lobby List UI")]
        public Button Button_RefreshLobbies;        // 로비 목록 새로고침 버튼
        public Transform ContentsRoot_Lobbies;      // 로비 목록 항목들을 자식으로 추가할 컨테이너
        public GameObject Prefab_LobbyEntry;        // 로비 목록 항목용 프리팹

        [Header("Create Lobby UI")]
        public TMP_InputField InputField_LobbyName; // 로비 이름 입력 필드
        public Button Button_CreateLobby;           // 로비 생성 버튼

        [Header("Lobby Room UI")]
        public TMP_Text Text_RoomName;
        public TMP_Text Text_PlayersList;           // 현재 로비에 참가한 플레이어 목록 표시용 텍스트
        public Button Button_LeaveLobby;            // 로비 나가기 버튼
        public Button Button_StartGame;             // (호스트 전용) 게임 시작 버튼

        [Header("Player Info")]
        public TMP_Text Text_PlayerID;              // 플레이어 ID 표시용 텍스트

        // --- 로비 관련 변수 ---
        private Lobby currentLobby;
        private const int maxPlayers = 4;
        private const int minPlayersToStart = 1;
        private float lobbyUpdateInterval = 2f;
        private Coroutine lobbyPollingCoroutine;

        private void Start()
        {
            View = GetComponentInParent<GameView>();

            // 버튼 이벤트 연결
            Button_RefreshLobbies.onClick.AddListener(() => { _ = RefreshLobbyListAsync(); });
            Button_CreateLobby.onClick.AddListener(() => { _ = CreateLobbyAsync(InputField_LobbyName.text); });
            Button_LeaveLobby.onClick.AddListener(() => { _ = LeaveLobbyAsync(); });
            Button_StartGame.onClick.AddListener(() => { _ = ForceStartGameAsync(); });

            this.AddObserver(OnShowLobbies, ClientDialect.SignInAnonymouslyAsync, View.Container);
            this.AddObserver(OnStartGame, Global.MessageNotification(GameCommand.StartGame), View.Container);
        }

        #region 로비 목록 관련

        // 로비 목록 쿼리 및 UI 업데이트
        public async Task RefreshLobbyListAsync()
        {
            LogUtility.Log<LobbyViewManager>($"Refresh Lobby...");

            foreach (Transform child in ContentsRoot_Lobbies)
            {
                Destroy(child.gameObject);
            }

            QueryResponse response = await ALobbyService.QueryLobbiesAsync();

            if (response.Results.Count > 0)
            {
                foreach (Lobby lobby in response.Results)
                {
                    GameObject entry = Instantiate(Prefab_LobbyEntry, ContentsRoot_Lobbies);
                    Content_LobbyEntry entryUI = entry.GetComponent<Content_LobbyEntry>();
                    entryUI.Setup(lobby, OnJoinLobbyButtonClicked);
                }
            }
        }

        // 로비 항목의 "참가" 버튼 클릭 시 호출
        private async void OnJoinLobbyButtonClicked(Lobby lobby)
        {
            await JoinLobbyAsync(lobby.Id);
        }

        #endregion

        #region 로비 생성, 참가, 나가기

        // 로비 생성 (호스트가 됨)
        public async Task CreateLobbyAsync(string lobbyName)
        {
            // 내가 접속해 있는 로비들이 있는지 검증부터 해야함.
            await ALobbyService.LeaveAllLobbiesAsync(AAuthenticationService.PlayerID);

            currentLobby = await ALobbyService.CreateLobbyAsync(lobbyName, maxPlayers);

            if (currentLobby != null)
            {
                LogUtility.Log<LobbyViewManager>("로비 생성됨 : " + currentLobby.Id);
                ShowLobbyRoomUI();
                StartLobbyPolling();
            }
            else
            {
                LogUtility.Log<LobbyViewManager>("로비 생성 실패");
            }

        }

        // 기존 로비에 참가
        public async Task JoinLobbyAsync(string lobbyId)
        {
            await ALobbyService.LeaveAllLobbiesAsync(AAuthenticationService.PlayerID);

            currentLobby = await ALobbyService.JoinLobbyByIDAsync(lobbyId);

            if (currentLobby != null)
            {
                LogUtility.Log<LobbyViewManager>("로비 참가 성공 : " + currentLobby.Id);
                ShowLobbyRoomUI();
                StartLobbyPolling();
            }
            else
            {
                LogUtility.Log<LobbyViewManager>("로비 참가 실패");
            }
        }

        // 로비 나가기
        public async Task LeaveLobbyAsync()
        {
            if (currentLobby != null)
            {
                var sucess = await ALobbyService.LeaveLobbyAsync(currentLobby.Id, AAuthenticationService.PlayerID);
                if (sucess)
                {
                    LogUtility.Log<LobbyViewManager>("로비 나감");
                    currentLobby = null;
                    StopLobbyPolling();
                    ShowLobbyListUI();

                    if (NetworkManager.Singleton.IsConnectedClient)
                    {
                        NetworkManager.Singleton.Shutdown();
                    }

                    await RefreshLobbyListAsync();
                }
                else
                {
                    LogUtility.Log<LobbyViewManager>("로비 나가기 실패");
                }
            }
        }

        // UI 패널 전환 함수

        // UI 패널 전환 함수
        private void ShowLobbyListUI()
        {
            Panel_LobbyList.SetActive(true);
            Panel_LobbyRoom.SetActive(false);
        }
        private void ShowLobbyRoomUI()
        {
            Panel_LobbyList.SetActive(false);
            Panel_LobbyRoom.SetActive(true);
        }

        #endregion

        #region 로비 폴링 및 UI 업데이트

        // 로비 상태를 주기적으로 갱신
        private void StartLobbyPolling()
        {
            if (lobbyPollingCoroutine != null)
            {
                StopCoroutine(lobbyPollingCoroutine);
            }
            lobbyPollingCoroutine = StartCoroutine(PollLobbyCoroutine());
        }

        private void StopLobbyPolling()
        {
            if (lobbyPollingCoroutine != null)
            {
                StopCoroutine(lobbyPollingCoroutine);
                lobbyPollingCoroutine = null;
            }
        }

        private IEnumerator PollLobbyCoroutine()
        {
            while (currentLobby != null)
            {
                Task<Lobby> lobbyTask = ALobbyService.GetLobbyAsync(currentLobby.Id);
                yield return new WaitUntil(() => lobbyTask.IsCompleted);
                if (lobbyTask.IsCompletedSuccessfully)
                {
                    currentLobby = lobbyTask.Result;
                    UpdateLobbyRoomUI();

                    // 로비 데이터에 gameStarted 플래그가 있으면 게임 시작
                    if (currentLobby.Data != null &&
                       currentLobby.Data.ContainsKey("gameStarted") &&
                       currentLobby.Data["gameStarted"].Value == "true")
                    {
                        string joinCode = currentLobby.Data["joinCode"].Value;
                        LogUtility.Log<LobbyViewManager>("게임 시작! Relay Join Code: " + joinCode);

                        if (currentLobby.HostId != AAuthenticationService.PlayerID)
                        {
                            Task<bool> relayTask = ARelayService.StartClientWithRelay(joinCode);
                            yield return new WaitUntil(() => relayTask.IsCompleted);
                        }
                        else
                        {
                            LogUtility.Log<LobbyViewManager>("Host는 StartClientRelayCoroutine()를 실행하지 않음.");
                        }


                        StopLobbyPolling();
                        yield break;
                    }
                }
                else
                {
                    LogUtility.LogError<LobbyViewManager>("로비 정보 갱신 실패");
                }

                yield return new WaitForSeconds(lobbyUpdateInterval);
            }
        }

        // 로비 내 플레이어 목록 등 UI 업데이트
        private void UpdateLobbyRoomUI()
        {
            if (currentLobby == null)
                return;

            Text_RoomName.text = currentLobby.Name;
            // 예제에서는 각 플레이어의 "playerName"이라는 커스텀 데이터를 사용합니다.
            string playerListStr = "참가 플레이어\n";
            foreach (var player in currentLobby.Players)
            {
                // player.Data에 "playerName"이 있다면 표시 (없으면 ID로 대체)
                if (player.Data != null && player.Data.ContainsKey("playerName"))
                {
                    playerListStr += player.Data["playerName"].Value + "\n";
                }
                else
                {
                    string shortID = player.Id.Length > 4 ? $"{player.Id.Substring(0, 4)}-{player.Id.Substring(4, 2)}" : player.Id;
                    playerListStr += $"플레이어 {shortID}\n";
                }
            }
            Text_PlayersList.text = playerListStr;

            // 현재 플레이어가 호스트이며, 최소 3명 이상이면 게임 시작 버튼 활성화
            bool isHost = currentLobby.HostId == AAuthenticationService.PlayerID;
            bool enoughPlayers = currentLobby.Players.Count >= minPlayersToStart;
            Button_StartGame.interactable = isHost && enoughPlayers;
        }

        #endregion

        #region 게임 시작 (호스트 전용)
        public async Task ForceStartGameAsync()
        {
            LogUtility.Log<LobbyViewManager>("게임 시작 버튼 눌림");

            if (currentLobby == null)
            {
                LogUtility.LogError<LobbyViewManager>("현재 로비가 없습니다.");
                return;
            }
            if (currentLobby.Players.Count < minPlayersToStart)
            {
                LogUtility.LogError<LobbyViewManager>("최소 플레이어 수가 부족합니다.");
                return;
            }
            if (currentLobby.HostId != AAuthenticationService.PlayerID)
            {
                LogUtility.LogError<LobbyViewManager>("호스트만 게임을 시작할 수 있습니다.");
                return;
            }

            StopLobbyPolling();

            string joinCode = await ARelayService.StartHostWithRelay(maxPlayers);
            if (joinCode != string.Empty)
            {
                LogUtility.Log<LobbyViewManager>("게임 시작! Relay Join Code: " + joinCode);

                // 로비 데이터 업데이트: gameStarted 플래그와 Relay Join Code 등록
                var data = new Dictionary<string, DataObject>
                {
                    { "gameStarted", new DataObject(DataObject.VisibilityOptions.Public, "true") },
                    { "joinCode", new DataObject(DataObject.VisibilityOptions.Public, joinCode) }
                };
                var updateOptions = new UpdateLobbyOptions { Data = data };
                currentLobby = await ALobbyService.UpdateLobbyAsync(currentLobby.Id, updateOptions);
            }
            else
            {
                LogUtility.LogError<LobbyViewManager>("게임 시작 실패");

                if (lobbyPollingCoroutine == null)
                    StartLobbyPolling();
            }
        }
        #endregion


        #region On Notify
        private async void OnShowLobbies(object sender, object args)
        {
            ShowLobbyListUI();

            var originalID = AAuthenticationService.PlayerID;
            Text_PlayerID.text = $"Player ID : {originalID.Substring(0, 4)}-{originalID.Substring(4, 2)}";

            // 시작 시 로비 목록 갱신
            await RefreshLobbyListAsync();
        }

        private void OnStartGame(object sender, object args)
        {
            Panel_LobbyList.SetActive(false);
            Panel_LobbyRoom.SetActive(false);
            Canvas_Lobby.SetActive(false);
        }
        #endregion


        async void OnDestroy()
        {
            Debug.Log("OnDisable: Play Mode 종료 또는 오브젝트 비활성화됨. 로비에서 나갑니다.");
            await ALobbyService.LeaveAllLobbiesAsync(AAuthenticationService.PlayerID);
        }
    }
}