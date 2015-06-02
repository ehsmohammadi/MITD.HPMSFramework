

using System.Collections.Generic;
using MITD.Core.RuleEngine.Model;
using MITD.Domain.Repository;

namespace MITD.Core.RuleEngine
{
    public interface IRuleService : IService
    {
        #region Rule Functions

        void DeleteFunction(RuleFunctionId ruleFunctionId);
        RuleFunctionBase AddRuleFunction(string name, string content);
        RuleFunctionBase UpdateRuleFunction(RuleFunctionId ruleFunctionId, string name, string content);
        IList<RuleFunctionBase> FindWithPagingBy(IList<RuleFunctionId> ruleFunctions);
        RuleFunctionBase GetById(RuleFunctionId ruleFunctionId); 

        #endregion


        #region Rules

        void DeleteRule(RuleId ruleId);
        RuleBase AddRule(string name, string ruleText, RuleType ruleType,int order);
        RuleBase UpdateRule(RuleId ruleId, string name, string ruleText, RuleType ruleType,int order);
        IList<RuleBase> FindWithPagingBy(IList<RuleId> rules);
        RuleBase GetById(RuleId ruleId);
        void CompileRule(IReadOnlyList<RuleFunctionId> ruleFunctions, string ruleContent);
        IList<RuleTrail> GetRuleTrails(RuleId ruleId, ListFetchStrategy<RuleTrail> fs);
        #endregion
    }
}
