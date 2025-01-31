﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace MITD.Domain.Model
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Predicate {get;}

        bool IsSatisfiedBy(T entity);

    }
}
