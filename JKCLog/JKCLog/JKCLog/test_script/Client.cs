using System;
using System.IO;
using System.Text;
using System.Net.Sockets;

public class Client
{
    public static void RunClient()
    {
        string str_xml_decleare = $"<Request><ProgramID>1</ProgramID><SubProgramID>1</SubProgramID><Function><FunctionID>1</FunctionID><Param>hello world</Param></Function></Request>";

        string str_xml_subscribe = $"<Request><ProgramID>1</ProgramID><SubProgramID>1</SubProgramID><Function><FunctionID>2</FunctionID><Param>127.0.0.1</Param><Param>5005</Param><Param>1</Param></Function></Request>";

        // string str_xml_add_doc_pipe = $"<Request><ProgramID>1</ProgramID><SubProgramID>1</SubProgramID><Function><FunctionID>5</FunctionID><Param>4232</Param><Param>1</Param></Function></Request>";

        // string str_xml_get_document = $"<Request><ProgramID>1</ProgramID><SubProgramID>1</SubProgramID><Function><FunctionID>4</FunctionID><Param>1</Param></Function></Request>";

        // str_xml_finish_with_doc = $"<Request><ProgramID>1</ProgramID><SubProgramID>1</SubProgramID><Function><FunctionID>7</FunctionID><Param>4232</Param><Param>1</Param></Function></Request>";

        string responseData = "";

        responseData = sendData("127.0.0.1", 52616, str_xml_decleare);
        Console.WriteLine("Received: {0}", responseData);

        responseData = sendData("127.0.0.1", 52616, str_xml_subscribe);
        Console.WriteLine("Received: {0}", responseData);

        Console.WriteLine("\n Press Enter to continue...");
        Console.Read();
    }

    public static string sendData(string server, int port, string message)
    {
        try
        {
            TcpClient client = new TcpClient(server, port);

            // Translate the passed message into ASCII and store it as a Byte array.
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

            // Get a client stream for reading and writing.
            //  Stream stream = client.GetStream();
            NetworkStream stream = client.GetStream();

            // print the message
            Console.WriteLine("Sent: {0}", message);

            // Buffer to store the response bytes.
            data = new Byte[2048];

            // String to store the response ASCII representation.
            String responseData = String.Empty;

            // Read the first batch of the TcpServer response bytes.
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            Console.WriteLine("Received: {0}", responseData);

            // Close everything.
            stream.Close();
            client.Close();

            return responseData;

        }

        catch (ArgumentNullException e)
        {
            Console.WriteLine("ArgumentNullException: {0}", e);
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }

        return null;
    }
}
