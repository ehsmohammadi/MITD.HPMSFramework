using System;
using System.Linq.Expressions;
using System.Linq;

namespace MITD.Domain.Repository
{
    public interface ISortCriteria<T> where T : class
    {
        IQueryable<T> ApplyOrder(IQueryable<T> query);
    }
}
