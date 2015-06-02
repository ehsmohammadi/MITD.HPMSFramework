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
using System.Diagnostics;
using MITD.Presentation.UI.OutlookBarPanel;

namespace MITD.Presentation.UI
{
    /// <summary>
    /// Represents the control that shows a preview of the OutlookBarGridSplitter's
    /// redistribution of space between columns or rows of a Grid control.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    //[TemplatePart(Name = OutlookBarGridSplitterPreviewControl.ElementHorizontalTemplateName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = OutlookBarGridSplitterPreviewControl.ElementVerticalTemplateName, Type = typeof(FrameworkElement))]
    internal partial class OutlookBarGridSplitterPreviewControl : Control
    {
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
        /// Is Null until the PreviewControl is bound to a GridSplitter.
        /// </summary>
        private OutlookBarGridSplitter.GridResizeDirection _currentGridResizeDirection;

        /// <summary>
        /// Tracks the bound GridSplitter's location for calculating the
        /// PreviewControl's offset.
        /// </summary>
        private Point _gridSplitterOrigin;

        /// <summary>
        /// Instantiate the PreviewControl.
        /// </summary>
        public OutlookBarGridSplitterPreviewControl()
        {
            _gridSplitterOrigin = new Point();
        }

        /// <summary>
        /// Called when template should be applied to the control.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ElementHorizontalTemplateFrameworkElement = this.GetTemplateChild(OutlookBarGridSplitterPreviewControl.ElementHorizontalTemplateName) as FrameworkElement;
            ElementVerticalTemplateFrameworkElement = this.GetTemplateChild(OutlookBarGridSplitterPreviewControl.ElementVerticalTemplateName) as FrameworkElement;

            if (_currentGridResizeDirection == OutlookBarGridSplitter.GridResizeDirection.Columns)
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
        }

        /// <summary>
        /// Bind the the dimensions of the preview control to the associated
        /// grid splitter.
        /// </summary>
        /// <param name="outlookBarGridSplitter">GridSplitter instance to target.</param>
        public void Bind(OutlookBarGridSplitter outlookBarGridSplitter)
        {
            Debug.Assert(outlookBarGridSplitter != null, "gridSplitter should not be null!");
            Debug.Assert(outlookBarGridSplitter.Parent != null, "gridSplitter.Parent should not be null!");

            this.Style = outlookBarGridSplitter.PreviewStyle;
            this.Height = outlookBarGridSplitter.ActualHeight;
            this.Width = outlookBarGridSplitter.ActualWidth;

            if (outlookBarGridSplitter.ResizeDataInternal != null)
            {
                _currentGridResizeDirection = outlookBarGridSplitter.ResizeDataInternal.ResizeDirection;
            }

            GeneralTransform gt = outlookBarGridSplitter.TransformToVisual((UIElement)outlookBarGridSplitter.Parent);
            Point p = new Point(0, 0);
            p = gt.Transform(p);

            _gridSplitterOrigin.X = p.X;
            _gridSplitterOrigin.Y = p.Y;

            SetValue(Canvas.LeftProperty, _gridSplitterOrigin.X);
            SetValue(Canvas.TopProperty, _gridSplitterOrigin.Y);
        }

        /// <summary>
        /// Gets or sets the x-axis offset for the underlying render transform.
        /// </summary>
        public double OffsetX
        {
            get { return (double)GetValue(Canvas.LeftProperty) - _gridSplitterOrigin.X; }
            set { SetValue(Canvas.LeftProperty, _gridSplitterOrigin.X + value); }
        }

        /// <summary>
        /// Gets or sets the y-axis offset for the underlying render transform.
        /// </summary>
        public double OffsetY
        {
            get { return (double)GetValue(Canvas.TopProperty) - _gridSplitterOrigin.Y; }
            set { SetValue(Canvas.TopProperty, _gridSplitterOrigin.Y + value); }
        }
    }
}
