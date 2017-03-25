using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TEC.Core.Collections;
using TEC.Core.ComponentModel;

namespace TECGoodJob.Core.Info
{
    public class ClientInfo : NotifyPropertyChangedBase
    {
        private string machineName = null;
        private IPAddress ipAddress = null;
        private int? tokenId = null;
        public ClientInfo()
        {
            this.JobInfoCollection = new ThreadSafeObservableCollection<JobInfo>();
        }
        /// <summary>
        /// [變更通知]
        /// </summary>
        public string MachineName
        {
            set
            {
                this.setPropertyAndNotifyOnChanged(value, t => t.machineName, t => t.MachineName);
            }
            get
            {
                return this.machineName;
            }
        }

        /// <summary>
        /// [變更通知]
        /// </summary>
        public IPAddress IPAddress
        {
            set
            {
                this.setPropertyAndNotifyOnChanged(value, t => t.ipAddress, t => t.IPAddress);
            }
            get
            {
                return this.ipAddress;
            }
        }

        /// <summary>
        /// [變更通知]
        /// </summary>
        public int? TokenId
        {
            set
            {
                this.setPropertyAndNotifyOnChanged(value, t => t.tokenId, t => t.TokenId);
            }
            get
            {
                return this.tokenId;
            }
        }

        public ThreadSafeObservableCollection<JobInfo> JobInfoCollection {private set; get; }
    }
}
