using CCGP.AspectContainer;
using CCGP.Shared;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace CCGP.Client
{
    public class ClientSystem : MonoBehaviour, IAspect
    {
        IContainer _container;

        public IContainer Container
        {
            get
            {
                if(_container == null)
                {
                    _container = MainSystemFactory.Create();
                    _container.AddAspect(this);
                }
                return _container;
            }
            set
            {
                _container = value;
            }
        }

        private NetworkSystem network;

        private void Awake()
        {
            Container.Awake();
            network = Container.GetNetwork();
            network.NetworkManager.OnClientConnectedCallback += OnConnect;
        }

        private void OnConnect(ulong clientID)
        {
            if (clientID == network.NetworkManager.LocalClientId)
            {
                OnMyClientConnected();
            }
            else
            {
                OnOtherClientConnected();
            }
        }

        private void OnMyClientConnected()
        {
            network.NetworkManager.CustomMessagingManager.RegisterNamedMessageHandler("FromServer", OnReceivedServerMessage);

            if(network.NetworkManager.IsHost)
            {
            }
        }

        private void OnOtherClientConnected()
        {
            if(!network.NetworkManager.IsHost)
            {
                Debug.Log("뭔가 잘못됨");
            }
        }

        private void OnReceivedServerMessage(ulong clientID, FastBufferReader reader)
        {
            reader.ReadValueSafe(out ushort commandTye);

            
        }
    }
}