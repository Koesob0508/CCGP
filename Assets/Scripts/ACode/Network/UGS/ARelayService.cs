using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace ACode.UGS
{
    public static class ARelayService
    {
        public static async Task<string> StartHostWithRelay(int maxConnection = 4)
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnection);
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

#if UNITY_WEBGL
            transport.UseWebSockets = true;
#endif
            RelayServerData serverData;

            if (transport.UseWebSockets)
            {
                serverData = AllocationUtils.ToRelayServerData(allocation, "wss");
            }
            else
            {
                serverData = AllocationUtils.ToRelayServerData(allocation, "udp");
            }

            transport.SetRelayServerData(serverData);

            Task<string> joinTask = RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            var joinCode = await joinTask;
            var success = false;

            if (joinTask.IsCompletedSuccessfully)
            {
                success = NetworkManager.Singleton.StartHost();
                if (success)
                {
                    Debug.Log("Start Host with Relay");
                }
                else
                {
                    Debug.Log("Start Host Failed");
                }
            }
            else
            {
                Debug.Log("Get Join Code Failed");
            }

            return success ? joinCode : null;
        }

        public static async Task<bool> StartClientWithRelay(string joinCode)
        {
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
#if UNITY_WEBGL
            var serverData = AllocationUtils.ToRelayServerData(joinAllocation, "wss");
#else
            var serverData = AllocationUtils.ToRelayServerData(joinAllocation, "udp");
#endif

            transport.SetRelayServerData(serverData);

#if UNITY_WEBGL
            transport.UseWebSockets = true;
#endif

            var success = NetworkManager.Singleton.StartClient();

            if (success)
            {
                Debug.Log("Relay Start Client");
            }
            else
            {
                Debug.Log("Start Client Failed");
            }

            return !string.IsNullOrEmpty(joinCode) && success;
        }
    }
}