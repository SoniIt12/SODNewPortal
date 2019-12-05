using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using SOD.CommonWebMethod;
using SOD.EmailNotification;
using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;
using SOD.Services.Repository;


namespace SOD.Controllers
{
    public class SJSisBookingController : Controller, IActionFilter, IExceptionFilter
    {
        private readonly IUserRepository _userRepository;
        private readonly ITransportRepository _transportRepository;
        private readonly ISjSisConcernRepository _sJsisConcernRepository;

        /// <summary>
        /// SJ SisBooking Controller Constructor
        /// </summary>
        public SJSisBookingController()
        {
            _userRepository = new UserRepository(new SodEntities());
            _transportRepository = new TransportRepository(new SodEntities());
            _sJsisConcernRepository = new SjSisConcernRepository(new SodEntities());
        }

        /// <summary>
        ///  GET: SJSisBooking
        /// </summary>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Index()
        {
            if (Session["SjsUserId"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            ManageUserSession();
            return View();
        }

        public ActionResult Flight()
        {
            if (Session["SjsUserId"] == null)
            {
                var eKey = ConfigurationManager.AppSettings["DecryptKey"].Trim();
                //var empid =Cipher.Decrypt(Request.QueryString[0],eKey);
                //Test Env.
                /////////////return RedirectToRoute("SJSisBooking/flight.cshtml");
                var empid = Request.QueryString[0];
                Session["SjsUserId"] = _sJsisConcernRepository.GetUserData("00" + empid);     ////////which table to use to get the data
            }
            if (Session["SjsUserId"].ToString().Trim().Equals("0"))
            {
                var strMsg = "User Mail-Id does not exist.Please contact to HR/Administrator.";
                TempData["ErrorMessage"] = strMsg;
                return RedirectToRoute("Error/Error.cshtml");
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <returns></returns>
        public ActionResult UserProfile()
        {
            return RedirectToAction("ViewProfile");
            //return UserProfile();
        }

        ////sjscuser profile
        //public ActionResult UserProfile()
        //{
        //    return UserProfile();
        // }
        /// <summary>
        /// Reset Password
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetPassword()
        {
            var strQuery = string.Empty;
            TempData["UserId"] = CipherURL.Decrypt(Request.QueryString[0].ToString());
            var ExpiryTime = CipherURL.Decrypt(Request.QueryString[1].ToString());
            if (Convert.ToDateTime(ExpiryTime) < DateTime.Now)
            {
                TempData["jsonmsg"] = "Reset password link has been expired.Please go to Forgot Password link.";
            }
            else
            {
                var IsPwdReset = _sJsisConcernRepository.IsPwdreset(TempData.Peek("UserId").ToString().Trim());
                if (IsPwdReset == "yes")
                {
                    TempData["jsonmsg"] = "Sorry ! You have already reset your password.";
                }
                else
                {
                    TempData["jsonmsg"] = "";
                }
            }
            return View();
        }

        /// <summary>
        /// Verify Email Notifications
        /// </summary>
        /// <returns></returns>
        public ActionResult Verify()
        {
            TempData["jsonmsg"] = verifyEmailNotification(CipherURL.Decrypt(Request.QueryString[0].ToString()));
            return View();
        }

        /// <summary>
        /// profile setting
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewProfile()
        {
            return View();
        }
        /// <summary>
        /// Submit Hotel Info
        /// </summary>
        /// <param name="sodRequestsList"></param>
        /// <param name="sodflightList"></param>
        /// <param name="hotelDetailList"></param>
        /// <returns></returns>
        
        public JsonResult SubmitHotelInfo(List<TravelRequestMasterModels> sodRequestsList, List<FlightDetailModels> sodflightList, List<TravelRequestHotelDetailModels> hotelDetailList, List<HodDetails> HodDetails)
        {
            var s = 0;
            var jsonmsg = string.Empty;
            var BookingType = string.Empty;
            string hodEmailId = HodDetails[0].HodEmail.Trim();
            sodRequestsList[0].IsVendorBooking = false;
            sodRequestsList[0].IsSJSC = true;
            //check duplicate hotel data
            if (sodRequestsList[0].IsHotelRequired == true)
            {
                foreach (var i in hotelDetailList)
                {
                    var empcode = i.EmployeeCode;
                    var checkin = i.CheckInDate;
                    var hotelcity = i.City;

                    var result = _sJsisConcernRepository.FindDuplicateDataHotel(empcode, checkin, hotelcity);
                    if (result == true)
                    {
                        jsonmsg = "Warning: A similar hotel booking already exists. Kindly cancel the previous hotel booking and try again.";
                        TempData["jsonmsg"] = jsonmsg;
                        return Json(jsonmsg, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            //Save master Information, flight information, hotel information
            s = _userRepository.SaveOnlyHotelBookingRequest(sodRequestsList, sodflightList, hotelDetailList);
            //send mail to approver and user
            var hotelList = new List<TravelRequestHotelDetailModels>();
            var dicList = new Dictionary<string, object>();
            dicList = _userRepository.GetSodHotelInfo(Convert.ToInt64(s.ToString()));
            TempData["bookingInfo"] = dicList["bookingInfolist"] as List<TravelRequestMasterModels>;
            TempData["hotelList"] = dicList["hotelinfolist"] as List<TravelRequestHotelDetailModels>;
            TempData["flightinfo"] = dicList["flightInfolist"] as List<FlightDetailModels>;

            //Send Email Notification  
            if (TempData["hotelList"] != null && TempData["bookingInfo"] != null && TempData["flightinfo"] != null)
            {
                var hotel_List = TempData["hotelList"] as List<TravelRequestHotelDetailModels>;
                var bookingInfo = TempData["bookingInfo"] as List<TravelRequestMasterModels>;
                var flightinfo = TempData["flightinfo"] as List<FlightDetailModels>;
                var emailSubject = "SOD-SJSC Hotel Booking Request Notification :" + System.DateTime.Now.ToString();
                var emailTemplateName = "OnlyHotelBookingRequestFor_User.html";

                var controller = DependencyResolver.Current.GetService<HotelOnlyController>();
                var emailCredentials = controller.EmailCredentialsHotel(emailSubject, emailTemplateName, hotel_List, bookingInfo, flightinfo, s.ToString(), "pending");

                TempData["emailData"] = emailCredentials;
                TempData["emailId"] = bookingInfo[0].EmailId.Trim();
                var emaildata = TempData["emailData"] as EmailNotificationModel;
                var emailid = TempData["emailId"].ToString();
                EmailNotifications.SendBookingRequestNotificationTo_Requester(emaildata, emailid);

                //mail to approver
                hodEmailId = HodDetails[0].HodEmail;
                if (bookingInfo[0].EmailId.Trim() == HodDetails[0].HodEmail.Trim())
                {
                    _userRepository.UpdateOnlyHotelApprovalStatus(bookingInfo[0].TravelRequestId);
                    //Redirect to Thank You Page
                    jsonmsg = "SOD-SJSC Hotel Booking Request ID : " + s.ToString() + " ." + "Booking process has been completed successfully.Your booking request has been sent to Traveldesk for Hotel alignment.";
                    TempData["jsonmsg"] = jsonmsg;
                    return Json(jsonmsg, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var emailSubject2 = "SOD-SJSC Hotel Booking Request Notification :" + System.DateTime.Now.ToString();
                    var emailTemplateName2 = "SodHotelBookingRequestFor_HodHotelApproval.html";
                    var emailCredentials2 = controller.EmailCredentialsHotelHod(emailSubject2, emailTemplateName2, hotel_List, bookingInfo, flightinfo, s.ToString(), hodEmailId);
                    TempData["emailData_hod"] = emailCredentials2;
                    TempData["emailId_hod"] = HodDetails[0].HodEmail.Trim();

                    var templateData = emailCredentials2.TemplateFilePath;
                    var appLink = string.Empty;
                    var approvaltype = string.Empty;
                    if (hodEmailId.Length > 0)
                    {
                        var skey = new StringBuilder();
                        skey.Append(bookingInfo[0].TravelRequestId.ToString() + ",");
                        skey.Append(bookingInfo[0].EmailId.Trim() + ",");
                        skey.Append(bookingInfo[0].SodBookingTypeId.ToString() + ",");
                        skey.Append(bookingInfo[0].BookingFor.Trim() + ",");
                        skey.Append(hodEmailId.ToString().Trim() + ",");
                        skey.Append(0);

                        var uri1 = ConfigurationManager.AppSettings["emailOnlyHotelApprovalPathHod"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=a");
                        var uri2 = ConfigurationManager.AppSettings["emailOnlyHotelApprovalPathHod"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=r");

                        approvaltype = "Please help to accord your Acceptance or Rejection.";
                        appLink = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a name='app' style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Acceptance</a></td> <td>&nbsp;</td> <td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a name='rej' style='color:#fff; text-decoration:none;' href='" + uri2 + "'>Rejection</a> </td></tr></table>";

                        templateData = templateData.Replace("[approvaltype]", approvaltype);
                        templateData = templateData.Replace("[appLink]", appLink);
                        templateData = templateData.Replace("[hodName]", HodDetails[0].HoDName.Trim() + ".");
                        templateData = templateData.Replace("[RequesterName]", bookingInfo[0].Title + " " + bookingInfo[0].RequestedEmpName);
                        emailCredentials2.TemplateFilePath = templateData;

                        var emaildata2 = TempData["emailData_hod"] as EmailNotificationModel;
                        var emailid2 = TempData["emailId_hod"].ToString();
                        EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials2, emailid2);
                        //SMS Approval
                        //SendSMSforApproval(HodDetails, bookingInfo[0], hotel_List, uri1, uri2);
                    }
                    //Redirect to Thank You Page
                    jsonmsg = "SOD-SJSC Hotel Booking Request ID : " + s.ToString() + " ." + "Booking process has been completed successfully.Your booking request has been sent to your respected approver at  " + hodEmailId.Split(',')[0].ToString();
                    TempData["jsonmsg"] = jsonmsg;
                    return Json(jsonmsg, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                jsonmsg = "Error: Hotel information cannot be saved. Please try again later.";
                TempData["jsonmsg"] = jsonmsg;
                return Json(jsonmsg, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// User Registration
        /// </summary>
        /// <param name="UserData"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UserRegister(SJSCUserMasterModels UserData)
        {
            if (UserData.EmpCode == null)
            {
                Random generator = new Random();
                String r = generator.Next(0, 999999).ToString("D6");
                UserData.EmpCode = r;
            }
            Int64 IsExist = _sJsisConcernRepository.checkUserIdWithEmpCode(UserData.EmailID, UserData.EmpCode);
            if (IsExist == 0)
            {
                Int64 s = _sJsisConcernRepository.registerUser(UserData);
                Session["SjsUserId"] = UserData.EmailID.Trim();
                Session["SjsPwd"] = UserData.Pwd.Trim();
                var emailSubject = "SpiceJet SOD-SJSC User Registration Confirmation Notification:" + System.DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss tt");
                var emailTemplateName = "SJSCUserRegistrationConfirmation.html";
                var emailCredentials = EmailCredentials(emailSubject, emailTemplateName, UserData);
                var SJSCRegistrationVerificationLink = ConfigurationManager.AppSettings["SJSCRegistrationVerification"].Trim();
                var uri = SJSCRegistrationVerificationLink + "?str=" + CipherURL.Encrypt(UserData.EmailID.Trim());

                var templateData = emailCredentials.TemplateFilePath;
                templateData = templateData.Replace("[hrefLink]", uri);
                emailCredentials.TemplateFilePath = templateData;
                TempData["emailData"] = emailCredentials;
                TempData["emailId"] = UserData.EmailID.Trim();
                return Json(s > 0 ? "Your registration process has been completed successfully. Please Activate your account via clicking on the link sent to your registered Email Id and login." : "Oops! something went wrong", JsonRequestBehavior.AllowGet);
            }
            else
            {
                var msg = "This Employee Code :" + UserData.EmpCode + " or Employee Email ID : " + UserData.EmailID + " has already been registered.";
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Validate User-Id for Forget Password
        /// </summary>
        /// <param name="UserData"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult validateUserIdforForgetPassword(SJSCUserMasterModels UserData)
        {
            Int64 IsExist = _sJsisConcernRepository.checkUserId(UserData);
            if (IsExist != 0)
            {
                try
                {
                    var regUserData = _sJsisConcernRepository.GetUserData(UserData.EmailID);
                    _sJsisConcernRepository.modifyPwd(UserData.EmailID);
                    UserData.FirstName = regUserData.FirstName;
                    UserData.LastName = regUserData.LastName;
                    var ExpDate = DateTime.Now.AddMinutes(720);
                    var emailSubject = "SOD-SJSC Account Recovery Notification:" + System.DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss tt");
                    var emailTemplateName = "SJSCUserForgotPassword.html";
                    var emailCredentials = EmailCredentials(emailSubject, emailTemplateName, UserData);
                    var SJSCResetPasswordLink = ConfigurationManager.AppSettings["SJSCResetPassword"].Trim();
                    var uri = SJSCResetPasswordLink + "?str =" + CipherURL.Encrypt(UserData.EmailID.Trim()) + "&expdate=" + CipherURL.Encrypt(Convert.ToString(ExpDate));
                    var appLink = "<table><tr style='font-family:Arial;'><td style='padding:10px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a style='color:#fff; text-decoration:none;' href='" + uri + "'>Reset Password</a></td></tr></table>";
                    var templateData = emailCredentials.TemplateFilePath;
                    templateData = templateData.Replace("[hrefLink]", appLink);
                    emailCredentials.TemplateFilePath = templateData;
                    TempData["emailData"] = emailCredentials;
                    TempData["emailId"] = UserData.EmailID.Trim();
                }
                catch (Exception ex)
                {
                    return Json("Error", JsonRequestBehavior.AllowGet);
                }
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Invalid", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="UserData"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateUserInfo(SJSCUserMasterModels UserData)
        {
            Int64 s = _sJsisConcernRepository.UpdateUserInfo(UserData);
            return Json(s > 0 ? "Your User Profile has been updated succesfully." : "Oops! something went wrong", JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Check Current Password
        /// </summary>
        /// <param name="UserData"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult checkCurrentPassword(PasswordModal UserData)
        {
            string userID = Session["SjsUserId"].ToString().Trim();
            string IsExist = _sJsisConcernRepository.ValidatePwd(UserData.OldPwd, userID);
            try
            {
                if (IsExist == "Invalid")
                {
                    return Json("Invalid", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("valid", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Update Password Info
        /// </summary>
        /// <param name="UserData"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdatePasswordInfo(PasswordModal UserData)
        {
            string userID = string.Empty;
            if (Session["SjsUserId"] == null)
            {
                userID = TempData["UserId"].ToString().Trim();
            }
            else
            {
                userID = Session["SjsUserId"].ToString().Trim();
            }

            try
            {
                Int64 s = _sJsisConcernRepository.UpdatePassword(UserData, userID);
                return Json(s > 0 ? "Your Password has changed successfully. " : "Oops! something went wrong", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Update Password Info
        /// </summary>
        /// <param name="UserData"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ResetPassword(PasswordModal UserData)
        {
            string userID = string.Empty;
            if (Session["SjsUserId"] == null)
                userID = TempData["UserId"].ToString().Trim();
            else
                userID = Session["SjsUserId"].ToString().Trim();

            Int64 s = _sJsisConcernRepository.ResetPassword(UserData, userID);
            return Json(s > 0 ? "Your Password has been changed successfully." : "Password is already Reset from this Request", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Login REgistration
        /// </summary>
        /// <param name="UserData"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Loginregister(SJSCUserMasterModels UserData)
        {
            string IsExist = _sJsisConcernRepository.validateUserIdAndPwd(UserData.UserId, UserData.Pwd);
            try
            {
                if (IsExist == "Not varified")
                    return Json("Not varified", JsonRequestBehavior.AllowGet);
                else if (IsExist == "Invalid")
                    return Json("Invalid", JsonRequestBehavior.AllowGet);
                var regUserData = _sJsisConcernRepository.GetUserData(UserData.UserId);
                Session["SjsUserId"] = UserData.UserId.Trim();
                Session["SjsPwd"] = UserData.Pwd.Trim();
                Session["EmpCode"] = regUserData.EmpCode.Trim();
                Session["EmpId"] = 0;
                Session["HodEmailId"] = regUserData.HodEmailId.Trim();
                Session["FirstName"] = regUserData.FirstName;
                Session["LastName"] = regUserData.LastName;
                Session["Email"] = regUserData.EmailID;
                Session["Phone"] = regUserData.MobileNo;
                Session["Gender"] = regUserData.Title;
                Session["Designation"] = regUserData.Designation;
                Session["DeptIdCR"] = regUserData.Department;
                Session["Department"] = regUserData.Department;
                Session["Isapprover"] = regUserData.IsApprover;
                Session["HodName"] = regUserData.HodName.Trim();
                Session["HodTitle"] = regUserData.HodTitle.Trim();
                Session["HodMobileNo"] = regUserData.HodMobileNo == null ? "" : regUserData.HodMobileNo;
                Session["IsApprover"] = regUserData.IsApprover;
                if (regUserData.EmailID == regUserData.HodEmailId)
                {
                    Session["Role"] = 11;
                    Session["usertype"] = 1;
                }
                else
                {
                    Session["Role"] = 10;
                }
                return Json("Valid", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get Registered User Data
        /// </summary>
        /// <returns></returns>
        public ActionResult GetRegUserData()
        {
            var regUserData = new SJSCUserMasterModels();
            if (Session["SjsUserId"] != null)
            {
                string userId = Session["SjsUserId"].ToString();
                regUserData = _sJsisConcernRepository.GetUserData(userId);
                TempData["UserInfo"] = regUserData;
                return Json(regUserData, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        /// <summary>
        /// Get Employee Verticals details
        /// </summary>
        /// <returns></returns>
        public JsonResult GetVerticals()
        {
            var verticalList = new List<SJSCVerticalMasterModels>();
            verticalList = _sJsisConcernRepository.GetVerticals();
            return Json(verticalList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Verify Email Notifications
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string verifyEmailNotification(string userId)
        {
            var verifyId = _sJsisConcernRepository.verifyUserID(userId);
            string msg = string.Empty;
            if (verifyId == -1)
            {
                TempData["Img"] = "right";
                msg = "Your email address " + userId + " is already confirmed successfully.";
                return msg;
            }
            else if (verifyId > 0)
            {
                TempData["Img"] = "right";
                msg = "Your email address " + userId + " is confirmed successfully.";
                return msg;
            }
            else
            {
                TempData["Img"] = "Rejected";
                return "Sorry! Invalid Procesing request";
            }
        }

        /// <summary>
        /// Initialized Email Credentials to Property
        /// </summary>
        /// <returns></returns>
        public EmailNotificationModel EmailCredentials(string subjectName, string emailTemplateName, SJSCUserMasterModels UserData)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtpUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtpPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtpServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtpPort"].Trim(),
                TemplateFilePath = ReadFile(emailTemplateName, UserData),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }

        /// <summary>
        /// Read File Method
        /// </summary>
        /// <param name="emailTemplateName"></param>
        /// <param name="UserData"></param>
        /// <returns></returns>
        private string ReadFile(string emailTemplateName, SJSCUserMasterModels UserData)
        {
            var strContent = new StringBuilder();
            string line;
            var file = new System.IO.StreamReader(Server.MapPath("~/Template/SJSC/" + emailTemplateName));
            while ((line = file.ReadLine()) != null)
            {
                strContent.Append(line);
            }
            file.Close();
            var fullName = UserData.Title + " " + UserData.FirstName + " " + UserData.LastName;
            strContent = strContent.Replace("[userName]", fullName);
            strContent = strContent.Replace("[UserId]", UserData.EmailID);
            strContent = strContent.Replace("[PhNo]", UserData.MobileNo);
            return strContent.ToString();
        }

        /// <summary>
        /// Send SMS for Approval
        /// </summary>
        /// <param name="reqId"></param>
        /// <param name="paxName"></param>
        /// <param name="approvalURI"></param>
        public void SendSMSforApproval(List<HodDetails> hoddetails, TravelRequestMasterModels sodRequest, List<TravelRequestHotelDetailModels> hotelInfo, string appURI, string rejURI)
        {
            var smsText = ConfigurationManager.AppSettings["smsApprovalOnlyHotel"].ToString().Replace("@Hodname", hoddetails[0].HoDName);
            smsText = smsText.Replace("@PaxName", sodRequest.Title + " " + sodRequest.RequestedEmpName);
            smsText = smsText.Replace("@Sector", hotelInfo[0].City);
            smsText = smsText.Replace("@CheckInDate", hotelInfo[0].CheckInDate.ToString("dd/MMM/yyyy"));
            smsText = smsText.Replace("@CheckOutDate", hotelInfo[0].CheckOutDate.ToString("dd/MMM/yyyy"));
            smsText = smsText.Replace("@ReqId", sodRequest.TravelRequestId.ToString());
            smsText = smsText.Replace("@BookigFor", "Only Hotel booking");
            smsText = smsText.Replace("[AppQueryString]", appURI);
            smsText = smsText.Replace("[RejQueryString]", rejURI);
            smsText = System.Uri.EscapeDataString(smsText);
            smsText = smsText.Replace("25", "");
            //Send SMS
            var smsLogModel = new SodApproverSMSLogModels();
            smsLogModel.TrRequestId = sodRequest.TravelRequestId;
            smsLogModel.EmpCode = "";
            smsLogModel.EmpName = hoddetails[0].HoDName;
            smsLogModel.EmailID = hoddetails[0].HodEmail;
            smsLogModel.MobileNo = hoddetails[0].HodMobileNo; 
            smsLogModel.Source = "SJSC";
            smsLogModel.SMSText = smsText;
            smsLogModel.DeliveryDate = DateTime.Now;
            var status = SmsNotification.SmsNotifications.SendSmsViaApi(smsText, hoddetails[0].HodMobileNo); 
            smsLogModel.IsDelivered = status.Equals(true) ? true : false;
            _userRepository.SaveApproverSMSLog(smsLogModel);
        }

        /// <summary>
        /// Action Filter Method
        /// </summary>
        /// <param name="filterContext"></param>
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["SjsUserId"] == null)
            {
                Response.Clear();
                ViewBag.Message = String.Format("Sorry ! Your session has been expired.Please login again.");
                CloseBookingList();
            }
        }

        ///// <summary>
        ///// Manage User Session
        ///// </summary>
        ///// <returns></returns>
        public ActionResult CloseBookingList()
        {
            ViewBag.Message = String.Format("Sorry ! Your session has been expired.Please login again.");
            return View("Login");

        }

        /// <summary>
        /// Exception Handling
        /// </summary>
        /// <param name="filterContext"></param>
        private void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            Logging.ErrorLog.Instance.AddDBLogging(ex, filterContext.Controller.ControllerContext.ToString(), "Controllers/SJSisBooking.cs");
        }

        /// <summary>
        /// Log out
        /// </summary>
        public void ManageUserSession()
        {
            Response.Clear();
            Session.Abandon();
        }
    }

}