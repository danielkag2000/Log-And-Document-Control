using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogSystem.doc_flow
{
    public class DocProcessEventArgs : EventArgs
    {
        public DocProcessEventArgs(int count, int process, int pipe)
        {
            Count = count;
            Process = process;
            Pipe = pipe;
        }

        public int Count { get; set; }
        public int Process { get; set; }
        public int Pipe { get; set; }
    }
}
