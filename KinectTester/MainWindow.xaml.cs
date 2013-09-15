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

namespace KinectTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MyTcpListener server;
        private TcpClient client;
        private int playerIndex;
        private int playerScore;
        private Encoding encoding;

        public MainWindow()
        {
            InitializeComponent();
            encoding = Encoding.Default;
            playerIndex = 0;
            playerScore = 0;
            server = new MyTcpListener();
            client = server.GetClient();
            byte[] buffer = new byte[1024];
            client.GetStream().BeginRead(buffer, 0, buffer.Length, ReceiveCallback, buffer);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
           /* if (MainCanvas.Dispatcher.CheckAccess())
            {*/
                int read;
                try
                {
                    read = client.GetStream().EndRead(ar);
                }
                catch
                {
                    return;
                }

                // Nothing read, connection closed.
                if (read == 0)
                {
                    return;
                }

                byte[] buffer = ar.AsyncState as byte[];
                string message = encoding.GetString(buffer, 0, read);
                string[] tokens = message.Split(',');
                int shot = int.Parse(tokens[playerIndex]);
                Console.WriteLine(message);
                if (shot == 2)
                    playerScore += 1000;
                client.GetStream().BeginRead(buffer, 0, buffer.Length, ReceiveCallback, buffer);
            /*}
            else
            {
                MainCanvas.Dispatcher.Invoke(new Action(() => ReceiveCallback(ar)));
            }*/
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
            byte[] buffer = encoding.GetBytes(sb.ToString());
            client.GetStream().Write(buffer, 0, buffer.Length);
            client.GetStream().Flush();
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
            byte[] buffer = encoding.GetBytes(sb.ToString());
            client.GetStream().Write(buffer, 0, buffer.Length);
            client.GetStream().Flush();
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

    class MyTcpListener
    {
        private TcpListener server;

        public MyTcpListener()
        {
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 3000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }

        public TcpClient GetClient()
        {
            Console.Write("Waiting for a connection... ");

            // Perform a blocking call to accept requests.
            // You could also user server.AcceptSocket() here.
            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("Connected!");
            return client;
        }
    }
}
