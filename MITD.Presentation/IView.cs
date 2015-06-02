using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MITD.Presentation
{
    public interface IView
    {
        WorkspaceViewModel ViewModel { get; set; }
    }
}
