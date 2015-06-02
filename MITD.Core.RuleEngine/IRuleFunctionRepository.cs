using MITD.Core.RuleEngine.Model;
using MITD.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MITD.Core.RuleEngine
{
    public interface IRuleFunctionRepository : IRepository
    {
        RuleFunctionId GetNextId();
        void Add(RuleFunctionBase ruleFunction);
        IList<RuleFunctionBase> Find(System.Linq.Expressions.Expression<Func<RuleFunctionBase, bool>> predicate);

        RuleFunctionBase GetById(RuleFunctionId ruleFunctionId);
        void Delete(RuleFunctionBase ruleFunction);
    }
}
