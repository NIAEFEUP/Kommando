using Kinect.Gestures.Circles.Frames;

namespace Kinect.Gestures.Circles
{
    /// <summary>
    /// This classes is used to check for a circle gesture, with the right hand.
    /// </summary>
    public class KinectGestureCircleRightHand : KinectGesture
    {
        /// <summary>
        /// Creates a new circle gesture with the right hand.
        /// </summary>
        public KinectGestureCircleRightHand()
            : base(KinectGestureType.None, null)
        {
            this.gestureType = KinectGestureType.CircleRightHand;
            this.gestureFrames = this.CreateGestureFrames();
        }

        /// <summary>
        /// Creates the frame sequence that defines the gesture.
        /// </summary>
        /// <returns>Gesture's frame sequence</returns>
        private IKinectGestureFrame[] CreateGestureFrames()
        {
            KinectGestureCircleRightHandFrame1 frame1 = new KinectGestureCircleRightHandFrame1();
            KinectGestureCircleRightHandFrame2 frame2 = new KinectGestureCircleRightHandFrame2();
            KinectGestureCircleRightHandFrame3 frame3 = new KinectGestureCircleRightHandFrame3();
            KinectGestureCircleRightHandFrame4 frame4 = new KinectGestureCircleRightHandFrame4();
            IKinectGestureFrame[] gesture = new IKinectGestureFrame[5] { frame1, frame2, frame3, frame4, frame1 };
            return gesture;
        }
    }
}