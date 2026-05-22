using System.Net;
using System.Net.Sockets;

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

// Wrap the client socket in a write-only NetworkStream (simulating a unidirectional stream).
using var writeOnlyStream = new NetworkStream(clientSocket, FileAccess.Write);
Console.WriteLine("Created write-only NetworkStream");

// Now try to read from the write-only stream using different methods.
// Per dotnet/runtime#121620, ReadByte and CopyTo throw different exception types.

Console.WriteLine();
Console.WriteLine("--- ReadByte ---");
try
{
    writeOnlyStream.ReadByte();
    Console.WriteLine("Unexpectedly succeeded");
}
catch (Exception ex)
{
    Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("--- CopyTo ---");
try
{
    writeOnlyStream.CopyTo(new MemoryStream());
    Console.WriteLine("Unexpectedly succeeded");
}
catch (Exception ex)
{
    Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("--- Read ---");
try
{
    writeOnlyStream.Read(new byte[1024], 0, 1024);
    Console.WriteLine("Unexpectedly succeeded");
}
catch (Exception ex)
{
    Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("--- ReadAsync ---");
try
{
    await writeOnlyStream.ReadAsync(new byte[1024]);
    Console.WriteLine("Unexpectedly succeeded");
}
catch (Exception ex)
{
    Console.WriteLine($"{ex.GetType().Name}: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("Done");
