using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Core
{
    public class DelegateHandler<T> : IEventHandler<T>
    {
        private Action<T> handleAction;
        
        public DelegateHandler(Action<T> handleAction)
        {
            this.handleAction = handleAction;
        }

        public void Handle(T eventData)
        {
            handleAction(eventData);
        }
    }
}
