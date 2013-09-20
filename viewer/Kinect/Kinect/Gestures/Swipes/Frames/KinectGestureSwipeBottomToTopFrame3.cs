using Microsoft.Kinect;

namespace Kinect.Gestures.Swipes.Frames
{
    /// <summary>
    /// Describes the third frame of a bottom to top swipe.
    /// </summary>
    public class KinectGestureSwipeBottomToTopFrame3 : IKinectGestureFrame
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
            // Checks if right hand right of the right shoulder.
            if (skeleton.Joints[JointType.HandRight].Position.X > skeleton.Joints[JointType.ShoulderRight].Position.X)
            {
                // Checks if right hand is above the right elbow.
                if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.ElbowRight].Position.Y)
                {
                    // Checks if the right hand is above the head.
                    if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.Head].Position.Y)
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