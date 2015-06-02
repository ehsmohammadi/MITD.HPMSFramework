using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MITD.Core.RuleEngine.NH;
using MITD.Domain.Repository;
using NHibernate.Linq;
using MITD.Core.RuleEngine.Model;
using System.Linq;
using System.Collections.Generic;
using MITD.Data.NH;
using NHibernate;

namespace MITD.Core.RuleEngine.Test
{
	[TestClass]
	public class UnitTest1
	{
        class IdService 
        {
            private ISession session;
            public IdService(ISession session)
            {
                this.session = session;
            }
            public RuleFunctionId GetNextId()
            {
                return new  RuleFunctionId(session.CreateSQLQuery("select next value for [dbo].[RuleFunctionSeq]").UniqueResult<long>());
            }
        }

		[TestMethod]
		public void TestMethod1()
		{
            RuleFunction e; 
            using (var session = RuleEngineSession.GetSession())
				using (var tr = session.BeginTransaction())
				{
					var id = new IdService(session).GetNextId();
                    e = new RuleFunction(id, Guid.NewGuid().ToString(),"salam");
					session.Save(e);
					tr.Commit();
				}
            using (var session = RuleEngineSession.GetSession())
            using (var tr = session.BeginTransaction())
            {
                var e2 = session.Get<RuleFunction>(e.Id);
                e2.Update(new IdService(session).GetNextId(), e.Name, "Alayk" );
                tr.Commit();
            }
            using (var session = RuleEngineSession.GetSession())
            using (var tr = session.BeginTransaction())
            {
                var e2 = session.Get<RuleFunction>(e.Id);
                tr.Commit();
            }
        }

		[TestMethod]
		public void TestMethod2()
		{
			var ruleList = new Dictionary<string, RuleId>();
			using (var uow = new NHUnitOfWork(RuleEngineSession.GetSession()))
			{
				var ruleRep = new RuleRepository(uow);
				var rule = new Rule(new RuleId(ruleRep.GetNextId()),@"
	RuleExecutionUtil.Res = data.ToString();                            
", RuleType.PerCalculation);
				ruleList.Add("rule"+rule,rule.Id);
				ruleRep.Add(rule);
				uow.Commit();
			}


		    var uows = new UnitOfWorkScope(new NHUnitOfWorkFactory(RuleEngineSession.GetSession));
			using (var uow = uows.CurrentUnitOfWork as NHUnitOfWork)
			{
                var ruleRep = new RuleRepository(uow);
                var reConfigRep = new REConfigeRepository(uow);
                var rfRep = new RuleFunctionRepository(uow);
                var svc = new RuleEngineService(reConfigRep, ruleRep, rfRep, uows);
                svc.SetupForCalculation("MITD.Core.RuleEngine", ruleList, null);
				svc.ExecuteRule(ruleList.First().Key, 1);
				var res = svc.GetResult<string>();
			}
		}
		
	}
}
