using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Core
{
    public interface IFilterMapper<TEntity,TModel> : IMapper
        where TEntity : class
        where TModel : new()

    {
        TModel MapToModel(TEntity entity,string[] columns);
    }
}
