namespace OxyPlot.XF.Skia
{
    public class PlotCommands
    {
        /// <summary>
        /// Gets the pan/zoom touch command.
        /// </summary>
        public static IViewCommand<OxyTouchEventArgs> PanZoomByTouch { get; private set; }

        /// <summary>
        /// Gets the snap tracker command.
        /// </summary>
        public static IViewCommand<OxyTouchEventArgs> SnapTrackTouch { get; private set; }

        static PlotCommands()
        {
            PanZoomByTouch = new DelegatePlotCommand<OxyTouchEventArgs>((view, controller, args) => controller.AddTouchManipulator(view, new Manipulators.TouchManipulator(view), args));
            SnapTrackTouch = new DelegatePlotCommand<OxyTouchEventArgs>((view, controller, args) => controller.AddTouchManipulator(view, new Manipulators.TouchTrackerManipulator(view) { Snap = true, PointsOnly = true }, args));
        }
    }
}