using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Core.RuleEngine.Model
{
    public class RuleFunctionTrail : RuleFunctionBase
    {
        private readonly RuleFunctionBase current;
        private readonly DateTime effectiveDate;

        protected RuleFunctionTrail()
            : base()
        {

        }
        public RuleFunctionTrail(RuleFunctionBase current, RuleFunctionId id, string name, string functionsText, DateTime effectiveDate)
            : base(id, name, functionsText)
        {
            this.current = current;
            this.effectiveDate = effectiveDate;
        }

        public virtual RuleFunctionBase Current { get { return current; } }
        public virtual DateTime EffectiveDate { get { return effectiveDate; } }

    }
}
