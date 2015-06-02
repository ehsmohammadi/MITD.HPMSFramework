using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Core.RuleEngine.Model
{
    [Serializable]
    public class RuleTrail : RuleBase
    {
        private Rule current;
        private DateTime effectiveDate;
        
        protected RuleTrail()
            : base()
        {

        }
        public RuleTrail(Rule current, RuleId ruleId, string name, string ruleText, RuleType ruleType, DateTime effectiveDate,int executeOrder)
            : base(ruleId, name, ruleText, ruleType, executeOrder)
        {
            this.current = current;
            this.effectiveDate = effectiveDate;
        }

        public virtual Rule Current { get { return current; } }
        public virtual DateTime EffectiveDate { get { return effectiveDate; } }

    }
}
