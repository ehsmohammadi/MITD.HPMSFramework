using System;
using System.ServiceModel;

namespace MITD.Services
{
    public interface  IFaultExceptionAdapter
    {
        Exception ConvertToException(FaultException fault);
        FaultException ConvertToFault(Exception exp);
    }
}
