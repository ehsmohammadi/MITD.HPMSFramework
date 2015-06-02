using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace MITD.Services
{
    public class IocServiceBehavior:IServiceBehavior
    {

        public void AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            var iocProvider = new IocInstanceProvider(serviceDescription.ServiceType);
            foreach(var cdb in serviceHostBase.ChannelDispatchers )
            {
                if (cdb != null)
                {
                    var cd = cdb as ChannelDispatcher;
                    foreach(var ed in cd.Endpoints)
                    {
                        ed.DispatchRuntime.InstanceProvider = iocProvider;
                    }
                }

            }
        }

        public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
        }
    }
}
