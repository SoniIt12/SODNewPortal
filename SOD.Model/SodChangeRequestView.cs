using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOD
{
    public class SodChangeRequestView
    {
        public Int64 ReqId { get; set; }
        public string RequestedEmpId { get; set; }
        public string RequestedEmpName { get; set; }
        public string EmailID { get; set; }

        public string Phone { get; set; }

        public string UserRemarks { get; set; }

        public string EmployeCode { get; set; }

        public string Designation { get; set; }

        public string CR1Update { get; set; }
        public string CR2Update { get; set; }
        public string CR3Update { get; set; }

        public string RequestDate { get; set; }
        public string RequestType { get; set; }
        public bool HRApprovalStatus { get; set; }

        public string RemarksFin { get; set; }
        public bool FinanceStatus { get; set; }
        public int RequestTypeId { get; set; }
        public int DeptId { get; set; }

        public string DepartmentName { get; set; }
        public bool? IsRejectHR { get; set; }
        public bool? ISAccept { get; set; }

        public string FinanceApprovalRemarks { get; set; }
        public string HRApprovalRemarks { get; set; }
        
    }

}