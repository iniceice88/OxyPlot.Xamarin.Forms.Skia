using OxyPlot.XF.Skia.Core;

namespace OxyPlot.XF.Skia
{
    public class PlotController : ControllerBase, IPlotController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlotController" /> class.
        /// </summary>
        public PlotController()
        {
            var cmd = new CompositeDelegateViewCommand<OxyTouchEventArgs>(
                PlotCommands.SnapTrackTouch,
                PlotCommands.PanZoomByTouch
                );

            this.BindTouchDown(cmd);
        }
    }
}