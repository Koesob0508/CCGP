using CCGP.Shared;
using CCGP.Server;
using NUnit.Framework;

namespace CCGP.Tests.Unit
{
    [TestFixture]
    public class TileFactoryTests
    {
        [Test]
        public void LoadTest()
        {
            var tile = TileFactory.CreateTile("00-001");

            Assert.AreEqual("IMPERIAL BASIN", tile.Name);
            Assert.AreEqual(Space.Yellow, tile.Space);
        }
    }
}