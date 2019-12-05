using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOD.Model;
using SOD.Services.Interface;
using SOD.Services.EntityFramework;
using System.Globalization;
using System.Data.SqlClient;


namespace SOD.Services.Repository
{
    public class BillingRepository:IBillingRepository
    {
        #region "Constructor Initialization"
        /// <summary>
        /// Constructor Initilization
        /// </summary>
        private readonly SodEntities _context;
        public BillingRepository(SodEntities sodEntities)
        {
            this._context = sodEntities;
        }
        #endregion



        #region IBillingRepository Members


        /// <summary>
        /// Get Billing details -Hotel PriceInfo
        /// </summary>
        /// <param name="fromdate"></param>
        /// <param name="todate"></param>
        /// <param name="type"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public IList<BulkUploadMasterModels> GetHotelBulkBillingMaster(string fromdate, string todate, Int16 type, Int16 criteria)
        {
            return ADO.SodCommonServices.GetHotelBulkBillingMaster(fromdate, todate, type, criteria);
        }



        /// <summary>
        /// Get Billing details -Hotel PriceInfo
        /// </summary>
        /// <param name="fromdate"></param>
        /// <param name="todate"></param>
        /// <param name="type"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public IList<HotelRequestApprovalModel> GetHotelBillingDetails(string fromdate, string todate, Int16 type, Int16 criteria)
        {
            return ADO.SodCommonServices.GetHotelBillingDetails(fromdate, todate, type, criteria);
        }


        /// <summary>
        /// Get Billing details -Hotel PriceInfo
        /// </summary>
        /// <param name="fromdate"></param>
        /// <param name="todate"></param>
        /// <param name="type"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public IList<ExcelExportModelBilling> GetHotelBillingDetails_ExportToExcel(string fromdate, string todate, Int16 type, Int16 criteria, Int64 trId)
        {
            return ADO.SodCommonServices.GetHotelBillingDetails_ExportToExcel(fromdate, todate, type, criteria, trId);
        }


        /// <summary>
        /// Get Hotel Approval Status 
        /// </summary>
        /// <param name="trId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public IList<string> GetHotelApprovalStatus(Int64 trId, Int16 criteria)
        {
            return ADO.SodCommonServices.GetHotelApprovalStatus(trId, criteria);
        }

        public IList<ExcelExportOATModelBilling> GetFlightBillingDetails_ExportToExcel(string fromdate, string todate, Int16 type, Int16 criteria, Int64 trId)
        {
            return ADO.SodCommonServices.GetFlightBillingDetails_ExportToExcel(fromdate, todate, type, criteria, trId);
        }

        public IList<BulkUploadMasterModels> GetFlightBulkBillingMaster(string fromdate, string todate, Int16 type, Int16 criteria)
        {
            return ADO.SodCommonServices.GetFlightBulkBillingMaster(fromdate, todate, type, criteria);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
