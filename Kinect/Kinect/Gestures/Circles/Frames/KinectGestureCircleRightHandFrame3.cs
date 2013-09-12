using Microsoft.Kinect;

namespace Kinect.Gestures.Circles.Frames
{
    /// <summary>
    /// Describes the third frame of a right hand circle.
    /// </summary>
    public class KinectGestureCircleRightHandFrame3 : IKinectGestureFrame
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
                // Checks if the right hand is below the right elbow
                if (skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.ElbowRight].Position.Y)
                {
                    // Checks if the right hand is left of the right elbow
                    if (skeleton.Joints[JointType.HandRight].Position.X < skeleton.Joints[JointType.ElbowRight].Position.X)
                    {
                        // The third part of the gesture was completed.
                        return KinectGestureResult.Success;
                    }
                    else
                    {
                        // Gesture recognition will pause.
                        return KinectGestureResult.Waiting;
                    }
                }
            }

            // The standard result is failure.
            return KinectGestureResult.Fail;
        }
    }
}