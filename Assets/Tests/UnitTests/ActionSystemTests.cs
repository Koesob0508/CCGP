using NUnit.Framework;
using Moq;
using CCGP.Server;
using CCGP.Shared;
using System.Collections.Generic;
using CCGP.AspectContainer;
using System;

namespace CCGP.Tests.Unit
{
    [TestFixture]
    public class ActionSystemTests
    {
        private ActionSystem actionSystem;
        private Mock<IContainer> mockContainer;

        class TestAction1 : GameAction
        {

        }
        class TestAction2 : GameAction
        {

        }

        class TestAction3 : GameAction
        {

        }

        [SetUp]
        public void SetUp()
        {
            actionSystem = new ActionSystem();
            mockContainer = new Mock<IContainer>();
        }

        /// <summary>
        ///     각각의 GameAction은 Type에 따라 다른 ID를 가져야 한다.
        /// </summary>
        [Test]
        public void ActionTypesHaveUniqueIDs()
        {
            var action1 = new GameAction();
            var action2 = new GameAction();
            var action3 = new TestAction1();
            Assert.AreEqual(action1.ID, action2.ID, "GameAction IDs should be unique per Type.");
            Assert.AreNotEqual(action2.ID, action3.ID, "GameAction IDs should be unique per Type.");
        }

        /// <summary>
        ///     Perform 이후 Notification이 이뤄지는 Observe로 확인한다.
        /// </summary>
        [Test]
        public void Notifications()
        {
            var action = new GameAction();
            bool notificationReceived = false;

            mockContainer.Object.AddObserver((sender, e) => { notificationReceived = true; }, Global.PerformNotification(action.GetType()));

            actionSystem.Perform(action);

            Assert.IsTrue(notificationReceived, "Perform notification should be observed.");
        }

        /// <summary>
        ///     특정 액션의 Prepare를 Observe하고 Observe를 하고 있는지 확인한다.
        /// </summary>
        [Test]
        public void ReactionNotifications()
        {
            var action = new GameAction();
            bool reactionObserved = false;

            mockContainer.Object.AddObserver((sender, e) => { reactionObserved = true; }, Global.PrepareNotification(action.GetType()));

            actionSystem.Perform(action);

            Assert.IsTrue(reactionObserved, "Prepare notification should be observed.");
        }

        /// <summary>
        ///     특정 액션의 Prepare에 대해 OrderOfPlay 값이 다른 여러 Reaction을 등록하고,
        ///     이 Reaction들이 순차적으로 동작하는지 확인
        /// </summary>
        [Test]
        public void ReactionsAreSorted()
        {
            var mainAction = new GameAction { OrderOfPlay = 0 };
            var reaction1 = new GameAction { OrderOfPlay = 2 };
            var reaction2 = new GameAction { OrderOfPlay = 1 };

            actionSystem.AddReaction(reaction1);
            actionSystem.AddReaction(reaction2);
            actionSystem.Perform(mainAction);

            Assert.That(actionSystem.IsActive, Is.False, "All reactions should have been processed in order.");
        }

        /// <summary>
        ///     특정 액션 Perform
        ///     해당 액션 Prepare 단에서 이 액션을 Cancel 시키는 액션을 등록
        ///     Cancel이 제대로 이뤄지는지 확인
        /// </summary>
        [Test]
        public void CancelAction()
        {
            var action = new GameAction();
            var cancelAction = new GameAction();
            bool actionCancelled = false;

            mockContainer.Object.AddObserver((sender, e) => { actionCancelled = true; action.Cancel(); }, Global.PrepareNotification(action.GetType()));

            actionSystem.Perform(action);

            Assert.IsTrue(actionCancelled, "Action should be cancelled during Prepare phase.");
        }

        /// <summary>
        ///     클래스 TestAction1, TestAction2, TestAction3가 있다.
        ///     Entity 또는 Aspect가 각각의 Action의 다양한 알림에 대해 Observe하고 있다.
        ///     TestAction1, 2, 3가 Perform으로 등록됐을 때,
        ///     이 처리가 DFS, 즉 TestAction1 실행 - 관측하고 있던 동작 수행, 2 실행 - 관측 실행, 3 실행 - 관측 실행
        ///     의 순서로 동작해야한다.
        /// </summary>
        [Test]
        public void DepthFirstReactions()
        {
            var testAction1 = new TestAction1 { OrderOfPlay = 0 };
            var testAction2 = new TestAction2 { OrderOfPlay = 1 };
            var testAction3 = new TestAction3 { OrderOfPlay = 2 };

            List<string> executionOrder = new();

            var container1 = new Mock<IContainer>();
            var container2 = new Mock<IContainer>();
            var container3 = new Mock<IContainer>();

            container1.Object.AddObserver((sender, e) => { executionOrder.Add("TestAction1 Executed"); }, Global.PerformNotification(testAction1.GetType()));
            container2.Object.AddObserver((sender, e) => { executionOrder.Add("TestAction2 Executed"); }, Global.PerformNotification(testAction2.GetType()));
            container3.Object.AddObserver((sender, e) => { executionOrder.Add("TestAction3 Executed"); }, Global.PerformNotification(testAction3.GetType()));

            actionSystem.Perform(testAction1);
            actionSystem.Perform(testAction2);
            actionSystem.Perform(testAction3);

            List<string> expectedOrder = new() { "TestAction1 Executed", "TestAction2 Executed", "TestAction3 Executed" };
            Assert.AreEqual(expectedOrder, executionOrder, $"Actions should execute in depth-first order.");
        }
    }
}
