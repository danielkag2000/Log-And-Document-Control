using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogSystem.doc_flow
{
    class PipeControl
    {
        private readonly Dictionary<int, List<int>> pipesMap = new Dictionary<int, List<int>>();

        public PipeControl()
        {

            // get the pipes codes
            DataTable dataTable = new DataTable();
            string connString = ConfigurationManager.ConnectionStrings["LogManager"].ConnectionString;
            string query = "select * from pipeCode";

            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataTable);
            conn.Close();
            da.Dispose();

            // initiate dictinary of notification
            foreach (DataRow row in dataTable.Rows)
            {
                DataColumn id = dataTable.Columns[0];
      

                DataColumn processes = dataTable.Columns[1];

                // the processes in format of "p1,p2,p3,p4,..."
                string[] str_processes = ((string)row[processes]).Split(',');
                
                List<int> l = new List<int>();
                foreach(string s in str_processes)
                {
                    l.Add(int.Parse(s));
                }

                pipesMap[(int)row[id]] = l;
            }
        }

        public List<int> GetPipe(int id)
        {
            return this.pipesMap[id];
        }

        public List<int> this[int pipeID]
        {
            get
            {
                // if this function exists
                if (!pipesMap.ContainsKey(pipeID))
                    return new List<int>(); // defualt

                return new List<int>(pipesMap[pipeID]);
            }
        }
        }
}
