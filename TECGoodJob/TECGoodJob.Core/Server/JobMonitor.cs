using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TEC.Core.Collections;
using TEC.Core.Sockets.Server;
using TECGoodJob.Core.Enums;
using TECGoodJob.Core.Info;
using TECGoodJob.Core.Server.Handlers.Job;

namespace TECGoodJob.Core.Server
{
    public class JobMonitor
    {
        public JobMonitor(SocketListener<DataHolder> socketListener, ThreadSafeObservableCollection<ClientInfo> connectedClientInfo)
        {
            this.SocketListener = socketListener;
            this.ManagedJobInfo = new ThreadSafeObservableCollection<JobInfo>();
            this.ReadOnlyManagedJobInfo = new ReadOnlyObservableCollection<JobInfo>(this.ManagedJobInfo);
            this.ManagedJobInfo.CollectionChanging += this.ManagedJobInfo_CollectionChanging;
            this.ManagedJobInfo.CollectionChanged += this.ManagedJobInfo_CollectionChanged;
            this.SocketListener.OnDataReceived += this.SocketListener_OnDataReceived;
            this.ConnectedClientInfo = connectedClientInfo;
        }
        private void SocketListener_OnDataReceived(object sender, TEC.Core.Sockets.Core.DataReceivedEventArgs e)
        {

            JToken jToken = JToken.Parse(System.Text.Encoding.UTF8.GetString(e.DataHolder.Data));
            SocketEventType eventType = (SocketEventType)jToken["EventType"].Value<int>();
            string eventName = jToken["EventName"].Value<string>();
            foreach (object handler in this.RegisteredSocketServerEventHandlers)
            {
                if (eventType == ((dynamic)handler).EventType && eventName == ((dynamic)handler).EventName)
                {
                    ((dynamic)handler).handle(this.SocketListener,
                        this.ConnectedClientInfo.First(t => t.TokenId == e.TokenId), ((dynamic)handler).ValueConverter.convert(jToken.ToString()));
                }
            }
        }
        private void ManagedJobInfo_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                ((JobInfo)e.NewItems[0]).PropertyChanged += this.JobInfo_PropertyChanged;
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                ((JobInfo)e.OldItems[0]).PropertyChanged -= this.JobInfo_PropertyChanged;
            }
        }

        private void ManagedJobInfo_CollectionChanging(object sender, CollectionChangingEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                lock (this.ManagedJobInfo)
                {
                    if (this.ManagedJobInfo.Any(t => t.JobId == ((JobInfo)e.NewItems[0]).JobId))
                    {
                        //擲例外 or 取消加入
                        e.Cancel = true;
                    }
                }
            }
        }

        public void addManagedJobInfo(JobInfo jobInfo)
        {
            lock (this.ManagedJobInfo)
            {
                this.ManagedJobInfo.Add(jobInfo);
            }
        }

        private void JobInfo_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            JobInfo jobInfo = sender as JobInfo;
            //更新資料來源
            if (e.PropertyName == nameof(jobInfo.JobStatus) && (jobInfo.JobStatus == Enums.JobStatus.Succeed
                || jobInfo.JobStatus == Enums.JobStatus.Failed))
            {
                lock (this.ManagedJobInfo)
                {
                    this.ManagedJobInfo.Remove(jobInfo);
                }
            }
        }

        /// <summary>
        /// 已管理的作業
        /// </summary>
        private ThreadSafeObservableCollection<JobInfo> ManagedJobInfo { set; get; }
        public ReadOnlyObservableCollection<JobInfo> ReadOnlyManagedJobInfo { private set; get; }

        public SocketListener<DataHolder> SocketListener { get; set; }
        private object[] RegisteredSocketServerEventHandlers
        {
            get
            {
                return new[] { new NotifyJobStatusUpdatedEventHandler(this) };
            }
        }
        private ThreadSafeObservableCollection<ClientInfo> ConnectedClientInfo { set; get; }
    }
}
