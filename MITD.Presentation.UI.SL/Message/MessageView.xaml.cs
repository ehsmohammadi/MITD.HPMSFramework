using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MITD.Presentation.UI.Message
{
    /// <summary>
    /// Interaction logic for MessageView.xaml
    /// </summary>
    public partial class MessageView : ViewBase, IMessageView
    {
        public MessageView()
        {
            InitializeComponent();
        }
        public MessageView(MessageVM vm)
        {
            InitializeComponent();
            ViewModel = vm;
        }
    }
}
