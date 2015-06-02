
namespace MITD.Domain.Repository
{
    public interface IPagingCriteria<T> where T :class
    {
        
        int PageNumber { get; }
        int PageSize { get; }
        int SkipCount { get; }
        PageResult<T> PageResult { get; }

    }
}
