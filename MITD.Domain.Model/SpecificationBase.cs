using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace MITD.Domain.Model
{
    public class SpecificationBase<T> : ISpecification<T>
    {
        protected Func<T, bool> compiledpredicate;
        private Expression<Func<T, bool>> predicate;

        public SpecificationBase()
        {
            
        }

        public SpecificationBase(Expression<Func<T,bool>> predicate) : this()
        {
            this.Predicate = predicate;
        }

        public Expression<Func<T,bool>> Predicate 
        {
            get { return predicate; } 
            private set
            {
                predicate = value;
                compiledpredicate = value.Compile();
            } 
        }

        public virtual bool IsSatisfiedBy(T entity)
        {
            return compiledpredicate(entity);
        }
       
    }
}
