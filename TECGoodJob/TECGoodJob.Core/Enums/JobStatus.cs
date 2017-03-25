using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TECGoodJob.Core.Enums
{
    public enum JobStatus
    {
        Unknown,
        Pending,
        Dispatching,
        Dispatched,
        Succeed,
        Failed,
        FailedToRetry
    }
}
