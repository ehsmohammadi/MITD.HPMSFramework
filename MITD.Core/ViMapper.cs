using Omu.ValueInjecter;
#if SILVERLIGHT
using Omu.ValueInjecter.Silverlight;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Core
{
    public class ViMapper<TEntity, TModel> : BaseMapper<TEntity, TModel>
        where TEntity : class, new()
        where TModel : new()
    {
        private readonly ValueInjecter _injecter;

        public ViMapper()
        {
            _injecter = new ValueInjecter();
        }

        public override TModel MapToModel(TEntity entity)
        {
            var viewModel = Activator.CreateInstance<TModel>();
            _injecter.Inject(viewModel, entity);
            return viewModel;
        }

        public override TEntity MapToEntity(TModel model)
        {
            var entity = Activator.CreateInstance<TEntity>();
            _injecter.Inject(entity, model);
            return entity;
        }

    }

}
