using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOD.Model
{
    [Table("SodTravelRequestHotelDetail")]
    public class TravelRequestHotelDetailModels
    {
        /// <summary>
        /// Columns Name are created as per table Column Name schema
        /// </summary>
        [Key]
        public Int64 Id { get; set; }
        public Int64 TravelRequestId { get; set; }
        public string HotelReferenceID { get; set; }
        public string City { get; set; }
        public string EmployeeCode { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public Int32 NoOfGuest { get; set; }
        public DateTime EntryDate { get; set; }
        public string HotelCode { get; set; }
        public string HotelName { get; set; }
        public string HotelAddress { get; set; }
        public string HotelPhoneNo { get; set; }
        public Byte IsAllocated { get; set; }
        public string Remarks_Status { get; set; }
        public string CheckinTime { get; set; }
        public string CheckoutTime { get; set; }
        public string PrimaryEmail { get; set; }
        public string SecondaryEmail { get; set; }
        public string HotelType { get; set; }
        public string HotelStatus { get; set; }
        public string HotelConfirmationNo { get; set; }
        public bool AirportTransport { get; set; }
        public string usercancellation { get; set; }
        public Byte IsCancelled { get; set; }
        public Int32 clubId { get; set; }
        public string HodApprovalStatus { get; set; }
        public decimal HotelPrice { get; set; }
        public bool IsCabRequiredAsPerETA { get; set; }
        public string CabPickupTime { get; set; }
        public string CheckinTimeHotel{ get; set; }
        public string CheckoutTimeHotel { get; set; }
        public bool UserCheckinCheckoutUpdate { get; set; }
        public int HotelRequestId { get; set; }
        public string Occupancy { get; set; }
        public string HotelCurrencyCode { get; set; }
        public string SubmittedBy { get; set; }
        [NotMapped]
        public string Designation { get; set; }
        [NotMapped]
        public string EmployeeName { get; set; }
    }

    [Table("SodHotelListData")]
    public class SodHotelListDataModels
    { 
        /// <summary>
        /// Columns Name are created as per table Column Name schema
        /// </summary>
        [Key]
        public Int64 ID { get; set; }
        public string HotelName { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string PrimaryEmail { get; set; }
        public string SecondaryEmail { get; set; }
        public string HotelCode { get; set; }
        public string StationCode { get; set; }
        public string Status { get; set; }
        public string GMname { get; set; }
        public bool IsTaxIncluded { get; set; }
        public DateTime ContractStartDate { get; set; }
        public DateTime ContractEndDate { get; set; }
        public string ContractFile { get; set; } 
    }

    [Table("HotelRequestCancellation")]
    public class HotelRequestCancellationModels
    {
        [Key]
        public Int64 ID { get; set; }
        public Int64 TravelRequestId { get; set; }
        public DateTime CancellationDate { get; set; }
        public string CancellationReason { get; set; }
        public string hotelStatus { get; set; }
        public int HotelRequestId { get; set; }
    }

    [Table("SodHotelPriceListMaster")]
    public class SodHotelPriceListMasterModels
    {
        public Int64 ID { get; set; }
        public Int64 HotelId { get; set; }
        public string HotelCode { get; set; }
        public string StationCode { get; set; }
        public decimal SinglePrice { get; set; }
        public decimal DoublePrice { get; set; }
        public string AddUser { get; set; }
        public string ModifiedUser { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Int16 TCId { get; set; }
        public string HotelCurrencyCode { get; set; }
    }
}
