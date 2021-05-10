using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
namespace Submit_System
{
    /// <summary>
    /// Use this socket with communicating with external APIs.
    /// </summary>
    public class ApiSocket : IDisposable
    {

        private readonly TcpClient socket;
        private StreamReader reader = null;
        private StreamWriter writer = null;
        public bool TestMode = false;
        /// <summary>
        /// Constructs the ApiSocket
        /// </summary>
        public ApiSocket()
        {
            socket = new TcpClient();
        }

        /// <summary>
        /// Connects the socket to the specified IP and port
        /// </summary>
        /// <param name="server">Server domain name or IP adress</param>
        /// <param name="port">Server's port number</param>
        /// <exception cref="ArgumentNullException">
        ///     server parameter is null.
        /// </exception>
        ///     <exception cref="ArgumentOutOfRangeException">
        ///         server parameter is longer than 255 characters.
        ///     </exception>
        ///     <exception cref="SocketException">
        ///         Host doesn't exist, or there was an issue with connecting to the Moss or DNS server.
        ///     </exception>
        ///     <exception cref="ArgumentException">
        ///         Server is an invalid Adresss.
        ///     </exception>
        /// 
        public void Connect(string server, int port)
        {
            socket.Connect(server, port);
            var stream = socket.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream)
            {
                AutoFlush = true
            };
        }
        /// <summary>
        ///     Read a line from the server.
        /// </summary>
        /// <returns>a line input from the server</returns>
        /// <exception cref="SocketException">Either the server timed out, or connection was cut.</exception>
        public String ReadLineFromServer()
        {
            string input;
            try
            {
                input = reader.ReadLine();
            }
            catch (IOException e)
            {
                throw e.InnerException;
            }
            if(TestMode)
            {
                Console.WriteLine("recv: " + input);
            }
            return input;
        }
        public async Task<string> ReadLineFromServerAsync() {
            string input;
            try
            {
                input = await reader.ReadLineAsync();
            }
            catch (IOException e)
            {
                throw e.InnerException;
            }
            return input;
        }
        /// <summary>
        ///     Send a message to the server.
        /// </summary>
        /// <param name="message">the message to be sent to the server</param>
        /// <exception cref="SocketException">Either the server timed out, or connection was cut.</exception>
        public void SendCommand(string message)
        {
            try
            {
                writer.Write(message);
            }
            catch (IOException e)
            {
                throw e.InnerException;
            }
            if(TestMode)
            {
                Console.WriteLine("sent: "+ message);
            }
        }
        public async Task SendCommandAsync(string message)
        {
            Console.WriteLine(message);
            try
            {
                await writer.WriteAsync(message);
            }
            catch (IOException e)
            {
                throw e.InnerException;
            }
        }
        public void SendCommandLine(string message) {
            SendCommand(message + '\n');
        }
        public async Task SendCommandLineAsync(string message) {
            await SendCommandAsync(message + '\n');
        }
        public void UploadFile() {

        }
        /// <summary>
        /// Frees up all the resources of the object.
        /// Be sure to use this after you're done with it.
        /// </summary>
        public void Dispose()
        {
            if(reader != null)
            {
                reader.Dispose();
            }
            if (writer != null)
            {
                writer.Dispose();
            }
            socket.Dispose();
        }
    }
}
