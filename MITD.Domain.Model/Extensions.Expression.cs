using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MITD.Domain.Model
{
    public static partial class Extensions
    {
        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of second to parameters of first)
            var map = first.Parameters.Select((f, i) => new
            {
                f,
                s = second.Parameters[i]
            }).ToDictionary(p => p.s, p => p.f);


            // replace parameters in the second lambda expression with parameters from the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);


            // apply composition of lambda expression bodies to parameters from the first expression 
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.And);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }
        public static string ToPropertyName<T,TRelated>(this Expression<Func<T, TRelated>> selector)
        {
            return new PropertyPathVisitor().GetPropertyPath(selector);
        }


        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> first)
        {
            return Expression.Lambda<Func<T, bool>>(Expression.Not(first.Body), first.Parameters);
        }

        class PropertyPathVisitor : ExpressionVisitor
        {
            private Stack<string> _stack;
            public string GetPropertyPath(Expression expression)
            {
                _stack = new Stack<string>();
                Visit(expression);
                return _stack.Aggregate(new StringBuilder(),
                    (sb, name) => (sb.Length > 0 ? sb.Append(".") : sb)
                        .Append(name)).ToString();
            }
            protected override Expression VisitMember(MemberExpression expression)
            {
                if (_stack != null)
                    _stack.Push(expression.Member.Name);
                return base.VisitMember(expression);
            }
            protected override Expression VisitMethodCall(MethodCallExpression expression)
            {
                if (IsLinqOperator(expression.Method))
                {
                    for (int i = 1; i < expression.Arguments.Count; i++)
                    {
                        Visit(expression.Arguments[i]);
                    }
                    Visit(expression.Arguments[0]);
                    return expression;
                }
                return base.VisitMethodCall(expression);
            }
            private static bool IsLinqOperator(MethodInfo method)
            {
                if (method.DeclaringType != typeof(Queryable) && method.DeclaringType != typeof(Enumerable))
                    return false;
                return Attribute.GetCustomAttribute(method, typeof(ExtensionAttribute)) != null;
            }
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query,string sortItem)
        {
            return applySortOrder(query, sortItem, "OrderBy");
        }
        
        public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> query, string sortItem)
        {
            return applySortOrder(query, sortItem, "OrderByDescending");
        }

        private static IQueryable<T> applySortOrder<T>(IQueryable<T> query, string sortItem, string methodName)
        {
            var pi = typeof(T).GetProperty(sortItem);

            var param = Expression.Parameter(typeof(T));
            var field = Expression.PropertyOrField(param, sortItem);
            var exp = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(T), pi.PropertyType), field, param);

            var m = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                && method.IsGenericMethodDefinition
                && method.GetGenericArguments().Length == 2
                && method.GetParameters().Length == 2).MakeGenericMethod(typeof(T), pi.PropertyType);

            return m.Invoke(null, new object[2] { query, exp }) as IQueryable<T>;
        }
    

    }
}
