using MITD.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Core.RuleEngine.Model
{
    [KnownType(typeof(Rule))]
    [Serializable]
    [DataContract]
    public class RuleBase : IEntity<RuleBase>
    {
        [DataMember]
        protected readonly RuleId id;
        [DataMember]
        protected string ruleText;
        [DataMember]
        protected RuleType ruleType;
        [DataMember]
        protected string name;
        [DataMember]
        protected int executeOrder;

        public virtual RuleId Id { get { return id; } }
        public virtual string RuleText { get { return ruleText; } }
        public virtual int ExecuteOrder { get { return executeOrder; } }

        //for ORM
        protected RuleBase()
        {

        }
        public RuleBase(RuleId ruleId, string name, string ruleText, RuleType ruleType,int executeOrder)
        {
            this.id = ruleId;
            this.ruleText = ruleText;
            this.ruleType = ruleType;
            this.name = name;
            this.executeOrder = executeOrder;
        }

        public virtual string Name { get { return name; } }
        
        public virtual RuleType RuleType { get { return ruleType; } }
        #region IEntity Member
        public virtual bool SameIdentityAs(RuleBase other)
        {
            return (other != null) && id.Equals(other.Id);
        }
        #endregion

        #region Override Object
        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var Rule = (RuleBase)obj;
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
