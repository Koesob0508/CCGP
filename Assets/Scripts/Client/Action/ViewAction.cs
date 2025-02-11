using CCGP.AspectContainer;
using CCGP.Shared;

namespace CCGP.Client
{
    public class ViewAction
    {
        public readonly int ID;
        public PlayerViewModel Player { get; set; }
        public int Priority { get; set; }
        public int OrderOfPlay { get; set; }
        public bool IsCanceled { get; protected set; }
        public ViewPhase PreparePhase { get; protected set; } // Player 입력에 반응
        public ViewPhase PerformPhase { get; protected set; } // Server에서 Perform 결정이 떨어진 경우
        public ViewPhase CancelPhase { get; protected set; } // Server에서 Cancel 결정이 떨어진 경우

        public ViewAction()
        {
            ID = Global.GenerateID(GetType());
        }

        protected virtual void OnPrepareKeyFrame(IContainer viewCenter)
        {
            var notificationName = Global.PrepareNotification(GetType());
            viewCenter.PostNotification(notificationName, this);
        }

        protected virtual void OnPerformKeyFrame(IContainer viewCenter)
        {
            var notificationName = Global.PerformNotification(GetType());
            viewCenter.PostNotification(notificationName, this);
        }

        protected virtual void OnCancelKeyFrame(IContainer viewCenter)
        {
            var notificationName = Global.CancelNotification(GetType());
            viewCenter.PostNotification(notificationName, this);
        }
    }
}