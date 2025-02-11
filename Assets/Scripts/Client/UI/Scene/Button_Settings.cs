using CCGP.Shared;
using UnityEngine;

namespace CCGP.Client
{
    public class Button_Settings : MonoBehaviour
    {
        private GameView View;
        private bool IsLobby = true;


        public void Start()
        {
            View = GetComponentInParent<GameView>();

            this.AddObserver(OnGameStart, Global.MessageNotification(GameCommand.StartGame), View.Container);
        }

        private void OnGameStart(object sender, object args)
        {
            IsLobby = false;
        }

        public void OnPointerEnter()
        {
            if(IsLobby)
            {
                LogUtility.Log("로비입니다. 현재 버전에서는 설정이 구현되어 있지 않습니다.");
            }
            else
            {
                LogUtility.Log("인게임입니다. 현재 버전에서는 설정이 구현되어 있지 않습니다.");
            }
        }

        public void OnPointerExit()
        {
            LogUtility.Log("말풍선 닫기");
        }

        public void OnPointerClick()
        {
            if(IsLobby)
            {
                LogUtility.Log("항복 기능이 없는 메뉴 오픈");
            }
            else
            {
                LogUtility.Log("항복 기능이 있는 메뉴 오픈");
            }
        }
    }
}