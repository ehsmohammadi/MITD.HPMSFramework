using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Diagnostics.CodeAnalysis;
using MITD.Presentation.UI.OutlookBarPanel;

//Much of the code here is taken directly from the Silverlight Toolkit TabItem

namespace MITD.Presentation.UI
{
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateUnselected, GroupName = VisualStates.GroupSelection)]
    [TemplateVisualState(Name = VisualStates.StateSelected, GroupName = VisualStates.GroupSelection)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateExpanded, GroupName = VisualStates.GroupExpansion)]
    [TemplateVisualState(Name = VisualStates.StateCollapsed, GroupName = VisualStates.GroupExpansion)]
    public partial class OutlookBarButton : ContentControl
    {

        #region Fields

        /// <summary>
        /// Gets or sets a value indicating whether Inherited code: Requires comment.
        /// </summary>
        private bool _isMouseOver { get; set; }


        #endregion Fields


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the OutlookBarButton.
        /// </summary>
        public OutlookBarButton()
            : base()
        {
            MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
            MouseEnter += new MouseEventHandler(OnMouseEnter);
            MouseLeave += new MouseEventHandler(OnMouseLeave);
            GotFocus += delegate { IsFocused = true; };
            LostFocus += delegate { IsFocused = false; };
            IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
            DefaultStyleKey = typeof(OutlookBarButton);
        }

        #endregion Constructors

        #region Events

        public event DependencyPropertyChangedEventHandler Selected;

        #endregion Events


        #region Templates

        /// <summary>
        /// Builds the visual tree for the OutlookBarButton when a new template
        /// is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Load Header
            UpdateHeaderVisuals();

            ChangeVisualState(false);
        }

        #endregion Templates


        #region Header

        /// <summary>
        /// Gets or sets the header of the OutlookBarButton" />.
        /// </summary>
        /// <value>
        /// The current header of the OutlookBarButton" />.
        /// </value>
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Identifies the OutlookBarButton.Header dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the OutlookBarButton.Header dependency property.
        /// </value>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
                "Header",
                typeof(object),
                typeof(OutlookBarButton),
                new PropertyMetadata(OnHeaderChanged));

        /// <summary>
        /// Header property changed handler.
        /// </summary>
        /// <param name="d">OutlookBarButton that changed its Header.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OutlookBarButton outlookBarButton = (OutlookBarButton)d;

            outlookBarButton.HasHeader = (e.NewValue != null) ? true : false;
            outlookBarButton.OnHeaderChanged(e.OldValue, e.NewValue);
        }

        /// <summary>
        /// Called when the OutlookBarButton.Header property changes.
        /// </summary>
        /// <param name="oldHeader">
        /// The previous value of the OutlookBarButton.Header property.
        /// </param>
        /// <param name="newHeader">
        /// The new value of the OutlookBarButton.Header property.
        /// </param>
        protected virtual void OnHeaderChanged(object oldHeader, object newHeader)
        {
            UpdateHeaderVisuals();
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void UpdateHeaderVisuals()
        {
            ContentPresenter headerContentPresenter = OutlookBarButtonContentPresenter;
            if (headerContentPresenter != null)
            {
                headerContentPresenter.Content = this.Header;
                headerContentPresenter.ContentTemplate = this.HeaderTemplate;
            }
        }

        /// <summary>
        /// Gets or sets the ContentHost of the OutlookBarButton.
        /// </summary>
        internal ContentPresenter OutlookBarButtonContentPresenter { get; set; }


        #endregion Header


        #region HasHeader

        /// <summary>
        /// Gets a value indicating whether the OutlookBarButton has a header.
        /// </summary>
        /// <value>
        /// True if OutlookBarButton.Header is not null; otherwise, false.
        /// </value>
        public bool HasHeader
        {
            get { return (bool)GetValue(HasHeaderProperty); }
            private set { SetValue(HasHeaderProperty, value); }
        }

        /// <summary>
        /// Identifies the OutlookBarButton.HasHeader dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the OutlookBarButton.HasHeader dependency property.
        /// </value>
        public static readonly DependencyProperty HasHeaderProperty =
            DependencyProperty.Register(
                "HasHeader",
                typeof(bool),
                typeof(OutlookBarButton),
                null);

        #endregion HasHeader


        #region HeaderTemplate

        /// <summary>
        /// Gets or sets the template that is used to display the content of the
        /// OutlookBarButton header.
        /// </summary>
        /// <value>
        /// The current template that is used to display OutlookBarButton header content.
        /// </value>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the OutlookBarButton.HeaderTemplate dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the OutlookBarButton.HeaderTemplate dependency property.
        /// </value>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(
                "HeaderTemplate",
                typeof(DataTemplate),
                typeof(OutlookBarButton),
                new PropertyMetadata(OnHeaderTemplateChanged));

        /// <summary>
        /// HeaderTemplate property changed handler.
        /// </summary>
        /// <param name="d">OutlookBarButton that changed its HeaderTemplate.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OutlookBarButton outlookBarButton = (OutlookBarButton)d;
            outlookBarButton.OnHeaderTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
        }

        /// <summary>
        /// Called when the OutlookBarButton.HeaderTemplate property changes.
        /// </summary>
        /// <param name="oldHeaderTemplate">
        /// The previous value of the OutlookBarButton.HeaderTemplate property.
        /// </param>
        /// <param name="newHeaderTemplate">
        /// The new value of the OutlookBarButton.HeaderTemplate property.
        /// </param>
        protected virtual void OnHeaderTemplateChanged(DataTemplate oldHeaderTemplate, DataTemplate newHeaderTemplate)
        {
            UpdateHeaderVisuals();
        }

        #endregion HeaderTemplate


        #region IsSelected

        /// <summary>
        /// Gets or sets a value indicating whether the OutlookBarButton is currently selected.
        /// </summary>
        /// <value>
        /// True if the OutlookBarButton is selected; otherwise, false.
        /// </value>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// Identifies the OutlookBarButton.IsSelected dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the OutlookBarButton.IsSelected dependency property.
        /// </value>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(
                "IsSelected",
                typeof(bool),
                typeof(OutlookBarButton),
                new PropertyMetadata(OnIsSelectedChanged));


        /// <summary>
        /// IsSelected changed handler.
        /// </summary>
        /// <param name="d">OutlookBarButton that changed IsSelected.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OutlookBarButton outlookBarButton = d as OutlookBarButton;

            bool isSelected = (bool)e.NewValue;

            RoutedEventArgs args = new RoutedEventArgs();

            if (isSelected)
            {
                outlookBarButton.OnSelected(args);
            }
            else
            {
                outlookBarButton.OnUnselected(args);
            }

            outlookBarButton.IsTabStop = isSelected;
            outlookBarButton.UpdateVisualState();

        }


        /// <summary>
        /// Called to indicate that the OutlookBarButton.IsSelected property has changed to true.
        /// </summary>
        /// <param name="e">
        /// A <see cref="T:System.Windows.RoutedEventArgs" /> that contains the
        /// event data.
        /// </param>
        protected virtual void OnSelected(RoutedEventArgs e)
        {
            if (OutlookBarButtonParent != null)
            {
                OutlookBarButtonParent.SelectedItem = this;
            }

            DependencyPropertyChangedEventArgs args = new DependencyPropertyChangedEventArgs();

            if (Selected != null)
                Selected(this, args);
        }

        /// <summary>
        /// Called to indicate that the OutlookBarButton.IsSelected has changed to false.
        /// </summary>
        /// <param name="e">
        /// A <see cref="T:System.Windows.RoutedEventArgs" /> that contains the
        /// event data.
        /// </param>
        protected virtual void OnUnselected(RoutedEventArgs e)
        {
            if (OutlookBarButtonParent != null && OutlookBarButtonParent.SelectedItem == this)
            {
                OutlookBarButtonParent.SelectedIndex = -1;
            }
        }

        #endregion IsSelected


        #region HeaderImage

        /// <summary>
        /// Gets or sets the HeaderImage ImageSource.
        /// </summary>
        public ImageSource HeaderImage
        {
            get { return (ImageSource)GetValue(HeaderImageProperty); }
            set { SetValue(HeaderImageProperty, value); }
        }

        /// <summary>
        /// Identifies the OutlookBarButton.IsSelected dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the OutlookBarButton.IsSelected dependency property.
        /// </value>
        public static readonly DependencyProperty HeaderImageProperty =
            DependencyProperty.Register(
                "HeaderImage",
                typeof(ImageSource),
                typeof(OutlookBarButton),
                new PropertyMetadata(OnHeaderImageChanged));

        /// <summary>
        /// IsSelected changed handler.
        /// </summary>
        /// <param name="d">OutlookBarButton that changed IsSelected.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnHeaderImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion HeaderImage


        #region ContentChanged

        /// <summary>
        /// This method is invoked when the Content property changes.
        /// </summary>
        /// <param name="oldContent">
        /// The previous OutlookBarButton content.
        /// </param>
        /// <param name="newContent">
        /// The new OutlookBarButton content.
        /// </param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            OutlookBar outlookBar = OutlookBarButtonParent;
            if (outlookBar != null)
            {
                // If this is the selected OutlookBarButton then we should update
                // OutlookBar.SelectedContent
                if (IsSelected)
                {
                    outlookBar.SelectedContent = newContent;
                }
            }
        }

        #endregion ContentChanged


        #region KeyBoard

        /// <summary>
        /// This is the method that responds to the KeyDown event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="T:System.Windows.Input.KeyEventArgs" /> that contains
        /// the event data.
        /// </param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Handled)
            {
                return;
            }

            // Some keys (e.g. Left/Right) need to be translated in RightToLeft mode
            Key invariantKey = InteractionHelper.GetLogicalKey(FlowDirection, e.Key);

            OutlookBarButton nextOutlookBarButton = null;

            int direction = 0;
            int startIndex = OutlookBarButtonParent.Items.IndexOf(this);
            switch (invariantKey)
            {
                case Key.Right:
                case Key.Down:
                    direction = 1;
                    break;
                case Key.Left:
                case Key.Up:
                    direction = -1;
                    break;
                default:
                    return;
            }

            nextOutlookBarButton = OutlookBarButtonParent.FindNextOutlookBarButton(startIndex, direction);

            if (nextOutlookBarButton != null && nextOutlookBarButton != OutlookBarButtonParent.SelectedItem)
            {
                e.Handled = true;
                OutlookBarButtonParent.SelectedItem = nextOutlookBarButton;
                nextOutlookBarButton.Focus();
            }
        }

        #endregion KeyBoard


        #region Enabled

        /// <summary>
        /// Called when the IsEnabled property changes.
        /// </summary>
        /// <param name="sender">
        /// Control that triggers this property change.
        /// </param>
        /// <param name="e">Property changed args.</param>
        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool isEnabled = (bool)e.NewValue;
            if (!isEnabled)
            {
                _isMouseOver = false;
            }

            UpdateVisualState();
        }

        #endregion Enabled


        #region IsFocused
        /// <summary>
        /// Gets a value indicating whether this element has logical focus.
        /// </summary>
        public bool IsFocused
        {
            get { return (bool)GetValue(IsFocusedProperty); }
            internal set { SetValue(IsFocusedProperty, value); }
        }

        /// <summary>
        /// Identifies the OutlookBarButton.IsFocused dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the OutlookBarButton.IsFocused dependency property.
        /// </value>
        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.Register(
                "IsFocused",
                typeof(bool),
                typeof(OutlookBarButton),
                new PropertyMetadata(OnIsFocusedPropertyChanged));

        /// <summary>
        /// IsFocusedProperty property changed handler.
        /// </summary>
        /// <param name="d">OutlookBarButton that changed IsFocused.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnIsFocusedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OutlookBarButton outlookBarButton = d as OutlookBarButton;

            outlookBarButton.OnIsFocusChanged(e);
        }

        /// <summary>
        /// Called when the IsFocused property changes.
        /// </summary>
        /// <param name="e">
        /// A <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />
        /// that contains the event data.
        /// </param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Compat with WPF.")]
        protected virtual void OnIsFocusChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateVisualState();
        }
        #endregion IsFocused


        #region IsExpanded
        /// <summary>
        /// Gets a value indicating whether this element is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            internal set { SetValue(IsExpandedProperty, value); }
        }

        /// <summary>
        /// Identifies the OutlookBarButton.IsExpanded dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the OutlookBarButton.IsExpanded dependency property.
        /// </value>
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(
                "IsExpanded",
                typeof(bool),
                typeof(OutlookBarButton),
                new PropertyMetadata(OnIsExpandedPropertyChanged));

        /// <summary>
        /// IsExpandedProperty property changed handler.
        /// </summary>
        /// <param name="d">OutlookBarButton that changed IsExpanded.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnIsExpandedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OutlookBarButton outlookBarButton = d as OutlookBarButton;

            outlookBarButton.OnIsExpandedChanged(e);
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
        #endregion IsExpanded


        #region StateChanges

        /// <summary>
        /// Change to the correct visual state for the OutlookBarButton.
        /// </summary>
        internal void UpdateVisualState()
        {
            ChangeVisualState(true);
        }

        /// <summary>
        /// Change to the correct visual state for the OutlookBarButton.
        /// </summary>
        /// <param name="useTransitions">
        /// True to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.
        /// </param>
        private void ChangeVisualState(bool useTransitions)
        {
            // Update the CommonStates group
            if (!IsEnabled || (OutlookBarButtonParent != null && !OutlookBarButtonParent.IsEnabled))
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateDisabled, VisualStates.StateNormal);
            }
            else if (_isMouseOver && !IsSelected)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateMouseOver, VisualStates.StateNormal);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateNormal);
            }

            // Update the SelectionStates group
            if (IsSelected)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateSelected, VisualStates.StateUnselected);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateUnselected);
            }

            // Update the FocusStates group
            if (IsFocused && IsEnabled)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateFocused, VisualStates.StateUnfocused);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateUnfocused);
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


        #region Mouse

        /// <summary>
        /// Handles when the mouse leaves the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The MouseEventArgs.</param>
        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            _isMouseOver = false;
            UpdateVisualState();
        }

        /// <summary>
        /// Handles when the mouse enters the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The MouseEventArgs.</param>
        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            _isMouseOver = true;
            UpdateVisualState();
        }

        /// <summary>
        /// Handles the mouse left button down.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The MouseButtonEventArgs.</param>
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsEnabled && OutlookBarButtonParent != null && !IsSelected && !e.Handled)
            {
                IsTabStop = true;
                e.Handled = Focus();
                OutlookBarButtonParent.SelectedIndex = OutlookBarButtonParent.Items.IndexOf(this);
            }
        }

        #endregion Mouse


        #region OutlookBar

        /// <summary>
        /// Gets a reference to the OutlookBar that holds this OutlookBarButton.
        /// </summary>
        private OutlookBar OutlookBarButtonParent
        {
            get
            {
                // We need this for when the OutlookBar/OutlookBarButton is not in the
                // visual tree yet.
                OutlookBar outlookBar = Parent as OutlookBar;
                if (outlookBar != null)
                {
                    return outlookBar;
                }

                return null;
            }
        }

        #endregion OutlookBar

    }
}

