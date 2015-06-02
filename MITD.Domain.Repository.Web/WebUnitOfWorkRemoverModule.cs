using System;
using System.Web;

namespace MITD.Domain.Repository.Web
{
    public class WebUnitOfWorkRemoverModule : IHttpModule
    {
        /// <summary>
        /// You will need to configure this module in the web.config file of your
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpModule Members

        public void Dispose()
        {
            //clean-up code here.
        }

        public void Init(HttpApplication context)
        {
            context.EndRequest += new EventHandler(OnEndRequest);
        }

        #endregion

        public void OnEndRequest(Object source, EventArgs e)
        {
            foreach (var item in HttpContext.Current.Items.Values)
            {
                if (item is IUnitOfWork)
                {
                    var uow = item as IUnitOfWork;
                    if (!uow.IsDisposed)
                    {
                        uow.Dispose();
                    }
                }

            }
        }
    }
}
