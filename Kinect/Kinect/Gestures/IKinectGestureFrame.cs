using Microsoft.Kinect;

namespace Kinect.Gestures
{
    /// <summary>
    /// This interface is used to implemente single frames (parts)
    /// of a complex Kinect gesture.
    /// </summary>
    public interface IKinectGestureFrame
    {
        /// <summary>
        /// Checks if the given skeleton's tracking data matches
        /// the gesture represented by this frame.
        /// </summary>
        /// <param name="skeleton">Skeleton to analize</param>
        /// <returns>
        /// Success if the gesture was correct, Waiting if the
        /// gesture was not quite right, but still possible, Fail otherwise.
        /// </returns>
        KinectGestureResult ProcessFrame(Skeleton skeleton);
    }
}