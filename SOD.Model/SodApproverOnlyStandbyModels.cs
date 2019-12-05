using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOD.Model
{
    [Table("SodApprovers_OnlyStandBy")]
    public class SodApproverOnlyStandbyModels
    {
        /// <summary>
        /// Sod Approvals Properties
        /// </summary>
        [Key]
        public short SodApproverId { get; set; }
        public int DesignationId { get; set; }
        public int DepartmentId { get; set; }
        public short IsActive { get; set; }
        public int EmployeeId { get; set; }
        public string VerticleId { get; set; }
        public string EmailId { get; set; }

        [NotMapped]
        public string DepartmentName { get; set; }
        [NotMapped]
        public string DesignationName { get; set; }
    }
      
}