using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{
    [Table("SJSCVerticalMaster")]
    public class SJSCVerticalMasterModels
    {
        [Key]

        public Int16 VerticalID { get; set; }
        public  string VerticalName { get; set; }
        public bool IsActive { get; set; }
    }
}
