using MITD.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.TestLayer.Model
{
    public class Entity3:IEntity<Entity3>
    {
        public virtual EntityId3 Id { get; protected set; }
        public virtual string TestProperty { get; protected set; }

        protected Entity3()
        {
        }

        public Entity3(string id, string testProperty):this()
        {
            Id = new EntityId3(id);
            TestProperty = testProperty;
        }

        #region IEntity Member
        public virtual bool SameIdentityAs(Entity3 other)
        {
            return (other != null) && Id.Equals(other.Id);
        }
        #endregion

        #region Override Object
        public override bool Equals(object obj)
        {
            if  (this == obj) return true;
            if (obj == null || GetType() != obj.GetType()) return false;
            var AEntity = (Entity3)obj;
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
