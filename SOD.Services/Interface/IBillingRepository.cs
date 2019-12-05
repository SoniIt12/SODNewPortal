using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOD.Model;


namespace SOD.Services.Interface
{
    public interface IBillingRepository : IDisposable
    {

        /// <summary>
        /// Get Sod Booking and Cab Info 
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        IList<HotelRequestApprovalModel> GetHotelBillingDetails(string fromdate, string todate, Int16 type, Int16 criteria);

        /// <summary>
        /// Get Billing details -Hotel info Exort to excel
        /// </summary>
        /// <param name="fromdate"></param>
        /// <param name="todate"></param>
        /// <param name="type"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IList<ExcelExportModelBilling> GetHotelBillingDetails_ExportToExcel(string fromdate, string todate, Int16 type, Int16 criteria, Int64 trId);


        /// <summary>
        /// Get Hotel Bulk Booking Billing Master
        /// </summary>
        /// <param name="fromdate"></param>
        /// <param name="todate"></param>
        /// <param name="type"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IList<BulkUploadMasterModels> GetHotelBulkBillingMaster(string fromdate, string todate, Int16 type, Int16 criteria);


        /// <summary>
        /// Get Hotel Approval Status 
        /// </summary>
        /// <param name="trId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IList<string> GetHotelApprovalStatus(Int64 trId, Int16 criteria);

        IList<ExcelExportOATModelBilling> GetFlightBillingDetails_ExportToExcel(string fromdate, string todate, Int16 type, Int16 criteria, Int64 trId);

        IList<BulkUploadMasterModels> GetFlightBulkBillingMaster(string fromdate, string todate, Int16 type, Int16 criteria);
    }
}
