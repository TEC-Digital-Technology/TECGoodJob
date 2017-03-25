using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TEC.Core.Collections;
using TEC.Core.Sockets.Client;
using TECGoodJob.Core.Client.Event.Job;
using TECGoodJob.Core.Client.Handlers.Jobs;
using TECGoodJob.Core.Enums;
using TECGoodJob.Core.Info;

namespace TECGoodJob.Core
{
    public class JobExecutor : IDisposable
    {
        public JobExecutor(SocketClient<DataHolder> socketClient)
        {
            this.SocketClient = socketClient;
            this.SocketClient.OnDataReceived += this.SocketClient_OnDataReceived;
            this.SocketClient.OnConnected += this.SocketClient_OnConnected;
            this.SocketClient.OnClosingSocket += this.SocketClient_OnClosingSocket;
            this.ConnectedServerInfo = new ThreadSafeObservableCollection<ServerInfo>();
            this.ClientJobInfoObservableCollection = new ThreadSafeObservableCollection<ClientJobInfo>();
            this.ClientJobInfoObservableCollection.CollectionChanged += this.ClientJobInfoObservableCollection_CollectionChanged;
        }

        private void ClientJobInfoObservableCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                ((ClientJobInfo)e.NewItems[0]).PropertyChanged += this.ClientJobInfo_PropertyChanged;
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                ((ClientJobInfo)e.OldItems[0]).PropertyChanged -= this.ClientJobInfo_PropertyChanged;
            }
        }

        private void ClientJobInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ClientJobInfo clientJobInfo = sender as ClientJobInfo;
            if (e.PropertyName == nameof(clientJobInfo.JobStatus) ||
                e.PropertyName == nameof(clientJobInfo.Message))
            {
                this.SocketClient.sendDataAsync(clientJobInfo.ServerInfo.TokenId, System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new NotifyJobStatusUpdatedEvent()
                {
                    JobId = clientJobInfo.JobId,
                    JobStatus = clientJobInfo.JobStatus,
                    Message = clientJobInfo.Message
                })));
            }
        }

        private void SocketClient_OnClosingSocket(object sender, DataHoldingEventArgs e)
        {
            lock (this.ConnectedServerInfo)
            {
                this.ConnectedServerInfo.Remove(this.ConnectedServerInfo.First(t => t.TokenId == e.DataHoldingUserToken.TokenId));
            }
        }

        private void SocketClient_OnConnected(object sender, ConnectedEventArgs e)
        {
            lock (this.ConnectedServerInfo)
            {
                this.ConnectedServerInfo.Add(new ServerInfo()
                {
                    TokenId = e.DataHoldingUserToken.TokenId
                });
            }
        }

        private void SocketClient_OnDataReceived(object sender, TEC.Core.Sockets.Core.DataReceivedEventArgs e)
        {
            ServerInfo serverInfo;
            lock (this.ConnectedServerInfo)
            {
                serverInfo = this.ConnectedServerInfo.FirstOrDefault(t => t.TokenId == e.TokenId);
            }
            if (serverInfo == null)
            {
                return;
            }
            JToken jToken = JToken.Parse(System.Text.Encoding.UTF8.GetString(e.DataHolder.Data));
            SocketEventType eventType = (SocketEventType)jToken["EventType"].Value<int>();
            string eventName = jToken["EventName"].Value<string>();
            foreach (object handler in this.RegisteredSocketClientEventHandlers)
            {
                if (eventType == ((dynamic)handler).EventType && eventName == ((dynamic)handler).EventName)
                {
                    ((dynamic)handler).handle(this.SocketClient, serverInfo, ((dynamic)handler).ValueConverter.convert(jToken.ToString()));
                }
            }
        }
        public void executeJob(Guid jobId, string functionKey, ServerInfo serverInfo)
        {
            ClientJobInfo clientJobInfo = new ClientJobInfo();
            clientJobInfo.JobId = jobId;
            clientJobInfo.CreatedDateTime = DateTime.Now;
            clientJobInfo.ServerInfo = serverInfo;
            this.ClientJobInfoObservableCollection.Add(clientJobInfo);
            clientJobInfo.ExceptionObject = null;
            clientJobInfo.JobStatus = JobStatus.Dispatched;
            clientJobInfo.Message = "已接受工作。";
            clientJobInfo.LastUpdatedDateTime = DateTime.Now;
            switch (functionKey)
            {
                case "123456":
                    System.Threading.Thread.Sleep(3000);
                    break;
            }
            clientJobInfo.LastUpdatedDateTime = DateTime.Now;
            clientJobInfo.JobStatus = JobStatus.Succeed;
            clientJobInfo.Message = "工作完成。";

        }
        public void Dispose()
        {
            this.ClientJobInfoObservableCollection.CollectionChanged -= this.ClientJobInfoObservableCollection_CollectionChanged;
            this.SocketClient.OnConnected -= this.SocketClient_OnConnected;
            this.SocketClient.OnClosingSocket -= this.SocketClient_OnClosingSocket;
            this.SocketClient.OnDataReceived -= this.SocketClient_OnDataReceived;
        }
        public ThreadSafeObservableCollection<ClientJobInfo> ClientJobInfoObservableCollection { private set; get; }
        public SocketClient<DataHolder> SocketClient { private set; get; }
        private object[] RegisteredSocketClientEventHandlers
        {
            get
            {
                return new[] { new DispatchJobEventHanlder(this) };
            }
        }
        /// <summary>
        /// 已連線的客戶端
        /// </summary>
        public ThreadSafeObservableCollection<ServerInfo> ConnectedServerInfo { private set; get; }
    }
}
