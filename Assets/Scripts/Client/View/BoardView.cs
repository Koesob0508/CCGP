using CCGP.AspectContainer;
using CCGP.Shared;
using System.Collections.Generic;
using UnityEngine;

namespace CCGP.Client
{
    public class BoardView : BaseView
    {
        public SerializedBoard Data { get; private set; }

        public List<TileView> Tiles;

        public override void Activate()
        {
            this.AddObserver(OnStartGame, Global.MessageNotification(GameCommand.StartGame), Container);
            this.AddObserver(OnShowAvailableTiles, Global.MessageNotification(GameCommand.ShowAvailableTiles), Container);
            this.AddObserver(OnHideAvailableTiles, Global.MessageNotification(GameCommand.PlayCard), Container);
            this.AddObserver(OnHideAvailableTiles, Global.MessageNotification(GameCommand.CancelPlayCard), Container);
        }

        public override void Deactivate()
        {
            this.RemoveObserver(OnStartGame, Global.MessageNotification(GameCommand.StartGame), Container);
        }

        private void OnStartGame(object sender, object args)
        {
            Data = GetComponent<MatchView>().Data.Board;

            for (int i = 0; i < Data.Tiles.Count; i++)
            {
                Tiles[i].gameObject.SetActive(true);
                Tiles[i].UpdateView(Data.Tiles[i]);
            }
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

        private void OnHideAvailableTiles(object sender, object args)
        {
            LogUtility.Log<BoardView>("Hide available tiles", colorName: ColorCodes.Client);

            for (int i = 0; i < Data.Tiles.Count; i++)
            {
                Tiles[i].BlockImage.SetActive(false);
            }
        }
    }
}