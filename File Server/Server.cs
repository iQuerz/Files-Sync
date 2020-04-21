using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace File_Server {
    class Server {
        string ip;
        int port;

        public Server( string hostIP, int port) {
            ip = hostIP;
            this.port = port;

            //test
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            server.Close();

        }
        public void sendFile(string filePath, string root) {
            FileInfo file = new FileInfo(filePath);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Parse(ip), port));

            //send song name length

            server.Listen(0);
            Socket client = server.Accept();
            byte[] send;
            if (filePath.Replace(root, "").Length < 10)
                send = new UTF8Encoding(true).GetBytes("0" + filePath.Replace(root, "").Length);
            else
                send = new UTF8Encoding(true).GetBytes(filePath.Replace(root, "").Length.ToString());
            client.Send(send);
            client.Close();
            server.Close();

            //send song name

            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            server.Listen(0);
            client = server.Accept();
            send = new UTF8Encoding(true).GetBytes(filePath.Replace(root,""));
            client.Send(send);
            client.Close();
            server.Close();

            //send the song

            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            server.Listen(0);
            client = server.Accept();
            client.SendFile(file.FullName);
            client.Close();

            server.Close();
        }
        public void send(string data) {
            switch (data.Length) {
                case 1:
                    data = "000" + data;
                    break;
                case 2:
                    data = "00" + data;
                    break;
                case 3:
                    data = "0" + data;
                    break;
                default:
                    data = "0000";
                    break;
            }
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            server.Listen(0);
            Socket client = server.Accept();
            byte[] send = new UTF8Encoding(true).GetBytes(data);
            client.Send(send);
            client.Close();
            server.Close();
        }
    }
}
