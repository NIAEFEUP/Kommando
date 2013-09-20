using Microsoft.Kinect;

namespace Kinect.Gestures.Swipes.Frames
{
    /// <summary>
    /// Describes the third frame of a top to botom swipe.
    /// </summary>
    public class KinectGestureSwipeTopToBottomFrame3 : IKinectGestureFrame
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
            // Checks if left hand left of the left shoulder.
            if (skeleton.Joints[JointType.HandLeft].Position.X < skeleton.Joints[JointType.ShoulderLeft].Position.X)
            {
                // Checks if left hand is above the left elbow.
                if (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.ElbowLeft].Position.Y)
                {
                    // Checks if the left hand is above the head.
                    if (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.Head].Position.Y)
                    {
                        // The third part of the gesture was completed.
                        return KinectGestureResult.Success;
                    }
                    else
                    {
                        // Will pause recognition and try later.
                        return KinectGestureResult.Waiting;
                    }
                }

                // The standard result is failure.
                return KinectGestureResult.Fail;
            }

            // The standard result is failure.
            return KinectGestureResult.Fail;
        }
    }
}