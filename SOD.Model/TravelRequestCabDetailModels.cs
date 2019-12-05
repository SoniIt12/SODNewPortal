using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOD.Model
{
   [Table("SodTravelRequestCabDetail")]
   public class TravelRequestCabDetailModels
    {
        /// <summary>
        /// Columns Name are created as per table Column Name schema
        /// </summary>
        [Key]
        public Int64 Id { get; set; }
        public Int64 TravelRequestId { get; set; }        
        public string OneWay_From { get; set; }
        public string OneWay_From_Time { get; set; }
        public string OneWay_To { get; set; }
        public string OneWay_To_Time { get; set; }
        public string Return_From { get; set; }
        public string Return_From_Time { get; set; }
        public string Return_To { get; set; }
        public string Return_To_Time { get; set; }
        public DateTime Entry_Date { get; set; }
        public Byte IsAllocated { get; set; }
        public string Remarks_Status { get; set; }
        public string CabReferenceID { get; set; }
     
    }
}
