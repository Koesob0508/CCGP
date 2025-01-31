using CCGP.AspectContainer;
using CCGP.Shared;
using UnityEngine;

namespace CCGP.Client
{
    public class ConnectView : MonoBehaviour
    {
        IEntity system;

        #region Unity Callbacks

        private void Awake()
        {
            system = GetComponentInParent<ClientSystem>().Entity;
        }

        #endregion

        public void OnStartHostButtonPressed()
        {
            if(CanConnect())
            {
                system.GetNetwork().NetworkManager.StartHost();
            }
            else
            {

            }
        }

        public void OnStartClientButtonPressed()
        {
            if(CanConnect())
            {
                system.GetNetwork().NetworkManager.StartClient();
            }
            else
            {

            }
        }

        public void StartGame()
        {

        }

        private bool CanConnect()
        {
            return true;
        }
    }
}