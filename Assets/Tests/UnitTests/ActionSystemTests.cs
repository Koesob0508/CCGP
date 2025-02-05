using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

using CCGP.AspectContainer;  // Container, IContainer
using CCGP.Server;           // ActionSystem, GameAction, CardPlayAction 등
using UnityEngine;

namespace CCGP.Tests.Unit
{
    public class ActionSystemTests
    {
        /// <summary>
        /// 1) 별도의 Waiter(입력 대기) 없이 즉시 완료되는 GameAction 테스트
        /// </summary>
        [Test]
        public void SimpleAction_FinishesQuickly()
        {
            // 1. 컨테이너, 액션 시스템 준비
            var container = new Container();
            var actionSystem = container.AddAspect<ActionSystem>();

            // 2. 간단한 GameAction 생성
            var simpleAction = new GameAction();

            // 3. 액션 등록 (Perform → 내부 큐)
            container.Perform(simpleAction);

            // 4. Update()를 여러 번 호출해 코루틴 한 스텝씩 진행
            for (int i = 0; i < 10; i++)
            {
                actionSystem.Update();
            }

            // 5. 액션이 완료되었는지 확인
            Assert.IsFalse(actionSystem.IsActive, "SimpleAction should have finished");
        }
        public class TestCardPlayAction : GameAction
        {
            public bool ConditionMet { get; set; } = false;

            public TestCardPlayAction()
            {
                // PerformPhase에 Waiter를 넣어 사용자 입력(또는 임의 조건)이 충족될 때까지 대기
                this.PerformPhase.Waiter = WaitForCondition;
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

        [Test]
        public void TestCardPlayAction_WaitsAndThenCompletes()
        {
            // 1. 컨테이너, 액션 시스템 준비
            var container = new Container();
            var actionSystem = container.AddAspect<ActionSystem>();

            // 2. TestCardPlayAction 생성 (perform 단계에서 ConditionMet을 기다림)
            var testAction = new TestCardPlayAction
            {
                Priority = 1,
                OrderOfPlay = 0
            };

            // 3. 큐에 등록
            container.Perform(testAction);

            // 4. 아직 ConditionMet == false이므로 몇 번 Update해도 끝나지 않음
            for (int i = 0; i < 5; i++)
            {
                actionSystem.Update();
            }

            Assert.IsTrue(actionSystem.IsActive, "Action should still be active (waiting).");

            // 5. ConditionMet = true 로 설정 → 다음 Update 이후 액션 완료
            testAction.ConditionMet = true;

            // 6. 다시 Update 여러 번
            for (int i = 0; i < 5; i++)
            {
                actionSystem.Update();
            }

            Assert.IsFalse(actionSystem.IsActive, "Action should be finished after ConditionMet=true.");
            Assert.IsFalse(testAction.IsCanceled, "Action shouldn't be canceled.");
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
                    action.PreparePhase.Waiter = WaitTargetSelect;
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

            // 5. for 루프를 통해 Update()를 여러 번 호출하여 "프레임" 흐름 시뮬레이션
            for (int frame = 0; frame < 10; frame++)
            {
                actionSystem.Update();
            }

            // 6. 5프레임 후 WaitTargetSelect가 실행되어 Cancel이 호출되었으므로,
            //    testAction.IsCanceled가 true여야 함
            Assert.IsTrue(testAction.IsCanceled, "TestCardPlayAction should be canceled after 5 frames waiting in PreparePhase.Waiter.");
            // 또한 ActionSystem의 루트 액션이 해제되었음을 확인 (즉, 처리 완료)
            Assert.IsFalse(actionSystem.IsActive, "ActionSystem should not be active after the action is canceled.");
        }
    }

}