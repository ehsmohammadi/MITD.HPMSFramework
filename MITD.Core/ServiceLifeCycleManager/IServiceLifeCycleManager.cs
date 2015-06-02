
namespace MITD.Core
{
    public interface IServiceLifeCycleManager<T> : IService where T:IService
    {
        T GetService();
    }
}
