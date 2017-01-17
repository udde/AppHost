using Android.Runtime;
using Android.Views;
using System;

namespace AppHost.Droid.WebContainer
{
    public class NativeWebView : Android.Webkit.WebView
    {
        /// <summary>
        /// The detector
        /// </summary>
        private readonly GestureDetector detector;

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeWebView"/> class.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        public NativeWebView(AdvancedWebViewRenderer renderer) : base(renderer.Context)
        {
            var listener = new MyGestureListener(renderer);
            this.detector = new GestureDetector(this.Context, listener);
        }

        /// <summary>
        /// This is an Android specific constructor that sometimes needs to be called by the underlying
        /// Xamarin ACW environment.
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="handle"></param>
        public NativeWebView(IntPtr ptr, JniHandleOwnership handle) : base(ptr, handle)
        {

        }

        /// <summary>
        /// Implement this method to handle touch screen motion events.
        /// </summary>
        /// <param name="e">The motion event.</param>
        /// <returns>To be added.</returns>
        /// <since version="Added in API level 1" />
        /// <remarks><para tool="javadoc-to-mdoc">Implement this method to handle touch screen motion events.</para>
        /// <para tool="javadoc-to-mdoc">
        ///   <format type="text/html">
        ///     <a href="http://developer.android.com/reference/android/view/View.html#onTouchEvent(android.view.MotionEvent)" target="_blank">[Android Documentation]</a>
        ///   </format>
        /// </para></remarks>
        public override bool OnTouchEvent(MotionEvent e)
        {
            this.detector.OnTouchEvent(e);
            return base.OnTouchEvent(e);
        }

        /// <summary>
        /// Class MyGestureListener.
        /// </summary>
        private class MyGestureListener : GestureDetector.SimpleOnGestureListener
        {
            /// <summary>
            /// The swip e_ mi n_ distance
            /// </summary>
            private const int SWIPE_MIN_DISTANCE = 120;
            /// <summary>
            /// The swip e_ ma x_ of f_ path
            /// </summary>
            private const int SWIPE_MAX_OFF_PATH = 200;
            /// <summary>
            /// The swip e_ threshol d_ velocity
            /// </summary>
            private const int SWIPE_THRESHOLD_VELOCITY = 200;

            /// <summary>
            /// The web hybrid
            /// </summary>
            private readonly WeakReference<AdvancedWebViewRenderer> webHybrid;

            /// <summary>
            /// Initializes a new instance of the <see cref="MyGestureListener"/> class.
            /// </summary>
            /// <param name="renderer">The renderer.</param>
            public MyGestureListener(AdvancedWebViewRenderer renderer)
            {
                this.webHybrid = new WeakReference<AdvancedWebViewRenderer>(renderer);
            }

            //                public override void OnLongPress(MotionEvent e)
            //                {
            //                    Console.WriteLine("OnLongPress");
            //                    base.OnLongPress(e);
            //                }
            //
            //                public override bool OnDoubleTap(MotionEvent e)
            //                {
            //                    Console.WriteLine("OnDoubleTap");
            //                    return base.OnDoubleTap(e);
            //                }
            //
            //                public override bool OnDoubleTapEvent(MotionEvent e)
            //                {
            //                    Console.WriteLine("OnDoubleTapEvent");
            //                    return base.OnDoubleTapEvent(e);
            //                }
            //
            //                public override bool OnSingleTapUp(MotionEvent e)
            //                {
            //                    Console.WriteLine("OnSingleTapUp");
            //                    return base.OnSingleTapUp(e);
            //                }
            //
            //                public override bool OnDown(MotionEvent e)
            //                {
            //                    Console.WriteLine("OnDown");
            //                    return base.OnDown(e);
            //                }

            /// <summary>
            /// Called when [fling].
            /// </summary>
            /// <param name="e1">The e1.</param>
            /// <param name="e2">The e2.</param>
            /// <param name="velocityX">The velocity x.</param>
            /// <param name="velocityY">The velocity y.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
            {
                AdvancedWebViewRenderer hybrid;

                if (this.webHybrid.TryGetTarget(out hybrid) && Math.Abs(velocityX) > SWIPE_THRESHOLD_VELOCITY)
                {
                    if (e1.GetX() - e2.GetX() > SWIPE_MIN_DISTANCE)
                    {
                        hybrid.Element.OnLeftSwipe(this, EventArgs.Empty);
                    }
                    else if (e2.GetX() - e1.GetX() > SWIPE_MIN_DISTANCE)
                    {
                        hybrid.Element.OnRightSwipe(this, EventArgs.Empty);
                    }
                }

                return base.OnFling(e1, e2, velocityX, velocityY);
            }

            //                public override bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
            //                {
            //                    Console.WriteLine("OnScroll");
            //                    return base.OnScroll(e1, e2, distanceX, distanceY);
            //                }
            //
            //                public override void OnShowPress(MotionEvent e)
            //                {
            //                    Console.WriteLine("OnShowPress");
            //                    base.OnShowPress(e);
            //                }
            //
            //                public override bool OnSingleTapConfirmed(MotionEvent e)
            //                {
            //                    Console.WriteLine("OnSingleTapConfirmed");
            //                    return base.OnSingleTapConfirmed(e);
            //                }

        }
    }
}