using ACode.UGS;
using UnityEngine;
using UnityEngine.UI;

namespace CCGP.Client
{
    public class AuthenticationViewManager : MonoBehaviour
    {
        private GameView View;

        public GameObject Panel_Info;
        public Button Button_SignIn;

        private async void Start()
        {
            await AUGS.InitializeAsync();

            View = GetComponentInParent<GameView>();

            Button_SignIn.onClick.AddListener(() => SignInAnonymouslyAsync());

            Panel_Info.SetActive(true);
        }

        private async void SignInAnonymouslyAsync()
        {
            await AAuthenticationService.SignInAnonymouslyAsync();

            Panel_Info.SetActive(false);

            View.Container.PostNotification(ClientDialect.SignInAnonymouslyAsync);
        }
    }
}