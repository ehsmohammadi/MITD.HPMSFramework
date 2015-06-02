using Castle.Core;
using MITD.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Core.Config
{
    public class RegisterWithEventPublisher : ICommissionConcern
    {
        private readonly IEventPublisher publisher;

        public RegisterWithEventPublisher(IEventPublisher publisher)
        {
            this.publisher = publisher;
        }

        public void Apply(ComponentModel model, object component)
        {
            publisher.RegisterHandlers(component);
        }
    } 

}
