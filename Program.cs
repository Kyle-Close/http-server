using System.Net;
using System.Net.Sockets;
using System.Text;

var ipEndPoint = new IPEndPoint(IPAddress.Any, 8080);
TcpListener listener = new(ipEndPoint);

try
{
    listener.Start();

    TcpClient handler = await listener.AcceptTcpClientAsync();
    await using NetworkStream stream = handler.GetStream();

    byte[] byteBuffer = new byte[1024];
    int bytesRead = await stream.ReadAsync(byteBuffer);

    var msg = Encoding.UTF8.GetString(byteBuffer, 0, bytesRead);
    Console.WriteLine(msg);

    // var message = $"{DateTime.Now}";
    // var dateTimeBytes = Encoding.UTF8.GetBytes(message);

    // await stream.WriteAsync(dateTimeBytes);
    // Console.WriteLine($"Sent message: \"{message}\"");
}
finally
{
    listener.Stop();
}
