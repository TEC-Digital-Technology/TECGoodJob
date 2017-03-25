using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TEC.Core.ComponentModel;
using TECGoodJob.Core.Enums;

namespace TECGoodJob.Core.Info
{
    public class JobInfo : NotifyPropertyChangedBase
    {
        private JobStatus jobStatus = JobStatus.Unknown;
        public Guid JobId { set; get; }
        public string JobName { set; get; }
        /// <summary>
        /// [通知變更]
        /// </summary>
        public JobStatus JobStatus {
            set
            {
                this.setPropertyAndNotifyOnChanged(value, t => t.jobStatus, t => t.JobStatus);
            }
            get
            {
                return this.jobStatus;
            }
        }
        public string JobFunctionKey { set; get; }
    }
}
