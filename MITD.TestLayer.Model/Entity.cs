using MITD.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.TestLayer.Model
{
    public class Entity:IEntity<Entity>
    {
        public virtual EntityId Id { get; protected set; }
        public virtual string TestProperty { get; set; }
        public virtual IList<EntityItem2> EntityItems2 { get; protected set; }

        public virtual void AddEntityItem2(string testProperty) 
        {
            EntityItems2.Add(new EntityItem2(this, testProperty));
        }

        protected Entity()
        {
            EntityItems2 = new List<EntityItem2>();
        }

        public Entity(EntityId id, string testProperty):this()
        {
            Id = id;
            TestProperty = testProperty;
        }

        #region IEntity Member
        public virtual bool SameIdentityAs(Entity other)
        {
            return (other != null) && Id.Equals(other.Id);
        }
        #endregion

        #region Override Object
        public override bool Equals(object obj)
        {
            if  (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var AEntity = (Entity)obj;
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
