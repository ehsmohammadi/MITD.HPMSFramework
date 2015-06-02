using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using System.Linq;
using IDependencyResolver =System.Web.Http.Dependencies.IDependencyResolver;

namespace MITD.Core
{
    public class IocDependencyResolver : System.Web.Mvc.IDependencyResolver
    {
        private List<string> nameSpaceParts;
        public IocDependencyResolver(List<string> nameSpaceParts):base()
        {
            this.nameSpaceParts = nameSpaceParts;
        }
        public object GetService(Type serviceType)
        {
            var s = serviceType.Namespace;
            if (nameSpaceParts.Any(c => s.Contains(c)))
                return ServiceLocator.Current.GetInstance(serviceType);
            return null;
        }


        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return ServiceLocator.Current.IsRegistered(serviceType) ? ServiceLocator.Current.GetAllInstances(serviceType) : new List<object>();
            }
            catch
            {
                return new List<object>();
            }
        }


    }

    public class ServiceResolverAdapter : IDependencyResolver
    {
        private readonly System.Web.Mvc.IDependencyResolver dependencyResolver;
        public ServiceResolverAdapter(System.Web.Mvc.IDependencyResolver dependencyResolver)
        {
            if (dependencyResolver == null) throw new ArgumentNullException("dependencyResolver");
            this.dependencyResolver = dependencyResolver;
        }

        public object GetService(Type serviceType)
        {
            return dependencyResolver.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return dependencyResolver.GetServices(serviceType);
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }

        public void Dispose()
        {

        }
    }
    public static class ServiceResolverExtensions
    {
        public static IDependencyResolver ToServiceResolver(this System.Web.Mvc.IDependencyResolver dependencyResolver)
        {
            return new ServiceResolverAdapter(dependencyResolver);
        }
    }

}