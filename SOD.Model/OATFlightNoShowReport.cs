using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SOD.Model
{

     [Table("OATFlightNoShowReport")]
    public class OATFlightNoShowReportModal
    {
        [Key]
        public Int64 ID { get; set; }
        public Int64 OATReqId { get; set; }
        public string Sector { get; set; }
        public string AirlineName { get; set; }
        public DateTime TravelDate { get; set; }
        public string DepartureTime { get; set; }
        public decimal Price { get; set; }
        public bool IsInternational { get; set; }
        public Int16? NoOfPassengers { get; set; }
        public DateTime EntryDate { get; set; }
        public string EntryBy { get; set; }
        public string FlightNo { get; set; }
    }
}
