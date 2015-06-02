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
using System.Linq;
using System.Text.RegularExpressions;

namespace MITD.Presentation.UI.Behaviors
{
    public enum TexBoxFarsiBehaviorOption  {AlphaNumeric, Alpha, Numeric};

    public class TextBoxFarsiBehavior : Behavior<TextBox>
    {
        private string indicNumbers = "٠١٢٣٤٥٦٧٨٩";
        private string numbers = "0123456789";
        private string toArabicDigit(string inputString)
        {
            var outString = new System.Text.StringBuilder();
            foreach (var c in inputString)
            {
                if (numbers.Contains(c))
                {
                    string s = "" + c;
                    outString.Append(indicNumbers[Int32.Parse(s)]);
                }
                else
                {
                    outString.Append(c);
                }
            }
            return outString.ToString();
        }

        public static readonly DependencyProperty TexBoxFarsiBehaviorOptionProperty =
        DependencyProperty.Register("TexBoxFarsiBehaviorOption", typeof(TexBoxFarsiBehaviorOption), typeof(TextBoxFarsiBehavior), 
                    new PropertyMetadata(TexBoxFarsiBehaviorOption.AlphaNumeric));
        public TexBoxFarsiBehaviorOption TexBoxFarsiBehaviorOption
        {
            get { return (TexBoxFarsiBehaviorOption)GetValue(TexBoxFarsiBehaviorOptionProperty); }
            set { SetValue(TexBoxFarsiBehaviorOptionProperty, value); }
        } 

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = (sender as TextBox);
            tb.TextChanged -= textBox_TextChanged;
            string regexExp=string.Empty;
            switch (TexBoxFarsiBehaviorOption)
            {
                case TexBoxFarsiBehaviorOption.AlphaNumeric:
                    regexExp = @"[^\p{IsArabic},\s,0-9]";
                    break;
                case TexBoxFarsiBehaviorOption.Alpha:
                    regexExp = @"[^\p{IsArabic},\s]";
                    break;
                case TexBoxFarsiBehaviorOption.Numeric:
                    regexExp = @"[^0-9,٠-٩,\.]";
                    break;
            }
            var regex = new Regex(regexExp);
            var s = regex.Replace((sender as TextBox).Text, "");
            tb.Text = toArabicDigit(s);
            tb.SelectionStart = tb.Text.Length;
            tb.TextChanged += textBox_TextChanged;
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.TextChanged += new TextChangedEventHandler(textBox_TextChanged);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.TextChanged -= new TextChangedEventHandler(textBox_TextChanged);
        }
    }
}
