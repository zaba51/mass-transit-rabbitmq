using EmailChannelService;
using MassTransit;
using OpenTelemetry.Metrics;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Shared.Contracts;
using System.Diagnostics.Metrics;
var meter = new Meter("EmailChannel.Metrics", "1.0");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(meter);
builder.Services.AddSingleton(sp => new NotificationMetrics(meter, "email"));
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<EmailNotificationConsumer>();
    x.AddConsumer<NotificationFaultObserver>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("email_queue", e =>
        {
            e.PrefetchCount = 1;
            e.UseConcurrencyLimit(1);

            e.UseMessageRetry(r => r.Interval(2, TimeSpan.FromSeconds(1)));

            e.ConfigureConsumer<EmailNotificationConsumer>(context);
        });

        cfg.ReceiveEndpoint("email_queue_error", e =>
        {
            e.ConfigureConsumer<NotificationFaultObserver>(context);
        });
    });
});

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        metrics
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("EmailChannelService"))
            .AddMeter("EmailChannel.Metrics")
            .AddPrometheusExporter();
    });


var app = builder.Build();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.MapGet("/", () => "Hello World!");

app.Run();
