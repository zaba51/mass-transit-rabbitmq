using Microsoft.EntityFrameworkCore;
using NotificationAPI.Entities;
using NotificationAPI.Models;
using NotificationAPI.Repositories;

namespace NotificationAPI.Services
{
    public class NotificationService: INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        public NotificationService(INotificationRepository notificationRepository)
        { 
            _notificationRepository = notificationRepository;
        }

        public async Task<Notification> CreateNotification(CreateNotificationRequest request)
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(request.TimeZone);
            var scheduledAtLocal = TimeZoneInfo.ConvertTimeFromUtc(request.ScheduledAt, timeZoneInfo);

            if (scheduledAtLocal.Hour >= 22)
            {
                scheduledAtLocal = scheduledAtLocal.Date.AddDays(1).AddHours(6);
            }
            else if (scheduledAtLocal.Hour < 6)
            {
                scheduledAtLocal = scheduledAtLocal.Date.AddHours(6);
            }

            var adjustedUtc = TimeZoneInfo.ConvertTimeToUtc(scheduledAtLocal, timeZoneInfo);

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                Content = request.Content,
                Channel = request.Channel,
                Recipient = request.Recipient,
                ScheduledAtUtc = adjustedUtc,
                Priority = request.Priority,
                TimeZone = request.TimeZone,
            };

            await _notificationRepository.AddAsync(notification);

            return notification;
        }
        public async Task CancelNotification(Guid Id)
        {
            var notification = await _notificationRepository.GetByIdAsync(Id);
            if (notification == null)
            {
                throw new InvalidOperationException();
            }

            notification.Status = NotificationStatus.Cancelled;

            await _notificationRepository.UpdateAsync(notification);
        }

        public async Task ForceSend(Guid Id)
        {
            var notification = await _notificationRepository.GetByIdAsync(Id);
            if (notification == null)
            {
                throw new InvalidOperationException();
            }

            notification.ForceSend = true;

            await _notificationRepository.UpdateAsync(notification);
        }
    }
}
