using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TECGoodJob.Core.Enums;

namespace TECGoodJob.Core.Client.Event.Job
{
    public class NotifyJobStatusUpdatedEvent : SocketEventBase
    {
        public override string EventName
        {
            get
            {
                return nameof(NotifyJobStatusUpdatedEvent);
            }
        }

        public override SocketEventType EventType
        {
            get
            {
                return SocketEventType.Jobs;
            }
        }
        public Guid JobId { set; get; }
        public JobStatus JobStatus { set; get; }
        public string Message { set; get; }
    }
}
