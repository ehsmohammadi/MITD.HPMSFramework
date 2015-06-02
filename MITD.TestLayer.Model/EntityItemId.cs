using MITD.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MITD.TestLayer.Model
{
    public class EntityItemId : IValueObject<EntityItemId>
    {
        #region Properties
        public virtual long Id
        {
            get;
            protected set;
        }
        #endregion

        #region Constructors
        // for ORM
        public EntityItemId()
        {

        }

        #endregion
        
        #region IValueObject Member
        public virtual bool SameValueAs(EntityItemId other)
        {
            return Id.Equals(other.Id);
        }
        #endregion

        #region Object Override
        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var other = (EntityItemId)obj;
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
