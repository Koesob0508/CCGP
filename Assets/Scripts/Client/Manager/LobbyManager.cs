using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using CCGP.Shared;

namespace CCGP.Client
{
    public class LobbyManager : MonoBehaviour
    {
        [Header("Pannel Settings")]
        public GameObject lobbyListPanel;       // 로비 목록이 보이는 패널
        public GameObject lobbyRoomPanel;       // 로비에 입장했을 때 보이는 패널

        [Header("Lobby List UI")]
        public Button refreshLobbiesButton;     // 로비 목록 새로고침 버튼
        public Transform lobbyListContent;      // 로비 목록 항목들을 자식으로 추가할 컨테이너
        public GameObject lobbyEntryPrefab;     // 로비 목록 항목용 프리팹

        [Header("Create Lobby UI")]
        public TMP_InputField lobbyNameInputField;  // 로비 이름 입력 필드
        public Button createLobbyButton;        // 로비 생성 버튼

        [Header("Lobby Room UI")]
        public TMP_Text roomName;
        public TMP_Text playersListText;            // 현재 로비에 참가한 플레이어 목록 표시용 텍스트
        public Button leaveLobbyButton;         // 로비 나가기 버튼
        public Button startGameButton;          // (호스트 전용) 게임 시작 버튼

        // --- 로비 관련 변수 ---
        private Lobby currentLobby;
        private const int maxPlayers = 4;
        private const int minPlayersToStart = 3;
        private float lobbyUpdateInterval = 2f;
        private Coroutine lobbyPollingCoroutine;

        async void Start()
        {
            // Unity Services 초기화 및 익명 로그인
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            // 초기 화면은 로비 목록
            ShowLobbyListUI();

            // 버튼 이벤트 연결
            refreshLobbiesButton.onClick.AddListener(() => { _ = RefreshLobbyListAsync(); });
            createLobbyButton.onClick.AddListener(() => { _ = CreateLobbyAsync(lobbyNameInputField.text); });
            leaveLobbyButton.onClick.AddListener(() => { _ = LeaveLobbyAsync(); });
            startGameButton.onClick.AddListener(() => { _ = ForceStartGameAsync(); });

            // 시작 시 로비 목록 갱신
            await RefreshLobbyListAsync();

            this.AddObserver(OnGameStart, "StartGame");
        }

        #region 로비 목록 관련

        // 로비 목록 쿼리 및 UI 업데이트
        public async Task RefreshLobbyListAsync()
        {
            try
            {
                QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync();

                // 기존 항목 제거
                foreach (Transform child in lobbyListContent)
                {
                    Destroy(child.gameObject);
                }

                foreach (Lobby lobby in response.Results)
                {
                    GameObject entry = Instantiate(lobbyEntryPrefab, lobbyListContent);
                    Content_LobbyEntry entryUI = entry.GetComponent<Content_LobbyEntry>();
                    entryUI.Setup(lobby, OnJoinLobbyButtonClicked);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("로비 목록 조회 실패 : " + e.Message);
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
            try
            {
                currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);
                Debug.Log("로비 생성됨 : " + currentLobby.Id);
                ShowLobbyRoomUI();
                StartLobbyPolling();
            }
            catch (System.Exception e)
            {
                Shared.LogUtility.Log<LobbyManager>("로비 생성 실패 : " + e.Message);
            }
        }

        // 기존 로비에 참가
        public async Task JoinLobbyAsync(string lobbyId)
        {
            try
            {
                currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
                Debug.Log("로비 참가 성공 : " + currentLobby.Id);
                ShowLobbyRoomUI();
                StartLobbyPolling();
            }
            catch (System.Exception e)
            {
                Shared.LogUtility.Log<LobbyManager>("로비 참가 실패 : " + e.Message);
            }
        }

        // 로비 나가기
        public async Task LeaveLobbyAsync()
        {
            if (currentLobby != null)
            {
                try
                {
                    await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId);
                    Debug.Log("로비 나감");
                    currentLobby = null;
                    StopLobbyPolling();
                    ShowLobbyListUI();
                    await RefreshLobbyListAsync();
                }
                catch (System.Exception e)
                {
                    Debug.LogError("로비 나가기 실패: " + e.Message);
                }
            }
        }

        // UI 패널 전환 함수

        // UI 패널 전환 함수
        private void ShowLobbyListUI()
        {
            lobbyListPanel.SetActive(true);
            lobbyRoomPanel.SetActive(false);
        }
        private void ShowLobbyRoomUI()
        {
            lobbyListPanel.SetActive(false);
            lobbyRoomPanel.SetActive(true);
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
                Task<Lobby> lobbyTask = LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
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
                        Debug.Log("게임 시작! Relay Join Code: " + joinCode);

                        if (currentLobby.HostId != AuthenticationService.Instance.PlayerId)
                        {
                            // 클라이언트는 joinCode를 이용해 Relay Allocation에 참여합니다.
                            StartCoroutine(StartClientRelayCoroutine(joinCode));
                        }
                        else
                        {
                            Debug.Log("[LobbySystem] Host는 StartClientRelayCoroutine()를 실행하지 않음.");
                        }


                        StopLobbyPolling();
                        yield break;
                    }
                }
                else
                {
                    Debug.LogError("로비 정보 갱신 실패");
                }
                yield return new WaitForSeconds(lobbyUpdateInterval);
            }
        }

        private IEnumerator StartClientRelayCoroutine(string joinCode)
        {
            Task<RelaySystem.RelayJoinData> relayTask = RelaySystem.JoinRelay(joinCode);
            yield return new WaitUntil(() => relayTask.IsCompleted);
            if (relayTask.IsCompletedSuccessfully)
            {
                RelaySystem.RelayJoinData joinData = relayTask.Result;
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(joinData.IPv4Address, joinData.Port, joinData.AllocationIdBytes, joinData.Key, joinData.ConnectionData, joinData.HostConnectionData);
                NetworkManager.Singleton.StartClient();
                Debug.Log("[Relay] Client가 Relay를 사용하여 시작되었습니다.");
            }
            else
            {
                Debug.LogError("Relay Client 시작 중 오류 발생");
            }
        }

        // 로비 내 플레이어 목록 등 UI 업데이트
        private void UpdateLobbyRoomUI()
        {
            if (currentLobby == null)
                return;

            roomName.text = currentLobby.Name;
            // 예제에서는 각 플레이어의 "playerName"이라는 커스텀 데이터를 사용합니다.
            string playerListStr = "플레이어 목록:\n";
            foreach (var player in currentLobby.Players)
            {
                // player.Data에 "playerName"이 있다면 표시 (없으면 ID로 대체)
                if (player.Data != null && player.Data.ContainsKey("playerName"))
                {
                    playerListStr += player.Data["playerName"].Value + "\n";
                }
                else
                {
                    string shortID = player.Id.Length > 4 ? player.Id.Substring(0, 4) : player.Id;
                    playerListStr += $"Player {shortID}\n";
                }
            }
            playersListText.text = playerListStr;

            // 현재 플레이어가 호스트이며, 최소 3명 이상이면 게임 시작 버튼 활성화
            bool isHost = currentLobby.HostId == AuthenticationService.Instance.PlayerId;
            bool enoughPlayers = currentLobby.Players.Count >= minPlayersToStart;
            startGameButton.interactable = isHost && enoughPlayers;
        }

        #endregion

        #region 게임 시작 (호스트 전용)
        public async Task ForceStartGameAsync()
        {
            if (currentLobby == null)
            {
                Debug.LogError("현재 로비가 없습니다.");
                return;
            }
            if (currentLobby.Players.Count < minPlayersToStart)
            {
                Debug.Log("최소 플레이어 수가 부족합니다.");
                return;
            }
            if (currentLobby.HostId != AuthenticationService.Instance.PlayerId)
            {
                Debug.Log("호스트만 게임을 시작할 수 있습니다.");
                return;
            }
            try
            {
                // RelayManager를 통해 호스트용 Relay 할당 생성 (환경 이름 지정 필요)
                RelaySystem.RelayHostData hostData = await RelaySystem.SetupRelay(maxPlayers);
                Debug.Log("호스트가 게임 시작했습니다. Relay Join Code: " + hostData.JoinCode);

                // 로비 데이터 업데이트: gameStarted 플래그와 Relay Join Code 등록
                var data = new Dictionary<string, DataObject>
            {
                { "gameStarted", new DataObject(DataObject.VisibilityOptions.Public, "true") },
                { "joinCode", new DataObject(DataObject.VisibilityOptions.Public, hostData.JoinCode) }
            };
                var updateOptions = new UpdateLobbyOptions { Data = data };
                currentLobby = await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, updateOptions);

                // 호스트는 여기서 Relay 연결 초기화 (예: RelayManager.StartHostRelay(allocation);)
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(hostData.IPv4Address, hostData.Port, hostData.AllocationIdBytes, hostData.Key, hostData.ConnectionData);
                NetworkManager.Singleton.StartHost();

                StopLobbyPolling();
            }
            catch (System.Exception e)
            {
                Debug.LogError("게임 시작 실패: " + e.Message);
            }
        }
        #endregion


        #region On Notify
        private void OnGameStart(object sender, object args)
        {
            lobbyListPanel.SetActive(false);
            lobbyRoomPanel.SetActive(false);
        }
        #endregion

    }
}