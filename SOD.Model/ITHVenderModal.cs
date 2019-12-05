using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Web;

namespace SOD.Model
{
    [Table("ITHVendorMaster")]
    public class ITHVenderModal
    {
        [Key]
        public Int64 ID { get; set; }
        public string ITHCode { get; set; }
        public string ITHName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string PrimaryEmail { get; set; }
        public string SecondaryEmail { get; set; }
        [NotMapped]
        public string submittedBy { get; set; }
        [NotMapped]
        public string Remarks { get; set; }
        [NotMapped]
        public String ithResponsestatus { get; set; }
        public bool IsActive { get; set; }
    }


    [Table("ITHServiceProviderRejectionLog")]
    public class ITHTransactionRejectionModal
    {
        [Key]
        public Int64 ID { get; set; }
        public Int64 TrnID { get; set; }
        public Int64 OATRequestID { get; set; }
        public DateTime RejectedDate { get; set; }
        public string RequestedBy { get; set; }
        public string ITHVendorCode { get; set; }
        public Int16 IsITHSentResponse { get; set; }
        public String Remarks { get; set; }
    }

    [Table("ITHTransactionMaster")]
    public class ITHTransactionMasterModal
    {
        [Key]
        public Int64 TrnID { get; set; }
        public Int64 OATRequestID { get; set; }
        public DateTime RequestedDate { get; set; }
        public string RequestedBy { get; set; }
        public string Remarks { get; set; }
        public string ITHVendorCode { get; set; }
        public string HODEmailID { get; set; }
        public bool IsReqSentToIth { get; set; }
        public Int16? IsITHSentResponse { get; set; }
        public DateTime? IthSentResponseDate { get; set; }
        public Int16?  OATDeskApproval { get; set; }
        public DateTime? OATDeskApprovalDate { get; set; }
        public Int16? HodApproval { get; set; }
        public DateTime? HodApprovalDate { get; set; }
        public Int16? UserApproval { get; set; }
        public DateTime? UserApprovalDate { get; set; }

        public String CXOEmailId { get; set; }
        public DateTime? CXO_ApprovalDate { get; set; }
        public Int16? CXO_ApprovalStatus { get; set; }

        [NotMapped]
        public String BookingFor { get; set; }
        [NotMapped]
        public bool IsFlightOnHold { get; set; }
        [NotMapped]
        public string CancellationStatus { get; set; }
        [NotMapped]
        public String CancellationType { get; set; }
        [NotMapped]
        public Int16? BookingStatus { get; set; }
    }

    [Table("ITHTransactionDetail")]
    public class ITHTransactionDetailModal
    {
        //hg
        [Key]
        public Int64 ID { get; set; }
        public Int64 TrnId { get; set; }
        public Int64 OATRequestID { get; set; }
        public Int64 PassengerID { get; set; }
        public string FlightType { get; set; }
        public string OriginPlace { get; set; }
        public string DestinationPlace { get; set; }
        public DateTime DepartureDate { get; set; }
        public string DepartureTime { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public string ArrivalTime { get; set; }
        public string AirCraftName { get; set; }
        public string FlightNumber { get; set; }
        public Decimal? Amount { get; set; }
        public bool IsInternational { get; set; }
        public bool IsFlightCancel { get; set; }
        public string FlightCancelBy { get; set; }
        public string FlightBookingStatus { get; set; }
        public bool IsFlightOnHold { get; set; }
        public string FlightOnHoldBy { get; set; }
        [NotMapped]
        public HttpPostedFileBase ITHUploadRefFiles { get; set; }
        [NotMapped]
        public string UploadedImage { get; set; }
        public String ITHUploadRefType { get; set; }        
        public byte[] ITHUploadRefFile { get; set; }
        public Int16 UserApproval { get; set; }
        public Int16 HodApproval { get; set; }
        public Int16 OATDeskApproval { get; set; }
        public DateTime? UserApprovalDate { get; set; }
        public DateTime? HodApprovalDate { get; set; }
        public DateTime? OATDeskApprovalDate { get; set; }
        public byte[] AttachedFileToRevert { get; set; }
        public String AttachedFileToRevertType { get; set; }        
        public String ReasonForCancellation { get; set; }

        [NotMapped]
        public bool isAlreadyNoShow { get; set; }

    }
    [Table("ITHTransactionDetailLog")]
    public class ITHTransactionDetailLogModal
    {
        [Key]
        public Int64 ID { get; set; }
        public Int64 TrnId { get; set; }
        public Int64 OATRequestID { get; set; }
        public Int64 PassengerID { get; set; }
        public String FlightType { get; set; }
        public String OriginPlace { get; set; }
        public String DestinationPlace { get; set; }

        public DateTime? DepartureDate { get; set; }

        public String DepartureTime { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public String ArrivalTime { get; set; }
        public String AirCraftName { get; set; }
        public String FlightNumber { get; set; }
        public Decimal? Amount { get; set; }
        public bool IsInternational { get; set; }
        [NotMapped]
        public string UploadedImage { get; set; }
        public String ITHUploadRefFileType { get; set; }
        public byte[] ITHUploadRefFile { get; set; }
        public Int16 OATDeskApproval { get; set; }
        public string OAtDeskUploadRefFileType { get; set; }
        public byte[] OATDeskUploadRefFile { get; set; }
        public DateTime? OATDeskApprovalDate { get; set; }      
        public String OATDeskRemarks { get; set; }
        public DateTime? EntryDate { get; set; }
        public Int64 LogBatchID { get; set; }

    }

    public class ITHResponseDataViewModal
    {
        public string OriginPlace { get; set; }
        public string DestinationPlace { get; set; }
        public DateTime DepartureDate { get; set; }
        public bool IsFlightOnHold { get; set; }
        public bool IsFlightCancel { get; set; }
        public string FlightOnHoldBy { get; set; }
        public String filePath { get; set; }
        public Int16 oatDeskApproval { get; set; }
        
      
        public List<ITHTransactionDetailModal> newIthDetail { get; set; }      
    }

    public class ITHTransactionDetailModalwithoutImage
    {
        //hg
        [Key]
        public Int64 ID { get; set; }
        public Int64 TrnId { get; set; }
        public Int64 OATRequestID { get; set; }
        public Int64 PassengerID { get; set; }
        public string FlightType { get; set; }
        public string OriginPlace { get; set; }
        public string DestinationPlace { get; set; }
        public DateTime DepartureDate { get; set; }
        public string DepartureTime { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public string ArrivalTime { get; set; }
        public string AirCraftName { get; set; }
        public string FlightNumber { get; set; }
        public Decimal? Amount { get; set; }
        public bool IsInternational { get; set; }
        public bool IsFlightCancel { get; set; }
        public string FlightCancelBy { get; set; }
        public string FlightBookingStatus { get; set; }
        public bool IsFlightOnHold { get; set; }
        public string FlightOnHoldBy { get; set; }
        //[NotMapped]
        //public HttpPostedFileBase ITHUploadRefFiles { get; set; }
        [NotMapped]
        public string UploadedImage { get; set; }
        //public byte[] ITHUploadRefFile { get; set; }
        public Int16 UserApproval { get; set; }
        public Int16 HodApproval { get; set; }
        public Int16 OATDeskApproval { get; set; }
        public DateTime? UserApprovalDate { get; set; }
        public DateTime? HodApprovalDate { get; set; }
        public DateTime? OATDeskApprovalDate { get; set; }
    }
}

