using NotificationAPI.Entities;

namespace NotificationAPI.Models
{
    public class CreateNotificationRequest
    {
        public string Content { get; set; } = default!;
        public string Channel { get; set; } = default!;
        public string Recipient { get; set; } = default!;
        public string TimeZone { get; set; } = default!;
        public NotificationPriority Priority { get; set; } = NotificationPriority.Low;
        public DateTime ScheduledAt { get; set; }
    }
}
