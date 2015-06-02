using MITD.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using NHibernate.Linq;


namespace MITD.Data.NH
{
    public static class FetchStrategyHelper
    {
        public static RESULT ExecuteQuery<T, RESULT>(Func<IQueryable<T>, RESULT> action, IQueryable<T> query, IFetchStrategy<T> fetchStrategy)
            where T : class
        {

            RESULT result;

            var q = applyFetchStrategy<T>(fetchStrategy, query);

            if (fetchStrategy.NoLock)
                result = ExecuteWithNoLock(() => action(q));
            else
                result = action(q);

            if (fetchStrategy is IListFetchStrategy<T>
                && typeof(RESULT) == typeof(IList<T>)
                && ((IListFetchStrategy<T>)fetchStrategy).PageCriteria != null)
                ((IListFetchStrategy<T>)fetchStrategy).PageCriteria.PageResult.Result = (IList<T>)result;


            return result;
        }

        private static IQueryable<T> applyFetchStrategy<T>(IFetchStrategy<T> fetchStrategy, IQueryable<T> query)
            where T : class
        {
            if (fetchStrategy is ISingleResultFetchStrategy<T>)
                return applySingleFetchStrategy<T>(fetchStrategy as ISingleResultFetchStrategy<T>, query);
            else if (fetchStrategy is IListFetchStrategy<T>)
                return applyListFetchStrategy<T>(fetchStrategy as IListFetchStrategy<T>, query);
            else
                return query;
        }

        private static IQueryable<T> applyListFetchStrategy<T>(IListFetchStrategy<T> fetchStrategy, IQueryable<T> query)
            where T : class
        {

            var q = applyIncludes<T>(fetchStrategy, query);
            var q2 = applySorting<T>(fetchStrategy, q, fetchStrategy.PageCriteria != null);

            return applyPaging<T>(fetchStrategy, q2);

        }

        private static IQueryable<T> applySingleFetchStrategy<T>(ISingleResultFetchStrategy<T> fetchStrategy, IQueryable<T> query)
            where T : class
        {

            var q = applyIncludes<T>(fetchStrategy, query);
            return applySorting<T>(fetchStrategy, q);

        }

        private static IQueryable<T> applyPaging<T>(IListFetchStrategy<T> fetchStrategy, IQueryable<T> query)
            where T : class
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

        private static IQueryable<T> applyIncludes<T>(IFetchStrategy<T> fetchStrategy, IQueryable<T> query)
            where T : class
        {
            IQueryable<T> oq = query;

            fetchStrategy.Includes.ToList().ForEach(i =>
            {
                oq = oq.Fetch(i.Expression);
            });

            return oq;
        }

        private static IQueryable<T> applySorting<T>(IFetchStrategy<T> fetchStrategy, IQueryable<T> query,
            bool applyDefaultSortWhenNoSortingDefined = false)
            where T : class
        {
            IQueryable<T> oq = query;

            //if ((fetchStrategy.SortCriterias.Count == 0) && applyDefaultSortWhenNoSortingDefined
            //    && (typeof(ANY).IsAssignableFrom(typeof(T)) || typeof(ANY) == typeof(T)))
            //{
            //    oq = orderByKey(oq);
            //}

            fetchStrategy.SortCriterias.ToList().ForEach(i =>
            {
                oq = i.ApplyOrder(oq);
            });

            return oq;
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
    }
}
