using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{
    [Table("SODIntegratedApplicationMaster")]
    public class SODIntegratedApplicationMaster
    {
       [Key]
        public int ApplicationId { get; set; }

        public string ApplicationName { get; set; }
        public bool IsActive { get; set; }
    }
}
