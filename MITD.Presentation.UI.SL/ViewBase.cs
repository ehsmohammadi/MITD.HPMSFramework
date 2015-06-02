using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using MITD.Presentation.UI;
using MITD.Presentation.UI.DateTimePicker.Views;

namespace MITD.Presentation.UI
{
    public class ViewBase : UserControl, IView
    {
        public void ForceValidation(UIElement element)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = (UIElement)VisualTreeHelper.GetChild(element, i);
                ForceValidation(child);
            }

            BindingExpression bindingExpression = null;

            string uiElementType = element.GetType().ToString();
            switch (uiElementType)
            {
                case "System.Windows.Controls.TextBox":
                    bindingExpression = ((TextBox)element).GetBindingExpression(TextBox.TextProperty);
                    break;

                case "System.Windows.Controls.RadioButton":
                    bindingExpression = ((RadioButton)element).GetBindingExpression(RadioButton.IsCheckedProperty);
                    break;
                case "SIC.Presentation.UI.ComboBoxEx":
                    bindingExpression = ((ComboBoxEx)element).GetBindingExpression(ComboBoxEx.SelectedValueProperProperty);
                    break;
                case "SIC.Presentation.UI.DateTimePicker.Views.PDatePicker":
                    bindingExpression = ((PDatePicker)element).GetBindingExpression(PDatePicker.SelectedDateProperty);
                    break;
            }

            if (bindingExpression == null || bindingExpression.ParentBinding == null) return;
            //if (!bindingExpression.ParentBinding.ValidatesOnExceptions) return;

            bindingExpression.UpdateSource();
        }    

        public WorkspaceViewModel ViewModel
        {
            get
            {
                WorkspaceViewModel vm = null;
                if(DataContext!=null)
                {
                    vm = DataContext as WorkspaceViewModel;
                }
                return vm;
            }
            set
            {
                if (DataContext != value)
                {
                    DataContext = value;
                    if (value != null)
                    {
                        value.View = this;                     
                    }
                }
            }
        }

    }
}
