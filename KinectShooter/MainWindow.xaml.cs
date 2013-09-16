using ControLib;
using Kinect.Gestures;
using Kinect.Gestures.Waves;
using Kinect.Sensor;
using Kinect.Toolbox;
using Microsoft.Kinect;
using System;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Utility;
using WebSocketSharp;

namespace KinectShooter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Storyboard backgroundAnimation;
        private SoundPlayer backgroundMusic;
        private KinectSensorController controller;
        private bool endGame = false;
        private HitSkeleton hitSkeleton;
        private Player[] players;
        private int skeletonId = -1;
        private string token;
        private WebSocket ws;

        /// <summary>
        /// MaindWindow constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        private object SensorTiltLock { get; set; }

        /// <summary>
        /// Adds a new player to the game.
        /// </summary>
        /// <param name="index">Index of the new player</param>
        /// <param name="score">Score of the new player, tipically "0"</param>
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

        /// <summary>
        /// Handles gesture recognized events.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="args">Event arguments</param>
        public void callback_KinectGestureRecognized(object sender, KinectGestureEventArgs args)
        {
            // Engage animations.
            if (args.GestureType == KinectGestureType.WaveRightHand && this.skeletonId == -1 && !endGame)
            {
                this.skeletonId = args.TrackingId;
                token = "";
                this.endGame = false;
                this.controller.StartTrackingSkeleton(this.skeletonId);
                foreach(var parts in hitSkeleton.BodyParts)
                {
                    if(parts.Key == HitSkeletonBodyPart.ForearmLeft || parts.Key == HitSkeletonBodyPart.ForearmRight)
                    {
                        parts.Value.State = BodyPartState.Armor;
                    }
                    else
                    {
                        parts.Value.State = BodyPartState.NotHit;
                    }
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

                // Start websocket client.
                ws = new WebSocket("ws://127.0.0.1:1234/");
                ws.OnMessage += ws_OnMessage;
                ws.Connect();
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
                this.TokenLabel.Content = "";
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

                // Disconnect socket.
                ws.Close();

                // Scoreboard animation.
                Storyboard sb = (Storyboard)this.FindResource("ScoreboardDisappearStoryboard");
                sb.Begin();
            }
        }

        /// <summary>
        /// Fire a shot from a player.
        /// </summary>
        /// <param name="index">Index of the player who took the shot</param>
        public void FirePlayer(int index)
        {
            Player p = this.players[index];
            if (p.PlayerActive)
            {
                // Resize all sights.
                float size = (float)this.ActualWidth * 0.05f;
                float sightsWidth = size * 0.1f;
                float sightsClose = size * 0.5f;
                float sightsInnerWidth = size * 0.2f;
                SightsControl s = new SightsControl
                {
                    SightsColor = p.Sights.SightsColor,
                    Width = size,
                    Height = size,
                    SightsWidth = 0.0f,
                    SightsClose = sightsClose,
                    SightsInnerWidth = 0.0f
                };
                MainCanvas.Children.Add(s);
                Canvas.SetLeft(s, p.X - size / 2.0f);
                Canvas.SetTop(s, p.Y - size / 2.0f);
                float x = p.X / (float)this.ActualWidth;
                float y = p.Y / (float)this.ActualHeight;
                s.FireAnimationStoryBoard.Completed += (snd, arg) => ShotEnded(p, x, y, s);
                s.Fire();
            }
        }

        /// <summary>
        /// Moves the player's sights to a new position.
        /// </summary>
        /// <param name="index">Index of the player</param>
        /// <param name="x">X coordinate of the new position</param>
        /// <param name="y">Y coordinate of the new position</param>
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

        /// <summary>
        /// Removes a player from the game.
        /// </summary>
        /// <param name="index">Player's index</param>
        public void RemovePlayer(int index)
        {
            Player p = this.players[index];
            p.PlayerActive = false;
            p.Score.BeginStoryboard((Storyboard)this.FindResource("PlayerScoreDisappearStoryboard"));
            Canvas.SetLeft(p.Sights, -500);
            Canvas.SetTop(p.Sights, -500);
        }

        /// <summary>
        /// Handles the end of a shot animation.
        /// </summary>
        /// <param name="p">Player who took the shot</param>
        /// <param name="x">X coordinate of the shot</param>
        /// <param name="y">Y coordinate of the shot</param>
        /// <param name="s">SightsControl being fired</param>
        public void ShotEnded(Player p, float x, float y, SightsControl s)
        {
            // Remove the control.
            MainCanvas.Children.Remove(s);

            // Locking here prevents concurrence from the network listener.
            lock (this.players)
            {
                if (p.PlayerActive && hitSkeleton.DamageTaken < 1.0)
                {
                    float pX = x * (float)this.ActualWidth;
                    float pY = y * (float)this.ActualHeight;

                    // Build bullet polygon.
                    Polygon pol = new Polygon();
                    pol.Points = hitSkeleton.RegularPolygonToPointCollection(new Vector2(pX, pY), (float)p.Sights.ActualWidth * 0.5f, 15);

                    // Check for weapon hits.
                    if (Tools.IsPolygonColliding(pol, hitSkeleton.PaddleRight.Shape))
                    {
                        hitSkeleton.PaddleRight.State = PaddleState.Hit;
                        SetFlyout("DEFEND!", pX, pY, Brushes.MidnightBlue);
                        p.Status = ShotStatus.Defend;
                    }
                    else if (Tools.IsPolygonColliding(pol, hitSkeleton.PaddleLeft.Shape))
                    {
                        hitSkeleton.PaddleLeft.State = PaddleState.Hit;
                        SetFlyout("DEFEND!", pX, pY, Brushes.MidnightBlue);
                        p.Status = ShotStatus.Defend;
                    }
                    // Possible body part hits.
                    else
                    {
                        var hitParts = hitSkeleton.BodyParts.Where(pa => 
                            pa.Key != HitSkeletonBodyPart.ForearmRight &&
                            pa.Key != HitSkeletonBodyPart.ForearmLeft &&
                            Tools.IsPolygonColliding(pa.Value.Shape, pol));
                        BodyPart part = hitParts.FirstOrDefault(pa => pa.Value.State == BodyPartState.NotHit).Value;

                        // Only one part may be hit per shot, but precedence is
                        // always given to parts that haven't been shot before.
                        if (part != null)
                        {
                            part.State = BodyPartState.Hit;
                            SetFlyout("HIT!", pX, pY, Brushes.MidnightBlue);
                            p.Status = ShotStatus.Hit;
                        }
                        // A part previously hit was hit again.
                        else if (hitParts.Count() > 0)
                        {
                            SetFlyout("HIT!", pX, pY, Brushes.MidnightBlue);
                            p.Status = ShotStatus.Hit;
                        }
                        // Nothing was hit.
                        else
                        {
                            SetFlyout("MISSED!", pX, pY, Brushes.MidnightBlue);
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
                                case ShotStatus.Defend:
                                    response.Append("3,");
                                    break;
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
                    float damage = hitSkeleton.DamageTaken;
                    response.Append(damage);
                    ws.Send(response.ToString());
                }
            }
        }

        /// <summary>
        /// Tilts the Kinect sensor, avoiding overworking the sensor's motor.
        /// </summary>
        /// <param name="obj">Event args of the key up event</param>
        public void TiltSensor(object obj)
        {
            // A new tilt command cannot be issued if previous tilt is still
            // locked.
            KeyEventArgs e = (KeyEventArgs)obj;

            // At least 1200ms should pass between tilts.
            if (System.Threading.Monitor.TryEnter(this.SensorTiltLock))
            {
                try
                {
                    // Increase the tilt.
                    if (e.Key == Key.Up)
                    {
                        if (this.controller.Sensor.ElevationAngle <= 20)
                        {
                            this.controller.Sensor.ElevationAngle += 5;
                        }
                    }
                    else if (e.Key == Key.Down)
                    {
                        // Decrease the tilt.
                        if (this.controller.Sensor.ElevationAngle >= -20)
                        {
                            this.controller.Sensor.ElevationAngle -= 5;
                        }
                    }
                    System.Threading.Thread.Sleep(1200);
                }
                catch (Exception)
                {
                    // No exception handling needed, just catching.
                }
                finally
                {
                    System.Threading.Monitor.Exit(this.SensorTiltLock);
                }
            }
        }

        /// <summary>
        /// Handles termination of the victory animation.
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private void callback_VictoryStoryboardCompleted(object sender, EventArgs e)
        {
            Storyboard vicImageStoryboard = (Storyboard)this.FindResource("VictoryImageStoryboard");
            vicImageStoryboard.Completed -= callback_VictoryStoryboardCompleted;
            callback_TrackedSkeletonReady(null);
        }

        /// <summary>
        /// Updates the flyout text elements' positions.
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
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

        /// <summary>
        /// Handles key up events.
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event parameters</param>
        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            // Tilt should only be available if sensor is active.
            if (this.controller.FoundSensor())
            {
                if (e.Key == Key.Up || e.Key == Key.Down)
                {
                    System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(TiltSensor));
                    thread.Start(e);
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Sets up a new flyout text element.
        /// </summary>
        /// <param name="text">Text of the flyout</param>
        /// <param name="x">X coordinate of the center of the flyout</param>
        /// <param name="y">Y coordinate of the center of the flyout</param>
        /// <param name="color">Color of the text</param>
        private void SetFlyout(string text, float x, float y, Brush color)
        {
            FlyoutTextControl flyout = new FlyoutTextControl
            {
                FlyoutTextColor = color,
                FlyoutTextContent = text,
                FlyoutTextSize = (float)this.ActualHeight * 0.1f
            };
            MainCanvas.Children.Add(flyout);
            flyout.X = x;
            flyout.Y = y;
            Canvas.SetLeft(flyout, x - flyout.ActualWidth * 0.5f);
            Canvas.SetTop(flyout, y - flyout.ActualHeight * 0.5f);
            flyout.FlyoutAnimation.Completed += (s, a) => MainCanvas.Children.Remove(flyout);
            flyout.Flyout();
        }

        /// <summary>
        /// Sets up the game.
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Binds the skeleton representation to the canvas.
            hitSkeleton = new HitSkeleton();
            this.SensorTiltLock = new object();
            this.KeyUp += MainWindow_KeyUp;
            foreach (BodyPart part in hitSkeleton.BodyParts.Values)
            {
                MainCanvas.Children.Add(part.Shape);
            }
            MainCanvas.Children.Add(this.hitSkeleton.PaddleRight.Shape);
            MainCanvas.Children.Add(this.hitSkeleton.PaddleLeft.Shape);

            // Load background music.
            backgroundMusic = new SoundPlayer("Sounds/background.wav");
            backgroundMusic.Load();
            backgroundMusic.PlayLooping();

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
            }

            // Setup timed event to update flyout text positions.
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_FlyoutUpdate);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            dispatcherTimer.Start();

            // Sets up and starts the sensor.
            controller = new KinectSensorController(KinectSensorType.Xbox360Sensor);
            controller.TrackedSkeletonReady += new KinectSensorController.TrackedSkeletonReadyHandler(callback_TrackedSkeletonReady);
            controller.Gestures.KinectGestureRecognized += new EventHandler<KinectGestureEventArgs>(callback_KinectGestureRecognized);
            controller.Gestures.AddGesture(new KinectGestureWaveRightHand());
            controller.StartSensor();
        }

        /// <summary>
        /// Handles window resizes.
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
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
                foreach (UIElement elem in MainCanvas.Children)
                {
                    SightsControl s = elem as SightsControl;
                    if (s != null)
                    {
                        s.Width = size;
                        s.Height = size;
                        s.SightsWidth = sightsWidth;
                        s.SightsClose = sightsClose;
                        s.SightsInnerWidth = sightsInnerWidth;
                    }
                }
            }
        }

        /// <summary>
        /// Tears down the game.
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            // Stops the sensor if it's still running.
            if (this.controller.Sensor.IsRunning)
            {
                this.controller.StopSensor();
            }
        }

        /// <summary>
        /// Handles websockets' on message events.
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="args">Event arguments</param>
        private void ws_OnMessage(object sender, MessageEventArgs args)
        {
            string message = args.Data;

            // Check for handshake communication.
            if (message == "whoareyou")
            {
                ws.Send("v");
                return;
            }

            if (message.Length == 4)
            {
                MainCanvas.Dispatcher.BeginInvoke(new Action(() => this.TokenLabel.Content = message));
                this.token = message;
                return;
            }

            // Checks if an invoke is required.
            if (MainCanvas.Dispatcher.CheckAccess())
            {
                // No changes after endgame.
                if (this.endGame)
                {
                    return;
                }

                string[] tokens = message.Split(',');
                lock (this.players)
                {
                    // No longer accepting updates after player dying.
                    if (hitSkeleton.DamageTaken >= 1.0f && !endGame)
                    {
                        float[] scores = new float[players.Length];
                        for (int i = 0; i < players.Length; ++i)
                        {
                            players[i].Score.ScoreContent = tokens[i * 4 + 3];
                            scores[i] = float.Parse(tokens[i * 4 + 3]);
                            if (scores[i] == -1)
                            {
                                RemovePlayer(i);
                            }
                        }
                        int index = scores.ToList().IndexOf(scores.Max());
                        Player p = players[index];
                        endGame = true;

                        // Animate victory screen.
                        Storyboard vicStoryboard = (Storyboard)this.FindResource("VictoryStoryboard");
                        Storyboard vicImageStoryboard = (Storyboard)this.FindResource("VictoryImageStoryboard");
                        vicImageStoryboard.Completed += callback_VictoryStoryboardCompleted;
                        vicStoryboard.Completed += (s, a) => this.endGame = false;
                        VictoryBackgroundRectangle.Fill = p.Sights.SightsColor;
                        VictoryBackgroundRectangle.BeginStoryboard(vicStoryboard);
                        VictoryImageRectangle.BeginStoryboard(vicImageStoryboard);
                    }
                    else if (!endGame)
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
            }
            else
            {
                MainCanvas.Dispatcher.Invoke(new Action(() => ws_OnMessage(sender, args)));
            }
        }
    }
}