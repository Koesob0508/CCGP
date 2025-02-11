using CCGP.AspectContainer;
using CCGP.Server;
using CCGP.Shared;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace CCGP.Tests.Integration
{
    public class CardPlayActionTests
    {
        Container game;
        List<ulong> players = new() { 21, 32, 96, 72 };

        /// <summary>
        /// 만일 Scene Load와 같은 비동기 작업이 필요하다면
        /// UnitySetUp과
        /// yield return을 사용하시오.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            game = GameFactory.Create();
            game.Activate();
        }

        [TearDown]
        public void TearDown()
        {
            game.Deactivate();
            game = null;
        }

        [UnityTest]
        public IEnumerator ValidTarget_Test()
        {
            game.TryGetAspect<FlowSystem>(out var flowSystem);

            flowSystem.StartGame();

            var index = game.GetMatch().CurrentPlayerIndex;

            var card = CardFactory.CreateCard("00-001", index);
            card.Zone = Zone.Hand;
            var space = card.Space;

            var action = new CardPlayAction(card);

            game.Perform(action);

            yield return new WaitForSeconds(5f / 60f);

            game.TryGetAspect<ActionSystem>(out var actionSystem);

            Assert.IsTrue(actionSystem.IsActive);

            game.TryGetAspect<TargetSystem>(out var targetSystem);

            var tile = TileFactory.CreateTile("00-003");

            var cardAction = actionSystem.CurrentAction as CardPlayAction;

            Assert.IsNotNull(cardAction);

            targetSystem.SetTarget(actionSystem.CurrentAction as CardPlayAction, tile);

            yield return new WaitForSeconds(5f / 60f);

            Assert.IsFalse(actionSystem.IsActive);
            Assert.IsFalse(cardAction.IsCanceled);
        }

        [UnityTest]
        public IEnumerator InvalidTarget_Test()
        {
            game.TryGetAspect<FlowSystem>(out var flowSystem);

            flowSystem.StartGame();

            var index = game.GetMatch().CurrentPlayerIndex;

            var card = CardFactory.CreateCard("00-001", index);
            card.Zone = Zone.Hand;
            var space = card.Space;

            var action = new CardPlayAction(card);

            game.Perform(action);

            yield return new WaitForSeconds(5f / 60f);

            game.TryGetAspect<ActionSystem>(out var actionSystem);

            Assert.IsTrue(actionSystem.IsActive);

            game.TryGetAspect<TargetSystem>(out var targetSystem);

            var tile = TileFactory.CreateTile("00-017");

            var cardAction = actionSystem.CurrentAction as CardPlayAction;

            Assert.IsNotNull(cardAction);

            targetSystem.SetTarget(actionSystem.CurrentAction as CardPlayAction, tile);

            yield return new WaitForSeconds(5f / 60f);

            Assert.IsFalse(actionSystem.IsActive);
            Assert.IsTrue(cardAction.IsCanceled);
        }
    }
}