using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{
    [Table("SODEmployeeChangeRequestDetails")]
    public class SODEmployeeChangeRequestDetails
    {
        [Key]
        public Int64 ID { get; set; }
   

        public int RequestTypeId { get; set; }

        public string RequestTypeName { get; set; }


        public string CRUpdatet1 { get; set; }

        public string CRUpdatet2 { get; set; }

        public string CRUpdatet3 { get; set; }

        public bool HRApprovalStatus { get; set; }

        public bool FinanceStatus { get; set; }
        public string UserRemarks { get; set; }
        public string HRApprovalRemarks { get; set; }

        public string FinanceApprovalRemarks { get; set; }

        public Int64 ReqId { get; set; }
    }


    
}








