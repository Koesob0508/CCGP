using System;
using System.Threading.Tasks;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace CCGP.Client
{
    public static class RelaySystem
    {
        public struct RelayHostData
        {
            public string JoinCode;
            public string IPv4Address;
            public ushort Port;
            public Guid AllocationId;
            public byte[] AllocationIdBytes;
            public byte[] ConnectionData;
            public byte[] Key;
        }

        public struct RelayJoinData
        {
            public string IPv4Address;
            public ushort Port;
            public Guid AllocationId;
            public byte[] AllocationIdBytes;
            public byte[] ConnectionData;
            public byte[] HostConnectionData;
            public byte[] Key;
        }

        public static async Task<RelayHostData> SetupRelay(int maxConnections)
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);

            RelayHostData data = new RelayHostData
            {
                JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId),
                IPv4Address = allocation.RelayServer.IpV4,
                Port = (ushort)allocation.RelayServer.Port,
                AllocationId = allocation.AllocationId,
                AllocationIdBytes = allocation.AllocationIdBytes,
                ConnectionData = allocation.ConnectionData,
                Key = allocation.Key
            };

            return data;
        }

        public static async Task<RelayJoinData> JoinRelay(string joinCode)
        {
            JoinAllocation allocation;

            try
            {
                allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            }
            catch (RelayServiceException e)
            {
                Debug.LogError($"RelayServiceException during JoinRelay: {e.Message}");
                return default;
            }

            return new RelayJoinData
            {
                IPv4Address = allocation.RelayServer.IpV4,
                Port = (ushort)allocation.RelayServer.Port,
                AllocationId = allocation.AllocationId,
                AllocationIdBytes = allocation.AllocationIdBytes,
                ConnectionData = allocation.ConnectionData,
                HostConnectionData = allocation.HostConnectionData,
                Key = allocation.Key
            };
        }
    }
}