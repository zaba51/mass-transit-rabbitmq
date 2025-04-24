using MassTransit;
using NotificationAPI.Entities;
using Shared.Contracts;

namespace NotificationAPI.Scheduler.Channels
{
    public class EmailNotificationChannel : INotificationChannel
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public EmailNotificationChannel(ISendEndpointProvider sendEndpointProvider)
        {
            _sendEndpointProvider = sendEndpointProvider;
        }

        public string ChannelName => "email";

        public async Task<bool> SendAsync(Notification notification)
        {
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:email_queue"));

            await endpoint.Send(new EmailNotificationMessage
            {
                NotificationId = notification.Id,
                Recipient = notification.Recipient,
                Content = notification.Content
            });

            return true;
        }
    }
}