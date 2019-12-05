using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{
    [Table("OATPAXTyepeMaster")]
    public class OATPAXTyepeMasterModal
    {        
        [Key]
        public Int16 PAXID { get; set; }
        public string PAXType { get; set; }
        public string PaxTypeRemarks { get; set; }
        public bool IsActive { get; set; }
    }
}
