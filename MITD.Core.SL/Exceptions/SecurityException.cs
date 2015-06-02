using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Core.Exceptions
{
    public class SecurityException : ApplicationException
    {
        public SecurityException()
            : base("Unauthorized access.")
        {}
        public SecurityException(string message)
            : base(message)
        {}
        public SecurityException(string message, Exception innerException)
            : base(message, innerException)
        {}
    }
}
