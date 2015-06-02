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
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace MITD.Presentation.UI
{
    public class DataGridEx : DataGrid
    {
        public class SortData : INotifyPropertyChanged
        {
            private DataGridEx parentDataGrid;
            private string displayName;
            
            public SortData(DataGridEx parentDataGrid)
            {
                this.parentDataGrid = parentDataGrid;
            }

            public string DisplayName
            {
                get { return displayName; }
                set
                {
                    if (value != displayName)
                    {
                        displayName = value;
                        OnPropertyChanged("DisplayName");
                    }
                }
            }

            private BitmapImage sortIcon;
            public BitmapImage SortIcon
            {
                get { return sortIcon; }
                set
                {
                    if (value != sortIcon)
                    {
                        sortIcon = value;
                        OnPropertyChanged("SortIcon");
                    }
                }
            }


            private SortInfo sortInfo;
            public SortInfo SortInfo
            {
                get { return sortInfo; }
                set
                {
                    if (value != sortInfo)
                    {
                        sortInfo = value;
                        OnPropertyChanged("SortInfo");
                    }
                }
            }

            private ICommand sortCommand;
            public ICommand SortCommand
            {
                get
                {
                    if (sortCommand == null)
                    {
                        sortCommand = new DelegateCommand(
                            () => parentDataGrid.UpdateSorting(this));
                    }
                    return sortCommand;
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
        private List<SortData> SortingDataList = new List<SortData>();
        private BitmapImage upIcon;
        private BitmapImage downIcon;

        private bool prepared = false;
        public DataGridEx():base()
        {
            upIcon = new BitmapImage(
                    new Uri("1uparrow.png", UriKind.Relative));
            downIcon = new BitmapImage(
                    new Uri("1downarrow.png", UriKind.Relative));
            
            Loaded += (s, args) =>
                {
                    if (prepared) return;
                    PrepareSortDataList();
                    updateSortInfo(CurrentSortInfo);
                    prepared = true;
                };
        }

        void PrepareSortDataList()
        {
            foreach (var c in Columns)
            {
                var sd = new SortData(this)
                {
                    SortInfo = new SortInfo
                    {
                        FieldName = (c as DataGridTextColumn).Binding.Path.Path,
                        IsDescending = false,
                    },
                    SortIcon = null,
                    DisplayName = (c as DataGridTextColumn).Header as string,
                };
                SortingDataList.Add(sd);
                c.Header = sd;
                c.HeaderStyle = (Style)Application.Current.Resources["myColHeader"];
            }
        }

        public static readonly DependencyProperty CurrentSortOrderInfoProperty =
            DependencyProperty.Register(
            "CurrentSortInfo", typeof(SortInfo),
            typeof(DataGridEx), new PropertyMetadata(CurrentSortOrderInfoPropertyChanged)
            );
        public SortInfo CurrentSortInfo
        {
            get { return (SortInfo)GetValue(CurrentSortOrderInfoProperty); }
            set { SetValue(CurrentSortOrderInfoProperty, value); }
        }

        private static void CurrentSortOrderInfoPropertyChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as DataGridEx).updateSortInfo((SortInfo)args.NewValue);
        }

        private void updateSortInfo(SortInfo sortInfo)
        {
            if (sortInfo == null) return;
            var sd = SortingDataList.FirstOrDefault(s => s.SortInfo.FieldName == sortInfo.FieldName);
            updateSortDataList(sd);
        }
        private void updateSortDataList(SortData sd)
        {
            if (sd == null) return;
            if (currentSortOrderData == sd)
            {
                sd.SortInfo.IsDescending = !sd.SortInfo.IsDescending;
            }
            else
            {
                if (currentSortOrderData != null)
                {
                    currentSortOrderData.SortIcon = null;
                    currentSortOrderData.SortInfo.IsDescending = false;
                }
                currentSortOrderData = sd;
            }
            currentSortOrderData.SortIcon = currentSortOrderData.SortInfo.IsDescending ? downIcon : upIcon;
        }

        private SortData currentSortOrderData;

        public static readonly DependencyProperty SortCommandProperty =
            DependencyProperty.Register(
            "SortCommand", typeof(ICommand),
            typeof(DataGridEx), null
            );
        public ICommand SortCommand
        {
            get { return (ICommand)GetValue(SortCommandProperty); }
            set { SetValue(SortCommandProperty, value); }
        }

        public void UpdateSorting(SortData sd)
        {
            if (CurrentSortInfo == sd.SortInfo)
                updateSortDataList(sd);
            else
                CurrentSortInfo = sd.SortInfo;

            if (SortCommand != null)
                SortCommand.Execute(sd.SortInfo);
        }
    }
}
