using Quartz;
using Quartz.Impl;
using SOD.Scheduler;
//using SOD.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOD.App_Start
{
    /// <summary>
    /// For Running the Sync Job
    /// </summary>
    public class AppScheduler
    {
        public static void Start()
        {
           // IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
           // scheduler.Start();
           //IJobDetail job = JobBuilder.Create<AppJob>().Build();
           // ITrigger trigger = TriggerBuilder.Create()
           //     .WithDailyTimeIntervalSchedule
           //       (s =>
           //          s.WithIntervalInHours(24)
           //         .OnEveryDay()
           //         .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(15, 30)) // Set time
           //       )
           //     .Build();
           // scheduler.ScheduleJob(job, trigger);




            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();
            IJobDetail job = JobBuilder.Create<AppJob>().Build();
            ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity("trigger11", "group11")
            .StartNow()
            .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(2)
            .RepeatForever())
            .Build();
            scheduler.ScheduleJob(job, trigger);
        }
    }
}