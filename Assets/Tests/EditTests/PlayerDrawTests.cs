using CCGP.AspectContainer;
using CCGP.Server;
using CCGP.Shared;
using NUnit.Framework;
using System.Collections.Generic;

namespace CCGP.Tests
{
    public class PlayerSystemTests
    {
        private PlayerSystem playerSystem;
        private Player testPlayer;

        [SetUp]
        public void SetUp()
        {
            // PlayerSystem 인스턴스 생성
            var container = new Container();
            playerSystem = container.AddAspect<PlayerSystem>();
            container.AddAspect<CardSystem>();

            // 테스트용 Player 생성
            var playerInfo = new PlayerInfo();
            testPlayer = new Player(1, playerInfo);
        }

        [Test]
        public void Draw_WithEnoughDeckCards_ShouldDrawCorrectly()
        {
            // 덱에 카드 5장 추가
            for (int i = 0; i < 5; i++)
            {
                testPlayer[Zone.Deck].Add(new Card { Name = $"Card{i}" });
            }

            // 3장 드로우 요청
            List<Card> drawnCards = playerSystem.Draw(testPlayer, 3);

            // 검증
            Assert.AreEqual(3, drawnCards.Count, "드로우한 카드 수가 맞아야 합니다.");
            Assert.AreEqual(2, testPlayer[Zone.Deck].Count, "덱에 남은 카드 수가 맞아야 합니다.");
            Assert.AreEqual(3, testPlayer[Zone.Hand].Count, "손에 추가된 카드 수가 맞아야 합니다.");
        }

        [Test]
        public void Draw_WithEmptyDeck_ShouldRefillFromGraveyardAndShuffle()
        {
            // 덱이 빈 상태
            testPlayer[Zone.Deck].Clear();

            // 묘지(Graveyard)에 5장의 카드 추가
            for (int i = 0; i < 5; i++)
            {
                testPlayer[Zone.Graveyard].Add(new Card { Name = $"GraveyardCard{i}" });
            }

            // 3장 드로우 요청
            List<Card> drawnCards = playerSystem.Draw(testPlayer, 3);

            // 검증
            Assert.AreEqual(3, drawnCards.Count, "드로우한 카드 수가 맞아야 합니다.");
            Assert.AreEqual(2, testPlayer[Zone.Deck].Count, "덱에 남은 카드 수가 맞아야 합니다.");
            Assert.AreEqual(3, testPlayer[Zone.Hand].Count, "손에 추가된 카드 수가 맞아야 합니다.");
            Assert.AreEqual(0, testPlayer[Zone.Graveyard].Count, "묘지가 비워졌어야 합니다.");
        }

        [Test]
        public void Draw_WithDeckAndGraveyardEmpty_ShouldDrawNothing()
        {
            // 덱과 묘지 모두 비우기
            testPlayer[Zone.Deck].Clear();
            testPlayer[Zone.Graveyard].Clear();

            // 3장 드로우 요청
            List<Card> drawnCards = playerSystem.Draw(testPlayer, 3);

            // 검증
            Assert.AreEqual(0, drawnCards.Count, "드로우할 카드가 없을 경우 0장이 나와야 합니다.");
            Assert.AreEqual(0, testPlayer[Zone.Deck].Count, "덱은 여전히 비어 있어야 합니다.");
            Assert.AreEqual(0, testPlayer[Zone.Hand].Count, "손에도 추가된 카드가 없어야 합니다.");
            Assert.AreEqual(0, testPlayer[Zone.Graveyard].Count, "묘지는 비어 있어야 합니다.");
        }

        [Test]
        public void Draw_WithPartialDeckAndGraveyard_ShouldRefillAndContinueDraw()
        {
            // 덱에 2장 추가
            testPlayer[Zone.Deck].Add(new Card { Name = "DeckCard1" });
            testPlayer[Zone.Deck].Add(new Card { Name = "DeckCard2" });

            // 묘지에 3장 추가
            testPlayer[Zone.Graveyard].Add(new Card { Name = "GraveyardCard1" });
            testPlayer[Zone.Graveyard].Add(new Card { Name = "GraveyardCard2" });
            testPlayer[Zone.Graveyard].Add(new Card { Name = "GraveyardCard3" });

            // 4장 드로우 요청 (덱에는 2장만 있음)
            List<Card> drawnCards = playerSystem.Draw(testPlayer, 4);

            // 검증
            Assert.AreEqual(4, drawnCards.Count, "드로우한 카드 수가 맞아야 합니다.");
            Assert.AreEqual(1, testPlayer[Zone.Deck].Count, "묘지에서 덱으로 이동한 후, 덱에 1장이 남아야 합니다.");
            Assert.AreEqual(4, testPlayer[Zone.Hand].Count, "손에 추가된 카드 수가 맞아야 합니다.");
            Assert.AreEqual(0, testPlayer[Zone.Graveyard].Count, "묘지가 비워졌어야 합니다.");
        }
    }
}
