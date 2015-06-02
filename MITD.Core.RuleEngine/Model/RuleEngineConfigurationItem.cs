using MITD.Core.Builders;
using MITD.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Core.RuleEngine.Model
{
    public class RuleEngineConfigurationItem : EntityWithDbId<long, RuleEngineConfigurationItemId>, IEntity<RuleEngineConfigurationItem>
    {
        private string value;


        public virtual RuleEngineConfigurationItemId Id { get { return id; } }
        public virtual string Value { get { return value; } }

        //for ORM
        protected RuleEngineConfigurationItem(){}

        public RuleEngineConfigurationItem(RuleEngineConfigurationItemId id, string value)
        {
            this.id = id;
            this.value = value;
        }

        #region Object Override
        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var other = (RuleEngineConfigurationItem)obj;
            return SameIdentityAs(other);
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        #endregion

        #region IEntity Member
        public virtual bool SameIdentityAs(RuleEngineConfigurationItem other)
        {
            return (other != null) && Id.Equals(other.Id);
        }
        #endregion
    }
}
