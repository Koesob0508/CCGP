using CCGP.Shared;
using Unity.Netcode;
using UnityEditor;
using UnityEngine.UI;

namespace CCGP.Client
{
    public class TurnView : BaseView
    {
        private SerializedMatch Match;
        private SerializedPlayer Player;
        public Button Button_TurnEnd;

        public override void Activate()
        {
            // TurnView가 반응해야할 곳은
            // GameStart : 내 턴인지 확인해야한다.
            // TurnEnd : 이제 누구의 턴인지 확인해서 활성화 결정해야함
            // 내 턴을 확인해야하므로, Player를 갖고 있어야한다.
            Button_TurnEnd.onClick.AddListener(OnClick);

            this.AddObserver(OnStartGame, Global.MessageNotification(GameCommand.StartGame), Container);
            this.AddObserver(OnStartTurn, Global.MessageNotification(GameCommand.StartTurn), Container);
            this.AddObserver(OnEndGame, Global.MessageNotification(GameCommand.EndGame), Container);
        }

        public override void Deactivate()
        {
            Button_TurnEnd.onClick = null;

            this.RemoveObserver(OnStartGame, Global.MessageNotification(GameCommand.StartGame), Container);
            this.RemoveObserver(OnStartTurn, Global.MessageNotification(GameCommand.StartTurn), Container);
            this.RemoveObserver(OnEndGame, Global.MessageNotification(GameCommand.EndGame), Container);
        }

        private void OnClick()
        {
            // 버튼에 등록할 이벤트
            // 턴 종료 메시지를 보낼 것
            var action = new SerializedTurnEndAction() { Player = Player };
            this.PostNotification(ClientDialect.EndTurn, action);
        }

        private void OnStartGame(object sender, object args)
        {
            Match = GetComponent<MatchView>().Data;
            Player = GetComponent<PlayerView>().Data.Find(p => p.ClientID == NetworkManager.Singleton.LocalClientId);

            RefreshButton();
        }

        private void OnStartTurn(object sender, object args)
        {
            Match = GetComponent<MatchView>().Data;
            LogUtility.Log<TurnView>($"Start Turn : {Match.CurrentPlayerIndex}", colorName: ColorCodes.ClientSequencer);
            RefreshButton();
        }

        private void OnEndGame(object sender, object args)
        {
            LogUtility.Log<TurnView>($"End Game", colorName: ColorCodes.ClientSequencer);
            CloseButton();
        }

        private void RefreshButton()
        {
            // 버튼 활성화 여부 결정
            if (Match.CurrentPlayerIndex == Player.Index)
            {
                Button_TurnEnd.interactable = true;
            }
            else
            {
                Button_TurnEnd.interactable = false;
            }
        }

        private void CloseButton()
        {
            Button_TurnEnd.interactable = false;
        }
    }
}