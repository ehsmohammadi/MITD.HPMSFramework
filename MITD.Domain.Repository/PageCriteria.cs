
namespace MITD.Domain.Repository
{
    public class PageCriteria<T> : IPagingCriteria<T> where T : class
    {

        public PageCriteria(int pageSize,int pageNumber)
        {
            this.PageSize = pageSize;
            this.PageNumber = pageNumber;
            this.PageResult = new PageResult<T>();
        }

        public int PageNumber
        {
            get;
            private set;
        }

        public int PageSize
        {
            get;
            private set;
        }

        public PageResult<T> PageResult
        {
            get;
            private set;
        }


        public int SkipCount
        {
            get { return PageSize * (PageNumber-1); }
        }
    }
}
