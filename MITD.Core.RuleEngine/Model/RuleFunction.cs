using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Core.RuleEngine.Model
{
    public class RuleFunction : RuleFunctionBase
    {
        private readonly IDictionary<DateTime, RuleFunctionTrail> ruleFunctionsTrail = new Dictionary<DateTime, RuleFunctionTrail>();
        
        public virtual IReadOnlyDictionary<DateTime, RuleFunctionTrail> RuleFunctionsTrail { get { return ruleFunctionsTrail.ToDictionary(x => x.Key, x => x.Value); } }
        
        protected RuleFunction()
            : base()
        {
        }

        public RuleFunction(RuleFunctionId ruleId, string name, string functionsText)
            : base(ruleId, name, functionsText)
        {
        }

        private void addFunctionTextTrail(RuleFunctionId id,  string oldName, string oldFunctionText)
        {
            ruleFunctionsTrail.Add(DateTime.Now, new RuleFunctionTrail(this, id, this.name, this.functionsText, DateTime.Now));
        }
        
        public virtual void Update(RuleFunctionId verId, string name, string functionText)
        {
            var oldName = this.name;
            this.name = name;
            var oldFunctionsText = this.functionsText;
            this.functionsText = functionText;
            addFunctionTextTrail(verId, oldName, oldFunctionsText);
        }


    }
}
