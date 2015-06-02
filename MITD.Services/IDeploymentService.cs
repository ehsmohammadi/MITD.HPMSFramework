using System.Collections.Generic;
using System.ServiceModel;
using MITD.Domain.Model;

namespace MITD.Services
{
    [ServiceContract]
    public interface IDeploymentService
    {
        #region Xap File Managment

        [OperationContract]
        [FaultContract(typeof(ErrorDetail))]
        string GetXapFileVersion(string fileName);

        [OperationContract]
        [FaultContract(typeof(ErrorDetail))]
        string[] GetXapFileVersion(string[] fileNames);

        #endregion
    }
}