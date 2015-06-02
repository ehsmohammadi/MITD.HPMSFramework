using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MITD.Domain.Repository;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core;

namespace MITD.DataAccess.EF
{
    public class EFUnitOfWork : IUnitOfWork
    {
        public ObjectContext Context { get; protected set; }


        private void init()
        {
            Context.ContextOptions.LazyLoadingEnabled = true;
            Context.ContextOptions.ProxyCreationEnabled = true;
        }

        public EFUnitOfWork(ObjectContext context)
        {
            Context = context;

            init();
        }

        public EFUnitOfWork(DbContext dbContext)
        {
            this.Context = ((IObjectContextAdapter)dbContext).ObjectContext;

            init();
        }

        public void Commit()
        {

            Context.SaveChanges();
        }

        public void CommitAndRefreshChanges()
        {
            try
            {
                Context.SaveChanges();
            }
            catch (OptimisticConcurrencyException ex)
            {

                Context.Refresh(RefreshMode.ClientWins, ex.StateEntries.Select(d => d.Entity));
                Context.SaveChanges();
            }

        }

        public void RollBackChanges()
        {
            ///IEnumerable<object> 
        }

        bool isDisposed = false;

        public bool IsDisposed
        {
            get { return isDisposed; }
        }

        public IList<T> GetManagedEntities<T>(Func<T, bool> where) where T : class
        {

            return Context.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Deleted |
                                                                    EntityState.Modified | EntityState.Unchanged)
                                                                    .Select(entr => entr.Entity).OfType<T>().Where(where).ToList();
        }

        public IList<T> GetManagedEntities<T>() where T : class
        {

            return Context.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Deleted |
                                                                    EntityState.Modified | EntityState.Unchanged)
                                                                    .Select(entr => entr.Entity).OfType<T>().ToList();
        }

        public IList<object> GetManagedEntities()
        {
            return Context.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Deleted |
                                                                        EntityState.Modified | EntityState.Unchanged)
                                                                        .Select(entr => entr.Entity).ToList();

        }


        public ObjectStateEntry GetStateEntry<T>(T entity) where T : class
        {
            ObjectStateEntry objectStateEntry = null;
            Context.ObjectStateManager.TryGetObjectStateEntry(entity, out objectStateEntry);
            return objectStateEntry;
        }

        public ObjectStateEntry GetStateEntry(EntityKey entityKey)
        {
            ObjectStateEntry objectStateEntry = null;
            Context.ObjectStateManager.TryGetObjectStateEntry(entityKey, out objectStateEntry);
            return objectStateEntry;
        }

        public void Dispose()
        {
            if (Context != null)
            {
                Context.Dispose();

            }
            isDisposed = true;
            GC.SuppressFinalize(this);

        }

    }
}
