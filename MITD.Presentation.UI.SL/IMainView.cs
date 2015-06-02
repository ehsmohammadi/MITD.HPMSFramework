using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MITD.Presentation.UI
{
    public interface IMainView : IView
    {
        TabControl TabControl { get; set; }
        DataTemplate TabHeaderTemplate { get; set; }
        BusyIndicator BusyIndicator { get; set; }
    }
}
