using CCGP.Shared;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CCGP.Client
{
    public class Button_Settings : MonoBehaviour
    {
        private GameView View;
        private bool IsLobby = true;

        public GameObject Panel_Lobby;
        public Button Button_LobbyReturn;
        public Button Button_LobbyQuit;

        public GameObject Panel_GameMenu;
        public Button Button_GameReturn;
        public Button Button_GameQuit;


        public void Start()
        {
            View = GetComponentInParent<GameView>();

            this.AddObserver(OnGameStart, Global.MessageNotification(GameCommand.StartGame), View.Container);

            Button_LobbyReturn.onClick.AddListener(() =>
            {
                Panel_GameMenu.SetActive(false);
            });

            Button_LobbyQuit.onClick.AddListener(() =>
            {
                LogUtility.Log("게임 종료");
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;  // 에디터에서는 Play 모드 종료
#else
                Application.Quit();  // 빌드된 게임에서는 정상 종료
#endif
            });


            Button_GameReturn.onClick.AddListener(() =>
            {
                Panel_GameMenu.SetActive(false);
            });

            Button_GameQuit.onClick.AddListener(() =>
            {
                LogUtility.Log("게임 종료");
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;  // 에디터에서는 Play 모드 종료
#else
                Application.Quit();  // 빌드된 게임에서는 정상 종료
#endif
            });
        }

        private void OnGameStart(object sender, object args)
        {
            IsLobby = false;
        }

        public void OnPointerEnter()
        {
            if (IsLobby)
            {
                gameObject.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
            }
            else
            {
                gameObject.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
            }
        }

        public void OnPointerExit()
        {
            gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        public void OnPointerClick()
        {
            if (IsLobby)
            {
                Panel_Lobby.SetActive(true);
            }
            else
            {
                Panel_GameMenu.SetActive(true);
            }
        }
    }
}