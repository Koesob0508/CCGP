using CCGP.Server;
using UnityEngine;

namespace CCGP.Shared
{
    public class ManagersInitializer : MonoBehaviour
    {
        Server.Server Server;

        private void Start()
        {
            Server = ServerFactory.Create();

            Server.Init();
        }
    }
}