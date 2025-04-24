using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts
{
    public class PushNotificationMessage: NotificationMessage
    {
        public string Recipient { get; set; }
        public string Content { get; set; }
    }
}
