using CCGP.Client;
using CCGP.Server;
using UnityEngine;

namespace CCGP.Shared
{
    public class Managers : MonoBehaviour
    {
        Server.Server Server;
        Client.Client Client;

        private void Start()
        {
            Server = ServerFactory.Create();
            Client = ClientFactory.Create();

            Server.Init();
            Client.Init();
        }

        private void Update()
        {
            Client.Update();
        }
    }
}