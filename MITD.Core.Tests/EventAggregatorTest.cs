using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.Unity;
using MITD.Core;
using Moq;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;
using MITD.Core.Config;

namespace MITD.Core.Tests
{
    [TestClass]
    public class EventAggregatorTest
    {
        [TestMethod]
        public void EventHandlerShouldBeSubscribedOnResolution()
        {
            var container = new UnityContainer();
            var mockPublisher = new Mock<IEventPublisher>();
            var mockHandler = new Mock<IEventHandler<int>>();
            container.AddNewExtension<EventAggregatorExtension>();
            container.RegisterInstance<IEventPublisher>(mockPublisher.Object);
            container.RegisterInstance(typeof(IEventHandler<int>), mockHandler.Object);

            mockPublisher.Setup(p => p.RegisterHandlers(mockHandler.Object)).Returns(new List<EventHandlerOptions>());

            container.BuildUp(mockHandler.Object);

            mockPublisher.VerifyAll();
        }

        [TestMethod]
        public void EventHandlerShouldHandleEventWhenPublished()
        {
            var container = new UnityContainer();
            var mockHandler = new Mock<IEventHandler<int>>();
            container.AddNewExtension<EventAggregatorExtension>();
            container.RegisterType<IEventPublisher, EventPublisher>(new ContainerControlledLifetimeManager());
            container.RegisterInstance(typeof(IEventHandler<int>), mockHandler.Object);

            var publisher = container.Resolve<IEventPublisher>();
            container.BuildUp(mockHandler.Object);
            publisher.Publish<int>(1);

            mockHandler.Verify(h => h.Handle(1));
        }

        [TestMethod]
        public void UnRegisterHandlersTest()
        {
            var container = new UnityContainer();
            var mockIntHandler = new Mock<IEventHandler<int>>();
            var mockStringHandler = mockIntHandler.As<IEventHandler<string>>();
            container.AddNewExtension<EventAggregatorExtension>();
            container.RegisterType<IEventPublisher, EventPublisher>(new ContainerControlledLifetimeManager());
            container.RegisterInstance(typeof(IEventHandler<int>), mockStringHandler.Object);
            container.RegisterInstance(typeof(IEventHandler<string>), mockStringHandler.Object);

            var publisher = container.Resolve<IEventPublisher>();

            container.BuildUp(mockStringHandler.Object);
            publisher.UnregisterHandlers(mockIntHandler.Object);

            publisher.Publish<int>(1);
            publisher.Publish<string>("1");

            mockIntHandler.Verify(h => h.Handle(1), Times.AtMost(0));
            mockStringHandler.Verify(h => h.Handle("1"), Times.AtMost(0));
        }

        [TestMethod]
        public void UnRegisterHandlerTest()
        {
            var container = new UnityContainer();
            var mockIntHandler = new Mock<IEventHandler<int>>();
            var mockStringHandler = mockIntHandler.As<IEventHandler<string>>();
            container.AddNewExtension<EventAggregatorExtension>();
            container.RegisterType<IEventPublisher, EventPublisher>(new ContainerControlledLifetimeManager());
            container.RegisterInstance(typeof(IEventHandler<int>), mockStringHandler.Object);
            container.RegisterInstance(typeof(IEventHandler<string>), mockStringHandler.Object);

            var publisher = container.Resolve<IEventPublisher>();

            container.BuildUp(mockStringHandler.Object);
            publisher.UnregisterHandler<string>(mockStringHandler.Object);

            publisher.Publish<string>("1");
            publisher.Publish<int>(1);

            mockStringHandler.Verify(h => h.Handle("1"), Times.AtMost(0));
            mockIntHandler.Verify(h => h.Handle(1));
        }

        class TestHandler : IEventHandler<int>
        {
            public void Handle(int eventData)
            {
            }
        }
        
        [TestMethod]
        public void RegisterWithThreadHandle()
        {
            
            var publisher = new EventPublisher();
            
            var sub = new DelegateHandler<int>(i => i = 1);
            publisher.RegisterHandler(sub);


        }
    }
}
