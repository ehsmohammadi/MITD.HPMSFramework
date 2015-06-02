using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Core
{
    public abstract class BaseFilterMapper<TEntity, TModel> : IFilterMapper<TEntity, TModel>
        where TEntity : class
        where TModel : new()
    {
        public TModel MapToModel(TEntity entity, string[] columns)
        {
            var model = MapToModel(entity);
            return filter(model, columns);
        }

        protected abstract TModel MapToModel(TEntity entity);

        private TModel filter<TModel>(TModel obj, string[] columns)
        {
            if (columns.Length == 0 || string.IsNullOrWhiteSpace(columns.SingleOrDefault()))
                return obj;
            foreach (var column in columns)
            {
                if ( !obj.GetType().GetProperties().Select(p => p.Name).Contains(column))
                    throw new SettingsPropertyNotFoundException("'"+ column + "' Property Name Is Inavalid ");
            }

            foreach (var property in obj.GetType().GetProperties())
            {
                if (!columns.Contains(property.Name))
                {
                    property.SetValue(obj, null);
                }
            }
            return obj;
        }

    }
}
