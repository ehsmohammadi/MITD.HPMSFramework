using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ObjectBuilder2;
using MITD.Core;

namespace MITD.Core.Config
{
    public class EventSubscriptionStrategy : BuilderStrategy
    {
        public override void PreBuildUp(IBuilderContext context)
        {
            if (context.Existing != null)
            {
                if (context.Existing.GetType().GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandler<>)))
                {
                    object newCreated = context.NewBuildUp(new NamedTypeBuildKey(typeof(IEventPublisher)));
                    if (newCreated != null)
                        if (newCreated is IEventPublisher)
                        {
                            IEventPublisher eventPublisher = (IEventPublisher)newCreated;
                            eventPublisher.RegisterHandlers(context.Existing);
                        }
                }
            }
            base.PreBuildUp(context);
        }
    }
}
