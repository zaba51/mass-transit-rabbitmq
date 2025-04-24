using NotificationAPI.Entities;
using NotificationAPI.Scheduler.Channels;

namespace NotificationAPI.Scheduler.Sender
{
    public class NotificationSender : INotificationSender
    {
        private readonly Dictionary<string, INotificationChannel> _channels;

        public NotificationSender(IEnumerable<INotificationChannel> channels)
        {
            _channels = channels.ToDictionary(c => c.ChannelName.ToLower());
        }

        public async Task ProcessAsync(Notification notification)
        {
            if (!_channels.TryGetValue(notification.Channel.ToLower(), out var handler))
            {
                throw new InvalidOperationException();
            }

            notification.Status = NotificationStatus.Processing;
            var success = await handler.SendAsync(notification);
        }
    }
}
