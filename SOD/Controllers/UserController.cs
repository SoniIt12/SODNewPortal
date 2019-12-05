using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;
using SOD.Services.Repository;
using SOD.CommonWebMethod;
using System.Configuration;
using SOD.Logging;
using System.Web.UI;
using System.Data.Entity.Infrastructure;


namespace SOD.Controllers
{
    [HandleError(ExceptionType = typeof(DbUpdateException), View = "Error")]
    public class UserController : Controller, IActionFilter, IExceptionFilter
    {
        /// <summary>
        /// Constructor Initialization
        /// </summary>
        private readonly IUserRepository _userRepository;

        public UserController()
        { 
            _userRepository = new UserRepository(new SodEntities());
        }


        /// <summary>
        /// Travel Request View
        /// </summary>
        /// <returns></returns>
        //[HttpPost]
        //[OutputCache(CacheProfile="csatyam")]
        public ActionResult SearchFlight()
        {
            if (Session["EmpId"] == null)
            {
                //var eKey = ConfigurationManager.AppSettings["DecryptKey"].Trim();
                //var empid =Cipher.Decrypt(Request.QueryString[0],eKey);
                //Test Env.
                var empid = Request.QueryString[0];
                Session["EmpId"] = _userRepository.GetLoginEmployeeID("00" + empid);
            }
            if (Session["EmpId"].ToString().Trim().Equals("0"))
            {
                var strMsg = "Employee Id does not exist.Please contact to HR/Administrator.";
                TempData["ErrorMessage"] = strMsg;
                return RedirectToRoute("Error/Index");
            }
            else
            {
                LoginUserInfo(Session["EmpId"].ToString());
                return View();
            }
        }

        /// <summary>
        /// Sod Request Submit
        /// </summary>
        /// <param name="sodRequestsList"></param>
        /// <returns></returns>
        //[OutputCache(CacheProfile = "csatyam")]
        [HttpPost]
        public JsonResult SodRequestSubmit(List<TravelRequestModels> sodRequestsList)
        {
            TempData["SodbookingRequest"] = sodRequestsList;
            TempData["TravelRequestTypeId"] = sodRequestsList[0].TravelRequestTypeId;
            TempData["Passengers"] = sodRequestsList[0].Passengers;
            TempData["SodBookingType"] = sodRequestsList[0].SodBookingType;
            TempData["BookingFor"] = sodRequestsList[0].BookingFor;
            //spliting the destination place and getting the full destination name
            TempData["DestinationName"] = sodRequestsList[0].DestinationPlace.Split('-')[0];
            TempData["RequestListCount"] = sodRequestsList.Count;

            if (sodRequestsList[0].ReturnDate != null)
            {
                TempData["ReturnDateRoundTrip"] = sodRequestsList[0].ReturnDate.ToString();
            }
            var destList = new List<String>();
            //spliting the destination name and keeping the destination code in the sodRequestsList object.
            for (int i = 0; i < sodRequestsList.Count; i++)
            {
                destList.Add(sodRequestsList[i].DestinationPlace.Split('-')[0]);
            }
            TempData["DestinationList"] = destList;

            var s = "";
            switch (sodRequestsList[0].TravelRequestTypeId)
            {
                case 1:
                    s = GetTravelResposefromNavitaire();
                    ViewBag.OriginPlace = "Departure : " + sodRequestsList[0].OriginPlace + "-" + sodRequestsList[0].DestinationPlace;
                    break;
                case 2:
                    s = GetTravelResposefromNavitaire();
                    ViewBag.OriginPlace = "Departure : " + sodRequestsList[0].OriginPlace + "-" + sodRequestsList[0].DestinationPlace;
                    ViewBag.ReturnPlace = "Return :" + sodRequestsList[0].DestinationPlace + "-" + sodRequestsList[0].OriginPlace;
                    break;
                case 3:
                    s = GetTravelResposefromNavitaire();
                    break;
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Get Travel Response from Navitair
        /// </summary>
        /// <returns></returns>
        //[OutputCache(CacheProfile = "csatyam")]
        public string GetTravelResposefromNavitaire()
        {
            var lst = TempData["SodbookingRequest"] as List<TravelRequestModels>;
            var s = "";
            switch (lst[0].TravelRequestTypeId)
            {
                case 1:
                case 2:
                    s = NavitaireServices.GetAvailability1(lst[0].OriginPlace, lst[0].DestinationPlace, lst[0].TravelDate,
                   lst[0].ReturnDate, lst[0].TravelRequestTypeId.ToString(), lst[0].Passengers, lst[0].BookingFor);
                    break;
                case 3:
                    s = NavitaireServices.GetAvailabilityMulticity(lst);
                    break;
            }
            return s;
        }



        /// <summary>
        /// Get Original Station
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        //[OutputCache(Duration =43200,Location=OutputCacheLocation.Server,VaryByParam = "none")]
        public JsonResult GetOriginStation()
        {
            var o = CommonWebMethod.CommonWebMethods.GetOriginStation();
            return Json(o, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Get Destination Place
        /// </summary>
        /// <param name="objArr"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetArrivalStation(string objArr)
        {
            var a = CommonWebMethod.CommonWebMethods.GetArrivalStation(objArr);
            return Json(a, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Manage Login User Info
        /// </summary>
        /// <param name="empId"></param>
        public void LoginUserInfo(string empId)
       {
            //Get User Role Info
            var s = _userRepository.GetEmployeeList(int.Parse(empId));
            if (s.Count.Equals(0)) return;
            var r = _userRepository.IsSodApproverHodRole(Convert.ToInt32(s[0].EmpId.ToString().Trim()));
            //Check CXO Level of Approvals
            var cxo = _userRepository.IsCXOApproverRole(Convert.ToInt32(s[0].EmpId.ToString().Trim()));

            Session["UserInfo"] = "Welcome : " + s[0].EmpName + " | " + s[0].Designation + " | " + s[0].Department;
            Session["EmpCode"] = s[0].EmpCode.Trim();
            var names = s[0].EmpName.Split(' ');
            Session["FirstName"] = names[0];
            var lastname = s[0].EmpName.Replace(names[0] + ' ', "");
            Session["LastName"] = lastname;
            Session["Email"] = s[0].Email.Trim();
            Session["Phone"] = s[0].Phone.Trim();
            Session["Gender"] = s[0].Gender.Trim();
            Session["Designation"] = s[0].Designation;
            Session["DeptIdCR"] = s[0].DepartmentId;
            Session["DeptCR"] = s[0].DepartmentId;
            Session["DesigIdM"] = s[0].DesignationId;
            Session["VerticalId"] = s[0].EmployeeVertical;
            Session["Department"] = s[0].Department;
            if (r)
            {
                TempData["approverInfo"] = _userRepository.GetApproverDept_Desig(Convert.ToInt32(s[0].EmpId.ToString().Trim()));
                ViewBag.DeptId = TempData["approverInfo"].ToString().Split(',')[0];
                ViewBag.DesigId = TempData["approverInfo"].ToString().Split(',')[1];
                Session["usertype"] = "1";
            }
            else
            {
                Session["usertype"] = "0";
            }
            if (cxo)
            {
                Session["cxo"] = "1";//CXO Level Approval
                Session["usertype"] = "1";
                ViewBag.DeptId = s[0].DepartmentId;
                ViewBag.DesigId = s[0].DesignationId;
            }
            else
                Session["cxo"] = "0";
            //bind user role
            Session["Role"] = _userRepository.GetRoleOFEmp(Session["EmpCode"].ToString());
            //Check Bulk Booking Rights
            string[] bulkArray = ConfigurationManager.AppSettings["Bulk_Booking_AllowedId"].Split(',');
            int index = Array.IndexOf(bulkArray, empId);
            @ViewBag.blkRight = index > -1 ? 1 : 0;
            TempData["blkRight"] = index > -1 ? 1 : 0;
        }


        /// <summary>
        /// Action Filter Method
        /// </summary>
        /// <param name="filterContext"></param>
        void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (Session["EmpId"] == null || Session["SjsUserId"]==null)
            {
                Response.Clear();
                CloseBookingList();
            }
        }


        /// <summary>
        /// On Action Executed
        /// </summary>
        /// <param name="filterContext"></param>
        //void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    if (Session["EmpId"] == null)
        //    {
        //        Response.Clear();
        //        CloseBookingList();
        //    }
        //}


        /// <summary>
        /// Manage User Session
        /// </summary>
        /// <returns></returns>
        public ActionResult CloseBookingList()
        {
            ViewBag.Message = String.Format("Sorry ! Your session has been expired.Please click on the sod link again.");
            return View();
        }



        /// <summary>
        /// Exception Handling
        /// </summary>
        /// <param name="filterContext"></param>
        void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            Logging.ErrorLog.Instance.AddDBLogging(ex, filterContext.Controller.ControllerContext.ToString(), "Controllers/UserController.cs");
        }
    }
}
