﻿using System;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;
using Rectangle = Xamarin.Forms.Rectangle;

namespace OxyPlot.XF.Skia
{
    public partial class TrackerControl
    {
        private Timer updateTimer;

        public TrackerControl()
        {
            InitializeComponent();
            Background = new SolidColorBrush(Color.FromHex("#E0FFFFA0"));
            LineStroke = new SolidColorBrush(Color.FromHex("#80000000"));
            ControlTemplate = (ControlTemplate) Resources["TrackerControlTemplate"];
        }

        #region BindableProperty

        /// <summary>
        /// Identifies the <see cref="HorizontalLineVisibility" /> dependency property.
        /// </summary>
        public static readonly BindableProperty HorizontalLineVisibilityProperty =
            BindableProperty.Create(
                nameof(HorizontalLineVisibility),
                typeof(bool),
                typeof(TrackerControl),
                true);

        /// <summary>
        /// Identifies the <see cref="VerticalLineVisibility" /> dependency property.
        /// </summary>
        public static readonly BindableProperty VerticalLineVisibilityProperty =
            BindableProperty.Create(
                nameof(VerticalLineVisibility),
                typeof(bool),
                typeof(TrackerControl),
                true);

        /// <summary>
        /// Identifies the <see cref="LineStroke" /> dependency property.
        /// </summary>
        public static readonly BindableProperty LineStrokeProperty = BindableProperty.Create(
            nameof(LineStroke), typeof(Brush), typeof(TrackerControl));

        /// <summary>
        /// Identifies the <see cref="LineExtents" /> dependency property.
        /// </summary>
        public static readonly BindableProperty LineExtentsProperty = BindableProperty.Create(
            nameof(LineExtents), typeof(OxyRect), typeof(TrackerControl),
            new OxyRect());

        /// <summary>
        /// Identifies the <see cref="LineDashArray" /> dependency property.
        /// </summary>
        public static readonly BindableProperty LineDashArrayProperty = BindableProperty.Create(
            nameof(LineDashArray), typeof(DoubleCollection), typeof(TrackerControl), new DoubleCollection());

        /// <summary>
        /// Identifies the <see cref="ShowPointer" /> dependency property.
        /// </summary>
        public static readonly BindableProperty ShowPointerProperty = BindableProperty.Create(
            nameof(ShowPointer), typeof(bool), typeof(TrackerControl), true);

        /// <summary>
        /// Identifies the <see cref="CornerRadius" /> dependency property.
        /// </summary>
        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
            nameof(CornerRadius), typeof(double), typeof(TrackerControl), 0.0);

        public static readonly BindableProperty BorderThicknessProperty = BindableProperty.Create(
            nameof(BorderThickness), typeof(int), typeof(TrackerControl), 1);

        public static readonly BindableProperty BorderBrushProperty = BindableProperty.Create(
            nameof(BorderBrush), typeof(Brush), typeof(TrackerControl), Brush.Black);

        /// <summary>
        /// Identifies the <see cref="Distance" /> dependency property.
        /// </summary>
        public static readonly BindableProperty DistanceProperty = BindableProperty.Create(
            nameof(Distance), typeof(double), typeof(TrackerControl), 7.0);

        /// <summary>
        /// Identifies the <see cref="CanCenterHorizontally" /> dependency property.
        /// </summary>
        public static readonly BindableProperty CanCenterHorizontallyProperty =
            BindableProperty.Create(
                nameof(CanCenterHorizontally), typeof(bool), typeof(TrackerControl), true);

        /// <summary>
        /// Identifies the <see cref="CanCenterVertically" /> dependency property.
        /// </summary>
        public static readonly BindableProperty CanCenterVerticallyProperty =
            BindableProperty.Create(
                nameof(CanCenterVertically), typeof(bool), typeof(TrackerControl), true);

        /// <summary>
        /// Identifies the <see cref="Position" /> dependency property.
        /// </summary>
        public static readonly BindableProperty PositionProperty = BindableProperty.Create(
            nameof(Position),
            typeof(ScreenPoint),
            typeof(TrackerControl),
            new ScreenPoint(), propertyChanged: PositionChanged);

        /// <summary>
        /// The path part string.
        /// </summary>
        private const string PartPath = "PART_Path";

        /// <summary>
        /// The content part string.
        /// </summary>
        private const string PartContent = "PART_Content";

        /// <summary>
        /// The content container part string.
        /// </summary>
        private const string PartContentContainer = "PART_ContentContainer";

        /// <summary>
        /// The horizontal line part string.
        /// </summary>
        private const string PartHorizontalLine = "PART_HorizontalLine";

        /// <summary>
        /// The vertical line part string.
        /// </summary>
        private const string PartVerticalLine = "PART_VerticalLine";

        /// <summary>
        /// The content.
        /// </summary>
        private ContentPresenter content;

        /// <summary>
        /// The horizontal line.
        /// </summary>
        private Line horizontalLine;

        /// <summary>
        /// The path.
        /// </summary>
        private Path path;

        /// <summary>
        /// The content container.
        /// </summary>
        private Grid contentContainer;

        /// <summary>
        /// The vertical line.
        /// </summary>
        private Line verticalLine;

        /// <summary>
        /// Gets or sets HorizontalLineVisibility.
        /// </summary>
        public bool HorizontalLineVisibility
        {
            get => (bool)GetValue(HorizontalLineVisibilityProperty);
            set => SetValue(HorizontalLineVisibilityProperty, value);
        }

        /// <summary>
        /// Gets or sets VerticalLineVisibility.
        /// </summary>
        public bool VerticalLineVisibility
        {
            get => (bool)GetValue(VerticalLineVisibilityProperty);
            set => SetValue(VerticalLineVisibilityProperty, value);
        }

        /// <summary>
        /// Gets or sets LineStroke.
        /// </summary>
        public Brush LineStroke
        {
            get => (Brush)GetValue(LineStrokeProperty);
            set => SetValue(LineStrokeProperty, value);
        }

        /// <summary>
        /// Gets or sets LineExtents.
        /// </summary>
        public OxyRect LineExtents
        {
            get => (OxyRect)GetValue(LineExtentsProperty);
            set => SetValue(LineExtentsProperty, value);
        }

        /// <summary>
        /// Gets or sets LineDashArray.
        /// </summary>
        public DoubleCollection LineDashArray
        {
            get => (DoubleCollection)GetValue(LineDashArrayProperty);
            set => SetValue(LineDashArrayProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show a 'pointer' on the border.
        /// </summary>
        public bool ShowPointer
        {
            get => (bool)GetValue(ShowPointerProperty);
            set => SetValue(ShowPointerProperty, value);
        }

        /// <summary>
        /// Gets or sets the corner radius (only used when ShowPoint=<c>false</c>).
        /// </summary>
        public double CornerRadius
        {
            get => (double)GetValue(CornerRadiusProperty);

            set => SetValue(CornerRadiusProperty, value);
        }

        /// <summary>
        /// </summary>
        public int BorderThickness
        {
            get => (int)GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }

        public Brush BorderBrush
        {
            get => (Brush)GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the distance of the content container from the trackers Position.
        /// </summary>
        public double Distance
        {
            get => (double)GetValue(DistanceProperty);
            set => SetValue(DistanceProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tracker can center its content box horizontally.
        /// </summary>
        public bool CanCenterHorizontally
        {
            get => (bool)GetValue(CanCenterHorizontallyProperty);
            set => SetValue(CanCenterHorizontallyProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tracker can center its content box vertically.
        /// </summary>
        public bool CanCenterVertically
        {
            get => (bool)GetValue(CanCenterVerticallyProperty);
            set => SetValue(CanCenterVerticallyProperty, value);
        }

        /// <summary>
        /// Gets or sets Position of the tracker.
        /// </summary>
        public ScreenPoint Position
        {
            get => (ScreenPoint)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        #endregion

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call
        /// <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            path = GetTemplateChild(PartPath) as Path;
            content = GetTemplateChild(PartContent) as ContentPresenter;
            contentContainer = GetTemplateChild(PartContentContainer) as Grid;
            horizontalLine = GetTemplateChild(PartHorizontalLine) as Line;
            verticalLine = GetTemplateChild(PartVerticalLine) as Line;

            if (contentContainer == null)
            {
                throw new InvalidOperationException($"The TrackerControl template must contain a content container with name +'{PartContentContainer}'");
            }

            if (path == null)
            {
                throw new InvalidOperationException($"The TrackerControl template must contain a Path with name +'{PartPath}'");
            }

            if (content == null)
            {
                throw new InvalidOperationException($"The TrackerControl template must contain a ContentPresenter with name +'{PartContent}'");
            }

            content.SizeChanged += Content_SizeChanged;
            UpdatePositionAndBorder();
        }

        private void Content_SizeChanged(object sender, EventArgs e)
        {
            UpdatePositionAndBorderDelay();
        }

        /// <summary>
        /// Called when the position is changed.
        /// </summary>
        private static void PositionChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((TrackerControl) bindable).OnPositionChanged();
        }

        /// <summary>
        /// Called when the position is changed.
        /// </summary>
        private void OnPositionChanged()
        {
            UpdatePositionAndBorderDelay();
        }

        private void UpdatePositionAndBorderDelay()
        {
            if (updateTimer == null)
            {
                updateTimer = new Timer(30);
                updateTimer.AutoReset = false;
                updateTimer.Elapsed += (s, e) =>
                {
                    updateTimer = null;
                    Device.BeginInvokeOnMainThread(UpdatePositionAndBorder);
                };
            }
            else
            {
                updateTimer.Stop();
            }

            updateTimer.Start();
        }

        /// <summary>
        /// Update the position and border of the tracker.
        /// </summary>
        private void UpdatePositionAndBorder()
        {
            if (contentContainer == null || BindingContext == null)
            {
                return;
            }

            var parent = Parent as View;
            if (parent == null)
            {
                return;
            }

            var canvasWidth = parent.Width;
            var canvasHeight = parent.Height;

            var contentSize = content.Measure(canvasWidth, canvasHeight).Request;

            var contentWidth = contentSize.Width;
            var contentHeight = contentSize.Height;

            // Minimum allowed margins around the tracker
            const double marginLimit = 10;

            var ha = HorizontalAlignment.Center;
            if (CanCenterHorizontally)
            {
                if (Position.X - contentWidth / 2 < marginLimit)
                {
                    ha = HorizontalAlignment.Left;
                }

                if (Position.X + contentWidth / 2 > canvasWidth - marginLimit)
                {
                    ha = HorizontalAlignment.Right;
                }
            }
            else
            {
                ha = Position.X < canvasWidth / 2 ? HorizontalAlignment.Left : HorizontalAlignment.Right;
            }

            var va = VerticalAlignment.Middle;
            if (CanCenterVertically)
            {
                if (Position.Y - contentHeight / 2 < marginLimit)
                {
                    va = VerticalAlignment.Top;
                }

                if (ha == HorizontalAlignment.Center)
                {
                    va = VerticalAlignment.Bottom;
                    if (Position.Y - contentHeight < marginLimit)
                    {
                        va = VerticalAlignment.Top;
                    }
                }

                if (va == VerticalAlignment.Middle && Position.Y + contentHeight / 2 > canvasHeight - marginLimit)
                {
                    va = VerticalAlignment.Bottom;
                }

                if (va == VerticalAlignment.Top && Position.Y + contentHeight > canvasHeight - marginLimit)
                {
                    va = VerticalAlignment.Bottom;
                }
            }
            else
            {
                va = Position.Y < canvasHeight / 2 ? VerticalAlignment.Top : VerticalAlignment.Bottom;
            }

            var dx = ha == HorizontalAlignment.Center ? -0.5 : ha == HorizontalAlignment.Left ? 0 : -1;
            var dy = va == VerticalAlignment.Middle ? -0.5 : va == VerticalAlignment.Top ? 0 : -1;

            path.Data = ShowPointer
                ? CreatePointerBorderGeometry(ha, va, contentWidth, contentHeight, out var margin)
                : CreateBorderGeometry(ha, va, contentWidth, contentHeight, out margin);

            content.Margin = margin;

            var contentContainerSize = new Size(contentSize.Width + margin.Left + margin.Right, contentSize.Height + margin.Top + margin.Bottom);

            contentContainer.TranslationX = dx * contentContainerSize.Width;
            contentContainer.TranslationY = dy * contentContainerSize.Height;

            AbsoluteLayout.SetLayoutBounds(contentContainer,
                new Rectangle(Position.X, Position.Y,
                    contentContainerSize.Width,
                    contentContainerSize.Height));

            var pos = Position;

            if (horizontalLine != null)
            {
                if (LineExtents.Width > 0)
                {
                    horizontalLine.X1 = LineExtents.Left;
                    horizontalLine.X2 = LineExtents.Right;
                }
                else
                {
                    horizontalLine.X1 = 0;
                    horizontalLine.X2 = canvasWidth;
                }

                horizontalLine.Y1 = pos.Y;
                horizontalLine.Y2 = pos.Y;
            }

            if (verticalLine != null)
            {
                if (LineExtents.Width > 0)
                {
                    verticalLine.Y1 = LineExtents.Top;
                    verticalLine.Y2 = LineExtents.Bottom;
                }
                else
                {
                    verticalLine.Y1 = 0;
                    verticalLine.Y2 = canvasHeight;
                }

                verticalLine.X1 = pos.X;
                verticalLine.X2 = pos.X;
            }

            Opacity = 1;
        }

        /// <summary>
        /// Create the border geometry.
        /// </summary>
        /// <param name="ha">The horizontal alignment.</param>
        /// <param name="va">The vertical alignment.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="margin">The margin.</param>
        /// <returns>The border geometry.</returns>
        private Geometry CreateBorderGeometry(
            HorizontalAlignment ha, VerticalAlignment va, double width, double height, out Thickness margin)
        {
            var m = Distance;
            var rect = new Rect(
                ha == HorizontalAlignment.Left ? m : 0, va == VerticalAlignment.Top ? m : 0, width, height);
            margin = new Thickness(
                ha == HorizontalAlignment.Left ? m : 0,
                va == VerticalAlignment.Top ? m : 0,
                ha == HorizontalAlignment.Right ? m : 0,
                va == VerticalAlignment.Bottom ? m : 0);

            return new RoundRectangleGeometry
            {
                Rect = rect,
                CornerRadius = new CornerRadius(CornerRadius)
            };
        }

        /// <summary>
        /// Create a border geometry with a 'pointer'.
        /// </summary>
        /// <param name="ha">The horizontal alignment.</param>
        /// <param name="va">The vertical alignment.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="margin">The margin.</param>
        /// <returns>The border geometry.</returns>
        private Geometry CreatePointerBorderGeometry(
            HorizontalAlignment ha, VerticalAlignment va, double width, double height, out Thickness margin)
        {
            Point[] points = null;
            var m = Distance;
            margin = new Thickness();

            if (ha == HorizontalAlignment.Center && va == VerticalAlignment.Bottom)
            {
                double x0 = 0;
                var x1 = width;
                var x2 = (x0 + x1) / 2;
                double y0 = 0;
                var y1 = height;
                margin = new Thickness(0, 0, 0, m);
                points = new[]
                {
                    new Point(x0, y0),
                    new Point(x1, y0),
                    new Point(x1, y1),
                    new Point(x2 + m / 2, y1),
                    new Point(x2, y1 + m),
                    new Point(x2 - m / 2, y1),
                    new Point(x0, y1)
                };
            }

            if (ha == HorizontalAlignment.Center && va == VerticalAlignment.Top)
            {
                double x0 = 0;
                var x1 = width;
                var x2 = (x0 + x1) / 2;
                var y0 = m;
                var y1 = m + height;
                margin = new Thickness(0, m, 0, 0);
                points = new[]
                {
                    new Point(x0, y0),
                    new Point(x2 - m / 2, y0),
                    new Point(x2, 0),
                    new Point(x2 + m / 2, y0),
                    new Point(x1, y0),
                    new Point(x1, y1),
                    new Point(x0, y1)
                };
            }

            if (ha == HorizontalAlignment.Left && va == VerticalAlignment.Middle)
            {
                var x0 = m;
                var x1 = m + width;
                double y0 = 0;
                var y1 = height;
                var y2 = (y0 + y1) / 2;
                margin = new Thickness(m, 0, 0, 0);
                points = new[]
                {
                    new Point(0, y2),
                    new Point(x0, y2 - m / 2),
                    new Point(x0, y0),
                    new Point(x1, y0),
                    new Point(x1, y1),
                    new Point(x0, y1),
                    new Point(x0, y2 + m / 2)
                };
            }

            if (ha == HorizontalAlignment.Right && va == VerticalAlignment.Middle)
            {
                double x0 = 0;
                var x1 = width;
                double y0 = 0;
                var y1 = height;
                var y2 = (y0 + y1) / 2;
                margin = new Thickness(0, 0, m, 0);
                points = new[]
                {
                    new Point(x1 + m, y2),
                    new Point(x1, y2 + m / 2),
                    new Point(x1, y1),
                    new Point(x0, y1),
                    new Point(x0, y0),
                    new Point(x1, y0),
                    new Point(x1, y2 - m / 2)
                };
            }

            if (ha == HorizontalAlignment.Left && va == VerticalAlignment.Top)
            {
                m *= 0.67;
                var x0 = m;
                var x1 = m + width;
                var y0 = m;
                var y1 = m + height;
                margin = new Thickness(m, m, 0, 0);
                points = new[]
                {
                    new Point(0, 0),
                    new Point(m * 2, y0),
                    new Point(x1, y0),
                    new Point(x1, y1),
                    new Point(x0, y1),
                    new Point(x0, m * 2)
                };
            }

            if (ha == HorizontalAlignment.Right && va == VerticalAlignment.Top)
            {
                m *= 0.67;
                double x0 = 0;
                var x1 = width;
                var y0 = m;
                var y1 = m + height;
                margin = new Thickness(0, m, m, 0);
                points = new[]
                {
                    new Point(x1 + m, 0),
                    new Point(x1, y0 + m),
                    new Point(x1, y1),
                    new Point(x0, y1),
                    new Point(x0, y0),
                    new Point(x1 - m, y0)
                };
            }

            if (ha == HorizontalAlignment.Left && va == VerticalAlignment.Bottom)
            {
                m *= 0.67;
                var x0 = m;
                var x1 = m + width;
                double y0 = 0;
                var y1 = height;
                margin = new Thickness(m, 0, 0, m);
                points = new[]
                {
                    new Point(0, y1 + m),
                    new Point(x0, y1 - m),
                    new Point(x0, y0),
                    new Point(x1, y0),
                    new Point(x1, y1),
                    new Point(x0 + m, y1)
                };
            }

            if (ha == HorizontalAlignment.Right && va == VerticalAlignment.Bottom)
            {
                m *= 0.67;
                double x0 = 0;
                var x1 = width;
                double y0 = 0;
                var y1 = height;
                margin = new Thickness(0, 0, m, m);
                points = new[]
                {
                    new Point(x1 + m, y1 + m),
                    new Point(x1 - m, y1),
                    new Point(x0, y1),
                    new Point(x0, y0),
                    new Point(x1, y0),
                    new Point(x1, y1 - m)
                };
            }

            if (points == null)
            {
                return null;
            }

            var pc = new PointCollection();
            foreach (var p in points)
            {
                pc.Add(p);
            }

            var segments = new PathSegmentCollection {new PolyLineSegment {Points = pc}};
            var pf = new PathFigure {StartPoint = points[0], Segments = segments, IsClosed = true};
            return new PathGeometry {Figures = new PathFigureCollection {pf}};
        }


        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == BindingContextProperty.PropertyName)
            {
                Opacity = 0;
            }
            else if (propertyName == BackgroundColorProperty.PropertyName)
            {
                Background = new SolidColorBrush(BackgroundColor);
            }
        }
    }
}