using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Microsoft.Practices.Unity;
using MITD.DataAccess.EF;
using MITD.TestLayer.DataAccess.EF;
using System.Data.Entity;
using Microsoft.Practices.ServiceLocation;

namespace MITD.Domain.Repository.Web.Test
{
    public class Global : System.Web.HttpApplication
    {

        void Application_Start(object sender, EventArgs e)
        {
            var container = new UnityContainer();
            container.RegisterType(typeof(IRepository<>),typeof(EFRepository<>), new InjectionConstructor(
                new ResolvedParameter<IUnitOfWorkScope>()));
            container.RegisterType<IUnitOfWorkScope, PerHttpContextUnitOfWorkScope>(
                new ContainerControlledLifetimeManager(), new InjectionConstructor(
                    new ResolvedParameter<IUnitOfWorkFactory>()));
            container.RegisterType<IUnitOfWorkFactory, EFUnitOfWorkFactory>(
                new InjectionConstructor(new InjectionParameter<Func<DbContext>>(() =>
                {
                    return new FrameWorkDBEntities();
                })));

            var locator = new UnityServiceLocator(container);
            ServiceLocator.SetLocatorProvider(()=>locator);
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started

        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

        }

    }
}
