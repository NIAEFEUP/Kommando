using Kinect.Gestures.Swipes.Frames;

namespace Kinect.Gestures.Swipes
{
    /// <summary>
    /// This classes is used to check for a swipe gesture, from bottom to top.
    /// </summary>
    public class KinectGestureSwipeBottomToTop : KinectGesture
    {
        /// <summary>
        /// Creates a new swipe bottom to top gesture.
        /// </summary>
        public KinectGestureSwipeBottomToTop()
            : base(KinectGestureType.None, null)
        {
            this.gestureType = KinectGestureType.SwipeBottomToTop;
            this.gestureFrames = this.CreateGestureFrames();
        }

        /// <summary>
        /// Creates the frame sequence that defines the gesture.
        /// </summary>
        /// <returns>Gesture's frame sequence</returns>
        private IKinectGestureFrame[] CreateGestureFrames()
        {
            KinectGestureSwipeBottomToTopFrame1 frame1 = new KinectGestureSwipeBottomToTopFrame1();
            KinectGestureSwipeBottomToTopFrame2 frame2 = new KinectGestureSwipeBottomToTopFrame2();
            KinectGestureSwipeBottomToTopFrame3 frame3 = new KinectGestureSwipeBottomToTopFrame3();
            IKinectGestureFrame[] gesture = new IKinectGestureFrame[5] { frame1, frame2, frame3, frame2, frame1 };
            return gesture;
        }
    }
}