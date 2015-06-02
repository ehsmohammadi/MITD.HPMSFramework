using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.ComponentActivator;
using Castle.MicroKernel.ModelBuilder;
using MITD.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MITD.Core.Config
{
    public class EventBrokerContributor : IContributeComponentModelConstruction 
    { 
        public void ProcessModel(IKernel kernel, ComponentModel model) 
        {
            if (!model.Implementation.GetInterfaces()
                .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandler<>))) 
            { return; }
            var broker = kernel.Resolve<IEventPublisher>(); 
            model.Lifecycle.Add(new RegisterWithEventPublisher(broker)); 
            model.Lifecycle.Add(new UnRegisterWithEventPublisher(broker)); 
        } 
    }
}
