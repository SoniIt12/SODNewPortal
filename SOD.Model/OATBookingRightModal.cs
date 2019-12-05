using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{
    [Table("OATBookingRight")]
    public class OATBookingRightModal
    {
        [Key]
        public Int64 ID { get; set; }
        public string EmpCode { get; set; }
        public bool IsRoisteringRight { get; set; }
        public bool IsSpocRight { get; set; }      
    }
}
