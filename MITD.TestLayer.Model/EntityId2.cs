using MITD.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.TestLayer.Model
{
    public class EntityId2:IValueObject<EntityId2>
    {
        private readonly long id;
        #region Properties
        public long Id
        {
            get { return id; }
        }
        #endregion

        #region Constructors
        // for ORM
        public EntityId2()
        {

        }

        #endregion
        
        #region IValueObject Member
        public bool SameValueAs(EntityId2 other)
        {
            return Id.Equals(other.Id);
        }
        #endregion

        #region Object Override
        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var other = (EntityId2)obj;
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
