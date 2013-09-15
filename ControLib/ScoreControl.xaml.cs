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

namespace ControLib
{
    /// <summary>
    /// Interaction logic for ScoreContol.xaml
    /// </summary>
    public partial class ScoreControl : UserControl
    {
        public ScoreControl()
        {
            InitializeComponent();
        }

        public Brush ScoreColor
        {
            get { return (Brush)GetValue(ScoreColorProperty); }
            set { SetValue(ScoreColorProperty, value); }
        }

        public static readonly DependencyProperty ScoreColorProperty =
            DependencyProperty.Register("ScoreColor", typeof(Brush), typeof(ScoreControl), new PropertyMetadata(Brushes.White));

        public string ScoreContent
        {
            get { return (string)GetValue(ScoreContentProperty); }
            set { SetValue(ScoreContentProperty, value); }
        }

        public static readonly DependencyProperty ScoreContentProperty =
            DependencyProperty.Register("ScoreContent", typeof(string), typeof(ScoreControl), new PropertyMetadata(""));
    }
}
