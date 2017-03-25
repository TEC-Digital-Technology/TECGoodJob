using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TEC.Core.Data;
using TEC.Core.Sockets.Server;
using TECGoodJob.Core.Client.Event.Authenticate;
using TECGoodJob.Core.Enums;
using TECGoodJob.Core.Info;

namespace TECGoodJob.Core.Server.Handlers.Authenticate
{
    public class ClientAuthenticateEventHandler : SocketServerEventHandlerBase<ClientAuthenticateEvent>
    {

        public override void handle(SocketListener<DataHolder> socketServer, ClientInfo connectedClientInfo, ClientAuthenticateEvent @event)
        {
            connectedClientInfo.MachineName = @event.MachineName;
        }
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

        public override ValueConverterBase<string, ClientAuthenticateEvent> ValueConverter
        {
            get
            {
                return new ClientAuthenticateEventConverter();
            }
        }
        public class ClientAuthenticateEventConverter : ValueConverterBase<string, ClientAuthenticateEvent>
        {
            protected override string convertBackInternal(ClientAuthenticateEvent value, Type targetType, object parameter, CultureInfo culture)
            {
                return JsonConvert.SerializeObject(value);
            }

            protected override ClientAuthenticateEvent convertInternal(string value, Type targetType, object parameter, CultureInfo culture)
            {
                return JsonConvert.DeserializeObject<ClientAuthenticateEvent>(value);
            }
        }
    }
}
