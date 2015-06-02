using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MITD.Domain.Repository;
using System.Data;
using System.Linq.Expressions;
using System.Transactions;
using IsolationLevel = System.Transactions.IsolationLevel;
using System.Reflection;
using MITD.Domain.Model;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core;
using System.Data.Entity;

namespace MITD.DataAccess.EF
{
    public class EFRepository<T> : IRepository<T> where T : class
    {

        private EFUnitOfWork unitofwork;
        private IUnitOfWorkScope unitofworkscope;
        private ObjectContext objectContext;
        private ObjectSet<T> objectSet;
        private EntitySet entitySet;
        private ReadOnlyMetadataCollection<EdmMember> keyMembers;
        private string containerName;
        private string qualifiedEntitySetName;

        private void init()
        {
           
            this.objectContext = unitofwork.Context;
            this.objectSet = objectContext.CreateObjectSet<T>();
            this.entitySet = objectSet.EntitySet;
            this.keyMembers = this.entitySet.ElementType.KeyMembers;
            this.containerName = objectContext.DefaultContainerName;
            this.qualifiedEntitySetName = this.containerName + "." + entitySet.Name;
        }

        public EFRepository(EFUnitOfWork unitofwork)
        {
            this.unitofwork = unitofwork;
            init();
        }

        public EFRepository(IUnitOfWorkScope unitofworkscope)
        {
            this.unitofworkscope = unitofworkscope;
            this.unitofwork = (unitofworkscope.CurrentUnitOfWork as EFUnitOfWork);
            init();

        }

        protected ObjectContext Context { get { return objectContext; } }

        public IQueryable<T> GetQuery()
        {
            return this.objectSet;
        }

        public IList<T> GetAll()
        {
            return this.objectSet.ToList();
        }

        public IList<T> GetAll(IListFetchStrategy<T> fetchStrategy)
        {

            var query = objectSet;

            Func<IQueryable<T>, IList<T>> action = q => q.ToList();

            return executeQuery(action, query, fetchStrategy);

        }

        private IQueryable<T> applyFetchStrategy(IFetchStrategy<T> fetchStrategy, ObjectQuery<T> query)
        {
            if (fetchStrategy is ISingleResultFetchStrategy<T>)
                return applySingleFetchStrategy(fetchStrategy as ISingleResultFetchStrategy<T>, query);
            else if (fetchStrategy is IListFetchStrategy<T>)
                return applyListFetchStrategy(fetchStrategy as IListFetchStrategy<T>, query);
            else
                return query;
        }

        private IQueryable<T> applyListFetchStrategy(IListFetchStrategy<T> fetchStrategy, ObjectQuery<T> query)
        {

            var q = applyIncludes(fetchStrategy, query);
            var q2 = applySorting(fetchStrategy, q, fetchStrategy.PageCriteria != null);

            return applyPaging(fetchStrategy, q2);

        }

        private IQueryable<T> applySingleFetchStrategy(ISingleResultFetchStrategy<T> fetchStrategy, ObjectQuery<T> query)
        {

            var q = applyIncludes(fetchStrategy, query);
            return applySorting(fetchStrategy, q);

        }

        private IQueryable<T> applyPaging(IListFetchStrategy<T> fetchStrategy, IQueryable<T> query)
        {

            if (fetchStrategy.PageCriteria == null)
                return query;

            var skipCount = fetchStrategy.PageCriteria.SkipCount;
            IQueryable<T> result = query;
            var currentPage = fetchStrategy.PageCriteria.PageNumber;
            var pageSize = fetchStrategy.PageCriteria.PageSize;

            var cnt = query.Count();


            if (cnt < skipCount)
            {
                skipCount = Math.Max(0, cnt - pageSize);
                currentPage = (skipCount / pageSize) + 1;
            }

            if (skipCount != 0)
                result = result.Skip(skipCount).Take(pageSize);

            result = result.Take(pageSize);

            fetchStrategy.PageCriteria.PageResult.CurrentPage = currentPage;
            fetchStrategy.PageCriteria.PageResult.PageSize = pageSize;
            fetchStrategy.PageCriteria.PageResult.TotalCount = cnt;
            fetchStrategy.PageCriteria.PageResult.TotalPages = Convert.ToInt32(Math.Ceiling((decimal)(cnt / pageSize)));

            return result;
        }

        private ObjectQuery<T> applyIncludes(IFetchStrategy<T> fetchStrategy, ObjectQuery<T> query)
        {
            ObjectQuery<T> oq = query;

            fetchStrategy.Includes.ToList().ForEach(i => oq = oq.Include(i.Name));

            return oq;
        }

 
        private IQueryable<T> applySorting(IFetchStrategy<T> fetchStrategy, IQueryable<T> query, bool applyDefaultSortWhenNoSortingDefined = false)
        {
            IQueryable<T> oq = query;

            if (fetchStrategy.SortCriterias.Count == 0 && applyDefaultSortWhenNoSortingDefined)
            {
                oq = orderByKey(oq);
                
            }

            fetchStrategy.SortCriterias.ToList().ForEach(i =>
            {
                oq = i.ApplyOrder(oq);
            });

            return oq;
        }

        private IQueryable<T> orderByKey(IQueryable<T> query)
        {
            return query.OrderBy(keyMembers[0].Name);
        }

        private static RESULT ExecuteWithNoLock<RESULT>(Func<RESULT> action)
        {
            var transaactionptions = new TransactionOptions();
            transaactionptions.IsolationLevel = IsolationLevel.ReadUncommitted;

            using (var transactionscope = new TransactionScope(TransactionScopeOption.Required, transaactionptions))
            {
                var res = action();
                transactionscope.Complete();

                return res;
            }

        }

        public IList<T> Find(Expression<Func<T, bool>> where)
        {
            return this.objectSet.Where(where).ToList();
        }

        public IList<T> Find(Expression<Func<T, bool>> where, IListFetchStrategy<T> fetchStrategy)
        {

            var query = objectSet.Where(where) as ObjectQuery<T>;

            Func<IQueryable<T>, IList<T>> action = q => q.ToList();

            return executeQuery(action, query, fetchStrategy);

        }

        public T FindByKey<Key>(Key keyValue)
        {

            var entityKey = createEntityKey<Key>(keyValue);
            return GetObjectByKey(entityKey);
        }

        public T FindByKey(T entity)
        {

            var entityKey = createEntityKey(entity);

            return GetObjectByKey(entityKey);
        }

        private T GetObjectByKey(EntityKey entityKey)
        {
            object result = null;
            objectContext.TryGetObjectByKey(entityKey, out result);

            return (T)result;
        }

        private EntityKey createEntityKey(T entity)
        {
            var type = typeof(T);
            var keyNames = keyMembers.Select(d => d.Name);
            var keyFields = type.GetProperties().Where(fi => keyNames.Contains(fi.Name));
            var keyDic = keyFields.Select(fi => new KeyValuePair<string, object>(fi.Name, fi.GetValue(entity, null)));

            return new EntityKey(qualifiedEntitySetName, keyDic);

        }

        private EntityKey createEntityKey<Key>(Key keyValue)
        {
            var entityKey = new EntityKey(this.qualifiedEntitySetName, this.keyMembers[0].Name, keyValue);

            return entityKey;
        }

        public T Single(Expression<Func<T, bool>> where)
        {
            return this.objectSet.SingleOrDefault(where);
        }

        private RESULT executeQuery<RESULT>(Func<IQueryable<T>, RESULT> action, ObjectQuery<T> query, IFetchStrategy<T> fetchStrategy)
        {
            MergeOption oldMergeOption = this.objectSet.MergeOption;
            RESULT result;

            try
            {
                this.objectSet.MergeOption = ConvertToMergeOption(fetchStrategy.FetchInUnitOfWorkOption);

                var q = applyFetchStrategy(fetchStrategy, query);

                if (fetchStrategy.NoLock)
                    //result = ExecuteWithNoLock(() => { return action(); });
                    result = ExecuteWithNoLock(() => action(q));
                else
                    result = action(q);

                if (fetchStrategy is IListFetchStrategy<T>
                    && typeof(RESULT) == typeof(IList<T>)
                    && ((IListFetchStrategy<T>)fetchStrategy).PageCriteria != null)
                    ((IListFetchStrategy<T>)fetchStrategy).PageCriteria.PageResult.Result = (IList<T>)result;


            }
            finally
            {
                this.objectSet.MergeOption = oldMergeOption;
            }

            return result;
        }


        public T First(Expression<Func<T, bool>> where)
        {
            var x = this.objectSet.FirstOrDefault(where);

            return x;
        }

        public T First(Expression<Func<T, bool>> where, ISingleResultFetchStrategy<T> fetchStrategy)
        {
            var query = objectSet;

            Func<IQueryable<T>, T> action = q => q.FirstOrDefault(where);

            return executeQuery(action, query, fetchStrategy);

        }


        public void Delete(T entity)
        {
            this.objectSet.DeleteObject(entity);
        }

        public void Add(T entity)
        {
            this.objectSet.AddObject(entity);
        }

        public void Attach(T entity)
        {
            this.objectSet.Attach(entity);
        }

        public void Detach(T entity)
        {
            this.objectSet.Detach(entity);
        }

        public void Update(T orginalEntity, T currentEntity)
        {

            //System.Data.Metadata.Edm.EntitySet set;
            //System.Data.Metadata.Edm.EdmMember pk;
            //findKeyName(out set, out pk);
            //var type = typeof(T);
            //var orginalKeyValue =type.GetField(pk.Name).GetValue(orginalEntity);
            //var orginalKeyValue = type.GetField(pk.Name).GetValue(orginalEntity);

            var attachedEntity = this.unitofwork.GetManagedEntities<T>(d => d == orginalEntity).SingleOrDefault<T>();
            if (attachedEntity == null)
            {
                Attach(orginalEntity);
            }
            this.objectSet.ApplyCurrentValues(currentEntity);

        }

        public void Update(T currentEntity)
        {

            var entry = this.unitofwork.GetStateEntry(currentEntity);
            if (entry == null)
            {
                var entityKey = createEntityKey(currentEntity);
                entry = this.unitofwork.GetStateEntry(entityKey);
                if (entry == null)
                {
                    Attach(currentEntity);
                    entry = this.unitofwork.GetStateEntry(currentEntity);
                }
                else
                {
                    this.objectSet.ApplyCurrentValues(currentEntity);
                }
            }

            entry.ChangeState(EntityState.Modified);


        }

        protected static MergeOption ConvertToMergeOption(MITD.Domain.Repository.Enums.FetchInUnitOfWorkOption fetchInUnitOfWorkOption)
        {
            MergeOption resulteMergeOption = MergeOption.AppendOnly;

            switch (fetchInUnitOfWorkOption)
            {
                case Enums.FetchInUnitOfWorkOption.NoTracking:
                    {
                        resulteMergeOption = MergeOption.NoTracking;
                        break;
                    }
                case Enums.FetchInUnitOfWorkOption.AppendOnly:
                    {
                        resulteMergeOption = MergeOption.AppendOnly;
                        break;
                    }
                case Enums.FetchInUnitOfWorkOption.OverWriteChanges:
                    {
                        resulteMergeOption = MergeOption.OverwriteChanges;
                        break;
                    }
                case Enums.FetchInUnitOfWorkOption.PreServeChanges:
                    {
                        resulteMergeOption = MergeOption.PreserveChanges;
                        break;
                    }
            }
            return resulteMergeOption;
        }

        public T FindByKey<Key>(Key keyValue, IFetchStrategy<T> fetchStrategy)
        {
            throw new NotImplementedException();
        }

        public T Single(Expression<Func<T, bool>> where, ISingleResultFetchStrategy<T> fetchStrategy)
        {
            var query = objectSet;

            Func<IQueryable<T>, T> action = q => q.SingleOrDefault(where);

            return executeQuery(action, query, fetchStrategy);
        }

        public T2 CreateObject<T2>() where T2 : class,T
        {
            return objectSet.CreateObject<T2>();
        }

        public T CreateObject()
        {
            return objectSet.CreateObject();
        }

        public IUnitOfWork UnitOfWork
        {
            get { return unitofwork; }
        }
    }
}
