using Castle.Core;
using Castle.MicroKernel.Facilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITD.Core;
using Castle.MicroKernel.Registration;

namespace MITD.Core.Config
{
    public class EventAggregatorFacility : AbstractFacility
    {
        protected override void Init()
        {
            Kernel.Register(
                Component.For<IEventPublisher>().ImplementedBy<EventPublisher>().LifeStyle.Singleton); 
            Kernel.ComponentModelBuilder.AddContributor(new EventBrokerContributor()); 
        }
    }
}
