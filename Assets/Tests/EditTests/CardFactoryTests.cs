using CCGP.Server;
using CCGP.Shared;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace CCGP.Tests.Unit
{
    [TestFixture]
    public class CardFactoryTests
    {
        [Test]
        public void LogTest()
        {
            var card = new Card();
            LogUtility.Log<CardFactoryTests>($"{card.Space}");
            card.Space |= Space.Yellow;
            card.Space |= Space.BeneGesserit;
            card.Space |= Space.Fremen;
            LogUtility.Log<CardFactoryTests>($"{card.Space}");

            LogAssert.Expect(UnityEngine.LogType.Log, "[CardFactoryTests] <color=black><b>Yellow, BeneGesserit, Fremen</b></color>");
        }

        [Test]
        public void LoadTest()
        {
            var card = CardFactory.CreateCard("00-001", 1);

            LogUtility.Log<CardFactoryTests>(card.Name);
            LogUtility.Log<CardFactoryTests>(card.Space.ToString());

            LogAssert.Expect("[CardFactoryTests] <color=black><b>Signet Ring</b></color>");
            LogAssert.Expect("[CardFactoryTests] <color=black><b>Yellow, Blue, Green</b></color>");
        }

        [Test]
        public void GuidTest()
        {
            var card1 = CardFactory.CreateCard("00-001", 1);
            var card2 = CardFactory.CreateCard("00-001", 1);

            Assert.AreNotEqual(card1.GUID, card2.GUID);
        }

        [Test]
        public void CreateDeckTest()
        {
            var deck = CardFactory.CreateBaseDeck("InitialDeck", 1);

            Assert.AreEqual(Player.InitialDeck, deck.Count);
        }
    }
}