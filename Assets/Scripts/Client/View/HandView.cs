using System.Collections.Generic;
using CCGP.Shared;
using Unity.Netcode;
using UnityEngine;

namespace CCGP.Client
{
    public class HandView : BaseView
    {
        private int PlayerIndex;

        public GameObject Root_Hand;
        public GameObject Root_SelectedCard;
        public List<CardView> HandCards;

        [Header("Prefab")]
        public CardView Prefab_CardView;

        public override void Activate()
        {
            // this.AddObserver(OnStartGame, Global.MessageNotification(GameCommand.StartGame), Container);

            // this.AddObserver(OnRoundStartDraw, Global.MessageNotification(GameCommand.RoundStartDraw), Container);
            // this.AddObserver(OnDrawCards, Global.MessageNotification(GameCommand.DrawCards), Container);
            // this.AddObserver(OnGenerateCards, Global.MessageNotification(GameCommand.GenerateCard), Container);
            // this.AddObserver(OnRevealCards, Global.MessageNotification(GameCommand.RevealCards), Container);
            // this.AddObserver(OnEndTurn, Global.MessageNotification(GameCommand.EndTurn), Container);
        }

        public override void Deactivate()
        {
            // this.RemoveObserver(OnStartGame, Global.MessageNotification(GameCommand.StartGame), Container);

            // this.RemoveObserver(OnRoundStartDraw, Global.MessageNotification(GameCommand.RoundStartDraw), Container);
            // this.RemoveObserver(OnDrawCards, Global.MessageNotification(GameCommand.DrawCards), Container);
            // this.RemoveObserver(OnGenerateCards, Global.MessageNotification(GameCommand.GenerateCard), Container);
            // this.RemoveObserver(OnRevealCards, Global.MessageNotification(GameCommand.RevealCards), Container);
            // this.RemoveObserver(OnEndTurn, Global.MessageNotification(GameCommand.EndTurn), Container);
        }

        private void OnStartGame(object sender, object args)
        {
            HandCards = new();
            PlayerIndex = GetComponent<PlayerView>().ClientPlayer.Index;
        }

        private void OnRoundStartDraw(object sender, object args)
        {
            var sData = args as SerializedData;
            var sAction = sData.Get<SerializedRoundStartDrawAction>();

            foreach (var card in sAction[PlayerIndex])
            {
                var cardView = Instantiate(Prefab_CardView, Root_Hand.transform);
                cardView.UpdateData(card);
                cardView.Enable();

                HandCards.Add(cardView);
            }
        }

        private void OnDrawCards(object sender, object args)
        {
            var sData = args as SerializedData;
            var sAction = sData.Get<SerializedDrawCardsAction>();

            LogUtility.Log<HandView>($"{sAction.Player.ClientID} Player draw cards", colorName: ColorCodes.ClientSequencer);

            if (sAction.Player.ClientID == NetworkManager.Singleton.LocalClientId)
            {
                foreach (var card in sAction.Cards)
                {
                    var cardView = Instantiate(Prefab_CardView, Root_Hand.transform);
                    cardView.UpdateData(card);
                    cardView.Enable();

                    HandCards.Add(cardView);
                }
            }
        }

        private void OnGenerateCards(object sender, object args)
        {
            var sData = args as SerializedData;
            var sCard = sData.Get<SerializedCard>();

            LogUtility.Log<HandView>($"Player {sCard.OwnerIndex} generates card.", colorName: ColorCodes.ClientSequencer);

            if (PlayerIndex == sCard.OwnerIndex)
            {
                var cardView = Instantiate(Prefab_CardView, Root_Hand.transform);
                cardView.UpdateData(sCard);
                cardView.Enable();

                HandCards.Add(cardView);
            }
        }

        private void OnRevealCards(object sender, object args)
        {
            var sData = args as SerializedData;
            var sPlayer = sData.Get<SerializedPlayer>();

            LogUtility.Log<HandView>($"Player {sPlayer.Index} reveals Card.", colorName: ColorCodes.ClientSequencer);

            if (PlayerIndex != sPlayer.Index) return;

            foreach (var cardView in HandCards)
            {
                LogUtility.Log<HandView>($"Card {cardView.Name} set active false", colorName: ColorCodes.ClientSequencer);
                cardView.gameObject.SetActive(false);
            }
        }

        private void OnEndTurn(object sender, object args)
        {
            var targetCards = new List<CardView>();

            foreach (var cardView in HandCards)
            {
                if (cardView.gameObject.activeSelf == false)
                {
                    targetCards.Add(cardView);
                }
            }

            for (int i = targetCards.Count - 1; i >= 0; i--)
            {
                var cardView = targetCards[i];

                HandCards.Remove(cardView);
                targetCards.Remove(cardView);

                Destroy(cardView.gameObject);
            }
        }
    }
}