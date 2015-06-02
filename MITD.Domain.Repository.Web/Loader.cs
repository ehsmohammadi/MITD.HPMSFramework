using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;

namespace MITD.Domain.Repository.Web
{
    public class Loader
    {
        public static void LoadModule()
        {
            DynamicModuleUtility.RegisterModule(typeof(WebUnitOfWorkRemoverModule));
        } 

    }
}
