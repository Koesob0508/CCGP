using CCGP.AspectContainer;
using CCGP.Server;
using CCGP.Shared;
using NUnit.Framework;
using System.Collections.Generic;

namespace CCGP.Tests.Unit
{
    [TestFixture]
    public class CardSystemTests
    {
        private Container game;
        private List<ulong> ids = new() { 1, 2, 3, 4 };

        [SetUp]
        public void SetUp()
        {
            game = GameFactory.Create();
            game.Awake();
            game.TryGetAspect<FlowSystem>(out var flow);
            flow.StartGame();
        }

        [TearDown]
        public void TearDown()
        {
            game.Sleep();
            game = null;
        }

        [Test]
        public void ChangeZoneTest()
        {
            // Arrange
            var targetPlayer = game.GetMatch().Players[0];
            var targetCard = targetPlayer[Zone.Hand][0];

            // Act
            game.TryGetAspect<CardSystem>(out var cardSys);
            cardSys.ChangeZone(targetCard, Zone.Graveyard);

            // Assert
            Assert.AreEqual(Player.InitialHand - 1, targetPlayer[Zone.Hand].Count);
            Assert.AreEqual(1, targetPlayer[Zone.Graveyard].Count);
            Assert.AreEqual(targetCard.GUID, targetPlayer[Zone.Graveyard][0].GUID);
        }
    }
}