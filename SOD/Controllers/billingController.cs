using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;
using SOD.Services.Repository;
using System.Data.Entity.Infrastructure;
using System.ComponentModel;
using System.IO;
using SOD.CommonWebMethod;
namespace SOD.Controllers
{
    [HandleError(ExceptionType = typeof(DbUpdateException), View = "Error")]
    [SessionTimeout]
    public class billingController : Controller, IActionFilter, IExceptionFilter
    {

        /// <summary>
        /// Constructor Initialization
        /// </summary>
        private readonly IBillingRepository _billingRepository;
        public billingController()
        {

            this._billingRepository = new BillingRepository(new SodEntities());
        }


        // GET: billing
        public ActionResult blist()
        {
            return View();
        }

        /// <summary>
        /// Get OAT Billing Details
        /// </summary>
        /// <returns></returns>
        public ActionResult oatblist()
        {
            return View();
        }

        /// <summary>
        /// Get Hotel Booking Price List 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetHotelBillingList(string prm)
        {
            var fromdate = prm.Split(',')[0];
            var todate = prm.Split(',')[1];

            var criteria = Convert.ToInt16(prm.Split(',')[2]);
            if (criteria == 1)
                return Json(_billingRepository.GetHotelBillingDetails_ExportToExcel(fromdate, todate, 0, criteria, 0), JsonRequestBehavior.AllowGet);
            else
                return Json(_billingRepository.GetHotelBulkBillingMaster(fromdate, todate, 1, criteria), JsonRequestBehavior.AllowGet);
        }




        /// <summary>
        /// Get HOD/Finance Approval Status for Contractual/Non-Contractual Booking
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetApprovalStatus(string prm)
        {
            var s = _billingRepository.GetHotelApprovalStatus(Convert.ToInt64(prm), 1);
            return Json(s, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Get Hotel Bulk Booking Detail List 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetHotelBulkBillingList(string prm)
        {
            var fromdate = "01/01/1900";
            var todate = "01/01/1900";
            var criteria = Convert.ToInt16("3");
            var trid = Convert.ToInt64(prm);
            var s = _billingRepository.GetHotelBillingDetails_ExportToExcel(fromdate, todate, 0, criteria, trid);
            TempData["bulkList"] = s;
            return Json(s, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// Export data in an Excel Format
        /// </summary>
        /// <param name="strprm"></param>
        public void ExportListFromTsv(string strprm)
        {
            if (strprm.Equals(string.Empty))
            {
                return;
            }

            //Production Environment
            var fdate = strprm.Split(',')[0];
            var tdate = strprm.Split(',')[1];

            var data = _billingRepository.GetHotelBillingDetails_ExportToExcel(fdate, tdate, 1, 1, 0);

            var arr = data.ToArray();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=HotacList.xls");
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            WriteTsv(arr, Response.Output);
            Response.End();
        }


        /// <summary>
        /// Export data in an Excel Format
        /// </summary>
        /// <param name="strprm"></param>
        public void ExportListFromTsvConsolidatedBulkBooking(string strprm)
        {
            if (strprm.Equals(string.Empty))
            {
                return;
            }

            //Production Environment
            var fdate = strprm.Split(',')[0];
            var tdate = strprm.Split(',')[1];

            var data = _billingRepository.GetHotelBillingDetails_ExportToExcel(fdate, tdate, 1, 3, 0);
            var arr = data.ToArray();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=ConsolidatedBulkHotacList.xls");
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            WriteTsv(arr, Response.Output);
            Response.End();
        }


        /// <summary>
        /// Bulk Export data in an Excel Format for Bulk Booking Request
        /// </summary>
        /// <param name="strprm"></param>
        public void ExportBulkListFromTsv(string prm)
        {
            if (prm.Equals(string.Empty))
            {
                return;
            }
            var expList = _billingRepository.GetHotelBillingDetails_ExportToExcel("01/01/1900", "01/01/1900", 0, 3, Convert.ToInt64(prm));
            var arr = expList.ToArray();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=HotacBulkList.xls");
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            WriteTsv(arr, Response.Output);
            Response.End();
        }


        /// <summary>
        /// Method to show output
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="output"></param>
        public void WriteTsv<T>(IEnumerable<T> data, TextWriter output)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));

            foreach (PropertyDescriptor prop in props)
            {
                output.Write(prop.DisplayName); // header
                output.Write("\t");
            }
            output.WriteLine();
            foreach (T item in data)
            {
                foreach (PropertyDescriptor prop in props)
                {
                    output.Write(prop.Converter.ConvertToString(
                         prop.GetValue(item)));
                    output.Write("\t");
                }
                output.WriteLine();
            }
        }

        [HttpGet]
        public JsonResult GetFlightBillingList(string prm)
        {
            var fromdate = prm.Split(',')[0];
            var todate = prm.Split(',')[1];

            var criteria = Convert.ToInt16(prm.Split(',')[2]);
            if (criteria == 1)
                return Json(_billingRepository.GetFlightBillingDetails_ExportToExcel(fromdate, todate, 0, criteria, 0), JsonRequestBehavior.AllowGet);
            else
                return Json(_billingRepository.GetFlightBulkBillingMaster(fromdate, todate, 1, criteria), JsonRequestBehavior.AllowGet);
        }
        #region IExceptionFilter Members
        public void ExportOatListFromTsv(string strprm)
        {
            if (strprm.Equals(string.Empty))
            {
                return;
            }

            //Production Environment
            var fdate = strprm.Split(',')[0];
            var tdate = strprm.Split(',')[1];

            var data = _billingRepository.GetFlightBillingDetails_ExportToExcel(fdate, tdate, 1, 1, 0);

            var arr = data.ToArray();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=Flight.xls");
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            WriteTsv(arr, Response.Output);
            Response.End();
        }
        void IExceptionFilter.OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            Logging.ErrorLog.Instance.AddDBLogging(ex, filterContext.Controller.ControllerContext.ToString(), "Controllers/billing.cs");

        }

        #endregion
    }
}