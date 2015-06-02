using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MITD.Core.RuleEngine
{
    public interface IRule
    {
        void Execute();
    }
    
    public interface IRule<T>
    {
        void Execute(T input);
    }
}
