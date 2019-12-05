using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{
    [Table("CRHRApprovalStatus")]
    public class CRHRApprovalStatus
    {
        [Key]
        public Int64 appid { get; set; }

        public Int64 ReqId { get; set; }
        public bool IsApproved { get; set; }

        public bool IsRejected { get; set; }

        public DateTime StatusTime { get; set; }

        public string HRName { get; set; }

        public string HREmpCode { get; set; }
     
    }
}
