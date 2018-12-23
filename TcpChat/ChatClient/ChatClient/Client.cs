using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    public class Client
    {
        public Socket handler;

        public int port { get; private set; }
        public string ip { get; private set; }
        public Client(int port,string ip)
        {
            this.port = port;
            this.ip = ip;
            handler = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.IP);
        }

        public void Connect()
        {
            handler.Connect(new IPEndPoint(IPAddress.Parse(ip),port));
        }

        internal void Send(string mess)
        {
            handler.Send(Encoding.UTF8.GetBytes(mess));
        }
    }
}
