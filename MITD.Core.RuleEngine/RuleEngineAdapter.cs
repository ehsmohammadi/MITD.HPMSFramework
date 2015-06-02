using MITD.Core.RuleEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Core.RuleEngine
{
    public class RuleEngineAdapter : MarshalByRefObject, IDisposable
    {
        RuleEngineService service;

        public RuleEngineAdapter(IServiceLocatorProvider locatorProvider)
        {
            ServiceLocator.SetLocatorProvider(() => locatorProvider.GetLocator());
        }

        public RuleCompileResult CompileResult { get { return service.CompileResult; } }

        public void ExecuteRule<T>(string ruleClassName, T input)
        {
            service.ExecuteRule<T>(ruleClassName, input);
        }
        public void ExecuteRule(string ruleClassName)
        {
            service.ExecuteRule(ruleClassName);
        }
        public T GetResult<T>()
        {
            return service.GetResult<T>();
        }
        public void Clear()
        {
            service.Clear();
        }
        public Dictionary<string, RuleBase> SetupForCalculation(string nameSpaceName,
            Dictionary<string, RuleId> rules, IReadOnlyList<RuleFunctionId> ruleFunctions)
        {
            service = ServiceLocator.Current.GetInstance<RuleEngineService>();
            return service.SetupForCalculation(nameSpaceName, rules, ruleFunctions);
        }
        public Dictionary<string, RuleBase> SetupForCalculation(string nameSpaceName, string libraryText, string referencedAssemblies,
            Dictionary<string, RuleBase> rules)
        {
            service = ServiceLocator.Current.GetInstance<RuleEngineService>();
            return service.SetupForCalculation(nameSpaceName, libraryText, referencedAssemblies, rules);
        }

        ~RuleEngineAdapter()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (ServiceLocator.Current != null)
                    ServiceLocator.Current.Release(this);
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
