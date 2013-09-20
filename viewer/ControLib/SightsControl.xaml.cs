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
    /// Interaction logic for SightsControl.xaml
    /// </summary>
    public partial class SightsControl : UserControl
    {
        public SightsControl()
        {
            InitializeComponent();
        }

        public Storyboard FireAnimationStoryBoard
        {
            get
            {
                return (Storyboard)FindResource("SightAnimationStoryboard");
            }
        }

        public double SightsWidth
        {
            get { return (double)GetValue(SightsWidthProperty); }
            set { SetValue(SightsWidthProperty, value); }
        }

        public static readonly DependencyProperty SightsWidthProperty =
            DependencyProperty.Register("SightsWidth", typeof(double), typeof(SightsControl), new PropertyMetadata(0.0));

        public Brush SightsColor
        {
            get { return (Brush)GetValue(SightsColorProperty); }
            set { SetValue(SightsColorProperty, value); }
        }

        public static readonly DependencyProperty SightsColorProperty =
            DependencyProperty.Register("SightsColor", typeof(Brush), typeof(SightsControl), new PropertyMetadata(Brushes.White));

        public double SightsClose
        {
            get { return (double)GetValue(SightsCloseProperty); }
            set { SetValue(SightsCloseProperty, value); }
        }

        public static readonly DependencyProperty SightsCloseProperty =
            DependencyProperty.Register("SightsClose", typeof(double), typeof(SightsControl), new PropertyMetadata(0.0));

        public void Fire()
        {
            this.FireAnimationStoryBoard.Begin();
        }

        public double SightsInnerWidth
        {
            get { return (double)GetValue(SightsInnerWidthProperty); }
            set { SetValue(SightsInnerWidthProperty, value); }
        }

        public static readonly DependencyProperty SightsInnerWidthProperty =
            DependencyProperty.Register("SightsInnerWidth", typeof(double), typeof(SightsControl), new PropertyMetadata(0.0));
    }
}
