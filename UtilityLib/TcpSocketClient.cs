using System;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace Utility
{
    public class TcpSocketClient
    {
        //private readonly string ServerAddress = "192.168.27.133";
        private readonly string ServerAddress = "127.0.0.1";
        private readonly int ServerPort = 11000;
        private TcpClient client;
        private NetworkStream stream;
        public byte[] Buffer { get; set; }

        public delegate void MessageReceivedHandler(TcpSocketClient client, string message);

        public event MessageReceivedHandler MessageReceived;

        public bool Connected { get; set; }

        public Encoding Encoding { get; set; }

        public TcpSocketClient()
        {
            this.Connected = false;
            this.Encoding = Encoding.Default;
            client = new TcpClient();
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                this.Connected = true;
                client.EndConnect(ar);
                this.stream = client.GetStream();
                this.stream.BeginRead(Buffer, 0, Buffer.Length, ReceiveCallback, Buffer);
            }
            catch
            {
            }
        }

        public void BeginConnect()
        {
            client.BeginConnect(System.Net.IPAddress.Parse(ServerAddress), ServerPort, ConnectCallback, null);
        }

        public void EndReceive()
        {
            if (this.Connected)
            {
                this.client.Close();
            }
        }

        public void BeginSend(string message)
        {
            byte[] bytes = Encoding.GetBytes(message);
            BeginSend(bytes);
        }

        public void BeginSend(byte[] bytes)
        {
            stream.BeginWrite(bytes, 0, bytes.Length, SendCallback, null);
        }

        private void SendCallback(IAsyncResult result)
        {
            stream.EndWrite(result);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            int read;
            try
            {
                read = stream.EndRead(ar);
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

            // Get data.
            string data = this.Encoding.GetString(Buffer, 0, read);
            if (this.MessageReceived != null)
            {
                this.MessageReceived(this, data);
            }
        }

        public void BeginReceive()
        {
            stream.BeginRead(Buffer, 0, Buffer.Length, ReceiveCallback, Buffer);
        }
    }
}