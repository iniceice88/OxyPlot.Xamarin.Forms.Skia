// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlotViewBase.Events.cs" company="OxyPlot">
//   Copyright (c) 2020 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using OxyPlot.XF.Skia.Effects;

namespace OxyPlot.XF.Skia
{
    /// <summary>
    /// Base class for WPF PlotView implementations.
    /// </summary>
    public abstract partial class PlotViewBase
    {
        /// <summary>
        /// The touch points of the previous touch event.
        /// </summary>
        private ScreenPoint[] previousTouchPoints;

        private void AddTouchEffect()
        {
            var touchEffect = new MyTouchEffect();
            touchEffect.TouchAction += TouchEffect_TouchAction;
            this.Effects.Add(touchEffect);
        }

        private void TouchEffect_TouchAction(object sender, TouchActionEventArgs e)
        {
            switch (e.Type)
            {
                case TouchActionType.Pressed:
                    OnTouchDownEvent(e);
                    break;
                case TouchActionType.Moved:
                    OnTouchMoveEvent(e);
                    break;
                case TouchActionType.Released:
                    OnTouchUpEvent(e);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Handles touch down events.
        /// </summary>
        /// <param name="e">The motion event arguments.</param>
        /// <returns><c>true</c> if the event was handled.</returns>
        private bool OnTouchDownEvent(TouchActionEventArgs e)
        {
            var args = ToTouchEventArgs(e, Scale);
            var handled = this.ActualController.HandleTouchStarted(this, args);
            this.previousTouchPoints = GetTouchPoints(e, Scale);
            return handled;
        }

        /// <summary>
        /// Handles touch move events.
        /// </summary>
        /// <param name="e">The motion event arguments.</param>
        /// <returns><c>true</c> if the event was handled.</returns>
        private bool OnTouchMoveEvent(TouchActionEventArgs e)
        {
            var currentTouchPoints = GetTouchPoints(e, Scale);
            var args = new OxyTouchEventArgs(currentTouchPoints, this.previousTouchPoints);
            var handled = this.ActualController.HandleTouchDelta(this, args);
            this.previousTouchPoints = currentTouchPoints;
            return handled;
        }

        /// <summary>
        /// Handles touch released events.
        /// </summary>
        /// <param name="e">The motion event arguments.</param>
        /// <returns><c>true</c> if the event was handled.</returns>
        private bool OnTouchUpEvent(TouchActionEventArgs e)
        {
            return this.ActualController.HandleTouchCompleted(this, ToTouchEventArgs(e, Scale));
        }

        /// <summary>
        /// Converts an <see cref="TouchActionEventArgs" /> to a <see cref="OxyTouchEventArgs" />.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        /// <param name = "scale">The resolution scale factor.</param>
        /// <returns>The converted event arguments.</returns>
        public static OxyTouchEventArgs ToTouchEventArgs(TouchActionEventArgs e, double scale)
        {
            return new OxyTouchEventArgs
            {
                Position = new ScreenPoint(e.Location.X / scale, e.Location.Y / scale),
                DeltaTranslation = new ScreenVector(0, 0),
                DeltaScale = new ScreenVector(1, 1)
            };
        }

        /// <summary>
        /// Gets the touch points from the specified <see cref="MotionEvent" /> argument.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        /// <param name = "scale">The resolution scale factor.</param>
        /// <returns>The touch points.</returns>
        public static ScreenPoint[] GetTouchPoints(TouchActionEventArgs e, double scale)
        {
            var result = new ScreenPoint[e.Locations.Length];
            for (int i = 0; i < e.Locations.Length; i++)
            {
                result[i] = new ScreenPoint(e.Locations[i].X / scale, e.Locations[i].Y / scale);
            }

            return result;
        }

        ///// <summary>
        ///// Called when the <see cref="E:System.Windows.UIElement.ManipulationStarted" /> event occurs.
        ///// </summary>
        ///// <param name="e">The data for the event.</param>
        //protected override void OnManipulationStarted(ManipulationStartedEventArgs e)
        //{
        //    base.OnManipulationStarted(e);
        //    if (e.Handled)
        //    {
        //        return;
        //    }

        //    e.Handled = this.ActualController.HandleTouchStarted(this, e.ToTouchEventArgs(this));
        //}

        ///// <summary>
        ///// Called when the <see cref="E:System.Windows.UIElement.ManipulationDelta" /> event occurs.
        ///// </summary>
        ///// <param name="e">The data for the event.</param>
        //protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        //{
        //    //PanGestureRecognizer
        //    //PinchGestureRecognizer

        //    base.OnManipulationDelta(e);
        //    if (e.Handled)
        //    {
        //        return;
        //    }

        //    e.Handled = this.ActualController.HandleTouchDelta(this, e.ToTouchEventArgs(this));
        //}

        ///// <summary>
        ///// Called when the <see cref="E:System.Windows.UIElement.ManipulationCompleted" /> event occurs.
        ///// </summary>
        ///// <param name="e">The data for the event.</param>
        //protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        //{
        //    base.OnManipulationCompleted(e);
        //    if (e.Handled)
        //    {
        //        return;
        //    }

        //    e.Handled = this.ActualController.HandleTouchCompleted(this, e.ToTouchEventArgs(this));
        //}
    }
}
