using CCGP.AspectContainer;
using CCGP.Shared;
using Unity.Netcode;

namespace CCGP.Client
{
    public class Client : Container
    {
        public void Init()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnConnect;
        }

        private void OnConnect(ulong clientID)
        {
            if (clientID != NetworkManager.Singleton.LocalClientId) return;

            this.Awake();
        }

        public void Upate()
        {
            this.Update();
        }
    }
}