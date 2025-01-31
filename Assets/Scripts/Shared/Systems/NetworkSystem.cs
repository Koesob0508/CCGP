using CCGP.AspectContainer;
using Unity.Netcode;

namespace CCGP.Shared
{
    public class NetworkSystem : Aspect
    {
        public NetworkManager NetworkManager => NetworkManager.Singleton;
    }

    public static class NetworkSystemExtensions
    {
        public static NetworkSystem GetNetwork(this IEntity system)
        {
            if(system.TryGetAspect<NetworkSystem>(out var networkSystem))
            {
                return networkSystem;
            }
            else
            {
                return null;
            }
        }
    }
}