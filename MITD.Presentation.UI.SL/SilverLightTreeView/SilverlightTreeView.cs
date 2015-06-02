using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;

namespace MITD.Presentation.UI
{
    public class SilverlightTreeView : TreeView
    {
        public SilverlightTreeView()
        {
            this.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(TreeViewEx_SelectedItemChanged);
        }
        void TreeViewEx_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.SelectedItem = e.NewValue;
        }

        public new object SelectedItem
        {
            get { return this.GetValue(SilverlightTreeView.SelectedItemProperty); }
            set { this.SetValue(SilverlightTreeView.SelectedItemProperty, value); }
        }

        public new static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(SilverlightTreeView), new PropertyMetadata(SelectedItemProperty_Changed));

        static void SelectedItemProperty_Changed(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            SilverlightTreeView targetObject = dependencyObject as SilverlightTreeView;
            if (targetObject != null)
            {
                TreeViewItem tvi = targetObject.FindItemNode(targetObject.SelectedItem) as TreeViewItem;
                if (tvi != null)
                    tvi.IsSelected = true;
            }
        }

        public TreeViewItem FindItemNode(object item)
        {
            TreeViewItem node = null;
            foreach (object data in this.Items)
            {
                node = this.ItemContainerGenerator.ContainerFromItem(data) as TreeViewItem;
                if (node != null)
                {
                    if (data == item)
                        break;
                    node = FindItemNodeInChildren(node, item);
                    if (node != null)
                        break;
                }
            }
            return node;
        }

        protected TreeViewItem FindItemNodeInChildren(TreeViewItem parent, object item)
        {
            TreeViewItem node = null;
            bool isExpanded = parent.IsExpanded;
            if (!isExpanded) //Can't find child container unless the parent node is Expanded once
            {
                parent.IsExpanded = true;
                parent.UpdateLayout();
            }
            foreach (object data in parent.Items)
            {
                node = parent.ItemContainerGenerator.ContainerFromItem(data) as TreeViewItem;
                if (data == item && node != null)
                    break;
                node = FindItemNodeInChildren(node, item);
                if (node != null)
                    break;
            }
            if (node == null && parent.IsExpanded != isExpanded)
                parent.IsExpanded = isExpanded;
            if (node != null)
                parent.IsExpanded = true;
            return node;
        }


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
