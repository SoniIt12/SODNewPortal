using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOD.Model
{
    [Table("NONSODVendorMaster")]
    public class VendorModels
    {
        [Key]
        public Int64 ID { get; set; }
        public string VendorCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string EmailId { get; set; }
        public string MobileNo { get; set; }        
        public string ReqEmpTitle { get; set; }
        public string ReqEmpCode { get; set; }
        public string ReqEmpName { get; set; }
        public string ReqEmpEmailID { get; set; }
        public string ReqMobile { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime?  ModifiedDate { get; set; }
        public bool? IsActive { get; set; }
        public Int16 IsApproved { get; set; }
        public string ApprovedbyEmpEmailID { get; set; }
        public DateTime? Approvaldate { get; set; }
        public string CompanyName { get; set; }
        public Int32 BatchId { get; set; }
        [NotMapped]
        public bool Ischecked { get; set; }
        public string AddVendorOnBehalfof { get; set; }
        public bool? IsMailSent { get; set; }
    }
    [Table("NONSODVendorApproverMaster")]
    public class VendorHODDetails
    {
        [Key]
        public Int32 ID { get; set; }
        public string UserEmailID { get; set; }
        public string ApproverEmailID { get; set; }
        public string ApproverName { get; set; }
        public string ApproverTitle { get; set; }
        public bool? IsActive { get; set; }
        public string RequestorName { get; set; }
    }
    [Table("NONSODVendorApprovalLog")]
    public class VendorApprovallog
    {
        [Key]
        public Int64 ID { get; set; }
        public string VendorCode { get; set; }
        public Int16 ApprovalStatus { get; set; }
        public string ApprovedbyEmpEmailID { get; set; }
        public DateTime Approvaldate { get; set; }
        public DateTime ApprovalModifiedDate { get; set; }
        public Int32 BatchID { get; set; }
        public string Remarks { get; set; }
    }
    [Table("NONSODVendorMaster")]
    public class ExportVendorExcel
    {
        [Key]
        public Int64 SNo { get; set; }
        public string VendorCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string EmailId { get; set; }
        public string MobileNo { get; set; }
        public string ReqEmpCode { get; set; }
        public string ReqEmpName { get; set; }  
        public DateTime? AddDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsActive { get; set; }
        public string IsApproved { get; set; }
        public string ApprovedbyEmpEmailID { get; set; }
        public DateTime? Approvaldate { get; set; }
        public string CompanyName { get; set; }
       
    }
}
