using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using LogSystem.xmlHandler;
using LogSystem.doc_flow;

namespace LogSystem
{
    class User
    {
        public string IP { get;}
        public int Port { get;}
        public int Process { get; }

        public User(string ip, int port, int process)
        {
            this.IP = ip;
            this.Port = port;
            this.Process = process;
        }

        public void SendNotification(object sender, EventArgs e)
        {
            DocProcessEventArgs args = (DocProcessEventArgs)e;
            string msg = new NotificationXML(1, args.Count, args.Process, args.Pipe).ToXML();

            try
            {
                TcpClient client = new TcpClient(IP, Port);

                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(msg);

                // Get a client stream for reading and writing.
                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);

                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine("ArgumentNullException: {0}", ex);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("SocketException: {0}", ex);
            }
        }

        public static bool operator ==(User u1, User u2)
        {
            return u1.Equals(u2);
        }

        public static bool operator !=(User u1, User u2)
        {
            return !(u1 == u2);
        }

        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }

            User u = (User)obj;
            return this.IP.Equals(u.IP) && (this.Port == u.Port) && (this.Process == u.Process);
        }

        public override int GetHashCode()
        {
            return this.IP.GetHashCode() + this.Port + this.Process;
        }
    }
}
