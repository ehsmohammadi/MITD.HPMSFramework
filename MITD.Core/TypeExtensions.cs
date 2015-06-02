using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Core
{
    public static class TypeExtensions
    {
        public static MethodInfo GetMethodEx(this Type staticType, string methodName, params Type[] paramTypes)
        {
            var methods = from method in staticType.GetMethods()
                          where method.Name == methodName
                                && method.GetParameters()
                                          .Select(parameter => parameter.ParameterType)
                                          .Select(type => type.IsGenericType ? type.GetGenericTypeDefinition() : type)
                                          .SequenceEqual(paramTypes)
                          select method;
            try
            {
                return methods.SingleOrDefault();
            }
            catch (InvalidOperationException)
            {
                throw new AmbiguousMatchException();
            }
        }

    }
}
