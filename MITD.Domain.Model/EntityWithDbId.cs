using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Domain.Model
{
    public class ObjectWithDbId<T>
    {
        private T _dbId;
        protected T dbId
        {
            get { return _dbId; }
            set
            {
                _dbId = value;
                if (dbIdChanged != null)
                    dbIdChanged(this, _dbId);
            }
        }
        protected event EventHandler<T> dbIdChanged;

        public class EntityWithDbId<T2> : ObjectWithDbId<T> where T2 : ObjectWithDbId<T>
        {
            private T2 _id;
            protected  T2 id
            {
                get { return _id; }
                set 
                { 
                    _id = value; 
                    _dbId = _id.dbId; 
                }
            }
            public EntityWithDbId()
            {
                this.dbIdChanged += (s, a) => 
                { 
                    if(_id!=null)
                        _id._dbId = a; 
                };
            }
        }
    }

    public class EntityWithDbId<T1, T2> : ObjectWithDbId<T1>.EntityWithDbId<T2> where T2 : ObjectWithDbId<T1>
    { }
}
