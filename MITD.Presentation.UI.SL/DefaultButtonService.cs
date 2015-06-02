using System;
using System.Net;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MITD.Presentation.UI
{
    public static class DefaultButtonService
    {
        public static DependencyProperty DefaultButtonProperty =
              DependencyProperty.RegisterAttached("DefaultButton",
                                                  typeof(Button),
                                                  typeof(DefaultButtonService),
                                                  new PropertyMetadata(null, DefaultButtonChanged));

        private static void DefaultButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uiElement = d as UIElement;
            var button = e.NewValue as Button;
            if (uiElement != null && button != null)
            {
                uiElement.KeyUp += (sender, arg) =>
                {
                    var peer = new ButtonAutomationPeer(button);

                    if (arg.Key == Key.Enter)
                    {
                        peer.SetFocus();
                        uiElement.Dispatcher.BeginInvoke((Action)delegate
                        {

                            var invokeProv =
                                peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                            if (invokeProv != null)
                                invokeProv.Invoke();
                        });
                    }
                };
            }

        }

        public static Button GetDefaultButton(UIElement obj)
        {
            return (Button)obj.GetValue(DefaultButtonProperty);
        }

        public static void SetDefaultButton(DependencyObject obj, Button button)
        {
            obj.SetValue(DefaultButtonProperty, button);
        }
    }
    

}
