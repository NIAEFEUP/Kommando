using Microsoft.Kinect;

namespace Kinect.Gestures.Swipes.Frames
{
    /// <summary>
    /// Describes the third frame of a left to right swipe.
    /// </summary>
    public class KinectGestureSwipeLeftToRightFrame3 : IKinectGestureFrame
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
            // Checks if right hand is down.
            if (skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.ElbowRight].Position.Y)
            {
                // Checks if left hand is below the left shoulder.
                if (skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.ShoulderLeft].Position.Y)
                {
                    // Checks if left hand is above the hip
                    if (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.HipLeft].Position.Y)
                    {
                        // Checks if the left hand is at the right of the neck.
                        if (skeleton.Joints[JointType.HandLeft].Position.X > skeleton.Joints[JointType.Spine].Position.X)
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