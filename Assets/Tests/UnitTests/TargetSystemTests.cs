using CCGP.AspectContainer;
using CCGP.Server;
using CCGP.Shared;
using NUnit.Framework;
using System.Collections.Generic;

namespace CCGP.Tests.Unit
{
    [TestFixture]
    public class TargetSystemTests
    {
        uint MatchID = 1;
        List<ulong> Players = new() { 5, 6, 21, 77 };
        Container game;

        [SetUp]
        public void SetUp()
        {
            game = GameSystemFactory.Create(MatchID, Players);
            game.Awake();

            game.TryGetAspect<ActionSystem>(out var actionSys);
            game.TryGetAspect<FlowSystem>(out var flowSys);
            
            flowSys.StartGame();

            for (int i = 0; i < 100; i++)
            {
                actionSys.Update();
            }
        }

        [TearDown]
        public void TearDown()
        {
            game.Sleep();
            game = null;
        }

        [Test]
        public void ValidTargetTest()
        {
            game.TryGetAspect<ActionSystem>(out var actionSys);

            var current = game.GetMatch().CurrentPlayerIndex;
            var card = game.GetMatch().Players[current][Zone.Hand][0];
            Logger.Log<TargetSystemTests>($"{card.Name} {card.Space}");

            var cardPlayAction = new CardPlayAction(card);
            game.Perform(cardPlayAction);

            for (int i = 0; i < 3; i++)
            {
                actionSys.Update();
                Assert.IsTrue(actionSys.IsActive);
            }

            game.TryGetAspect<TargetSystem>(out var targetSys);
            var currentAction = actionSys.CurrentAction as CardPlayAction;

            Tile targetTile = null;
            
            foreach(var tile in game.GetMatch().Board.Tiles)
            {
                if(card.Space.Contains(tile.Space))
                {
                    targetTile = tile;
                    break;
                }
            }

            targetSys.SetTarget(currentAction, targetTile);

            for (int i = 0; i < 3; i++)
            {
                actionSys.Update();
            }

            Assert.IsFalse(currentAction.IsCanceled);
        }

        [Test]
        public void InvalidTargetTest()
        {
            game.TryGetAspect<ActionSystem>(out var actionSys);

            var current = game.GetMatch().CurrentPlayerIndex;
            var card = game.GetMatch().Players[current][Zone.Hand][0];
            Logger.Log<TargetSystemTests>($"{card.Name} {card.Space}");

            var cardPlayAction = new CardPlayAction(card);
            game.Perform(cardPlayAction);

            for (int i = 0; i < 3; i++)
            {
                actionSys.Update();
                Assert.IsTrue(actionSys.IsActive);
            }

            game.TryGetAspect<TargetSystem>(out var targetSys);
            var currentAction = actionSys.CurrentAction as CardPlayAction;

            Tile targetTile = new Tile() { Name = "None", Space = Space.None };

            targetSys.SetTarget(currentAction, targetTile);

            for (int i = 0; i < 3; i++)
            {
                actionSys.Update();
            }

            Assert.IsTrue(currentAction.IsCanceled);
        }

    }
}