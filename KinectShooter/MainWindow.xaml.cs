using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Kinect.Sensor;
using Kinect.Toolbox;
using Microsoft.Kinect;

namespace KinectShooter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensorController controller;

        public MainWindow()
        {
            InitializeComponent();
            controller = new KinectSensorController(KinectSensorType.Xbox360Sensor);
            controller.TrackedSkeletonReady += new KinectSensorController.TrackedSkeletonReadyHandler(callback_TrackedSkeletonReady);
            controller.StartSensor();
        }

        public void callback_TrackedSkeletonReady(Skeleton skeleton) 
        {
            MainCanvas.Children.Clear();
            foreach(Joint joint in skeleton.Joints)
            {
                Vector2 vector2 = Tools.ConvertSkeletonPointToScreen(controller.Sensor, joint.Position);
                float centerX = (float)((vector2.X * MainCanvas.ActualWidth / 2.0) + (MainCanvas.ActualWidth / 4.0));
                float centerY = (float)((vector2.Y * MainCanvas.ActualHeight / 2.0) + (MainCanvas.ActualHeight / 4.0));
                const double diameter = 8;

                Ellipse ellipse = new Ellipse
                {
                    Width = diameter,
                    Height = diameter,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Fill = Brushes.SkyBlue
                };
                Canvas.SetLeft(ellipse, centerX - ellipse.Width / 2);
                Canvas.SetTop(ellipse, centerY - ellipse.Height / 2);
                MainCanvas.Children.Add(ellipse);
            }
        }
    }
}
