using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{

    [Table("SodTransportVendorMaster")]
    public class TransportVendorMasterModels
    {

        /// <summary>
        /// Transport Vendor Models
        /// </summary>

        [Key]
        public Int32 VendorID { get; set; }
        public String VendorCode { get; set; }
        public String VendorName { get; set; }
        public String Vendor_Address { get; set; }
        public String CIN { get; set; }
        public String PAN { get; set; }
        public String TIN { get; set; }
        public String Service_Tax_No { get; set; }
        public String PhNo { get; set; }
        public String Email { get; set; }
        public String WebAddress { get; set; }
        public String StateCode { get; set; }
        public Int32 CountryCode { get; set; }
        public Byte IsActive { get; set; }
        public Byte IsDeleted { get; set; }
    }
}
