using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOD.Model
{
   [Table("SodUsersInfo")]
    public class UserAccountModels
    {
        /// <summary>
        /// User Account Properties
        /// </summary>
        [Key]
        public int Id { get; set; }
        public int EmpCode { get; set; }
        public int DepartmentId { get; set; }
        public int DesignationId { get; set; }
        public string Email { get; set; }

        //[MaxLength(10, ErrorMessage = "BloggerName must be 10 characters or less"), MinLength(5)]
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }

        [NotMapped]
        public string OldPassword { get; set; }
    }
}