using UnityEngine;
using Unity.Services.Core;
using System.Threading.Tasks;

namespace ACode.UGS
{
    public static class AUGS
    {
        public static async Task InitializeAsync()
        {
            try
            {
                if (UnityServices.State != ServicesInitializationState.Initialized)
                {
                    await UnityServices.InitializeAsync();
                    Debug.Log("Unity Services initialized.");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error initializing Unity Services: " + e.Message);
                throw;
            }
        }
    }
}