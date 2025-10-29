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
        List<string> headerLines = [];

        //  ReadAsync will only return 0, if zero bytes were requested or the socket performed a graceful shutdown
        while ((bytesRead = await stream.ReadAsync(byteBuffer)) > 0)
        {
            // Get the string from the byte buffer. Read from the start of the buffer to however many bytes were read
            string msg = Encoding.UTF8.GetString(byteBuffer, 0, bytesRead);
            string[] lines = msg.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                // If any line is blank, that signifies we reached the end of the header section
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    RequestLine rqLine = Parse.ParseRequestLine(headerLines[0]);
                    var headers = Parse.ParseHeaders(headerLines);

                    // Body is present
                    if (headers.ContainsKey("Content-Length"))
                    {
                        string body = "";
                        int bodyBytesRead = 0;
                        int bodyByteLength = int.Parse(headers["Content-Length"]);

                        // We have to read that amount of bytes in
                        // 1. Read remaining lines that come after the blank new line (if any)
                        // 2. Continue reading from the stream until we reach the end of the body (indicated by how many bytes)
                        for (int j = i + 1; j < lines.Length; j++)
                        {
                            body += lines[j];
                            bodyBytesRead = Encoding.UTF8.GetByteCount(body);
                        }

                        while ((bytesRead = await stream.ReadAsync(byteBuffer)) > 0 && bodyBytesRead < bodyByteLength)
                        {
                            msg = Encoding.UTF8.GetString(byteBuffer, 0, bytesRead);
                            body += msg;
                            bodyBytesRead = Encoding.UTF8.GetByteCount(body);
                            if (bodyBytesRead >= bodyByteLength)
                            {
                                break;
                            }
                        }

                        Console.WriteLine(body);
                    }

                    break;
                }
                else
                {
                    headerLines.Add(lines[i]);
                }
            }
        }
    }
}
finally
{
    server.Stop();
}
