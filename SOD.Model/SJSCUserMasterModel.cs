using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{
    [Table("SJSCUserMaster")]
    public class SJSCUserMasterModels
    {
        public Int64 ID { get; set; }
        public string Title { get; set; }
        public string EmpCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string EmailID { get; set; }
        public string MobileNo { get; set; }
        public Int16 SJSCVerticalID { get; set; }
        public string UserId { get; set; }
        public string Pwd { get; set; }
        public bool IsVarified { get; set; }
        public bool IsActive { get; set; }
        public bool IsApprover { get; set; }

        public bool ResetPwd { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string HodName { get; set; }
        public string HodEmailId { get; set; }
        public string HodTitle { get; set; }
        public string HodMobileNo { get; set; }
        [NotMapped]
        public string ConfirmPwd { get; set; }
    }
}
