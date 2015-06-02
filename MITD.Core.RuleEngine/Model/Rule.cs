using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Core.RuleEngine.Model
{
    [Serializable]
    [DataContract]
    public class Rule : RuleBase
    {
        private readonly IDictionary<DateTime, RuleTrail> rulesTrail = new Dictionary<DateTime, RuleTrail>();

        public virtual IReadOnlyDictionary<DateTime, RuleTrail> RulesTrail { get { return rulesTrail.ToDictionary(x => x.Key, x => x.Value); } }

        protected Rule()
            : base()
        {

        }
        public Rule(RuleId ruleId, string name, string ruleText, RuleType ruleType,int executeOrder)
            : base(ruleId, name, ruleText, ruleType,executeOrder)
        {
        }

        private void addRuleTrail(RuleId id, string oldName, string oldRuleText, RuleType oldRuleType,int oldOrder)
        {
            rulesTrail.Add(DateTime.Now, new RuleTrail(this, id, oldName, oldRuleText, oldRuleType, DateTime.Now, oldOrder));
        }

        public virtual void Update(RuleId verId, string name, string ruleText, RuleType ruleType,int order)
        {
            var oldName = this.name;
            this.name = name;
            var oldRuleText = this.ruleText;
            this.ruleText = ruleText;
            var oldRuleType = this.ruleType;
            this.ruleType = ruleType;
            var oldOrder = this.executeOrder;
            this.executeOrder = order;
            addRuleTrail(verId, oldName, oldRuleText, oldRuleType,oldOrder);
        }


    }


}
