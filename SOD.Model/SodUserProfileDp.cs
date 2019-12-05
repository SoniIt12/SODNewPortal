using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
 

namespace SOD.Model
{
    [Table("SodUserProfileDp")]
    public class SodUserProfileDp 
    {
        [Key]
        public Int64 dpid { get; set; }
        public string ImageName { get; set; }
        public byte[] DpImage { get; set; }
        public string EmployeeCode { get; set; }
        [NotMapped]
        public  HttpPostedFileWrapper ImageFile { get; set; }
    }
}
