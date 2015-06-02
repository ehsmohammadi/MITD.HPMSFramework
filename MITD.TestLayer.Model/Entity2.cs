using MITD.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.TestLayer.Model
{
    public class Entity2:IEntity<Entity2>
    {
        private readonly long dbId;
        private readonly IList<EntityId> entityIds;
        public virtual EntityId2 Id { get; protected set; }
        public virtual string TestProperty { get; protected set; }
        public virtual IReadOnlyList<EntityId> EntityIds { get { return new List<EntityId>(entityIds).AsReadOnly(); } }
        public virtual IList<EntityId3> EntityIds3 { get; set; }

        protected Entity2()
        {
            entityIds = new List<EntityId>();
            EntityIds3 = new List<EntityId3>();
        }

        public Entity2(string testProperty):this()
        {
            Id = new EntityId2();
            TestProperty = testProperty;
        }

        public virtual void AddEntity(Entity entity)
        {
            entityIds.Add(entity.Id);
        }

        public virtual void RemoveEntity(Entity entity)
        {
            entityIds.Remove(entity.Id);
        }
        #region IEntity Member
        public virtual bool SameIdentityAs(Entity2 other)
        {
            return (other != null) && Id.Equals(other.Id);
        }
        #endregion

        #region Override Object
        public override bool Equals(object obj)
        {
            if  (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var AEntity = (Entity2)obj;
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
