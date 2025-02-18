using CCGP.Shared;
using UnityEngine;

namespace CCGP.Client
{
    public class GameView : BaseView
    {
        public GameObject GamePanel;

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
            GamePanel.SetActive(true);
        }
    }
}