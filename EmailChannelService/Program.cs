using EmailChannelService;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<EmailNotificationConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("email_queue", e =>
        {
            e.ConfigureConsumer<EmailNotificationConsumer>(context);
        });
    });
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
