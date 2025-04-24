using MassTransit;
using Shared.Contracts;

namespace PushChannelService
{
    public class NotificationFaultObserver : IConsumer<Fault<PushNotificationMessage>>
    {
        private readonly NotificationMetrics _metrics;

        private readonly IPublishEndpoint _publishEndpoint;

        public NotificationFaultObserver(IPublishEndpoint publishEndpoint, NotificationMetrics metrics)
        {
            _publishEndpoint = publishEndpoint;
            _metrics = metrics;
        }

        public Task Consume(ConsumeContext<Fault<PushNotificationMessage>> context)
        {
            _metrics.RecordFailure();

            return _publishEndpoint.Publish(new NotificationStatusEvent
            {
                NotificationId = context.Message.Message.NotificationId,
                Status = NotificationDeliveryStatus.Failed
            });
        }
    }
}
