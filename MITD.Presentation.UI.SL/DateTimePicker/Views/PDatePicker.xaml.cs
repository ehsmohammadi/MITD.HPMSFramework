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
using System.Windows.Controls.Primitives;
using MITD.Core;



namespace MITD.Presentation.UI.DateTimePicker.Views
{
    /// <summary>
    /// PDatePicker's view class
    /// </summary>
    /// <author>
    ///   <name>Vahid Nasiri</name>
    ///   <email>vahid_nasiri@yahoo.com</email>
    /// </author>    
    public partial class PDatePicker
    {
        #region Fields (4)

        /// <summary>
        /// Selected Gregorian Date.
        /// </summary>
        public static readonly DependencyProperty SelectedDateProperty =
                DependencyProperty.Register("SelectedDate",
                typeof(string),
                typeof(PDatePicker),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// Selected Persian Date.
        /// </summary>
        public static readonly DependencyProperty SelectedPersianDateProperty =
                DependencyProperty.Register("SelectedPersianDate",
                typeof(string),
                typeof(PDatePicker),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// Height of the TextBox.
        /// </summary>
        public static readonly DependencyProperty TextBoxHeightProperty =
                DependencyProperty.Register(
                "TextBoxHeight",
                typeof(int),
                typeof(PDatePicker),
                new PropertyMetadata(24));

        /// <summary>
        /// Width of the TextBox.
        /// </summary>
        public static readonly DependencyProperty TextBoxWidthProperty =
                DependencyProperty.Register(
                "TextBoxWidth",
                typeof(int),
                typeof(PDatePicker),
                new PropertyMetadata(100));

        #endregion Fields

        #region Constructors (1)

        /// <summary>
        /// Initialization point.
        /// </summary>
        public PDatePicker()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
            //you can listen to handled events too.
            dateTextBox.AddHandler(
                MouseLeftButtonDownEvent,
                new MouseButtonEventHandler(dateTextBoxMouseLeftButtonDown),
                true);
            persianCalnedarPopup.Opened += (s, e) =>
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        double pageHeight = Application.Current.Host.Content.ActualHeight;
                        double pageWidth = Application.Current.Host.Content.ActualWidth;
                        double calHeight = pcal1.ActualHeight;
                        double calWidth = pcal1.ActualWidth;
                        double dpHeight = this.ActualHeight;
                        MatrixTransform mt = persianCalnedarPopup.TransformToVisual(null) as MatrixTransform;
                        var dpX = mt.Matrix.OffsetX;
                        var dpY = mt.Matrix.OffsetY;

                        if (dpX - calWidth < 0)
                        {
                            persianCalnedarPopup.HorizontalOffset = (dpX - calWidth);
                        }

                        if (dpY + calHeight > pageHeight)
                        {
                            persianCalnedarPopup.VerticalOffset = pageHeight - (dpY + calHeight);
                        }
                    }
                    );
                };
        }

        #endregion Constructors

        #region Properties (4)

        /// <summary>
        /// Selected Gregorian Date.
        /// </summary>
        public string SelectedDate
        {
            get
            {
                var s = (string)GetValue(SelectedDateProperty);
                return s;
            }
            set
            {
                pcal1.SelectedDate = value;
                SetValue(SelectedDateProperty, value);
                persianCalnedarPopup.IsOpen = false;
            }
        }

        /// <summary>
        /// Selected Persian Date.
        /// </summary>
        public string SelectedPersianDate
        {
            get
            {
                var s = (string)GetValue(SelectedPersianDateProperty);
                return s;
            }
            set
            {
                //todo: validation
                pcal1.SelectedPersianDate = value;
                SetValue(SelectedPersianDateProperty, value);
                persianCalnedarPopup.IsOpen = false;
            }
        }

        /// <summary>
        /// Height of the TextBox.
        /// </summary>
        public int TextBoxHeight
        {
            get { return (int)GetValue(TextBoxHeightProperty); }
            set { SetValue(TextBoxHeightProperty, value); }
        }

        /// <summary>
        /// Width of the TextBox.
        /// </summary>
        public int TextBoxWidth
        {
            get { return (int)GetValue(TextBoxWidthProperty); }
            set { SetValue(TextBoxWidthProperty, value); }
        }

        #endregion Properties

        #region Methods (4)

        // Private Methods (4) 

        private void dateTextBoxMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //showPopup();
        }

        private void dateTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (persianCalnedarPopup.IsOpen)
                persianCalnedarPopup.IsOpen = false;
            dateTextBox.Focus();
        }

        private void openCalendarButtonClick(object sender, RoutedEventArgs e)
        {
            if (persianCalnedarPopup.IsOpen)
            {
                persianCalnedarPopup.IsOpen = false;
                dateTextBox.Focus();
            }
            else
                showPopup();
        }

        private void showPopup()
        {
            persianCalnedarPopup.IsOpen = true;
            pcal1.FlipAnim1.Begin();
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!persianCalnedarPopup.IsOpen)
            {
                if (!string.IsNullOrEmpty(SelectedPersianDate) && SelectedPersianDate != @"____/__/__")
                    SelectedDate = PDateHelper.HijriToGregorian(SelectedPersianDate).ToString();
            }
            else
            {

                var focusElt = FocusManager.GetFocusedElement() as DependencyObject;
                while (focusElt != null)
                {
                    if (focusElt is PCalendar)
                        return; // Still has the focus
                    focusElt = VisualTreeHelper.GetParent(focusElt);
                }
                persianCalnedarPopup.IsOpen = false; // Lost focus        
            }
        }
    }

        #endregion Methods
}



