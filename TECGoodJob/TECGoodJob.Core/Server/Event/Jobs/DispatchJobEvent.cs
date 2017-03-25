using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TECGoodJob.Core.Enums;

namespace TECGoodJob.Core.Server.Event.Jobs
{
    public class DispatchJobEvent : SocketEventBase
    {
        public override string EventName
        {
            get
            {
                return nameof(DispatchJobEvent);
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
        public string JobFunctionKey { set; get; }
    }
}
