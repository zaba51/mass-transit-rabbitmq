using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationAPI.Entities;
using NotificationAPI.Repositories;
using NotificationAPI.Scheduler.Sender;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationAPI.NotificationSystem
{
    public class NotificationScheduler : BackgroundService
    {
        private readonly ILogger<NotificationScheduler> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        private static readonly TimeSpan _pollingInterval = TimeSpan.FromSeconds(10);

        public NotificationScheduler(
            IServiceScopeFactory scopeFactory,
            ILogger<NotificationScheduler> logger)
        {
            _scopeFactory = scopeFactory;
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
                        try
                        {
                            await notificationRepo.UpdateAsync(notification);
                            await notificationSender.ProcessAsync(notification);
                        }
                        catch(InvalidOperationException)
                        {
                            await notificationRepo.RemoveAsync(notification);
                            continue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during notification processing");
                }

                await Task.Delay(_pollingInterval, stoppingToken);
            }
        }
    }
}
