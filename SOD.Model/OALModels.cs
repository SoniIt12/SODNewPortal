using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{
    [Table("OALTravelRequestFlightDetail")]
    public class OALModels
    {
        public Int64 Id { get; set; }
        public Int64 TravelRequestId { get; set; }
        public string OriginPlace { get; set; }
        public string DestinationPlace { get; set; }
       public DateTime DepartureDate { get; set; }
        public string DepartureTime { get; set; }
        public string DepFlightInfo { get; set; }
        public string DepFlightNumber { get; set; }
        public string DepReason { get; set; }
        
    }

    [Table("OALTravelRequestPassengerDetail")]
    public class OALPassengerModel
    {
        public Int64 Id { get; set; }
        public Int64 TravelRequestId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        
        
    }

    [Table("OALTravelRequestHotelDetail")]
    public class OALHotelModel
    { 
        public Int64 Id { get; set; }
        public Int64 TravelRequestId { get; set; }
        public string City { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string CheckinTime { get; set; }
        public string CheckoutTime { get; set; }
        public string Entitlement { get; set; }
        public DateTime EntryDate { get; set; }
        public string HotelCode { get; set; }
        public string HotelName { get; set; }
        public string HotelAddress { get; set; }
        public string HotelPhoneNo { get; set; }
        public Byte IsAllocated { get; set; }
        public string Remarks_Status { get; set; }
        public string HotelReferenceId { get; set; }
        public int guests { get; set; }
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
        public int HotelRequestId { get; set; }
        public string Occupancy { get; set; }
    }

    [Table("OALTravelRequestCabDetail")]
    public class OALCabModel
    {
        public Int64 Id { get; set; }
        public Int64 TravelRequestId { get; set; }
        public string PickupLoc { get; set; }
        public string PickupTime { get; set; }
        public string DropLoc { get; set; }
        public string DropTime { get; set; }
        
    }

    [Table("OALTravelRequestMaster")]
    public class OALTravelRequestMasterModel
    { 
        [Key]
        public Int64 TravelRequestId { get; set; }
        public string TravelRequestCode { get; set; }
        public DateTime RequestDate { get; set; }
        public string ReasonForTravel { get; set; }
        public string Gender { get; set; } 
        public int RequestedEmpId { get; set; }
        public string RequestedEmpCode { get; set; }
        public string RequestedEmpName { get; set; }
        public string RequestedEmpDesignation { get; set; }
        public string RequestedEmpDept { get; set; }
        public int RequestedEmpDeptId { get; set; }
        public string EmailId { get; set; }
        public string Phno { get; set; }
        public Int16 Passengers { get; set; }
        public string Meals { get; set; }
        public string Pnr { get; set; }
        public decimal PnrAmount { get; set; }
        public string BookingStatus { get; set; }
        public DateTime StatusDate { get; set; }
        public bool IsCabRequired { get; set; }
        public bool IsHotelRequired { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string ITHResponseStatus { get; set; }
        public string FinancialApproval { get; set; }

        [NotMapped]
        public string OriginPlace { get; set; }

        [NotMapped]
        public string DestinationPlace { get; set; }

        [NotMapped]
        public string FlightInfo { get; set; }

        [NotMapped]
        public string FlightNo { get; set; }

        [NotMapped]
        public DateTime TravelDate { get; set; }

        [NotMapped]
        public string HotelStatus { get; set; }

        [NotMapped]
        public string CancellationStatus { get; set; }

        [NotMapped]
        public string HodApprovalStatus { get; set; }

        [NotMapped]
        public string HotelName { get; set; }

        [NotMapped]
        public int HotelRequestId { get; set; }

    }


    [Table("OatTravelRequestApproval")]
    public class OatTravelRequestApprovalModel
    { 
        /// <summary>
        /// Travel Request Approval 
        /// </summary>

        [Key]
        public Int64 Id { get; set; }
        public Int64 TravelRequestId { get; set; }
        public int ApprovedByEmpId { get; set; }
        public DateTime ApprovalDate { get; set; }
        public short ApprovalStatus { get; set; }
        public short IsMandatoryTravel { get; set; }
        public string Comment { get; set; }

        public int ApprovedByEmpIdCLevel1 { get; set; }
        public int ApprovedByEmpIdCLevel2 { get; set; }
        public Int16 ApprovalStatusCLevel1 { get; set; }
        public Int16 ApprovalStatusCLevel2 { get; set; }
        public string CLevelComment1 { get; set; }
        public string CLevelComment2 { get; set; }
        public DateTime CLevelAppDate1 { get; set; }
        public DateTime CLevelAppDate2 { get; set; }

        public Int16 RevenueApprovedStatus { get; set; }
        public DateTime RevenueApprovedDate { get; set; }
    }


    [Table("OALUploadItenary")]
    public class OALUploadModel
    {
        [Key]
        public Int64 Id { get; set; }
        public Int64 TravelRequestId { get; set; }
        public string Pnr { get; set; }
        public string FlightInfo { get; set; }
        public DateTime TravelDate { get; set; }        
        public string FileName { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }  
    }

    [Table("HotelRequestApprovalOat")]
    public class HotelRequestApprovalOatModels 
    {  
        /// <summary>
        /// Travel Request Approval 
        /// </summary>
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

    [Table("HotelRequestCancellationOat")]
    public class HotelRequestCancellationOatModels
    { 
        [Key]
        public Int64 ID { get; set; }
        public Int64 TravelRequestId { get; set; }
        public DateTime CancellationDate { get; set; }
        public string CancellationReason { get; set; }
        public string hotelStatus { get; set; }
        public int HotelRequestId { get; set; }
    }

    [Table("ITHVendorListData")]
    public class IthVendorListDataModels
    { 
        [Key]
        public Int64 ID { get; set; }
        public string IthName { get; set; }
        public string Phone { get; set; }
        public string PrimaryEmail { get; set; }
        public string SecondaryEmail { get; set; }
        public string IthCode { get; set; }
    }

    [Table("ITHRequestApproval")]
    public class ITHRequestApprovalModels
    {
        [Key]
        public Int64 ID { get; set; }
        public Int64 TravelRequestId { get; set; }
        public DateTime RequestDate { get; set; }
        public string IthName { get; set; }
        public string Phone { get; set; }
        public string PrimaryEmail { get; set; }
        public string SecondaryEmail { get; set; }
        public string IthCode { get; set; }
        public string Status { get; set; }
	    public DateTime ApprovalDate { get; set; }
	    public string FlightNo { get; set; }
	    public string FlightName { get; set; }
	    public string DepartureTime { get; set; }
	    public string ArrivalTime { get; set; }
	    public decimal Amount { get; set; }
        public string FlightType { get; set; }
        public DateTime TravelDate { get; set; }
        public string FileName { get; set; }
        public string Pnr { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
    }

    [Table("ITHResponseDetail")]
    public class ITHResponseDetailModels
    { 
        [Key]
        public Int64 ID { get; set; }
        public Int64 TravelRequestId { get; set; }
        public DateTime ResponseDate { get; set; }
	    public string OriginPlace { get; set; }
	    public string DestinationPlace { get; set; }
        public DateTime TravelDate { get; set; }
        public string FlightNo { get; set; }
        public string FlightName { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public decimal Amount { get; set; }
        public string FlightType { get; set; }
        public string HodApproval { get; set; }
    }

    [Table("ITHFinancialApprovalHOD")]
    public class ITHFinancialApprovalHODModels
    { 
        [Key]
        public Int64 ID { get; set; }
        public Int64 TravelRequestId { get; set; }
        public DateTime ResponseDate { get; set; }
        public string OriginPlace { get; set; }
        public string DestinationPlace { get; set; }
        public DateTime TravelDate { get; set; }
        public string FlightNo { get; set; }
        public string FlightName { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }
        public decimal Amount { get; set; }
        public string FlightType { get; set; }
    }

    [Table("HotelRequestHODFinancialApprovalOat")]
    public class HotelRequestHODFinancialApprovalOatModels
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

    [Table("HotelInclusionNonContractualMasterOat")]
    public class HotelInclusionNonContractualMasterOatModels 
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
