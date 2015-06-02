using System;
using System.ComponentModel;

namespace MITD.Presentation
{
    public interface IDeploymentManagement
    {
        void AddModule(Type type, Action<Object> action, Action<Exception> onError);
    }
}
