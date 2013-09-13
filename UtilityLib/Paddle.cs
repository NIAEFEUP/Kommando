using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Utility
{
    public enum PaddleState
    {
        PreGame,
        NotHit,
        Hit
    }

    public class Paddle
    {
        public static readonly TimeSpan AnimationDuration = new TimeSpan(0, 0, 2);
        public static readonly TimeSpan HitAnimationDuration = new TimeSpan(0, 0, 0, 500);

        public Storyboard FillAnimation { get; private set; }
        public Storyboard AppearAnimation { get; private set; }

        public Polygon Shape { get; private set; }

        public Paddle()
        {
            // Sets up the body part's shape.
            this.Shape = new Polygon
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                StrokeThickness = 4.0,
                Opacity = 0.0,
                Fill = Brushes.Orange,
                Stroke = Brushes.Black
            };

            // Sets up the hit animation.
            this.FillAnimation = new Storyboard();
            ColorAnimation ca = new ColorAnimation(Colors.Red, new Duration(Paddle.HitAnimationDuration));
            ca.AutoReverse = true;
            Storyboard.SetTarget(ca, this.Shape);
            Storyboard.SetTargetProperty(ca, new PropertyPath("Fill.Color", new object[] { Polygon.FillProperty, SolidColorBrush.ColorProperty }));
            this.FillAnimation.Children.Add(ca);

            // Sets up the appear animation.
            this.AppearAnimation = new Storyboard();
            DoubleAnimation da = new DoubleAnimation(1.0, new Duration(Paddle.AnimationDuration));
            Storyboard.SetTarget(da, this.Shape);
            Storyboard.SetTargetProperty(da, new PropertyPath(Polygon.OpacityProperty));
            this.AppearAnimation.Children.Add(da);
        }

        public PaddleState State
        {
            set
            {
                // Stop animation.
                this.FillAnimation.Stop();
                ColorAnimation ca = ((ColorAnimation)this.FillAnimation.Children[0]);
                DoubleAnimation da = ((DoubleAnimation)this.AppearAnimation.Children[0]);
                switch (value)
                {
                    case PaddleState.Hit:
                        this.FillAnimation.Begin();
                        break;

                    case PaddleState.NotHit:
                        da.To = 1.0;
                        this.AppearAnimation.Begin();
                        break;

                    case PaddleState.PreGame:
                        da.To = 0.0;
                        this.AppearAnimation.Begin();
                        break;
                }
            }
        }
    }
}
