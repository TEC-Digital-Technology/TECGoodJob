using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TECGoodJob.Core.Enums;

namespace TECGoodJob.Core.Client.Event.Authenticate
{
    public class ClientAuthenticateEvent : SocketEventBase
    {
        public override string EventName
        {
            get
            {
                return nameof(ClientAuthenticateEvent);
            }
        }

        public override SocketEventType EventType
        {
            get
            {
                return SocketEventType.Authenticate;
            }
        }
        public string MachineName { set; get; }
    }
}
