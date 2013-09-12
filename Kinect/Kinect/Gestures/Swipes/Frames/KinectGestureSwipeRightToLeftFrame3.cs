using Microsoft.Kinect;

namespace Kinect.Gestures.Swipes.Frames
{
    /// <summary>
    /// Describes the third frame of a right to left swipe.
    /// </summary>
    public class KinectGestureSwipeRightToLeftFrame3 : IKinectGestureFrame
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
                // Checks if right hand is below the right shoulder.
                if (skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.ShoulderRight].Position.Y)
                {
                    // Checks if right hand is above the hip
                    if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.HipRight].Position.Y)
                    {
                        // Checks if the right hand is at the left of the neck.
                        if (skeleton.Joints[JointType.HandRight].Position.X < skeleton.Joints[JointType.Spine].Position.X)
                        {
                            // The third part of the gesture was completed.
                            return KinectGestureResult.Success;
                        }

                        // Gesture was not completed, but it's still possible to achieve.
                        else
                        {
                            // Will pause recognition and try later.
                            return KinectGestureResult.Waiting;
                        }
                    }

                    // The standard result is failure.
                    return KinectGestureResult.Fail;
                }
            }

            // The standard result is failure.
            return KinectGestureResult.Fail;
        }
    }
}