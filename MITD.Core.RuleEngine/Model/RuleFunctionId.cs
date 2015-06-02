using MITD.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MITD.Core.RuleEngine.Model
{
    [Serializable]
    public class RuleFunctionId : IValueObject<RuleFunctionId>
    {
        private long id;
        #region Properties
        public long Id
        {
            get { return id; }
        } 
        #endregion

        #region Constructors
        // for ORM
        protected RuleFunctionId(){}

        public RuleFunctionId(long id)
        {
            this.id = id;
        } 
        #endregion
        
        #region IValueObject Member
        public bool SameValueAs(RuleFunctionId other)
        {
            return Id.Equals(other.Id);
        }
        #endregion

        #region Object Override
        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var other = (RuleFunctionId)obj;
            return SameValueAs(other);
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
