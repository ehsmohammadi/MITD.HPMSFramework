using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MITD.Domain.Repository
{
    public interface IRepository
    {
        IUnitOfWork UnitOfWork { get; }
    }

    public interface IRepository<T> : IRepository where T : class
    {
        IList<T> GetAll();
        IList<T> GetAll(IListFetchStrategy<T> fetchStrategy);


       /// <summary>
       /// p=>p.name=="hassan" Func
       /// </summary>
       /// <param name="where"></param>
       /// <returns></returns>
        IList<T> Find(Expression<Func<T,bool>> where);
        IList<T> Find(Expression<Func<T, bool>> where, IListFetchStrategy<T> fetchStrategy);

        T FindByKey<Key>(Key keyValue);
        T FindByKey<Key>(Key keyValue, IFetchStrategy<T> fetchStrategy);

        T Single(Expression<Func<T, bool>> where, ISingleResultFetchStrategy<T> fetchStrategy);
        T Single(Expression<Func<T, bool>> where);

        T First(Expression<Func<T, bool>> where, ISingleResultFetchStrategy<T> fetchStrategy);
        T First(Expression<Func<T, bool>> where);

        void Delete(T entity);

        void Add(T entity);

        void Attach(T entity);

        void Detach(T entity);

        void Update(T orginalEntity,T currentEntity);

        void Update(T currentEntity);

        T2 CreateObject<T2>() where T2: class,T;

        T CreateObject();
    }
}
