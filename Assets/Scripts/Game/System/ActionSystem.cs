using CCGP.AspectContainer;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace CCGP.Server
{
    public class ActionSystem : Aspect
    {
        #region Notifications
        public const string BeginSequenceNotification = "ActionSystem.BeginSequenceNotification";
        public const string EndSequenceNotification = "ActionSystem.EndSequenceNotification";
        public const string DeathReaperNotification = "ActionSystem.DeathReaperNotification";
        public const string CompleteNotification = "ActionSystem.CompleteNotification";
        #endregion

        private Queue<GameAction> actionQueue = new();
        private GameAction rootAction;
        private IEnumerator rootSequence;
        private List<GameAction> openReactions;

        public GameAction CurrentAction => rootAction;
        public bool IsActive => rootAction != null;

        public async void Perform(GameAction action)
        {
            actionQueue.Enqueue(action);

            while (rootSequence != null || actionQueue.Count > 0)
            {
                await UniTask.Yield();
                Update();
            }
        }

        public void AddReaction(GameAction action)
        {
            openReactions ??= new List<GameAction>();
            openReactions.Add(action);
        }

        #region Private Method

        private void Update()
        {
            if (rootSequence == null && actionQueue.Count > 0)
            {
                rootAction = actionQueue.Dequeue();
                rootSequence = Sequence(rootAction);
            }

            if (rootSequence != null)
            {
                if (!rootSequence.MoveNext())
                {
                    rootAction = null;
                    rootSequence = null;
                    openReactions = null;
                    this.PostNotification(CompleteNotification);
                }
            }
        }

        private IEnumerator Sequence(GameAction action)
        {
            this.PostNotification(BeginSequenceNotification, action);

            // 1) Validate
            if (!action.Validate())
            {
                action.Cancel();
            }

            // 2) Prepare(phase)
            var phase = MainPhase(action.PreparePhase);
            while (phase.MoveNext()) { yield return null; }

            if (!action.IsCanceled)
            {
                // 3) Perform(phase)
                phase = MainPhase(action.PerformPhase);
                while (phase.MoveNext()) { yield return null; }
            }
            else
            {
                // 4) Cancel(phase)
                phase = MainPhase(action.CancelPhase);
                while (phase.MoveNext()) { yield return null; }
            }

            // (루트 액션만 deathReaper 등 처리)
            if (action == rootAction)
            {
                phase = EventPhase(DeathReaperNotification, action, true);
                while (phase.MoveNext()) { yield return null; }
            }

            this.PostNotification(EndSequenceNotification, action);
        }

        private IEnumerator MainPhase(Phase phase)
        {
            //// 취소 여부와 cancel Phase인지 여부 확인
            //bool isActionCanceled = phase.Owner.IsCanceled;
            //bool isCancelPhase = (phase.Owner.CancelPhase == phase);
            //// 취소/취소단계 불일치면 skip
            //if (isActionCanceled ^ isCancelPhase)
            //    yield break;

            // 현 Phase 실행
            openReactions = new List<GameAction>();
            var flow = phase.Flow(Container);
            while (flow.MoveNext()) { yield return null; }

            // Reaction 처리
            var react = ReactPhase(openReactions);
            while (react.MoveNext()) { yield return null; }
        }

        private IEnumerator ReactPhase(List<GameAction> reactions)
        {
            reactions.Sort(SortActions);
            foreach (var reaction in reactions)
            {
                var subFlow = Sequence(reaction);
                while (subFlow.MoveNext())
                    yield return null;
            }
        }

        private IEnumerator EventPhase(string notification, GameAction action, bool repeats = false)
        {
            List<GameAction> reactions;
            do
            {
                reactions = openReactions = new List<GameAction>();
                this.PostNotification(notification, action);

                var phase = ReactPhase(reactions);
                while (phase.MoveNext()) { yield return null; }
            }
            while (repeats && reactions.Count > 0);
        }

        private int SortActions(GameAction x, GameAction y)
        {
            // 우선순위 정렬
            if (x.Priority != y.Priority)
                return y.Priority.CompareTo(x.Priority);
            return x.OrderOfPlay.CompareTo(y.OrderOfPlay);
        }

        #endregion
    }

    public static class ActionSystemExtensions
    {
        public static void Perform(this IContainer game, GameAction action)
        {
            if (game.TryGetAspect<ActionSystem>(out var system))
            {
                system.Perform(action);
            }
        }

        public static void AddReaction(this IContainer game, GameAction action)
        {
            if (game.TryGetAspect<ActionSystem>(out var system))
            {
                system.AddReaction(action);
            }
        }
    }
}