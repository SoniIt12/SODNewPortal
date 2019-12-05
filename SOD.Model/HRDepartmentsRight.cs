using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{
    [Table("HRDepartmentsRight")]
    public class HRDepartmentsRight
    {
        [System.ComponentModel.DataAnnotations.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int DepartmentId { get; set; }

        public string EmployeeCode { get; set; }

        public string EmailId { get; set; }

        public Int16 IsActive { get; set; }
    }
}
