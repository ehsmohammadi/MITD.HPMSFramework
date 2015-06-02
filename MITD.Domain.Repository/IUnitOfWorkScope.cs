
namespace MITD.Domain.Repository
{
    /// <summary>
    /// Manages Unit of work Life Time  
    /// </summary>
    public interface IUnitOfWorkScope
    {
        IUnitOfWork CurrentUnitOfWork { get;}

        void Commit();

    }
}
