
namespace MITD.Domain.Repository
{
   public class UnitOfWorkScope :IUnitOfWorkScope
    {
       IUnitOfWorkFactory unitOfWorkFactory;

       IUnitOfWork unitOfWork;

       protected virtual IUnitOfWork LoadUnitOfWork()
       {

           return unitOfWork;
       }

       protected virtual void SaveUnitOfWork(IUnitOfWork unitOfWork) 
       {
           this.unitOfWork = unitOfWork;
       }

       public UnitOfWorkScope(IUnitOfWorkFactory unitOfWorkFactory)
       {
           this.unitOfWorkFactory = unitOfWorkFactory;
       }
        public IUnitOfWork CurrentUnitOfWork
        {
            get {

                var uow = LoadUnitOfWork();
                if (uow == null || uow.IsDisposed)
                {
                    uow = unitOfWorkFactory.Create();
                    SaveUnitOfWork(uow);
                }

                return uow;
            
            }
        }

        public void Commit()
        {
            CurrentUnitOfWork.Commit();
        }
    }
}
