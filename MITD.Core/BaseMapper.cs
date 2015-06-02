using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MITD.Core
{
    public abstract class BaseMapper<TEntity, TModel> : IMapper<TEntity, TModel>  
        where TEntity : class
        where TModel : new()
    {
        public BaseMapper()
        {
            //RFU
        }

        public abstract TModel MapToModel(TEntity entity);
        public abstract TEntity MapToEntity(TModel model);

        public virtual IEnumerable<TModel> MapToModel(IEnumerable<TEntity> entities)
        {
            return entities.Select(entity => MapToModel(entity)).AsEnumerable();
        }

        public virtual IEnumerable<TEntity> MapToEntity(IEnumerable<TModel> models)
        {
            return models.Select(model => MapToEntity(model)).AsEnumerable();
        }

        public virtual TModel RemapModel(TModel model)
        {
            return MapToModel(MapToEntity(model));
        }
    }
}
