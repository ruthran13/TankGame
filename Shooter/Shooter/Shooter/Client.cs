using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace Shooter
{
    class Client 
    {
        System.Net.Sockets.TcpClient clientSocket;
        private NetworkStream clientStream; //Stream - outgoing
        private TcpClient client; //To talk back to the client
        private BinaryWriter writer; //To write to the clients
        public int connCount;

        private NetworkStream serverStream; //Stream - incoming        
        private TcpListener listener; //To listen to the clients        
        public string reply = ""; //The message to be written

        public Client()
        {
            load();
        }
        private void load()
        {

            msg("Client Started");

            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7000);
            listener.Start();
        }

        public void connect()
        {
            try
            {
                clientSocket.Connect("127.0.0.1", 6000);
            }
            catch (Exception e)
            {
            }
            if (clientSocket.Connected)
            {
                Console.WriteLine("Client Socket Program - Server Connected ...");
            }
        }


        public void send(String message)
        {
            Console.WriteLine(message);
            connCount = 0;
            clientSocket = new System.Net.Sockets.TcpClient();
            connect();
            if (clientSocket.Connected)
            {
                NetworkStream serverStream = clientSocket.GetStream();
                try
                {
                    byte[] outStream = System.Text.Encoding.ASCII.GetBytes(message);
                    serverStream.Write(outStream, 0, outStream.Length);
                    clientSocket.Close();
                    serverStream.Flush();
                    connCount++;
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine("send\t" + e.Message.ToString());
                }
                
            }
            else
            {
                
            }
      

        }
        public String receive()
        {
            
            if (connCount > 0)
            {

                Socket connection = listener.AcceptSocket();
                if (connection.Connected)
                {
                    Console.WriteLine("recieving data from server");
                }
                this.serverStream = new NetworkStream(connection);

                SocketAddress sockAdd = connection.RemoteEndPoint.Serialize();
                string s = connection.RemoteEndPoint.ToString();
                List<Byte> inputStr = new List<byte>();

                int asw = 0;
                while (asw != -1)
                {
                    asw = this.serverStream.ReadByte();
                    inputStr.Add((Byte)asw);
                }

                reply = Encoding.UTF8.GetString(inputStr.ToArray());
                this.serverStream.Close();


                return reply.Substring(0, reply.IndexOf("#"));
            }
            else
            {
                return "Not connected";
            }
        }

        public void msg(string mesg)
        {
            Console.WriteLine(Environment.NewLine + " >> " + mesg);
        }




    }
}
