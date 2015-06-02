using System;
using System.Linq.Expressions;
using System.Linq;

namespace MITD.Domain.Repository
{
    
    public class SortCriteria<T,S> : ISortCriteria<T> where T : class
    {

        public SortCriteria(Expression<Func<T,S>> sortItem, Enums.SortOrder sortOrder=Enums.SortOrder.Ascending)
        {
            this.sortItem = sortItem;
            this.sortOrder = sortOrder;
        }

        private Enums.SortOrder sortOrder;


        private System.Linq.Expressions.Expression<Func<T, S>> sortItem;



        public System.Linq.IQueryable<T> ApplyOrder(System.Linq.IQueryable<T> query)
        {
            var oq = query;
            if (sortOrder == Enums.SortOrder.Ascending)
                oq = oq.OrderBy(sortItem);
            else
                oq = oq.OrderByDescending(sortItem);
            return oq;
        }
    }
}
