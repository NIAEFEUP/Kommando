using Kinect.Gestures.Swipes.Frames;

namespace Kinect.Gestures.Swipes
{
    /// <summary>
    /// This classes is used to check for a swipe gesture, from left to right.
    /// </summary>
    public class KinectGestureSwipeLeftToRight : KinectGesture
    {
        /// <summary>
        /// Creates a new swipe left to right gesture.
        /// </summary>
        public KinectGestureSwipeLeftToRight()
            : base(KinectGestureType.None, null)
        {
            this.gestureType = KinectGestureType.SwipeLeftToRight;
            this.gestureFrames = this.CreateGestureFrames();
        }

        /// <summary>
        /// Creates the frame sequence that defines the gesture.
        /// </summary>
        /// <returns>Gesture's frame sequence</returns>
        private IKinectGestureFrame[] CreateGestureFrames()
        {
            KinectGestureSwipeLeftToRightFrame1 frame1 = new KinectGestureSwipeLeftToRightFrame1();
            KinectGestureSwipeLeftToRightFrame2 frame2 = new KinectGestureSwipeLeftToRightFrame2();
            KinectGestureSwipeLeftToRightFrame3 frame3 = new KinectGestureSwipeLeftToRightFrame3();
            IKinectGestureFrame[] gesture = new IKinectGestureFrame[3] { frame1, frame2, frame3 };
            return gesture;
        }
    }
}