using System;
using System.Net;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.ComponentModel;

namespace MITD.Presentation
{
    public class SortInfo : INotifyPropertyChanged
    {
        private string fieldName;
        public string FieldName
        {
            get { return fieldName; }
            set
            {
                if (value != fieldName)
                {
                    fieldName = value;
                    OnPropertyChanged("FieldName");
                }
            }
        }

        private bool isDescending;
        public bool IsDescending
        {
            get { return isDescending; }
            set
            {
                if (value != isDescending)
                {
                    isDescending = value;
                    OnPropertyChanged("IsDescending");
                }
            }
        }

        protected virtual void OnPropertyChanged(String propertyName)
        {
            if (_propertyChanged != null)
            {
                _propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _propertyChanged += value; }
            remove { _propertyChanged -= value; }
        }
        private event PropertyChangedEventHandler _propertyChanged;
    }

}
