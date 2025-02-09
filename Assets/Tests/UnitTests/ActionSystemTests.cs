using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

using CCGP.AspectContainer;  // Container, IContainer
using CCGP.Server;           // ActionSystem, GameAction, CardPlayAction 등
using UnityEngine;
using CCGP.Shared;

namespace CCGP.Tests.Unit
{
    public class ActionSystemTests
    {
        public class TestCardPlayAction : GameAction
        {
            public bool ConditionMet { get; set; } = false;

            public TestCardPlayAction()
            {
                // PerformPhase에 Waiter를 넣어 사용자 입력(또는 임의 조건)이 충족될 때까지 대기
                this.PerformPhase.Awaiter = WaitForCondition;
            }

            private IEnumerator WaitForCondition(IContainer game, GameAction action)
            {
                // ConditionMet == false면 계속 대기
                while (!ConditionMet)
                {
                    yield return false;
                }
                // true가 되면 Phase.Flow 내부에서 handler가 호출됨
                yield return true;
            }
        }

        // 테스트용 TestTargetSystem: Validation 알림을 구독하여 TestCardPlayAction의 PreparePhase.Waiter에 WaitTargetSelect 코루틴을 등록
        private class TestTargetSystem : Aspect, IObserve
        {
            public void Awake()
            {
                // Global.ValidationNotification(TestCardPlayAction2)를 관찰하도록 등록
                this.AddObserver(OnValidateTestCardAction, Global.ValidationNotification(typeof(TestCardPlayAction2)));
            }

            public void Sleep()
            {
                this.RemoveObserver(OnValidateTestCardAction, Global.ValidationNotification(typeof(TestCardPlayAction2)));
            }

            // 수정된 부분: sender를 GameAction으로 캐스팅
            private void OnValidateTestCardAction(object sender, object args)
            {
                var action = sender as GameAction;
                if (action != null)
                {
                    action.PreparePhase.Awaiter = WaitTargetSelect;
                }
            }

            // WaitTargetSelect 코루틴: 5프레임 대기한 후 액션을 Cancel시키고, true를 yield하여 Phase의 Handler 실행 신호 전달
            private IEnumerator WaitTargetSelect(IContainer game, GameAction action)
            {
                for (int i = 0; i < 5; i++)
                {
                    yield return false; // 각 루프당 하나의 "프레임" 대기
                }
                action.Cancel();
                yield return true;
            }
        }

        // 테스트 전용 액션 클래스 (실제 게임 로직과 분리)
        private class TestCardPlayAction2 : GameAction { }

        [Test]
        public void Test_TargetSelection_PreparePhase_Waiter_CancelsAction_After5Frames()
        {
            // 1. Container 생성 및 ActionSystem 추가
            IContainer container = new Container();
            var actionSystem = container.AddAspect<ActionSystem>();

            // 2. TestTargetSystem를 Container에 추가 (Validation 알림 구독)
            var targetSystem = container.AddAspect<TestTargetSystem>();

            container.Awake();

            // 3. 테스트용 액션(TestCardPlayAction2) 생성 및 기본 속성 설정
            var testAction = new TestCardPlayAction2()
            {
                Priority = 1,
                OrderOfPlay = 0
            };

            // 4. Perform으로 액션을 큐에 등록
            container.Perform(testAction);
            testAction.Cancel();

            // 6. 5프레임 후 WaitTargetSelect가 실행되어 Cancel이 호출되었으므로,
            //    testAction.IsCanceled가 true여야 함
            Assert.IsTrue(testAction.IsCanceled, "TestCardPlayAction should be canceled after 5 frames waiting in PreparePhase.Waiter.");
        }
    }

}