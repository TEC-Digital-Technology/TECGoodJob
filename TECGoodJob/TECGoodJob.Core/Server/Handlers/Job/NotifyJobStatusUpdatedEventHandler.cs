using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TEC.Core.Data;
using TEC.Core.Sockets.Server;
using TECGoodJob.Core.Client.Event.Job;
using TECGoodJob.Core.Enums;
using TECGoodJob.Core.Info;

namespace TECGoodJob.Core.Server.Handlers.Job
{
    public class NotifyJobStatusUpdatedEventHandler : SocketServerEventHandlerBase<NotifyJobStatusUpdatedEvent>
    {
        /// <summary>
        /// 不要把 JobMonitor 放進來!!! 請用 event handler
        /// </summary>
        /// <param name="jobMonitor"></param>
        public NotifyJobStatusUpdatedEventHandler(JobMonitor jobMonitor)
        {
            this.JobMonitor = jobMonitor;
        }
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

        public override ValueConverterBase<string, NotifyJobStatusUpdatedEvent> ValueConverter
        {
            get
            {
                return new DispatchJobEventConverter();
            }
        }
        private JobMonitor JobMonitor { set; get; }
        public override void handle(SocketListener<DataHolder> socketServer, ClientInfo connectedClientInfo, NotifyJobStatusUpdatedEvent @event)
        {
            JobInfo jobInfo = this.JobMonitor.ReadOnlyManagedJobInfo.FirstOrDefault(t => t.JobId == @event.JobId);
            if (jobInfo == null)
            {
                return;
            }
            jobInfo.JobStatus = @event.JobStatus;
        }
        public class DispatchJobEventConverter : ValueConverterBase<string, NotifyJobStatusUpdatedEvent>
        {
            protected override string convertBackInternal(NotifyJobStatusUpdatedEvent value, Type targetType, object parameter, CultureInfo culture)
            {
                return JsonConvert.SerializeObject(value);
            }

            protected override NotifyJobStatusUpdatedEvent convertInternal(string value, Type targetType, object parameter, CultureInfo culture)
            {
                return JsonConvert.DeserializeObject<NotifyJobStatusUpdatedEvent>(value);
            }
        }
    }
}
