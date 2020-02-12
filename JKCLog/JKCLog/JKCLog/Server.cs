using LogSystem.xmlHandler;
using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace LogSystem
{
    /// <summary>
    /// The Multithreding Server for multiple clients
    /// </summary>
    class Server
    {
        TcpListener server = null;

        public Server(String link, int port)
        {
            IPAddress ipAddress;

            // if not ip
            if (!IPAddress.TryParse(link, out ipAddress))
            {
                ipAddress = Dns.GetHostEntry(link).AddressList[0];
            }

            Console.WriteLine($"start server in link:{ipAddress} and port {port}");
            server = new TcpListener(ipAddress, port);

            server.Start();
            StartListener();
        }

        /// <summary>
        /// Start the listener
        /// </summary>
        public void StartListener()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");

                    // wait for the client
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    // create new thread to handle the client
                    Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                    t.Start(client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                server.Stop();
            }
        }

        /// <summary>
        /// client handler
        /// </summary>
        /// <param name="obj">the client</param>
        public void HandleClient(Object obj)
        {
            TcpClient client = (TcpClient)obj;
            var stream = client.GetStream();
            string imei = String.Empty;
            Byte[] bytes = new Byte[4096];
            try
            {
                var input = stream.Read(bytes, 0, bytes.Length);
                if (input == 0)
                {
                    client.Close();
                    return;
                }

                // convert to string
                string str = Encoding.ASCII.GetString(bytes, 0, input);
                Console.WriteLine($"recived: {str}");
                // the answer from the handler
                string state = new XMLRequstHandler(str).Handle();
                Console.WriteLine($"return: {state}");

                // replay
                Byte[] reply = System.Text.Encoding.ASCII.GetBytes(state);
                stream.Write(reply, 0, reply.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.ToString());
            }
            finally
            {
                client.Close();
            }
        }
    }
}
