using MassTransit;
using Microsoft.Extensions.Logging;
using NotificationAPI.Entities;
using NotificationAPI.Repositories;
using Shared.Contracts;
using System.Threading.Tasks;

namespace NotificationAPI.NotificationSystem
{
    public class NotificationStatusConsumer : IConsumer<NotificationStatusEvent>
    {
        private readonly INotificationRepository _repository;

        public NotificationStatusConsumer(INotificationRepository repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<NotificationStatusEvent> context)
        {
            var message = context.Message;
            var notification = await _repository.GetByIdAsync(message.NotificationId);

            if (notification == null)
            {
                return;
            }

            if (message.Status == NotificationDeliveryStatus.Sent)
            {
                notification.Status = NotificationStatus.Sent;
                notification.SentAt = DateTime.UtcNow;
            }
            else if (message.Status == NotificationDeliveryStatus.Failed)
            {
                notification.Status = NotificationStatus.Failed;
            }

            await _repository.UpdateAsync(notification);
        }
    }

}
