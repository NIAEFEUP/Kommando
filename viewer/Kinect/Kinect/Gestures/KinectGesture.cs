using Microsoft.Kinect;
using System;

namespace Kinect.Gestures
{
    /// <summary>
    /// This class describes and analyzes complex gestures.
    /// </summary>
    public class KinectGesture
    {
        protected IKinectGestureFrame[] gestureFrames;
        protected KinectGestureType gestureType;
        private int currentFrameCount;
        private int currentFrameIndex;
        private bool gesturePaused;
        private int pausedFrameCount;

        /// <summary>
        /// Creates a new KinectGesture instance.
        /// </summary>
        /// <param name="type">Type of the recognizable gesture</param>
        /// <param name="gestureFrames">Sequence of frames that represent the gesture</param>
        public KinectGesture(KinectGestureType type, IKinectGestureFrame[] gestureFrames)
        {
            this.gestureType = type;
            this.gestureFrames = gestureFrames;
            this.currentFrameIndex = 0;
            this.currentFrameCount = 0;
            this.pausedFrameCount = 0;
            this.gesturePaused = false;
        }

        /// <summary>
        /// Event that triggers when a gesture is recognized. Propagates the callback
        /// to the gesture controller.
        /// </summary>
        public event EventHandler<KinectGestureEventArgs> KinectGestureRecognized;

        /// <summary>
        /// Resets the gesture state.
        /// </summary>
        public void ResetGesture()
        {
            this.currentFrameIndex = 0;
            this.currentFrameCount = 0;
            this.pausedFrameCount = 5;
            this.gesturePaused = true;
        }

        /// <summary>
        /// Updates the gesture state. If a gesture is successfully recognized
        /// an event is triggered.
        /// </summary>
        /// <param name="skeleton">Skeleton to analyze</param>
        public void UpdateGesture(Skeleton skeleton)
        {
            // Checks if the enough time between active frame checks
            // has passed, unpausing them if if true.
            if (this.gesturePaused)
            {
                if (this.currentFrameCount == this.pausedFrameCount)
                {
                    this.gesturePaused = false;
                }
                this.currentFrameCount++;
            }

            // Compares the skeleton data with the current frame.
            KinectGestureResult result = this.gestureFrames[this.currentFrameIndex].ProcessFrame(skeleton);

            // In case of a successful recognition, moves on to the next frame if there's any,
            // otherwise triggers the recognized gesture event.
            if (result == KinectGestureResult.Success)
            {
                if (this.currentFrameIndex + 1 < this.gestureFrames.Length)
                {
                    this.currentFrameIndex++;
                    this.currentFrameCount = 0;
                    this.pausedFrameCount = 10;
                    this.gesturePaused = true;
                }
                else
                {
                    // Triggers any registered event
                    if (this.KinectGestureRecognized != null)
                    {
                        KinectGestureEventArgs args = new KinectGestureEventArgs(this.gestureType, skeleton.TrackingId);
                        this.KinectGestureRecognized(this, args);
                    }

                    // Resets the gesture state.
                    this.ResetGesture();
                }
            }

            // If the gesture recognition failed beyond recovery, or the gesture is being
            // processed for to long, the gesture state is reseted.
            else if (result == KinectGestureResult.Fail || this.currentFrameCount == 50)
            {
                this.currentFrameIndex = 0;
                this.currentFrameCount = 0;
                this.pausedFrameCount = 5;
                this.gesturePaused = true;
            }

            // If the gesture recognition was inconclusive and there's still
            // time to wait, the gesture waits for the next frame.
            else
            {
                this.currentFrameCount++;
                this.pausedFrameCount = 5;
                this.gesturePaused = true;
            }
        }
    }
}