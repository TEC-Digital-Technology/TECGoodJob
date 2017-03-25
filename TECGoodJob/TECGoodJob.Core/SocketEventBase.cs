using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TECGoodJob.Core.Enums;

namespace TECGoodJob.Core
{
    /// <summary>
    /// 描述類別具有儲存 Socket 事件資料的基底型別
    /// </summary>
    public abstract class SocketEventBase
    {
        /// <summary>
        /// 取得關於此訂閱的事件類型
        /// </summary>
        public abstract SocketEventType EventType { get; }
        /// <summary>
        /// 取得與此動作相關的事件名稱
        /// </summary>
        public abstract string EventName { get; }
    }
}
