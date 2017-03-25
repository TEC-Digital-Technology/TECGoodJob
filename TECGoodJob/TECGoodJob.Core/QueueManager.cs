using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TEC.Core.Sockets.Server;
using TECGoodJob.Core.Info;
using TECGoodJob.Core.Server;
using TECGoodJob.Core.Server.Event.Jobs;

namespace TECGoodJob.Core
{
    public class QueueManager
    {
        public QueueManager(JobMonitor jobMonitor, NodeManager nodeManager, SocketListener<DataHolder> socketListener)
        {
            this.SocketListener = socketListener;
            this.JobMonitor = jobMonitor;
            this.NodeManager = nodeManager;
        }
        public void dispatchJob(JobInfo jobInfo)
        {
            ClientInfo clientInfo = this.NodeManager.ConnectedClientInfo.OrderBy(t => t.JobInfoCollection.Count).First();
            clientInfo.JobInfoCollection.Add(jobInfo);
            this.JobMonitor.addManagedJobInfo(jobInfo);
            this.SocketListener.sendDataAsync(clientInfo.TokenId.Value, System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new DispatchJobEvent()
            {
                JobFunctionKey = jobInfo.JobFunctionKey,
                JobId = jobInfo.JobId
            })));
        }
        private JobMonitor JobMonitor { set; get; }
        private NodeManager NodeManager { set; get; }
        private SocketListener<DataHolder> SocketListener { get; set; }
    }
}
