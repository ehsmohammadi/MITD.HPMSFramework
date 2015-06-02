using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Core
{
    public interface IMapper<TEntity, TModel>:IMapper
        where TEntity : class
        where TModel : new()
    {
        TModel MapToModel(TEntity entity);
        TEntity MapToEntity(TModel model);

        IEnumerable<TModel> MapToModel(IEnumerable<TEntity> entities);
        IEnumerable<TEntity> MapToEntity(IEnumerable<TModel> models);

        TModel RemapModel(TModel model);
    }


}
