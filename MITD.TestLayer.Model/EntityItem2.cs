using MITD.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.TestLayer.Model
{
    public class EntityItem2: IEntity<EntityItem2>
    {
        private long dbId;
        public virtual EntityItemId2 Id { get; protected set; }
        public virtual string TestProperty { get; protected set; }
        public virtual Entity Entity { get; protected set; }

        protected EntityItem2()
        {

        }

        internal EntityItem2(Entity entity, string testProperty)
        {
            Id = new EntityItemId2();
            Entity = entity;
            TestProperty = testProperty;
        }

        #region IEntity Member
        public virtual bool SameIdentityAs(EntityItem2 other)
        {
            return (other != null) && Id.Equals(other.Id);
        }
        #endregion

        #region Override Object
        public override bool Equals(object obj)
        {
            if  (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var AEntity = (EntityItem2)obj;
            return SameIdentityAs(AEntity);
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
