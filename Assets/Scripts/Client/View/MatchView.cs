using CCGP.Shared;

namespace CCGP.Client
{
    public class MatchView : BaseView
    {
        public SerializedMatch Data { get; private set; }

        public override void Activate()
        {
            this.AddObserver(OnUpdateData, Global.MessageNotification(GameCommand.StartGame), Container);
            this.AddObserver(OnUpdateData, Global.MessageNotification(GameCommand.StartRound), Container);
            this.AddObserver(OnUpdateData, Global.MessageNotification(GameCommand.StartTurn), Container);
            this.AddObserver(OnUpdateData, Global.MessageNotification(GameCommand.EndTurn), Container);
            this.AddObserver(OnUpdateData, Global.MessageNotification(GameCommand.EndRound), Container);
            this.AddObserver(OnUpdateData, Global.MessageNotification(GameCommand.EndGame), Container);
        }

        public override void Deactivate()
        {
            this.RemoveObserver(OnUpdateData, Global.MessageNotification(GameCommand.StartGame), Container);
            this.RemoveObserver(OnUpdateData, Global.MessageNotification(GameCommand.StartRound), Container);
            this.RemoveObserver(OnUpdateData, Global.MessageNotification(GameCommand.StartTurn), Container);
            this.RemoveObserver(OnUpdateData, Global.MessageNotification(GameCommand.EndTurn), Container);
            this.RemoveObserver(OnUpdateData, Global.MessageNotification(GameCommand.EndRound), Container);
            this.RemoveObserver(OnUpdateData, Global.MessageNotification(GameCommand.EndGame), Container);
        }

        private void OnUpdateData(object sender, object args)
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