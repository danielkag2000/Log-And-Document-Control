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
    class NotificationControl
    {
        public delegate void NotificationFunc(object sender, EventArgs e);
        public event EventHandler UserReach;

        // the dictionary of the evensts
        private readonly Dictionary<int, EventHandler> notificationMap = new Dictionary<int, EventHandler>();
        private Dictionary<int, int> countMap = new Dictionary<int, int>();

        // the users
        HashSet<User> users = new HashSet<User>();

        public NotificationControl()
        {
 
            // get the pipes codes
            DataTable dataTable = new DataTable();
            string connString = ConfigurationManager.ConnectionStrings["LogManager"].ConnectionString;
            string query = "select * from programCodes";

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
            foreach(DataRow row in dataTable.Rows)
            {
                DataColumn column = dataTable.Columns[0];
                int id = (int)row[column];
                notificationMap[id] = UserReach;
                countMap[id] = 0;
            }
        }

        public void AddSubscribe(User u, int process)
        {
            if (!this.users.Contains(u))
            {
                this.notificationMap[process] += u.SendNotification;
                this.users.Add(u);
            }
        }

        public void RemoveSubscribe(User u, int process)
        {
            User user;
            // for working with the same user
            if (this.users.TryGetValue(u, out user))
            {
                this.notificationMap[process] -= user.SendNotification;
                this.users.Remove(user);
            }
        }

        public void Notify(DocProcessEventArgs args)
        {
            this.notificationMap[args.Process]?.Invoke(this, args);
        }

        public void AddProcess(int process)
        {
            this.countMap[process]++;
        }

        public void RemoveProcess(int process)
        {
            this.countMap[process]--;

            if (this.countMap[process] < 0)
            {
                this.countMap[process] = 0;
            }
        }

        public int ProcessQueueSize(int process)
        {
            return this.countMap[process];
        }
    }
}
