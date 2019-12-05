using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SOD.Model;
using SOD.Services.Interface;
using SOD.Services.Repository;
using SOD.Services.EntityFramework;
using SOD.CommonWebMethod;

namespace SOD.Controllers
{
    public class LoginController : Controller, IExceptionFilter
    {

        #region Constructor
        /// <summary>
        /// Constructor Initilization
        /// </summary>
        private readonly IUserAccountRepository _userAccountRepository;
        public LoginController()
        {
            this._userAccountRepository = new UserAccountRepository(new SodEntities());
        }

        #endregion

        #region "View"
        /// <summary>
        /// Open Login View
        /// </summary>
        /// <returns></returns>
        public ActionResult UserAccount()
        {
            ManageUserSession();
            return View();
        }


        /// <summary>
        /// Change Password View
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePassword()
        {
            return View(TempData["user"]);
        }

        #endregion

        #region "Method"
        /// <summary>
        /// Validate Login credentials
        /// </summary>
        /// <param name="form">using formcollection data</param>
        /// <returns>Login View</returns>
        [HttpPost]
        public ActionResult ValidateLogin(FormCollection form)
        {
            var userAccountModel = new UserAccountModels
            {
                UserName = form["username"],
                Password = form["password"]
            };
            var userList = _userAccountRepository.GetLoginUserList(userAccountModel);
            if (userList.UserName != null)
            {
                //Store Login Data in Session
                Session["user"] = userAccountModel.UserName.Trim().ToUpper();
                Session["DesignationId"] = userList.DesignationId;
                Session["DepartmentId"] = userList.DepartmentId;
                Session["EmpId"] = userList.EmpCode;

                if (form["username"].Equals("admin"))
                {
                    Session["formtype"] = "Admin";
                    return RedirectToAction("Index", "Admin");
                }
                else if (userList.DepartmentId.Equals(108))
                {
                    Session["formtype"] = "Revenue";
                    return RedirectToAction("BookingList", "Revenue");
                }
                else if (userList.DepartmentId.Equals(109))
                {
                    Session["formtype"] = "Helpdesk";
                    return RedirectToAction("bklist", "hd");
                }
                else if (userList.DepartmentId.Equals(110))
                {
                    Session["formtype"] = "Traveldesk";
                    return RedirectToAction("hotelList", "trns");
                }
                else if (userList.DepartmentId.Equals(111))
                {
                    Session["formtype"] = "Billingdesk";
                    return RedirectToAction("blist", "billing");
                }
                else if (userList.DepartmentId.Equals(112))
                {
                    Session["formtype"] = "Financedesk";
                    return RedirectToAction("finlist", "finance");
                }
                else if (userList.DepartmentId.Equals(113))
                {
                    Session["formtype"] = "TraveldeskAdmin";
                    return RedirectToAction("hotelList", "trns");
                }
                else if (userList.DepartmentId.Equals(114))
                {
                    Session["formtype"] = "OATTraveldesk";
                    return RedirectToAction("flist", "Oat");
                }
                else if (userList.DepartmentId.Equals(115))
                {
                    Session["formtype"] = "OATBillingdesk";
                    return RedirectToAction("oatblist", "billing");
                }
                else
                {
                    Session["formtype"] = "User";
                    return RedirectToAction("SearchFlight", "User");
                }
            }
            else
            {
                ViewBag.LoginFailMessage = "Invalid Login-id or Password.";
                return View("UserAccount");
            }
        }


        /// <summary>
        /// Need to implement if required : Validate Login Js
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string ValidateLogins(string username, string pwd)
        {
            //If Required need to implement
            var s = 1;// _adminRepository.SaveSodBlanketApprover(blanketApproval);
            return s >= 1 ? "Save successfully" : string.Empty;
        }


        /// <summary>
        /// Update or Reset Password
        /// </summary>
        /// <param name="frm"></param>
        [HttpPost]
        public ActionResult UpdatePassword(FormCollection frm)
        {
            var userAccountModel = new UserAccountModels
            {
                UserName = Session["user"].ToString().Trim(),
                Password = frm["txtcfpwd"],
                OldPassword = frm["txtoldpwd"]
            };
            int? s = _userAccountRepository.ResetPassword(userAccountModel);
            if (s > 0)
            {
                return RedirectToAction("UserAccount");
            }
            else
            {
                ViewBag.LoginFailMessage = "Error : Invalid Reset Password.";
                return View("ChangePassword");
            }
        }


        /// <summary>
        /// Manage All User old Session
        /// </summary>
        void ManageUserSession()
        {
            Session.RemoveAll();
            Session.Abandon();
        }


        /// <summary>
        /// Exception Handling
        /// </summary>
        /// <param name="filterContext"></param>
        void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            Logging.ErrorLog.Instance.AddDBLogging(ex, filterContext.Controller.ControllerContext.ToString(), "Controllers/LoginController.cs");
        }
        #endregion
    }
}
