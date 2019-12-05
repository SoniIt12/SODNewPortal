using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOD.Scheduler
{
    public class AppJob :IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            //SyncData Method
            SyncData s = new SyncData();
            s.syncDatas();
        }
    }
}