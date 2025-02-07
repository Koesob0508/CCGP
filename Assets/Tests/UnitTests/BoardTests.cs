using CCGP.Server;
using CCGP.Shared;
using NUnit.Framework;
using System.Collections.Generic;

namespace CCGP.Tests.Integration
{
    [TestFixture]
    public class BoardTests
    {
        List<ulong> players = new() { 1, 2, 3, 4 };
        [Test]
        public void InitialoardCount()
        {
            var game = GameFactory.Create();
            game.Awake();

            var match = game.GetMatch();

            Assert.AreEqual(23, match.Board.Tiles.Count);
        }
    }
}