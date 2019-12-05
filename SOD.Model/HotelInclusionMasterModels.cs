using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOD.Model
{

    [Table("HotelInclusionMaster")]
    public class HotelInclusionMasterModels
    {

        /// <summary>
        /// Columns Name are created as per table Column Name schema
        /// </summary>

        [Key]
        public Int32 Id { get; set; }
        public string Location { get; set; }
        public string HotelName { get; set; }
        public string Accomodation { get; set; }
        public string Food { get; set; }
        public string AirportTransfers { get; set; }
        public string RoomService { get; set; }
        public string BuffetTime { get; set; }
        public string Laundry { get; set; }
        public string Amenities { get; set; }
        public string WiFi { get; set; }
        public string DrinkingWater { get; set; }
        public string TeaMaker { get; set; }
        public string Newspaper { get; set; }
        public string Discount { get; set; }
        public Int16 EntitlementType { get; set; }
        public string CheckinOutHours { get; set; }
        public string RetentionCancellation { get; set; }
        public string SpouseStay { get; set; }

    }

    [Table("HotelInclusionNonContractualMaster")]
    public class HotelInclusionNonContractualMasterModels
    { 
        [Key]
        public Int64 Id { get; set; }
        public Int64 TravelRequestId { get; set; }
        public string HotelName { get; set; }
        public string Accomodation { get; set; }
        public string Food { get; set; }
        public string AirportTransfers { get; set; }
        public string RoomService { get; set; }
        public string BuffetTime { get; set; }
        public string Laundry { get; set; }
        public int HotelRequestId { get; set; }
        
    }

}