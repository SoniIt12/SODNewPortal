using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOD.Model
{
    [Table("SodTravelRequest")]
    public class TravelRequestModels
    {
        /// <summary>
        /// Travel Request Properties
        /// </summary>

        [Key]
        public int TravelRequestId { get; set; }
        public string TravelRequestCode { get; set; }
        public int TravelRequestTypeId { get; set; }
        public int RequestedEmpId { get; set; }
        public string RequestedEmpName { get; set; }
        public string RequestedEmpDesignation { get; set; }
        public string RequestedEmpDept { get; set; }
        public string ReasonForTravel { get; set; }
        public string OriginPlace { get; set; }
        public string DestinationPlace { get; set; }
        public string Passengers { get; set; }
        public string RequestedUser { get; set; }
        public string TravelDate { get; set; }
        public string ReturnDate { get; set; }
        public DateTime RequestDate { get; set; }
        public string Sector { get; set; }
        public string FlightNo { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        [NotMapped]
        public string FlightName { get; set; }
        [NotMapped]
        public string EmailId { get; set; }
        [NotMapped]
        public string PassEmailId { get; set; }
        [NotMapped]
        public string PassAddressLine1 { get; set; }
        [NotMapped]
        public string PassAddressLine2 { get; set; }
        [NotMapped]
        public string PassCity { get; set; }
        [NotMapped]
        public string PassCountry { get; set; }
        [NotMapped]
        public string CLevelApproval1 { get; set; }
        [NotMapped]
        public string CLevelApproval2 { get; set; }
        [NotMapped]
        public string SodBookingType { get; set; }
        [NotMapped]
        public string BookingFor { get; set; }
        [NotMapped]
        public short IsMandatoryTravel { get; set; }
        [NotMapped]
        public string BookingRequestDate { get; set; }
        [NotMapped]
        public string Pnr { get; set; }
        [NotMapped]
        public string BookingStatus { get; set; }
        [NotMapped]
        public string RequestedEmpCode { get; set; }
        [NotMapped]
        public string Title { get; set; }
        [NotMapped]
        public string OrganizationName { get; set; }
        [NotMapped]
        public string OrganizationCode { get; set; }
        [NotMapped]
        public string CurrencyCode { get; set; }
        [NotMapped]
        public string PNRAmount { get; set; }
        [NotMapped]
        public string HotelStatus { get; set; }
        [NotMapped]
        public string Phone { get; set; }
        [NotMapped]
        public DateTime CheckinDate { get; set; }
        [NotMapped]
        public DateTime CheckoutDate { get; set; }
        [NotMapped]
        public bool accomodationRequired { get; set; }
        [NotMapped]
        public string ETA { get; set; }
        [NotMapped]
        public string CancellationStatus { get; set; }
        [NotMapped]
        public string CheckinTime { get; set; }
        [NotMapped]
        public string CheckoutTime { get; set; }
        [NotMapped]
        public string HotelType { get; set; }
        [NotMapped]
        public Int16 sharingId { get; set; }
        [NotMapped]
        public string HodApprovalStatus { get; set; }
        [NotMapped]
        public string HotelName { get; set; }
        [NotMapped]
        public decimal HotelPrice { get; set; }
        [NotMapped]
        public string HotelCity { get; set; }
        [NotMapped]
        public bool IsCabRequiredAsPerETA { get; set; }
        [NotMapped]
        public string CabPickupTime { get; set; }
        [NotMapped]
        public string HotelConfirmationNo { get; set; }
        [NotMapped]
        public string CheckinTimeHotel { get; set; }
        [NotMapped]
        public string CheckoutTimeHotel { get; set; }
        [NotMapped]
        public DateTime DateOfTravel { get; set; }
        [NotMapped]
        public int HotelRequestId { get; set; }
        [NotMapped]
        public Int16 FlightTypes { get; set; }
        [NotMapped]
        public int Id { get; set; }
        [NotMapped]
        public Int32 clubId { get; set; }
        [NotMapped]
        public DateTime ApprovalDate { get; set; }
        [NotMapped]
        public string Remarks_Status { get; set; }
        [NotMapped]
        public string HotelCurrencyCode { get; set; }
        [NotMapped]
        public bool IsVendorBooking { get; set; }
        [NotMapped]
        public bool SelectedFlight { get; set; }

    }
}