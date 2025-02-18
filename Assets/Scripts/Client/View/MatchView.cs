using CCGP.Shared;
using UnityEngine;

namespace CCGP.Client
{
    public class MatchView : BaseView
    {
        public SerializedMatch Data { get; private set; }

        public override void Activate()
        {
            this.AddObserver(OnGameStart, Global.MessageNotification(GameCommand.StartGame), Container);
            this.AddObserver(OnRoundStart, Global.MessageNotification(GameCommand.StartRound), Container);
        }

        public override void Deactivate()
        {
            this.RemoveObserver(OnGameStart, Global.MessageNotification(GameCommand.StartGame), Container);
        }

        private void OnGameStart(object sender, object args)
        {
            var sData = args as SerializedData;

            if(sData != null)
            {
                Data = sData.Get<SerializedMatch>();
            }
        }

        private void OnRoundStart(object sender, object args)
        {
            var sData = args as SerializedData;

            if(sData != null)
            {
                Data = sData.Get<SerializedMatch>();
            }
        }
    }
}