using MITD.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Core.RuleEngine.Model
{
    public abstract class RuleFunctionBase : IEntity<RuleFunctionBase>
    {
        protected readonly RuleFunctionId id;
        protected string name;
        protected string functionsText;

        public virtual RuleFunctionId Id { get { return id; } }
        public virtual string FunctionsText { get { return functionsText; } }
        public virtual string Name { get { return name; } }

        //for ORM
        protected RuleFunctionBase()
        {

        }
        public RuleFunctionBase(RuleFunctionId ruleId,string name, string functionsText)
        {
            this.id = ruleId;
            this.name = name;
            this.functionsText = functionsText;
        }

        #region IEntity Member
        public virtual bool SameIdentityAs(RuleFunctionBase other)
        {
            return (other != null) && Id.Equals(other.Id);
        }
        #endregion

        #region Override Object
        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var Rule = (RuleFunctionBase)obj;
            return SameIdentityAs(Rule);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        public override string ToString()
        {
            return Id.ToString();
        }
        #endregion
    }
}
