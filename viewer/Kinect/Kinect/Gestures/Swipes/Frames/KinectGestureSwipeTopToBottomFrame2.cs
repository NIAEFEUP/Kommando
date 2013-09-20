using Microsoft.Kinect;

namespace Kinect.Gestures.Swipes.Frames
{
    /// <summary>
    /// Describes the second frame of a top to bottom swipe.
    /// </summary>
    public class KinectGestureSwipeTopToBottomFrame2 : IKinectGestureFrame
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
                // Checks if left hand is below the left shoulder.
                if (skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.ShoulderLeft].Position.Y)
                {
                    // Checks if the left hand is above the left elbow.
                    if (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.ElbowLeft].Position.Y)
                    {
                        // The second part of the gesture was completed.
                        return KinectGestureResult.Success;
                    }
                    else
                    {
                        // Will pause recognition and try later.
                        return KinectGestureResult.Waiting;
                    }
                }

                // The standard result is failure.
                return KinectGestureResult.Waiting;
            }

            // The standard result is failure.
            return KinectGestureResult.Fail;
        }
    }
}