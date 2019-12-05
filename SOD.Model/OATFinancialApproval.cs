using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{
    [Table ("ITHFinancialApprovalHOD") ]
    public class OATFinancialApprovalHODModal
    {
        public Int64 OATRequestId {get;set;}
        public Int64 ITHVendorCode { get; set; }
        public Int64 PassengetrID { get; set; }
        public  String FlightType { get; set; }
        public String  OriginPlace { get; set; }
        public String DestinationPlace { get; set; }
        public DateTime DepartureDate { get; set; }
        public String DepartureTime { get; set; }
        public DateTime ArrivalDate { get; set; }
        public String ArrivalTime { get; set; }
        public String AirCraftName { get; set; }
        public String FlightNumber { get; set; }
        public int Amount { get; set; }
        public String IsInternational { get; set; }
        public String Approver1EmailID { get; set; }
        public DateTime Approver1Date { get; set; }
        public Int16 Approver1Status { get; set; }
        public String Approver2EmailID { get; set; }
        public String Approver2Date { get; set; }
        public Int16  Approver2Status { get; set; }
    }

    [Table("OATFinancialApprovalMaster_Roistering")]
    public class OATFinancialApprovalMaster_RoisteringModal
    {
        [Key]
        public Int64 Id { get; set; }
        public String Title { get; set; }
        public String EmpCode { get; set; }
        public String EmpName { get; set; }
        public String EmailId { get; set; }
        public String Designation { get; set; }
        public String Department { get; set; }
        public String MobileNo { get; set; }
        public String ApprovalFor { get; set; }
        public bool IsActive { get; set; }
   
    }

    [Table("OATFinancialApprovalDetail_Roistering")]
    public class OATFinancialApprovalDetail_RoisteringModal
    {
        [Key]
        public Int64 Id { get; set; }
        public Int64 OATId { get; set; }
        public Int64 PassengerId { get; set; }
        public Int64 FlightId { get; set; }
        public String Sector { get; set; }
        public String ApproverEmpCode { get; set; }
        public String ApproverEmpName { get; set; }
        public decimal? ApprovedAmount { get; set; }
        public DateTime ApprovalDate { get; set; }
        public String ApproverEmailID { get; set; }
        public Int16? ApprovalStatus { get; set; }
        [NotMapped]
        public String PassengerName { get; set; }
        [NotMapped]
        public DateTime departureDate { get; set; }
        [NotMapped]
        public String ApproverPhoneNo { get; set; }
    }
}
