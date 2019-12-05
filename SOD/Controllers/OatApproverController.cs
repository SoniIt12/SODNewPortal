using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;
using SOD.Services.Repository;
using SOD.Model;
using SOD.CommonWebMethod;
using SOD.EmailNotification;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Controllers
{
    public class OatApproverController : Controller, IActionFilter, IExceptionFilter
    {
        private readonly IOatApproverRepository _oatApproverRepository;

        public OatApproverController()
        {
            _oatApproverRepository = new OatApproverRepository(new SodEntities());
        }


        // GET: OatApprover
        public ActionResult GetOatApproverList()
        { 
            if (Request.QueryString["dep"] != null && Request.QueryString["deg"] != null)
            {
                var deptartmentId = Convert.ToInt32(Request.QueryString["dep"].ToString().Trim());
                var designationId = Convert.ToInt32(Request.QueryString["deg"].ToString().Trim());
                OatBookingList(deptartmentId, designationId);
            }
            return View();
        }


        /// <summary>
        /// Get Sod booking List 
        /// </summary>
        private void OatBookingList(int deptartmentId, int designationId)
        {
            var s = _oatApproverRepository.GetOatBookingListForApproval(deptartmentId, designationId, Convert.ToInt32(Session["EmpId"].ToString()), 1);
            TempData["ApproverList"] = s;
        }

        /// <summary>
        /// Get Employee Designation
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetBookingData()
        {
            return TempData["ApproverList"] != null
                ? Json(TempData["ApproverList"], JsonRequestBehavior.AllowGet)
                : Json("0", JsonRequestBehavior.AllowGet);
        }
    }
}