using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using MITD.Presentation.UI.OutlookBarPanel;

//This code is from the Silverlight toolkit GridSpliiter. I have added a DragIncrement dependency property
//and made some changes to how DragDeltaEvents are handled

namespace MITD.Presentation.UI
{
    /// <summary>
    /// Represents a control that redistributes space between the rows of
    /// columns of a <see cref="T:System.Windows.Controls.Grid" /> control.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    [TemplatePart(Name = OutlookBarGridSplitter.ElementHorizontalTemplateName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = OutlookBarGridSplitter.ElementVerticalTemplateName, Type = typeof(FrameworkElement))]
    [TemplateVisualState(Name = VisualStates.StateNormal, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateMouseOver, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateDisabled, GroupName = VisualStates.GroupCommon)]
    [TemplateVisualState(Name = VisualStates.StateUnfocused, GroupName = VisualStates.GroupFocus)]
    [TemplateVisualState(Name = VisualStates.StateFocused, GroupName = VisualStates.GroupFocus)]
    [StyleTypedProperty(Property = "PreviewStyle", StyleTargetType = typeof(Control))]
    public class OutlookBarGridSplitter : Control
    {

        #region Fields

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementHorizontalTemplateName = "HorizontalTemplate";

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementVerticalTemplateName = "VerticalTemplate";

        /// <summary>
        /// Gets or sets Inherited code: Requires comment.
        /// </summary>
        internal FrameworkElement ElementHorizontalTemplateFrameworkElement { get; set; }

        /// <summary>
        /// Gets or sets Inherited code: Requires comment.
        /// </summary>
        internal FrameworkElement ElementVerticalTemplateFrameworkElement { get; set; }

        /// <summary>
        /// Gets or sets the resize data.  This is null unless a resize
        /// operation is in progress.
        /// </summary>
        internal ResizeData ResizeDataInternal { get; set; }

        /// <summary>
        /// Is Null until a resize operation is initiated with ShowsPreview ==
        /// True, then it persists for the life of the OutlookBarGridSplitter.
        /// </summary>
        private Canvas _previewLayer;

        /// <summary>
        /// Is initialized in the constructor.
        /// </summary>
        private OutlookBarGridSplitterDragValidator _outlookBarGridSplitterDragValidator;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private GridResizeDirection _currentGridResizeDirection = GridResizeDirection.Auto;

        /// <summary>
        /// Holds the state for whether the mouse is over the control or not.
        /// </summary>
        private bool _isMouseOver;


        /// <summary>
        /// Default increment parameter.
        /// </summary>
        private const double KeyboardIncrement = 10.0;

        #endregion Fields


        #region DragIncrement

        /// <summary>
        /// Identifies the
        /// <see cref="P:System.Windows.Controls.OutlookBarGridSplitter.DragIncrement" />
        /// dependency property.
        /// </summary>
        /// <value>
        /// An identifier for the
        /// <see cref="P:System.Windows.Controls.OutlookBarGridSplitter.DragIncrement" />
        /// dependency property.
        /// </value>
        public static readonly DependencyProperty DragIncrementProperty =
            DependencyProperty.Register(
                "DragIncrement",
                typeof(double),
                typeof(OutlookBarGridSplitter),
                null);

        /// <summary>
        /// Gets or sets a value indicating the minimum distance the
        /// <see cref="T:System.Windows.Controls.OutlookBarGridSplitter" /> must be dragged
        /// before it moves.
        /// </summary>
        public double DragIncrement
        {
            get { return (double)GetValue(OutlookBarGridSplitter.DragIncrementProperty); }
            set { SetValue(OutlookBarGridSplitter.DragIncrementProperty, value); }
        }

        #endregion DragIncrement


        #region Preview

        /// <summary>
        /// Identifies the
        /// <see cref="P:System.Windows.Controls.OutlookBarGridSplitter.ShowsPreview" />
        /// dependency property.
        /// </summary>
        /// <value>
        /// An identifier for the
        /// <see cref="P:System.Windows.Controls.OutlookBarGridSplitter.ShowsPreview" />
        /// dependency property.
        /// </value>
        public static readonly DependencyProperty ShowsPreviewProperty =
            DependencyProperty.Register(
                "ShowsPreview",
                typeof(bool),
                typeof(OutlookBarGridSplitter),
                null);

        /// <summary>
        /// Identifies the
        /// <see cref="P:System.Windows.Controls.OutlookBarGridSplitter.PreviewStyle" />
        /// dependency property.
        /// </summary>
        /// <value>
        /// An identifier for the
        /// <see cref="P:System.Windows.Controls.OutlookBarGridSplitter.PreviewStyle" />
        /// dependency property.
        /// </value>
        public static readonly DependencyProperty PreviewStyleProperty =
            DependencyProperty.Register(
                "PreviewStyle",
                typeof(Style),
                typeof(OutlookBarGridSplitter),
                null);


        /// <summary>
        /// Gets or sets a value indicating whether the
        /// <see cref="T:System.Windows.Controls.OutlookBarGridSplitter" /> displays a
        /// preview.
        /// </summary>
        /// <value>
        /// True if a preview is displayed; otherwise, false.
        /// </value>
        public bool ShowsPreview
        {
            get { return (bool)GetValue(OutlookBarGridSplitter.ShowsPreviewProperty); }
            set { SetValue(OutlookBarGridSplitter.ShowsPreviewProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:System.Windows.Style" /> that is used
        /// for previewing changes.
        /// </summary>
        /// <value>
        /// The style that is used to preview changes.
        /// </value>
        public Style PreviewStyle
        {
            get { return (Style)GetValue(OutlookBarGridSplitter.PreviewStyleProperty); }
            set { SetValue(OutlookBarGridSplitter.PreviewStyleProperty, value); }
        }


        /// <summary>
        /// Creates the preview layer and adds it to the parent grid.
        /// </summary>
        /// <param name="parentGrid">Grid to add the preview layer to.</param>
        private void CreatePreviewLayer(Grid parentGrid)
        {
            Debug.Assert(parentGrid != null, "parentGrid should not be null!");
            Debug.Assert(parentGrid.RowDefinitions != null, "parentGrid.RowDefinitions should not be null!");
            Debug.Assert(parentGrid.ColumnDefinitions != null, "parentGrid.ColumnDefinitions should not be null!");

            _previewLayer = new Canvas();

            // RowSpan and ColumnSpan default to 1 and should not be set to 0 in
            // the case that a Grid has been created without explicitly setting
            // its ColumnDefinitions or RowDefinitions
            if (parentGrid.RowDefinitions.Count > 0)
            {
                _previewLayer.SetValue(Grid.RowSpanProperty, parentGrid.RowDefinitions.Count);
            }
            if (parentGrid.ColumnDefinitions.Count > 0)
            {
                _previewLayer.SetValue(Grid.ColumnSpanProperty, parentGrid.ColumnDefinitions.Count);
            }

            // REMOVE_RTM: Uncomment once Jolt Bug 11276 is fixed
            // this.previewLayer.SetValue(Grid.ZIndex, int.MaxValue);
            parentGrid.Children.Add(_previewLayer);
        }

        /// <summary>
        /// Add the preview layer to the Grid if it is not there already and
        /// then show the preview control.
        /// </summary>
        private void SetupPreview()
        {
            if (ResizeDataInternal.ShowsPreview)
            {
                if (_previewLayer == null)
                {
                    CreatePreviewLayer(ResizeDataInternal.Grid);
                }

                ResizeDataInternal.PreviewControl = new OutlookBarGridSplitterPreviewControl();
                ResizeDataInternal.PreviewControl.Bind(this);
                _previewLayer.Children.Add(ResizeDataInternal.PreviewControl);
                double[] changeRange = GetDeltaConstraints();
                Debug.Assert(changeRange.Length == 2, "The changeRange should have two elements!");
                ResizeDataInternal.MinChange = changeRange[0];
                ResizeDataInternal.MaxChange = changeRange[1];
            }
        }

        /// <summary>
        /// Remove the preview control from the preview layer if it exists.
        /// </summary>
        private void RemovePreviewControl()
        {
            if ((ResizeDataInternal.PreviewControl != null) && (_previewLayer != null))
            {
                Debug.Assert(_previewLayer.Children.Contains(ResizeDataInternal.PreviewControl), "The preview layer should contain the PreviewControl!");
                _previewLayer.Children.Remove(ResizeDataInternal.PreviewControl);
            }
        }

        #endregion Preview


        #region Enabled

        /// <summary>
        /// Called when the IsEnabled property changes.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Property changed args.</param>
        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Debug.Assert(e.NewValue is bool, "The new value should be a boolean!");
            bool isEnabled = (bool)e.NewValue;

            if (!isEnabled)
            {
                _isMouseOver = false;
            }
            ChangeVisualState();
        }

        #endregion Enabled


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Controls.OutlookBarGridSplitter" /> class.
        /// </summary>
        public OutlookBarGridSplitter()
        {
            this.IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
            this.KeyDown += new KeyEventHandler(GridSplitter_KeyDown);
            this.LayoutUpdated += delegate { UpdateTemplateOrientation(); };
            _outlookBarGridSplitterDragValidator = new OutlookBarGridSplitterDragValidator(this);
            _outlookBarGridSplitterDragValidator.DragStartedEvent += new EventHandler<DragStartedEventArgs>(DragValidator_DragStartedEvent);
            _outlookBarGridSplitterDragValidator.DragDeltaEvent += new EventHandler<DragDeltaEventArgs>(DragValidator_DragDeltaEvent);
            _outlookBarGridSplitterDragValidator.DragCompletedEvent += new EventHandler<DragCompletedEventArgs>(DragValidator_DragCompletedEvent);

            this.MouseEnter += delegate(object sender, MouseEventArgs e)
            {
                _isMouseOver = true;
                ChangeVisualState();
            };

            this.MouseLeave += delegate(object sender, MouseEventArgs e)
            {
                _isMouseOver = false;

                // Only change the visual state if we're not currently resizing,
                // the visual state will get updated when the resize operation
                // comples
                if (ResizeDataInternal == null)
                {
                    ChangeVisualState();
                }
            };

            this.GotFocus += delegate(object sender, RoutedEventArgs e)
            {
                ChangeVisualState();
            };

            this.LostFocus += delegate(object sender, RoutedEventArgs e)
            {
                ChangeVisualState();
            };

            DefaultStyleKey = typeof(OutlookBarGridSplitter);
        }

        #endregion Constructors


        #region Template

        /// <summary>
        /// Builds the visual tree for the
        /// <see cref="T:System.Windows.Controls.OutlookBarGridSplitter" />
        /// control when a new template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ElementHorizontalTemplateFrameworkElement = this.GetTemplateChild(OutlookBarGridSplitter.ElementHorizontalTemplateName) as FrameworkElement;
            ElementVerticalTemplateFrameworkElement = this.GetTemplateChild(OutlookBarGridSplitter.ElementVerticalTemplateName) as FrameworkElement;

            // We need to recalculate the orientation, so set
            // _currentGridResizeDirection back to Auto
            _currentGridResizeDirection = GridResizeDirection.Auto;

            UpdateTemplateOrientation();
            ChangeVisualState(false);
        }


        /// <summary>
        /// This code will run whenever the effective resize direction changes,
        /// to update the template being used to display this control.
        /// </summary>
        private void UpdateTemplateOrientation()
        {
            GridResizeDirection newGridResizeDirection = GetEffectiveResizeDirection();

            if (_currentGridResizeDirection != newGridResizeDirection)
            {
                if (newGridResizeDirection == GridResizeDirection.Columns)
                {
                    if (ElementHorizontalTemplateFrameworkElement != null)
                    {
                        ElementHorizontalTemplateFrameworkElement.Visibility = Visibility.Collapsed;
                    }
                    if (ElementVerticalTemplateFrameworkElement != null)
                    {
                        ElementVerticalTemplateFrameworkElement.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    if (ElementHorizontalTemplateFrameworkElement != null)
                    {
                        ElementHorizontalTemplateFrameworkElement.Visibility = Visibility.Visible;
                    }
                    if (ElementVerticalTemplateFrameworkElement != null)
                    {
                        ElementVerticalTemplateFrameworkElement.Visibility = Visibility.Collapsed;
                    }
                }
                _currentGridResizeDirection = newGridResizeDirection;
            }
        }

        #endregion Template


        #region ChangeState

        /// <summary>
        /// Method to change the visual state of the control.
        /// </summary>
        private void ChangeVisualState()
        {
            ChangeVisualState(true);
        }

        /// <summary>
        /// Change to the correct visual state for the OutlookBarGridSplitter.
        /// </summary>
        /// <param name="useTransitions">
        /// True to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.
        /// </param>
        private void ChangeVisualState(bool useTransitions)
        {
            if (!IsEnabled)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateDisabled, VisualStates.StateNormal);
            }
            else if (_isMouseOver)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateMouseOver, VisualStates.StateNormal);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateNormal);
            }

            if (HasKeyboardFocus && this.IsEnabled)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateFocused, VisualStates.StateUnfocused);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateUnfocused);
            }

            if (GetEffectiveResizeDirection() == GridResizeDirection.Columns)
            {
                this.Cursor = Cursors.SizeWE;
            }
            else
            {
                this.Cursor = Cursors.SizeNS;
            }
        }

        #endregion ChangeState


        #region DragValidator

        /// <summary>
        /// Handle the drag completed event to commit or cancel the resize
        /// operation in progress.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment.</param>
        /// <param name="e">Inherited code: Requires comment 1.</param>
        internal void DragValidator_DragCompletedEvent(object sender, DragCompletedEventArgs e)
        {
            if (ResizeDataInternal != null)
            {
                if (e.Canceled)
                {
                    CancelResize();
                }
                else
                {
                    if (ResizeDataInternal.ShowsPreview)
                    {
                        MoveSplitter(ResizeDataInternal.PreviewControl.OffsetX, ResizeDataInternal.PreviewControl.OffsetY);
                        RemovePreviewControl();
                    }
                }
                ResizeDataInternal = null;
            }
            ChangeVisualState();
        }


        /// <summary>
        /// Handle the drag delta event to update the UI for the resize
        /// operation in progress.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment.</param>
        /// <param name="e">Inherited code: Requires comment 1.</param>
        internal void DragValidator_DragDeltaEvent(object sender, DragDeltaEventArgs e)
        {
            if (ResizeDataInternal != null)
            {
                double horizontalChange = e.HorizontalChange;
                double verticalChange = e.VerticalChange;


                if (ResizeDataInternal.ResizeDirection == GridResizeDirection.Columns)
                {
                    if (Math.Abs(horizontalChange) >= this.DragIncrement)
                    {
                        if (ResizeDataInternal.ShowsPreview)
                        {
                            ResizeDataInternal.PreviewControl.OffsetX = Math.Min(Math.Max(horizontalChange, ResizeDataInternal.MinChange), ResizeDataInternal.MaxChange);
                        }
                        else
                        {
                            MoveSplitter(horizontalChange, verticalChange);
                        }
                    }
                }
                else
                {
                    if (Math.Abs(verticalChange) >= this.DragIncrement)
                    {
                        if (ResizeDataInternal.ShowsPreview)
                        {
                            ResizeDataInternal.PreviewControl.OffsetY = Math.Min(Math.Max(verticalChange, ResizeDataInternal.MinChange), ResizeDataInternal.MaxChange);
                        }
                        else
                        {
                            MoveSplitter(horizontalChange, verticalChange);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handle the drag started event to start a resize operation if the
        /// control is enabled.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment.</param>
        /// <param name="e">Inherited code: Requires comment 1.</param>
        internal void DragValidator_DragStartedEvent(object sender, DragStartedEventArgs e)
        {
            if (this.IsEnabled)
            {
                Focus();
                InitializeData(this.ShowsPreview);
            }
        }

        #endregion DragValidator


        #region KeyBoard

        /// <summary>
        /// Handle the key down event to allow keyboard resizing or canceling a
        /// resize operation.
        /// </summary>
        /// <param name="sender">Inherited code: Requires comment.</param>
        /// <param name="e">Inherited code: Requires comment 1.</param>
        internal void GridSplitter_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    e.Handled = KeyboardMoveSplitter(FlipForRTL(-KeyboardIncrement), 0.0);
                    return;

                case Key.Up:
                    e.Handled = KeyboardMoveSplitter(0.0, -KeyboardIncrement);
                    return;

                case Key.Right:
                    e.Handled = KeyboardMoveSplitter(FlipForRTL(KeyboardIncrement), 0.0);
                    return;

                case Key.Down:
                    e.Handled = KeyboardMoveSplitter(0.0, KeyboardIncrement);
                    break;

                case Key.Escape:
                    if (ResizeDataInternal == null)
                    {
                        break;
                    }
                    CancelResize();
                    e.Handled = true;
                    return;

                default:
                    return;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the control has keyboard
        /// focus.
        /// </summary>
        private bool HasKeyboardFocus
        {
            get { return FocusManager.GetFocusedElement() == this; }
        }


        /// <summary>
        /// Initialize the resize data and move the splitter by the specified
        /// amount.
        /// </summary>
        /// <param name="horizontalChange">
        /// Horizontal amount to move the splitter.
        /// </param>
        /// <param name="verticalChange">
        /// Vertical amount to move the splitter.
        /// </param>
        /// <returns>Inherited code: Requires comment.</returns>
        internal bool InitializeAndMoveSplitter(double horizontalChange, double verticalChange)
        {
            // resizing directly is not allowed if there is a mouse initiated
            // resize operation in progress
            if (ResizeDataInternal != null)
            {
                return false;
            }

            InitializeData(false);
            if (ResizeDataInternal == null)
            {
                return false;
            }

            MoveSplitter(horizontalChange, verticalChange);
            ResizeDataInternal = null;
            return true;
        }

        /// <summary>
        /// Called by keyboard event handler to move the splitter if allowed.
        /// </summary>
        /// <param name="horizontalChange">
        /// Horizontal amount to move the splitter.
        /// </param>
        /// <param name="verticalChange">
        /// Vertical amount to move the splitter.
        /// </param>
        /// <returns>Inherited code: Requires comment.</returns>
        private bool KeyboardMoveSplitter(double horizontalChange, double verticalChange)
        {
            if (HasKeyboardFocus && this.IsEnabled)
            {
                return InitializeAndMoveSplitter(horizontalChange, verticalChange);
            }
            return false;
        }

        #endregion KeyBoard


        #region Resize

        /// <summary>
        /// Initialize the resizeData object to hold the information for the
        /// resize operation in progress.
        /// </summary>
        /// <param name="showsPreview">
        /// Whether or not the preview should be shown.
        /// </param>
        private void InitializeData(bool showsPreview)
        {
            Grid parent = Parent as Grid;
            if (parent != null)
            {
                ResizeDataInternal = new ResizeData();
                ResizeDataInternal.Grid = parent;
                ResizeDataInternal.ShowsPreview = showsPreview;
                ResizeDataInternal.ResizeDirection = GetEffectiveResizeDirection();
                ResizeDataInternal.ResizeBehavior = GetEffectiveResizeBehavior(ResizeDataInternal.ResizeDirection);
                ResizeDataInternal.SplitterLength = Math.Min(ActualWidth, ActualHeight);
                if (!SetupDefinitionsToResize())
                {
                    ResizeDataInternal = null;
                }
                else
                {
                    SetupPreview();
                }
            }
        }

        /// <summary>
        /// Move the splitter and resize the affected columns or rows.
        /// </summary>
        /// <param name="horizontalChange">
        /// Amount to resize horizontally.
        /// </param>
        /// <param name="verticalChange">
        /// Amount to resize vertically.
        /// </param>
        /// <remarks>
        /// Only one of horizontalChange or verticalChange will be non-zero.
        /// </remarks>
        private void MoveSplitter(double horizontalChange, double verticalChange)
        {
            double resizeChange = (ResizeDataInternal.ResizeDirection == GridResizeDirection.Columns) ? horizontalChange : verticalChange;
            DefinitionAbstraction definition1 = ResizeDataInternal.Definition1;
            DefinitionAbstraction definition2 = ResizeDataInternal.Definition2;
            if ((definition1 != null) && (definition2 != null))
            {
                double definition1ActualLength = GetActualLength(definition1);
                double definition2ActualLength = GetActualLength(definition2);
                if ((ResizeDataInternal.SplitBehavior == SplitBehavior.Split) && !DoubleUtil.AreClose((double)(definition1ActualLength + definition2ActualLength), (double)(ResizeDataInternal.OriginalDefinition1ActualLength + ResizeDataInternal.OriginalDefinition2ActualLength)))
                {
                    this.CancelResize();
                }
                else
                {
                    double[] changeRange = GetDeltaConstraints();
                    Debug.Assert(changeRange.Length == 2, "The changeRange should contain two elements!");
                    double minDelta = changeRange[0];
                    double maxDelta = changeRange[1];

                    resizeChange = Math.Min(Math.Max(resizeChange, minDelta), maxDelta);

                    resizeChange = GetDragIncrementMultiple(resizeChange);

                    double newDefinition1Length = definition1ActualLength + resizeChange;
                    double newDefinition2Length = definition2ActualLength - resizeChange;
                    SetLengths(newDefinition1Length, newDefinition2Length);
                }
            }
        }

        /// <summary>
        /// Determine which adjacent column or row definitions need to be
        /// included in the resize operation and set up resizeData accordingly.
        /// </summary>
        /// <returns>True if it is a valid resize operation.</returns>
        private bool SetupDefinitionsToResize()
        {
            int spanAmount = (int)GetValue((ResizeDataInternal.ResizeDirection == GridResizeDirection.Columns) ? Grid.ColumnSpanProperty : Grid.RowSpanProperty);
            if (spanAmount == 1)
            {
                int definition1Index;
                int definition2Index;
                int splitterIndex = (int)GetValue((ResizeDataInternal.ResizeDirection == GridResizeDirection.Columns) ? Grid.ColumnProperty : Grid.RowProperty);
                switch (ResizeDataInternal.ResizeBehavior)
                {
                    case GridResizeBehavior.CurrentAndNext:
                        definition1Index = splitterIndex;
                        definition2Index = splitterIndex + 1;
                        break;

                    case GridResizeBehavior.PreviousAndCurrent:
                        definition1Index = splitterIndex - 1;
                        definition2Index = splitterIndex;
                        break;

                    default:
                        definition1Index = splitterIndex - 1;
                        definition2Index = splitterIndex + 1;
                        break;
                }
                int definitionCount = (ResizeDataInternal.ResizeDirection == GridResizeDirection.Columns) ? ResizeDataInternal.Grid.ColumnDefinitions.Count : ResizeDataInternal.Grid.RowDefinitions.Count;
                if ((definition1Index >= 0) && (definition2Index < definitionCount))
                {
                    ResizeDataInternal.SplitterIndex = splitterIndex;
                    ResizeDataInternal.Definition1Index = definition1Index;
                    ResizeDataInternal.Definition1 = GetGridDefinition(ResizeDataInternal.Grid, definition1Index, ResizeDataInternal.ResizeDirection);
                    ResizeDataInternal.OriginalDefinition1Length = ResizeDataInternal.Definition1.Size;
                    ResizeDataInternal.OriginalDefinition1ActualLength = GetActualLength(ResizeDataInternal.Definition1);
                    ResizeDataInternal.Definition2Index = definition2Index;
                    ResizeDataInternal.Definition2 = GetGridDefinition(ResizeDataInternal.Grid, definition2Index, ResizeDataInternal.ResizeDirection);
                    ResizeDataInternal.OriginalDefinition2Length = ResizeDataInternal.Definition2.Size;
                    ResizeDataInternal.OriginalDefinition2ActualLength = GetActualLength(ResizeDataInternal.Definition2);
                    bool isDefinition1Star = IsStar(ResizeDataInternal.Definition1);
                    bool isDefinition2Star = IsStar(ResizeDataInternal.Definition2);
                    if (isDefinition1Star && isDefinition2Star)
                    {
                        ResizeDataInternal.SplitBehavior = SplitBehavior.Split;
                    }
                    else
                    {
                        ResizeDataInternal.SplitBehavior = !isDefinition1Star ? SplitBehavior.ResizeDefinition1 : SplitBehavior.ResizeDefinition2;
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Cancel the resize operation in progress.
        /// </summary>
        private void CancelResize()
        {
            if (ResizeDataInternal.ShowsPreview)
            {
                RemovePreviewControl();
            }
            else
            {
                SetLengths(ResizeDataInternal.OriginalDefinition1ActualLength, ResizeDataInternal.OriginalDefinition2ActualLength);
            }
            ResizeDataInternal = null;
        }

        /// <summary>
        /// Get the actual length of the given definition.
        /// </summary>
        /// <param name="definition">
        /// Row or column definition to get the actual length for.
        /// </param>
        /// <returns>
        /// Height of a row definition or width of a column definition.
        /// </returns>
        private static double GetActualLength(DefinitionAbstraction definition)
        {
            if (definition.AsColumnDefinition != null)
            {
                return definition.AsColumnDefinition.ActualWidth;
            }
            return definition.AsRowDefinition.ActualHeight;
        }

        /// <summary>
        /// Determine the max and min that the two definitions can be resized.
        /// </summary>
        /// <returns>Inherited code: Requires comment.</returns>
        private double[] GetDeltaConstraints()
        {
            double definition1ActualLength = GetActualLength(ResizeDataInternal.Definition1);
            double definition1MinSize = ResizeDataInternal.Definition1.MinSize;
            double definition1MaxSize = ResizeDataInternal.Definition1.MaxSize;
            double definition2ActualLength = GetActualLength(ResizeDataInternal.Definition2);
            double definition2MinSize = ResizeDataInternal.Definition2.MinSize;
            double definition2MaxSize = ResizeDataInternal.Definition2.MaxSize;
            double minDelta, maxDelta;

            // Can't resize smaller than the size of the splitter control itself
            if (ResizeDataInternal.SplitterIndex == ResizeDataInternal.Definition1Index)
            {
                definition1MinSize = Math.Max(definition1MinSize, ResizeDataInternal.SplitterLength);
            }
            else if (ResizeDataInternal.SplitterIndex == ResizeDataInternal.Definition2Index)
            {
                definition2MinSize = Math.Max(definition2MinSize, ResizeDataInternal.SplitterLength);
            }

            if (ResizeDataInternal.SplitBehavior == SplitBehavior.Split)
            {
                minDelta = -Math.Min((double)(definition1ActualLength - definition1MinSize), (double)(definition2MaxSize - definition2ActualLength));
                maxDelta =  Math.Min((double)(definition1MaxSize - definition1ActualLength), (double)(definition2ActualLength - definition2MinSize));
            }
            else if (ResizeDataInternal.SplitBehavior == SplitBehavior.ResizeDefinition1)
            {
                minDelta = definition1MinSize - definition1ActualLength;
                maxDelta = definition1MaxSize - definition1ActualLength;
            }
            else
            {
                minDelta = definition2ActualLength - definition2MaxSize;
                maxDelta = definition2ActualLength - definition2MinSize;
            }

            minDelta = GetDragIncrementMultiple(minDelta);
            maxDelta = GetDragIncrementMultiple(maxDelta);

            return new double[] { minDelta, maxDelta };
        }

        private double GetDragIncrementMultiple(double delta)
        {
            double dragIncrementMultiples = Math.Round(delta / DragIncrement);
            return DragIncrement * dragIncrementMultiples;
        }

        /// <summary>
        /// Determine the resize behavior based on the given direction and
        /// alignment.
        /// </summary>
        /// <param name="direction">Inherited code: Requires comment.</param>
        /// <returns>Inherited code: Requires comment 1.</returns>
        private GridResizeBehavior GetEffectiveResizeBehavior(GridResizeDirection direction)
        {
            if (direction != GridResizeDirection.Columns)
            {
                switch (VerticalAlignment)
                {
                    case VerticalAlignment.Top:
                        return GridResizeBehavior.PreviousAndCurrent;

                    case VerticalAlignment.Bottom:
                        return GridResizeBehavior.CurrentAndNext;
                }
                return GridResizeBehavior.PreviousAndNext;
            }
            else
            {
                switch (HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        return GridResizeBehavior.PreviousAndCurrent;

                    case HorizontalAlignment.Right:
                        return GridResizeBehavior.CurrentAndNext;
                }
                return GridResizeBehavior.PreviousAndNext;
            }
        }

        /// <summary>
        /// Determine the resize direction based on the horizontal and vertical
        /// alignments.
        /// </summary>
        /// <returns>Inherited code: Requires comment.</returns>
        private GridResizeDirection GetEffectiveResizeDirection()
        {
            if (HorizontalAlignment != HorizontalAlignment.Stretch)
            {
                return GridResizeDirection.Columns;
            }
            if ((VerticalAlignment == VerticalAlignment.Stretch) && (ActualWidth <= ActualHeight))
            {
                return GridResizeDirection.Columns;
            }
            return GridResizeDirection.Rows;
        }

        /// <summary>
        /// Create a DefinitionAbstraction instance for the given row or column
        /// index in the grid.
        /// </summary>
        /// <param name="grid">Inherited code: Requires comment.</param>
        /// <param name="index">Inherited code: Requires comment 1.</param>
        /// <param name="direction">Inherited code: Requires comment 2.</param>
        /// <returns>Inherited code: Requires comment 3.</returns>
        private static DefinitionAbstraction GetGridDefinition(Grid grid, int index, GridResizeDirection direction)
        {
            if (direction != GridResizeDirection.Columns)
            {
                return new DefinitionAbstraction(grid.RowDefinitions[index]);
            }
            return new DefinitionAbstraction(grid.ColumnDefinitions[index]);
        }

        /// <summary>
        /// Flips a given length if FlowDirection is set to RightToLeft.  This is used for 
        /// keyboard handling.
        /// </summary>
        /// <param name="value">Value to flip for RightToLeft.</param>
        /// <returns>Value if FlowDirection is set to RightToLeft; otherwise, value.</returns>
        private double FlipForRTL(double value)
        {
            return (this.FlowDirection == FlowDirection.RightToLeft) ? -value : value;
        }

        /// <summary>
        /// Set the lengths of the two definitions depending on the split
        /// behavior.
        /// </summary>
        /// <param name="definition1Pixels">
        /// Inherited code: Requires comment.
        /// </param>
        /// <param name="definition2Pixels">
        /// Inherited code: Requires comment 1.
        /// </param>
        private void SetLengths(double definition1Pixels, double definition2Pixels)
        {
            if (ResizeDataInternal.SplitBehavior == SplitBehavior.Split)
            {
                IEnumerable enumerable = (ResizeDataInternal.ResizeDirection == GridResizeDirection.Columns) ? ((IEnumerable)ResizeDataInternal.Grid.ColumnDefinitions) : ((IEnumerable)ResizeDataInternal.Grid.RowDefinitions);
                int definitionIndex = 0;
                DefinitionAbstraction definitionAbstraction;
                foreach (DependencyObject definition in enumerable)
                {
                    definitionAbstraction = new DefinitionAbstraction(definition);
                    if (definitionIndex == ResizeDataInternal.Definition1Index)
                    {
                        SetDefinitionLength(definitionAbstraction, new GridLength(definition1Pixels, GridUnitType.Star));
                    }
                    else if (definitionIndex == ResizeDataInternal.Definition2Index)
                    {
                        SetDefinitionLength(definitionAbstraction, new GridLength(definition2Pixels, GridUnitType.Star));
                    }
                    else if (IsStar(definitionAbstraction))
                    {
                        SetDefinitionLength(definitionAbstraction, new GridLength(GetActualLength(definitionAbstraction), GridUnitType.Star));
                    }
                    definitionIndex++;
                }
            }
            else if (ResizeDataInternal.SplitBehavior == SplitBehavior.ResizeDefinition1)
            {
                SetDefinitionLength(ResizeDataInternal.Definition1, new GridLength(definition1Pixels));
            }
            else
            {
                SetDefinitionLength(ResizeDataInternal.Definition2, new GridLength(definition2Pixels));
            }
        }

        /// <summary>
        /// Set the height/width of the given row/column.
        /// </summary>
        /// <param name="definition">Inherited code: Requires comment.</param>
        /// <param name="length">Inherited code: Requires comment 1.</param>
        private static void SetDefinitionLength(DefinitionAbstraction definition, GridLength length)
        {
            if (definition.AsColumnDefinition != null)
            {
                definition.AsColumnDefinition.SetValue(ColumnDefinition.WidthProperty, length);
            }
            else
            {
                definition.AsRowDefinition.SetValue(RowDefinition.HeightProperty, length);
            }
        }

        /// <summary>
        /// Determine if the given definition has its size set to the "*" value.
        /// </summary>
        /// <param name="definition">Inherited code: Requires comment.</param>
        /// <returns>Inherited code: Requires comment 1.</returns>
        private static bool IsStar(DefinitionAbstraction definition)
        {
            if (definition.AsColumnDefinition != null)
            {
                return definition.AsColumnDefinition.Width.IsStar;
            }
            return definition.AsRowDefinition.Height.IsStar;
        }

        #endregion Resize


        /// <summary>
        /// Type to hold the data for the resize operation in progress.
        /// </summary>
        /// <QualityBand>Mature</QualityBand>
        internal class ResizeData
        {
            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public OutlookBarGridSplitterPreviewControl PreviewControl { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public DefinitionAbstraction Definition1 { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public int Definition1Index { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public DefinitionAbstraction Definition2 { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public int Definition2Index { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public Grid Grid { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public double MaxChange { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public double MinChange { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public double OriginalDefinition1ActualLength { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public GridLength OriginalDefinition1Length { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public double OriginalDefinition2ActualLength { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public GridLength OriginalDefinition2Length { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public GridResizeBehavior ResizeBehavior { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public GridResizeDirection ResizeDirection { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether Inherited code: Requires comment.
            /// </summary>
            public bool ShowsPreview { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public OutlookBarGridSplitter.SplitBehavior SplitBehavior { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public int SplitterIndex { get; set; }

            /// <summary>
            /// Gets or sets Inherited code: Requires comment.
            /// </summary>
            public double SplitterLength { get; set; }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <QualityBand>Mature</QualityBand>
        internal enum GridResizeDirection
        {
            /// <summary>
            /// Inherited code: Requires comment.
            /// </summary>
            Auto,

            /// <summary>
            /// Inherited code: Requires comment.
            /// </summary>
            Columns,

            /// <summary>
            /// Inherited code: Requires comment.
            /// </summary>
            Rows
        }


        /// <summary>
        /// Pretends to be the base class for RowDefinition and ClassDefinition
        /// types so that objects of either type can be treated as one.
        /// </summary>
        /// <QualityBand>Mature</QualityBand>
        internal class DefinitionAbstraction
        {
            /// <summary>
            /// Creates an instance of the DefinitionAbstraction class based on
            /// the given row or column definition.
            /// </summary>
            /// <param name="definition">
            /// RowDefinition or ColumnDefinition instance.
            /// </param>
            public DefinitionAbstraction(DependencyObject definition)
            {
                this.AsRowDefinition = definition as RowDefinition;
                if (this.AsRowDefinition == null)
                {
                    this.AsColumnDefinition = definition as ColumnDefinition;
                    Debug.Assert(this.AsColumnDefinition != null, "AsColumnDefinition should not be null!");
                }
            }

            /// <summary>
            /// Gets the stored definition cast as a row definition.
            /// </summary>
            /// <value>Null if not a RowDefinition.</value>
            public RowDefinition AsRowDefinition { get; private set; }

            /// <summary>
            /// Gets the stored definition cast as a column definition.
            /// </summary>
            /// <value>Null if not a ColumnDefinition.</value>
            public ColumnDefinition AsColumnDefinition { get; private set; }

            /// <summary>
            /// Gets the MaxHeight/MaxWidth for the row/column.
            /// </summary>
            public double MaxSize
            {
                get
                {
                    if (this.AsRowDefinition != null)
                    {
                        return this.AsRowDefinition.MaxHeight;
                    }
                    return this.AsColumnDefinition.MaxWidth;
                }
            }

            /// <summary>
            /// Gets the MinHeight/MinWidth for the row/column.
            /// </summary>
            public double MinSize
            {
                get
                {
                    if (this.AsRowDefinition != null)
                    {
                        return this.AsRowDefinition.MinHeight;
                    }
                    return this.AsColumnDefinition.MinWidth;
                }
            }

            /// <summary>
            /// Gets the Height/Width for the row/column.
            /// </summary>
            public GridLength Size
            {
                get
                {
                    if (this.AsRowDefinition != null)
                    {
                        return this.AsRowDefinition.Height;
                    }
                    return this.AsColumnDefinition.Width;
                }
            }
        }


        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <QualityBand>Mature</QualityBand>
        internal enum SplitBehavior
        {
            /// <summary>
            /// Inherited code: Requires comment.
            /// </summary>
            Split,

            /// <summary>
            /// Inherited code: Requires comment.
            /// </summary>
            ResizeDefinition1,

            /// <summary>
            /// Inherited code: Requires comment.
            /// </summary>
            ResizeDefinition2
        }


        /// <summary>
        /// A collection of helper methods for working with double data types.
        /// </summary>
        /// <QualityBand>Mature</QualityBand>
        internal static class DoubleUtil
        {
            /// <summary>
            /// Epsilon is the smallest value such that 1.0+epsilon != 1.0.  It
            /// can be used to determine the acceptable tolerance for rounding
            /// errors.
            /// </summary>
            /// <remarks>
            /// Epsilon is normally 2.2204460492503131E-16, but Silverlight 2
            /// uses floats so the effective epsilon is really 1.192093E-07.
            /// </remarks>
            private const double Epsilon = 1.192093E-07;

            /// <summary>
            /// Inherited code: Requires comment.
            /// </summary>
            private const double ScalarAdjustment = 10.0;

            /// <summary>
            /// Determine if the two doubles are effectively equal within
            /// tolerances.
            /// </summary>
            /// <param name="value1">Inherited code: Requires comment.</param>
            /// <param name="value2">Inherited code: Requires comment 1.</param>
            /// <returns>Inherited code: Requires comment 2.</returns>
            public static bool AreClose(double value1, double value2)
            {
                if (value1 == value2)
                {
                    return true;
                }
                double num = ((Math.Abs(value1) + Math.Abs(value2)) + DoubleUtil.ScalarAdjustment) * DoubleUtil.Epsilon;
                double num2 = value1 - value2;
                return ((-num < num2) && (num > num2));
            }
        }


        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        /// <QualityBand>Mature</QualityBand>
        internal enum GridResizeBehavior
        {
            /// <summary>
            /// Inherited code: Requires comment.
            /// </summary>
            BasedOnAlignment,

            /// <summary>
            /// Inherited code: Requires comment.
            /// </summary>
            CurrentAndNext,

            /// <summary>
            /// Inherited code: Requires comment.
            /// </summary>
            PreviousAndCurrent,

            /// <summary>
            /// Inherited code: Requires comment.
            /// </summary>
            PreviousAndNext
        }

    }
}
