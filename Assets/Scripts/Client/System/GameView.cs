using CCGP.Shared;
using UnityEngine;

namespace CCGP.Client
{
    public class GameView : BaseView
    {
        public GameObject GamePanel;

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
            GamePanel.SetActive(true);
        }
    }
}