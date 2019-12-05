using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOD.Model
{
   [Table("SodBlanketApprovals")]
    public class SodBlanketApprovalModels
    {
        /// <summary>
        /// Blanket Approvals Properties
        /// </summary>
        [Key]
        public short BlanketApprovalId { get; set; }
        public int EmployeeId { get; set; }
        public short IsActive { get; set; }

        [NotMapped]
        public string DepartmentName { get; set; }
    }
}