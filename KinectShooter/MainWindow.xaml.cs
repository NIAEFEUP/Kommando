using Kinect.Gestures;
using Kinect.Gestures.Waves;
using Kinect.Sensor;
using Microsoft.Kinect;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Utility;

namespace KinectShooter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Storyboard backgroundAnimation;
        private KinectSensorController controller;
        private HitSkeleton hitSkeleton;
        private int skeletonId = -1;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles gesture recognized events.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="args">Event arguments</param>
        public void callback_KinectGestureRecognized(object sender, KinectGestureEventArgs args)
        {
            // Engage animations.
            if (args.GestureType == KinectGestureType.WaveRightHand && this.skeletonId == -1)
            {
                this.skeletonId = args.TrackingId;
                this.controller.StartTrackingSkeleton(this.skeletonId);
                foreach (BodyPart part in hitSkeleton.BodyParts.Values)
                {
                    part.State = BodyPartState.NotHit;
                }

                // Paddle animation.
                this.hitSkeleton.PaddleRight.State = PaddleState.NotHit;

                // Background animation.
                this.backgroundAnimation.Stop();
                ((ColorAnimation)this.backgroundAnimation.Children[0]).To = Colors.Transparent;
                this.backgroundAnimation.Begin();
            }
            // Disengage animations (to remove later in production).
            else if (args.GestureType == KinectGestureType.WaveLeftHand && this.skeletonId != -1)
            {
                this.skeletonId = -1;
                this.controller.StopTrackingSkeleton();
                foreach (BodyPart part in hitSkeleton.BodyParts.Values)
                {
                    part.State = BodyPartState.PreGame;
                }

                // Paddle animation.
                this.hitSkeleton.PaddleRight.State = PaddleState.PreGame;

                // Background animation.
                this.backgroundAnimation.Stop();
                ((ColorAnimation)this.backgroundAnimation.Children[0]).To = Colors.Black;
                this.backgroundAnimation.Begin();
            }
        }

        /// <summary>
        /// Handles skeleton frame ready events.
        /// </summary>
        /// <param name="skeleton">Tracked skeleton</param>
        public void callback_TrackedSkeletonReady(Skeleton skeleton)
        {
            hitSkeleton.UpdateSkeleton(skeleton, controller.Sensor, (float)MainCanvas.ActualWidth, (float)MainCanvas.ActualHeight);
            if (this.skeletonId != -1 && ((skeleton == null) || (this.skeletonId != skeleton.TrackingId)))
            {
                this.skeletonId = -1;
                this.controller.StopTrackingSkeleton();
                foreach (BodyPart part in hitSkeleton.BodyParts.Values)
                {
                    part.State = BodyPartState.PreGame;
                }

                // Background animation.
                this.backgroundAnimation.Stop();
                ((ColorAnimation)this.backgroundAnimation.Children[0]).To = Colors.Black;
                this.backgroundAnimation.Begin();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Binds the skeleton representation to the canvas.
            hitSkeleton = new HitSkeleton();
            foreach (BodyPart part in hitSkeleton.BodyParts.Values)
            {
                MainCanvas.Children.Add(part.Shape);
            }
            MainCanvas.Children.Add(this.hitSkeleton.PaddleRight.Shape);

            // Setup background animation.
            this.backgroundAnimation = new Storyboard();
            ColorAnimation ca = new ColorAnimation(Colors.White, new Duration(BodyPart.AnimationDuration));
            Storyboard.SetTarget(ca, this.MainCanvas);
            Storyboard.SetTargetProperty(ca, new PropertyPath("Background.Color", new object[] { Canvas.BackgroundProperty, SolidColorBrush.ColorProperty }));
            this.backgroundAnimation.Children.Add(ca);

            // Sets up and starts the sensor.
            controller = new KinectSensorController(KinectSensorType.Xbox360Sensor);
            controller.TrackedSkeletonReady += new KinectSensorController.TrackedSkeletonReadyHandler(callback_TrackedSkeletonReady);
            controller.Gestures.KinectGestureRecognized += new EventHandler<KinectGestureEventArgs>(callback_KinectGestureRecognized);
            controller.Gestures.AddGesture(new KinectGestureWaveRightHand());
            controller.Gestures.AddGesture(new KinectGestureWaveLeftHand());
            controller.StartSensor();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            // Stops the sensor if it's still running.
            if (this.controller.Sensor.IsRunning)
            {
                this.controller.StopSensor();
            }
        }
    }
}