using NotificationAPI.Entities;

namespace NotificationAPI.Scheduler.Channels
{
    public interface INotificationChannel
    {
        string ChannelName { get; }
        Task<bool> SendAsync(Notification notification);
    }
}
