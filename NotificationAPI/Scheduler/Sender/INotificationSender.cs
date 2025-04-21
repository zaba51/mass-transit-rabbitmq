using NotificationAPI.Entities;

namespace NotificationAPI.Scheduler.Sender
{
    public interface INotificationSender
    {
        public Task ProcessAsync(Notification notification);
    }
}
