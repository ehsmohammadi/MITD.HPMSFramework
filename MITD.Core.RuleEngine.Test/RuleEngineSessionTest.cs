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
	public class RuleEngineSessionTest
	{
		static class IdService 
		{
			public static long GetNextId(ISession session, string seq)
			{
				return session.CreateSQLQuery(string.Format("select next value for [dbo].[{0}]",seq)).UniqueResult<long>();
			}
		}






		[TestMethod]
		public void RuleFunctionMappingTest()
		{
			RuleFunction e; 
			using (var session = RuleEngineSession.GetSession())
				using (var tr = session.BeginTransaction())
				{
					var id = new RuleFunctionId(IdService.GetNextId(session, "RuleFunctionSeq"));
					e = new RuleFunction(id, Guid.NewGuid().ToString(),"salam");
					session.Save(e);
					tr.Commit();
				}
            RuleFunction e2;
            using (var session = RuleEngineSession.GetSession())
			using (var tr = session.BeginTransaction())
			{
				e2 = session.Get<RuleFunction>(e.Id);
				e2.Update(new RuleFunctionId(IdService.GetNextId(session, "RuleFunctionSeq")), e.Name, "Alayk");
				tr.Commit();
			}
			RuleFunction e3;
			using (var session = RuleEngineSession.GetSession())
			using (var tr = session.BeginTransaction())
			{
				e3 = session.Get<RuleFunction>(e.Id);
                Assert.AreEqual(1, e3.RuleFunctionsTrail.Count());
                Assert.AreEqual(e2.Name, e3.Name);
                Assert.AreEqual(e.Name, e3.RuleFunctionsTrail.Values.First().Name);
                tr.Commit();
			}

		}

		[TestMethod]
		public void RuleMappingTest()
		{
			Rule e;
			using (var session = RuleEngineSession.GetSession())
			using (var tr = session.BeginTransaction())
			{
				var id = new RuleId(IdService.GetNextId(session, "RuleSeq"));
				e = new Rule(id, Guid.NewGuid().ToString(), "salam",RuleType.PerCalculation,1);
				session.Save(e);
				tr.Commit();
			}
            Rule e2;
            using (var session = RuleEngineSession.GetSession())
			using (var tr = session.BeginTransaction())
			{
				e2 = session.Get<Rule>(e.Id);
				e2.Update(new RuleId(IdService.GetNextId(session, "RuleSeq")), e.Name, "Alayk", RuleType.PreCalculation,1);
				tr.Commit();
			}
			Rule e3;
			using (var session = RuleEngineSession.GetSession())
			using (var tr = session.BeginTransaction())
			{
				e3 = session.Get<Rule>(e.Id);
                Assert.AreEqual(1, e3.RulesTrail.Count());
                Assert.AreEqual(e2.Name, e3.Name);
                Assert.AreEqual(e.Name, e3.RulesTrail.Values.First().Name);
                tr.Commit();
			}
		}

		[TestMethod]
		public void RuleCalculationTest()
		{
			var ruleList = new Dictionary<string, RuleId>();
			using (var uow = new NHUnitOfWork(RuleEngineSession.GetSession()))
			{
				var ruleRep = new RuleRepository(uow);
				var rule = new RuleBase(new RuleId(ruleRep.GetNextId()),Guid.NewGuid().ToString(),
@"
	RuleExecutionUtil.Res = data.ToString();                            
", RuleType.PerCalculation,1);
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
				var svc = new RuleEngineService(reConfigRep, ruleRep, rfRep);
				svc.SetupForCalculation("MITD.Core.RuleEngine", ruleList, null);
				svc.ExecuteRule(ruleList.First().Key, 1);
				var res = svc.GetResult<string>();
			}
		}
		
	}
}
