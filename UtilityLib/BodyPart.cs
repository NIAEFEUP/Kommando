using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Utility
{
    public enum BodyPartState
    {
        PreGame,
        NotHit,
        Hit
    }

    public class BodyPart
    {
        public static readonly TimeSpan AnimationDuration = new TimeSpan(0, 0, 2);

        public BodyPart()
        {
            // Sets up the body part's shape.
            this.Shape = new Polygon
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                StrokeThickness = 2.0,
                Fill = Brushes.Black,
                Stroke = Brushes.White
            };

            // Sets up the animation for the body part.
            this.FillAnimation = new Storyboard();
            ColorAnimation ca = new ColorAnimation(Colors.White, new Duration(BodyPart.AnimationDuration));
            Storyboard.SetTarget(ca, this.Shape);
            Storyboard.SetTargetProperty(ca, new PropertyPath("Fill.Color", new object[] { Polygon.FillProperty, SolidColorBrush.ColorProperty }));
            this.FillAnimation.Children.Add(ca);
        }

        public Storyboard FillAnimation { get; private set; }

        public Polygon Shape { get; private set; }

        public BodyPartState State
        {
            set
            {
                // Stop animation.
                this.FillAnimation.Stop();
                switch (value)
                {
                    case BodyPartState.Hit:
                        ((ColorAnimation)this.FillAnimation.Children[0]).To = Colors.Red;
                        this.FillAnimation.Begin();
                        break;

                    case BodyPartState.NotHit:
                        ((ColorAnimation)this.FillAnimation.Children[0]).To = Colors.SkyBlue;
                        this.FillAnimation.Begin();
                        break;

                    case BodyPartState.PreGame:
                        ((ColorAnimation)this.FillAnimation.Children[0]).To = Colors.Black;
                        this.FillAnimation.Begin();
                        break;
                }
            }
        }
    }
}