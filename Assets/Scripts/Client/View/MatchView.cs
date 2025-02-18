using CCGP.Shared;

namespace CCGP.Client
{
    public class MatchView : BaseView
    {
        public SerializedMatch Data { get; private set; }

        public override void Activate()
        {
            this.AddObserver(OnUpdateMatch, Global.MessageNotification(GameCommand.StartGame), Container);
            this.AddObserver(OnUpdateMatch, Global.MessageNotification(GameCommand.StartRound), Container);
            this.AddObserver(OnUpdateMatch, Global.MessageNotification(GameCommand.StartTurn), Container);
            this.AddObserver(OnUpdateMatch, Global.MessageNotification(GameCommand.EndTurn), Container);
            this.AddObserver(OnUpdateMatch, Global.MessageNotification(GameCommand.EndRound), Container);
            this.AddObserver(OnUpdateMatch, Global.MessageNotification(GameCommand.EndGame), Container);
        }

        public override void Deactivate()
        {
            this.RemoveObserver(OnUpdateMatch, Global.MessageNotification(GameCommand.StartGame), Container);
            this.RemoveObserver(OnUpdateMatch, Global.MessageNotification(GameCommand.StartRound), Container);
            this.RemoveObserver(OnUpdateMatch, Global.MessageNotification(GameCommand.StartTurn), Container);
            this.RemoveObserver(OnUpdateMatch, Global.MessageNotification(GameCommand.EndTurn), Container);
            this.RemoveObserver(OnUpdateMatch, Global.MessageNotification(GameCommand.EndRound), Container);
            this.RemoveObserver(OnUpdateMatch, Global.MessageNotification(GameCommand.EndGame), Container);
        }

        private void OnUpdateMatch(object sender, object args)
        {
            LogUtility.Log<MatchView>("Update Match", colorName: ColorCodes.Client);

            var sData = args as SerializedData;

            if (sData != null)
            {
                Data = sData.Get<SerializedMatch>();
            }
        }
    }
}