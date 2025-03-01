using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.Authentication;

namespace ACode.UGS
{
    public static class AAuthenticationService
    {
        public static string PlayerID => AuthenticationService.Instance.PlayerId;
        public static async Task SignInAnonymouslyAsync()
        {
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"Sign In Anonymously : {AuthenticationService.Instance.PlayerId}");
            }
            else
            {
                Debug.LogWarning($"Already Signed In : {AuthenticationService.Instance.PlayerId}");
            }
        }
    }
}