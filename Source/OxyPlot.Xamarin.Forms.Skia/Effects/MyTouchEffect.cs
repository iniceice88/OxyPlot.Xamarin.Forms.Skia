using System;
using Xamarin.Forms;

namespace OxyPlot.XF.Skia.Effects
{
    public class MyTouchEffect : RoutingEffect
    {
        public event TouchActionEventHandler TouchAction;

        public MyTouchEffect() : base("OxyPlot.XF.Skia.Effects.MyTouchEffect")
        {
        }

        public void OnTouchAction(Xamarin.Forms.Element element, TouchActionEventArgs args)
        {
            TouchAction?.Invoke(element, args);
        }
    }

    public delegate void TouchActionEventHandler(object sender, TouchActionEventArgs args);

    public enum TouchActionType
    {
        Pressed,
        Moved,
        Released
    }

    public class TouchActionEventArgs : EventArgs
    {
        public TouchActionEventArgs(long id, TouchActionType type, Point[] locations, bool isInContact)
        {
            Id = id;
            Type = type;
            Locations = locations;
            IsInContact = isInContact;
        }

        public long Id { get; }

        public TouchActionType Type { get; }

        public Point Location => Locations == null || Locations.Length == 0 ? Point.Zero : Locations[0];

        public Point[] Locations { get; }

        public bool IsInContact { get; }
    }
}
