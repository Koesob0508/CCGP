using NUnit.Framework;
using Moq;
using CCGP.Server;
using CCGP.Shared;
using System.Collections.Generic;
using CCGP.AspectContainer;
using UnityEngine.TestTools;

namespace CCGP.Tests
{
    [TestFixture]
    public class FlowSystemTests
    {
        private Entity game;
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
            Assert.AreEqual(4, match.Players.Count, "게임에는 4명의 플레이어가 참가해야 합니다.");

            // FirstPlayerIndex는 0~3 중 랜덤한 값이어야 한다.
            Assert.GreaterOrEqual(match.FirstPlayerIndex, 0, "FirstPlayerIndex는 0 이상이어야 합니다.");
            Assert.Less(match.FirstPlayerIndex, 4, "FirstPlayerIndex는 4 미만이어야 합니다.");
            Assert.AreEqual(match.FirstPlayerIndex, match.CurrentPlayerIndex, "게임 시작시 FirstPlayerIndex와 CurrentPlayerIndex는 같아야한다.");

            // ActionSystem과 연결됐는지 테스트
            Assert.IsTrue(isPerform, "GameStartAction이 ActionSystem에 의해 Perform 돼야함");
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

            // Start 이후에 RoundStart가 관측되는지 확인
            Assert.IsTrue(isRoundStartTriggered, "RoundStartAction이 ActionSystem에 의해 Perform 되어야 합니다.");
        }

        [Test]
        public void TurnStartsCorrectly()
        {
            flowSystem.StartGame();
            Assert.DoesNotThrow(() => flowSystem.OnPerformTurnStart(null, null));
        }

        [Test]
        public void TurnEndsCorrectly()
        {
            flowSystem.StartGame();
            Assert.DoesNotThrow(() => flowSystem.OnPerformTurnEnd(null, null));
        }

        [Test]
        public void RoundEndsCorrectly()
        {
            flowSystem.StartGame();
            Assert.DoesNotThrow(() => flowSystem.OnPerformRoundEnd(null, null));
        }

        [Test]
        public void GameEndsCorrectly()
        {
            flowSystem.StartGame();
            Assert.DoesNotThrow(() => flowSystem.OnPerformGameEnd(null, null));
        }
    }
}
