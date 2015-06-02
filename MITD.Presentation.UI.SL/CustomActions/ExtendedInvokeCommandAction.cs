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
using System.Windows.Interactivity;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace MITD.Presentation.UI
{
    public class ExtendedInvokeCommandAction : TriggerAction<FrameworkElement>
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(ExtendedInvokeCommandAction), new PropertyMetadata(null, CommandChangedCallback));
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(ExtendedInvokeCommandAction), new PropertyMetadata(null, CommandParameterChangedCallback));

        private static void CommandParameterChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var invokeCommand = d as ExtendedInvokeCommandAction;
            if (invokeCommand != null)
                invokeCommand.SetValue(CommandParameterProperty, e.NewValue);
        }

        private static void CommandChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var invokeCommand = d as ExtendedInvokeCommandAction;
            if (invokeCommand != null)
                invokeCommand.SetValue(CommandProperty, e.NewValue);
        }

        protected override void Invoke(object parameter)
        {
            if (this.Command == null)
                return;
            
            if (this.Command.CanExecute(CommandParameter))
            {
                this.Command.Execute(CommandParameter);
            }

            //if (this.Command.CanExecute(parameter))
            //{
            //    var commandParameter = new ExtendedCommandParameter(parameter as EventArgs, this.AssociatedObject,
            //                                                        GetValue(CommandParameterProperty));
            //    this.Command.Execute(commandParameter);
            //}
        }

        #region public properties

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public ICommand Command
        {
            get { return GetValue(CommandProperty) as ICommand; }
            set { SetValue(CommandParameterProperty, value); }
        }
        #endregion
    }
    
    public class ExtendedCommandParameter
    {
        public ExtendedCommandParameter(EventArgs eventArgs, FrameworkElement sender, object parameter)
        {
            EventArgs = eventArgs;
            Sender = sender;
            Parameter = parameter;
        }

        public EventArgs EventArgs { get; private set; }
        public FrameworkElement Sender { get; private set; }
        public object Parameter { get; private set; }
    }
}
