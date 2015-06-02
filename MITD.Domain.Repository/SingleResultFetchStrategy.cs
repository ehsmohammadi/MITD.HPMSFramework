using System;

namespace MITD.Domain.Repository
{
    public class SingleResultFetchStrategy<T> : FetchStrategy<T>, ISingleResultFetchStrategy<T> where T : class
    {
        public SingleResultFetchStrategy(Enums.FetchInUnitOfWorkOption fetchInUnitOfWorkOption = Enums.FetchInUnitOfWorkOption.AppendOnly, 
            bool nolock = false):base(fetchInUnitOfWorkOption,nolock)
        {

        }
        public new ISingleResultFetchStrategy<T> Include(System.Linq.Expressions.Expression<Func<T, dynamic>> path)
        {
            return base.Include(path) as ISingleResultFetchStrategy<T>;
        }

        public new ISingleResultFetchStrategy<T> OrderBy<S>(System.Linq.Expressions.Expression<Func<T, S>> sortItem)
        {
            return base.OrderBy(sortItem) as ISingleResultFetchStrategy<T>;
        }

        public new ISingleResultFetchStrategy<T> OrderByDescending<S>(System.Linq.Expressions.Expression<Func<T, S>> sortItem)
        {
            return base.OrderByDescending(sortItem) as ISingleResultFetchStrategy<T>;
        }


        public new ISingleResultFetchStrategy<T> OrderBy(string sortItem)
        {
            return base.OrderBy(sortItem) as ISingleResultFetchStrategy<T>;
        }

        public new ISingleResultFetchStrategy<T> OrderByDescending(string sortItem)
        {
            return base.OrderByDescending(sortItem) as ISingleResultFetchStrategy<T>;
        }
    }
}
