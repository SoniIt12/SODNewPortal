using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using SOD.SmsNotification;

namespace SOD.App_Start
{ 
    public class SmsScheduler
    {         
          /// <summary>
          /// Statr Schedular Method
          /// </summary>
        public static void Start()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<Jobclass>().Build();
            ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity("trigger1", "group1")
            .StartNow()
            .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(5)
            .RepeatForever())
            .Build();
            scheduler.ScheduleJob(job, trigger);
        }
    }
}