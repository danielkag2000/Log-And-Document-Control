using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Globals.Init();

                String link = ConfigurationManager.AppSettings.Get("IP");
                int port = int.Parse(ConfigurationManager.AppSettings.Get("Port"));
                Server s = new Server(link, port);
                Globals.Log.WriteLog($"start server in link:{link} and port {port}");
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Globals.Log.WriteLog(e.Message);
            }
            Console.ReadLine();
        }
    }
}
