using MITD.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MITD.TestLayer.Model
{
    public class EntityId3:IValueObject<EntityId3>
    {
        private readonly long dbId;
        private readonly string id;
        #region Properties
        
        public string Id
        {
            get { return id; }
        }
        #endregion

        #region Constructors
        // for ORM
        private EntityId3()
        {

        }
        public EntityId3(string id)
        {
            this.id = id;
        }

        #endregion
        
        #region IValueObject Member
        public bool SameValueAs(EntityId3 other)
        {
            return Id.Equals(other.Id);
        }
        #endregion

        #region Object Override
        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var other = (EntityId3)obj;
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
