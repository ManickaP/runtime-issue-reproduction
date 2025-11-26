// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Runtime.CompilerServices;

Console.WriteLine("Hello, World!");
if (IPAddress.IsValid("10.0.0.1"))
{
    Console.WriteLine("1");
}
if (IPAddress.IsValid("::1"))
{
    Console.WriteLine("2");
}
if (IPAddress.IsValid("10.0.1"))
{
    Console.WriteLine("3");
}
if (IPAddress.IsValid("::192.168.0.1"))
{
    Console.WriteLine("4");
}
if (IPAddress.IsValid("fe80::9656:d028:8652:66b6"))
{
    Console.WriteLine("5");
}