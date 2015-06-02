using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using Newtonsoft.Json;
using System.IO;
using MITD.Presentation.UI.Message;

namespace TestSilverlightControls
{
    public partial class MainPage : UserControl, INotifyPropertyChanged, IDataErrorInfo
    {
        public class VisibilityConverter : IValueConverter 
        { 
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture) 
            { 
                if (value != null && (bool)value ) 
                { 
                    return Visibility.Visible; 
                } 
                return Visibility.Collapsed; 
            } 
            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
            {
                if (value != null && (Visibility)value==Visibility.Visible)
                {
                    return true;
                }
                return false;
            } 
        }

        public class data
        {
            public int id { get; set; }
            public string name { get; set; }
            public ObservableCollection<data> subData { get; set; }
        }

        public MainPage()
        {
            InitializeComponent();
            this.MaskedText = "123456";
            dataGrid1.ItemsSource = new ObservableCollection<data> 
            { 
                new data { id = 1, name = "nader" , subData = new ObservableCollection<data>{ new data{ id=1, name="salam" }, new data { id=2, name="alayk" } } }, 
                new data { id = 2, name = "nima" , subData = new ObservableCollection<data>{ new data{ id=1, name="good" }, new data { id=2, name="by" } } }
            };
            DataContext = this;
        }

        private void Button_Loaded(object sender, RoutedEventArgs e)
        {
            ToggleButton button = sender as ToggleButton;
            button.IsChecked = true;
            DataGridRow row = button.FindAncestor<DataGridRow>();  //Custom Extension 
            row.SetBinding(DataGridRow.DetailsVisibilityProperty, new Binding()
            {
                Source = button,
                Path = new PropertyPath("IsChecked"),
                Converter = new VisibilityConverter(),
                Mode = BindingMode.TwoWay
            }); 
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ToggleButton button = sender as ToggleButton;
            button.Content = "-";
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleButton button = sender as ToggleButton;
            button.Content = "+";


        }


        public event PropertyChangedEventHandler PropertyChanged;
        private DateTime? selectedDate;
        private string maskedText;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public DateTime? SelectedDate
        {
            get 
            { 
                return selectedDate; 
            }
            set 
            {
                if (value != selectedDate)
                {
                    selectedDate = value;
                    OnPropertyChanged("SelectedDate");
                }
            }
        }

        public string MaskedText
        {
            get 
            { 
                return maskedText; 
            }
            set
            {
                if (value != maskedText)
                {
                    maskedText = value;
                    OnPropertyChanged("MaskedText");
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SelectedDate = null;
        }

        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string columnName]
        {
            get 
            {
                var res = "";
                if (columnName == "MaskedText")
                {
                    if (maskedText == "" || maskedText=="______")
                    {
                        res = "Text must not be null";
                    }
                }
                else if (columnName == "SelectedDate")
                {
                    if (selectedDate == null)
                    {
                        res = "Date must not be null";
                    }
                }
                return res;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //var vm = new MessageVM();
            //vm.Message = "salam";
            //var w = new MessageView(vm);
            //message.Children.Add(w);
        }
    }

    public static class helper
    {
        public static T FindAncestor<T>(this DependencyObject obj) where T : DependencyObject
        {
            while (obj != null)
            {
                T o = obj as T;
                if (o != null)
                    return o;

                obj = VisualTreeHelper.GetParent(obj);
            }
            return null;
        }

    }

}
