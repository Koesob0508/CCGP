using NUnit.Framework;
using Moq;
using CCGP.Server;
using CCGP.Shared;
using System.Collections.Generic;
using CCGP.AspectContainer;
using UnityEngine.TestTools;

namespace CCGP.Tests.Unit
{
    [TestFixture]
    public class FlowSystemTests
    {
        private Container game;
        private FlowSystem flowSystem;

        [SetUp]
        public void SetUp()
        {
            uint matchID = 1;
            List<ulong> players = new() { 1001, 1002, 1003, 1004 };
            game = GameSystemFactory.Create(matchID, players);
            game.Awake();
            game.TryGetAspect(out flowSystem);
        }

        /// <summary>
        ///     game.GetMatch를 통해서 Match 데이터는 가져올 수 있다.
        ///     flowSystem.StartGame 이후
        ///     Match의 Players가 null이 아니고, 4명인지 확인
        ///     그리고 FirstPlayerIndex가 랜덤하게 정해지는지 확인
        ///     마지막으로 flowSystem.StartGame으로 GameStartAciont이 ActionSystem에 의해 Trigger 됐는지 확인
        /// </summary>
        [Test]
        public void GameStartsCorrectly()
        {
            // Arrange
            // 테스트 진행을 위해 임의로 -1 지정
            game.GetMatch().FirstPlayerIndex = -1;
            game.GetMatch().CurrentPlayerIndex = -1;
            bool isPerform = false;

            this.AddObserver((sender, obj) =>
            {
                isPerform = true;
            }, Global.PerformNotification<GameStartAction>(), game);

            // Act
            flowSystem.StartGame();

            var match = game.GetMatch();
            Assert.NotNull(match, "Match 데이터가 존재해야 합니다.");
            Assert.NotNull(match.Players, "Players 리스트가 초기화되어 있어야 합니다.");
            Assert.GreaterOrEqual(match.Players.Count, 3, "게임에는 3~4명의 플레이어가 참가해야 합니다.");
            Assert.LessOrEqual(match.Players.Count, 4);

            // FirstPlayerIndex는 0~3 중 랜덤한 값이어야 한다.
            Assert.GreaterOrEqual(match.FirstPlayerIndex, 0, "FirstPlayerIndex는 0 이상이어야 합니다.");
            Assert.Less(match.FirstPlayerIndex, 4, "FirstPlayerIndex는 4 미만이어야 합니다.");

            // ActionSystem과 연결됐는지 테스트
            Assert.IsTrue(isPerform, "GameStartAction이 ActionSystem에 의해 Perform 돼야함");

            foreach(var player in match.Players)
            {
                Assert.AreEqual(Player.InitialDeck - Player.InitialHand, player[Zone.Deck].Count);
            }
        }

        [Test]
        public void RoundStartsCorrectly()
        {
            // Arrange
            bool isGameStartTriggered = false;
            bool isRoundStartTriggered = false;

            this.AddObserver((sender, obj) =>
            {
                isGameStartTriggered = true;
            }, Global.PerformNotification<GameStartAction>());

            this.AddObserver((sender, obj) =>
            {
                if (isGameStartTriggered)
                    isRoundStartTriggered = true;
            }, Global.PerformNotification<RoundStartAction>());

            // Act
            flowSystem.StartGame();

            LogAssert.Expect(UnityEngine.LogType.Log, "[FlowSystem] <color=black><b>라운드 시작</b></color>");

            var match = game.GetMatch();
            foreach (var player in match.Players)
            {
                Assert.AreEqual(Player.InitialHand, player[Zone.Hand].Count);
            }

            // Start 이후에 RoundStart가 관측되는지 확인
            Assert.IsTrue(isRoundStartTriggered, "RoundStartAction이 ActionSystem에 의해 Perform 되어야 합니다.");
        }

        [Test]
        public void TurnStartsCorrectly()
        {
            flowSystem.StartGame();
            var match = game.GetMatch();
            LogAssert.Expect(UnityEngine.LogType.Log, $"[FlowSystem] <color=black><b>{match.CurrentPlayerIndex}번째 플레이어 턴 시작</b></color>");
        }

        [Test]
        public void TurnEndsCorrectly()
        {
            // Arrange
            int turnEndCount = 0;
            this.AddObserver((sender, obj) =>
            {
                turnEndCount++;
            }, Global.PerformNotification<TurnEndAction>());

            flowSystem.StartGame();

            var match = game.GetMatch();
            var targetIndex = match.CurrentPlayerIndex;
            var nextIndex = (match.CurrentPlayerIndex + 1) % match.Players.Count;
            flowSystem.EndTurn(nextIndex);

            flowSystem.EndTurn(match.CurrentPlayerIndex);


            // Assert
            Assert.AreEqual(1, turnEndCount, "자신의 차례가 아닌 경우 턴 종료를 할 수 없습니다.");
            LogAssert.Expect(UnityEngine.LogType.Log, $"[FlowSystem] <color=black><b>{targetIndex}번째 플레이어 턴 종료</b></color>");
            Assert.AreEqual(nextIndex, match.CurrentPlayerIndex, "다음 차례에게 넘어가야합니다.");
            LogAssert.Expect(UnityEngine.LogType.Log, $"[FlowSystem] <color=black><b>{nextIndex}번째 플레이어 턴 시작</b></color>");
        }

        [Test]
        public void RoundEndsCorrectly()
        {
            // Arrange
            bool isTriggered = false;
            this.AddObserver((sender, args) =>
            {
                isTriggered = true;
            }, Global.PerformNotification<RoundEndAction>(), game);

            // Act
            var match = game.GetMatch();
            flowSystem.StartGame();
            flowSystem.EndTurn(match.CurrentPlayerIndex);
            flowSystem.EndTurn(match.CurrentPlayerIndex);
            flowSystem.EndTurn(match.CurrentPlayerIndex);
            flowSystem.EndTurn(match.CurrentPlayerIndex);

            Assert.IsFalse(match.Opened.Contains(false), "모든 Open이 막혀야함");
            Assert.IsTrue(isTriggered, "모든 턴 실행 이후 Round End Action이 수행되어야 합니다.");
        }

        [Test]
        public void GameEndsCorrectly()
        {
            flowSystem.StartGame();
            Assert.DoesNotThrow(() => flowSystem.OnPerformGameEnd(null, null));
        }
    }
}
