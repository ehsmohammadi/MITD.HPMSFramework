using MITD.Core.RuleEngine.Model;
using MITD.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MITD.Core.RuleEngine
{
    public interface IRuleRepository : IRepository
    {
        void Add(RuleBase rule);
        long GetNextId();

        IList<RuleBase> Find(Expression<Func<RuleBase,bool>> predicate, IListFetchStrategy<RuleBase> fetchStrategy);

        IList<RuleBase> Find(Expression<Func<RuleBase, bool>> predicate);
        RuleBase GetById(RuleId ruleId);
        void Delete(RuleBase rule);

        IList<RuleTrail> FindRuleTrails(System.Linq.Expressions.Expression<Func<RuleTrail, bool>> predicate,
                                       IListFetchStrategy<RuleTrail> fetchStrategy);
        IList<RuleTrail> FindRuleTrails(System.Linq.Expressions.Expression<Func<RuleTrail, bool>> predicate);

    }
}
