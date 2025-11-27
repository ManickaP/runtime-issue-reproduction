// See https://aka.ms/new-console-template for more information
using System.Buffers;
using System.Net;
using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Unicode;

var stream = new MemoryStream();
await SseFormatter.WriteAsync<int>(GetItems(), stream, (item, writer) =>
{
    writer.Write(Encoding.UTF8.GetBytes(item.Data.ToString()));
});

Console.WriteLine(Encoding.UTF8.GetString(stream.GetBuffer()));

stream.Seek(0, SeekOrigin.Begin);

var parser = SseParser.Create(stream, (type, data) =>
{
    var str = Encoding.UTF8.GetString(data);
    return Int32.Parse(str);
});
await foreach (var item in parser.EnumerateAsync())
{
    Console.WriteLine($"{item.EventType}: {item.Data} {item.EventId} {item.ReconnectionInterval} [{parser.LastEventId};{parser.ReconnectionInterval}]");
}

static async IAsyncEnumerable<SseItem<int>> GetItems()
{
    yield return new SseItem<int>(1) { ReconnectionInterval = TimeSpan.FromSeconds(1) };
    yield return new SseItem<int>(2);
    yield return new SseItem<int>(3);
    yield return new SseItem<int>(4);
}