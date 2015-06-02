using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;
using MITD.Core;


namespace MITD.Services
{
    public class IocInstanceProvider : IInstanceProvider
    {
        private Type contractType;
        public IocInstanceProvider(Type contractType)
        {
            this.contractType = contractType;
        }
        public object GetInstance(System.ServiceModel.InstanceContext instanceContext, System.ServiceModel.Channels.Message message)
        {
            Object result = null;
            result = ServiceLocator.Current.GetInstance(contractType);
            return result;
        }

        public object GetInstance(System.ServiceModel.InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        public void ReleaseInstance(System.ServiceModel.InstanceContext instanceContext, object instance)
        {

        }
    }
}
