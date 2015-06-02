using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MITD.Domain.Model;

namespace MITD.Domain.Repository
{
    public class StringSortCriteria<T> : ISortCriteria<T> where T : class
    {
        private Enums.SortOrder sortOrder;
        private string sortItem;

        public StringSortCriteria(string sortItem, Enums.SortOrder sortOrder = Enums.SortOrder.Ascending)
        {
            this.sortItem = sortItem;
            this.sortOrder = sortOrder;
        }

        public IQueryable<T> ApplyOrder(IQueryable<T> query)
        {
            var oq = query;
            if (sortOrder == Enums.SortOrder.Ascending)
                oq = oq.OrderBy<T>(sortItem);
            else
                oq = oq.OrderByDescending<T>(sortItem);
            return oq;
        }
    }
}
