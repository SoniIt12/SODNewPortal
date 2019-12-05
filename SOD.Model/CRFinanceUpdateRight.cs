using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{
    [Table("CRFinanceUpdateRight")]
    public class CRFinanceUpdateRight
    {
        [System.ComponentModel.DataAnnotations.Key]
        public Int64 FinanceId { get; set; }
        public string EmpCode { get; set; }
        public string Email { get; set; }
        public int DepartmentId { get; set; }
        public bool isActive { get; set; }
    }
}
