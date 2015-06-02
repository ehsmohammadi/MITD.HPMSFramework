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
    public class RuleFunctionRepository : NHRepository, IRuleFunctionRepository
    {
        private NHRepository<RuleFunctionBase> rep;

        private void init()
        {
            rep = new NHRepository<RuleFunctionBase>(unitOfWork);
        }
        public RuleFunctionRepository(NHUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            init();
        }

        public RuleFunctionRepository(IUnitOfWorkScope unitOfWorkScope)
            : base(unitOfWorkScope)
        {
            init();
        }

        public RuleFunctionId GetNextId()
        {
            return new RuleFunctionId(session.CreateSQLQuery("select next value for [dbo].[RuleFunctionSeq]").UniqueResult<long>());
        }


        public void Add(RuleFunctionBase rule)
        {
            rep.Add(rule);
        }


        public IList<RuleFunctionBase> Find(System.Linq.Expressions.Expression<Func<RuleFunctionBase, bool>> predicate)
        {
            return rep.Find(predicate);
        }


        public RuleFunctionBase GetById(RuleFunctionId ruleFunctionId)
        {
            return rep.FindByKey(ruleFunctionId);
        }

        public void Delete(RuleFunctionBase ruleFunction)
        {
            rep.Delete(ruleFunction);
        }
    }
}
