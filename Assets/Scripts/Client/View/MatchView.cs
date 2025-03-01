using CCGP.Shared;

namespace CCGP.Client
{
    public class MatchView : BaseView
    {
        public SerializedMatch Data { get; private set; }

        public override void Activate()
        {
            this.AddObserver(OnUpdateData, Global.MessageNotification(GameCommand.UpdateData), Container);
        }

        public override void Deactivate()
        {
            this.RemoveObserver(OnUpdateData, Global.MessageNotification(GameCommand.UpdateData), Container);
        }

        private void OnUpdateData(object sender, object args)
        {
            // LogUtility.Log<MatchView>("Update Match", colorName: ColorCodes.Client);

            // var sData = args as SerializedData;

            // if (sData != null)
            // {
            //     Data = sData.Get<SerializedMatch>();
            // }
        }
    }
}