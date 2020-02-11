using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogSystem
{
    interface IHandler
    {
        /// <summary>
        /// handlers
        /// </summary>
        /// <returns>State of the handler (falied, success and ect.)</returns>
        string Handle();
    }
}
