using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Mvc;
using SOD.CommonWebMethod;
using SOD.EmailNotification;
using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;
using SOD.Services.Repository;
using System.Data.Entity.Infrastructure;
using System.IO;

namespace SOD.Controllers
{
    [HandleError(ExceptionType = typeof(DbUpdateException), View = "Error")]
    public class HotelOnlyController : Controller, IActionFilter, IExceptionFilter
    {
        private readonly IUserRepository _userRepository;
        private readonly ITransportRepository _transportRepository;

        /// <summary>
        /// Constructor 
        /// </summary>
        public HotelOnlyController()
        {
            _userRepository = new UserRepository(new SodEntities());
            _transportRepository = new TransportRepository(new SodEntities());
        }

        // GET: Index page
        public ActionResult HotelIndex(string message)
        {
            return View("HotelIndex");
        }

        public ActionResult BookingResponse()
        {
            return View("BookingResponse");
        }


        /// <summary>
        /// Submit Hotel Info
        /// </summary>
        /// <param name="sodRequestsList"></param>
        /// <param name="sodflightList"></param>
        /// <param name="hotelDetailList"></param>
        /// <returns></returns>
        public JsonResult SubmitHotelInfo(List<TravelRequestMasterModels> sodRequestsList, List<FlightDetailModels> sodflightList, List<TravelRequestHotelDetailModels> hotelDetailList)
        {
            var s = 0;
            var jsonmsg = string.Empty;
            var BookingType = string.Empty;
            string hodEmailId = "";
            sodRequestsList[0].IsVendorBooking = false;
            sodRequestsList[0].IsSJSC = false;
            //check duplicate hotel data
            if (sodRequestsList[0].IsHotelRequired == true)
            {
                foreach (var i in hotelDetailList)
                {
                    var empcode = i.EmployeeCode;
                    var checkin = i.CheckInDate;
                    var hotelcity = i.City;

                    var result = _transportRepository.FindDuplicateDataHotel(empcode, checkin, hotelcity);
                    if (result == true)
                    {
                        jsonmsg = "Warning: A similar hotel booking already exists. Kindly cancel the previous hotel booking and try again.";
                        TempData["jsonmsg"] = jsonmsg;
                        return Json(jsonmsg, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            GetLoginUserInfo(sodRequestsList[0].RequestedEmpId.ToString());
            var empInfo = TempData["EmpInfo"] as List<string>;
            hodEmailId = _userRepository.GetHODEmailId(empInfo[8]);
            var r = _userRepository.IsBlanketApproverRole(Convert.ToInt32(empInfo[0]));
            sodRequestsList[0].SJSCHodEmailId = hodEmailId.Split(',')[0].ToString().Trim();
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
                var emailSubject = "SOD Only-Hotel Booking Request Notification :" + System.DateTime.Now.ToString();
                var emailTemplateName = "OnlyHotelBookingRequestFor_User.html";
                var emailCredentials = EmailCredentialsHotel(emailSubject, emailTemplateName, hotel_List, bookingInfo, flightinfo, s.ToString(), "pending");
                TempData["emailData"] = emailCredentials;
                TempData["emailId"] = bookingInfo[0].EmailId.Trim();
                var emaildata = TempData["emailData"] as EmailNotificationModel;
                var emailid = TempData["emailId"].ToString();
                EmailNotifications.SendBookingRequestNotificationTo_Requester(emaildata, emailid);

                if (bookingInfo[0].RequestedEmpId.ToString() == hodEmailId.Split(',')[2].ToString().Trim() || r == true)
                {
                    _userRepository.UpdateOnlyHotelApprovalStatus(bookingInfo[0].TravelRequestId);
                    //Redirect to Thank You Page
                    jsonmsg = "SOD Hotel Booking Request ID : " + s.ToString() + " ." + "Booking process has been completed successfully.Your booking request has been sent to Traveldesk for Hotel alignment.";

                    TempData["jsonmsg"] = jsonmsg;
                    return Json(jsonmsg, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var emailSubject2 = "SOD Hotel Booking Request Notification :" + System.DateTime.Now.ToString();
                    var emailTemplateName2 = "SodHotelBookingRequestFor_HodHotelApproval.html";
                    var emailCredentials2 = EmailCredentialsHotelHod(emailSubject2, emailTemplateName2, hotel_List, bookingInfo, flightinfo, s.ToString(), hodEmailId);

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
                        skey.Append(hodEmailId.Split(',')[2].ToString());

                        var uri1 = ConfigurationManager.AppSettings["emailOnlyHotelApprovalPathHod"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=a");
                        var uri2 = ConfigurationManager.AppSettings["emailOnlyHotelApprovalPathHod"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=r");

                        approvaltype = "Please help to accord your Acceptance or Rejection.";
                        appLink = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a name='app' style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Acceptance</a></td> <td>&nbsp;</td> <td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a name='rej' style='color:#fff; text-decoration:none;' href='" + uri2 + "'>Rejection</a> </td></tr></table>";

                        templateData = templateData.Replace("[approvaltype]", approvaltype);
                        templateData = templateData.Replace("[appLink]", appLink);
                        templateData = templateData.Replace("[hodName]", hodEmailId.Split(',')[1]);
                        templateData = templateData.Replace("[RequesterName]", bookingInfo[0].RequestedEmpName);
                        emailCredentials2.TemplateFilePath = templateData;
                        //EMail
                        EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials2, hodEmailId.Split(',')[0].ToString().Trim());
                        //SMS Approval
                        SendSMSforApproval(hodEmailId, bookingInfo[0], hotel_List, uri1, uri2);

                        //Redirect to Thank You Page
                        jsonmsg = "SOD Hotel Booking Request ID : " + s.ToString() + " ." + "Booking process has been completed successfully.Your booking request has been sent to your respected approver at  " + hodEmailId.Split(',')[0].ToString();
                    }
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


        public EmailNotificationModel EmailCredentialsHotel(string subjectName, string emailTemplateName, List<TravelRequestHotelDetailModels> hoteldetails, List<TravelRequestMasterModels> bookingInfo, List<FlightDetailModels> flightinfo, string reqNo, string approvalStatus)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtpHotelUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtptdPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtptdServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtptdPort"].Trim(),
                TemplateFilePath = ReadFileHotel(emailTemplateName, hoteldetails, bookingInfo, flightinfo, reqNo, approvalStatus),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }

        /// <summary>
        /// Email Credentials Hotel Hod
        /// </summary>
        /// <param name="subjectName"></param>
        /// <param name="emailTemplateName"></param>
        /// <param name="hoteldetails"></param>
        /// <param name="bookingInfo"></param>
        /// <param name="flightinfo"></param>
        /// <param name="reqNo"></param>
        /// <param name="hodemailID"></param>
        /// <returns></returns>
        public EmailNotificationModel EmailCredentialsHotelHod(string subjectName, string emailTemplateName, List<TravelRequestHotelDetailModels> hoteldetails, List<TravelRequestMasterModels> bookingInfo, List<FlightDetailModels> flightinfo, string reqNo, string hodemailID)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtpHotelUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtptdPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtptdServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtptdPort"].Trim(),
                TemplateFilePath = ReadFileHotelHod(emailTemplateName, hoteldetails, bookingInfo, flightinfo, reqNo, hodemailID),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }


        /// <summary>
        /// Read File to write in Email
        /// </summary>
        /// <param name="emailTemplateName"></param>
        /// <param name="hoteldetails"></param>
        /// <param name="bookingInfo"></param>
        /// <param name="flightinfo"></param>
        /// <param name="reqNo"></param>
        /// <param name="approvalStatus"></param>
        /// <returns></returns>
        private string ReadFileHotel(string emailTemplateName, List<TravelRequestHotelDetailModels> hoteldetails, List<TravelRequestMasterModels> bookingInfo, List<FlightDetailModels> flightinfo, string reqNo, string approvalStatus)
        {
            var strContent = new StringBuilder();
            string line;
            var file = new System.IO.StreamReader(
                       System.Web.HttpContext.Current.Server.MapPath("~/Template/Hotels/" + emailTemplateName));
            while ((line = file.ReadLine()) != null)
            {
                strContent.Append(line);
            }
            file.Close();

            var Title = bookingInfo[0].Title;
            var RequesterName = bookingInfo[0].RequestedEmpName;
            var HotelTitle = "Booking Information ";
            strContent = strContent.Replace("[RequesterName]", Title + " " + RequesterName);
            strContent = strContent.Replace("[binfo]", HotelTitle + "  (Request Id : " + reqNo + ")");
            strContent = strContent.Replace("[pinfo]", "  Passenger Information");
            strContent = strContent.Replace("[hinfo]", "  Hotel Information");

            var tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Reason for Booking</td><td>Booking Type</td><td>No. of Guests</td><td>Booking For</td></tr>";
            foreach (var b in bookingInfo)
            {
                tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td>" + b.ReasonForTravel + "</td><td>" + "SOD" + "</td><td>" + 1 + "</td><td>" + b.BookingFor + "</td></tr>";
            }

            var trp = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>S.No.</td><td>Name</td><td>Designation</td><td>Department</td></tr>";
            foreach (var b in bookingInfo)
            {
                trp = trp + "<tr style='font-family:Arial; font-size:12px;'><td>" + 1 + "</td><td>" + b.RequestedEmpName + "</td><td>" + b.RequestedEmpDesignation + "</td><td>" + b.RequestedEmpDept + "</td></tr>";
            }

            var trh = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>City</td><td>Check-In Date</td><td>Check-Out Date</td><td>Check-in time</td><td>Check-out time</td></tr>";
            foreach (var h in hoteldetails)
            {
                trh = trh + "<tr style='font-family:Arial; font-size:12px;'><td>" + h.City + "</td><td>" + h.CheckInDate.ToString("dd/MM/yyyy") + "</td><td>" + h.CheckOutDate.ToString("dd/MM/yyyy") + "</td><td>" + h.CheckinTime + "</td><td>" + h.CheckoutTime + "</td></tr>";
            }
            strContent = strContent.Replace("[tr]", tr);
            strContent = strContent.Replace("[trh]", trh);
            strContent = strContent.Replace("[trp]", trp);
            return strContent.ToString();
        }



        /// <summary>
        /// Read file for Hotel HOD
        /// </summary>
        /// <param name="emailTemplateName"></param>
        /// <param name="hoteldetails"></param>
        /// <param name="bookingInfo"></param>
        /// <param name="flightinfo"></param>
        /// <param name="reqNo"></param>
        /// <param name="approvalStatus"></param>
        /// <returns></returns>
        /// 
        //string emailTemplateName, List<BulkUploadModels> blist, string bbReqNo, string ReqName
        private string ReadFileHotelHod(string emailTemplateName, List<TravelRequestHotelDetailModels> hoteldetails, List<TravelRequestMasterModels> bookingInfo, List<FlightDetailModels> flightinfo, string reqNo, string approvalStatus)
        {
            var strContent = new StringBuilder();
            string line;
            string path = Path.Combine(HttpRuntime.AppDomainAppPath, "Template/Hotels/" + emailTemplateName);
            //var dgfjds = System.Web.HttpContext.Current.Server.MapPath("~/Template/Hotels/" + emailTemplateName);
            var file = new System.IO.StreamReader(path);
            while ((line = file.ReadLine()) != null)
            {
                strContent.Append(line);
            }
            file.Close();

            var Title = bookingInfo[0].Title;
            var RequesterName = bookingInfo[0].RequestedEmpName;
            var HotelTitle = "Hotel Booking Details ";
            strContent = strContent.Replace("[binfo]", HotelTitle + "  (Booking Request Id : " + reqNo + ")");
            strContent = strContent.Replace("[pinfo]", "Passenger(s) Information");
            strContent = strContent.Replace("[hinfo]", "Hotel(s) Information");

            var tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Reason for Booking</td><td>Booking Type</td><td>No. of Passengers</td><td>Booking For</td></tr>";
            tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td>" + bookingInfo[0].ReasonForTravel + "</td><td>" + "SOD" + "</td><td>" + bookingInfo[0].Passengers + "</td><td>" + bookingInfo[0].BookingFor + "</td></tr>";

            var trp = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Employee Id</td><td>Employee Name</td><td>Designation</td><td>Department</td></tr>";

            trp = trp + "<tr style='font-family:Arial; font-size:12px;'><td>" + bookingInfo[0].RequestedEmpCode + "</td><td>" + bookingInfo[0].RequestedEmpName + "</td><td>" + bookingInfo[0].RequestedEmpDesignation + "</td><td>" + bookingInfo[0].RequestedEmpDept + "</td></tr>";

            var trh = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>City</td><td>Check-In Date</td><td>Check-Out Date</td><td>No. of Passenger</td></tr>";
            foreach (var h in hoteldetails)
            {
                trh = trh + "<tr style='font-family:Arial; font-size:12px;'><td>" + h.City + "</td><td>" + h.CheckInDate.ToString("dd-MMM-yyyy") + "</td><td>" + h.CheckOutDate.ToString("dd-MMM-yyyy") + "</td><td>" + 1 + "</td></tr>";
            }

            strContent = strContent.Replace("[tr]", tr);
            strContent = strContent.Replace("[trp]", trp);
            strContent = strContent.Replace("[trh]", trh);
            return strContent.ToString();
        }

        /// <summary>
        /// get login user info
        /// </summary>
        /// <param name="empId"></param>
        public void GetLoginUserInfo(string empId)
        {
            var s = _userRepository.GetEmployeeList(int.Parse(empId));
            @ViewBag.EmpId = s[0].EmpId;
            @ViewBag.Name = s[0].EmpName;
            @ViewBag.Department = s[0].Department;
            @ViewBag.Email = s[0].Email;
            @ViewBag.Phone = s[0].Phone;
            @ViewBag.Designation = s[0].Designation;
            @ViewBag.DesignationId = s[0].DesignationId;
            @ViewBag.DepartmentId = s[0].DepartmentId;
            @ViewBag.Gender = s[0].Gender;
            @ViewBag.EmpCode = s[0].EmpCode;

            var empInfo = new List<string> { s[0].EmpId.ToString().Trim(), s[0].EmpName.Trim(), s[0].Department.Trim(),
                                            s[0].Designation.Trim(), s[0].DesignationId.ToString().Trim(),
                                            s[0].DepartmentId.ToString().Trim(), s[0].Gender.Trim(),s[0].EmpCode,s[0].EmployeeVertical };
            TempData["EmpInfo"] = empInfo;
            TempData["DeptId"] = s[0].DepartmentId;

            //Check Beverages Rights for Centre of Excellence Department
            string[] bevArray = ConfigurationManager.AppSettings["bevDepRight"].Split(',');
            int index = Array.IndexOf(bevArray, s[0].DepartmentId.ToString());
            @ViewBag.bevDepRight = index > -1 ? 1 : 0;
        }

        /// <summary>
        /// Send SMS for Approval
        /// </summary>
        /// <param name="reqId"></param>
        /// <param name="paxName"></param>
        /// <param name="approvalURI"></param>
        public void SendSMSforApproval(string hoddetails, TravelRequestMasterModels sodRequest, List<TravelRequestHotelDetailModels> hotelInfo, string appURI, string rejURI)
        {
            var smsText = ConfigurationManager.AppSettings["smsApprovalOnlyHotel"].ToString().Replace("@Hodname", hoddetails.Split(',')[1]);
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
            smsLogModel.EmpCode = hoddetails.Split(',')[4];
            smsLogModel.EmpName = hoddetails.Split(',')[1];
            smsLogModel.EmailID = hoddetails.Split(',')[0];
            smsLogModel.MobileNo = "7982641262";//hoddetails.Split(',')[3];
            smsLogModel.Source = "Only Hotel";
            smsLogModel.SMSText = smsText;
            smsLogModel.DeliveryDate = DateTime.Now;
            var status = SmsNotification.SmsNotifications.SendSmsViaApi(smsText, hoddetails.Split(',')[3]);
            smsLogModel.IsDelivered = status.Equals(true) ? true : false;
            _userRepository.SaveApproverSMSLog(smsLogModel);
        }

        /// <summary>
        /// Exception Handling
        /// </summary>
        /// <param name="filterContext"></param>
        private void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            Logging.ErrorLog.Instance.AddDBLogging(ex, filterContext.Controller.ControllerContext.ToString(), "Controllers/HotelOnly.cs");
        }
    }
}