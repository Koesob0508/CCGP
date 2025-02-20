using CCGP.Shared;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace CCGP.Client
{
    public class PlayerView : BaseView
    {
        private SerializedPlayer ClientPlayer;

        public List<SerializedPlayer> Data { get; private set; }
        public GameObject Hand;
        public GameObject SelectedCard;
        public CardView Prefab_CardView;

        #region Client Player
        public TMP_Text Text_Lunar;
        public TMP_Text Text_Marsion;
        public TMP_Text Text_Water;
        public TMP_Text Text_Emperor;
        public TMP_Text Text_SpacingGuild;
        public TMP_Text Text_BeneGesserit;
        public TMP_Text Text_Fremen;
        #endregion

        #region Player #1
        public TMP_Text Text_Player1_Lunar;
        public TMP_Text Text_Player1_Marsion;
        public TMP_Text Text_Player1_Water;
        public TMP_Text Text_Player1_Emperor;
        public TMP_Text Text_Player1_SpacingGuild;
        public TMP_Text Text_Player1_BeneGesserit;
        public TMP_Text Text_Player1_Fremen;
        #endregion

        #region Player #2
        public TMP_Text Text_Player2_Lunar;
        public TMP_Text Text_Player2_Marsion;
        public TMP_Text Text_Player2_Water;
        public TMP_Text Text_Player2_Emperor;
        public TMP_Text Text_Player2_SpacingGuild;
        public TMP_Text Text_Player2_BeneGesserit;
        public TMP_Text Text_Player2_Fremen;
        #endregion

        #region Player #3
        public TMP_Text Text_Player3_Lunar;
        public TMP_Text Text_Player3_Marsion;
        public TMP_Text Text_Player3_Water;
        public TMP_Text Text_Player3_Emperor;
        public TMP_Text Text_Player3_SpacingGuild;
        public TMP_Text Text_Player3_BeneGesserit;
        public TMP_Text Text_Player3_Fremen;
        #endregion

        public override void Activate()
        {
            // Phase callbacks
            this.AddObserver(OnUpdateData, Global.MessageNotification(GameCommand.StartGame), Container);
            this.AddObserver(OnUpdateData, Global.MessageNotification(GameCommand.StartRound), Container);
            this.AddObserver(OnUpdateData, Global.MessageNotification(GameCommand.StartTurn), Container);
            this.AddObserver(OnUpdateData, Global.MessageNotification(GameCommand.EndTurn), Container);
            this.AddObserver(OnUpdateData, Global.MessageNotification(GameCommand.EndRound), Container);
            this.AddObserver(OnUpdateData, Global.MessageNotification(GameCommand.EndGame), Container);

            // Operation callbacks
            this.AddObserver(OnDrawCards, Global.MessageNotification(GameCommand.DrawCards), Container);
        }

        public override void Deactivate()
        {
            this.RemoveObserver(OnUpdateData, Global.MessageNotification(GameCommand.StartGame), Container);
            this.RemoveObserver(OnUpdateData, Global.MessageNotification(GameCommand.StartRound), Container);
            this.RemoveObserver(OnUpdateData, Global.MessageNotification(GameCommand.StartTurn), Container);
            this.RemoveObserver(OnUpdateData, Global.MessageNotification(GameCommand.EndTurn), Container);
            this.RemoveObserver(OnUpdateData, Global.MessageNotification(GameCommand.EndRound), Container);
            this.RemoveObserver(OnUpdateData, Global.MessageNotification(GameCommand.EndGame), Container);

            this.RemoveObserver(OnDrawCards, Global.MessageNotification(GameCommand.DrawCards), Container);
        }

        private void OnDrawCards(object sender, object args)
        {
            var sData = args as SerializedData;
            var sAction = sData.Get<SerializedCardsDrawAction>();

            LogUtility.Log<PlayerView>($"{sAction.Player.ClientID}", colorName: ColorCodes.ClientSequencer);

            if (sAction.Player.ClientID == NetworkManager.Singleton.LocalClientId)
            {
                foreach (var card in sAction.Cards)
                {
                    var cardView = Instantiate(Prefab_CardView, Hand.transform);
                    cardView.Enable();
                    cardView.Refresh(card);
                }
            }
        }

        private void OnUpdateData(object sender, object args)
        {
            Data = new();

            int nonLocalIndex = 0;

            foreach (var player in GetComponent<MatchView>().Data.Players)
            {
                Data.Add(player);

                // 단 이 클라이언트에 해당하는 Player일 경우, 별도의 UI를 업데이트
                if (player.ClientID == NetworkManager.Singleton.LocalClientId)
                {
                    ClientPlayer = player;

                    Text_Lunar.text = ClientPlayer.Lunar.ToString();
                    Text_Marsion.text = ClientPlayer.Marsion.ToString();
                    Text_Water.text = ClientPlayer.Water.ToString();
                    Text_Emperor.text = ClientPlayer.EmperorInfluence.ToString();
                    Text_SpacingGuild.text = ClientPlayer.SpacingGuildInfluence.ToString();
                    Text_BeneGesserit.text = ClientPlayer.BeneGesseritInfluence.ToString();
                    Text_Fremen.text = ClientPlayer.FremenInfluence.ToString();
                }
                else
                {
                    switch (nonLocalIndex)
                    {
                        case 0:
                            Text_Player1_Lunar.text = player.Lunar.ToString();
                            Text_Player1_Marsion.text = player.Marsion.ToString();
                            Text_Player1_Water.text = player.Water.ToString();
                            Text_Player1_Emperor.text = player.EmperorInfluence.ToString();
                            Text_Player1_SpacingGuild.text = player.SpacingGuildInfluence.ToString();
                            Text_Player1_BeneGesserit.text = player.BeneGesseritInfluence.ToString();
                            Text_Player1_Fremen.text = player.FremenInfluence.ToString();
                            break;
                        case 1:
                            Text_Player2_Lunar.text = player.Lunar.ToString();
                            Text_Player2_Marsion.text = player.Marsion.ToString();
                            Text_Player2_Water.text = player.Water.ToString();
                            Text_Player2_Emperor.text = player.EmperorInfluence.ToString();
                            Text_Player2_SpacingGuild.text = player.SpacingGuildInfluence.ToString();
                            Text_Player2_BeneGesserit.text = player.BeneGesseritInfluence.ToString();
                            Text_Player2_Fremen.text = player.FremenInfluence.ToString();
                            break;
                        case 2:
                            Text_Player3_Lunar.text = player.Lunar.ToString();
                            Text_Player3_Marsion.text = player.Marsion.ToString();
                            Text_Player3_Water.text = player.Water.ToString();
                            Text_Player3_Emperor.text = player.EmperorInfluence.ToString();
                            Text_Player3_SpacingGuild.text = player.SpacingGuildInfluence.ToString();
                            Text_Player3_BeneGesserit.text = player.BeneGesseritInfluence.ToString();
                            Text_Player3_Fremen.text = player.FremenInfluence.ToString();
                            break;
                        default:
                            break;
                    }

                    nonLocalIndex++;
                }
            }
        }
    }
}