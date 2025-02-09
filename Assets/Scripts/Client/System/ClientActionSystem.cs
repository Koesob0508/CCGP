using CCGP.AspectContainer;
using CCGP.Shared;
using System.Collections;
using System.Collections.Generic;

namespace CCGP.Client
{
    class ClientActionSystem : Aspect, IUpdate
    {
        #region Notifications
        private const string beginSequenceNotification = "ClientActionSystem.BeginSequenceNotification";
        private const string endSequenceNotification = "ClientActionSystem.EndSequenceNotification";
        private const string completeNotification = "ClientActionSystem.CompleteNotification";
        #endregion

        private Queue<ViewAction> actionQueue = new();
        private ViewAction rootAction;
        private IEnumerator rootSequence;
        private List<ViewAction> openReactions;

        public ViewAction CurrentAction => rootAction;
        public bool IsActive => rootAction != null;

        public void Perform(ViewAction action)
        {
            actionQueue.Enqueue(action);
        }

        public void Update()
        {
            if(rootSequence == null && actionQueue.Count > 0)
            {
                rootAction = actionQueue.Dequeue();
                rootSequence = Sequence(rootAction);
            }

            if(rootSequence != null)
            {
                if(!rootSequence.MoveNext())
                {
                    rootAction = null;
                    rootSequence = null;
                    openReactions = null;
                    this.PostNotification(completeNotification);
                }
            }
        }

        private IEnumerator Sequence(ViewAction action)
        {
            this.PostNotification(beginSequenceNotification, action);

            var phase = MainPhase(action.PreparePhase);
            while(phase.MoveNext()) { yield return null; }

            phase = MainPhase(action.PerformPhase);
            while(phase.MoveNext()) { yield return null; }

            phase = MainPhase(action.CancelPhase);
            while(phase.MoveNext()) { yield return null; }

            this.PostNotification(endSequenceNotification, action);
        }

        private IEnumerator MainPhase(ViewPhase phase)
        {
            bool isActionCanceled = phase.Owner.IsCanceled;
            bool isCancelPhase = (phase.Owner.CancelPhase == phase);

            if (isActionCanceled ^ isCancelPhase)
                yield break;

            openReactions = new List<ViewAction>();
            var flow = phase.Flow(Container);
            while (flow.MoveNext()) { yield return null; }

            var react = ReactPhase(openReactions);
            while(react.MoveNext()) { yield return null; }
        }

        private IEnumerator ReactPhase(List<ViewAction> reactions)
        {
            reactions.Sort(SortActions);
            foreach(var reaction in reactions)
            {
                var reactFlow = Sequence(reaction);
                while (reactFlow.MoveNext()) { yield return null; }
            }
        }

        private int SortActions(ViewAction x, ViewAction y)
        {
            if (x.Priority != y.Priority)
                return y.Priority.CompareTo(x.Priority);
            return x.OrderOfPlay.CompareTo(y.OrderOfPlay);
        }
    }
}
