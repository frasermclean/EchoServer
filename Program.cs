using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EchoServer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine($"Invalid number arguments.");
                return;
            }

            if (int.TryParse(args[0], out int port))
            {
                if (port <= IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
                {
                    Console.Error.WriteLine($"Invalid port number: {port}");
                    return;

                }

                await ListenAsync(port);
            }
            else
            {
                Console.Error.WriteLine($"Could not parse argument to integer value.");
            }
        }

        private static async Task ListenAsync(int port)
        {
            TcpListener listener = new(IPAddress.Any, port);
            listener.Start();

            Console.WriteLine($"Listening on all interfaces on port: {port}.");

            while (true)
            {
                Console.WriteLine("Waiting for a connection...");

                using TcpClient client = await listener.AcceptTcpClientAsync();
                Console.WriteLine($"Connected to: {client.Client.RemoteEndPoint}");

                using NetworkStream stream = client.GetStream();

                // send welcome message
                byte[] welcome = Encoding.Default.GetBytes("Welcome to EchoServer.\r\n");
                await stream.WriteAsync(welcome.AsMemory(0, welcome.Length));

                byte[] buffer = new byte[1024];
                int bytesRead;

                // loop to receive all data from client
                while ((bytesRead = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length))) != 0)
                {
                    string data = Encoding.Default.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Received {bytesRead} bytes: \"{data.ToControlCodeString()}\".");

                    byte[] response = Encoding.Default.GetBytes(data);
                    await stream.WriteAsync(response.AsMemory(0, response.Length));
                    Console.WriteLine("Sent response.");
                }

                // end connection
                Console.WriteLine("Closing connection.");
                client.Close();
            }
        }
    }
}
