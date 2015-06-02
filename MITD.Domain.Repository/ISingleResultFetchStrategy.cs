using System;
using System.Linq.Expressions;

namespace MITD.Domain.Repository
{
    public interface ISingleResultFetchStrategy<T> : IFetchStrategy<T> where T : class
    {
        ISingleResultFetchStrategy<T> Include(Expression<Func<T, dynamic>> path);
        ISingleResultFetchStrategy<T> OrderBy<S>(Expression<Func<T, S>> sortItem);
        ISingleResultFetchStrategy<T> OrderByDescending<S>(Expression<Func<T, S>> sortItem);
        ISingleResultFetchStrategy<T> OrderBy(string sortItem);
        ISingleResultFetchStrategy<T> OrderByDescending(string sortItem);
    }
}
