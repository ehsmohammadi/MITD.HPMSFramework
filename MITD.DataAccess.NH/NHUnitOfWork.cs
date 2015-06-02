using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MITD.Domain.Repository;
using NHibernate;
using NHibernate.Engine;

namespace MITD.Data.NH
{
    public class NHUnitOfWork : IUnitOfWork
    {
        private bool isDisposed = false;
        
        public ISession Session { get; private set; }

        public NHUnitOfWork(ISession session)
        {
            Session = session;
            if (session.FlushMode != FlushMode.Commit)
                session.FlushMode = FlushMode.Commit;
        }

        public void Commit()
        {
            Session.Flush();
        }

        public void CommitAndRefreshChanges()
        {
            Commit();
        }

        public void RollBackChanges()
        {
        }

        public bool IsDisposed
        {
            get { return isDisposed; }
        }

        public void Dispose()
        {
            if (Session != null)
            {
                RollBackChanges();
                Session.Dispose();
                Session = null;
            }
            isDisposed = true;
            GC.SuppressFinalize(this);
        }

        IList<T> GetManagedEntities<T>()
        {
            IPersistenceContext pc = getPersistenceContext();

            return pc.EntitiesByKey.Values.OfType<T>().ToList();
        }

        private IPersistenceContext getPersistenceContext()
        {
            ISessionImplementor impl = Session.GetSessionImplementation();
            IPersistenceContext pc = impl.PersistenceContext;
            return pc;
        }

        public IList<T> GetManagedEntities<T>(Func<T, bool> where) where T : class
        {
            IPersistenceContext pc = getPersistenceContext();
            return pc.EntitiesByKey.Values.OfType<T>().Where(where).ToList();
        }

        IList<T> IUnitOfWork.GetManagedEntities<T>()
        {
            IPersistenceContext pc = getPersistenceContext();
            return pc.EntitiesByKey.Values.OfType<T>().ToList();
        }


        public IList<object> GetManagedEntities()
        {
            IPersistenceContext pc = getPersistenceContext();
            return pc.EntitiesByKey.Values.ToList();
        }
    }
}
