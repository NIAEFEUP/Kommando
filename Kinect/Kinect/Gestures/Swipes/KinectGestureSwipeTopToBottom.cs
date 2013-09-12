using Kinect.Gestures.Swipes.Frames;

namespace Kinect.Gestures.Swipes
{
    /// <summary>
    /// This classes is used to check for a swipe gesture, from top to bottom.
    /// </summary>
    public class KinectGestureSwipeTopToBottom : KinectGesture
    {
        /// <summary>
        /// Creates a new swipe top to bottom gesture.
        /// </summary>
        public KinectGestureSwipeTopToBottom()
            : base(KinectGestureType.None, null)
        {
            this.gestureType = KinectGestureType.SwipeTopToBottom;
            this.gestureFrames = this.CreateGestureFrames();
        }

        /// <summary>
        /// Creates the frame sequence that defines the gesture.
        /// </summary>
        /// <returns>Gesture's frame sequence</returns>
        private IKinectGestureFrame[] CreateGestureFrames()
        {
            KinectGestureSwipeTopToBottomFrame1 frame1 = new KinectGestureSwipeTopToBottomFrame1();
            KinectGestureSwipeTopToBottomFrame2 frame2 = new KinectGestureSwipeTopToBottomFrame2();
            KinectGestureSwipeTopToBottomFrame3 frame3 = new KinectGestureSwipeTopToBottomFrame3();
            IKinectGestureFrame[] gesture = new IKinectGestureFrame[5] { frame1, frame2, frame3, frame2, frame1 };
            return gesture;
        }
    }
}