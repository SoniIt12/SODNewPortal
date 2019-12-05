using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{
    [Table("SodEmployeeCommonDetails")]
    public class EmdmAPIModels
    {

        [Key]
        public Int64 EntryID { get; set; }
        public int ID { get; set; }
        public string EmployeeCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public int DesignationID { get; set; }
        public string DesignationName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string EmployeeVertical { get; set; }
        public string EmpVerticalDescription { get; set; }
        public string PersonalEmail { get; set; } 
    }
}
