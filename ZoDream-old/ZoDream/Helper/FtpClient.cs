using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Helper
{
    public class FtpClient
    {
        public Socket SocketConnect { get; private set; }

        public string Content { get; private set; }

        public string Reply { get; private set; }

        public int StatusCode { get; private set; }

        public TransferType Type { get; set; } = TransferType.BINARY;

        public string Host { get; set; }

        public int Port { get; set; } = 21;

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool IsConnect { get; private set; } = false;

        public string Path { get; set; }

        const int BlockSize = 512;

        public FtpClient()
        {

        }

        public FtpClient(string host)
            :this(host, 21)
        {

        }

        public FtpClient(string host, int port)
            :this(host, port, string.Empty, string.Empty)
        {
        }

        public FtpClient(string host, string username, string password)
            :this(host, 21, username, password)
        {

        }

        public FtpClient(string host, int port, string username, string password)
        {
            Host = host;
            Port = port;
            UserName = username;
            Password = password;
        }

        public async Task ConnectAsync()
        {
            SocketConnect = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await SocketConnect.ConnectAsync(IPAddress.Parse(Host), Port);
        }


        public async Task CreateDirectory(string name)
        {
            await Send("MKD " + name);
        }

        public async Task RemoveDirectory(string name)
        {
            await Send("RMD " + name);
        }

        private async Task<int> readReply()
        {
            Content = string.Empty;
            Reply = await readLine();
            return StatusCode = int.Parse(Reply.Substring(0, 3));
        }

        private async Task<string> readLine()
        {
            SocketConnect.ReceiveBufferSize = BlockSize;
            var content = new StringBuilder();
            var buffers = new byte[BlockSize];
            while (true)
            {
                var i = SocketConnect.Receive(buffers);
                content.Append(Encoding.UTF8.GetString(buffers));
                if (i < BlockSize)
                {
                    break;
                }
            }
            Content = content.ToString();
            content.Clear();
            var args = Content.Split('\n');
            if (args.Length > 2)
            {
                Content = args[args.Length - 2];
            } else
            {
                Content = args[0];
            }
            if (Content.Substring(3, 1).Equals(""))
            {
                return await readLine();
            }
            return Content;
        }

        public async Task<int> Send(string command)
        {
            var buffers = Encoding.UTF8.GetBytes(command + "\r\n");
            SocketConnect.SendBufferSize = buffers.Length;
            SocketConnect.Send(buffers);
            return await readReply();
        }
    }

    public enum TransferType
    {
        BINARY,
        ASCII
    };
}
