using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;


namespace SOD.Model
{
    [Table("OATTravelRequestMaster")]
    public class OATTravelRequestMasterModal
    {
        [Key]
        public Int64 OATRequestID { get; set; }
        public Int64 TravelRequestId { get; set; }
        public string OATRequestCode { get; set; }
        public DateTime RequestDate { get; set; }
        public string ReasonForTravel { get; set; }
        public string Gender { get; set; }
        public Int32 RequestedEmpId { get; set; }
        public string RequestedEmpCode { get; set; }
        public string RequestedEmpName { get; set; }
        public string RequestedEmpDesignation { get; set; }
        public string RequestedEmpDept { get; set; }
        public string EmailId { get; set; }
        public string PhoneNo { get; set; }

        public Int32 PaxNo { get; set; }
        public string BookingFor { get; set; }
        public string BookingForOnBehalfof { get; set; }
        public string BookingType { get; set; }
        public string Base { get; set; }
        public bool IsBulkHotel { get; set; }
        public Int16? BookingStatus { get; set; }
        public string ApproverEmailID { get; set; }
        public DateTime? ApproverDate { get; set; }

        public bool? IsSendRequestToITh { get; set; }
        [NotMapped]
        public Int16? IsITHSentResponse { get; set; }
        [NotMapped]
        public Int16? OATDeskApproval { get; set; }
        [NotMapped]
        public Int16? HodApproval { get; set; }
        [NotMapped]
        public Int16? UserApproval { get; set; }
        [NotMapped]
        public string Sector { get; set; }
        [NotMapped]
        public DateTime TravelDate { get; set; }
        [NotMapped]
        public bool CancellationStatus { get; set; }
        [NotMapped]
        public string cancelBy { get; set; }
        [NotMapped]
        public Int16? ApprovalStatus { get; set; }
        [NotMapped]
        public string Partailcancellation { get; set; }
    }

    [Table("OATTravelRequestFlightDetail")]
    public class OATTravelRequestFlightDetailModal
    {
        [Key]
        public Int64 ID { get; set; }
        public Int64 PassengerID { get; set; }
        public Int64 OATTravelRequestId { get; set; }
        public string FlightType { get; set; }
        public string OriginPlace { get; set; }
        public string DestinationPlace { get; set; }
        public DateTime DepartureDate { get; set; }
        public string DepartureTime { get; set; }
        public string AirCraftName { get; set; }
        public string FlightNumber { get; set; }
        public string ReasonForTravel { get; set; }
        public bool IsInternational { get; set; }
        public string CancelType { get; set; }
        public bool IsFlightCancel { get; set; }
        public String CancellationAttachmentType { get; set; }
        public byte[] CancellationAttachment { get; set; }
        public string CancellationReason { get; set; }
        public DateTime? CancelledTime { get; set; }
        public bool CancelledAcknowledgement { get; set; }
        public DateTime? CancelledAcknowledgementTime { get; set; }
        public string FlightCancelBy { get; set; }
        public string FlightBookingStatus { get; set; }
        public bool IsFlightOnHold { get; set; }
        public string FlightOnHoldBy { get; set; }

        public DateTime? HoldTime { get; set; }
        public bool HoldAcknowlwdgement { get; set; }
        public DateTime? HoldAcknowledgementTime { get; set; }
        public bool IsBoardedonFlight { get; set; }
        public string ReasonForNOTBoardingFlight { get; set; }
        public byte[] UploadBoardingPass { get; set; }

        [NotMapped]
        public int MulticityLength { get; set; }
        [NotMapped]
        public String Empcode { get; set; }
        [NotMapped]
        public String EmpName { get; set; }
        [NotMapped]
        public String Gender { get; set; }
        [NotMapped]
        public String PhoneNo { get; set; }

        [NotMapped]
        public String Emaild { get; set; }
        [NotMapped]
        public decimal? Amount { get; set; }

        [NotMapped]
        public DateTime? ArrivalDate { get; set; }
        [NotMapped]
        public String ArrivalTime { get; set; }
        [NotMapped]
        public String filePath { get; set; }
        [NotMapped]
        public String cancellationSrc { get; set; }
        [NotMapped]
        public String PNR { get; set; }
        [NotMapped]
        public Int16 oatDeskApproval { get; set; }

        [NotMapped]
        public ITHTransactionMasterModal IthDetail { get; set; }


    }



    [Table("OATTravelRequestPassengerDetail")]
    public class OATTravelRequestPassengerDetailModal
    {
        [Key]
        public Int64 PassengerID { get; set; }
        public Int64 OATTravelRequestId { get; set; }
        public string EmployeeCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public Int16 PAXType { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string PhoneNo { get; set; }
        public string EmailId { get; set; }
        public string Base { get; set; }
        public string PassportNo { get; set; }
        public DateTime DOB { get; set; }
        public bool IsFlightRequired { get; set; }
        public bool IsHotelRequired { get; set; }
        public bool IsEmployee { get; set; }
    }

    [Table("OATUploadItenary")]
    public class OATUploadItenaryModal
    {
        [Key]
        public Int64 ID { get; set; }
        public Int64 OATRequestId { get; set; }
        public Int64 PassengerID { get; set; }
        public string OriginPlace { get; set; }
        public string DestinationPlace { get; set; }

        public DateTime? DepartureDate { get; set; }
        public string DepartureTime { get; set; }

        public String AirlineName { get; set; }
        public String PNR { get; set; }
        public decimal? Amount { get; set; }
        public String AmountCurrencyCode { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }

        public DateTime? EntryDate { get; set; }
        [NotMapped]
        public string UploadedItenary { get; set; }
        [NotMapped]
        public string EmpName { get; set; }
        [NotMapped]
        public string EmpPhoneNo { get; set; }
        [NotMapped]
        public string EmpEmailId { get; set; }
    }

    public class viewOatDetailsModal
    {
        public Int64 ID { get; set; }
        public Int64 PassengerID { get; set; }
        public Int64 OATTravelRequestId { get; set; }
        public string EmployeeCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string OriginPlace { get; set; }
        public string DestinationPlace { get; set; }
        public string DepartureTime { get; set; }
        public string FlightType { get; set; }
        public string City { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public string IsFlightRequired { get; set; }
        public string IsHotelRequired { get; set; }
        public Int64 breqId { get; set; }
        public bool IsCancelled { get; set; }
        public string AirportTransport { get; set; }
        public Int64 TrnId { get; set; }

    }

    public class viewOatHotelDetailsModal
    {
        public Int64 TravelRequestId { get; set; }
        public String EmpCode { get; set; }
        public String ReasonForCancellation { get; set; }
        public String Sector { get; set; }
        public String City { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public String HotelStatus { get; set; }
        public string HotelConfirmationNo { get; set; }
        public string HotelName { get; set; }
        public string HotelAddress { get; set; }
        public string HotelPhoneNo { get; set; }
        public string Remarks_Status { get; set; }
        public bool IsBookingcancelled { get; set; }
    }
}
