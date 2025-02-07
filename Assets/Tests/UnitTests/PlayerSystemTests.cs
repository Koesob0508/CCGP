using CCGP.AspectContainer;
using CCGP.Server;
using CCGP.Shared;
using NUnit.Framework;
using System.Collections.Generic;

namespace CCGP.Tests.Unit
{
    public class PlayerSystemTests
    {
        List<ulong> Players = new() { 5, 6, 7, 8 };
        Container game;

        [SetUp]
        public void SetUp()
        {
            game = GameFactory.Create();
            game.Awake();

            // gameStart까지 진행해버리자
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
        public void OverDrawTest()
        {
            // Arrage
            var currentIndex = game.GetMatch().CurrentPlayerIndex;

            List<string> cardGUIDs = new();
            foreach (var card in game.GetMatch().Players[currentIndex][Zone.Deck])
            {
                cardGUIDs.Add(card.GUID);
            }
            foreach (var card in game.GetMatch().Players[currentIndex][Zone.Hand])
            {
                cardGUIDs.Add(card.GUID);
            }

            // Deck, Hand, Discard 세팅
            // 현재 Player에게 Discard 넣어주기
            List<Card> discards = new()
            {
                CardFactory.CreateCard("00-001", currentIndex),
                CardFactory.CreateCard("00-002", currentIndex),
                CardFactory.CreateCard("00-003", currentIndex),
                CardFactory.CreateCard("00-003", currentIndex),
                CardFactory.CreateCard("00-002", currentIndex),
            };

            foreach (var card in discards)
            {
                card.Zone = Zone.Graveyard;
            }

            game.GetMatch().Players[currentIndex][Zone.Graveyard].AddRange(discards);

            foreach (var card in game.GetMatch().Players[currentIndex][Zone.Graveyard])
            {
                cardGUIDs.Add(card.GUID);
            }

            // Act
            var action = new CardsDrawAction(game.GetMatch().Players[currentIndex], 10);
            game.Perform(action);

            // Assert
            Assert.AreEqual(15, game.GetMatch().Players[currentIndex][Zone.Hand].Count);
            // 동일성 검증
            foreach (var card in game.GetMatch().Players[currentIndex][Zone.Hand])
            {
                Assert.IsTrue(cardGUIDs.Contains(card.GUID));
            }
            // 셔플되면서 Graveyard에 카드가 없어야한다.
            Assert.AreEqual(0, game.GetMatch().Players[currentIndex][Zone.Graveyard].Count);

            var overAction = new CardsDrawAction(game.GetMatch().Players[currentIndex], 5);
            game.Perform(overAction);

            Assert.AreEqual(0, game.GetMatch().Players[currentIndex][Zone.Deck].Count);
            Assert.AreEqual(15, game.GetMatch().Players[currentIndex][Zone.Hand].Count);
            Assert.AreEqual(0, game.GetMatch().Players[currentIndex][Zone.Graveyard].Count);
        }

        [Test]
        public void PlayCard_Should_MyTurn()
        {
            // 우선 currentPlayerIndex는 알아두자
            var match = game.GetMatch();
            var currentPlayerIndex = match.CurrentPlayerIndex;

            // Turn 종료를 호출해서 바꾼다.
            game.TryGetAspect<FlowSystem>(out var flow);
            flow.EndTurn(currentPlayerIndex); // TurnEndAction perform이 맞긴 한데 아무리봐도 귀찮은 절차임. FlowSystem 직원이 있는데 왜 안써

            // 다시 기존 currentPlayerIndex로 CardPlay
            var targetCard = match.Players[currentPlayerIndex][Zone.Hand][0];
            var action = new CardPlayAction(targetCard);
            game.Perform(action);

            // Zone이 바뀌어선 안된다.
            Assert.AreEqual(Zone.Hand, targetCard.Zone);
            Assert.AreEqual(0, match.Players[currentPlayerIndex][Zone.Agent].Count);
            Assert.AreEqual(Player.InitialHand, match.Players[currentPlayerIndex][Zone.Hand].Count);
        }

        [Test]
        public void PlayCard_Should_InHand()
        {
            // currentPlayerIndex로 Deck에서 Play 시도
            var match = game.GetMatch();
            var currentPlayerIndex = match.CurrentPlayerIndex;
            var targetCard = match.Players[currentPlayerIndex][Zone.Deck][0];

            // 그리고 currentPlayerIndex로 CardPlay
            var action = new CardPlayAction(targetCard);
            game.Perform(action);

            // Zone 안바뀌면 됨
            Assert.AreEqual(Zone.Deck, targetCard.Zone);
            Assert.AreEqual(0, match.Players[currentPlayerIndex][Zone.Agent].Count);
        }
    }
}