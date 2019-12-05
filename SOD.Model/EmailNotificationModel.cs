using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{
   /// <summary>
   /// EMail Notification Property
   /// </summary>
    public class EmailNotificationModel
    {
        public string SmtpUser { get; set; }
        public string SmtpPass { get; set; }
        public string SmtpServer { get; set; }
        public string SmtpPort { get; set; }
        public string TemplateFilePath { get; set; }
        public string EmailSubjectName { get; set; }
        public string SenderEmailId { get; set; }
    }
}
