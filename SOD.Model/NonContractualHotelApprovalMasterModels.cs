using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOD.Model
{

    [Table("NonContractualHotelApprovalMaster")]
    public class NonContractualHotelApprovalMasterModels
    { 
        
          /// <summary>
         /// Columns Name are created as per table Column Name schema
         /// </summary>

        [Key]
      public Int64 Id { get; set; }
      public string Title { get; set; }
      public string EmpCode { get; set; }
      public string EmpName { get; set; }
      public string EmailId { get; set; }
      public string Status { get; set; }
      public string Designation { get; set; }
      public string MobileNo { get; set; }
    }

}