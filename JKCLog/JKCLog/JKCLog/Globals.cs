using JKCLog;
using LogSystem.doc_flow;
using LogSystem.xmlHandler;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogSystem
{
    static partial class Globals
    {
        public static FunctionsContainer functions;

        public static NotificationControl notification;

        public static PipeControl pipes;

        public static Logger Log;

        public static void Init()
        {
            Log = new Logger("log.txt");

            notification = new NotificationControl();

            pipes = new PipeControl();

            Init_functions();
        }

        private static void Init_functions()
        {
            // create the container
            functions = new FunctionsContainer();

            // string Decleare(string description)
            functions[1] = ((List<string> parameters, DBClient dbClient) =>
            {
                if (parameters.Count != 3)
                {
                    return new ResponseXML(0, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                }
                //string qurry = $@"INSERT INTO logs.eventTable (event_date, event_type, program_id, sub_program_id, description) VALUSE(@event_date, @event_type, @program_id, @sub_program_id, @description)";
                string qurry = ConfigurationManager.AppSettings.Get("InsertLog");

                SqlCommand command = new SqlCommand(qurry, dbClient.GetConnection());
                command.Parameters.Add("@event_date", SqlDbType.DateTime).Value = DateTime.Now;
                command.Parameters.AddWithValue("@event_type", 1);
                command.Parameters.AddWithValue("@program_id", parameters[0]);
                command.Parameters.AddWithValue("@sub_program_id", parameters[1]);
                command.Parameters.AddWithValue("@description", parameters[2]);

                if (dbClient.DoCommand(command))
                {
                    Console.WriteLine(new ResponseXML(1, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML());
                    return new ResponseXML(1, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                }
                Console.WriteLine(new ResponseXML(0, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML());
                return new ResponseXML(0, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
            });







            // string Subscribe(string ip, int port, int process)
            functions[2] = ((List<string> parameters, DBClient dbClient) =>
            {
                if (parameters.Count != 5)
                {
                    return new ResponseXML(0, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                }
                User u = new User(parameters[2], int.Parse(parameters[3]), int.Parse(parameters[0]));
                notification.AddSubscribe(u, int.Parse(parameters[4]));

                return new ResponseXML(1, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
            });






            // string Unsubscribe(string ip, int port, int process)
            functions[3] = ((List<string> parameters, DBClient dbClient) =>
            {
                if (parameters.Count != 5)
                {
                    return new ResponseXML(0, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                }
                User u = new User(parameters[2], int.Parse(parameters[3]), int.Parse(parameters[0]));
                notification.RemoveSubscribe(u, int.Parse(parameters[4]));

                return new ResponseXML(1, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
            });









            // string GetDocument(int process)
            functions[4] = ((List<string> parameters, DBClient dbClient) =>
            {
                if (parameters.Count != 3)
                {
                    return new ResponseXML(0, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                }
                int process = int.Parse(parameters[2]);
                string qurry = $"SELECT * FROM flowTable WHERE cprocess = {process} ORDER BY start_time ASC;";

                // AppDomain.CurrentDomain.SetData("PROCESS_ID", process);
                // string qurry = ConfigurationManager.AppSettings.Get("WaitingDocs");
                SqlCommand command = new SqlCommand(qurry, dbClient.GetConnection());

                try
                {
                    DataTable dataTable = dbClient.GetCommandValues(command);

                    foreach (DataRow row in dataTable.Rows)
                    {
                        DataColumn avaliable = dataTable.Columns[5];

                        // if the document is avaliable
                        if ((int)row[avaliable] > 0)
                        {

                            // document is not avliable
                            qurry = $"UPDATE flowTable SET avaliable = 0 WHERE doc_id = {(int)row[dataTable.Columns[0]]} AND pipe_id = {(int)row[dataTable.Columns[1]]}";
                            SqlCommand update_command = new SqlCommand(qurry, dbClient.GetConnection());
                            if (dbClient.DoCommand(update_command))
                            {
                                // remove document from the queue
                                notification.RemoveProcess(process);

                                // notify the change
                                notification.Notify(new DocProcessEventArgs(notification.ProcessQueueSize(process), process, (int)row[dataTable.Columns[1]]));

                                return new ResponseXML(1, int.Parse(parameters[0]), int.Parse(parameters[1]), $"{row[dataTable.Columns[0]]}").ToXML();
                            }
                            return new ResponseXML(0, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                        }
                    }

                    return new ResponseXML(0, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    dbClient.GetConnection().Close();
                    return new ResponseXML(0, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                }
            });







            // string AddDocPipe(int doc_id, int sub_pipe)
            functions[5] = ((List<string> parameters, DBClient dbClient) =>
            {
                if (parameters.Count != 4)
                {
                    return new ResponseXML(0, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                }
                int doc_id = int.Parse(parameters[2]);
                int pipe_id = int.Parse(parameters[3]);
                string qurry = ConfigurationManager.AppSettings.Get("InsertDoc");

                SqlCommand command = new SqlCommand(qurry, dbClient.GetConnection());
                command.Parameters.AddWithValue("@doc_id", doc_id);
                command.Parameters.AddWithValue("@pipe_id", pipe_id);
                command.Parameters.AddWithValue("@cprocess", pipes[pipe_id][0]);
                command.Parameters.AddWithValue("@pipe_index", 0);
                command.Parameters.Add("@start_time", SqlDbType.DateTime).Value = DateTime.Now;
                command.Parameters.AddWithValue("@avaliable", 1);
                command.Parameters.AddWithValue("@cround", 0);

                if (dbClient.DoCommand(command))
                {
                    // add the documents to the count
                    notification.AddProcess(pipes[pipe_id][0]);

                    // notify the change
                    notification.Notify(new DocProcessEventArgs(notification.ProcessQueueSize(pipes[pipe_id][0]), pipes[pipe_id][0], pipe_id));

                    return new ResponseXML(1, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                }
                return new ResponseXML(0, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
            });







            // string ResetDocPipe(int doc_id, int sub_pipe)
            functions[6] = ((List<string> parameters, DBClient dbClient) =>
            {
                if (parameters.Count != 4)
                {
                    return new ResponseXML(0, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                }
                int doc_id = int.Parse(parameters[2]);
                int pipe_id = int.Parse(parameters[3]);

                string qurry = $"SELECT * FROM flowTable WHERE doc_id = {doc_id} AND pipe_id = {pipe_id};";

                SqlCommand command = new SqlCommand(qurry, dbClient.GetConnection());

                try
                {
                    DataTable dataTable = dbClient.GetCommandValues(command);

                    if (dataTable.Rows.Count <= 0)
                    {
                        // not fount
                        return new ResponseXML(0, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                    }

                    var row = dataTable.Rows[0];

                    DataColumn avaliable = dataTable.Columns[5];
                    // if avaliable
                    if ((int)row[avaliable] > 0)
                    {
                        // reset the pipe
                        qurry = $"UPDATE flowTable SET avaliable = 0, start_time = GETDATE(), pipe_index = 0, cprocess = {pipes[pipe_id][0]}, cround = {(int)row[dataTable.Columns[6]] + 1} WHERE doc_id = {doc_id} AND pipe_id = {pipe_id}";

                        SqlCommand update_command = new SqlCommand(qurry, dbClient.GetConnection());
                        if (dbClient.DoCommand(update_command))
                        {
                            // remove from the current doucuments process count
                            notification.RemoveProcess((int)row[dataTable.Columns[2]]);
                            // notify of the current process in pipe
                            notification.Notify(new DocProcessEventArgs(notification.ProcessQueueSize((int)row[dataTable.Columns[2]]), (int)row[dataTable.Columns[2]], pipe_id));


                            // add to the first doucuments process count of the pipe
                            notification.AddProcess(pipes[pipe_id][0]);
                            // notify of the first process in pipe
                            notification.Notify(new DocProcessEventArgs(notification.ProcessQueueSize(pipes[pipe_id][0]), pipes[pipe_id][0], pipe_id));
                            return new ResponseXML(1, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                        }
                    }
                    return new ResponseXML(0, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    dbClient.GetConnection().Close();
                    return new ResponseXML(0, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                }
            });







            // string FinishWithDoc(int doc_id, int sub_pipe)
            functions[7] = ((List<string> parameters, DBClient dbClient) =>
            {
                if (parameters.Count != 4)
                {
                    return new ResponseXML(0, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                }
                int doc_id = int.Parse(parameters[2]);
                int pipe_id = int.Parse(parameters[3]);

                string qurry = $"SELECT * FROM flowTable WHERE doc_id = {doc_id} AND pipe_id = {pipe_id};";

                SqlCommand command = new SqlCommand(qurry, dbClient.GetConnection());

                try
                {
                    DataTable dataTable = dbClient.GetCommandValues(command);

                    if (dataTable.Rows.Count <= 0)
                    {
                        // not fount
                        return new ResponseXML(0, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                    }

                    var row = dataTable.Rows[0];

                    DataColumn avaliable = dataTable.Columns[5];
                    DataColumn cprocess = dataTable.Columns[2];
                    DataColumn pipe_index = dataTable.Columns[3];

                    // if not avaliable (was in use)
                    if ((int)row[avaliable] <= 0)
                    {
                        // if finish the pipe so the document is deleted
                        if ((int)row[pipe_index] + 1 >= pipes[pipe_id].Count)
                        {
                            qurry = $"DELETE FROM flowTable WHERE doc_id = {doc_id} AND pipe_id = {pipe_id}";
                            SqlCommand delete_command = new SqlCommand(qurry, dbClient.GetConnection());
                            if (dbClient.DoCommand(delete_command))
                            {
                                // decrease count of the process
                                notification.RemoveProcess((int)row[dataTable.Columns[2]]);
                                // notify the change
                                notification.Notify(new DocProcessEventArgs(notification.ProcessQueueSize((int)row[dataTable.Columns[2]]), (int)row[dataTable.Columns[2]], pipe_id));

                                return new ResponseXML(1, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                            }

                            // fail to delete
                            return new ResponseXML(0, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                        }

                        int new_index = (int)row[pipe_index] + 1;
                        // promote document in the pipe
                        qurry = $"UPDATE flowTable SET avaliable = 1, start_time = GETDATE(), pipe_index = {new_index}, cprocess = {pipes[pipe_id][new_index]} WHERE doc_id = {doc_id} AND pipe_id = {pipe_id}";

                        SqlCommand update_command = new SqlCommand(qurry, dbClient.GetConnection());
                        if (dbClient.DoCommand(update_command))
                        {
                            // remove the old process
                            notification.RemoveProcess(pipes[pipe_id][new_index-1]);
                            // add to the first doucuments process count of the pipe
                            notification.AddProcess(pipes[pipe_id][new_index]);
                            // notify of the first process in pipe
                            notification.Notify(new DocProcessEventArgs(notification.ProcessQueueSize(pipes[pipe_id][new_index]), pipes[pipe_id][new_index], pipe_id));
                            return new ResponseXML(1, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                        }
                    }
                    return new ResponseXML(0, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    dbClient.GetConnection().Close();
                    return new ResponseXML(0, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                }
            });








            // string DeleteDocPipe(int doc_id, int sub_pipe)
            functions[8] = ((List<string> parameters, DBClient dbClient) =>
            {
                if (parameters.Count != 4)
                {
                    return new ResponseXML(0, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                }
                int doc_id = int.Parse(parameters[2]);
                int pipe_id = int.Parse(parameters[3]);

                string qurry = $"SELECT * FROM flowTable WHERE doc_id = {doc_id} AND pipe_id = {pipe_id};";
                SqlCommand command = new SqlCommand(qurry, dbClient.GetConnection());

                try
                {
                    DataTable dataTable = dbClient.GetCommandValues(command);

                    if (dataTable.Rows.Count <= 0)
                    {
                        // not fount
                        return new ResponseXML(0, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                    }

                    var row = dataTable.Rows[0];

                    DataColumn avaliable = dataTable.Columns[5];
                    DataColumn cprocess = dataTable.Columns[2];
                    DataColumn pipe_index = dataTable.Columns[3];

                    // if avaliable
                    if ((int)row[avaliable] > 0)
                    {
                        qurry = $"DELETE FROM flowTable WHERE doc_id = {doc_id} AND pipe_id = {pipe_id}";
                        SqlCommand delete_command = new SqlCommand(qurry, dbClient.GetConnection());
                        if (dbClient.DoCommand(delete_command))
                        {
                            // decrease count of the process
                            notification.RemoveProcess((int)row[cprocess]);

                            // notify the change
                            notification.Notify(new DocProcessEventArgs(notification.ProcessQueueSize((int)row[dataTable.Columns[2]]), (int)row[dataTable.Columns[2]], pipe_id));

                            return new ResponseXML(1, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                        }

                        return new ResponseXML(1, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                    }
                    return new ResponseXML(0, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    dbClient.GetConnection().Close();
                    return new ResponseXML(0, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                }
            });










            /********** NEED TO TEST **********/
            // string ShowProcessQueue(int process)
            functions[9] = ((List<string> parameters, DBClient dbClient) => {
                if (parameters.Count != 3)
                {
                    return new ResponseXML(0, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                }
                int process = int.Parse(parameters[2]);
                string qurry = $"SELECT * FROM flowTable WHERE cprocess = {process} ORDER BY start_time ASC;";

                // AppDomain.CurrentDomain.SetData("PROCESS_ID", process);
                // string qurry = ConfigurationManager.AppSettings.Get("WaitingDocs");
                SqlCommand command = new SqlCommand(qurry, dbClient.GetConnection());

                try
                {
                    DataTable dataTable = dbClient.GetCommandValues(command);

                    List<string> docs = new List<string>();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        DataColumn avaliable = dataTable.Columns[5];

                        // if the document is avaliable
                        if ((int)row[avaliable] > 0)
                        {
                            // Add to the list
                            // doc id + sub_pipe id + round of the document + start time
                            // seperated with ';'
                            docs.Add($"{row[dataTable.Columns[0]]};{row[dataTable.Columns[1]]};{row[dataTable.Columns[6]]};{row[dataTable.Columns[4]]}");
                        }
                    }

                    return new ResponseXML(1, int.Parse(parameters[0]), int.Parse(parameters[1]), docs).ToXML();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    dbClient.GetConnection().Close();
                    return new ResponseXML(0, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                }
            });








            // string GetDocumentById(int doc_id, int sub_pipe)
            functions[10] = ((List<string> parameters, DBClient dbClient) =>
            {
                if (parameters.Count != 4)
                {
                    return new ResponseXML(0, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                }
                int doc_id = int.Parse(parameters[2]);
                int pipe_id = int.Parse(parameters[3]);

                string qurry = $"SELECT * FROM flowTable WHERE doc_id = {doc_id} AND pipe_id = {pipe_id};";

                SqlCommand command = new SqlCommand(qurry, dbClient.GetConnection());

                try
                {
                    DataTable dataTable = dbClient.GetCommandValues(command);

                    if (dataTable.Rows.Count <= 0)
                    {
                        // not fount
                        return new ResponseXML(0, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                    }

                    var row = dataTable.Rows[0];

                    DataColumn avaliable = dataTable.Columns[5];
                    // if avaliable
                    if ((int)row[avaliable] > 0)
                    {
                        // make it unavaliable the pipe
                        qurry = $"UPDATE flowTable SET avaliable = 0 WHERE doc_id = {(int)row[dataTable.Columns[0]]} AND pipe_id = {(int)row[dataTable.Columns[1]]}";

                        SqlCommand update_command = new SqlCommand(qurry, dbClient.GetConnection());
                        if (dbClient.DoCommand(update_command))
                        {
                            // remove from the current doucuments process count
                            notification.RemoveProcess((int)row[dataTable.Columns[2]]);
                            // notify of the current process in pipe
                            notification.Notify(new DocProcessEventArgs(notification.ProcessQueueSize((int)row[dataTable.Columns[2]]), (int)row[dataTable.Columns[2]], pipe_id));

                            return new ResponseXML(1, int.Parse(parameters[0]), int.Parse(parameters[1]), $"{row[dataTable.Columns[0]]}").ToXML();
                        }
                    }
                    return new ResponseXML(0, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    dbClient.GetConnection().Close();
                    return new ResponseXML(0, int.Parse(parameters[0]), int.Parse(parameters[1])).ToXML();
                }
            });



            /********* END - NEED TO TEST *********/

        }
    }
}
