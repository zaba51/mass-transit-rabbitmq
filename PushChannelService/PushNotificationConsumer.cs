using MassTransit;
using Shared.Contracts;

namespace PushChannelService
{
    public class PushNotificationConsumer : IConsumer<PushNotificationMessage>
    {
        private readonly NotificationMetrics _metrics;

        private readonly IPublishEndpoint _publishEndpoint;
        private static readonly Random _random = new();

        public PushNotificationConsumer(IPublishEndpoint publishEndpoint, NotificationMetrics metrics)
        {
            _publishEndpoint = publishEndpoint;
            _metrics = metrics;
        }
        public async Task Consume(ConsumeContext<PushNotificationMessage> context)
        {
            if (_random.NextDouble() < 0.5)
            {
                throw new InvalidOperationException("Push send failed", null);
            }

            Console.WriteLine($"[PushChannel] Push sent to {context.Message.Recipient}");
            _metrics.RecordSuccess();

            await _publishEndpoint.Publish(new NotificationStatusEvent
            {
                NotificationId = context.Message.NotificationId,
                Status = NotificationDeliveryStatus.Sent
            });
        }
    }

}
