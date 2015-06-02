using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
namespace MITD.Domain.Repository
{
    public class PerHttpContextUnitOfWorkScope : UnitOfWorkScope
    {

        string key = Guid.NewGuid().ToString();

        public PerHttpContextUnitOfWorkScope(IUnitOfWorkFactory unitOfWorkFactory)
            : base(unitOfWorkFactory)
        {

        }

        protected override IUnitOfWork LoadUnitOfWork()
        {
            return (IUnitOfWork)HttpContext.Current.Items[key];
        }

        protected override void SaveUnitOfWork(IUnitOfWork unitOfWork)
        {
            HttpContext.Current.Items[key] = unitOfWork;
        }
    }
}
