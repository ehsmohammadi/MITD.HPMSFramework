using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;

namespace MITD.Core.Config
{
    public class EventAggregatorExtension: UnityContainerExtension
    {
        protected override void Initialize()
        {
            Context.Strategies.AddNew<EventSubscriptionStrategy>(UnityBuildStage.Initialization);
        }
    }
}
