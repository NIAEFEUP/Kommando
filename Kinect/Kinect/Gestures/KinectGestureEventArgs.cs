using System;

namespace Kinect.Gestures
{
    /// <summary>
    /// This class is used to pass arguments to the "gesture recognized"
    /// event handlers.
    /// </summary>
    public class KinectGestureEventArgs : EventArgs
    {
        private KinectGestureType gestureType;
        private int trackingId;

        /// <summary>
        /// Creates a new KinectGestureEventArgs instance.
        /// </summary>
        /// <param name="type">Type of the recognized gesture</param>
        /// <param name="trackingId">TrackingID of the analyzed skeleton</param>
        public KinectGestureEventArgs(KinectGestureType type, int trackingId)
        {
            this.gestureType = type;
            this.trackingId = trackingId;
        }

        /// <summary>
        /// Sets or gets the type of gesture associated with the triggered event.
        /// </summary>
        public KinectGestureType GestureType
        {
            get { return this.gestureType; }
            set { this.gestureType = value; }
        }

        /// <summary>
        /// Gets or sets the TrackingID of the skeleton that caused the event
        /// to fire.
        /// </summary>
        public int TrackingId
        {
            get { return this.trackingId; }
            set { this.trackingId = value; }
        }
    }
}