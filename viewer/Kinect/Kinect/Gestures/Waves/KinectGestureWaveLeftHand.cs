using Kinect.Gestures.Waves.Frames;

namespace Kinect.Gestures.Waves
{
    /// <summary>
    /// This classes is used to check for a wave gesture, with the left hand.
    /// </summary>
    public class KinectGestureWaveLeftHand : KinectGesture
    {
        /// <summary>
        /// Creates a new wave with left hand gesture.
        /// </summary>
        public KinectGestureWaveLeftHand()
            : base(KinectGestureType.None, null)
        {
            this.gestureType = KinectGestureType.WaveLeftHand;
            this.gestureFrames = this.CreateGestureFrames();
        }

        /// <summary>
        /// Creates the frame sequence that defines the gesture.
        /// </summary>
        /// <returns>Gesture's frame sequence</returns>
        private IKinectGestureFrame[] CreateGestureFrames()
        {
            KinectGestureWaveLeftHandFrame1 frame1 = new KinectGestureWaveLeftHandFrame1();
            KinectGestureWaveLeftHandFrame2 frame2 = new KinectGestureWaveLeftHandFrame2();
            IKinectGestureFrame[] gesture = new IKinectGestureFrame[6] { frame1, frame2, frame1, frame2, frame1, frame2 };
            return gesture;
        }
    }
}