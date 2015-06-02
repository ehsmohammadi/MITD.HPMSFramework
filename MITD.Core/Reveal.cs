using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Core
{
    public static class Reveal
    {

        /// <summary>
        /// Reveals a hidden property or field for use instead of expressions.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="name">Name of property or field</param>
        /// <returns>Expression for the hidden property or field</returns>
        public static Expression<Func<TEntity, object>> Member<TEntity>(string name)
        {
            return CreateExpression<TEntity, object>(name);
        }


        /// <summary>
        /// Reveals a hidden property or field with a specific return type for use instead of expressions.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TReturn">Property or field return type</typeparam>
        /// <param name="name">Name of property or field</param>
        /// <returns>Expression for the hidden property or field</returns>
        public static Expression<Func<TEntity, TReturn>> Member<TEntity, TReturn>(string name)
        {
            return CreateExpression<TEntity, TReturn>(name);
        }


        static Expression<Func<TEntity, TReturn>> CreateExpression<TEntity, TReturn>(string propertyName)
        {
            var type = typeof(TEntity);
            var fields = new List<FieldInfo>();
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                if (!field.Name.StartsWith("<"))
                    fields.Add(field);

            var member = fields.FirstOrDefault(x => x.Name == propertyName);


            if (member == null)
                throw new UnknownPropertyException(type, propertyName);


            var param = Expression.Parameter(member.DeclaringType, "x");
            Expression expression = Expression.PropertyOrField(param, propertyName);


            if (member.FieldType.IsValueType)
                expression = Expression.Convert(expression, typeof(object));


            var res = (Expression<Func<TEntity, TReturn>>)Expression.Lambda(typeof(Func<TEntity, TReturn>), expression, param);
            return res;
        }
    }

}
