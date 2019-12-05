using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{
   [Table("SodTravelRequestApproval")]
   public class TravelRequestApprovalModels
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
}
