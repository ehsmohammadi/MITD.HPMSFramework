using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MITD.Domain.Repository;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;

namespace MITD.DataAccess.EF
{
    public  class EFUnitOfWorkFactory : IUnitOfWorkFactory
    {
        private Func<ObjectContext> ocDelegate;
        private Func<DbContext> dbDelegate;
        private bool dbContextAvailable;
        public EFUnitOfWorkFactory(Func<ObjectContext> ocDelegate)
        {
            this.ocDelegate = ocDelegate;
            dbContextAvailable = false;

        }

        public EFUnitOfWorkFactory(Func<DbContext> dbDelegate)
        {
            this.dbDelegate = dbDelegate;
            dbContextAvailable = true;
        }

        public IUnitOfWork Create()
        {
            if (dbContextAvailable)
                return new EFUnitOfWork(dbDelegate());
            else
                return new EFUnitOfWork(ocDelegate());
        }
    }
}
