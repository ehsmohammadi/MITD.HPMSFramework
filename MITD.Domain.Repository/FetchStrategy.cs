using System;
using System.Collections.Generic;
using MITD.Domain.Model;
using System.Linq.Expressions;

namespace MITD.Domain.Repository
{
    public class FetchStrategy<T> : IFetchStrategy<T> where T : class
    {
        private readonly IList<IncludeItem<T>> includes;

        public FetchStrategy(Enums.FetchInUnitOfWorkOption fetchInUnitOfWorkOption  = Enums.FetchInUnitOfWorkOption.AppendOnly, bool nolock = false)
        {
            includes = new List<IncludeItem<T>>();
            this.NoLock = nolock;
            this.FetchInUnitOfWorkOption = fetchInUnitOfWorkOption;
            this.SortCriterias = new List<ISortCriteria<T>>();
        }

        public bool NoLock
        {
            get;
            private set;
        }

        public Enums.FetchInUnitOfWorkOption FetchInUnitOfWorkOption
        {
            get;
            private set;
        }

        public IList<IncludeItem<T>> Includes
        {
            get { return includes; }
        }

        protected IFetchStrategy<T> Include(Expression<Func<T, dynamic>> path) 
        {
            includes.Add(new IncludeItem<T>(path.ToPropertyName(), path));
            return this; 
        }


        public IList<ISortCriteria<T>> SortCriterias
        {
            get;
            private set;
        }

        protected IFetchStrategy<T> OrderBy<S>(Expression<Func<T, S>> sortItem)
        {
            SortCriteria<T,S> sortCriteria = new SortCriteria<T,S>(sortItem,Enums.SortOrder.Ascending);

            SortCriterias.Add(sortCriteria);

            return this;
        }

        protected IFetchStrategy<T> OrderByDescending<S>(Expression<Func<T, S>> sortItem)
        {
            SortCriteria<T,S> sortCriteria = new SortCriteria<T,S>(sortItem, Enums.SortOrder.Descending);

            SortCriterias.Add(sortCriteria);

            return this;
        }

        protected IFetchStrategy<T> OrderBy(string sortItem)
        {
            var sortCriteria = new StringSortCriteria<T>(sortItem, Enums.SortOrder.Ascending);

            SortCriterias.Add(sortCriteria);

            return this;
        }
        
        protected IFetchStrategy<T> OrderByDescending(string sortItem)
        {
            var sortCriteria = new StringSortCriteria<T>(sortItem, Enums.SortOrder.Descending);

            SortCriterias.Add(sortCriteria);

            return this;
        }

    }
}
