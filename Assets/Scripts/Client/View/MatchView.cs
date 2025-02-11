using CCGP.Shared;
using UnityEngine;

namespace CCGP.Client
{
    public class MatchView : BaseView
    {
        public MatchViewModel Data { get; private set; }

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
                var sMatch = sData.Get<SerializedMatch>();
                Data = new MatchViewModel(sMatch);
            }
        }

        private void OnRoundStart(object sender, object args)
        {
            var sData = args as SerializedData;

            if(sData != null)
            {
                var sMatch = sData.Get<SerializedMatch>();
                Data = new MatchViewModel(sMatch);
            }
        }
    }
}