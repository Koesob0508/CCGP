using CCGP.Shared;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace CCGP.Client
{
    public class BoardView : BaseView
    {
        public SerializedBoard Data { get; private set; }

        public List<TileView> Tiles;

        public override void Activate()
        {
            this.AddObserver(OnUpdateData, Global.MessageNotification(GameCommand.UpdateData), Container);

            // Not Phase, But Change
            this.AddObserver(OnPlayCard, Global.MessageNotification(GameCommand.PlayCard), Container);

            // Not Phase, Not Change, Just Notify
            this.AddObserver(OnShowAvailableTiles, Global.MessageNotification(GameCommand.ShowAvailableTiles), Container);
            this.AddObserver(OnCancel, Global.MessageNotification(GameCommand.CancelTryPlayCard), Container);
            this.AddObserver(OnCancel, Global.MessageNotification(GameCommand.CancelPlayCard), Container);
        }

        public override void Deactivate()
        {
            this.RemoveObserver(OnUpdateData, Global.MessageNotification(GameCommand.UpdateData), Container);

            this.RemoveObserver(OnPlayCard, Global.MessageNotification(GameCommand.PlayCard), Container);

            // Not Phase, Not Change, Just Notify
            this.RemoveObserver(OnShowAvailableTiles, Global.MessageNotification(GameCommand.ShowAvailableTiles), Container);
            this.RemoveObserver(OnCancel, Global.MessageNotification(GameCommand.CancelTryPlayCard), Container);
            this.RemoveObserver(OnCancel, Global.MessageNotification(GameCommand.CancelPlayCard), Container);
        }

        private void OnUpdateData(object sender, object args)
        {
            Data = GetComponent<MatchView>().Data.Board;

            for (int i = 0; i < Tiles.Count; i++)
            {
                var id = Tiles[i].Text_ID.text;
                var sTile = Data.Tiles.Find(x => x.ID == id);

                if (sTile == null)
                {
                    LogUtility.LogWarning<BoardView>($"ID : {id} tile not found", colorName: ColorCodes.Yellow);
                }
                else
                {
                    Tiles[i].gameObject.SetActive(true);
                    Tiles[i].UpdateData(sTile);
                }
            }
        }

        private void OnPlayCard(object sender, object args)
        {
            HideAvailableTiles();
        }

        private void OnCancel(object sender, object args)
        {
            HideAvailableTiles();
        }

        private void OnShowAvailableTiles(object sender, object args)
        {
            var sData = args as SerializedData;
            var sTiles = sData.Get<SerializedTiles>();

            LogUtility.Log<BoardView>("Show available tiles", colorName: ColorCodes.Client);

            for (int i = 0; i < Data.Tiles.Count; i++)
            {
                if (sTiles.Tiles.Contains(Tiles[i].Data))
                {
                    Tiles[i].BlockImage.SetActive(false);
                }
                else
                {
                    Tiles[i].BlockImage.SetActive(true);
                }
            }
        }

        private void HideAvailableTiles()
        {
            LogUtility.Log<BoardView>("Hide available tiles", colorName: ColorCodes.Client);

            for (int i = 0; i < Data.Tiles.Count; i++)
            {
                Tiles[i].BlockImage.SetActive(false);
            }
        }
    }
}