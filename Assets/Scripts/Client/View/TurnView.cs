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
        public Button Button_EndTurn;
        public Button Button_RevealCard;

        public override void Activate()
        {
            // TurnView가 반응해야할 곳은
            // GameStart : 내 턴인지 확인해야한다.
            // TurnEnd : 이제 누구의 턴인지 확인해서 활성화 결정해야함
            // 내 턴을 확인해야하므로, Player를 갖고 있어야한다.
            // Button_EndTurn.onClick.AddListener(OnClickEndTurn);
            // Button_RevealCard.onClick.AddListener(OnClickOpenCard);

            // this.AddObserver(OnStartGame, Global.MessageNotification(GameCommand.StartGame), Container);
            // this.AddObserver(OnStartTurn, Global.MessageNotification(GameCommand.StartTurn), Container);
            // this.AddObserver(OnEndGame, Global.MessageNotification(GameCommand.EndGame), Container);
            // this.AddObserver(OnRevealCards, Global.MessageNotification(GameCommand.RevealCards), Container);
        }

        public override void Deactivate()
        {
            // Button_EndTurn.onClick = null;

            // this.RemoveObserver(OnStartGame, Global.MessageNotification(GameCommand.StartGame), Container);
            // this.RemoveObserver(OnStartTurn, Global.MessageNotification(GameCommand.StartTurn), Container);
            // this.RemoveObserver(OnEndGame, Global.MessageNotification(GameCommand.EndGame), Container);
            // this.RemoveObserver(OnRevealCards, Global.MessageNotification(GameCommand.RevealCards), Container);
        }

        private void OnClickEndTurn()
        {
            // 버튼에 등록할 이벤트
            // 턴 종료 메시지를 보낼 것
            this.PostNotification(ClientDialect.EndTurn, Player);
        }

        private void OnClickOpenCard()
        {
            this.PostNotification(ClientDialect.OpenCard, Player);
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
            Player = GetComponent<PlayerView>().Data.Find(p => p.ClientID == NetworkManager.Singleton.LocalClientId);

            LogUtility.Log<TurnView>($"Start Turn : {Match.CurrentPlayerIndex}", colorName: ColorCodes.ClientSequencer);
            RefreshButton();
        }

        private void OnRevealCards(object sender, object args)
        {
            Match = GetComponent<MatchView>().Data;
            Player = GetComponent<PlayerView>().Data.Find(p => p.ClientID == NetworkManager.Singleton.LocalClientId);

            // Match값 보고 Open을 인지할 것
            RefreshButton();
        }

        private void OnEndGame(object sender, object args)
        {
            Match = GetComponent<MatchView>().Data;
            Player = GetComponent<PlayerView>().Data.Find(p => p.ClientID == NetworkManager.Singleton.LocalClientId);

            LogUtility.Log<TurnView>($"End Game", colorName: ColorCodes.ClientSequencer);
            CloseButton();
        }

        private void RefreshButton()
        {
            // 버튼 활성화 여부 결정
            if (Match.CurrentPlayerIndex == Player.Index)
            {
                LogUtility.Log<TurnView>($"내 턴임", colorName: ColorCodes.ClientSequencer);
                if (Player.IsRevealPhase)
                {
                    if (Player.IsRevealed)
                    {
                        LogUtility.Log<TurnView>($"RevealPhase인데 Reveal 끝남", colorName: ColorCodes.ClientSequencer);
                        Button_EndTurn.gameObject.SetActive(true);
                        Button_RevealCard.gameObject.SetActive(false);
                        Button_EndTurn.interactable = true;
                    }
                    else
                    {
                        LogUtility.Log<TurnView>($"RevealPhase이고 Reveal 해야함", colorName: ColorCodes.ClientSequencer);
                        Button_RevealCard.gameObject.SetActive(true);
                        Button_EndTurn.gameObject.SetActive(false);
                        Button_RevealCard.interactable = true;
                    }
                }
                else
                {
                    LogUtility.LogWarning<TurnView>($"RevealPhase 아님", colorName: ColorCodes.ClientSequencer);
                    Button_EndTurn.gameObject.SetActive(true);
                    Button_RevealCard.gameObject.SetActive(false);
                    Button_EndTurn.interactable = true;
                }
            }
            else
            {
                LogUtility.LogWarning<TurnView>($"내 턴 아님", colorName: ColorCodes.ClientSequencer);
                Button_EndTurn.interactable = false;
                Button_RevealCard.interactable = false;
                Button_EndTurn.gameObject.SetActive(true);
                Button_RevealCard.gameObject.SetActive(false);
            }
        }

        private void CloseButton()
        {
            Button_RevealCard.interactable = false;
            Button_EndTurn.interactable = false;
        }
    }
}