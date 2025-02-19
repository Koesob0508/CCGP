using CCGP.AspectContainer;
using CCGP.Server;
using CCGP.Shared;
using NUnit.Framework;
using System.Collections.Generic;

namespace CCGP.Tests.Unit
{
    [TestFixture]
    public class PlayerTests
    {
        private const ulong ID = 5;
        private const int Index = 1;

        [Test]
        public void AccessTests()
        {
            var playerInfo = PlayerInfo.CreatePlayerInfo("asdf", ID);
            var player = new Player(Index, playerInfo);
            var leader = player[Zone.Leader];
            var deck = player[Zone.Deck];
            var hand = player[Zone.Hand];
            var graveyard = player[Zone.Graveyard];

            Assert.IsNotNull(leader);
            Assert.IsNotNull(deck);
            Assert.IsNotNull(hand);
            Assert.IsNotNull(graveyard);
        }
    }
}