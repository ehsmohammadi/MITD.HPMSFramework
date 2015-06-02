using Castle.Windsor;
using Castle.MicroKernel.Registration;
using MITD.Core.Config;
using MITD.DataAccess.Config;
using MITD.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Core.RuleEngine.NH
{
    [Serializable]
    public class LocatorProvider : IServiceLocatorProvider
    {
        private string connectionName;
        public LocatorProvider(string connectionName)
        {
            this.connectionName = connectionName;
            RuleEngineSession.sessionName = connectionName;
        }
        public virtual IServiceLocator GetLocator()
        {
            var container = new WindsorContainer();
            DataAccessConfigHelper.ConfigureContainer<RuleBaseMap>(container,
                () => {
                    return RuleEngineSession.GetSession();
                });
            container.Register(Component.For<RuleEngineService>().LifestyleTransient());
            return new WindsorServiceLocator(container);
        }
    }
}
