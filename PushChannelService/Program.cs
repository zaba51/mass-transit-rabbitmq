using PushChannelService;
using MassTransit;
using OpenTelemetry.Metrics;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Shared.Contracts;
using System.Diagnostics.Metrics;
var meter = new Meter("PushChannel.Metrics", "1.0");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(meter);
builder.Services.AddSingleton(sp => new NotificationMetrics(meter, "push"));
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PushNotificationConsumer>();
    x.AddConsumer<NotificationFaultObserver>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("push_queue", e =>
        {
            e.PrefetchCount = 1;
            e.UseConcurrencyLimit(1);

            e.UseMessageRetry(r => r.Interval(2, TimeSpan.FromSeconds(1)));

            e.ConfigureConsumer<PushNotificationConsumer>(context);
        });

        cfg.ReceiveEndpoint("push_queue_error", e =>
        {
            e.ConfigureConsumer<NotificationFaultObserver>(context);
        });
    });
});

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("PushChannelService"))
            .AddMeter("PushChannel.Metrics")
            .AddPrometheusExporter();
    });


var app = builder.Build();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.MapGet("/", () => "Hello World!");

app.Run();
