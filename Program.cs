using System.Net;
using System.Net.Sockets;
using System.Text;

var ipEndPoint = new IPEndPoint(IPAddress.Any, 8080);
TcpListener server = new(ipEndPoint);

try
{
    // Init the underlying socket, binding to local endpoint and listens for incoming requests to port 8080
    server.Start();

    // Buffer to hold data from incoming requests
    byte[] byteBuffer = new byte[1024];

    while (true) // Enter listening loop
    {
        // Wait until a client sends a request
        TcpClient tcpClient = await server.AcceptTcpClientAsync();

        // The stream is used to send & receive data
        await using NetworkStream stream = tcpClient.GetStream();

        // Since a client has connected, we can start reading the incoming stream of data
        //  If the incoming msg is > 1024 bytes then we have to process the data incrementally.
        int bytesRead;
        string fullMessage = "";

        //  ReadAsync will only return 0, if zero bytes were requested or the socket performed a graceful shutdown
        while ((bytesRead = await stream.ReadAsync(byteBuffer)) > 0)
        {
            // Get the string from the byte buffer. Read from the start of the buffer to however many bytes were read
            var msg = Encoding.UTF8.GetString(byteBuffer, 0, bytesRead);
            fullMessage += msg;
        }

        Console.WriteLine(fullMessage);
    }
}
finally
{
    server.Stop();
}
