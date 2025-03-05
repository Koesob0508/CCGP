using CCGP.Shared;
using UnityEngine;

namespace CCGP.Client
{
    public class GameView : BaseView
    {
        public GameObject Canvas_Game;

        public override void Activate()
        {
            this.AddObserver(OnStartGame, Global.MessageNotification(GameCommand.StartGame), Container);
        }

        public override void Deactivate()
        {
            this.RemoveObserver(OnStartGame, Global.MessageNotification(GameCommand.StartGame), Container);
        }

        private void OnStartGame(object sender, object args)
        {
            Canvas_Game.SetActive(true);
        }

        private void OnDisable()
        {
            Canvas_Game.SetActive(false);

            LogUtility.Log<GameView>("Game Canvas set active false");
        }
    }
}