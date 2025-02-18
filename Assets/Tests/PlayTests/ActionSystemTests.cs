using NUnit.Framework;

using CCGP.AspectContainer;  // Container, IContainer
using CCGP.Server;           // ActionSystem, GameAction, CardPlayAction 등
using CCGP.Shared;
using UnityEngine.TestTools;
using System.Collections;

namespace CCGP.Tests.Play
{
    [TestFixture]
    public class ActionSystemTests
    {
        Container container;
        ActionSystem actionSystem;

        [SetUp]
        public void SetUp()
        {
            container = new Container();
            actionSystem = container.AddAspect<ActionSystem>();
        }

        [TearDown]
        public void TearDown()
        {
            actionSystem = null;
            container = null;
        }
        
        [Test]
        public void ObserveTest()
        {
            // 여러 bool 플래그들을 선언 (각 Notification 관찰 여부를 기록)
            bool isBegin = false;
            bool isEnd = false;
            bool isDeath = false;
            bool isComplete = false;
            bool isValidate = false;
            bool isPrepare = false;
            bool isPerform = false;
            bool isCancel = false;

            var action = new GameAction();

            // Arrange 1: ActionSystem의 Sequence 단위 Notification 관찰
            this.AddObserver((sender, args) => {
                isBegin = true;
            }, ActionSystem.BeginSequenceNotification, actionSystem);

            this.AddObserver((sender, args) => {
                isEnd = true;
            }, ActionSystem.EndSequenceNotification, actionSystem);

            this.AddObserver((sender, args) => {
                isDeath = true;
            }, ActionSystem.DeathReaperNotification, actionSystem);

            this.AddObserver((sender, args) => {
                isComplete = true;
            }, ActionSystem.CompleteNotification, actionSystem);

            // Arrange 2: Global Notification (Validate, Prepare, Perform, Cancel) 관찰
            this.AddObserver((sender, args) => {
                isValidate = true;
            }, Global.ValidateNotification<GameAction>());

            this.AddObserver((sender, args) => {
                isPrepare = true;
            }, Global.PrepareNotification<GameAction>(), container);

            this.AddObserver((sender, args) => {
                isPerform = true;
            }, Global.PerformNotification<GameAction>(), container);

            this.AddObserver((sender, args) => {
                isCancel = true;
            }, Global.CancelNotification<GameAction>(), container);

            // Action
            container.Perform(action);

            // Assert
            Assert.IsTrue(isBegin, "BeginSequenceNotification was not observed.");
            Assert.IsTrue(isEnd, "EndSequenceNotification was not observed.");
            Assert.IsTrue(isDeath, "DeathReaperNotification was not observed.");
            Assert.IsTrue(isComplete, "CompleteNotification was not observed.");
            Assert.IsTrue(isValidate, "ValidationNotification<GameAction> was not observed.");
            Assert.IsTrue(isPrepare, "PrepareNotification<GameAction> was not observed.");
            Assert.IsTrue(isPerform, "PerformNotification<GameAction> was not observed.");
            Assert.IsFalse(isCancel, "CancelNotification<GameAction> was not observed.");
        }

        [Test]
        public void CancelTest()
        {
            bool isCancel = false;
            var action = new GameAction();

            this.AddObserver((sender, args) =>
            {
                var target = sender as GameAction;
                target.Cancel();
            }, Global.ValidateNotification<GameAction>());

            this.AddObserver((sender, args) => {
                isCancel = true;
            }, Global.CancelNotification<GameAction>(), container);

            // Action
            container.Perform(action);

            // Assert
            Assert.IsTrue(isCancel, "CancelNotification<GameAction> was not observed.");
        }

        //[UnityTest]
        //public IEnumerator Test()
        //{
        //    yield return null;
        //}
    }
}