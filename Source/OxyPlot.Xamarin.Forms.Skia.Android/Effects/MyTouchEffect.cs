using System;
using System.Collections.Generic;
using System.Linq;
using Android.Views;
using OxyPlot.XF.Skia.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;

[assembly: ResolutionGroupName("OxyPlot.XF.Skia.Effects")]
[assembly: ExportEffect(typeof(OxyPlot.XF.Skia.Droid.Effects.MyTouchEffect), "MyTouchEffect")]

namespace OxyPlot.XF.Skia.Droid.Effects
{
    public class MyTouchEffect : PlatformEffect
    {
        private static readonly Dictionary<View, MyTouchEffect> ViewDictionary =
            new Dictionary<View, MyTouchEffect>();

        private Xamarin.Forms.Element _formsElement;
        private Func<double, double> _fromPixels;
        private Skia.Effects.MyTouchEffect libMyTouchEffect;
        private View _view;

        protected override void OnAttached()
        {
            // Get the Android View corresponding to the Element that the effect is attached to
            _view = Control == null ? Container : Control;

            // Get access to the TouchEffect class in the .NET Standard library
            var touchEffect =
                (Skia.Effects.MyTouchEffect)Element.Effects.FirstOrDefault(e => e is Skia.Effects.MyTouchEffect);

            if (touchEffect != null && _view != null)
            {
                ViewDictionary.Add(_view, this);

                _formsElement = Element;

                libMyTouchEffect = touchEffect;

                // Save fromPixels function
                _fromPixels = _view.Context.FromPixels;

                // Set event handler on View
                _view.Touch += OnTouch;
            }
        }

        protected override void OnDetached()
        {
            if (ViewDictionary.ContainsKey(_view))
            {
                ViewDictionary.Remove(_view);
                _view.Touch -= OnTouch;
            }
        }

        private void OnTouch(object sender, View.TouchEventArgs args)
        {
            // Two object common to all the events
            var senderView = sender as View;
            var motionEvent = args.Event;


            int[] twoIntArray = new int[2];
            senderView.GetLocationOnScreen(twoIntArray);


            var list = new List<Point>();
            for (var pointerIndex = 0; pointerIndex < motionEvent.PointerCount; pointerIndex++)
            {

                senderView.GetLocationOnScreen(twoIntArray);

                list.Add(new Point(twoIntArray[0] + motionEvent.GetX(pointerIndex),
                    twoIntArray[1] + motionEvent.GetY(pointerIndex)));
            }

            var screenPointerCoords = list.ToArray();
            var id = motionEvent.GetPointerId(motionEvent.ActionIndex);

            // Use ActionMasked here rather than Action to reduce the number of possibilities
            switch (args.Event.ActionMasked)
            {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    FireEvent(this, id, TouchActionType.Pressed, screenPointerCoords, true);
                    break;
                case MotionEventActions.Move:
                    FireEvent(this, id, TouchActionType.Moved, screenPointerCoords, true);
                    break;
                case MotionEventActions.Up:
                case MotionEventActions.Pointer1Up:
                    FireEvent(this, id, TouchActionType.Released, screenPointerCoords, false);
                    break;
            }
        }

        private void FireEvent(MyTouchEffect myTouchEffect, int id, TouchActionType actionType, Point[] pointerLocations,
            bool isInContact)
        {
            // Get the method to call for firing events
            Action<Xamarin.Forms.Element, TouchActionEventArgs> onTouchAction =
                myTouchEffect.libMyTouchEffect.OnTouchAction;

            // Get the location of the pointer within the view
            int[] twoIntArray = new int[2];
            myTouchEffect._view.GetLocationOnScreen(twoIntArray);
            List<Point> locations = new List<Point>();
            foreach (var loc in pointerLocations)
            {
                var x = loc.X - twoIntArray[0];
                var y = loc.Y - twoIntArray[1];
                var point = new Point(_fromPixels(x), _fromPixels(y));
                locations.Add(point);
            }

            // Call the method
            onTouchAction(myTouchEffect._formsElement,
                new TouchActionEventArgs(id, actionType, locations.ToArray(), isInContact));
        }
    }
}