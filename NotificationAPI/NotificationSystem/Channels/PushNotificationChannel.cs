using MassTransit;
using NotificationAPI.Entities;
using NotificationAPI.Scheduler.Channels;
using Shared.Contracts;

namespace NotificationAPI.NotificationSystem.Channels
{
    public class PushNotificationChannel: INotificationChannel
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public PushNotificationChannel(ISendEndpointProvider sendEndpointProvider)
        {
            _sendEndpointProvider = sendEndpointProvider;
        }

        public string ChannelName => "push";

        public async Task<bool> SendAsync(Notification notification)
        {
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:push_queue"));

            await endpoint.Send(new PushNotificationMessage
            {
                NotificationId = notification.Id,
                Recipient = notification.Recipient,
                Content = notification.Content
            });

            return true;
        }
    }
}
