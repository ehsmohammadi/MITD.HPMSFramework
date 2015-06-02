using System.Collections.Generic;
using System.Windows;

namespace MITD.Presentation.UI
{
    public interface IViewWithContextMenu
    {
        IList<DependencyObject> ItemsWithContextMenu { get;}
    }
}
