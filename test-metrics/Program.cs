using System.Diagnostics;
using System.Diagnostics.Metrics;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;


var builder = WebApplication.CreateBuilder(args);

// Custom metrics for the application
var meter = new Meter("OTel.Example", "1.0.0");
var counter = meter.CreateCounter<int>("count");

// Custom ActivitySource for the application
var counterActivitySource = new ActivitySource("OTel.Example");

var otel = builder.Services.AddOpenTelemetry();

otel.WithMetrics(metrics =>
{
    metrics.AddMeter(meter.Name).AddView(
        instrumentName: counter.Name,
        new MetricStreamConfiguration { CardinalityLimit = 10 });
});

// Export OpenTelemetry data via OTLP, using env vars for the configuration
var OtlpEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
if (OtlpEndpoint != null)
{
    otel.UseOtlpExporter();
}

var app = builder.Build();

int tag = 0;

app.MapGet("/", StartCounter);
async Task<string> StartCounter()
{
    for (int i = 0; i < 20; ++i) {
        TagList tags = default;
        tags.Add("tag", $"value={Interlocked.Increment(ref tag)}");
        counter.Add(1, tags);
    }
    return "Hello World!";
}
app.Run();
