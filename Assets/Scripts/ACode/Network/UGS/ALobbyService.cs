using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace ACode.UGS
{
    public static class ALobbyService
    {
        public static async Task<Lobby> GetLobbyAsync(string lobbyID)
        {
            Lobby lobby = null;

            try
            {
                Debug.Log("[ALobbyService] Get Lobby Started...");
                lobby = await LobbyService.Instance.GetLobbyAsync(lobbyID);
                Debug.Log("[ALobbyService] Get Lobby Success. Lobby ID : " + lobby.Id);
                return lobby;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("[ALobbyService] Get Lobby Failed : " + e.Message);
                return lobby;
            }
        }

        public static async Task<List<string>> GetJoinedLobbiesAsync()
        {
            List<string> lobbyIDs = null;

            try
            {
                Debug.Log("[ALobbyService] Get Joined Lobbies Started...");
                lobbyIDs = await LobbyService.Instance.GetJoinedLobbiesAsync();
                Debug.Log("[ALobbyService] Get Joined Lobbies Success. Lobby Count : " + lobbyIDs.Count);
                return lobbyIDs;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("[ALobbyService] Get Joined Lobbies Failed : " + e.Message);
                return lobbyIDs;
            }
        }

        public static async Task<Lobby> GetCurrentLobbyAsync()
        {
            Lobby lobby = null;

            try
            {
                Debug.Log("[ALobbyService] Get Current Lobby Started...");
                List<string> lobbyIDs = await LobbyService.Instance.GetJoinedLobbiesAsync();
                if (lobbyIDs.Count > 1)
                {
                    Debug.LogWarning("[ALobbyService] Get Current Lobby Failed : More than 1 Lobby Joined.");
                }
                else if (lobbyIDs.Count == 1)
                {
                    string lobbyID = lobbyIDs[0];
                    lobby = await LobbyService.Instance.GetLobbyAsync(lobbyID);
                    Debug.Log("[ALobbyService] Get Current Lobby Success. Lobby ID : " + lobby.Id);
                }
                else
                {
                    Debug.Log("[ALobbyService] Get Current Lobby Failed : No Lobby Joined.");
                }

                return lobby;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("[ALobbyService] Get Current Lobby Failed : " + e.Message);
                return lobby;
            }
        }

        public static async Task<Lobby> UpdateLobbyAsync(string lobbyID, UpdateLobbyOptions options)
        {
            Lobby lobby = null;

            try
            {
                Debug.Log("[ALobbyService] Update Lobby Started...");
                lobby = await LobbyService.Instance.UpdateLobbyAsync(lobbyID, options);
                Debug.Log("[ALobbyService] Update Lobby Success. Lobby ID : " + lobby.Id);
                return lobby;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("[ALobbyService] Update Lobby Failed : " + e.Message);
                return lobby;
            }
        }

        public static async Task<QueryResponse> QueryLobbiesAsync()
        {
            QueryResponse response = null;

            try
            {
                Debug.Log("[ALobbyService] Query Lobby Started...");
                response = await LobbyService.Instance.QueryLobbiesAsync();
                Debug.Log("[ALobbyService] Query Lobby Success. Lobby Count : " + response.Results.Count);
                return response;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("[ALobbyService] Query Lobby Failed : " + e.Message);
                return response;
            }
        }

        public static async Task<Lobby> CreateLobbyAsync(string lobbyName, int maxPlayers)
        {
            Lobby lobby = null;

            try
            {
                Debug.Log("[ALobbyService] Create Lobby Started...");
                lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);
                Debug.Log("[ALobbyService] Create Lobby Success. Lobby ID : " + lobby.Id);
                return lobby;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("[ALobbyService] Create Lobby Failed : " + e.Message);
                return lobby;
            }
        }

        public static async Task<Lobby> JoinLobbyByIDAsync(string lobbyID)
        {
            Lobby lobby = null;

            try
            {
                Debug.Log("[ALobbyService] Join Lobby Started...");
                lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyID);
                Debug.Log("[ALobbyService] Join Lobby Success. Lobby ID : " + lobby.Id);
                return lobby;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("[ALobbyService] Join Lobby Failed : " + e.Message);
                return lobby;
            }
        }

        public static async Task<bool> LeaveLobbyAsync(string lobbyID, string playerID)
        {
            try
            {
                Debug.Log("[ALobbyService] Leave Lobby Started...");
                await LobbyService.Instance.RemovePlayerAsync(lobbyID, playerID);
                Debug.Log("[ALobbyService] Leave Lobby Success.");
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("[ALobbyService] Leave Lobby Failed : " + e.Message);
                return false;
            }
        }

        public static async Task LeaveAllLobbiesAsync(string playerID)
        {
            try
            {
                Debug.Log("[ALobbyService] Query all joined lobbies started...");
                List<string> lobbyIDs = await LobbyService.Instance.GetJoinedLobbiesAsync();

                if (lobbyIDs.Count > 0)
                {
                    Debug.Log("[ALobbyService] Found lobby. Leave All Lobbies Started...");
                    foreach (var lobbyID in lobbyIDs)
                    {
                        await LobbyService.Instance.RemovePlayerAsync(lobbyID, playerID);
                    }
                }

                Debug.Log("[ALobbyService] Leave All Lobbies Success.");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("[ALobbyService] Leave All Lobbies Failed : " + e.Message);
            }
        }
    }
}