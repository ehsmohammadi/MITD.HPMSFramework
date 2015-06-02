using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Practices.ServiceLocation;
using MITD.TestLayer.DataAccess.EF;
using MITD.TestLayer.Model;

namespace MITD.Domain.Repository.Web.Test
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var rep = ServiceLocator.Current.GetInstance<IRepository<TestEntity>>();
            var x = rep.GetAll();
        }
    }
}
