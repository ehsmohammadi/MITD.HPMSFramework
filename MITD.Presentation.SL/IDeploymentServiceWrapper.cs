using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace MITD.Presentation
{
    public interface IDeploymentServiceWrapper:IServiceWrapper
    {
        void GetXapVersion(string fileName, Action<string> onAction, Action<Exception> onError);
        void GetXapVersions(string[] fileNames, Action<string[]> onAction, Action<Exception> onError);
    }
}
