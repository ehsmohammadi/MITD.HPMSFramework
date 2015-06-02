using System.Runtime.CompilerServices;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MITD.Data.NH;
using MITD.Domain.Repository;
using System;
using NHibernate;
using MITD.Core;

namespace MITD.DataAccess.Config
{
    public static class DataAccessConfigHelper
    {
        public static void ConfigureContainer<T>(IWindsorContainer container, Func<ISession> contextDelegate,string key)
            where T : class
        {
            var uowfkeyName = key + "_uowf";
            var uowkeyName = key + "_uow";
            container.Register(
                    Component.For<IUnitOfWorkFactory>().ImplementedBy<NHUnitOfWorkFactory>()
                    .DependsOn(Property.ForKey<Func<ISession>>().Eq(contextDelegate)).LifeStyle.Singleton.Named(uowfkeyName),
                    Component.For<IUnitOfWork>().UsingFactoryMethod(c=> c.Resolve<NHUnitOfWorkFactory>(uowfkeyName).Create())
                    .Named(uowkeyName).LifestyleBoundTo<IService>(),
                    Classes.FromAssemblyContaining<T>().BasedOn<IRepository>().WithService.FromInterface()
                    .Configure(c => c.DependsOn(ServiceOverride.ForKey<NHUnitOfWork>().Eq(uowkeyName)).LifestyleBoundToNearest<IService>())
                    );

        }

        public static void ConfigureContainer<T>(IWindsorContainer container, Func<ISession> contextDelegate)
            where T : class
        {
            ConfigureContainer<T>(container, contextDelegate, Guid.NewGuid().ToString());
        }
    }
}
