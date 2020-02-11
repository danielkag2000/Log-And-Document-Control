using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JKCLog
{
    public class Logger
    {
        string path;
        Mutex m;

        public Logger(string path)
        {
            m = new Mutex();
            this.path = path;
        }

        public void WriteLog(string msg)
        {
            m.WaitOne();
            DateTime d = DateTime.Now;
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine($"[{d.ToString()}] {msg}");
            }
            m.ReleaseMutex();
        }
    }
}
