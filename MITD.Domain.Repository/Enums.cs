
namespace MITD.Domain.Repository
{
    public class Enums
    {
        public enum FetchInUnitOfWorkOption
        {
            AppendOnly = 1,
            NoTracking = 2,
            OverWriteChanges = 3,
            PreServeChanges = 4
        }

        public enum SortOrder
        {
            Ascending = 1,
            Descending = 2
        }
    }
}
