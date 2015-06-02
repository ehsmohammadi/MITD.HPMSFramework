using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MITD.Presentation.UI
{
    public class DeploymentModule
    {
        public string Name { get; set; }
        public string FileUrl { get; set; }
        public IList<Type> Types { get; set; }
    }
}
