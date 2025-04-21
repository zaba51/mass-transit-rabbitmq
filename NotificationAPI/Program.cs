using MassTransit;
using Microsoft.EntityFrameworkCore;
using NotificationAPI.DbContexts;
using NotificationAPI.Repositories;
using NotificationAPI.Scheduler.Channels;
using NotificationAPI.Scheduler.Sender;
using NotificationAPI.Services;
using Shared.Contracts;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationSender, NotificationSender>();
builder.Services.AddScoped<INotificationChannel, EmailNotificationChannel>();

//builder.Services.AddScoped<INotificationChannelDispatcher, Notifi|cationChannelDispatcher>();

//builder.Services.AddScoped<INotificationChannelSender, EmailChannelSender>();
//builder.Services.AddScoped<INotificationChannelSender, PushChannelSender>();

builder.Services.AddHostedService<NotificationScheduler>();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);

        cfg.Send<EmailNotificationMessage>(s =>
        {
            s.UseRoutingKeyFormatter((context) =>  "email_queue");
        });

        //cfg.Send<PushNotificationMessage>(s =>
        //{
        //    s.UseRoutingKeyFormatter((context, message) => "push_queue");
        //});
    });
});

builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
