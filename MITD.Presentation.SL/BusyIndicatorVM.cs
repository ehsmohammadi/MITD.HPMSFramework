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

namespace MITD.Presentation
{
    public class BusyIndicatorVM : ViewModelBase
    {
        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set { this.SetField(vm=>vm.IsBusy,ref isBusy,value); }
        }
        
        private string message;
        public string Message
        {
            get { return message; }
            set { this.SetField(vm => vm.Message, ref message, value); }
        }
    }
}
