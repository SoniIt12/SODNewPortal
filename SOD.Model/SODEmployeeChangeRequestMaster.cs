using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{
   [Table("SODEmployeeChangeRequestMaster")]
    public class SODEmployeeChangeRequestMaster
    {

        [Key]
        public Int64 ReqId { get; set; }
        public string RequestedEmpId { get; set; }

        public string RequestedEmpName { get; set; }
        public int DesignationId { get; set; }

        public string DesignationName { get; set; }

        public string EmailID { get; set; }

        public string PhoneNo { get; set; }

        public DateTime RequestedDate { get; set; }

        public string RequestStatus { get; set; }

        public DateTime RequestStatusCloseDate { get; set; }

        public int DeptId { get; set; }


        public string DepartmentName { get; set; }




    }
}