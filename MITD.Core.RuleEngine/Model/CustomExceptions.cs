using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MITD.Core.RuleEngine.Exceptions
{
    public class RuleEngineOperationExeption :Exception
    {
        public RuleEngineOperationExeption(string message)
            : base(message)
        { }
        public RuleEngineOperationExeption()
            : base("شما قادر به انجام عملیات مورد نظر نیستید")
        {
        }
    }

    public class RuleEngineCompileExeption : RuleEngineOperationExeption
    {
        public RuleEngineCompileExeption(string message)
            : base(message)
           
        {
            
        }
        


    }
    



}
