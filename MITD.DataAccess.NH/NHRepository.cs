using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MITD.Domain.Repository;
using NHibernate;
using NHibernate.Linq;
using System.Diagnostics;
using NHibernate.Engine;
using System.Transactions;
using System.Linq.Expressions;
using System.Collections;
using System.Reflection;
using MITD.Core;


namespace MITD.Data.NH
{

    public class NHRepository : IRepository
    {
        protected ISession session;
        protected NHUnitOfWork unitOfWork;
        protected IUnitOfWorkScope unitOfWorkScope;

        public NHRepository(NHUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            init();
        }

        public NHRepository(IUnitOfWorkScope unitOfWorkScope)
        {
            this.unitOfWorkScope = unitOfWorkScope;
            this.unitOfWork = (unitOfWorkScope.CurrentUnitOfWork as NHUnitOfWork);
            init();
        }

        private void init()
        {
            this.session = unitOfWork.Session;
        }

        protected ISession Session
        {
            get
            {
                return session;
            }
        }

        public IUnitOfWork UnitOfWork
        {
            get { return unitOfWork; }
        }
    }

    public class NHRepository<T> : NHRepository, IRepository<T> where T : class
    {
        public NHRepository(NHUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public NHRepository(IUnitOfWorkScope unitOfWorkScope)
            : base(unitOfWorkScope)
        {
        }

        public IQueryable<TCHILD> GetQuery<TCHILD>() where TCHILD : T
        {
            if (typeof(T) == typeof(TCHILD))
                return session.Query<TCHILD>();
            else
                return session.Query<T>().OfType<TCHILD>();
        }

        public IQueryable<T> GetQuery()
        {
            return session.Query<T>();
        }

        public IList<T> GetAll()
        {
            return GetAll<T>();
        }

        public IList<TCHILD> GetAll<TCHILD>() where TCHILD : T
        {
            return GetQuery<TCHILD>().ToList();
        }

        public IList<T> GetAll(IListFetchStrategy<T> fetchStrategy)
        {
            return GetAll<T>(fetchStrategy);
        }

        public IList<TCHILD> GetAll<TCHILD>(IListFetchStrategy<TCHILD> fetchStrategy)
            where TCHILD : class, T
        {
            var query = getQueryFor<TCHILD>(fetchStrategy.FetchInUnitOfWorkOption);
            Func<IQueryable<TCHILD>, IList<TCHILD>> action = q => q.ToList();
            return FetchStrategyHelper.ExecuteQuery<TCHILD, IList<TCHILD>>(action, query, fetchStrategy);
        }

        private IQueryable<TCHILD> getQueryFor<TCHILD>(Enums.FetchInUnitOfWorkOption fetchInUnitOfWorkOption) where TCHILD : T
        {
            IQueryable<TCHILD> query = null;
            if ((fetchInUnitOfWorkOption != Enums.FetchInUnitOfWorkOption.AppendOnly)
                && (fetchInUnitOfWorkOption != Enums.FetchInUnitOfWorkOption.NoTracking))
                throw new ArgumentOutOfRangeException("fetchStrategy");
            if (fetchInUnitOfWorkOption == Enums.FetchInUnitOfWorkOption.NoTracking)
            {
                if (typeof(T) == typeof(TCHILD))
                    query = session.SessionFactory.OpenStatelessSession().Query<TCHILD>();
                else
                    query = session.SessionFactory.OpenStatelessSession().Query<T>().OfType<TCHILD>();
            }
            else
            {
                query = GetQuery<TCHILD>();
            }
            return query;
        }


        private IQueryable<TCHILD> orderByKey<TCHILD>(IQueryable<TCHILD> query) where TCHILD : T
        {
            var idName = session.SessionFactory.GetClassMetadata(typeof(TCHILD)).IdentifierPropertyName;
            return query.OrderByField(idName, true);
        }

        public IList<T> Find(System.Linq.Expressions.Expression<Func<T, bool>> where)
        {
            return Find<T>(where);
        }

        public IList<TCHILD> Find<TCHILD>(System.Linq.Expressions.Expression<Func<TCHILD, bool>> where)
            where TCHILD : class, T
        {
            return GetQuery<TCHILD>().Where(where).ToList();
        }

        public IList<T> Find(System.Linq.Expressions.Expression<Func<T, bool>> where, IListFetchStrategy<T> fetchStrategy)
        {
            return Find<T>(where, fetchStrategy);
        }

        public IList<TCHILD> Find<TCHILD>(System.Linq.Expressions.Expression<Func<TCHILD, bool>> where,
            IListFetchStrategy<TCHILD> fetchStrategy)
            where TCHILD : class, T
        {
            var query = getQueryFor<TCHILD>(fetchStrategy.FetchInUnitOfWorkOption).Where(where);
            Func<IQueryable<TCHILD>, IList<TCHILD>> action = q => q.ToList();
            return FetchStrategyHelper.ExecuteQuery<TCHILD, IList<TCHILD>>(action, query, fetchStrategy);
        }

        public T FindByKey<Key>(Key keyValue)
        {
            return session.Get<T>(keyValue);
        }

        public T FindByKey<Key>(Key keyValue, IFetchStrategy<T> fetchStrategy)
        {
            throw new NotImplementedException();
        }

        public T Single(System.Linq.Expressions.Expression<Func<T, bool>> where, ISingleResultFetchStrategy<T> fetchStrategy)
        {
            return Single<T>(where, fetchStrategy);
        }

        public TCHILD Single<TCHILD>(System.Linq.Expressions.Expression<Func<TCHILD, bool>> where, ISingleResultFetchStrategy<TCHILD> fetchStrategy)
            where TCHILD : class, T
        {
            var query = getQueryFor<TCHILD>(fetchStrategy.FetchInUnitOfWorkOption).Where(where);
            Func<IQueryable<TCHILD>, TCHILD> action = q => q.AsEnumerable().Distinct().SingleOrDefault();
            return FetchStrategyHelper.ExecuteQuery<TCHILD, TCHILD>(action, query, fetchStrategy);

        }

        public T Single(System.Linq.Expressions.Expression<Func<T, bool>> where)
        {
            return Single<T>(where);
        }

        public TCHILD Single<TCHILD>(System.Linq.Expressions.Expression<Func<TCHILD, bool>> where)
            where TCHILD : class, T
        {
            return GetQuery<TCHILD>().SingleOrDefault(where);
        }

        public T First(System.Linq.Expressions.Expression<Func<T, bool>> where, ISingleResultFetchStrategy<T> fetchStrategy)
        {
            return First<T>(where, fetchStrategy);
        }

        public TCHILD First<TCHILD>(System.Linq.Expressions.Expression<Func<TCHILD, bool>> where, ISingleResultFetchStrategy<TCHILD> fetchStrategy)
            where TCHILD : class, T
        {
            var query = getQueryFor<TCHILD>(fetchStrategy.FetchInUnitOfWorkOption).Where(where);
            Func<IQueryable<TCHILD>, TCHILD> action = q => q.AsEnumerable().Distinct().FirstOrDefault();
            return FetchStrategyHelper.ExecuteQuery<TCHILD, TCHILD>(action, query, fetchStrategy);
        }

        public T First(System.Linq.Expressions.Expression<Func<T, bool>> where)
        {
            return First<T>(where);
        }

        public TCHILD First<TCHILD>(System.Linq.Expressions.Expression<Func<TCHILD, bool>> where)
            where TCHILD : class, T
        {
            return GetQuery<TCHILD>().FirstOrDefault(where);
        }

        public void Delete(T entity)
        {
            session.Delete(entity);
        }

        public void Add(T entity)
        {
            session.Save(entity);
        }

        public void Attach(T entity)
        {
            session.Lock(entity, LockMode.None);
        }

        public void Detach(T entity)
        {
            session.Evict(entity);
        }

        public void Update(T orginalEntity, T currentEntity)
        {
            if (!session.Contains(orginalEntity))
            {
                Attach(orginalEntity);
            }
            session.Merge(currentEntity);
        }

        public void Update(T currentEntity)
        {
            if (!session.Contains(currentEntity))
            {
                try
                {
                    session.Update(currentEntity);
                }
                catch (NonUniqueObjectException exp)
                {
                    session.Merge(currentEntity);
                }
            }
            else
            {
                session.Evict(currentEntity);
                session.Update(currentEntity);
            }
        }

        //private bool attachedWithTheSameKeyOf(T currentEntity)
        //{
        //    var impl = session.GetSessionImplementation();
        //    var ps = impl.PersistenceContext;
        //    var key = new EntityKey(377, impl.Factory.GetEntityPersister(typeof(T).FullName), session.ActiveEntityMode);//todo
        //    return ps.ContainsEntity(key);
        //}

        public TCHILD CreateObject<TCHILD>()
            where TCHILD : class, T
        {
            return Activator.CreateInstance<TCHILD>();
        }

        public T CreateObject()
        {
            return Activator.CreateInstance<T>();
        }
    }
}
