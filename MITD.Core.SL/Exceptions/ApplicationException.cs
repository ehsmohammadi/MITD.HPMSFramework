﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MITD.Core.Exceptions
{
    public class ApplicationException : Exception
    {
        public ApplicationException()
            : base("An Error has ocurred")
        {
        }
        public ApplicationException(string message)
            : base(message)
        {

        }
        public ApplicationException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
