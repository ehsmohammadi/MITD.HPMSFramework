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
    public class InstanceRequest
    {
        public string AssemblyName { get; set; }
        public string ClassName { get; set; }
        public Action<Object> ReturnAction { get; set; }
        public int PartCount { get; set; }
        public string OldUrl { get; set; }
        public string NewUrl { get; set; }

        public Action<Exception> OnError { get; set; }
    }
}
