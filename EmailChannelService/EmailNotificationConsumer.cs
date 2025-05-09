﻿using MassTransit;
using Shared.Contracts;
using System.Diagnostics.Metrics;

namespace EmailChannelService
{
    public class EmailNotificationConsumer : IConsumer<EmailNotificationMessage>
    {
        private readonly NotificationMetrics _metrics;

        private readonly IPublishEndpoint _publishEndpoint;
        private static readonly Random _random = new();

        public EmailNotificationConsumer(IPublishEndpoint publishEndpoint, NotificationMetrics metrics)
        {
            _publishEndpoint = publishEndpoint;
            _metrics = metrics;
        }
        public async Task Consume(ConsumeContext<EmailNotificationMessage> context)
        {
            if (_random.NextDouble() < 0.5)
            {
                throw new InvalidOperationException("Email send failed", null);
            }

            Console.WriteLine($"[EmailChannel] Email sent to {context.Message.Recipient}");
            _metrics.RecordSuccess();

            await _publishEndpoint.Publish(new NotificationStatusEvent
            {
                NotificationId = context.Message.NotificationId,
                Status = NotificationDeliveryStatus.Sent
            });
        }
    }

}
