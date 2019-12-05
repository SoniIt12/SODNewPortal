using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{

    [Table("BulkBookingDetails")]
    public class BulkUploadModels
    {

        /// <summary>
        /// Data Entity for Bulk Booking
        /// Upload Excel file
        /// Mapping Not Required
        /// </summary>

        [Key]
        public Int64 BReqId { get; set; }
        public Int64 TrnId { get; set; }       
        public string EmpCode { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string MobileNo { get; set; }
        public string EmailId { get; set; }
        public string TravelDate { get; set; }
        public string FlightNo { get; set; }
        public string Sector { get; set; }
        public string Purpose { get; set; }
        public string PNR { get; set; }
        public string Meal { get; set; }
        public string Beverage { get; set; }
        public Int16 PNR_Status { get; set; }
        public string BookingType { get; set; }
        public bool IsHotelRequired { get; set; }
        public string HotelCity { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string CheckinTime { get; set; }
        public string CheckoutTime { get; set; }
        public bool AirportTransport { get; set; }
        public string AgencyCode { get; set; }
        public bool IsChange { get; set; }
        public int AddNo { get; set; }
        public String ReasonForCancellation { get; set; }
        public bool IsBookingcancelled { get; set; }
        [NotMapped]
        public string HodApprovalStatus { get; set; }
        [NotMapped]
        public String PNR_Statuss { get; set; }
        [NotMapped]
        public DateTime CreatedDate { get; set; }
        [NotMapped]
        public bool IsDuplicate { get; set; }
        [NotMapped]
        public string HotelStatus { get; set; }
        [NotMapped]
        public string HotelName { get; set; }
        [NotMapped]
        public string HotelConfirmationNo { get; set; }
        [NotMapped]
        public Int16 IsAllocated { get; set; }
        [NotMapped]
        public Int16 sharingId { get; set; }
        [NotMapped]
        public Int32 clubId { get; set; }
        [NotMapped]
        public string PrimaryEmail { get; set; }
        [NotMapped]
        public string SecondaryEmail { get; set; }
        [NotMapped]
        public string HotelType { get; set; }
        [NotMapped]
        public int SrNo { get; set; }


    }

    /// <summary>
    /// For Bulk Booking Master
    /// </summary>

    [Table("BulkBookingMaster")]
    public class BulkUploadMasterModels
    {
        [Key]
        public Int64 TRId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime TransactionDate { get; set; }
        public string FileStatus { get; set; }
        public Int16 HotelStatus { get; set; }
        public string VerticalCode { get; set; }
        public Int32 DepartmentId { get; set; }        
        public string ApproverEmailID { get; set; }
        public string BookingType { get; set; }
        [NotMapped]
        public Int32 SNo { get; set; }
        [NotMapped]
        public DateTime TravelDate { get; set; }
        [NotMapped]
        public string Sector { get; set; }
        [NotMapped]
        public string EmpCode { get; set; }
        [NotMapped]
        public string HodApproval { get; set; }
    }



    /// <summary>
    /// Manage Employee PNR List
    /// </summary>
    public class BulkEmployeeList
    {
        public Int64 BTrId { get; set; }
        public string EmpCode { get; set; }
        public string PNRStatus { get; set; }
        public int SrNo { get; set; }
        public int AddNo { get; set; }
    }

    /// <summary>
    /// Manage Employee PNR List
    /// </summary>
    public class BulkEmployeeUpdatedList
    {
        public Int64 BTrId { get; set; }
        public string EmpCode { get; set; }
        public string MobileNo { get; set; }
        public string EmailId { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    public class BulkUpload_ExcelExport
    {
        public int SrNo { get; set; }
        public string EmpCode { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string MobileNo { get; set; }
        public string TravelDate { get; set; }
        public string FlightNo { get; set; }
        public string Sector { get; set; }
        public string Purpose { get; set; }
        public string PNR { get; set; }
        public string Meal { get; set; }
        public string Beverage { get; set; }
        public string BookingType { get; set; }

    }



    /// <summary>
    /// Export Bulk upload data PNR wise
    /// </summary>
    public class BulkUpload_ExcelExportPNRWise
    {
        public int SrNo { get; set; }
        public string EmpCode { get; set; }
        public string Name { get; set; }
        public string MobileNo { get; set; }
        public string EmailId { get; set; }
        public string TravelDate { get; set; }
        public string FlightNo { get; set; }
        public string Sector { get; set; }
        public string PNR { get; set; }
        public string Meal { get; set; }
        public string Beverage { get; set; }
        public bool IsHotelRequired { get; set; }
        public string BookingType { get; set; }
        public DateTime CreatedDate { get; set; }
        public string TrnId { get; set; }
    }


    /// <summary>
    /// Bulk hotel booking
    /// </summary>
    [Table("BulkTravelRequestHotelDetail")]
    public class BulkTravelRequestHotelDetailModels
    {

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
        public Int16 IsAllocated { get; set; }
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
        public Int16 IsCancelled { get; set; }
        public Int32 clubId { get; set; }
        public Int16 sharingId { get; set; }
        public decimal HotelPrice { get; set; }
        public string Occupancy { get; set; }
        public Int64 BReqId { get; set; }
        public string HotelCurrencyCode { get; set; }
        public string HodApprovalStatus { get; set; }

        public string SubmittedBy { get; set; }
    }

    /// <summary>
    /// Bulk hotel booking
    /// </summary>
    [Table("BulkTravelRequestHotelRejectionDetail")]
    public class BulkTravelRequestHotelRejectionDetailModels
    {

        [Key]
        public Int64 Id { get; set; }
        public Int64 TravelRequestId { get; set; }
        public string HotelReferenceID { get; set; }
        public string City { get; set; }
        public string EmployeeCode { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public Int32 NoOfGuest { get; set; }
        public DateTime? EntryDate { get; set; }
        public string HotelCode { get; set; }
        public string HotelName { get; set; }
        public string HotelAddress { get; set; }
        public string HotelPhoneNo { get; set; }
        public Int16 IsAllocated { get; set; }
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
        public Int16 IsCancelled { get; set; }
        public Int32 clubId { get; set; }
        public Int16 sharingId { get; set; }
        public decimal HotelPrice { get; set; }
        public string Occupancy { get; set; }
        public Int64 BReqId { get; set; }
        public string HotelCurrencyCode { get; set; }
        public string HodApprovalStatus { get; set; }

        public string SubmittedBy { get; set; }
        public DateTime? RejectionDate { get; set; }
    }
}
