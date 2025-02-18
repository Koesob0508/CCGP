using CCGP.Shared;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace CCGP.Client
{
    public class PlayerView : BaseView
    {
        public List<SerializedPlayer> Data { get; private set; }
        public GameObject Hand;
        public GameObject SelectedCard;
        public CardView Prefab_CardView;

        public override void Activate()
        {
            this.AddObserver(OnStartGame, Global.MessageNotification(GameCommand.StartGame), Container);
            this.AddObserver(OnDrawCards, Global.MessageNotification(GameCommand.DrawCards), Container);
        }

        public override void Deactivate()
        {
            this.RemoveObserver(OnStartGame, Global.MessageNotification(GameCommand.StartGame), Container);
            this.RemoveObserver(OnDrawCards, Global.MessageNotification(GameCommand.DrawCards), Container);
        }

        private void OnStartGame(object sender, object args)
        {
            Data = new();

            foreach (var player in GetComponent<MatchView>().Data.Players)
            {
                Data.Add(player);
            }

            // 플레이어 정보 등록
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
    }
}