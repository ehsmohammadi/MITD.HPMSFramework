using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Core.Tests
{
    public class AutoMockingLazyComponentLoader : ILazyComponentLoader
    {
        public static object DynamicMock(Type type)
        {
            var mock = typeof(Mock<>).MakeGenericType(type).GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
            return mock.GetType().GetProperties().Single(f => f.Name == "Object" && f.PropertyType == type).GetValue(mock, new object[] { });
        }

        public IRegistration Load(string key, Type service, IDictionary arguments)
        {
            return Component.For(service).Instance(DynamicMock(service));
        }
    }
}
