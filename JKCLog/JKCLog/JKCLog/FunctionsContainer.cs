using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogSystem
{

    class FunctionsContainer
    {
        // the dictionary of the functions
        private Dictionary<int, Func<List<string>, DBClient, string>> funcMap;

        public FunctionsContainer()
        {
            this.funcMap = new Dictionary<int, Func<List<string>, DBClient, string>>();
        }


        public Func<List<string>, DBClient, string> this[int funcID]
        {
            get
            {
                // if this function exists
                if (!funcMap.ContainsKey(funcID))
                    funcMap[funcID] = ((param, dbClient) => { return ""; }); // defualt

                return new Func<List<string>, DBClient, string>(funcMap[funcID]);
            }

            set
            {
                funcMap[funcID] = value;
            }
        }
    }
}
