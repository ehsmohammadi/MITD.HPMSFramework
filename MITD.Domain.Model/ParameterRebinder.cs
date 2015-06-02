using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace MITD.Domain.Model
{
	/// <summary>
	/// LINQ to Entities does not support InvocationExpressions. 
	/// Rather than invoking the expression with c1, I can manually rebind the parameter. 
	/// Matt Warren’s series of articles on IQueryable providers includes an ExpressionVisitor implementation 
	/// that makes it easy to rewrite expression trees. 
	/// If you do any LINQ expression manipulation, this class is a crucial tool. 
	/// Here’s an implementation of the visitor that rebinds parameters
	/// </summary>
	/// <see cref="http://blogs.msdn.com/b/meek/archive/2008/05/02/linq-to-entities-combining-predicates.aspx"/>
	/// <see cref="http://blogs.msdn.com/b/mattwar/archive/2007/07/30/linq-building-an-iqueryable-provider-part-i.aspx"/>
	public class ParameterRebinder : ExpressionVisitor
	{
	  
		private readonly Dictionary<ParameterExpression, ParameterExpression> map;
         


		public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
		{

			this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
           
		}



		public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
		{

			return new ParameterRebinder(map).Visit(exp);

		}



		protected override Expression VisitParameter(ParameterExpression p)
		{

			ParameterExpression replacement;

			if(map.TryGetValue(p, out replacement))
			{

				p = replacement;

			}

			return base.VisitParameter(p);

		}

	}
}
