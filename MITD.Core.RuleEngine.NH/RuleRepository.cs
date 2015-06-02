using MITD.Core.RuleEngine.Model;
using MITD.Data.NH;
using MITD.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Core.RuleEngine.NH
{
    public class RuleRepository : NHRepository, IRuleRepository
    {
        private NHRepository<RuleBase> rep;
        
        private void init()
        {
            rep = new NHRepository<RuleBase>(unitOfWork);
        }
        public RuleRepository(NHUnitOfWork unitOfWork):base(unitOfWork)
        {
            init();
        }

        public RuleRepository(IUnitOfWorkScope unitOfWorkScope)
            : base(unitOfWorkScope)
        {
            init();
        }

        public long GetNextId()
        {
            return session.CreateSQLQuery("select next value for [dbo].[RuleSeq]").UniqueResult<long>();
        }


        public void Add(RuleBase rule)
        {
            rep.Add(rule);
        }


        public IList<RuleBase> Find(System.Linq.Expressions.Expression<Func<RuleBase, bool>> predicate, IListFetchStrategy<RuleBase> fetchStrategy)
        {
            return rep.Find(predicate, fetchStrategy);
        }

        public IList<RuleBase> Find(System.Linq.Expressions.Expression<Func<RuleBase, bool>> predicate)
        {
            return rep.Find(predicate);
        }

        public IList<RuleTrail> FindRuleTrails(System.Linq.Expressions.Expression<Func<RuleTrail, bool>> predicate, IListFetchStrategy<RuleTrail> fetchStrategy)
        {
            //return rep.Find<RuleTrail>(predicate, fetchStrategy);
            return FindRuleTrails(predicate);
        }

        public IList<RuleTrail> FindRuleTrails(System.Linq.Expressions.Expression<Func<RuleTrail, bool>> predicate)
        {
            return rep.Find(predicate);
        }

        public RuleBase GetById(RuleId ruleId)
        {
            return rep.FindByKey(ruleId);
        }

        public void Delete(RuleBase rule)
        {
            rep.Delete(rule);
        }
    }
}
