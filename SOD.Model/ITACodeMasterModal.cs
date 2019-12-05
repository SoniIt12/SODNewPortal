using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{
    [Table("ITACodeSectorMaster")]
    public class ITACodeMasterModel
    {
        public Int64 ID { get; set; }
        public string SectorCode { get; set; }
        public string SectorName { get; set; }
        public string CountryCode { get; set; }
    }

}
