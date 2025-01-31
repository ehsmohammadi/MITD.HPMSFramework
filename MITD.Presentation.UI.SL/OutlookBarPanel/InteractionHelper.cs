﻿// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

//This code is taken directly from the Silverlight Toolkit

namespace MITD.Presentation.UI.OutlookBarPanel
{
    /// <summary>
    /// The InteractionHelper provides controls with support for all of the
    /// common interactions like mouse movement, mouse clicks, key presses,
    /// etc., and also incorporates proper event semantics when the control is
    /// disabled.
    /// </summary>
    internal sealed partial class InteractionHelper
    {
        // TODO: Consult with user experience experts to validate the double
        // click distance and time thresholds.

        /// <summary>
        /// The threshold used to determine whether two clicks are temporally
        /// local and considered a double click (or triple, quadruple, etc.).
        /// 500 milliseconds is the default double click value on Windows.
        /// This value would ideally be pulled form the system settings.
        /// </summary>
        private const double SequentialClickThresholdInMilliseconds = 500.0;

        /// <summary>
        /// The threshold used to determine whether two clicks are spatially
        /// local and considered a double click (or triple, quadruple, etc.)
        /// in pixels squared.  We use pixels squared so that we can compare to
        /// the distance delta without taking a square root.
        /// </summary>
        private const double SequentialClickThresholdInPixelsSquared = 3.0 * 3.0;

        /// <summary>
        /// Gets the control the InteractionHelper is targeting.
        /// </summary>
        public Control Control { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the control has focus.
        /// </summary>
        public bool IsFocused { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the mouse is over the control.
        /// </summary> 
        public bool IsMouseOver { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the read-only property is set.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Linked file.")]
        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the mouse button is pressed down
        /// over the control.
        /// </summary>
        public bool IsPressed { get; private set; }

        /// <summary>
        /// Gets or sets the last time the control was clicked.
        /// </summary>
        /// <remarks>
        /// The value is stored as Utc time because it is slightly more
        /// performant than converting to local time.
        /// </remarks>
        private DateTime LastClickTime { get; set; }

        /// <summary>
        /// Gets or sets the mouse position of the last click.
        /// </summary>
        /// <remarks>The value is relative to the control.</remarks>
        private Point LastClickPosition { get; set; }

        /// <summary>
        /// Gets the number of times the control was clicked.
        /// </summary>
        public int ClickCount { get; private set; }

        /// <summary>
        /// Reference used to call UpdateVisualState on the base class.
        /// </summary>
        private IUpdateVisualState _updateVisualState;

        /// <summary>
        /// Initializes a new instance of the InteractionHelper class.
        /// </summary>
        /// <param name="control">Control receiving interaction.</param>
        public InteractionHelper(Control control)
        {
            Debug.Assert(control != null, "control should not be null!");
            Control = control;
            _updateVisualState = control as IUpdateVisualState;

            // Wire up the event handlers for events without a virtual override
            control.Loaded += OnLoaded;
            control.IsEnabledChanged += OnIsEnabledChanged;
        }

        #region UpdateVisualState
        /// <summary>
        /// Update the visual state of the control.
        /// </summary>
        /// <param name="useTransitions">
        /// A value indicating whether to automatically generate transitions to
        /// the new state, or instantly transition to the new state.
        /// </param>
        /// <remarks>
        /// UpdateVisualState works differently than the rest of the injected
        /// functionality.  Most of the other events are overridden by the
        /// calling class which calls Allow, does what it wants, and then calls
        /// Base.  UpdateVisualState is the opposite because a number of the
        /// methods in InteractionHelper need to trigger it in the calling
        /// class.  We do this using the IUpdateVisualState internal interface.
        /// </remarks>
        private void UpdateVisualState(bool useTransitions)
        {
            if (_updateVisualState != null)
            {
                _updateVisualState.UpdateVisualState(useTransitions);
            }
        }

        /// <summary>
        /// Update the visual state of the control.
        /// </summary>
        /// <param name="useTransitions">
        /// A value indicating whether to automatically generate transitions to
        /// the new state, or instantly transition to the new state.
        /// </param>
        public void UpdateVisualStateBase(bool useTransitions)
        {
            // Handle the Common states
            if (!Control.IsEnabled)
            {
                VisualStates.GoToState(Control, useTransitions, VisualStates.StateDisabled, VisualStates.StateNormal);
            }
            else if (IsReadOnly)
            {
                VisualStates.GoToState(Control, useTransitions, VisualStates.StateReadOnly, VisualStates.StateNormal);
            }
            else if (IsPressed)
            {
                VisualStates.GoToState(Control, useTransitions, VisualStates.StatePressed, VisualStates.StateMouseOver, VisualStates.StateNormal);
            }
            else if (IsMouseOver)
            {
                VisualStates.GoToState(Control, useTransitions, VisualStates.StateMouseOver, VisualStates.StateNormal);
            }
            else
            {
                VisualStates.GoToState(Control, useTransitions, VisualStates.StateNormal);
            }

            // Handle the Focused states
            if (IsFocused)
            {
                VisualStates.GoToState(Control, useTransitions, VisualStates.StateFocused, VisualStates.StateUnfocused);
            }
            else
            {
                VisualStates.GoToState(Control, useTransitions, VisualStates.StateUnfocused);
            }
        }
        #endregion UpdateVisualState

        /// <summary>
        /// Handle the control's Loaded event.
        /// </summary>
        /// <param name="sender">The control.</param>
        /// <param name="e">Event arguments.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            UpdateVisualState(false);
        }

        /// <summary>
        /// Handle changes to the control's IsEnabled property.
        /// </summary>
        /// <param name="sender">The control.</param>
        /// <param name="e">Event arguments.</param>
        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool enabled = (bool)e.NewValue;
            if (!enabled)
            {
                IsPressed = false;
                IsMouseOver = false;
                IsFocused = false;
            }

            UpdateVisualState(true);
        }

        /// <summary>
        /// Handles changes to the control's IsReadOnly property.
        /// </summary>
        /// <param name="value">The value of the property.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Linked file.")]
        public void OnIsReadOnlyChanged(bool value)
        {
            IsReadOnly = value;
            if (!value)
            {
                IsPressed = false;
                IsMouseOver = false;
                IsFocused = false;
            }

            UpdateVisualState(true);
        }

        /// <summary>
        /// Update the visual state of the control when its template is changed.
        /// </summary>
        public void OnApplyTemplateBase()
        {
            UpdateVisualState(false);
        }

        #region GotFocus
        /// <summary>
        /// Check if the control's GotFocus event should be handled.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        /// <returns>
        /// A value indicating whether the event should be handled.
        /// </returns>
        public bool AllowGotFocus(RoutedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            bool enabled = Control.IsEnabled;
            if (enabled)
            {
                IsFocused = true;
            }
            return enabled;
        }

        /// <summary>
        /// Base implementation of the virtual GotFocus event handler.
        /// </summary>
        public void OnGotFocusBase()
        {
            UpdateVisualState(true);
        }
        #endregion GotFocus

        #region LostFocus
        /// <summary>
        /// Check if the control's LostFocus event should be handled.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        /// <returns>
        /// A value indicating whether the event should be handled.
        /// </returns>
        public bool AllowLostFocus(RoutedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            bool enabled = Control.IsEnabled;
            if (enabled)
            {
                IsFocused = false;
            }
            return enabled;
        }

        /// <summary>
        /// Base implementation of the virtual LostFocus event handler.
        /// </summary>
        public void OnLostFocusBase()
        {
            IsPressed = false;
            UpdateVisualState(true);
        }
        #endregion LostFocus

        #region MouseEnter
        /// <summary>
        /// Check if the control's MouseEnter event should be handled.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        /// <returns>
        /// A value indicating whether the event should be handled.
        /// </returns>
        public bool AllowMouseEnter(MouseEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            bool enabled = Control.IsEnabled;
            if (enabled)
            {
                IsMouseOver = true;
            }
            return enabled;
        }

        /// <summary>
        /// Base implementation of the virtual MouseEnter event handler.
        /// </summary>
        public void OnMouseEnterBase()
        {
            UpdateVisualState(true);
        }
        #endregion MouseEnter

        #region MouseLeave
        /// <summary>
        /// Check if the control's MouseLeave event should be handled.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        /// <returns>
        /// A value indicating whether the event should be handled.
        /// </returns>
        public bool AllowMouseLeave(MouseEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            bool enabled = Control.IsEnabled;
            if (enabled)
            {
                IsMouseOver = false;
            }
            return enabled;
        }

        /// <summary>
        /// Base implementation of the virtual MouseLeave event handler.
        /// </summary>
        public void OnMouseLeaveBase()
        {
            UpdateVisualState(true);
        }
        #endregion MouseLeave

        #region MouseLeftButtonDown
        /// <summary>
        /// Check if the control's MouseLeftButtonDown event should be handled.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        /// <returns>
        /// A value indicating whether the event should be handled.
        /// </returns>
        public bool AllowMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            bool enabled = Control.IsEnabled;
            if (enabled)
            {
                // Get the current position and time
                DateTime now = DateTime.UtcNow;
                Point position = e.GetPosition(Control);

                // Compute the deltas from the last click
                double timeDelta = (now - LastClickTime).TotalMilliseconds;
                Point lastPosition = LastClickPosition;
                double dx = position.X - lastPosition.X;
                double dy = position.Y - lastPosition.Y;
                double distance = dx * dx + dy * dy;

                // Check if the values fall under the sequential click temporal
                // and spatial thresholds
                if (timeDelta < SequentialClickThresholdInMilliseconds &&
                    distance < SequentialClickThresholdInPixelsSquared)
                {
                    // TODO: Does each click have to be within the single time
                    // threshold on WPF?
                    ClickCount++;
                }
                else
                {
                    ClickCount = 1;
                }

                // Set the new position and time
                LastClickTime = now;
                LastClickPosition = position;

                // Raise the event
                IsPressed = true;
            }
            else
            {
                ClickCount = 1;
            }

            return enabled;
        }

        /// <summary>
        /// Base implementation of the virtual MouseLeftButtonDown event
        /// handler.
        /// </summary>
        public void OnMouseLeftButtonDownBase()
        {
            UpdateVisualState(true);
        }
        #endregion MouseLeftButtonDown

        #region MouseLeftButtonUp
        /// <summary>
        /// Check if the control's MouseLeftButtonUp event should be handled.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        /// <returns>
        /// A value indicating whether the event should be handled.
        /// </returns>
        public bool AllowMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            bool enabled = Control.IsEnabled;
            if (enabled)
            {
                IsPressed = false;
            }
            return enabled;
        }

        /// <summary>
        /// Base implementation of the virtual MouseLeftButtonUp event handler.
        /// </summary>
        public void OnMouseLeftButtonUpBase()
        {
            UpdateVisualState(true);
        }
        #endregion MouseLeftButtonUp

        #region KeyDown
        /// <summary>
        /// Check if the control's KeyDown event should be handled.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        /// <returns>
        /// A value indicating whether the event should be handled.
        /// </returns>
        public bool AllowKeyDown(KeyEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            return Control.IsEnabled;
        }
        #endregion KeyDown

        #region KeyUp
        /// <summary>
        /// Check if the control's KeyUp event should be handled.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        /// <returns>
        /// A value indicating whether the event should be handled.
        /// </returns>
        public bool AllowKeyUp(KeyEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            return Control.IsEnabled;
        }
        #endregion KeyUp

        #region RightToLeft key translation
        /// <summary>
        /// Translates keys for proper RightToLeft mode support.
        /// </summary>
        /// <param name="flowDirection">Control's flow direction mode.</param>
        /// <param name="originalKey">Original key.</param>
        /// <returns>
        /// A translated key code, indicating how the original key should be interpreted.
        /// </returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Linked file.")]
        public static Key GetLogicalKey(FlowDirection flowDirection, Key originalKey)
        {
            Key result = originalKey;
            if (flowDirection == FlowDirection.RightToLeft)
            {
                switch (originalKey)
                {
                    case Key.Left:
                        result = Key.Right;
                        break;
                    case Key.Right:
                        result = Key.Left;
                        break;
                }
            }
            return result;
        }
        #endregion
    }
}