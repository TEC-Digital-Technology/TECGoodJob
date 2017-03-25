using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TEC.Core.ComponentModel;
using TECGoodJob.Core.Enums;

namespace TECGoodJob.Core.Info
{
    public class ClientJobInfo : NotifyPropertyChangedBase
    {
        private JobStatus jobStatus = JobStatus.Unknown;
        private DateTime lastUpdatedDateTime = DateTime.MinValue;
        private string message = null;
        public Guid JobId { set; get; }
        public JobStatus JobStatus
        {
            set
            {
                this.setPropertyAndNotifyOnChanged(value, t => t.jobStatus, t => t.JobStatus);
            }
            get
            {
                return this.jobStatus;
            }
        }
        public DateTime LastUpdatedDateTime
        {
            set
            {
                this.setPropertyAndNotifyOnChanged(value, t => t.lastUpdatedDateTime, t => t.LastUpdatedDateTime);
            }
            get
            {
                return this.lastUpdatedDateTime;
            }
        }
        public DateTime CreatedDateTime { set; get; }
        public object ExceptionObject { set; get; }
        public string Message
        {
            set
            {
                this.setPropertyAndNotifyOnChanged(value, t => t.message, t => t.Message);
            }
            get
            {
                return this.message;
            }
        }
        public ServerInfo ServerInfo { set; get; }
    }
}
