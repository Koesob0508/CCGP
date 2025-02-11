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
                LogUtility.Log("�κ��Դϴ�. ���� ���������� ������ �����Ǿ� ���� �ʽ��ϴ�.");
            }
            else
            {
                LogUtility.Log("�ΰ����Դϴ�. ���� ���������� ������ �����Ǿ� ���� �ʽ��ϴ�.");
            }
        }

        public void OnPointerExit()
        {
            LogUtility.Log("��ǳ�� �ݱ�");
        }

        public void OnPointerClick()
        {
            if(IsLobby)
            {
                LogUtility.Log("�׺� ����� ���� �޴� ����");
            }
            else
            {
                LogUtility.Log("�׺� ����� �ִ� �޴� ����");
            }
        }
    }
}