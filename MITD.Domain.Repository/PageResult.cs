using System.Collections.Generic;

namespace MITD.Domain.Repository
{
    public class PageResult<T> where T : class
    {
        public IList<T> Result { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}
