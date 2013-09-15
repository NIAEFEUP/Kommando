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
using System.Windows.Shapes;
using Kinect.Toolbox;
using ControLib;
using System.Threading;
using System.Linq;
using System.Text;

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
        private Player[] players;
        private TcpSocketClient client;

        // REMOVE
        private float xP = 0.0f;
        private float yP = 0.0f;
        // END REMOVE

        public MainWindow()
        {
            InitializeComponent();
        }

        public void AddPlayer(int index, string score)
        {
            // Players are only added in running games.
            if (this.skeletonId != -1)
            {
                // Reset player information.
                Player p = this.players[index];
                p.PlayerActive = true;
                p.Score.ScoreContent = score;
                p.Status = ShotStatus.None;
                p.Score.BeginStoryboard((Storyboard)this.FindResource("PlayerScoreAppearStoryboard"));
            }
        }

        public void RemovePlayer(int index)
        {
            Player p = this.players[index];
            p.PlayerActive = false;
            p.Score.BeginStoryboard((Storyboard)this.FindResource("PlayerScoreDisappearStoryboard"));
            Canvas.SetLeft(p.Sights, -500);
            Canvas.SetTop(p.Sights, -500);
        }

        public void MovePlayer(int index, float x, float y)
        {
            Player p = this.players[index];
            if (p.PlayerActive)
            {
                float width = (float)this.ActualWidth * x;
                float height = (float)this.ActualHeight * y;
                float sightsWidth = (float)p.Sights.ActualWidth * 0.5f;
                float sightsHeight = (float)p.Sights.ActualHeight * 0.5f;
                p.X = width;
                p.Y = height;
                Canvas.SetLeft(p.Sights, width - sightsWidth);
                Canvas.SetTop(p.Sights, height - sightsHeight);
            }
        }

        public void FirePlayer(int index)
        {
            Player p = this.players[index];
            if (p.PlayerActive)
            {
                p.Sights.Fire();
            }
        }

        public void ShotEnded(Player p)
        {
            // Locking here prevents concurrence from the network listener.
            lock (this.players)
            {
                if (p.PlayerActive)
                {
                    // Build bullet polygon.
                    Polygon pol = new Polygon();
                    pol.Points = hitSkeleton.RegularPolygonToPointCollection(new Vector2(p.X, p.Y), (float)p.Sights.ActualWidth * 0.5f, 15);

                    // Check for weapon hits.
                    if (Tools.IsPolygonColliding(pol, hitSkeleton.PaddleRight.Shape))
                    {
                        hitSkeleton.PaddleRight.State = PaddleState.Hit;
                        SetFlyout("DEFEND!", p.X, p.Y, Brushes.SlateBlue);
                        p.Status = ShotStatus.Miss;
                    }
                    else if (Tools.IsPolygonColliding(pol, hitSkeleton.PaddleLeft.Shape))
                    {
                        hitSkeleton.PaddleLeft.State = PaddleState.Hit;
                        SetFlyout("DEFEND!", p.X, p.Y, Brushes.SlateBlue);
                        p.Status = ShotStatus.Miss;
                    }
                    // Possible body part hits.
                    else
                    {
                        var hitParts = hitSkeleton.BodyParts.Where(pa => Tools.IsPolygonColliding(pa.Value.Shape, pol));
                        BodyPart part = hitParts.FirstOrDefault(pa => pa.Value.State == BodyPartState.NotHit).Value;
                        
                        // Only one part may be hit per shot, but precedence
                        // is always given to parts that haven't been shot before.
                        if(part != null)
                        {
                            part.State = BodyPartState.Hit;
                            SetFlyout("HIT!", p.X, p.Y, Brushes.SlateBlue);
                            p.Status = ShotStatus.Hit;
                        }
                        // A part previously hit was hit again.
                        else if (hitParts.Count() > 0)
                        {
                            SetFlyout("HIT!", p.X, p.Y, Brushes.SlateBlue);
                            p.Status = ShotStatus.Hit;
                        }
                        // Nothing was hit.
                        else
                        {
                            SetFlyout("MISSED!", p.X, p.Y, Brushes.SlateBlue);
                            p.Status = ShotStatus.Miss;
                        }
                    }

                    // Build response to server.
                    StringBuilder response = new StringBuilder("");
                    foreach (Player pl in this.players)
                    {
                        // Current shot player.
                        if (p == pl)
                        {
                            switch (pl.Status)
                            {
                                case ShotStatus.Hit:
                                    response.Append("2,");
                                    break;
                                case ShotStatus.Miss:
                                    response.Append("1,");
                                    break;
                                case ShotStatus.None:
                                    response.Append("0,");
                                    break;
                            }
                            pl.Status = ShotStatus.None;
                        }
                        // Other players.
                        else
                        {
                            // If player is active.
                            if (pl.PlayerActive)
                            {
                                response.Append("0,");
                            }
                            // If player is not active.
                            else
                            {
                                response.Append("-1,");
                            }
                        }
                    }

                    // Append player health to response and send it.
                    response.Append(hitSkeleton.DamageTaken);
                    client.BeginSend(response.ToString());
                }
            }
        }

        private void SetFlyout(string text, float x, float y, Brush color)
        {
            FlyoutTextControl flyout = new FlyoutTextControl
            {
                FlyoutTextColor = color,
                FlyoutTextContent = text,
                FlyoutTextSize = (float)this.ActualHeight * 0.05f
            };
            MainCanvas.Children.Add(flyout);
            flyout.X = x;
            flyout.Y = y;
            Canvas.SetLeft(flyout, x - flyout.ActualWidth * 0.5f);
            Canvas.SetTop(flyout, y - flyout.ActualHeight * 0.5f);
            flyout.FlyoutAnimation.Completed += (s, a) => MainCanvas.Children.Remove(flyout);
            flyout.Flyout();
        }

        public void callback_MessageReceived(TcpSocketClient client, string message)
        {
            // Data can only be processed if the player is alive.
            if (hitSkeleton.DamageTaken >= 1.0f)
            {
                return;
            }

            // Check for handshake communication.
            if (message == "whoareyou")
            {
                client.BeginSend("v");
                return;
            }

            // Checks if an invoke is required.
            if (MainCanvas.Dispatcher.CheckAccess())
            {
                string[] tokens = message.Split(',');
                lock (this.players)
                {
                    for (int i = 0; i < players.Length; ++i)
                    {
                        Player p = this.players[i];
                        float x = float.Parse(tokens[i * 4]);
                        float y = float.Parse(tokens[i * 4 + 1]);
                        int shot = int.Parse(tokens[i * 4 + 2]);
                        string score = tokens[i * 4 + 3];
                        p.Score.ScoreContent = score;
                        MovePlayer(i, x, y);

                        // Player exited.
                        if (shot == -1 && p.PlayerActive)
                        {
                            RemovePlayer(i);
                        }
                        // Player entered.
                        else if (shot != -1 && !p.PlayerActive)
                        {
                            AddPlayer(i, score);
                        }

                        // A player shot.
                        if (shot == 1)
                        {
                            FirePlayer(i);
                        }
                    }
                }
            }
            else
            {
                MainCanvas.Dispatcher.Invoke(new Action(() => callback_MessageReceived(client, message)));
            }
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
                this.hitSkeleton.PaddleLeft.State = PaddleState.NotHit;

                // Background animation.
                this.backgroundAnimation.Stop();
                ((ColorAnimation)this.backgroundAnimation.Children[0]).To = Colors.Transparent;
                this.backgroundAnimation.Begin();

                // Scoreboard animation.
                Storyboard sb = (Storyboard)this.FindResource("ScoreboardAppearStoryboard");
                sb.Begin();

                // Start TCP client.
                this.client = new TcpSocketClient();
                this.client.MessageReceived += callback_MessageReceived;
                this.client.BeginReceive();
            }
            /*// Disengage animations (to remove later in production).
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
                this.hitSkeleton.PaddleLeft.State = PaddleState.PreGame;

                // Background animation.
                this.backgroundAnimation.Stop();
                ((ColorAnimation)this.backgroundAnimation.Children[0]).To = Colors.Black;
                this.backgroundAnimation.Begin();

                // Scoreboard animation.
                Storyboard sb = (Storyboard)this.FindResource("ScoreboardDisappearStoryboard");
                sb.Begin();
            }*/
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

                // Paddle animation.
                this.hitSkeleton.PaddleRight.State = PaddleState.PreGame;
                this.hitSkeleton.PaddleLeft.State = PaddleState.PreGame;

                // Background animation.
                this.backgroundAnimation.Stop();
                ((ColorAnimation)this.backgroundAnimation.Children[0]).To = Colors.Black;
                this.backgroundAnimation.Begin();

                // Scores animation.
                for (int i = 0; i < players.Length; ++i)
                {
                    RemovePlayer(i);
                }

                // Scoreboard animation.
                Storyboard sb = (Storyboard)this.FindResource("ScoreboardDisappearStoryboard");
                sb.Begin();

                // Close server connection.
                if (this.client != null)
                {
                    this.client.EndReceive();
                    this.client.MessageReceived -= callback_MessageReceived;
                }
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
            MainCanvas.Children.Add(this.hitSkeleton.PaddleLeft.Shape);

            // Setup background animation.
            this.backgroundAnimation = new Storyboard();
            ColorAnimation ca = new ColorAnimation(Colors.White, new Duration(BodyPart.AnimationDuration));
            Storyboard.SetTarget(ca, this.MainCanvas);
            Storyboard.SetTargetProperty(ca, new PropertyPath("Background.Color", new object[] { Canvas.BackgroundProperty, SolidColorBrush.ColorProperty }));
            this.backgroundAnimation.Children.Add(ca);

            // Create all players.
            this.players = new Player[6];
            this.players[0] = new Player { Status = ShotStatus.None, Score = Player1Score, Sights = new ControLib.SightsControl { SightsColor = (Brush)this.FindResource("Player1Color") } };
            this.players[1] = new Player { Status = ShotStatus.None, Score = Player2Score, Sights = new ControLib.SightsControl { SightsColor = (Brush)this.FindResource("Player2Color") } };
            this.players[2] = new Player { Status = ShotStatus.None, Score = Player3Score, Sights = new ControLib.SightsControl { SightsColor = (Brush)this.FindResource("Player3Color") } };
            this.players[3] = new Player { Status = ShotStatus.None, Score = Player4Score, Sights = new ControLib.SightsControl { SightsColor = (Brush)this.FindResource("Player4Color") } };
            this.players[4] = new Player { Status = ShotStatus.None, Score = Player5Score, Sights = new ControLib.SightsControl { SightsColor = (Brush)this.FindResource("Player5Color") } };
            this.players[5] = new Player { Status = ShotStatus.None, Score = Player6Score, Sights = new ControLib.SightsControl { SightsColor = (Brush)this.FindResource("Player6Color") } };
            float size = (float)this.ActualWidth * 0.05f;
            float sightsWidth = size * 0.1f;
            float sightsClose = size * 0.5f;
            float sightsInnerWidth = size * 0.2f;
            foreach (Player p in this.players)
            {
                // Adds the player's sights to the canvas and adjust their size.
                MainCanvas.Children.Add(p.Sights);
                Canvas.SetLeft(p.Sights, -500);
                Canvas.SetTop(p.Sights, -500);
                p.Sights.Width = size;
                p.Sights.Height = size;
                p.Sights.SightsWidth = sightsWidth;
                p.Sights.SightsClose = sightsClose;
                p.Sights.SightsInnerWidth = sightsInnerWidth;

                // Add fire callback.
                p.Sights.FireAnimationStoryBoard.Completed += (s, a) => this.ShotEnded(p);
            }

            // Setup timed event to update flyout text positions.
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_FlyoutUpdate);
            dispatcherTimer.Interval = new TimeSpan(0,0,0,0,10);
            dispatcherTimer.Start();

            // Sets up and starts the sensor.
            controller = new KinectSensorController(KinectSensorType.Xbox360Sensor);
            controller.TrackedSkeletonReady += new KinectSensorController.TrackedSkeletonReadyHandler(callback_TrackedSkeletonReady);
            controller.Gestures.KinectGestureRecognized += new EventHandler<KinectGestureEventArgs>(callback_KinectGestureRecognized);
            controller.Gestures.AddGesture(new KinectGestureWaveRightHand());
            controller.Gestures.AddGesture(new KinectGestureWaveLeftHand());
            controller.StartSensor();
        }

        private void dispatcherTimer_FlyoutUpdate(object sender, EventArgs e)
        {
            foreach (UIElement elem in MainCanvas.Children)
            {
                FlyoutTextControl flyout = elem as FlyoutTextControl;
                if (flyout != null)
                {
                    Canvas.SetLeft(flyout, flyout.X - flyout.ActualWidth * 0.5f);
                    Canvas.SetTop(flyout, flyout.Y - flyout.ActualHeight * 0.5f);
                }
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            // Stops the sensor if it's still running.
            if (this.controller.Sensor.IsRunning)
            {
                this.controller.StopSensor();
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.players != null)
            {
                // Resize all sights.
                float size = (float)e.NewSize.Width * 0.05f;
                float sightsWidth = size * 0.1f;
                float sightsClose = size * 0.5f;
                float sightsInnerWidth = size * 0.2f;
                foreach (Player p in players)
                {
                    p.Sights.Width = size;
                    p.Sights.Height = size;
                    p.Sights.SightsWidth = sightsWidth;
                    p.Sights.SightsClose = sightsClose;
                    p.Sights.SightsInnerWidth = sightsInnerWidth;
                }
            }
        }

        // REMOVE
        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case System.Windows.Input.Key.Space:
                    this.AddPlayer(0, "0");
                    break;
                case System.Windows.Input.Key.Q:
                    this.RemovePlayer(0);
                    break;
                case System.Windows.Input.Key.Left:
                    if (xP > 0.0f)
                        xP -= 0.1f;
                    this.MovePlayer(0, xP, yP);
                    break;
                case System.Windows.Input.Key.Right:
                    if (xP < 1.0f)
                        xP += 0.1f;
                    this.MovePlayer(0, xP, yP);
                    break;
                case System.Windows.Input.Key.Up:
                    if (yP > 0.0f)
                        yP -= 0.1f;
                    this.MovePlayer(0, xP, yP);
                    break;
                case System.Windows.Input.Key.Down:
                    if (yP < 1.0f)
                        yP += 0.1f;
                    this.MovePlayer(0, xP, yP);
                    break;
                case System.Windows.Input.Key.F:
                    this.FirePlayer(0);
                    break;
            }
        }
    }
}