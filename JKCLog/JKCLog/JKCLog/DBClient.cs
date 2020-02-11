using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace LogSystem
{
    class DBClient
    {
        private string connectionString;
        private SqlConnection cnn;

        public DBClient ()
        {
            //this.connectionString = @"Data Source=WIN-50GP30FG75; Initial Catalog=Demodb; User ID=sa;Pasword=demo123";
            this.connectionString = ConfigurationManager.ConnectionStrings["LogManager"].ConnectionString;
            this.cnn = new SqlConnection(connectionString);
        }

        public SqlConnection GetConnection()
        {
            return this.cnn;
        }

        /// <summary>
        /// do sql command
        /// </summary>
        /// <param name="command">the sql command</param>
        /// <returns>does the command executed proparly</returns>
        public bool DoCommand(SqlCommand command)
        {
            try
            {
                this.cnn.Open();
                int res = command.ExecuteNonQuery();
                bool sucsees = !(res < 0);  // if (res < 0) it is fail
                command.Dispose();
                this.cnn.Close();
                Console.WriteLine($"Status: {sucsees}, with res: {res}. of the qurry: {command.ToString()}");
                return sucsees;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                this.cnn.Close();
                return false;  // fail
            }
        }

        public DataTable GetCommandValues(SqlCommand command)
        {
            DataTable dataTable = new DataTable();
            string connString = ConfigurationManager.ConnectionStrings["LogManager"].ConnectionString;
            string query = "select * from programCodes";

            this.cnn.Open();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(command);
            // this will query your database and return the result to your datatable
            da.Fill(dataTable);
            this.cnn.Close();
            da.Dispose();

            return dataTable;
        }

    }
}
