using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

namespace CCGP.Shared
{
    public static class LobbyUtility
    {
        public static async Task<Lobby> GetCurrentLobbyAsync()
        {
            try
            {
                var lobbyIDs = await LobbyService.Instance.GetJoinedLobbiesAsync();
                if (lobbyIDs.Count > 0)
                {
                    string lobbyID = lobbyIDs[0];
                    LogUtility.Log($"[LobbySystem] 현재 플레이어가 속한 로비 ID : {lobbyID}");

                    return await LobbyService.Instance.GetLobbyAsync(lobbyID);
                }
                else
                {
                    LogUtility.Log("[LobbySystem] 현재 참가한 로비가 없습니다.");
                    return null;
                }
            }
            catch (System.Exception e)
            {
                LogUtility.LogError($"현재 로비 정보를 가져오는 중 오류 발생 : {e.Message}");
                return null;
            }
        }
    }
}