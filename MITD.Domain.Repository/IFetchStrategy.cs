using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MITD.Domain.Repository
{
    public interface IFetchStrategy<T>  where T : class
    {
        bool NoLock { get; }
        Enums.FetchInUnitOfWorkOption FetchInUnitOfWorkOption { get; }
        IList<IncludeItem<T>> Includes { get; }
        IList<ISortCriteria<T>> SortCriterias { get; }
    }


}
