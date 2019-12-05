using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Model
{
    public class ExcelExportModelBilling
    {
        public Int32 SNo { get; set; }
        public string StaffId { get; set; }
        public string City_Name { get; set; }
        public string ETA { get; set; }
        public string Check_In { get; set; }
        public string Check_Out { get; set; }
        public string Check_In_Time { get; set; }
        public string Check_Out_Time { get; set; }
        public string Gender { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string Room_Night { get; set; }
        public string Room_Type { get; set; }
        public string HOTAC { get; set; }
        public string HOD_Approval { get; set; }
        //public string TravelRequestCode { get; set; }
        public string TravelRequestId { get; set; }
        public string FlightNo { get; set; }
        //public string HotelName { get; set; }
        public string ApprovalStatus { get; set; }
        public string HotelConfirmationNo { get; set; }
        public string ApproverName1 { get; set; }
        public string ApproverName2 { get; set; }
        public string AmountPerNight { get; set; }
        public string TotalAmount { get; set; }
        public string CurrencyCode { get; set; }
        public string Taxes_IsInclusive { get; set; }
        public string HotelType { get; set; }
    }


    public class ExcelExportOATModelBilling
    {

        public Int32 Sno { get; set; }
        public Int32 ReqNo { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string Gender { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public DateTime TravelDate { get; set; }
        public string AirlineName { get; set; }
        public string FlightNo { get; set; }
        public string Sector { get; set; }

        public string Pnr { get; set; }
        public string Price { get; set; }

        public string IsNoShow { get; set; }
        public string IsCancelled { get; set; }
        public string IsFullRedundCancelled { get; set; }
        public string HODApproval { get; set; }
        public string HODApprovalDate { get; set; }

    }
}
