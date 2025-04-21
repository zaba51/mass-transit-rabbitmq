namespace Shared.Contracts
{
    public class EmailNotificationMessage
    {
        public Guid NotificationId { get; set; }
        public string Recipient { get; set; }
        public string Content { get; set; }
    }
}
