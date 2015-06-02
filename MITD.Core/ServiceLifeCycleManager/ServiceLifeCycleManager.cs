namespace MITD.Core
{
    public class ServiceLifeCycleManager<T> : IServiceLifeCycleManager<T> where T : IService
    {
        private readonly T t;

        public ServiceLifeCycleManager(T t)
        {
            this.t = t;
        }

        public T GetService()
        {
            return t;
        }
    }
}
