using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TEC.Core.Collections;
using TEC.Core.Sockets.Server;
using TECGoodJob.Core.Enums;
using TECGoodJob.Core.Info;
using TECGoodJob.Core.Server.Handlers.Authenticate;
using TECGoodJob.Core.Server.Handlers;

namespace TECGoodJob.Core.Server
{
    public sealed class NodeManager : IDisposable
    {
        public NodeManager(SocketListener<DataHolder> socketListener)
        {
            this.ConnectedClientInfo = new ThreadSafeObservableCollection<ClientInfo>();
            this.SocketListener = socketListener;
            this.SocketListener.OnAccepted += this.SocketListener_OnAccepted;
            this.SocketListener.OnDataReceived += this.SocketListener_OnDataReceived;
            this.SocketListener.OnClosingClientSocket += this.SocketListener_OnClosingClientSocket;
            this.ConnectedClientInfo.CollectionChanged += this.ConnectedClientInfo_CollectionChanged;
        }

        private void SocketListener_OnDataReceived(object sender, TEC.Core.Sockets.Core.DataReceivedEventArgs e)
        {
            ClientInfo clientInfo;
            lock (this.ConnectedClientInfo)
            {
                clientInfo = this.ConnectedClientInfo.FirstOrDefault(t => t.TokenId.Value == e.TokenId);
            }
            if (clientInfo == null)
            {
                return;
            }
            JToken jToken = JToken.Parse(System.Text.Encoding.UTF8.GetString(e.DataHolder.Data));
            SocketEventType eventType = (SocketEventType)jToken["EventType"].Value<int>();
            string eventName = jToken["EventName"].Value<string>();
            foreach (object handler in this.RegisteredSocketServerEventHandlers)
            {
                if (eventType == ((dynamic)handler).EventType && eventName == ((dynamic)handler).EventName)
                {
                    ((dynamic)handler).handle(this.SocketListener, clientInfo, ((dynamic)handler).ValueConverter.convert(jToken.ToString()));
                }
            }
        }

        private void ConnectedClientInfo_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }

        private void SocketListener_OnAccepted(object sender, AcceptedEventArgs e)
        {
            lock (this.ConnectedClientInfo)
            {
                this.ConnectedClientInfo.Add(new ClientInfo()
                {
                    IPAddress = (e.DataHoldingUserToken.RemoteEndpoint as IPEndPoint).Address,
                    MachineName = null,
                    TokenId = e.DataHoldingUserToken.TokenId
                });

            }
        }
        private void SocketListener_OnClosingClientSocket(object sender, DataHoldingEventArgs e)
        {
            lock (this.ConnectedClientInfo)
            {
                this.ConnectedClientInfo.Remove(this.ConnectedClientInfo.First(t => t.TokenId == e.DataHoldingUserToken.TokenId));
            }
        }

        public void Dispose()
        {
            this.SocketListener.OnDataReceived -= this.SocketListener_OnDataReceived;
            this.SocketListener.OnAccepted -= this.SocketListener_OnAccepted;
            this.SocketListener.OnClosingClientSocket -= this.SocketListener_OnClosingClientSocket;
            this.ConnectedClientInfo.CollectionChanged -= this.ConnectedClientInfo_CollectionChanged;
        }
        private object[] RegisteredSocketServerEventHandlers
        {
            get
            {
                return new[] { new ClientAuthenticateEventHandler() };
            }
        }
        /// <summary>
        /// 已連線的客戶端
        /// </summary>
        public ThreadSafeObservableCollection<ClientInfo> ConnectedClientInfo { private set; get; }
        public SocketListener<DataHolder> SocketListener { get; set; }
    }
}
