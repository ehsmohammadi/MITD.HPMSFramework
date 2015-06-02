
namespace MITD.Domain.Repository
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create();
    }
}
