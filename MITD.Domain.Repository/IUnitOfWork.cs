using System;
using System.Collections.Generic;

namespace MITD.Domain.Repository
{
  public  interface IUnitOfWork:IDisposable
    {
      void Commit();

      void CommitAndRefreshChanges();

      void RollBackChanges();

      bool IsDisposed { get; }

      IList<T> GetManagedEntities<T>() where T : class;

      IList<object> GetManagedEntities();

      IList<T> GetManagedEntities<T>(Func<T, bool> where) where T : class;
    }
}
