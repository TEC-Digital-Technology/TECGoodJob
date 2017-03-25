using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TEC.Core.Data;
using TEC.Core.Sockets.Client;
using TECGoodJob.Core.Enums;
using TECGoodJob.Core.Info;

namespace TECGoodJob.Core.Client.Handlers
{
    /// <summary>
    /// 描述類別具有處理 Socket 事件的伺服器端功能的基底型別
    /// </summary>
    public abstract class SocketClientEventHandlerBase<TEvent>
        where TEvent : SocketEventBase
    {
        public SocketClientEventHandlerBase()
        {
        }
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
        /// <param name="socketClient">引發事件的 Socket Client</param>
        /// <param name="serverInfo">遠端傳送資料的 Server 連線資訊</param>
        public abstract void handle(SocketClient<DataHolder> socketClient, ServerInfo serverInfo, TEvent @event);
        /// <summary>
        /// 取得將接收到的資料參數轉換為<paramref name="TEventArgs"/>型別的值轉換器，若傳回 <c>null</c> 參考則使用 Json 格式化字串
        /// </summary>
        public abstract ValueConverterBase<string, TEvent> ValueConverter { get; }
    }
}
