using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationAPI.Entities;
using NotificationAPI.Repositories;
using NotificationAPI.Scheduler.Sender;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationAPI.Services
{
    public class NotificationScheduler : BackgroundService
    {
        //private readonly INotificationRepository _repository;
        //private readonly INotificationSender _sender;
        private readonly ILogger<NotificationScheduler> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        private static readonly TimeSpan _pollingInterval = TimeSpan.FromSeconds(10);

        public NotificationScheduler(
            //INotificationRepository repository,
            IServiceScopeFactory scopeFactory,
            //INotificationSender sender, 
            ILogger<NotificationScheduler> logger)
        {
            //_repository = repository;
            _scopeFactory = scopeFactory;
            //_sender = sender;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();

                var notificationRepo = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
                var notificationSender = scope.ServiceProvider.GetRequiredService<INotificationSender>();

                try
                {
                    var nowUtc = DateTime.UtcNow;

                    var notifications = await notificationRepo.GetPendingNotificationsAsync(nowUtc);

                    foreach (var notification in notifications)
                    {
                        await notificationSender.ProcessAsync(notification);

                        //notification.RetryCount++;

                        //if (success)
                        //{
                        //    notification.SentAt = DateTime.UtcNow;
                        //    notification.Status = NotificationStatus.Sent;
                        //}
                        //else if (notification.RetryCount >= 3)
                        //{
                        //    notification.Status = NotificationStatus.Failed;
                        //}

                        //notification.ForceSend = false;

                        await notificationRepo.UpdateAsync(notification);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during notification processing cycle");
                }

                await Task.Delay(_pollingInterval, stoppingToken);
            }
        }
    }
}
