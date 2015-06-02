using System;

namespace MITD.Domain.Repository
{
    public class ListFetchStrategy<T> : FetchStrategy<T>,IListFetchStrategy<T> where T : class
    {

        public ListFetchStrategy(Enums.FetchInUnitOfWorkOption fetchInUnitOfWorkOption = Enums.FetchInUnitOfWorkOption.AppendOnly, 
            bool nolock = false):base(fetchInUnitOfWorkOption,nolock)
        {

        }

        public IListFetchStrategy<T> WithPaging(int pageSize, int pageNumber)
        {
            PageCriteria = new PageCriteria<T>(pageSize, pageNumber);

            return this;
        }

        public IPagingCriteria<T> PageCriteria
        {
            get;
            private set;
        }


        public new IListFetchStrategy<T> OrderBy<S>(System.Linq.Expressions.Expression<Func<T, S>> sortItem)
        {
            return base.OrderBy(sortItem) as IListFetchStrategy<T>;
        }

        public new IListFetchStrategy<T> OrderByDescending<S>(System.Linq.Expressions.Expression<Func<T, S>> sortItem)
        {
            return base.OrderByDescending(sortItem) as IListFetchStrategy<T>;
        }

        public new IListFetchStrategy<T> Include(System.Linq.Expressions.Expression<Func<T, dynamic>> path)
        {
           return base.Include(path) as IListFetchStrategy<T>;
        
        }


        public new IListFetchStrategy<T> OrderBy(string sortItem)
        {
            return base.OrderBy(sortItem) as IListFetchStrategy<T>;
        }

        public new IListFetchStrategy<T> OrderByDescending(string sortItem)
        {
            return base.OrderByDescending(sortItem) as IListFetchStrategy<T>;
        }
    }
}
