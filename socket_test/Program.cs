﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace socket_test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //await Task.WhenAll(RunServer(), RunClient());
        }

        static readonly TaskCompletionSource<IPEndPoint> serverEndpoint = new TaskCompletionSource<IPEndPoint>();

        static async Task RunClient()
        {
            var socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            socket.NoDelay = true;
            var endpoint = await serverEndpoint.Task;
            Console.WriteLine("Client connecting to: " + endpoint);
            socket.Connect(endpoint);
            Console.WriteLine("Client connected to: " + socket.RemoteEndPoint);
            var stream = new NetworkStream(socket, ownsSocket: true);
            await stream.WriteAsync(UTF8Encoding.UTF8.GetBytes("Ahoj"));
            var buffer = new byte[100];
            int readBytes;
            while ((readBytes = await stream.ReadAsync(buffer)) > 0)
            {
                Console.WriteLine("Client:" + UTF8Encoding.UTF8.GetString(buffer, 0, readBytes));
            }
            Console.WriteLine("Client:" + readBytes);
        }
        static async Task RunServer()
        {
            var listenSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(new IPEndPoint(IPAddress.IPv6Any, 0));
            listenSocket.Listen();
            serverEndpoint.SetResult(listenSocket.LocalEndPoint as IPEndPoint);
            Console.WriteLine("Server listening on: " + listenSocket.LocalEndPoint);
            var socket = await listenSocket.AcceptAsync().ConfigureAwait(false);
            var stream = new NetworkStream(socket, ownsSocket: true);
            var buffer = new byte[100];
            int readBytes = await stream.ReadAsync(buffer);
            Console.WriteLine("Server:" + UTF8Encoding.UTF8.GetString(buffer, 0, readBytes));
            stream.Dispose();
        }
    }
}
