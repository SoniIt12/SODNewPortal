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
    [Table("SodTravelRequestMaster_RejectedwithStandbyPNR")]
    public class TravelRequestMasterModels_RejectwithStandByPNR
    {
        /// <summary>
        /// Columns Name are created as per table Column Name schema
        /// </summary>
        [Key]
        public Int64 ID { get; set; }
        public Int64 TravelRequestId { get; set; }
        public string BookingFor { get; set; }
        public Int32? RequestedEmpId { get; set; }
        public string RequestedEmpCode { get; set; }
        public string Title { get; set; }
        public string RequestedEmpName { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string Pnr { get; set; }
        public decimal PnrAmount { get; set; }
        public DateTime? PNRCreateddate { get; set; }
        public string PNRCreatedby { get; set; }
        public bool? IsAmountPaidByTraveller { get; set; }
        public bool? IsHotelRequired { get; set; }
        public bool? IsOKtoBoard { get; set; }
        public bool? HotelApproval { get; set; }
        public string  SJSCHodEmailId { get; set; }
        public bool? IsVendorBooking { get; set; }
        public bool? IsSJSC { get; set; }
    }
}
