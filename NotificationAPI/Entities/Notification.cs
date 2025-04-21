using System.ComponentModel.DataAnnotations.Schema;

namespace NotificationAPI.Entities
{
    public enum NotificationPriority
    {
        Low = 0,
        High = 1
    }
    public enum NotificationStatus   
    {
        Scheduled = 0,
        Sent = 1,
        Failed = 2,
        Cancelled = 3,
    }

    public class Notification
    {
        public Guid Id { get; set; }
        public string Recipient { get; set; }
        public string Channel { get; set; }
        public string Content { get; set; }
        public DateTime ScheduledAtUtc { get; set; }
        public string TimeZone { get; set; }


        public NotificationPriority Priority { get; set; } = NotificationPriority.Low;

        public DateTime? SentAt { get; set; }

        public int RetryCount { get; set; } = 0;

        public NotificationStatus Status { get; set; } = NotificationStatus.Scheduled;
        public bool ForceSend { get; set; } = false;
    }
}
