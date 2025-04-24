namespace Shared.Contracts
{
    public class EmailNotificationMessage: NotificationMessage
    {
        public string Recipient { get; set; }
        public string Content { get; set; }
    }
}
