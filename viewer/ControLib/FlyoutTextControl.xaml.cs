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
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ControLib
{
    /// <summary>
    /// Interaction logic for FlyoutTextControl.xaml
    /// </summary>
    public partial class FlyoutTextControl : UserControl
    {
        public FlyoutTextControl()
        {
            InitializeComponent();
        }

        public float X { get; set; }
        public float Y { get; set; }

        public double FlyoutTextSize
        {
            get { return (double)GetValue(FlyoutTextSizeProperty); }
            set { SetValue(FlyoutTextSizeProperty, value); }
        }

        public static readonly DependencyProperty FlyoutTextSizeProperty =
            DependencyProperty.Register("FlyoutTextSize", typeof(double), typeof(FlyoutTextControl), new PropertyMetadata(10.0));

        public Brush FlyoutTextColor
        {
            get { return (Brush)GetValue(FlyoutTextColorProperty); }
            set { SetValue(FlyoutTextColorProperty, value); }
        }

        public static readonly DependencyProperty FlyoutTextColorProperty =
            DependencyProperty.Register("FlyoutTextColor", typeof(Brush), typeof(FlyoutTextControl), new PropertyMetadata(Brushes.White));

        public string FlyoutTextContent
        {
            get { return (string)GetValue(FlyoutTextContentProperty); }
            set { SetValue(FlyoutTextContentProperty, value); }
        }

        public static readonly DependencyProperty FlyoutTextContentProperty =
            DependencyProperty.Register("FlyoutTextContent", typeof(string), typeof(FlyoutTextControl), new PropertyMetadata(""));

        public void Flyout()
        {
            Storyboard flyoutAnimation = (Storyboard)FindResource("FlyoutAnimationStoryboard");
            flyoutAnimation.Begin();
        }

        public Storyboard FlyoutAnimation
        {
            get
            {
                return (Storyboard)FindResource("FlyoutAnimationStoryboard");
            }
        }
    }
}
