using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using SOD.SmsNotification;
using SOD.Controllers;
using SOD.Scheduler;

namespace SOD.App_Start
{
    public class Jobclass : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
             // Hotel Confirmation Notification
            SmsNotifications.getSmsData();
            
            //Reminder Approval notification
            var s = ReminderTimeLog.GetTimeLog();
            if (s == 240)
            {                
                ReminderNotificationController objreminder = new ReminderNotificationController();

                objreminder.HodApprovalReminder();
                objreminder.HodFinancialReminder();

               // objreminder.HodApprovalReminder();
              // objreminder.HodFinancialReminder();

                ReminderTimeLog.ResetTextTimeLog();
           }
        }
    }
}