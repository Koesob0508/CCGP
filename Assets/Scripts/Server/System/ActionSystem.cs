using CCGP.AspectContainer;
using CCGP.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting.YamlDotNet.Core.Events;

namespace CCGP.Server
{
    public class ActionSystem : Aspect
    {
        #region Notifications
        private const string beginSequenceNotification = "ActionSystem.BeginSequenceNotification";
        private const string endSequenceNotification = "ActionSystem.EndSequenceNotification";
        private const string deathRepaerNotification = "ActionSystem.DeathReaperNotification";
        private const string completeNotification = "ActionSystem.CompleteNotification";
        #endregion

        private Queue<GameAction> actionQueue = new();
        private GameAction rootAction;
        private List<GameAction> openReactions;
        public bool IsActive => rootAction != null;

        public void Perform(GameAction action)
        {
            actionQueue.Enqueue(action);
            while (actionQueue.Count > 0)
            {
                if (rootAction == null)
                {
                    rootAction = actionQueue.Dequeue();
                    Sequence(rootAction);
                }
            }
        }

        public void AddReaction(GameAction action)
        {
            if (openReactions == null)
            {
                openReactions = new();
            }

            openReactions.Add(action);
        }

        private void Sequence(GameAction action)
        {
            this.PostNotification(beginSequenceNotification, action);

            if (action.Validate() == false)
            {
                action.Cancel();
            }

            openReactions = new();
            Entity.PostNotification(Global.PrepareNotification(action.GetType()));
            ProcessReactions();

            if (action.IsCanceled)
            {
                Entity.PostNotification(Global.CancelNotification(action.GetType()));
                ProcessReactions();
            }
            else
            {
                Entity.PostNotification(Global.PerformNotification(action.GetType()));
                ProcessReactions();
            }

            if (action == rootAction)
            {
                Entity.PostNotification(deathRepaerNotification, action);
                ProcessReactions();
            }

            this.PostNotification(endSequenceNotification, action);

            if (action == rootAction)
            {
                rootAction = null;
                openReactions = null;
                this.PostNotification(completeNotification);
            }
        }

        private void ProcessReactions()
        {
            openReactions.Sort((x, y) => x.OrderOfPlay.CompareTo(y.OrderOfPlay));

            var sortedReactions = new SortedList<int, GameAction>();
            foreach (var reaction in openReactions)
            {
                sortedReactions.Add(reaction.OrderOfPlay, reaction);
            }

            foreach (var reaction in sortedReactions.Values)
            {
                Sequence(reaction);
            }
        }
    }

    public static class ActionSystemExtensions
    {
        public static void Perform(this IEntity game, GameAction action)
        {
            if (game.TryGetAspect<ActionSystem>(out var system))
            {
                system.Perform(action);
            }
        }

        public static void AddReaction(this IEntity game, GameAction action)
        {
            if (game.TryGetAspect<ActionSystem>(out var system))
            {
                system.AddReaction(action);
            }
        }
    }
}