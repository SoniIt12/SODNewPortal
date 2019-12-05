using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{
    public class ExcelExportModel
    {
        public string BookingRequestDate { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationCode { get; set; }
        public string RequestedEmpCode { get; set; }
        public string RequestedEmpName { get; set; }
        public string RequestedEmpDesignation { get; set; }
        public string RequestedEmpDept { get; set; }
        public string TravelDate { get; set; }
        public string Sector { get; set; }
        public string SodBookingType { get; set; }
        public string FlightNo { get; set; }
        public string Pnr { get; set; }
        public string PNRAmount { get; set; }
        public string TravelRequestCode { get; set; }
        public string BookingFor { get; set; }
        public string IsMandatoryTravel { get; set; }
        public string IsVendorBooking { get; set; }
    }

    /// <summary>
    /// Only for to export SOD Approval Data
    /// </summary>
    public class ExcelExportApprovalModel
    {
        public string TravelRequestId { get; set; }
        public string TravelRequestCode { get; set; }
        public string RequestDate { get; set; }
        public string BookingFor { get; set; }
        public string IsMandatoryTravel { get; set; }
        public string ReasonForMandatoryTravel { get; set; }
        public string ReasonForTravel { get; set; }
        public string RequestedEmpCode { get; set; }
        public string RequestedEmpName { get; set; }
        public string RequestedEmpDesignation { get; set; }
        public string RequestedEmpDept { get; set; }
        public string EmailId { get; set; }
        public string Phno { get; set; }
        public string Pnr { get; set; }
        public string BookingStatus { get; set; }
        public string IsHotelRequired { get; set; }  
    }
}
