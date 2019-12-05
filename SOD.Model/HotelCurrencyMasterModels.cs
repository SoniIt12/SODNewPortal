using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOD.Model
{
    [Table("HotelCurrencyMaster")]
    public class HotelCurrencyMasterModels
    {
        /// <summary>
        /// Columns Name are created as per table Column Name schema
        /// </summary>
        [Key]
        public Int32 CurrencyID { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyCountry { get; set; }
        public DateTime CreatedDate { get; set; }
        //public DateTime ModifiedDate { get; set; }
        //public string CreatedBy { get; set; }
        //public string ModifiedBy { get; set; }
        //public bool IsActive { get; set; }
    }
}
