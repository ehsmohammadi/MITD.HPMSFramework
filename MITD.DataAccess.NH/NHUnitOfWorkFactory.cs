using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MITD.Domain.Repository;
using NHibernate;

namespace MITD.Data.NH
{
    public class NHUnitOfWorkFactory : IUnitOfWorkFactory
    {
        private Func<ISession> _sessionDelegate;
        private readonly Object _lockObject = new object();

        public NHUnitOfWorkFactory(Func<ISession> sessionDelegate)
        {
            _sessionDelegate = sessionDelegate;
        }

        public IUnitOfWork Create()
        {
            ISession session;
            lock (_lockObject)
            {
                session = _sessionDelegate();
            }
            return new NHUnitOfWork(session);
        }

    }
}
