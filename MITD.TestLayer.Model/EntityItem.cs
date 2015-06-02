using MITD.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.TestLayer.Model
{
    public class EntityItem: IEntity<EntityItem>
    {
        private long dbId;
        public virtual EntityItemId Id { get; protected set; }
        public virtual string TestProperty { get; protected set; }
        public virtual EntityId EntityId { get; protected set; }

        protected EntityItem()
        {

        }

        public EntityItem(EntityId entityId, string testProperty)
        {
            Id = new EntityItemId();
            EntityId = entityId;
            TestProperty = testProperty;
        }

        #region IEntity Member
        public virtual bool SameIdentityAs(EntityItem other)
        {
            return (other != null) && Id.Equals(other.Id);
        }
        #endregion

        #region Override Object
        public override bool Equals(object obj)
        {
            if  (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var AEntity = (EntityItem)obj;
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
