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
        public static readonly TimeSpan AppearAnimationDuration = new TimeSpan(0, 0, 1);
        public static readonly TimeSpan HitAnimationDuration = new TimeSpan(0, 0, 0, 0, 600);
        private PaddleState state;

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
                StrokeThickness = 3.0,
                Opacity = 1.0,
                Fill = Brushes.Orange,
                Stroke = Brushes.Black
            };
            this.state = PaddleState.PreGame;

            // Sets up the appear animation.
            this.AppearAnimation = new Storyboard();
            DoubleAnimation da = new DoubleAnimation(1.0, new Duration(Paddle.AppearAnimationDuration));
            Storyboard.SetTarget(da, this.Shape);
            Storyboard.SetTargetProperty(da, new PropertyPath(Polygon.OpacityProperty));
            this.AppearAnimation.Children.Add(da);

            // Sets up the hit animation.
            this.FillAnimation = new Storyboard();
            ColorAnimation ca = new ColorAnimation(Colors.Red, Colors.Orange, new Duration(Paddle.HitAnimationDuration));
            Storyboard.SetTarget(ca, this.Shape);
            Storyboard.SetTargetProperty(ca, new PropertyPath("Fill.Color", new object[] { Polygon.FillProperty, SolidColorBrush.ColorProperty }));
            this.FillAnimation.Children.Add(ca);
            this.State = PaddleState.PreGame;
        }

        public PaddleState State
        {
            set
            {
                // Stop animation.
                this.FillAnimation.Stop();
                this.AppearAnimation.Stop();
                this.state = value == PaddleState.Hit ? this.state : value;
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
            get
            {
                return this.state;
            }
        }
    }
}