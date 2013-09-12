using Kinect.Gestures.Waves.Frames;

namespace Kinect.Gestures.Waves
{
    /// <summary>
    /// This classes is used to check for a wave gesture, with the right hand.
    /// </summary>
    public class KinectGestureWaveRightHand : KinectGesture
    {
        /// <summary>
        /// Creates a new wave with right hand gesture.
        /// </summary>
        public KinectGestureWaveRightHand()
            : base(KinectGestureType.None, null)
        {
            this.gestureType = KinectGestureType.WaveRightHand;
            this.gestureFrames = this.CreateGestureFrames();
        }

        /// <summary>
        /// Creates the frame sequence that defines the gesture.
        /// </summary>
        /// <returns>Gesture's frame sequence</returns>
        private IKinectGestureFrame[] CreateGestureFrames()
        {
            KinectGestureWaveRightHandFrame1 frame1 = new KinectGestureWaveRightHandFrame1();
            KinectGestureWaveRightHandFrame2 frame2 = new KinectGestureWaveRightHandFrame2();
            IKinectGestureFrame[] gesture = new IKinectGestureFrame[6] { frame1, frame2, frame1, frame2, frame1, frame2 };
            return gesture;
        }
    }
}