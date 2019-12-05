using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;
using SOD.Services.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SOD.Controllers
{
    [HandleError(ExceptionType = typeof(DbUpdateException), View = "Error")]
    public class hdController : Controller,IActionFilter,IExceptionFilter
    {        
        /// <summary>
        /// Constructor Initialization
        /// </summary>
        private readonly IHdRepository  _hdRepositorty;
        public hdController()
        {
            _hdRepositorty = new HdRepository(new SodEntities());
        }    
        // GET: hd
        public ActionResult bklist()
        {
            return View();
        }
        // GET: Download Excel List
        public ActionResult dwnlist()
        {
            return View();
        }
        // GET: Download Excel List
        public ActionResult bulkbList()
        {
            return View();
        }
        // GET: Get Employee Data
        public ActionResult employeeDetails()
        {
            return View();
        }
        /// <summary>
        /// Get Employee History
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetEmployeeBookingHistory(int EmpId)
        {
            var s=Json(_hdRepositorty.GetSodEmployeeBookingHistoryList_Helpdesk(0, EmpId, 3), JsonRequestBehavior.AllowGet);
            s.MaxJsonLength = int.MaxValue;
            return s;
        }
        /// <summary>
        /// Get All Employee Data
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetEmployeeViewDetails(string EmpId)
        {
            var s = Json(_hdRepositorty.GetSodEmployeeViewDetails(EmpId, 3), JsonRequestBehavior.AllowGet);
            s.MaxJsonLength = int.MaxValue;
            return s;
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
            var type =Convert.ToInt16(strprm.Split(',')[2]);
            var ctrl = Convert.ToInt16(strprm.Split(',')[3]);
            var data = _hdRepositorty.GetSodEmployeeBookingHistoryList_Helpdesk_ExcelExport(fdate, tdate, type,"","", ctrl);
            var sodRequestsList = data["bookingList"] as List<ExcelExportModel>;
            var arr = sodRequestsList.ToArray();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=PNRList.xls");
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
        /// <summary>
        /// On OnActionExecuting
        /// </summary>
        /// <param name="filterContext"></param>
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["EmpId"] == null)
            {
                Response.Clear();
                Response.Redirect("../Login/UserAccount");
            }
        }
        /// <summary>
        /// Exception Handling
        /// </summary>
        /// <param name="filterContext"></param>
        void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            Logging.ErrorLog.Instance.AddDBLogging(ex, filterContext.Controller.ControllerContext.ToString(), "Controllers/hdController.cs");
        }
    }
}