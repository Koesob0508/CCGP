using System;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace CCGP.Client
{
    public class Content_LobbyEntry : MonoBehaviour
    {
        public TMP_Text lobbyNameText;    // 로비 이름 표시용 텍스트
        public TMP_Text playerCountText;  // 플레이어 수 표시용 텍스트
        public Button joinButton;     // 로비 참가 버튼

        private Lobby lobbyData;
        private Action<Lobby> onJoinCallback;

        // 로비 데이터와 콜백을 설정하여 UI 초기화
        public void Setup(Lobby lobby, Action<Lobby> joinCallback)
        {
            lobbyData = lobby;
            lobbyNameText.text = lobby.Name;
            playerCountText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
            onJoinCallback = joinCallback;
            joinButton.onClick.RemoveAllListeners();
            joinButton.onClick.AddListener(OnJoinButtonClicked);
        }

        private void OnJoinButtonClicked()
        {
            onJoinCallback?.Invoke(lobbyData);
        }
    }
}