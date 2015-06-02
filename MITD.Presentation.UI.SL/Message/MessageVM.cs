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

namespace MITD.Presentation.UI.Message
{
    public class MessageVM : WorkspaceViewModel
    {
        private IApplicationController controller;

        private string message;
        public string Message 
        {
            get { return message;}
            set { this.SetField(vm => vm.Message, ref message, value); }
        }
        private CommandViewModel okCommand;
        public CommandViewModel OkCommand
        {
            get { return okCommand; }
            set { this.SetField(vm => vm.OkCommand, ref okCommand, value); }
        }
        public MessageVM(IApplicationController controller)
        {
            this.controller = controller;
            okCommand = new CommandViewModel("تایید",new DelegateCommand(()=>controller.Close(this)));
        }
    }
}
