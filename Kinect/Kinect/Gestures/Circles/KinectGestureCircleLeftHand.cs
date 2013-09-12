using Kinect.Gestures.Circles.Frames;

namespace Kinect.Gestures.Circles
{
    /// <summary>
    /// This classes is used to check for a circle gesture, with the left hand.
    /// </summary>
    public class KinectGestureCircleLeftHand : KinectGesture
    {
        /// <summary>
        /// Creates a new circle gesture with the left hand.
        /// </summary>
        public KinectGestureCircleLeftHand()
            : base(KinectGestureType.None, null)
        {
            this.gestureType = KinectGestureType.CircleLeftHand;
            this.gestureFrames = this.CreateGestureFrames();
        }

        /// <summary>
        /// Creates the frame sequence that defines the gesture.
        /// </summary>
        /// <returns>Gesture's frame sequence</returns>
        private IKinectGestureFrame[] CreateGestureFrames()
        {
            KinectGestureCircleLeftHandFrame1 frame1 = new KinectGestureCircleLeftHandFrame1();
            KinectGestureCircleLeftHandFrame2 frame2 = new KinectGestureCircleLeftHandFrame2();
            KinectGestureCircleLeftHandFrame3 frame3 = new KinectGestureCircleLeftHandFrame3();
            KinectGestureCircleLeftHandFrame4 frame4 = new KinectGestureCircleLeftHandFrame4();
            IKinectGestureFrame[] gesture = new IKinectGestureFrame[5] { frame1, frame2, frame3, frame4, frame1 };
            return gesture;
        }
    }
}