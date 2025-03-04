using CCGP.Shared;
using UnityEngine;

namespace CCGP.Client
{
    public class ImperiumView : BaseView
    {
        public SerializedImperium Data { get; private set; }

        public GameObject Row;
        public ImperiumCardView ImperiumCard_1;
        public ImperiumCardView ImperiumCard_2;
        public ImperiumCardView ImperiumCard_3;
        public ImperiumCardView ImperiumCard_4;
        public ImperiumCardView ImperiumCard_5;

        public override void Activate()
        {
            // this.AddObserver(OnUpdateData, Global.MessageNotification(GameCommand.UpdateData), Container);
        }

        public override void Deactivate()
        {
            // this.RemoveObserver(OnUpdateData, Global.MessageNotification(GameCommand.UpdateData), Container);
        }

        private void OnUpdateData(object sender, object args)
        {
            Data = GetComponent<MatchView>().Data.Imperium;

            // Card Update
            ImperiumCardView[] ImperiumCards = { ImperiumCard_1, ImperiumCard_2, ImperiumCard_3, ImperiumCard_4, ImperiumCard_5 };

            foreach (var cardObj in ImperiumCards)
            {
                cardObj.gameObject.SetActive(false);
            }

            for (int i = 0; i < Data.Row.Count; i++)
            {
                var imperiumCard = ImperiumCards[i];
                imperiumCard.UpdateData(Data.Row[i]);
                imperiumCard.gameObject.SetActive(true);
            }
        }
    }
}