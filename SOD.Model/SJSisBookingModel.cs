using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{
     public class SJSisBookingModel
    {      
        public string EmpCode { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string Gender { get; set; }
        public string Remarks { get; set; }

    }

    public class HodDetails
    {
        public string HoDTitle { get; set; }
        public string HoDName { get; set; }
        public string HodEmail { get; set; }
        public string HodMobileNo { get; set; }
    }

    
    public class PasswordModal
    {
        public string OldPwd { get; set; }
        public string NewPwd { get; set; }      
        public string ConfirmPwd { get; set; }
    }

}
