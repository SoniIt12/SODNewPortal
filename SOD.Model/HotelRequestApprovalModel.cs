using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{
    [Table("HotelRequestApproval")]
    public class HotelRequestApprovalModel
    {

        [Key]
        public Int64 Id { get; set; }
        public Int64 TravelRequestId { get; set; }
        public string HotelCode { get; set; }
        public DateTime RequestDate { get; set; }
        public string HotelName { get; set; }
        public string HotelAddress { get; set; }
        public string HotelPhone { get; set; }
        public string HotelType { get; set; }
        public string PrimaryEmail { get; set; }
        public string SecondaryEmail { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public string HotelConfirmationNo { get; set; }
        public DateTime ApprovalDate { get; set; }
        public Int32 clubId { get; set; }
        public decimal HotelPrice { get; set; }
        public string FlightNo { get; set; }
        public string ETA { get; set; }
        public bool IsTaxIncluded { get; set; }
        public int HotelRequestId { get; set; }
        [NotMapped]
        public string SubmittedBy { get; set; }
        public string HotelCurrencyCode { get; set; }
    }

    [Table("HotelRequestHODFinancialApproval")]
    public class HotelRequestHODFinancialApprovalModels
    {
        [Key]
        public Int64 Id { get; set; }
        public Int64 TravelRequestId { get; set; }
        public string ApproverEmpCodeLevel1 { get; set; }
        public string ApproverEmpCodeLevel2 { get; set; }
        public DateTime ApprovalDateLevel1 { get; set; }
        public DateTime ApprovalDateLevel2 { get; set; }
        public string ApproverNameLevel1 { get; set; }
        public string ApproverNameLevel2 { get; set; }
        public Int16 ApprovalStatusLevel1 { get; set; }
        public Int16 ApprovalStatusLevel2 { get; set; }
        public Int16 ApprovalStatus { get; set; }
        public int HotelRequestId { get; set; }
    }


    [Table("HotelRequestRejection")]
    public class HotelRequestRejectionModel
    {

        [Key]
        public Int64 Id { get; set; }
        public Int64 TravelRequestId { get; set; }
        public string HotelCode { get; set; }
        public DateTime RequestDate { get; set; }
        public string HotelName { get; set; }
        public string HotelAddress { get; set; }
        public string HotelPhone { get; set; }
        public string HotelType { get; set; }
        public string PrimaryEmail { get; set; }
        public string SecondaryEmail { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public string HotelConfirmationNo { get; set; }
        public DateTime ApprovalDate { get; set; }
        public Int32 clubId { get; set; }
        public decimal HotelPrice { get; set; }
        public string FlightNo { get; set; }
        public string ETA { get; set; }
        public bool IsTaxIncluded { get; set; }
        public int HotelRequestId { get; set; }
    }


    [Table("HotelRequestCancellationByTraveldesk")]
    public class HotelCancellationByTraveldeskModel
    {
        [Key]
        public Int64 Id { get; set; }
        public Int64 TravelRequestId { get; set; }
        public string HotelCode { get; set; }
        public DateTime RequestDate { get; set; }
        public string HotelName { get; set; }
        public string HotelAddress { get; set; }
        public string HotelPhone { get; set; }
        public string HotelType { get; set; }
        public string PrimaryEmail { get; set; }
        public string SecondaryEmail { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public string HotelConfirmationNo { get; set; }
        public DateTime ApprovalDate { get; set; }
        public Int32 clubId { get; set; }
        public decimal HotelPrice { get; set; }
        public string FlightNo { get; set; }
        public string ETA { get; set; }
        public bool IsTaxIncluded { get; set; }
        public int HotelRequestId { get; set; }
    }

}
