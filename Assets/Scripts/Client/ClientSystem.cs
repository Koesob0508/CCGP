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
        IEntity _container;
        [SerializeField] int Count = 0;
        [SerializeField] Button button_StartGame;
        [SerializeField] TMP_Text Text_StartGame;

        public IEntity Entity
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
            Entity.Awake();
            network = Entity.GetNetwork();
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
                Count++;
                button_StartGame.gameObject.SetActive(true);
                Text_StartGame.text = $"Start Game\nCurrent Player({Count})";
                if(Count < 3)
                {
                    button_StartGame.interactable = false;
                }
                else
                {
                    button_StartGame.interactable = true;
                }
            }
        }

        private void OnOtherClientConnected()
        {
            if(!network.NetworkManager.IsHost)
            {
                Debug.Log("뭔가 잘못됨");
            }

            Count++;
            Text_StartGame.text = $"Start Game\nCurrent Player({Count})";
            if (Count < 3)
            {
                button_StartGame.interactable = false;
            }
            else
            {
                button_StartGame.interactable = true;
            }
        }

        private void OnReceivedServerMessage(ulong clientID, FastBufferReader reader)
        {
            reader.ReadValueSafe(out ushort commandTye);

            
        }
    }
}