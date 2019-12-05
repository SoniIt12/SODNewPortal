using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{

  [Table("BulkBookingRequestApproval")]
   public class BulkBookingRequestApprovalModels
    {
        /// <summary>
        /// Travel Request Approval 
        /// </summary>

        [Key]
        public Int64 Id { get; set; }
        public Int64 TrRequestId { get; set; }
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
        public int AddNo { get; set; }
        public Int64 BReqId  { get; set; }
    }


    [Table("BulkBookingHODFinancialApproval")]
    public class BulkBookingHODFinancialApprovalModels
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

        public Int64 BreqId { get; set; }
    }
    
    [Table("BulkHotelInclusionNonContractualMaster")]
    public class BulkHotelInclusionNonContractualMasterModels
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
        public Int64 BreqId { get; set; }

        public int HotelRequestId { get; set; }

    }

    [Table("BulkBooking_DepartmentRight")]
    public class EmployeeDepartmentRights
    {
        [Key]
        public Int32 Id { get; set; }
        public string EmpCode { get; set; }
        public int DeptId { get; set; }
        public string DeptName { get; set; }
        public string VerticalCode { get; set; }
        public string VerticalName { get; set; }
        public bool IsActive { get; set; }
    }
}
