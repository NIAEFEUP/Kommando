using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ControLib;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Utility
{
    public class Player
    {
        public SightsControl Sights { get; set; }
        public ScoreControl Score { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public ShotStatus Status { get; set; }
        public bool PlayerActive { get; set; }

        public Player()
        {
        }
    }

    public enum ShotStatus
    {
        None,
        Hit,
        Miss,
        Defend
    }
}
