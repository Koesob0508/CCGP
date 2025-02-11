using CCGP.Shared;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace CCGP.Client
{
    public class PlayerView : BaseView
    {
        public List<PlayerViewModel> Data { get; private set; }
        public GameObject Hand;
        public CardView CardView;

        public override void Activate()
        {
            this.AddObserver(OnStartRound, Global.MessageNotification(GameCommand.StartRound), Container);
            this.AddObserver(OnDrawCards, Global.MessageNotification(GameCommand.DrawCards), Container);
        }

        public override void Deactivate()
        {
            this.RemoveObserver(OnStartRound, Global.MessageNotification(GameCommand.StartRound), Container);
        }

        private void OnStartRound(object sender, object args)
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

            // 만일 내꺼라면,
            // 카드를 뽑는다.
            // 이 때, 카드 정보는 SerializedCard로 갱신
            // 아 지금 당장은 SerializedCard가 아니라 CardViewModel이다.

            if(sAction.Player.ID == NetworkManager.Singleton.LocalClientId)
            {
                foreach(var card in sAction.Cards)
                {
                    var cardView = Instantiate(CardView, Hand.transform);
                    var cardVM = new CardViewModel(card);
                    cardView.Enable();
                    cardView.Refresh(cardVM);
                }
            }
        }
    }
}