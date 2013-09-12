using Kinect.Gestures.Swipes.Frames;

namespace Kinect.Gestures.Swipes
{
    /// <summary>
    /// This classes is used to check for a swipe gesture, from right to left.
    /// </summary>
    public class KinectGestureSwipeRightToLeft : KinectGesture
    {
        /// <summary>
        /// Creates a new swipe right to left gesture.
        /// </summary>
        public KinectGestureSwipeRightToLeft()
            : base(KinectGestureType.None, null)
        {
            this.gestureType = KinectGestureType.SwipeRightToLeft;
            this.gestureFrames = this.CreateGestureFrames();
        }

        /// <summary>
        /// Creates the frame sequence that defines the gesture.
        /// </summary>
        /// <returns>Gesture's frame sequence</returns>
        private IKinectGestureFrame[] CreateGestureFrames()
        {
            KinectGestureSwipeRightToLeftFrame1 frame1 = new KinectGestureSwipeRightToLeftFrame1();
            KinectGestureSwipeRightToLeftFrame2 frame2 = new KinectGestureSwipeRightToLeftFrame2();
            KinectGestureSwipeRightToLeftFrame3 frame3 = new KinectGestureSwipeRightToLeftFrame3();
            IKinectGestureFrame[] gesture = new IKinectGestureFrame[3] { frame1, frame2, frame3 };
            return gesture;
        }
    }
}