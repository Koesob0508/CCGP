using CCGP.Server;
using NUnit.Framework;
using System.Collections.Generic;

namespace CCGP.Tests.Unit
{
    [TestFixture]
    public class DataSystemTests
    {
        List<ulong> ids = new List<ulong> { 1, 2, 3, 4 };

        [Test]
        public void InitializeTest()
        {
            var system = new DataSystem();

            Assert.IsNotNull(system.match);
            Assert.IsNotNull(system.match.Players);
            Assert.AreEqual(Player.InitialDeck, system.match.Players[0][Zone.Deck].Count);
        }
    }
}