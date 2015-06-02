using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MITD.Domain.Model
{
    public static class ISpecification_Extentions
    {
        public static ISpecification<T> And<T>(this ISpecification<T> left,ISpecification<T> right)
        {
            return new SpecificationBase<T>(left.Predicate.And(right.Predicate));
        }

        public static ISpecification<T> Or<T>(this ISpecification<T> left,ISpecification<T> right)
        {
            return new SpecificationBase<T>(left.Predicate.Or(right.Predicate));
        }

        public static ISpecification<T> Not<T>(this ISpecification<T> left)
        {
            return new SpecificationBase<T>(left.Predicate.Not());
        }
    }
}
