using System;
using SkiaSharp;

namespace OxyPlot.XF.Skia
{
    /// <summary>
    /// Plot settings for Xamarin Forms Skia
    /// </summary>
    public static class XFPlotSetting
    {
        /// <summary>
        /// Custom Font Directory
        /// use for unicode fonts
        /// </summary>
        public static string CustomFontDirectory { get; set; }

        /// <summary>
        /// Provide SKTypeface
        /// </summary>
        public static Func<string/* FontFamily */, SKTypeface> SKTypefaceProvider { get; set; }
    }
}