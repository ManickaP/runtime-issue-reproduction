using System.Net;
using System.Net.Sockets;
using System.Text;

// Start a TCP listener on a random port.
using var listener = new TcpListener(IPAddress.Loopback, 0);
listener.Start();
int port = ((IPEndPoint)listener.LocalEndpoint).Port;
Console.WriteLine($"Server listening on port {port}");

// Accept a connection on the server side.
var acceptTask = listener.AcceptSocketAsync();

// Connect from the client side.
using var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
await clientSocket.ConnectAsync(IPAddress.Loopback, port);
Console.WriteLine("Client connected");

using var serverSocket = await acceptTask;
Console.WriteLine("Server accepted connection");

// Shutdown the client's SEND side (half-close).
// The socket is still alive and not disposed, but writing is no longer allowed.
clientSocket.Shutdown(SocketShutdown.Send);
Console.WriteLine("Client shut down the sending side");

// Now try to write on the client socket whose write side is closed.
try
{
    byte[] data = Encoding.UTF8.GetBytes("Hello after shutdown");
    int sent = await clientSocket.SendAsync(data, SocketFlags.None);
    Console.WriteLine($"Unexpectedly sent {sent} bytes");
}
catch (SocketException ex)
{
    Console.WriteLine($"SocketException on send: {ex.SocketErrorCode} — {ex.Message}");
    Console.WriteLine($"  {ex}");
}
catch (Exception ex)
{
    Console.WriteLine($"Exception on send: {ex.GetType().Name} — {ex.Message}");
}

// Also show that reading on the server side sees EOF (0 bytes) because the client closed its send side.
byte[] buf = new byte[1024];
int read = await serverSocket.ReceiveAsync(buf, SocketFlags.None);
Console.WriteLine($"Server received {read} bytes (0 = EOF as expected)");

Console.WriteLine("Done");
