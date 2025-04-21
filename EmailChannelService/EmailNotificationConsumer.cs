using MassTransit;
using Shared.Contracts;

namespace EmailChannelService
{
    public class EmailNotificationConsumer : IConsumer<EmailNotificationMessage>
    {
        public async Task Consume(ConsumeContext<EmailNotificationMessage> context)
        {
            var message = context.Message;

            Console.WriteLine($"[EmailChannel] Wysyłam email do {message.Recipient}: {message.Content}");

            await Task.CompletedTask;
        }
    }
}
