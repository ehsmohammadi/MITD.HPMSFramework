using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Domain.Repository
{
    public class IncludeItem<T> where T: class
    {
        public IncludeItem(string name, Expression<Func<T,dynamic>> expression)
        {
            this.Expression = expression;
            this.Name = name;
        }
        public string Name { get; private set; }
        public Expression<Func<T,dynamic>> Expression { get; private set; }
    }
}
