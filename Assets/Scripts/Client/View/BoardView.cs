using CCGP.AspectContainer;
using CCGP.Shared;
using System.Collections.Generic;
using UnityEngine;

namespace CCGP.Client
{
    public class BoardView : BaseView
    {
        public BoardViewModel Data { get; private set; }

        public List<TileView> Tiles;

        public override void Activate()
        {
            this.AddObserver(OnGameStart, Global.MessageNotification(GameCommand.StartGame), Container);
        }

        public override void Deactivate()
        {
            this.RemoveObserver(OnGameStart, Global.MessageNotification(GameCommand.StartGame), Container);
        }

        private void OnGameStart(object sender, object args)
        {
            Data = GetComponent<MatchView>().Data.Board;

            for(int i = 0; i < Data.Tiles.Count; i++)
            {
                Tiles[i].gameObject.SetActive(true);
                Tiles[i].Refresh(Data.Tiles[i]);
            }
        }
    }
}