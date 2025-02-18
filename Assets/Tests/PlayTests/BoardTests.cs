using CCGP.Server;
using CCGP.Shared;
using NUnit.Framework;
using System.Collections.Generic;

namespace CCGP.Tests.Play
{
    [TestFixture]
    public class BoardTests
    {
        List<ulong> players = new() { 1, 2, 3, 4 };
        [Test]
        public void InitialLoadCount()
        {
            var game = GameFactory.Create();
            game.Activate();

            var match = game.GetMatch();

            Assert.AreEqual(23, match.Board.Tiles.Count);
        }
    }
}