using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{
    [Table("SodApproverSMSLog")]
    public class SodApproverSMSLogModels
    {
        [Key]
        public Int64 ID { get; set; }
        public Int64 TrRequestId { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string MobileNo { get; set; }
        public string EmailID { get; set; }
        public string Source { get; set; }
        public string SMSText { get; set; }
        public bool IsDelivered { get; set; }
        public DateTime DeliveryDate { get; set; }
    }
}
