using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;

namespace MITD.Domain.Repository
{
    
    public class PerThreadUnitOfWorkScope : UnitOfWorkScope
    {
        private ThreadLocal<IUnitOfWork> threaduow = new ThreadLocal<IUnitOfWork>();

        public PerThreadUnitOfWorkScope(IUnitOfWorkFactory unitOfWorkFactory)
            : base(unitOfWorkFactory)
        {
            
        }
        protected override IUnitOfWork LoadUnitOfWork()
        {
            return threaduow.Value;
        }

        protected override void SaveUnitOfWork(IUnitOfWork unitOfWork)
        {
            threaduow.Value = unitOfWork;
        }
    }
}
