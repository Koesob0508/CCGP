using NUnit.Framework;
using System;
using CCGP.Server;

namespace CCGP.Tests.Unit
{
    public class ServerNotificationCenterTests
    {
        [SetUp]
        public void Setup()
        {
            // 매 테스트마다 싱글톤 인스턴스를 리셋합니다.
            ServerNotificationCenter.ResetInstacne();
        }

        [Test]
        public void AddObserverAndPostNotification_CallsHandler()
        {
            // Arrange
            bool wasCalled = false;
            object actualSender = null;
            object actualEventData = null;
            Action<object, object> handler = (sender, e) =>
            {
                wasCalled = true;
                actualSender = sender;
                actualEventData = e;
            };

            string notificationName = "TestNotification";
            object testSender = new object();
            object testData = "TestData";

            // Act
            ServerNotificationCenter.Instance.AddObserver(handler, notificationName);
            ServerNotificationCenter.Instance.PostNotification(notificationName, testSender, testData);

            // Assert
            Assert.IsTrue(wasCalled, "Handler should be called after posting notification.");
            Assert.AreEqual(testSender, actualSender, "Sender should match the posted sender.");
            Assert.AreEqual(testData, actualEventData, "Event data should match the posted event data.");
        }

        [Test]
        public void RemoveObserver_HandlerNotCalledAfterRemoval()
        {
            // Arrange
            bool wasCalled = false;
            Action<object, object> handler = (sender, e) => { wasCalled = true; };
            string notificationName = "TestNotification";

            // Act
            ServerNotificationCenter.Instance.AddObserver(handler, notificationName);
            ServerNotificationCenter.Instance.RemoveObserver(handler, notificationName);
            ServerNotificationCenter.Instance.PostNotification(notificationName, null, null);

            // Assert
            Assert.IsFalse(wasCalled, "Handler should not be called after removal.");
        }

        [Test]
        public void MultipleObservers_AllCalledAndRemovalWorksCorrectly()
        {
            // Arrange
            int callCount = 0;
            Action<object, object> handler1 = (sender, e) => { callCount++; };
            Action<object, object> handler2 = (sender, e) => { callCount++; };
            string notificationName = "TestNotification";

            // Act & Assert
            // 등록 후 첫 알림에서는 두 핸들러 모두 호출되어야 함.
            ServerNotificationCenter.Instance.AddObserver(handler1, notificationName);
            ServerNotificationCenter.Instance.AddObserver(handler2, notificationName);
            ServerNotificationCenter.Instance.PostNotification(notificationName, null, null);
            Assert.AreEqual(2, callCount, "Both handlers should be called on first notification.");

            // handler1 제거 후 두 번째 알림에서는 handler2만 호출되어야 함.
            ServerNotificationCenter.Instance.RemoveObserver(handler1, notificationName);
            ServerNotificationCenter.Instance.PostNotification(notificationName, null, null);
            Assert.AreEqual(3, callCount, "Only remaining handler should be called after removal.");
        }

        [Test]
        public void ObserverWithSpecificSender_OnlyCalledWhenSenderMatches()
        {
            // Arrange
            bool wasCalled = false;
            Action<object, object> handler = (sender, e) => { wasCalled = true; };
            string notificationName = "TestNotification";
            object specificSender = new object();

            // Act
            // 특정 sender로 Observer 등록
            ServerNotificationCenter.Instance.AddObserver(handler, notificationName, specificSender);

            // 다른 sender로 PostNotification 하면 호출되지 않아야 함.
            ServerNotificationCenter.Instance.PostNotification(notificationName, new object(), null);
            Assert.IsFalse(wasCalled, "Handler should not be called if sender does not match.");

            // 올바른 sender로 알림을 Post하면 호출되어야 함.
            ServerNotificationCenter.Instance.PostNotification(notificationName, specificSender, null);
            Assert.IsTrue(wasCalled, "Handler should be called if sender matches.");
        }

        [Test]
        public void RemoveObserverDuringNotification_HandlerNotCalledInLaterNotifications()
        {
            // Arrange
            int callCount = 0;
            Action<object, object> handler = null;
            string notificationName = "TestNotification";

            // Observer가 호출되면, 자기 자신을 제거합니다.
            handler = (sender, e) =>
            {
                callCount++;
                // 재진입 상황: 알림 발송 도중 Observer 제거
                ServerNotificationCenter.Instance.RemoveObserver(handler, notificationName);
            };

            // Observer 등록 (sender 인자는 null 사용, 기본 구독자 등록)
            ServerNotificationCenter.Instance.AddObserver(handler, notificationName);

            // Act
            // 첫 번째 알림에서는 핸들러가 호출되어, 내부에서 제거가 이루어짐
            ServerNotificationCenter.Instance.PostNotification(notificationName, null, "FirstCall");
            // 두 번째 알림에서는 제거된 Observer가 호출되지 않아야 함
            ServerNotificationCenter.Instance.PostNotification(notificationName, null, "SecondCall");

            // Assert
            Assert.AreEqual(1, callCount, "Handler should be called only once if removal during notification is effective.");
        }
    }
}
