using Microsoft.Kinect;
using System;
using System.Collections.Generic;

namespace Kinect.Gestures
{
    /// <summary>
    /// Controls the application's gesture recognition, feeding all the gestures
    /// current skeletal data.
    /// </summary>
    public class KinectGestureController
    {
        private List<KinectGesture> recognizableGestures = new List<KinectGesture>();

        /// <summary>
        /// Creates a new gesture controller instance.
        /// </summary>
        public KinectGestureController()
        {
        }

        /// <summary>
        /// Event that triggers when a gesture is recognized. Propagates the callback
        /// upwards.
        /// </summary>
        public event EventHandler<KinectGestureEventArgs> KinectGestureRecognized;

        /// <summary>
        /// Adds a new complex gesture to the gestures controller.
        /// </summary>
        /// <param name="type">Type of the gesture to be added</param>
        /// <param name="frames">Frames that represent the complex gesture</param>
        public void AddGesture(KinectGestureType type, IKinectGestureFrame[] frames)
        {
            KinectGesture gesture = new KinectGesture(type, frames);

            // Bind the controller "gesture recognized" event trigger to the
            // added gesture.
            gesture.KinectGestureRecognized += new EventHandler<KinectGestureEventArgs>(this.KinectGesture_KinectGestureRecognized);

            this.recognizableGestures.Add(gesture);
        }

        /// <summary>
        /// Adds a new complex gesture to the gestures controller.
        /// </summary>
        /// <param name="gesture">Gesture to be added</param>
        public void AddGesture(KinectGesture gesture)
        {
            // Bind the controller "gesture recognized" event trigger to the
            // added gesture.
            gesture.KinectGestureRecognized += new EventHandler<KinectGestureEventArgs>(this.KinectGesture_KinectGestureRecognized);

            this.recognizableGestures.Add(gesture);
        }

        /// <summary>
        /// Callback method for any "gestured recognized" event that is triggered.
        /// Will in turn invoke the registered callback of the client, if any is available.
        /// </summary>
        /// <param name="sender">Object that triggered the event</param>
        /// <param name="e">Triggered event arguments</param>
        public void KinectGesture_KinectGestureRecognized(object sender, KinectGestureEventArgs e)
        {
            // Inkoke callback method if the client registered one.
            if (this.KinectGestureRecognized != null)
            {
                this.KinectGestureRecognized(this, e);
            }

            // Reset all gesture states
            foreach (KinectGesture gesture in this.recognizableGestures)
            {
                gesture.ResetGesture();
            }
        }

        /// <summary>
        /// Updates all the gestures' states with the most recent skeletal data.
        /// </summary>
        /// <param name="skeleton">Skeleton to analyze</param>
        public void UpdateGestures(Skeleton skeleton)
        {
            foreach (KinectGesture gesture in this.recognizableGestures)
            {
                gesture.UpdateGesture(skeleton);
            }
        }
    }
}