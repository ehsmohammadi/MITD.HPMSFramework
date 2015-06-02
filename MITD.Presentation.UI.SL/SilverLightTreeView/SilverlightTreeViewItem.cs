using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;

namespace MITD.Presentation.UI
{
    public class SilverlightTreeViewItem : TreeViewItem
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            SilverlightTreeViewItem tvi = new SilverlightTreeViewItem();
            Binding expandedBinding = new Binding("IsExpanded");
            expandedBinding.Mode = BindingMode.TwoWay;
            tvi.SetBinding(SilverlightTreeViewItem.IsExpandedProperty, expandedBinding);
            Binding selectedBinding = new Binding("IsSelected");
            selectedBinding.Mode = BindingMode.TwoWay;
            tvi.SetBinding(SilverlightTreeViewItem.IsSelectedProperty, selectedBinding);
            return tvi;
        }
    }
}
