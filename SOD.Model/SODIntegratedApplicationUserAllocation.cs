using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{
   [Table("SODIntegratedApplicationUserAllocation")]
    public class SODIntegratedApplicationUserAllocation
    {
        public int ID { get; set; }
        public int ApplicationId { get; set; }
        public string EmployeeCode { get; set; }
        public bool IsActive { get; set; }

    }
}
