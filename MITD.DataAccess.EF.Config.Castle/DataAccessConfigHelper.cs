using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MITD.DataAccess.EF;
using MITD.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.DataAccess.Config
{
    public static class DataAccessConfigHelper
    {
        public static void ConfigureContainer<T1, T2>(IWindsorContainer container, Func<T2> contextDelegate)
            where T1 : IUnitOfWorkScope
            where T2 : DbContext
        {
            var uowfkeyName = Guid.NewGuid().ToString();
            var uowskeyName = Guid.NewGuid().ToString();
            container.Register(
                    Component.For<IUnitOfWorkFactory>().ImplementedBy<EFUnitOfWorkFactory>()
                    .DependsOn(Property.ForKey<Func<DbContext>>().Eq(contextDelegate)).LifeStyle.Singleton.Named(uowfkeyName),
                    Component.For<IUnitOfWorkScope>().ImplementedBy<T1>()
                    .DependsOn(ServiceOverride.ForKey<IUnitOfWorkFactory>().Eq(uowfkeyName)).LifeStyle.Singleton.Named(uowskeyName),
                    Classes.FromAssemblyContaining<T2>().BasedOn<IRepository>().WithService.FromInterface()
                    .Configure(c => c.DependsOn(ServiceOverride.ForKey<IUnitOfWorkScope>().Eq(uowskeyName)).LifestyleTransient())
                    );

        }
    }
}
