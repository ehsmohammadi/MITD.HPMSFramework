using System;
using System.Linq.Expressions;

namespace MITD.Domain.Repository
{
    public interface IListFetchStrategy<T> : IFetchStrategy<T> where T :class
    {
        IListFetchStrategy<T> WithPaging(int pageSize, int pageNumber);
        IPagingCriteria<T> PageCriteria { get; }
        IListFetchStrategy<T> Include(Expression<Func<T, dynamic>> path);
        IListFetchStrategy<T> OrderBy<S>(Expression<Func<T, S>> sortItem);
        IListFetchStrategy<T> OrderByDescending<S>(Expression<Func<T, S>> sortItem);
        IListFetchStrategy<T> OrderBy(string sortItem);
        IListFetchStrategy<T> OrderByDescending(string sortItem);
    }
}
