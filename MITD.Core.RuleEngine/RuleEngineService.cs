using MITD.Core.RuleEngine.Exceptions;
using MITD.Core.RuleEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MITD.Domain.Repository;
using System.Transactions;

namespace MITD.Core.RuleEngine
{
    public class RuleEngineService : IRuleService
    {
        const string RuleTextTemplateKey = "RuleTextTemplate";
        const string LibraryTextTemplateKey = "LibraryTextTemplate";
        const string ReferencedAssembliesKey = "ReferencedAssemblies";
        private string ruleLibraryText = "";

        private CompileResult compileResult;
        private System.Reflection.Assembly ruleCompiledLibrary;
        private Dictionary<string, RuleContainer> rules = new Dictionary<string, RuleContainer>();
        private object ruleResult = null;
        private IREConfigeRepository recRep;
        private IRuleRepository ruleRep;
        private IRuleFunctionRepository funRep;
        private string nameSpaceName;

        class RuleContainer
        {
            public RuleBase Rule { get; set; }
            public object RuleInstance { get; set; }
        }

        public RuleEngineService(IREConfigeRepository recRep,
            IRuleRepository ruleRep,
            IRuleFunctionRepository funRep)
        {
            this.recRep = recRep;
            this.ruleRep = ruleRep;
            this.funRep = funRep;
        }

        private string compile(IList<RuleEngineConfigurationItem> configurations,
            Dictionary<string, string> ruleList, string functionsText)
        {
            ruleLibraryText = "";
            ruleCompiledLibrary = null;
            ruleResult = null;
            foreach (var item in rules)
            {
                item.Value.RuleInstance = null;
            }
            var Id = new RuleEngineConfigurationItemId(ReferencedAssembliesKey);
            var s = configurations.Single(c => c.Id == Id).Value;
            var referencedAssemblies = s.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            string libraryText = buildLibraryTextToCompile(configurations, ruleList, functionsText);
            compileResult = CodeDomCompilerHelper.Compile(libraryText, referencedAssemblies);
            if (!compileResult.HasError)
            {
                ruleLibraryText = libraryText;
                ruleCompiledLibrary = compileResult.Assembly;
            }
            CompileResult = new RuleCompileResult(compileResult.ErrorMsg, libraryText);
            return libraryText;
        }

        private void compile(string libraryText, string referencedAssemblies)
        {
            ruleLibraryText = "";
            ruleCompiledLibrary = null;
            ruleResult = null;
            var referencedAssemblyList = referencedAssemblies.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            compileResult = CodeDomCompilerHelper.Compile(libraryText, referencedAssemblyList);
            if (!compileResult.HasError)
            {
                ruleLibraryText = libraryText;
                ruleCompiledLibrary = compileResult.Assembly;
            }
            CompileResult = new RuleCompileResult(compileResult.ErrorMsg, libraryText);
        }

        private string buildLibraryTextToCompile(IList<RuleEngineConfigurationItem> configurations, Dictionary<string, string> ruleList, string functionsText)
        {
            var s = configurations.Single(c => c.Id == new RuleEngineConfigurationItemId(LibraryTextTemplateKey)).Value;
            s = s.Replace("<#functions#>", functionsText);
            var rules = new StringBuilder();
            foreach (var rule in ruleList)
            {
                var ruleTemplate = configurations.Single(c => c.Id == new RuleEngineConfigurationItemId(RuleTextTemplateKey)).Value;
                ruleTemplate = ruleTemplate.Replace("<#classname#>", rule.Key);
                ruleTemplate = ruleTemplate.Replace("<#ruletext#>", rule.Value);
                rules.Append(ruleTemplate);
            }
            s = s.Replace("<#rules#>", rules.ToString());
            return s;
        }

        public RuleCompileResult CompileResult { get; private set; }

        public void ExecuteRule<T>(string ruleClassName, T input)
        {
            if (ruleCompiledLibrary == null) return;
            var rule = rules[ruleClassName];
            if (rule.RuleInstance == null)
                rule.RuleInstance = ruleCompiledLibrary.CreateInstance(nameSpaceName + "." + ruleClassName);
            if (rule.RuleInstance is IRule<T>)
            {
                (rule.RuleInstance as IRule<T>).Execute(input);
            }
        }

        public void ExecuteRule(string ruleClassName)
        {
            if (ruleCompiledLibrary == null) return;
            var rule = rules[ruleClassName];
            if (rule.RuleInstance == null)
                rule.RuleInstance = ruleCompiledLibrary.CreateInstance(nameSpaceName + "." + ruleClassName);
            if (rule.RuleInstance is IRule)
            {
                (rule.RuleInstance as IRule).Execute();
            }
        }

        public T GetResult<T>()
        {
            if (ruleCompiledLibrary == null) return default(T);
            if ((ruleResult == null) || !(ruleResult is IRuleResult<T>))
            {
                var ruleResultType = ruleCompiledLibrary.GetTypes().Single(t => typeof(IRuleResult<T>).IsAssignableFrom(t));
                ruleResult = ruleCompiledLibrary.CreateInstance(ruleResultType.FullName);
            }
            return (ruleResult as IRuleResult<T>).GetResult();
        }
        public void Clear()
        {
            if (ruleResult == null) return;
            (ruleResult as IRuleResultCleaner).Clear();
        }

        public IList<Core.RuleEngine.Model.RuleEngineConfigurationItem> GetAllRuleEngineConfigs()
        {
            return recRep.GetAll();
        }

        public Dictionary<string, RuleBase> SetupForCalculation(string nameSpaceName,
            Dictionary<string, RuleId> rules, IReadOnlyList<RuleFunctionId> ruleFunctions)
        {
            this.nameSpaceName = nameSpaceName;
            IList<RuleEngineConfigurationItem> configs = getConfigs();
            string functionsText = getFunctionsText(ruleFunctions);
            Dictionary<string, string> rulesDic = getRulesText(rules);
            compile(configs, rulesDic, functionsText);
            if (compileResult.HasError)
                throw new Exception(compileResult.ErrorMsg);
            var res = this.rules.ToDictionary(x => x.Key, x => x.Value.Rule);
            return res;
        }

        public Dictionary<string, RuleBase> SetupForCalculation(string nameSpaceName, string libraryText, string referencedAssemblies,
            Dictionary<string, RuleBase> rules)
        {
            this.nameSpaceName = nameSpaceName;
            this.rules = rules.ToDictionary(x => x.Key, x => new RuleContainer { Rule = x.Value });
            compile(libraryText, referencedAssemblies);
            return rules;
        }

        private Dictionary<string, string> getRulesText(Dictionary<string, RuleId> rules)
        {
            var res = ruleRep.Find(r => rules.Values.Contains(r.Id), new ListFetchStrategy<RuleBase>(Enums.FetchInUnitOfWorkOption.NoTracking)).OrderBy(r=>r.ExecuteOrder);
            this.rules = rules.Join(res, x => x.Value, y => y.Id, (x, y) => new { x.Key, Rule = new RuleContainer { Rule = y } }).ToDictionary(x => x.Key, x => x.Rule);
            return this.rules.ToDictionary(x => x.Key, x => x.Value.Rule.RuleText);
        }

        private string getFunctionsText(IReadOnlyList<RuleFunctionId> ruleFunctions)
        {
            if (ruleFunctions == null) return "";
            var builder = new StringBuilder();
            var lst = funRep.Find(rf => ruleFunctions.Contains(rf.Id));
            foreach (var item in lst)
            {
                builder.Append(item.FunctionsText);
            }
            return builder.ToString();
        }

        private IList<RuleEngineConfigurationItem> getConfigs()
        {
            return recRep.GetAll();
        }

        public IList<RuleBase> Find(Expression<Func<RuleBase, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        #region Rule Service Member

        public void DeleteFunction(RuleFunctionId ruleFunctionId)
        {
            using (var scope = new TransactionScope())
            {
                var ruleFunction = funRep.GetById(ruleFunctionId);
                funRep.Delete(ruleFunction);
                scope.Complete();
            }
        }

        public RuleFunctionBase AddRuleFunction(string name, string content)
        {
            using (var scope = new TransactionScope())
            {
                var id = funRep.GetNextId();
                var ruleFunction = new RuleFunction(id, name, content);
                funRep.Add(ruleFunction);
                funRep.UnitOfWork.Commit();
                scope.Complete();
                return ruleFunction;
            }
        }

        public RuleFunctionBase UpdateRuleFunction(RuleFunctionId ruleFunctionId, string name, string content)
        {
            using (var scope = new TransactionScope())
            {
                var ruleFunction = funRep.GetById(ruleFunctionId);
                (ruleFunction as RuleFunction).Update(funRep.GetNextId(), name, content);
                scope.Complete();
                return ruleFunction;
            }
        }

        public IList<RuleFunctionBase> FindWithPagingBy(IList<RuleFunctionId> ruleFunctions)
        {

            return funRep.Find(fr => ruleFunctions.Contains(fr.Id)).ToList();
        }

        public RuleFunctionBase GetById(RuleFunctionId ruleFunctionId)
        {
            return funRep.GetById(ruleFunctionId);
        }

        public void DeleteRule(RuleId ruleId)
        {
            using (var scope = new TransactionScope())
            {
                var rule = ruleRep.GetById(ruleId);
                ruleRep.Delete(rule);
                scope.Complete();
            }
        }

        public RuleBase AddRule(string name, string ruleText, RuleType ruleType,int order)
        {
            using (var scope = new TransactionScope())
            {
                var id = ruleRep.GetNextId();
                var rule = new Rule(new RuleId(id), name, ruleText, ruleType, order);
                ruleRep.Add(rule);
                ruleRep.UnitOfWork.Commit();
                scope.Complete();
                return rule;
            }

        }

        public RuleBase UpdateRule(RuleId ruleId, string name, string ruleText, RuleType ruleType,int order)
        {
            using (var scope = new TransactionScope())
            {
                var rule = ruleRep.GetById(ruleId);
                (rule as Rule).Update(new RuleId(ruleRep.GetNextId()), name, ruleText, ruleType,order);
                scope.Complete();
                return rule;
            }
        }

        public IList<RuleBase> FindWithPagingBy(IList<RuleId> rules)
        {

            return ruleRep.Find(fr => rules.Contains(fr.Id)).ToList();
        }

        public RuleBase GetById(RuleId ruleId)
        {
            return ruleRep.GetById(ruleId);
        }
        #endregion


        public void CompileRule(IReadOnlyList<RuleFunctionId> ruleFunctions, string ruleContent)
        {
            IList<RuleEngineConfigurationItem> configs = getConfigs();
            string functionsText = getFunctionsText(ruleFunctions);
            Dictionary<string, string> rulesDic = new Dictionary<string, string> { { "rule01", ruleContent } };
            var s = compile(configs, rulesDic, functionsText);
            if (compileResult.HasError)
            {
                var l = s.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();
                var x = l.IndexOf(l.FirstOrDefault(it=>it.Contains("rule01")))+4;
                var y = compileResult.ErrorMsg;
                int z = -1;
                int k = 0;
                string res = string.Empty;
                while ((z = y.IndexOf("Line (",k)) >= 0)
                {
                    var i = z + 6;
                    var j = y.IndexOf(")", i);
                    var n = y.Substring(i, j - i);
                    res = res + y.Substring(k, i - k) + (Convert.ToInt32(n) - x).ToString();
                    k = j;
                }
                res = res + y.Substring(k);
                throw new RuleEngineCompileExeption(res);
            }
        }

        public IList<RuleTrail> GetRuleTrails(RuleId ruleId, ListFetchStrategy<RuleTrail> fs)
        {
            return ruleRep.FindRuleTrails(f => f.Current.Id.Id == ruleId.Id, fs);
        }
    }
}
