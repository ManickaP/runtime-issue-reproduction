using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;


var counterValues = new ConcurrentDictionary<counterKey, int>();

var builder = WebApplication.CreateBuilder(args);

// Custom metrics for the application
var meter = new Meter("OTel.Example", "1.0.0");
var counter = meter.CreateObservableCounter<int>("count", GetCounters);

IEnumerable<Measurement<int>> GetCounters()
{
    foreach (var counter in counterValues)
    {
        if (counter.Value > 0)
        {
            yield return new Measurement<int>(counter.Value, counter.Key.TagList);
        }
    }
}

// Custom ActivitySource for the application
var counterActivitySource = new ActivitySource("OTel.Example");

var otel = builder.Services.AddOpenTelemetry();

otel.WithMetrics(metrics =>
{
    metrics.AddMeter(meter.Name).AddView(
        instrumentName: counter.Name,
        new MetricStreamConfiguration() /*{ CardinalityLimit = 10 }*/ );
    metrics.AddConsoleExporter(/*(_, options) =>
    {
        options.TemporalityPreference = MetricReaderTemporalityPreference.Delta;
    }*/);
});

var app = builder.Build();

int tag = 0;

app.MapGet("/", StartCounter);
async Task<string> StartCounter()
{
    for (int i = tag - 15; i >= 0 && i <= tag; ++i){
        var key = new counterKey($"value={i}");
        if (counterValues.AddOrUpdate(key, (key) => {
            Debug.Assert(false, $"The {key} must be present in the dictionary");
            return -1;
        }, (key, val) => val - 1) == 0)
        {
            var removed = counterValues.TryRemove(key, out int value);
            Debug.Assert(removed);
            Debug.Assert(value == 0);
        }
    }
    for (int i = 0; i < 20; ++i) {
        var key = new counterKey($"value={Interlocked.Increment(ref tag)}");
        counterValues.AddOrUpdate(key, 1, (key, val) => val + 1);
    }

    return "Hello World!";
}
app.Run();

struct counterKey : IEquatable<counterKey>
{
    public readonly string TagValue;

    public counterKey(string tagValue)
    {
        TagValue = tagValue;
    }

    public bool Equals(counterKey other)
        => TagValue.Equals(other.TagValue);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is counterKey ck ? Equals(ck) : false;

    public override int GetHashCode()
        => TagValue.GetHashCode();

    public TagList TagList
        => new TagList()
        {
            { "tag", TagValue }
        };
}
