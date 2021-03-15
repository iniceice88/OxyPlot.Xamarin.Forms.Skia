using System;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;
using Rectangle = Xamarin.Forms.Rectangle;

namespace OxyPlot.XF.Skia.Tracker
{
    public partial class TrackerControl
    {
        public TrackerControl()
        {
            InitializeComponent();
            this.Background = Brush.Transparent;
            this.TrackerBackground = new SolidColorBrush(Color.FromHex("#E0FFFFA0"));
            this.LineStroke = new SolidColorBrush(Color.FromHex("#80000000"));
            this.ControlTemplate = (ControlTemplate)Resources["TrackerControlTemplate"];
        }

        #region BindableProperty

        public static readonly BindableProperty TrackerBackgroundProperty = BindableProperty.Create(
            nameof(TrackerBackground), typeof(Brush), typeof(TrackerControl));

        /// <summary>
        /// Identifies the <see cref="HorizontalLineVisibility"/> dependency property.
        /// </summary>
        public static readonly BindableProperty HorizontalLineVisibilityProperty =
            BindableProperty.Create(
                nameof(HorizontalLineVisibility),
                typeof(bool),
                typeof(TrackerControl),
                true);

        /// <summary>
        /// Identifies the <see cref="VerticalLineVisibility"/> dependency property.
        /// </summary>
        public static readonly BindableProperty VerticalLineVisibilityProperty =
            BindableProperty.Create(
                nameof(VerticalLineVisibility),
                typeof(bool),
                typeof(TrackerControl),
                true);

        /// <summary>
        /// Identifies the <see cref="LineStroke"/> dependency property.
        /// </summary>
        public static readonly BindableProperty LineStrokeProperty = BindableProperty.Create(
            nameof(LineStroke), typeof(Brush), typeof(TrackerControl));

        /// <summary>
        /// Identifies the <see cref="LineExtents"/> dependency property.
        /// </summary>
        public static readonly BindableProperty LineExtentsProperty = BindableProperty.Create(
            nameof(LineExtents), typeof(OxyRect), typeof(TrackerControl),
            new OxyRect());

        /// <summary>
        /// Identifies the <see cref="LineDashArray"/> dependency property.
        /// </summary>
        public static readonly BindableProperty LineDashArrayProperty = BindableProperty.Create(
            nameof(LineDashArray), typeof(DoubleCollection), typeof(TrackerControl), new DoubleCollection());

        /// <summary>
        /// Identifies the <see cref="ShowPointer"/> dependency property.
        /// </summary>
        public static readonly BindableProperty ShowPointerProperty = BindableProperty.Create(
            nameof(ShowPointer), typeof(bool), typeof(TrackerControl), true);

        /// <summary>
        /// Identifies the <see cref="CornerRadius"/> dependency property.
        /// </summary>
        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
            nameof(CornerRadius), typeof(double), typeof(TrackerControl), 0.0);

        public static readonly BindableProperty BorderThicknessProperty = BindableProperty.Create(
            nameof(BorderThickness), typeof(int), typeof(TrackerControl), 1);

        public static readonly BindableProperty BorderBrushProperty = BindableProperty.Create(
            nameof(BorderBrush), typeof(Brush), typeof(TrackerControl), Brush.Black);

        /// <summary>
        /// Identifies the <see cref="Distance"/> dependency property.
        /// </summary>
        public static readonly BindableProperty DistanceProperty = BindableProperty.Create(
            nameof(Distance), typeof(double), typeof(TrackerControl), 7.0);

        /// <summary>
        /// Identifies the <see cref="CanCenterHorizontally"/> dependency property.
        /// </summary>
        public static readonly BindableProperty CanCenterHorizontallyProperty =
            BindableProperty.Create(
                nameof(CanCenterHorizontally), typeof(bool), typeof(TrackerControl), true);

        /// <summary>
        /// Identifies the <see cref="CanCenterVertically"/> dependency property.
        /// </summary>
        public static readonly BindableProperty CanCenterVerticallyProperty =
            BindableProperty.Create(
                nameof(CanCenterVertically), typeof(bool), typeof(TrackerControl), true);

        /// <summary>
        /// Identifies the <see cref="Position"/> dependency property.
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
            get => (bool)this.GetValue(HorizontalLineVisibilityProperty);
            set => this.SetValue(HorizontalLineVisibilityProperty, value);
        }

        /// <summary>
        /// Gets or sets VerticalLineVisibility.
        /// </summary>
        public bool VerticalLineVisibility
        {
            get => (bool)this.GetValue(VerticalLineVisibilityProperty);
            set => this.SetValue(VerticalLineVisibilityProperty, value);
        }

        /// <summary>
        /// Gets or sets TrackerBackground.
        /// </summary>
        public Brush TrackerBackground
        {
            get => (Brush)this.GetValue(TrackerBackgroundProperty);
            set => this.SetValue(TrackerBackgroundProperty, value);
        }

        /// <summary>
        /// Gets or sets LineStroke.
        /// </summary>
        public Brush LineStroke
        {
            get => (Brush)this.GetValue(LineStrokeProperty);
            set => this.SetValue(LineStrokeProperty, value);
        }

        /// <summary>
        /// Gets or sets LineExtents.
        /// </summary>
        public OxyRect LineExtents
        {
            get => (OxyRect)this.GetValue(LineExtentsProperty);
            set => this.SetValue(LineExtentsProperty, value);
        }

        /// <summary>
        /// Gets or sets LineDashArray.
        /// </summary>
        public DoubleCollection LineDashArray
        {
            get => (DoubleCollection)this.GetValue(LineDashArrayProperty);
            set => this.SetValue(LineDashArrayProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show a 'pointer' on the border.
        /// </summary>
        public bool ShowPointer
        {
            get => (bool)this.GetValue(ShowPointerProperty);
            set => this.SetValue(ShowPointerProperty, value);
        }

        /// <summary>
        /// Gets or sets the corner radius (only used when ShowPoint=<c>false</c>).
        /// </summary>
        public double CornerRadius
        {
            get => (double)this.GetValue(CornerRadiusProperty);

            set => this.SetValue(CornerRadiusProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public int BorderThickness
        {
            get => (int)this.GetValue(BorderThicknessProperty);
            set => this.SetValue(BorderThicknessProperty, value);
        }

        public Brush BorderBrush
        {
            get => (Brush)this.GetValue(BorderBrushProperty);
            set => this.SetValue(BorderBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the distance of the content container from the trackers Position.
        /// </summary>
        public double Distance
        {
            get => (double)this.GetValue(DistanceProperty);
            set => this.SetValue(DistanceProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tracker can center its content box horizontally.
        /// </summary>
        public bool CanCenterHorizontally
        {
            get => (bool)this.GetValue(CanCenterHorizontallyProperty);
            set => this.SetValue(CanCenterHorizontallyProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tracker can center its content box vertically.
        /// </summary>
        public bool CanCenterVertically
        {
            get => (bool)this.GetValue(CanCenterVerticallyProperty);
            set => this.SetValue(CanCenterVerticallyProperty, value);
        }

        /// <summary>
        /// Gets or sets Position of the tracker.
        /// </summary>
        public ScreenPoint Position
        {
            get => (ScreenPoint)this.GetValue(PositionProperty);
            set => this.SetValue(PositionProperty, value);
        }
        #endregion

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.path = this.GetTemplateChild(PartPath) as Path;
            this.content = this.GetTemplateChild(PartContent) as ContentPresenter;
            this.contentContainer = this.GetTemplateChild(PartContentContainer) as Grid;
            this.horizontalLine = this.GetTemplateChild(PartHorizontalLine) as Line;
            this.verticalLine = this.GetTemplateChild(PartVerticalLine) as Line;

            if (this.contentContainer == null)
            {
                throw new InvalidOperationException($"The TrackerControl template must contain a content container with name +'{PartContentContainer}'");
            }

            if (this.path == null)
            {
                throw new InvalidOperationException($"The TrackerControl template must contain a Path with name +'{PartPath}'");
            }

            if (this.content == null)
            {
                throw new InvalidOperationException($"The TrackerControl template must contain a ContentPresenter with name +'{PartContent}'");
            }

            content.SizeChanged += Content_SizeChanged;
            this.UpdatePositionAndBorder();
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
            ((TrackerControl)bindable).OnPositionChanged();
        }

        /// <summary>
        /// Called when the position is changed.
        /// </summary>
        private void OnPositionChanged()
        {
            this.UpdatePositionAndBorderDelay();
        }

        private Timer updateTimer;
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
            if (this.contentContainer == null || BindingContext == null)
            {
                return;
            }

            var parent = this.Parent as View;
            if (parent == null)
            {
                return;
            }

            Console.WriteLine("UpdatePositionAndBorder:" + (content.Content as Label).Text);

            double canvasWidth = parent.Width;
            double canvasHeight = parent.Height;

            var contentSize = this.content.Measure(canvasWidth, canvasHeight).Request;

            double contentWidth = contentSize.Width;
            double contentHeight = contentSize.Height;

            // Minimum allowed margins around the tracker
            const double marginLimit = 10;

            var ha = HorizontalAlignment.Center;
            if (this.CanCenterHorizontally)
            {
                if (this.Position.X - (contentWidth / 2) < marginLimit)
                {
                    ha = HorizontalAlignment.Left;
                }

                if (this.Position.X + (contentWidth / 2) > canvasWidth - marginLimit)
                {
                    ha = HorizontalAlignment.Right;
                }
            }
            else
            {
                ha = this.Position.X < canvasWidth / 2 ? HorizontalAlignment.Left : HorizontalAlignment.Right;
            }

            var va = VerticalAlignment.Middle;
            if (this.CanCenterVertically)
            {
                if (this.Position.Y - (contentHeight / 2) < marginLimit)
                {
                    va = VerticalAlignment.Top;
                }

                if (ha == HorizontalAlignment.Center)
                {
                    va = VerticalAlignment.Bottom;
                    if (this.Position.Y - contentHeight < marginLimit)
                    {
                        va = VerticalAlignment.Top;
                    }
                }

                if (va == VerticalAlignment.Middle && this.Position.Y + (contentHeight / 2) > canvasHeight - marginLimit)
                {
                    va = VerticalAlignment.Bottom;
                }

                if (va == VerticalAlignment.Top && this.Position.Y + contentHeight > canvasHeight - marginLimit)
                {
                    va = VerticalAlignment.Bottom;
                }
            }
            else
            {
                va = this.Position.Y < canvasHeight / 2 ? VerticalAlignment.Top : VerticalAlignment.Bottom;
            }

            double dx = ha == HorizontalAlignment.Center ? -0.5 : ha == HorizontalAlignment.Left ? 0 : -1;
            double dy = va == VerticalAlignment.Middle ? -0.5 : va == VerticalAlignment.Top ? 0 : -1;

            this.path.Data = this.ShowPointer
                                 ? this.CreatePointerBorderGeometry(ha, va, contentWidth, contentHeight, out Thickness margin)
                                 : this.CreateBorderGeometry(ha, va, contentWidth, contentHeight, out margin);

            this.content.Margin = margin;

            var contentContainerSize = new Size(contentSize.Width + margin.Left + margin.Right, contentSize.Height + margin.Top + margin.Bottom);

            contentContainer.TranslationX = dx * contentContainerSize.Width;
            contentContainer.TranslationY = dy * contentContainerSize.Height;

            AbsoluteLayout.SetLayoutBounds(contentContainer,
                new Rectangle(Position.X, Position.Y,
                    contentContainerSize.Width,
                    contentContainerSize.Height));

            var pos = this.Position;

            if (this.horizontalLine != null)
            {
                if (this.LineExtents.Width > 0)
                {
                    this.horizontalLine.X1 = this.LineExtents.Left;
                    this.horizontalLine.X2 = this.LineExtents.Right;
                }
                else
                {
                    this.horizontalLine.X1 = 0;
                    this.horizontalLine.X2 = canvasWidth;
                }

                this.horizontalLine.Y1 = pos.Y;
                this.horizontalLine.Y2 = pos.Y;
            }

            if (this.verticalLine != null)
            {
                if (this.LineExtents.Width > 0)
                {
                    this.verticalLine.Y1 = this.LineExtents.Top;
                    this.verticalLine.Y2 = this.LineExtents.Bottom;
                }
                else
                {
                    this.verticalLine.Y1 = 0;
                    this.verticalLine.Y2 = canvasHeight;
                }

                this.verticalLine.X1 = pos.X;
                this.verticalLine.X2 = pos.X;
            }

            this.Opacity = 1;
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
            double m = this.Distance;
            var rect = new Rect(
                ha == HorizontalAlignment.Left ? m : 0, va == VerticalAlignment.Top ? m : 0, width, height);
            margin = new Thickness(
                ha == HorizontalAlignment.Left ? m : 0,
                va == VerticalAlignment.Top ? m : 0,
                ha == HorizontalAlignment.Right ? m : 0,
                va == VerticalAlignment.Bottom ? m : 0);
            return new RectangleGeometry
            {
                Rect = rect,
                //TODO RadiusX = this.CornerRadius,
                //RadiusY = this.CornerRadius
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
            double m = this.Distance;
            margin = new Thickness();

            if (ha == HorizontalAlignment.Center && va == VerticalAlignment.Bottom)
            {
                double x0 = 0;
                double x1 = width;
                double x2 = (x0 + x1) / 2;
                double y0 = 0;
                double y1 = height;
                margin = new Thickness(0, 0, 0, m);
                points = new[]
                    {
                        new Point(x0, y0), new Point(x1, y0), new Point(x1, y1), new Point(x2 + (m / 2), y1),
                        new Point(x2, y1 + m), new Point(x2 - (m / 2), y1), new Point(x0, y1)
                    };
            }

            if (ha == HorizontalAlignment.Center && va == VerticalAlignment.Top)
            {
                double x0 = 0;
                double x1 = width;
                double x2 = (x0 + x1) / 2;
                double y0 = m;
                double y1 = m + height;
                margin = new Thickness(0, m, 0, 0);
                points = new[]
                    {
                        new Point(x0, y0), new Point(x2 - (m / 2), y0), new Point(x2, 0), new Point(x2 + (m / 2), y0),
                        new Point(x1, y0), new Point(x1, y1), new Point(x0, y1)
                    };
            }

            if (ha == HorizontalAlignment.Left && va == VerticalAlignment.Middle)
            {
                double x0 = m;
                double x1 = m + width;
                double y0 = 0;
                double y1 = height;
                double y2 = (y0 + y1) / 2;
                margin = new Thickness(m, 0, 0, 0);
                points = new[]
                    {
                        new Point(0, y2), new Point(x0, y2 - (m / 2)), new Point(x0, y0), new Point(x1, y0),
                        new Point(x1, y1), new Point(x0, y1), new Point(x0, y2 + (m / 2))
                    };
            }

            if (ha == HorizontalAlignment.Right && va == VerticalAlignment.Middle)
            {
                double x0 = 0;
                double x1 = width;
                double y0 = 0;
                double y1 = height;
                double y2 = (y0 + y1) / 2;
                margin = new Thickness(0, 0, m, 0);
                points = new[]
                    {
                        new Point(x1 + m, y2), new Point(x1, y2 + (m / 2)), new Point(x1, y1), new Point(x0, y1),
                        new Point(x0, y0), new Point(x1, y0), new Point(x1, y2 - (m / 2))
                    };
            }

            if (ha == HorizontalAlignment.Left && va == VerticalAlignment.Top)
            {
                m *= 0.67;
                double x0 = m;
                double x1 = m + width;
                double y0 = m;
                double y1 = m + height;
                margin = new Thickness(m, m, 0, 0);
                points = new[]
                    {
                        new Point(0, 0), new Point(m * 2, y0), new Point(x1, y0), new Point(x1, y1), new Point(x0, y1),
                        new Point(x0, m * 2)
                    };
            }

            if (ha == HorizontalAlignment.Right && va == VerticalAlignment.Top)
            {
                m *= 0.67;
                double x0 = 0;
                double x1 = width;
                double y0 = m;
                double y1 = m + height;
                margin = new Thickness(0, m, m, 0);
                points = new[]
                    {
                        new Point(x1 + m, 0), new Point(x1, y0 + m), new Point(x1, y1), new Point(x0, y1),
                        new Point(x0, y0), new Point(x1 - m, y0)
                    };
            }

            if (ha == HorizontalAlignment.Left && va == VerticalAlignment.Bottom)
            {
                m *= 0.67;
                double x0 = m;
                double x1 = m + width;
                double y0 = 0;
                double y1 = height;
                margin = new Thickness(m, 0, 0, m);
                points = new[]
                    {
                        new Point(0, y1 + m), new Point(x0, y1 - m), new Point(x0, y0), new Point(x1, y0),
                        new Point(x1, y1), new Point(x0 + m, y1)
                    };
            }

            if (ha == HorizontalAlignment.Right && va == VerticalAlignment.Bottom)
            {
                m *= 0.67;
                double x0 = 0;
                double x1 = width;
                double y0 = 0;
                double y1 = height;
                margin = new Thickness(0, 0, m, m);
                points = new[]
                    {
                        new Point(x1 + m, y1 + m), new Point(x1 - m, y1), new Point(x0, y1), new Point(x0, y0),
                        new Point(x1, y0), new Point(x1, y1 - m)
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

            var segments = new PathSegmentCollection { new PolyLineSegment { Points = pc } };
            var pf = new PathFigure { StartPoint = points[0], Segments = segments, IsClosed = true };
            return new PathGeometry { Figures = new PathFigureCollection { pf } };
        }


        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == BindingContextProperty.PropertyName)
            {
                this.Opacity = 0;
            }
        }
    }
}
