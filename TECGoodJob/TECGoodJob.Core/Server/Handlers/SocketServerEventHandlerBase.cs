using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TEC.Core.Data;
using TEC.Core.Sockets.Server;
using TECGoodJob.Core.Enums;
using TECGoodJob.Core.Info;

namespace TECGoodJob.Core.Server.Handlers
{
    public abstract class SocketServerEventHandlerBase<T>
        where T : SocketEventBase

    {
        /// <summary>
        /// 取得關於此訂閱的事件類型
        /// </summary>
        public abstract SocketEventType EventType { get; }
        /// <summary>
        /// 取得與此動作相關的事件名稱
        /// </summary>
        public abstract string EventName { get; }
        /// <summary>
        /// 處理接收到的資料
        /// </summary>
        /// <param name="event">接收資料的參數</param>
        /// <param name="connectedClientInfo">與此事件相關的客戶端連線資訊</param>
        /// <param name="socketServer">引發事件的 Socket Server</param>
        public abstract void handle(SocketListener<DataHolder> socketServer, ClientInfo connectedClientInfo, T @event);
        /// <summary>
        /// 取得將接收到的資料參數轉換為<paramref name="TEventArgs"/>型別的值轉換器，若傳回 <c>null</c> 參考則使用 Json 格式化字串
        /// </summary>
        public abstract ValueConverterBase<string, T> ValueConverter { get; }
    }
}
