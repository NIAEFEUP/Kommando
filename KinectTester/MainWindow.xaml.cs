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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace KinectTester 
{
    public class Echo : WebSocketService
    {
        protected override void OnMessage (MessageEventArgs e)
        {
            Send (e.Data);
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private TcpClient client;
        private int playerIndex;
        private int playerScore;
        private Encoding encoding;
        private WebSocketServer wsServer;
        /*private WebSocketServer wsServer;
        private UserContext client;*/

        public MainWindow()
        {
            InitializeComponent();
            wsServer = new WebSocketServer(IPAddress.Parse("127.0.0.1"), 11000);
            wsServer.AddWebSocketService<Echo>("/");
            wsServer.Start();
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(MainCanvas);
            StringBuilder sb = new StringBuilder("");
            for (int i = 0; i < 6; ++i)
            {
                if (i == playerIndex)
                {
                    sb.Append(p.X / MainCanvas.ActualWidth);
                    sb.Append(",");
                    sb.Append(p.Y / MainCanvas.ActualHeight);
                    sb.Append(",0,");
                    sb.Append(playerScore);
                    if (i != 5)
                        sb.Append(",");
                }
                else
                {
                    if (i == 5)
                        sb.Append("-1,-1,-1,-1");
                    else if(i == 3)
                        sb.Append("0,0,0,20000,");
                    else
                        sb.Append("-1,-1,-1,-1,");
                }
            }
            sb.Append('\n');
            IWebSocketSession ws = wsServer.WebSocketServices.GetSessions("/").Sessions.FirstOrDefault();
            if(ws != null)
                ws.Context.WebSocket.Send(sb.ToString());
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(MainCanvas);
            StringBuilder sb = new StringBuilder("");
            for (int i = 0; i < 6; ++i)
            {
                if (i == playerIndex)
                {
                    sb.Append(p.X / MainCanvas.ActualWidth);
                    sb.Append(",");
                    sb.Append(p.Y / MainCanvas.ActualHeight);
                    sb.Append(",1,");
                    sb.Append(playerScore);
                    if (i != 5)
                        sb.Append(",");
                }
                else
                {
                    if (i == 5)
                        sb.Append("-1,-1,-1,-1");
                    else if (i == 3)
                        sb.Append("0,0,0,20000,");
                    else
                        sb.Append("-1,-1,-1,-1,");
                }
            }
            sb.Append('\n');
            IWebSocketSession ws = wsServer.WebSocketServices.GetSessions("/").Sessions.FirstOrDefault();
            if (ws != null)
                ws.Context.WebSocket.Send(sb.ToString());
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.NumPad1:
                    this.playerIndex = 0;
                    this.playerScore = 0;
                    break;
                case Key.NumPad2:
                    this.playerIndex = 1;
                    this.playerScore = 0;
                    break;
                case Key.NumPad3:
                    this.playerIndex = 2;
                    this.playerScore = 0;
                    break;
                case Key.NumPad4:
                    this.playerIndex = 3;
                    this.playerScore = 0;
                    break;
                case Key.NumPad5:
                    this.playerIndex = 4;
                    this.playerScore = 0;
                    break;
                case Key.NumPad6:
                    this.playerIndex = 5;
                    this.playerScore = 0;
                    break;
            }
        }
    }
}
