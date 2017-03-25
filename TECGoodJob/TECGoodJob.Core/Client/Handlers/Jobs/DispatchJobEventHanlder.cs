using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TEC.Core.Data;
using TEC.Core.Sockets.Client;
using TECGoodJob.Core.Enums;
using TECGoodJob.Core.Info;
using TECGoodJob.Core.Server.Event.Jobs;

namespace TECGoodJob.Core.Client.Handlers.Jobs
{
    public class DispatchJobEventHanlder : SocketClientEventHandlerBase<DispatchJobEvent>
    {
        public DispatchJobEventHanlder(JobExecutor jobExecutor)
        {
            this.JobExecutor = jobExecutor;
        }
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

        public override ValueConverterBase<string, DispatchJobEvent> ValueConverter
        {
            get
            {
                return new DispatchJobEventConverter();
            }
        }
        private JobExecutor JobExecutor { set; get; }
        public override void handle(SocketClient<DataHolder> socketClient, ServerInfo serverInfo, DispatchJobEvent @event)
        {
            this.JobExecutor.executeJob(@event.JobId, @event.JobFunctionKey, serverInfo);
        }
        public class DispatchJobEventConverter : ValueConverterBase<string, DispatchJobEvent>
        {
            protected override string convertBackInternal(DispatchJobEvent value, Type targetType, object parameter, CultureInfo culture)
            {
                return JsonConvert.SerializeObject(value);
            }

            protected override DispatchJobEvent convertInternal(string value, Type targetType, object parameter, CultureInfo culture)
            {
                return JsonConvert.DeserializeObject<DispatchJobEvent>(value);
            }
        }
    }
}
