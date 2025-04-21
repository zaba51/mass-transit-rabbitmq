using NotificationAPI.Entities;
using NotificationAPI.Models;

namespace NotificationAPI.Services
{
    public interface INotificationService
    {
        public Task<Notification> CreateNotification(CreateNotificationRequest request);
    }
}
