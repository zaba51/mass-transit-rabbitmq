using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts
{
    public class NotificationStatusEvent
    {
        public Guid NotificationId { get; set; }
        public NotificationDeliveryStatus Status { get; set; }
    }

    public enum NotificationDeliveryStatus
    {
        Sent,
        Failed
    }
}
