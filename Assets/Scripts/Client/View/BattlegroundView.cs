using System.Collections.Generic;
using CCGP.Shared;
using UnityEngine;

namespace CCGP.Client
{
    public class BattlegroundView : BaseView
    {
        private int PlayerIndex;

        public GameObject Root_Battlefield;
        public BoardCardView Prefab_BoardCard;

        private List<BoardCardView> BoardCards;

        public override void Activate()
        {
            // this.AddObserver(OnStartGame, Global.MessageNotification(GameCommand.StartGame), Container);
            // this.AddObserver(OnRevealCards, Global.MessageNotification(GameCommand.RevealCards), Container);
            // this.AddObserver(OnEndRound, Global.MessageNotification(GameCommand.EndRound), Container);
        }

        public override void Deactivate()
        {
            // this.RemoveObserver(OnStartGame, Global.MessageNotification(GameCommand.StartGame), Container);
            // this.RemoveObserver(OnRevealCards, Global.MessageNotification(GameCommand.RevealCards), Container);
            // this.RemoveObserver(OnEndRound, Global.MessageNotification(GameCommand.EndRound), Container);
        }

        private void OnStartGame(object sender, object args)
        {
            // BoardCards = new();
            // PlayerIndex = GetComponent<PlayerView>().ClientPlayer.Index;
        }

        private void OnRevealCards(object sender, object args)
        {
            // var sData = args as SerializedData;
            // var sPlayer = sData.Get<SerializedPlayer>();

            // LogUtility.Log<BattlefieldView>($"Player {sPlayer.Index} reveals Card.", colorName: ColorCodes.ClientSequencer);

            // if (sPlayer.Index == PlayerIndex)
            // {
            //     foreach (var card in sPlayer.Reveal)
            //     {
            //         var boardCardView = Instantiate(Prefab_BoardCard, Root_Battlefield.transform);
            //         boardCardView.UpdateData(card);
            //         BoardCards.Add(boardCardView);
            //     }
            // }
            // else
            // {

            // }
        }

        private void OnEndRound(object sender, object args)
        {
            // for (int i = BoardCards.Count - 1; i >= 0; i--)
            // {
            //     var boardCardView = BoardCards[i];

            //     BoardCards.Remove(boardCardView);

            //     Destroy(boardCardView.gameObject);
            // }
        }
    }
}