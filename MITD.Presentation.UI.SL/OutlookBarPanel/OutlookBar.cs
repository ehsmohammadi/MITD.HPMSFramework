using System.Windows.Controls;
using System.Windows;
using System;
using System.Windows.Input;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Specialized;
using System.Windows.Controls.Primitives;
using System.Reflection;
using MITD.Presentation.UI.OutlookBarPanel;

//Much of the code here is taken directly from the Silverlight Toolkit TabControl

namespace MITD.Presentation.UI
{
    [TemplatePart(Name = OutlookBar.OutlookBarFooterTrayStackPanelName, Type = typeof(StackPanel))]
    [TemplatePart(Name = OutlookBar.OutlookBarButtonsPresenterName, Type = typeof(ItemsPresenter))]
    [TemplatePart(Name = OutlookBar.OutlookBarContentPresenterName, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = OutlookBar.OutlookToggleButtonName, Type = typeof(ToggleButton))]
    [TemplatePart(Name = OutlookBar.OutlookBarGridSplitterName, Type = typeof(OutlookBarGridSplitter))]
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateExpanded, GroupName = VisualStates.GroupExpansion)]
    [TemplateVisualState(Name = VisualStates.StateCollapsed, GroupName = VisualStates.GroupExpansion)]
    public class OutlookBar : HeaderedItemsControl
    {

        #region Fields

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private int _desiredIndex;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private bool _updateIndex = true;

        /// <summary>
        /// Used to hold the current width of the OutlookBar control before it is
        /// collapsed to a sidebar
        /// </summary>
        private double _normalWidth;

        /// <summary>
        /// Gets or sets the ContentHost of the OutlookBar.
        /// </summary>
        internal ContentPresenter OutlookBarContentPresenter { get; set; }

        /// <summary>
        /// Used to hold the name of the OutlookBar ContentPresenter
        /// </summary>
        internal const string OutlookBarContentPresenterName = "theOutlookBarContentPresenter";

        /// <summary>
        /// Gets or sets the OutlookBarButtonsItemsPresenter of the OutlookBar.
        /// </summary>
        internal ItemsPresenter OutlookBarButtonsPresenter { get; set; }

        /// <summary>
        /// Used to hold the name of the OutlookBar ItemsPresenter
        /// </summary>
        internal const string OutlookBarButtonsPresenterName = "theOutlookBarButtonsPresenter";


        internal ToggleButton OutlookToggleButton { get; set; }

        /// <summary>
        /// Used to hold the name of the OutlookBar ToggleButton
        /// </summary>
        internal const string OutlookToggleButtonName = "theOutlookToggleButton";


        internal OutlookBarGridSplitter LocalOutlookBarGridSplitter { get; set; }

        /// <summary>
        /// Used to hold the name of the OutlookBar GridSplitter
        /// </summary>
        internal const string OutlookBarGridSplitterName = "theOutlookBarGridSplitter";


        internal StackPanel OutlookBarFooterTrayStackPanel { get; set; }

        /// <summary>
        /// Used to hold the name of the OutlookBar GridSplitter
        /// </summary>
        internal const string OutlookBarFooterTrayStackPanelName = "theOutlookBarFooterTrayStackPanel";


        #endregion Fields


        #region Constructors

        public OutlookBar()
        {
            DefaultStyleKey = typeof(OutlookBar);    
        }

        #endregion Constructors


        #region Template

        /// <summary>
        /// Builds the visual tree for the OutlookBar when a new
        /// template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.OutlookToggleButton = base.GetTemplateChild(OutlookBar.OutlookToggleButtonName) as ToggleButton;
            this.OutlookToggleButton.Click += new RoutedEventHandler(OutlookToggleButton_Click);

            this.OutlookBarButtonsPresenter = base.GetTemplateChild(OutlookBar.OutlookBarButtonsPresenterName) as ItemsPresenter;

            this.OutlookBarFooterTrayStackPanel = base.GetTemplateChild(OutlookBar.OutlookBarFooterTrayStackPanelName) as StackPanel;

            this.LocalOutlookBarGridSplitter = base.GetTemplateChild(OutlookBar.OutlookBarGridSplitterName) as OutlookBarGridSplitter;
            this.LocalOutlookBarGridSplitter.LostMouseCapture += new MouseEventHandler(LocalOutlookBarGridSplitter_LostMouseCapture);
            

            ChangeVisualState(false);
        }

        #endregion Template


        #region ButtonsOnShow

        void LocalOutlookBarGridSplitter_LostMouseCapture(object sender, MouseEventArgs e)
        {
            int numberofOutlookBarButtons = this.Items.Count;
            int numberofOutlookBarButtonsOnShow = GetButtonsOnShowCount();
            int numberofHiddenOutlookBarButtons = numberofOutlookBarButtons - numberofOutlookBarButtonsOnShow;

            this.OutlookBarFooterTrayStackPanel.Children.Clear();

            if (numberofOutlookBarButtons > numberofOutlookBarButtonsOnShow)
            {
                for (int i = numberofHiddenOutlookBarButtons; i > 0; i--)
                {
                    OutlookBarButton outlookBarButton = this.Items[numberofOutlookBarButtons - i] as OutlookBarButton;
                    AddToFooterTray(outlookBarButton);
                }
            }
        }

        private void AddToFooterTray(OutlookBarButton OutlookBarButton)
        {
            OutlookBarFooterTrayButton footerTrayButton = new OutlookBarFooterTrayButton();
            Image image = new Image();
            image.Source = OutlookBarButton.HeaderImage;
            footerTrayButton.Content = image;
            footerTrayButton.Tag = OutlookBarButton;
            footerTrayButton.Click += new RoutedEventHandler(footerTrayButton_Click);

            this.OutlookBarFooterTrayStackPanel.Children.Add(footerTrayButton);
        }

        void footerTrayButton_Click(object sender, RoutedEventArgs e)
        {
            this.SelectedItem = ((OutlookBarFooterTrayButton)sender).Tag;
        }


        private int GetButtonsOnShowCount()
        {
            double outlookBarButtonHeight = 30.0;
            double outlookGridSplitterHeight = 10.0;
            double outlookBarButtonsPresenterHeight = this.OutlookBarButtonsPresenter.ActualHeight - outlookGridSplitterHeight;
            return (int)Math.Round(outlookBarButtonsPresenterHeight / outlookBarButtonHeight);
        }


        #endregion ButtonsOnShow


        #region SelectedItem
        /// <summary>
        /// Gets or sets the currently selected OutlookBarButton.
        /// </summary>
        /// <value>
        /// The currently selected OutlookBarButton, or null if an OutlookBarButton is not selected.
        /// </value>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set
            {
                if (value == null || Items.Contains(value))
                {
                    SetValue(SelectedItemProperty, value);
                }
            }
        }

        /// <summary>
        /// Identifies the OutlookBar.SelectedItem dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the OutlookBar.SelectedItem dependency property.
        /// </value>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                "SelectedItem",
                typeof(object),
                typeof(OutlookBar),
                new PropertyMetadata(OnSelectedItemChanged));

        /// <summary>
        /// SelectedItem property changed handler.
        /// </summary>
        /// <param name="d">OutlookBar that changed its SelectedItem.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OutlookBar outlookBar = (OutlookBar)d;

            OutlookBarButton oldOutlookBarButton = e.OldValue as OutlookBarButton;
            OutlookBarButton newOutlookBarButton = e.NewValue as OutlookBarButton;
            
            // if you select an item not in the OutlookBar keep the old
            // selection
            int index = (newOutlookBarButton == null ? -1 : outlookBar.Items.IndexOf(newOutlookBarButton));
            if (newOutlookBarButton != null && index == -1)
            {
                outlookBar.SelectedItem = oldOutlookBarButton;
                return;
            }
            else
            {
                outlookBar.SelectedIndex = index;
                outlookBar.SelectItem(oldOutlookBarButton, newOutlookBarButton);
            }
        }
        #endregion SelectedItem
        

        #region SelectedIndex
        /// <summary>
        /// Gets or sets the index of the currently selected OutlookBarButton.
        /// </summary>
        /// <value>
        /// The index of the currently selected OutlookBarButton or -1 if an OutlookBarButton is not selected.
        /// </value>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// Identifies the OutlookBar.SelectedIndex dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the OutlookBar.SelectedIndex dependency property.
        /// </value>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(
                "SelectedIndex",
                typeof(int),
                typeof(OutlookBar),
                new PropertyMetadata(OnSelectedIndexChanged));

        /// <summary>
        /// SelectedIndex property changed handler.
        /// </summary>
        /// <param name="d">OutlookBar that changed its SelectedIndex.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OutlookBar outlookBar = (OutlookBar)d;

            int newIndex = (int)e.NewValue;
            int oldIndex = (int)e.OldValue;

            if (newIndex < -1)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Properties.ApplicationStrings.OutlookBar_InvalidIndex, newIndex.ToString(CultureInfo.CurrentCulture)));
            }

            // Coercion Workaround
            // -------------------
            if (outlookBar._updateIndex)
            {
                outlookBar._desiredIndex = newIndex;
            }
            else if (!outlookBar._updateIndex)
            {
                outlookBar._updateIndex = true;
            }
            if (newIndex >= outlookBar.Items.Count)
            {
                outlookBar._updateIndex = false;
                outlookBar.SelectedIndex = oldIndex;
                return;
            }
            // -------------------

            OutlookBarButton item = outlookBar.GetItemAtIndex(newIndex);
            if (outlookBar.SelectedItem != item)
            {
                outlookBar.SelectedItem = item;
            }
        }

        internal ContentPresenter OutlookBarContent { get; set; }

        /// <summary>
        /// Given the OutlookBarButton in the list of Items, we will set that item as the
        /// currently selected item, and un-select the rest of the items.
        /// </summary>
        /// <param name="oldItem">Previous OutlookBarButton that was unselected.</param>
        /// <param name="newItem">New OutlookBarButton to set as the SelectedItem.</param>
        private void SelectItem(OutlookBarButton oldItem, OutlookBarButton newItem)
        {
            if (newItem == null)
            {
                ContentPresenter contentHost = OutlookBarContentPresenter;
                if (contentHost != null)
                {
                    contentHost.Content = null;
                }
                SetValue(SelectedContentProperty, null);
            }

            foreach (object item in Items)
            {
                OutlookBarButton outlookBarButton = item as OutlookBarButton;
                if (outlookBarButton == null)
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Properties.ApplicationStrings.OutlookBar_InvalidChild, item.GetType().ToString()));
                }
                if (outlookBarButton != newItem && outlookBarButton.IsSelected)
                {
                    outlookBarButton.IsSelected = false;
                }
                else if (outlookBarButton == newItem)
                {
                    outlookBarButton.IsSelected = true;
                    this.Header = outlookBarButton.Header;
                    SetValue(SelectedContentProperty, outlookBarButton.Content);
                }
            }


            // Fire SelectionChanged Event
            SelectionChangedEventHandler handler = SelectionChanged;
            if (handler != null)
            {
                SelectionChangedEventArgs args = new SelectionChangedEventArgs(
                    (oldItem == null ? new List<OutlookBarButton> { } : new List<OutlookBarButton> { oldItem }),
                    (newItem == null ? new List<OutlookBarButton> { } : new List<OutlookBarButton> { newItem }));
                handler(this, args);
            }

        }
        #endregion SelectedIndex


        #region SelectedContent
        /// <summary>
        /// Gets the content of the currently selected OutlookBarButton 
        /// </summary>
        /// <value>
        /// The content of the currently selected OutlookBarButton. The default is null.
        /// </value>
        public object SelectedContent
        {
            get { return GetValue(SelectedContentProperty); }
            internal set { SetValue(SelectedContentProperty, value); }
        }

        /// <summary>
        /// Identifies the OutlookBar.SelectedContent dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the OutlookBar.SelectedContent dependency property.
        /// </value>
        public static readonly DependencyProperty SelectedContentProperty =
            DependencyProperty.Register(
                "SelectedContent",
                typeof(object),
                typeof(OutlookBar),
                new PropertyMetadata(OnSelectedContentChanged));

        /// <summary>
        /// SelectedContent property changed handler.
        /// </summary>
        /// <param name="d">OutlookBar that changed its SelectedContent.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnSelectedContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OutlookBar outlookBar = (OutlookBar)d;

            outlookBar.UpdateSelectedContent(e.NewValue);
        }
        #endregion SelectedContent


        #region IsExpanded
        /// <summary>
        /// Gets a value indicating whether this element is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        /// <summary>
        /// Identifies the OutlookBar.IsExpanded dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the OutlookBar.IsExpanded dependency property.
        /// </value>
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(
                "IsExpanded",
                typeof(bool),
                typeof(OutlookBar),
                new PropertyMetadata(OnIsExpandedPropertyChanged));

        /// <summary>
        /// IsExpandedProperty property changed handler.
        /// </summary>
        /// <param name="d">OutlookBar that changed IsExpanded.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnIsExpandedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OutlookBar outlookBar = d as OutlookBar;

            foreach (var item in outlookBar.Items)
            {
                OutlookBarButton outlookBarButton = item as OutlookBarButton;
                outlookBarButton.IsExpanded = outlookBar.IsExpanded;
            }

            outlookBar.OnIsExpandedChanged(e);

        }


        /// <summary>
        /// Called when the IsExpanded property changes.
        /// </summary>
        /// <param name="e">
        /// A <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />
        /// that contains the event data.
        /// </param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Compat with WPF.")]
        protected virtual void OnIsExpandedChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateVisualState();
        }

        /// <summary>
        /// Called when the OulookToggleButton is clicked
        /// </summary>
        /// <param name="d">OutlookBar whose OutlookToggleButton was clicked.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        void OutlookToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)this.OutlookToggleButton.IsChecked)
            {
                this.IsExpanded = false;
                _normalWidth = this.Width;
                this.Width = 34;
            }
            else
            {
                this.IsExpanded = true;
                this.Width = _normalWidth;
            }

        }

        #endregion IsExpanded


        #region SelectionChanges
        /// <summary>
        /// Updates the current selection when Items has changed.
        /// </summary>
        /// <param name="e">Data used by the event.</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Need to handle Add/Remove/Replace")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Compat with WPF.")]
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    int newSelectedIndex = -1;
                    foreach (object o in e.NewItems)
                    {
                        OutlookBarButton outlookBarButton = o as OutlookBarButton;
                        if (outlookBarButton == null)
                        {
                            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Properties.ApplicationStrings.OutlookBar_InvalidChild, o.GetType().ToString()));
                        }
                        int index = Items.IndexOf(outlookBarButton);

                        // If we are adding a selected item
                        if (outlookBarButton.IsSelected)
                        {
                            newSelectedIndex = index;
                        }
                        else if (SelectedItem != GetItemAtIndex(SelectedIndex))
                        {
                            newSelectedIndex = Items.IndexOf(SelectedItem);
                        }
                        else if ((_desiredIndex < Items.Count) && (_desiredIndex >= 0))
                        {
                            // Coercion Workaround
                            newSelectedIndex = _desiredIndex;
                        }
                        outlookBarButton.UpdateVisualState();
                    }

                    if (newSelectedIndex == -1)
                    {
                        // If we are adding many items through xaml, one could
                        // already be specified as selected. If so, we don't
                        // want to override the value.
                        foreach (object item in Items)
                        {
                            OutlookBarButton outlookBarButton = item as OutlookBarButton;
                            if (outlookBarButton == null)
                            {
                                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Properties.ApplicationStrings.OutlookBar_InvalidChild, item.GetType().ToString()));
                            }

                            if (outlookBarButton.IsSelected)
                            {
                                return;
                            }
                        }

                        // To follow WPF behavior, we only select the item if
                        // the user has not explicitly set the IsSelected field
                        // to false, or if there are 2 or more items in the
                        // OutlookBar.
                        if (Items.Count > 1 || ((Items[0] as OutlookBarButton).ReadLocalValue(OutlookBarButton.IsSelectedProperty) as bool?) != false)
                        {
                            newSelectedIndex = 0;
                        }
                    }

                    // When we add a new item into the selected position, 
                    // SelectedIndex does not change, so we need to update both
                    // the SelectedItem and the SelectedIndex.
                    SelectedItem = GetItemAtIndex(newSelectedIndex);
                    SelectedIndex = newSelectedIndex;
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object o in e.OldItems)
                    {
                        // if there are no items, the selected index is set to
                        // -1
                        if (Items.Count == 0)
                        {
                            SelectedIndex = -1;
                        }
                        else if (Items.Count <= SelectedIndex)
                        {
                            SelectedIndex = Items.Count - 1;
                        }
                        else
                        {
                            SelectedItem = GetItemAtIndex(SelectedIndex);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    SelectedIndex = -1;

                    // For Setting the ItemsSource
                    foreach (object item in Items)
                    {
                        OutlookBarButton outlookBarButton = item as OutlookBarButton;
                        if (outlookBarButton == null)
                        {
                            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Properties.ApplicationStrings.OutlookBar_InvalidChild, item.GetType().ToString()));
                        }
                        if (outlookBarButton.IsSelected)
                        {
                            SelectedItem = outlookBarButton;
                        }
                    }
                    if (SelectedIndex == -1 && Items.Count > 0)
                    {
                        SelectedIndex = 0;
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    break;
            }
        }

        /// <summary>
        /// Occurs when the selected OutlookBarButton changes.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Raises the OutlookBar.SelectionChanged event.
        /// </summary>
        /// <param name="args">
        /// Provides data for the OutlookBar.SelectionChanged event.
        /// </param>
        protected virtual void OnSelectionChanged(SelectionChangedEventArgs args)
        {
        }

        /// <summary>
        /// This is the method that responds to the KeyDown event.
        /// </summary>
        /// <param name="e">Data used by the event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Handled)
            {
                return;
            }
            OutlookBarButton nextOutlookBarButton = null;

            int direction = 0;
            int startIndex = -1;
            switch (e.Key)
            {
                case Key.Home:
                    direction = 1;
                    startIndex = -1;
                    break;
                case Key.End:
                    direction = -1;
                    startIndex = Items.Count;
                    break;
                default:
                    return;
            }

            nextOutlookBarButton = FindNextOutlookBarButton(startIndex, direction);

            if (nextOutlookBarButton != null && nextOutlookBarButton != SelectedItem)
            {
                e.Handled = true;
                SelectedItem = nextOutlookBarButton;
                nextOutlookBarButton.Focus();
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="startIndex">Inherited code: Requires comment 1.</param>
        /// <param name="direction">Inherited code: Requires comment 2.</param>
        /// <returns>Inherited code: Requires comment 3.</returns>
        internal OutlookBarButton FindNextOutlookBarButton(int startIndex, int direction)
        {
            OutlookBarButton nextOutlookBarButton = null;

            int index = startIndex;
            for (int i = 0; i < Items.Count; i++)
            {
                index += direction;
                if (index >= Items.Count)
                {
                    index = 0;
                }
                else if (index < 0)
                {
                    index = Items.Count - 1;
                }

                OutlookBarButton outlookBarButton = GetItemAtIndex(index);
                if (outlookBarButton != null && outlookBarButton.IsEnabled && outlookBarButton.Visibility == Visibility.Visible)
                {
                    nextOutlookBarButton = outlookBarButton;
                    break;
                }
            }
            return nextOutlookBarButton;
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="index">Inherited code: Requires comment 1.</param>
        /// <returns>Inherited code: Requires comment 2.</returns>
        private OutlookBarButton GetItemAtIndex(int index)
        {
            if (index < 0 || index >= Items.Count)
            {
                return null;
            }
            else
            {
                return Items[index] as OutlookBarButton;
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <param name="content">Inherited code: Requires comment 1.</param>
        private void UpdateSelectedContent(object content)
        {
            OutlookBarButton outlookBarButton = SelectedItem as OutlookBarButton;
            if (outlookBarButton != null)
            {
                ContentPresenter contentHost = OutlookBarContentPresenter;
                if (contentHost != null)
                {
                    contentHost.HorizontalAlignment = outlookBarButton.HorizontalContentAlignment;
                    contentHost.VerticalAlignment = outlookBarButton.VerticalContentAlignment;
                    contentHost.ContentTemplate = outlookBarButton.ContentTemplate;
                    contentHost.Content = content;
                }
            }
        }

        #endregion SelectionChanges


        #region StateChanges
        /// <summary>
        /// Called when the IsEnabled property changes.
        /// </summary>
        /// <param name="sender">
        /// Control that triggers this property change.
        /// </param>
        /// <param name="e">Property changed args.</param>
        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateVisualState();
        }


        /// <summary>
        /// Update the current visual state of the OutlookBar.
        /// </summary>
        internal void UpdateVisualState()
        {
            ChangeVisualState(true);
        }

        /// <summary>
        /// Change to the correct visual state for the OutlookBar.
        /// </summary>
        /// <param name="useTransitions">
        /// True to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.
        /// </param>
        internal void ChangeVisualState(bool useTransitions)
        {
            // Update the FocusStates group
            if (!IsEnabled)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateDisabled, VisualStates.StateNormal);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateNormal);
            }

            // Update the ExpandedStates group
            if (IsExpanded)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateExpanded);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateCollapsed);
            }
        }

        #endregion StateChanges

    }


}
