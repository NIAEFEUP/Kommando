using Microsoft.Kinect;

namespace Kinect.Gestures.Circles.Frames
{
    /// <summary>
    /// Describes the first frame of a right hand circle.
    /// </summary>
    public class KinectGestureCircleRightHandFrame1 : IKinectGestureFrame
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
        public KinectGestureResult ProcessFrame(Skeleton skeleton)
        {
            // Checks if left hand is down.
            if (skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.ElbowLeft].Position.Y)
            {
                // Checks if the right hand is above of the right elbow
                if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.ElbowRight].Position.Y)
                {
                    // Checks if the right hand is right of the right elbow
                    if (skeleton.Joints[JointType.HandRight].Position.X > skeleton.Joints[JointType.ElbowRight].Position.X)
                    {
                        // The first part of the gesture was completed.
                        return KinectGestureResult.Success;
                    }

                    // The standard result is failure.
                    return KinectGestureResult.Waiting;
                }
            }

            // The standard result is failure.
            return KinectGestureResult.Fail;
        }
    }
}