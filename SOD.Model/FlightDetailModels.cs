using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOD.Model
{
    [Table("SodTravelRequestFlightDetail")]
    public class FlightDetailModels
    {
        
          /// <summary>
         /// Columns Name are created as per table Column Name schema
         /// </summary>
         [Key]
         public Int64 Id { get; set; }
         public Int64 TravelRequestId { get; set; }
         public string OriginPlace { get; set; }
         public string DestinationPlace { get; set; }
         public DateTime TravelDate { get; set; }
	     public string FlightNo { get; set; }
	     public string DepartureTime { get; set; }
         public string ArrivalTime { get; set; }
         public string FlightName { get; set; }
         public Int16 FlightTypes { get; set; }
		 public bool IsHotelSendSms { get; set; } 
         
		 [NotMapped]
         public string Meals { get; set; }

         [NotMapped]
         public string Beverages { get; set; }
        [NotMapped]
        public string TravelDates { get; set;}
        [NotMapped]
        public string ReturnDates { get; set; }
    }
}