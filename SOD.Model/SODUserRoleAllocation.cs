using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{
    [Table("SODUserRoleAllocation")]
    public class SODUserRoleAllocation
    {
       
        [Key]
        public int Id { get; set; }

        public string RoleId { get; set; }
     
        public string EmployeeCode { get; set; }

        public DateTime CreatedDate { get; set; }

        [NotMapped]
        public DateTime ModifiedDate { get; set; }

        public string CreatedByUser { get; set; }

        [NotMapped]
        public string ModifiedByUser { get; set; }
        public bool IsActive { get; set; }

    }
}
