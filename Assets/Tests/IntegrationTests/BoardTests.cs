using CCGP.Server;
using CCGP.Shared;
using NUnit.Framework;
using System.Collections.Generic;

namespace CCGP.Tests.Integration
{
    [TestFixture]
    public class BoardTests
    {
        uint matchID = 1;
        List<ulong> players = new() { 1, 2, 3, 4 };
        [Test]
        public void InitialoardCount()
        {
            var game = GameSystemFactory.Create(matchID, players);
            game.Awake();

            var match = game.GetMatch();

            Assert.AreEqual(23, match.Board.Tiles.Count);
        }
    }
}