using NUnit.Framework;
using CCGP.Server;

namespace CCGP.Tests.Unit
{
    [TestFixture]
    public class NotificationSystemTests
    {
        [SetUp]
        public void SetUp()
        {
            ServerNotificationCenter.ResetInstacne();
        }

        [Test]
        public void AddObserver_ShoudlAddHandlerNotification()
        {
            var notificationSystem = ServerNotificationCenter.Instance;
            bool handlerCalled = false;

            void TestHandler(object sender, object e)
            {
                handlerCalled = true;
            }

            notificationSystem.AddObserver(TestHandler, "TestNotification");
            notificationSystem.PostNotification("TestNotification");

            Assert.IsTrue(handlerCalled);
        }

        [Test]
        public void AddObserver_ShouldNotAllowDuplicateHandlers()
        {
            var notificationSystem = ServerNotificationCenter.Instance;
            int handlerCallCount = 0;

            void TestHandler(object sender, object e)
            {
                handlerCallCount++;
            }

            notificationSystem.AddObserver(TestHandler, "TestNotification");
            notificationSystem.AddObserver(TestHandler, "TestNotification");

            notificationSystem.PostNotification("TestNotification");

            Assert.AreEqual(1, handlerCallCount);
        }

        [Test]
        public void RemoveObserver_ShouldRemoveHandler()
        {
            var notificationSystem = ServerNotificationCenter.Instance;
            bool handlerCalled = false;

            void TestHandler(object sender, object e)
            {
                handlerCalled = true;
            }

            notificationSystem.AddObserver(TestHandler, "TestNotification");
            notificationSystem.RemoveObserver(TestHandler, "TestNotification");

            notificationSystem.PostNotification("TestNotification");

            Assert.IsFalse(handlerCalled);
        }

        [Test]
        public void PostNotification_ShouldCallRegisteredHandler()
        {
            var notificationSystem = ServerNotificationCenter.Instance;
            bool handlerCalled = false;

            void TestHandler(object sender, object e)
            {
                handlerCalled = true;
            }

            notificationSystem.AddObserver(TestHandler, "TestNotification");
            notificationSystem.PostNotification("TestNotification");

            Assert.IsTrue(handlerCalled);
        }

        [Test]
        public void PostNotification_ShouldCallHandlerForSpecificSender()
        {
            var notificationSystem = ServerNotificationCenter.Instance;
            bool handlerCalled = false;
            var sender = new object();

            void TestHandler(object sender, object e)
            {
                handlerCalled = true;
            }

            notificationSystem.AddObserver(TestHandler, "TestNotification", sender);
            notificationSystem.PostNotification("TestNotification", sender);

            Assert.IsTrue(handlerCalled);
        }

        [Test]
        public void Clean_ShouldRemoveEmptyNotifications()
        {
            var notificationSystem = ServerNotificationCenter.Instance;
            bool handlerCalled = false;

            void TestHandler(object sender, object e)
            {
                handlerCalled = true;
            }

            notificationSystem.AddObserver(TestHandler, "TestNotification");
            notificationSystem.RemoveObserver(TestHandler, "TestNotification");

            notificationSystem.Clean();

            notificationSystem.PostNotification("TestNotification");

            Assert.IsFalse(handlerCalled);
        }
    }
}