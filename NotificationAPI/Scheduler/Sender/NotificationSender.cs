using NotificationAPI.Entities;
using NotificationAPI.Scheduler.Channels;
using NotificationAPI.Entities;
using System;
using System.Threading.Tasks;

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
            try
            {
                if(!_channels.TryGetValue(notification.Channel.ToLower(), out var handler)){
                    return;
                }

                var success = await handler.SendAsync(notification);

                if (success)
                {
                    notification.Status = NotificationStatus.Sent;
                    notification.SentAt = DateTime.UtcNow;
                }
                else
                {
                    HandleRetry(notification);
                }
            }
            catch
            {
                HandleRetry(notification);
            }

            notification.ForceSend = false;
        }

        private void HandleRetry(Notification notification)
        {
            notification.RetryCount++;

            if (notification.RetryCount >= 3)
            {
                notification.Status = NotificationStatus.Failed;
            }
        }
    }
}
