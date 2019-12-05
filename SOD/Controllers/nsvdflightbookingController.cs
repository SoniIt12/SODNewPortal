using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SOD.Services.Interface;
using SOD.Services.Repository;
using System.Data.Entity.Infrastructure;
using SOD.Model;
using SOD.Services.EntityFramework;
using System.Configuration;

namespace SOD.Controllers
{
    public class nsvdflightbookingController : Controller
    {
        private IVendorApprovalRepository _vendorApprovalRepository;
        public nsvdflightbookingController()
        {
            _vendorApprovalRepository = new VendorApprovalRepository(new SodEntities());
        }
        // GET: nsvdflightbooking
        public ActionResult searchflight()
        {
            string[] bulkArray = ConfigurationManager.AppSettings["Bulk_Booking_AllowedId"].Split(',');
            int index = Array.IndexOf(bulkArray, Session["EmpId"].ToString());
            @ViewBag.blkRight = index > -1 ? 1 : 0;
            ViewBag.DeptId = Session["DepartmentId"];
            ViewBag.DesigId = Session["DesignationId"];
            return View();
        }
        public JsonResult GetApprovedList()
        {
            var ReqEmpCode = Session["EmpCode"].ToString();
            var data = _vendorApprovalRepository.GetApprovedList(ReqEmpCode);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
