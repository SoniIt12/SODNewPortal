using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web.Mvc;
using SOD.CommonWebMethod;
using SOD.EmailNotification;
using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;
using SOD.Services.Repository;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Web;

namespace SOD.Controllers
{
    [HandleError(ExceptionType = typeof(DbUpdateException), View = "Error")]
    public class EmployeeBookingDetailController : Controller, IActionFilter, IExceptionFilter
    {

        /// <summary>
        /// Constructor Initialization
        /// </summary>
        private readonly IUserRepository _userRepository;
        private readonly ITransportRepository _transportRepository;
        private readonly IOALRepository _oalRepository;
        private readonly ISjSisConcernRepository _sJsisConcernRepository;

        public string strHoldPNR = ConfigurationManager.AppSettings["strHOLDPNR"].Trim();
        public static string strHoldPNRHOD = ConfigurationManager.AppSettings["strHOLDPNRRequest"].Trim();

        public EmployeeBookingDetailController()
        {
            _userRepository = new UserRepository(new SodEntities());
            _transportRepository = new TransportRepository(new SodEntities());
            _oalRepository = new OALRepository(new SodEntities());
            _sJsisConcernRepository = new SjSisConcernRepository(new SodEntities());
        }

        /// <summary>
        /// EmployeeBookingDetail View
        /// </summary>
        /// <returns></returns>
        public ActionResult EmpBookingDetail()
        {
            if (TempData["Passengers"] != null)
            {
                ViewBag.Passengers = TempData["Passengers"].ToString();
                ViewBag.TravelRequestTypeId = TempData["TravelRequestTypeId"];
                ViewBag.SodBookingType = TempData["SodBookingType"].ToString();
                ViewBag.BookingFor = TempData["BookingFor"].ToString();
                ViewBag.Destination = TempData["DestinationName"].ToString();
                ViewBag.RequestListCount = TempData["RequestListCount"].ToString();

                if (TempData["TravelRequestTypeId"].ToString() == "3")
                {
                    ViewBag.DestinationList = TempData["DestinationList"] as List<String>;
                }
                if (TempData["ReturnDateRoundTrip"] != null)
                {
                    ViewBag.ReturnDateRoundTrip = TempData["ReturnDateRoundTrip"].ToString();
                }
                GetLoginUserInfo(Session["EmpId"].ToString());
                ViewBag.CXOList = _userRepository.GetCXO_ApproverList(Convert.ToInt16(TempData["DeptId"]), Convert.ToInt16(TempData["SodBookingType"]));
                return View();
            }
            else
            {
                return RedirectToAction("../User/SearchFlight");
            }
        }


        /// <summary>
        /// Post Flight Booking Info
        /// </summary>
        /// <param name="sodRequestsList"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SubmitFlightInfo(List<TravelRequestModels> sodRequestsList)
        {
            string s;
            if (sodRequestsList == null)
            {
                s = "Error : Flight information not available.";
                return Json(s, JsonRequestBehavior.AllowGet);
            }
            var a = sodRequestsList[0].FlightNo;
            TempData["flightBookingInfo"] = sodRequestsList;
            TempData["flightDateTimeInfo"] = sodRequestsList[0].TravelDate + " " + sodRequestsList[0].DepartureTime;
            s = "Flight details received successfully.";
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Submit Sod hotel Info post flight booking
        /// </summary>
        /// <param name="hotelDetailList"></param>
        /// <returns>True if the hotel booking is submitted</returns>
        public JsonResult SubmitHotelInfo(List<TravelRequestHotelDetailModels> hotelDetailList)
        {
            //Save booking Information
            var s = _userRepository.SaveHotelRequest(hotelDetailList);

            //get details for email notification
            var hotelList = new List<TravelRequestHotelDetailModels>();
            var dicList = new Dictionary<string, object>();

            dicList = _userRepository.GetSodHotelInfo(Convert.ToInt64(s.ToString()));

            TempData["bookingInfo"] = dicList["bookingInfolist"] as List<TravelRequestMasterModels>;
            TempData["hotelList"] = dicList["hotelinfolist"] as List<TravelRequestHotelDetailModels>;

            //Send Email Notification  
            if (TempData["hotelList"] != null && TempData["bookingInfo"] != null)
            {
                var hotel_List = TempData["hotelList"] as List<TravelRequestHotelDetailModels>;
                var bookingInfo = TempData["bookingInfo"] as List<TravelRequestMasterModels>;
                var emailSubject = "SOD Hotel Booking Request Notification :" + System.DateTime.Now.ToString();
                var emailTemplateName = "SodHotelBookingRequestFor_UserTravelHistory.html";
                var emailCredentials = EmailCredentialsHotel(emailSubject, emailTemplateName, hotel_List, bookingInfo, s.ToString(), "pending");
                TempData["emailData"] = emailCredentials;
                TempData["emailId"] = bookingInfo[0].EmailId.Trim();
                var emaildata = TempData["emailData"] as EmailNotificationModel;
                var emailid = TempData["emailId"].ToString();
                var TemplateName=string.Empty;
                //EmailNotifications.SendBookingRequestNotificationTo_Requester(emaildata, emailid);

                //mail to approver
                GetLoginUserInfo(bookingInfo[0].RequestedEmpId.ToString());
                var empInfo = TempData["EmpInfo"] as List<string>;
                string hodEmailId;

                if (bookingInfo[0].TravelRequestCode.Contains("SOD SJSC"))
                {
                    //Check User (User Email NOtification)
                    var hoddata = _sJsisConcernRepository.GetHodDetails(Session["HodEmailId"].ToString());
                    hodEmailId = hoddata[0].HodEmail + "," + hoddata[0].HoDTitle + " " + hoddata[0].HoDName + "," + "0" + "," + hoddata[0].HodMobileNo + "," + "0";
                    if (bookingInfo[0].RequestedEmpId.ToString() == hodEmailId.Split(',')[2].ToString().Trim())
                    {
                        _userRepository.UpdateHotelApprovalStatus(bookingInfo[0].TravelRequestId);
                    }
                    else
                    {
                        var Subject = "SOD" + " " + "SJSC" + " Hotel Booking Request Notification :" + System.DateTime.Now.ToString();
                        WriteEmailCredentialsForUser(Subject, TemplateName, hotel_List, bookingInfo, s.ToString(), hodEmailId);
                    }
                }
                else
                {
                    hodEmailId=_userRepository.GetHODEmailId(empInfo[8]);
                    //Check User Role : Blanket Approver List/Approver
                    var r = _userRepository.IsBlanketApproverRole(Convert.ToInt32(empInfo[0]));
                    //Approver Role
                    if (bookingInfo[0].RequestedEmpId.ToString() == hodEmailId.Split(',')[2].ToString().Trim())
                    {
                        _userRepository.UpdateHotelApprovalStatus(bookingInfo[0].TravelRequestId);
                    }
                    else if (r == true)
                    {
                        _userRepository.UpdateHotelApprovalStatus(bookingInfo[0].TravelRequestId);
                    }
                    else
                    {
                        var Subject = "SOD Hotel Booking Request Notification :" + System.DateTime.Now.ToString();
                        WriteEmailCredentialsForUser(Subject, TemplateName, hotel_List, bookingInfo, s.ToString(), hodEmailId);
                    }
                }
            }
            var jsonmsg = "SOD Hotel booking process has been initiated successfully.Please wait for further information.";
            return Json(s >= 1 ? jsonmsg : "Sorry! Hotel data cannot be submitted.", JsonRequestBehavior.AllowGet);
        }
       
        private void WriteEmailCredentialsForUser(string Subject, string TemplateName, List<TravelRequestHotelDetailModels> hotel_List, List<TravelRequestMasterModels> bookingInfo, string s, string hodEmailId)
        {

            var emailTemplateName = "SodHotelBookingRequestFor_HodHotelApproval.html";
            var emailCredentials = EmailCredentialsHotelHod(Subject, emailTemplateName, hotel_List, bookingInfo, s, hodEmailId);
            TempData["emailData_hod"] = emailTemplateName;
            TempData["emailId_hod"] = hodEmailId.Split(',')[0].ToString().Trim();

            var templateData = emailCredentials.TemplateFilePath;
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

                var uri1 = ConfigurationManager.AppSettings["emailApprovalPathHodHotel"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=a");
                var uri2 = ConfigurationManager.AppSettings["emailApprovalPathHodHotel"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=r");   
                

                approvaltype = "Please help to accord your Acceptance or Rejection.";
                appLink = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a name='app' style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Acceptance</a></td> <td>&nbsp;</td> <td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a name='rej' style='color:#fff; text-decoration:none;' href='" + uri2 + "'>Rejection</a> </td></tr></table>";

                templateData = templateData.Replace("[approvaltype]", approvaltype);
                templateData = templateData.Replace("[appLink]", appLink);
                templateData = templateData.Replace("[hodName]", hodEmailId.Split(',')[1]);
                templateData = templateData.Replace("[RequesterName]", bookingInfo[0].RequestedEmpName);
                emailCredentials.TemplateFilePath = templateData;
            }
            //var emaildata = TempData["emailData_hod"] as EmailNotificationModel;
            var emailid = TempData["emailId_hod"].ToString();
            EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials, emailid);
        }

        /// <summary>
        /// Submit Sod/Non-Sod Booking Info
        /// </summary>
        /// <param name="sodRequestsList"></param>
        /// <param name="sodflightList"></param>
        /// <param name="passengerList"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SubmitBookingInfo(List<TravelRequestMasterModels> sodRequestsList, List<FlightDetailModels> sodflightList, List<PassengerDetailModels> passengerList, List<PassengerMealAllocationModels> passengerMealsList, List<TravelRequestCabDetailModels> cabDetailList,
                List<TravelRequestHotelDetailModels> hotelDetailList)
        {
            var jsonmsg = string.Empty;
            var BookingType = string.Empty;

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
                        jsonmsg = "Warning: A similar hotel booking request has been already exist. Kindly cancel the previous hotel booking and follow the same process again";
                        TempData["jsonmsg"] = jsonmsg;
                        return Json(jsonmsg, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            try
            {
                var empInfo = TempData["EmpInfo"] as List<string>;
                var fname = empInfo[1].Substring(0, empInfo[1].LastIndexOf(' '));
                var lname = empInfo[1].Substring(empInfo[1].LastIndexOf(' ') + 1, empInfo[1].Length - empInfo[1].LastIndexOf(' ') - 1);
                if (fname.Split('.').Length > 1)
                    fname = fname.Replace('.', ' ');

                if (lname.Split('.').Length > 1)
                    lname = lname.Replace('.', ' ');

                if (lname == "." || lname == " ")
                    lname = fname.Split(' ')[0];

                if (empInfo == null)
                {
                    return Json("Error : Please follow again the same process to book the ticket.", JsonRequestBehavior.AllowGet);
                }
                //sod Booking Checking Type Id
                if (sodRequestsList[0].SodBookingTypeId == (short)SodType.NonSodBookingType)
                {
                    //Update Existing Login Emp Info  
                    BookingType = "NON-SOD";
                    sodRequestsList[0].RequestedEmpId = Convert.ToInt32(empInfo[0]);
                    sodRequestsList[0].RequestedEmpCode = empInfo[7];
                    sodRequestsList[0].RequestedEmpDept = empInfo[2];
                    sodRequestsList[0].RequestedEmpDesignation = empInfo[3];
                    sodRequestsList[0].RequestDate = DateTime.Now;
                    sodRequestsList[0].BookingStatus = "Open";
                    sodRequestsList[0].StatusDate = DateTime.Parse("01/01/1900");
                }
                else
                {
                    //Pass Traveller Info
                    BookingType = "SOD";
                    passengerList = new List<PassengerDetailModels>();
                    passengerList.Add(new PassengerDetailModels
                    {
                        Title = empInfo[6].Trim() == "M" ? "Mr" : "Ms",
                        TravelerFirstName = fname.Trim(),
                        TravelerLastName = lname.Trim(),
                        TravelerGender = empInfo[6].Trim()
                    });

                    sodRequestsList[0].RequestDate = DateTime.Now;
                    sodRequestsList[0].BookingStatus = "Open";
                    sodRequestsList[0].StatusDate = DateTime.Parse("01/01/1900");
                    //for Sod only one passenger allowed.
                    sodRequestsList[0].Passengers = 1;

                    //Add Passenger meal Option for SOD Flights
                    passengerMealsList = new List<PassengerMealAllocationModels>();
                    foreach (var item in sodflightList)
                    {
                        var passengerMealsListItem = new PassengerMealAllocationModels();
                        passengerMealsListItem.TravelRequestId = item.TravelRequestId;
                        passengerMealsListItem.Sector = item.OriginPlace + "-" + item.DestinationPlace;
                        passengerMealsListItem.MealType = item.Meals;
                        passengerMealsListItem.PassengerId = 0;
                        passengerMealsList.Add(passengerMealsListItem);
                    }

                    //Add Beverage Option for SOD Flights
                    foreach (var item in sodflightList)
                    {
                        if (item.Beverages != null)
                            if (item.Beverages.ToLower().Trim() != "not required")
                            {
                                var passengerMealsListItem = new PassengerMealAllocationModels();
                                passengerMealsListItem.TravelRequestId = item.TravelRequestId;
                                passengerMealsListItem.Sector = item.OriginPlace + "-" + item.DestinationPlace;
                                passengerMealsListItem.MealType = item.Beverages;
                                passengerMealsListItem.PassengerId = 0;
                                passengerMealsList.Add(passengerMealsListItem);
                            }
                    }
                }
                sodRequestsList[0].IsVendorBooking = false;
                sodRequestsList[0].IsSJSC = false;
                sodRequestsList[0].RequestedEmpName = fname + " " + lname;
                sodRequestsList[0].Title = empInfo[6].Trim() == "M" ? "Mr." : "Ms.";
                sodRequestsList[0].IsOKtoBoard = CommonWebMethod.OKToBoard.CheckIsOTB(sodRequestsList[0].TravelRequestTypeId, sodflightList);

                //Save booking Information
                var s = _userRepository.SaveBookingRequest(sodRequestsList, sodflightList, passengerList, passengerMealsList, cabDetailList, hotelDetailList);
                if (s >= 1)
                {
                    //Check Duplicate PNR  
                    var chkpnr = Services.ADO.SodCommonServices.CheckDuplicatePNR(Convert.ToInt64(sodRequestsList[0].TravelRequestId), 1);
                    if (!chkpnr.Equals("0"))
                    {
                        return Json("Sorry : PNR has already generated for this booking request. Existing PNR No. :" + chkpnr, JsonRequestBehavior.AllowGet);
                    }
                    //Case 1 :  Confirm Booking
                    if (sodRequestsList[0].BookingFor.Trim().ToLower() == "confirm")
                    {
                        //Check User Role : Blanket Approver List/Approver
                        var r = _userRepository.IsBlanketApproverRole(Convert.ToInt32(empInfo[0]));
                        if (r == true)
                        {
                            //Generate PNR For Blanket Approver Booking
                            var pnr = NavitaireServices.Generate_PNR(sodRequestsList, sodflightList, passengerList, passengerMealsList);
                            if (pnr == "ER001")
                            {
                                pnr = pnr + "|0.00|Error";
                                var c = _userRepository.UpdatePnr(pnr, s);
                                jsonmsg = "PNR generation fail.Error Code :" + pnr.Split('|')[0];
                            }
                            else
                            {
                                pnr = pnr + "|Close";
                                //Update PNR in Database
                                var c = _userRepository.UpdatePnr(pnr, s);
                                if (c > 0)
                                {
                                    //Upadte Hotel Approval status
                                    if (sodRequestsList[0].IsHotelRequired == true)
                                        _userRepository.UpdateHotelApprovalStatus(sodRequestsList[0].TravelRequestId);

                                    //Redirect to Thank You Page message
                                    var pnrc = "Your Confirm PNR is :" + pnr.Split('|')[0];
                                    jsonmsg = BookingType + " " + "booking process has been completed successfully. Your PNR No. :" + pnr.Split('|')[0];
                                    if (sodRequestsList[0].IsAmountPaidByTraveller.Equals(true))
                                    {
                                        //Get PNR Booking Time
                                        var pnrInfo = _userRepository.GetPNRAmountAndTime(sodRequestsList[0].TravelRequestId);
                                        var holdAmount = float.Parse(pnrInfo.Split('|')[1]);
                                        var holdAmounts = string.Format("{0:0.00}", holdAmount).ToString();
                                        var pnrGenerationTime = Convert.ToDateTime(pnrInfo.Split('|')[2]);

                                        //Get Hold Time
                                        var holdDateTime = CommonWebMethod.CommonWebMethods.GetHoldBookingDateTime(sodRequestsList[0].SodBookingTypeId, sodRequestsList[0].BookingFor, sodflightList[0].TravelDate, sodflightList[0].DepartureTime, pnrGenerationTime);
                                        var HoldTime = holdDateTime.ToString("t");
                                        var HoldDate = holdDateTime.ToString("dd/MMM/yyyy");
                                        var strholdDT = HoldTime + " dated " + HoldDate;
                                        strHoldPNR = strHoldPNR.Replace("[holdDT]", strholdDT);
                                        strHoldPNR = strHoldPNR.Replace("[pnrAmt]", holdAmounts);
                                        pnrc = pnrc + "." + strHoldPNR;
                                    }
                                    else
                                        strHoldPNR = "";

                                    //Send Email Notification  && Send Attached Stand By Ticket 
                                    var emailSubject = BookingType + " " + "Confirm Booking Request Notification :" + System.DateTime.Now.ToString();
                                    var emailTemplateName = "SodBookingRequestNotificationTemplateFor_HOD_Booking.html";
                                    var emailCredentials = EmailCredentials(emailSubject, emailTemplateName, sodRequestsList, sodflightList, passengerList, passengerMealsList, hotelDetailList, s.ToString(), "");
                                    pnrc = "Your Confirm PNR is :" + pnr.Split('|')[0];
                                    emailCredentials.TemplateFilePath = emailCredentials.TemplateFilePath.Replace("[PNR]", pnrc);

                                    //Email details for background notifications
                                    TempData["emailData"] = emailCredentials;
                                    TempData["emailId"] = sodRequestsList[0].EmailId.Trim();
                                    //Redirect to Thank You Page
                                    jsonmsg = BookingType + " " + "booking process has been completed successfully. Your PNR No. :" + pnr.Split('|')[0];
                                }
                            }
                        }
                        else
                        {
                            //Check User Role & fwd email to hod for approval
                            //Get Hold booking Status
                            if (sodRequestsList[0].IsAmountPaidByTraveller.Equals(true))
                                strHoldPNR = ConfigurationManager.AppSettings["strHOLDPNRPassenger"].Trim();
                            else
                                strHoldPNR = "";

                            //Send Email Notification
                            var emailSubject = BookingType + " " + "Confirm Booking Request Notification :" + System.DateTime.Now.ToString();
                            var emailTemplateName_User = "SodConfirmBookingRequestNotificationTemplate_User.html";
                            var emailCredentials = EmailCredentials(emailSubject, emailTemplateName_User, sodRequestsList, sodflightList, passengerList, passengerMealsList, hotelDetailList, s.ToString(), strHoldPNR);
                            TempData["emailData"] = emailCredentials;
                            TempData["emailId"] = sodRequestsList[0].EmailId.Trim();

                            //Email Notification to HOD Process
                            //Get HOD Email Id
                            string hodEmailId = _userRepository.GetHODEmailId(empInfo[8]);
                            //Update Hod Email Template
                            var emailTemplateName_hod = "SodBookingRequestNotificationTemplate_HOD.html";
                            emailCredentials = EmailCredentials(emailSubject, emailTemplateName_hod, sodRequestsList, sodflightList, passengerList, passengerMealsList, hotelDetailList, s.ToString(), strHoldPNR);
                            //Wrap link in template
                            var templateData = emailCredentials.TemplateFilePath;
                            var appLink = string.Empty;
                            var approvaltype = string.Empty;
                            if (hodEmailId.Length > 0)
                            {
                                var skey = new StringBuilder();
                                skey.Append(sodRequestsList[0].TravelRequestId.ToString() + ",");
                                skey.Append(sodRequestsList[0].EmailId.Trim() + ",");
                                skey.Append(sodRequestsList[0].SodBookingTypeId.ToString() + ",");
                                skey.Append(sodRequestsList[0].BookingFor.Trim() + ",");
                                skey.Append(hodEmailId.Split(',')[2] + ",");
                                skey.Append(sodRequestsList[0].IsMandatoryTravel.ToString());

                                var uri1 = ConfigurationManager.AppSettings["emailApprovalPath"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=a");
                                var uri2 = ConfigurationManager.AppSettings["emailApprovalPath"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=r");

                                if (sodRequestsList[0].IsMandatoryTravel.Equals(1))
                                {
                                    approvaltype = "Please help to accord your Acceptance or Rejection or Mandatory Travel – Approval.";
                                    appLink = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a name='app' style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Acceptance</a></td> <td>&nbsp;</td> <td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a name='rej'  style='color:#fff; text-decoration:none;' href='" + uri2 + "'>Rejection</a> </td><td>&nbsp;</td> <td style='width:275x; height:25px; background-color:#01DF3A;text-align:center;border-radius:5px'><a name='mapp'  style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Mandatory Travel – Approve</a></td></tr></table>";
                                }
                                else
                                {
                                    approvaltype = "Please help to accord your Acceptance or Rejection.";
                                    appLink = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a name='app' style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Acceptance</a></td> <td>&nbsp;</td> <td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a name='rej' style='color:#fff; text-decoration:none;' href='" + uri2 + "'>Rejection</a> </td></tr></table>";
                                }

                                templateData = templateData.Replace("[approvaltype]", approvaltype);
                                templateData = templateData.Replace("[appLink]", appLink);
                                templateData = templateData.Replace("[hodName]", hodEmailId.Split(',')[1]);
                                emailCredentials.TemplateFilePath = templateData;

                                //Set CXO Email ID in Temp Data
                                setCXOEmailCC(Convert.ToInt32(empInfo[5]), hodEmailId.Split(',')[0]);

                                //send email notification
                                TempData["emailData_Hod"] = emailCredentials;
                                TempData["emailId_Hod"] = hodEmailId.Split(',')[0];

                                //SMS Approval 
                                SendSMSforApproval(hodEmailId, sodRequestsList[0], sodflightList, uri1, uri2);
                            }
                            //Redirect to Thank You Page
                            jsonmsg = BookingType + " " + "booking process has been completed successfully.Your booking request has been sent to your respected approver at  " + hodEmailId.Split(',')[0].ToString();
                            if (sodRequestsList[0].IsAmountPaidByTraveller.Equals(true))
                            {
                                jsonmsg = jsonmsg + ".\n" + strHoldPNR;
                            }
                        }
                    }

                    //Case 2 :  Stand By Booking
                    else if (sodRequestsList[0].BookingFor.Trim().ToLower() == "standby")
                    {
                        //Check User Role : HOD or Approver List
                        bool r = _userRepository.IsSodApproverHodRole(Convert.ToInt32(empInfo[0]));
                        //Date :20 May,2016-Discuss with Mr. Pradeep (Blanket Approver also book Standby ticket)
                        var b = _userRepository.IsBlanketApproverRole(Convert.ToInt32(empInfo[0]));
                        if (r == true || b == true)
                        {
                            //Generate Stand By PNR For Blanket Approver or HOD 
                            var pnr = NavitaireServices.Generate_PNR(sodRequestsList, sodflightList, passengerList, passengerMealsList);
                            if (pnr == "ER001")
                            {
                                pnr = pnr + "|0.00|Error";
                                var c = _userRepository.UpdatePnr(pnr, s);
                                jsonmsg = "PNR generation fail.Error Code :" + pnr.Split('|')[0];
                            }
                            else
                            {
                                pnr = pnr + "|Close";
                                //Update in database
                                var c = _userRepository.UpdatePnr(pnr, s);
                                if (c > 0)
                                {
                                    //Upadte Hotel Approval status
                                    if (sodRequestsList[0].IsHotelRequired == true)
                                        _userRepository.UpdateHotelApprovalStatus(sodRequestsList[0].TravelRequestId);

                                    //Redirect Message for Thank You Page
                                    var pnrc = "Your Standby PNR is :" + pnr.Split('|')[0];
                                    jsonmsg = BookingType + " " + "booking process has been completed successfully.Your PNR No. :" + pnr.Split('|')[0];
                                    //Send Email Notification  && Send Attached Stand By Ticket 
                                    if (sodRequestsList[0].IsAmountPaidByTraveller.Equals(true))
                                    {
                                        //Get PNR Booking Time
                                        var pnrInfo = _userRepository.GetPNRAmountAndTime(sodRequestsList[0].TravelRequestId);
                                        var holdAmount = float.Parse(pnrInfo.Split('|')[1]);
                                        var holdAmounts = string.Format("{0:0.00}", holdAmount).ToString();
                                        var pnrGenerationTime = Convert.ToDateTime(pnrInfo.Split('|')[2]);

                                        //Get Hold Time
                                        var holdDateTime = CommonWebMethod.CommonWebMethods.GetHoldBookingDateTime(sodRequestsList[0].SodBookingTypeId, sodRequestsList[0].BookingFor, sodflightList[0].TravelDate, sodflightList[0].DepartureTime, pnrGenerationTime);
                                        var HoldTime = holdDateTime.ToString("t");
                                        var HoldDate = holdDateTime.ToString("dd/MMM/yyyy");
                                        var strholdDT = HoldTime + " dated " + HoldDate;
                                        strHoldPNR = strHoldPNR.Replace("[holdDT]", strholdDT);
                                        strHoldPNR = strHoldPNR.Replace("[pnrAmt]", holdAmounts);
                                        pnrc = pnrc + "." + strHoldPNR;
                                    }
                                    else
                                        strHoldPNR = "";

                                    var emailSubject = BookingType + " " + "Standby Booking Request Notification :" + System.DateTime.Now.ToString();
                                    var emailTemplateName = "SodBookingRequestNotificationTemplateFor_HOD_Booking.html";
                                    var emailCredentials = EmailCredentials(emailSubject, emailTemplateName, sodRequestsList, sodflightList, passengerList, passengerMealsList, hotelDetailList, s.ToString(), strHoldPNR);
                                    emailCredentials.TemplateFilePath = emailCredentials.TemplateFilePath.Replace("[PNR]", pnrc);

                                    //hold Email Background notification data
                                    TempData["emailData"] = emailCredentials;
                                    TempData["emailId"] = sodRequestsList[0].EmailId.Trim();
                                }
                            }
                        }
                        else
                        {
                            //No PNR Generation                     
                            if (sodRequestsList[0].IsAmountPaidByTraveller.Equals(true))
                                strHoldPNR = ConfigurationManager.AppSettings["strHOLDPNRPassenger"].Trim();
                            else
                                strHoldPNR = "";

                            //Send Email Notification to user and Hod
                            var emailSubject = BookingType + " " + "Standby Booking Request Notification :" + System.DateTime.Now.ToString();
                            var emailTemplateName = "SodStandbyBookingRequestNotificationTemplate_User.html";
                            var emailCredentials = EmailCredentials(emailSubject, emailTemplateName, sodRequestsList, sodflightList, passengerList, passengerMealsList, hotelDetailList, s.ToString(), strHoldPNR);
                            TempData["emailData"] = emailCredentials;
                            TempData["emailId"] = sodRequestsList[0].EmailId.Trim();   

                            //Only Standby Approval Check Department :CC mail
                            string hodEmailId = string.Empty;
                            string hodEmailIdCC = string.Empty;
                            string[] deptId_forStandby = ConfigurationManager.AppSettings["Only_Standby_Approval"].Trim().Split(',');
                            var empDeptId = empInfo[5].ToString();
                            int index = Array.IndexOf(deptId_forStandby, empDeptId);
                            if (index > -1)
                            {
                                //Get Only Standby HOD Approval Id
                                hodEmailId = _userRepository.GetHODEmailId_OnlyStandby(empInfo[8]);
                                hodEmailIdCC = _userRepository.GetHODEmailId(empInfo[8]);
                                if (hodEmailId == "")
                                {
                                    hodEmailId = hodEmailIdCC;
                                    TempData["emailId_HodCC"] = null;
                                }
                                else
                                    TempData["emailId_HodCC"] = hodEmailIdCC.Split(',')[0];
                            }
                            else
                                hodEmailId = _userRepository.GetHODEmailId(empInfo[8]);//pass VertivalID  

                            //Update Hod Email Template
                            var emailTemplateName_hod = "SodBookingRequestNotificationTemplate_HOD.html";
                            emailCredentials = EmailCredentials(emailSubject, emailTemplateName_hod, sodRequestsList, sodflightList, passengerList, passengerMealsList, hotelDetailList, s.ToString(), strHoldPNR);

                            var approvaltype = string.Empty;
                            if (hodEmailId.Length > 0)
                            {
                                var skey = new StringBuilder();
                                skey.Append(sodRequestsList[0].TravelRequestId.ToString() + ",");
                                skey.Append(sodRequestsList[0].EmailId.Trim() + ",");
                                skey.Append(sodRequestsList[0].SodBookingTypeId.ToString() + ",");
                                skey.Append(sodRequestsList[0].BookingFor.Trim() + ",");
                                skey.Append(hodEmailId.Split(',')[2] + ",");
                                skey.Append(sodRequestsList[0].IsMandatoryTravel.ToString());

                                var uri1 = ConfigurationManager.AppSettings["emailApprovalPath"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=a");
                                var uri2 = ConfigurationManager.AppSettings["emailApprovalPath"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=r");

                                var appLink = string.Empty;

                                if (sodRequestsList[0].IsMandatoryTravel.Equals(1))
                                {
                                    approvaltype = "Please help to accord your Acceptance or Rejection or Mandatory Travel – Approval.";
                                    appLink = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a name='app'   style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Acceptance</a></td> <td>&nbsp;</td> <td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a name='rej' style='color:#fff; text-decoration:none;' href='" + uri2 + "'>Rejection</a> </td> <td>&nbsp;</td> <td style='width:275x; height:25px; background-color:#01DF3A;text-align:center;border-radius:5px'><a name='mapp' style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Mandatory Travel – Approve</a></td></tr></table>";
                                }
                                else
                                {
                                    approvaltype = "Please help to accord your Acceptance or Rejection.";
                                    appLink = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a name='app'  style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Acceptance</a></td> <td>&nbsp;</td> <td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a name='rej'  style='color:#fff; text-decoration:none;' href='" + uri2 + "'>Rejection</a> </td></tr></table>";
                                }

                                var templateData = emailCredentials.TemplateFilePath;
                                var hodName = hodEmailId.Split(',')[1];
                                templateData = templateData.Replace("[approvaltype]", approvaltype);
                                templateData = templateData.Replace("[appLink]", appLink);
                                templateData = templateData.Replace("[hodName]", hodName);
                                emailCredentials.TemplateFilePath = templateData;

                                //Redirect to Thank You Page
                                jsonmsg = BookingType + " " + "Standby booking process has been completed successfully.Your booking request has been sent to your respected approver at  " + hodEmailId.Split(',')[0].ToString();
                                if (sodRequestsList[0].IsAmountPaidByTraveller.Equals(true))
                                {
                                    jsonmsg = jsonmsg + ".\n" + strHoldPNR;
                                }
                                //Set CXO Email ID in Temp Data
                                setCXOEmailCC(Convert.ToInt32(empDeptId), hodEmailId.Split(',')[0]);

                                TempData["emailData_Hod"] = emailCredentials;
                                TempData["emailId_Hod"] = sodRequestsList[0].EmailId.Trim();   

                                //SMS Approval 
                                SendSMSforApproval(hodEmailId, sodRequestsList[0], sodflightList, uri1, uri2);
                            }
                        }
                    }
                }
                TempData["jsonmsg"] = jsonmsg;
                return Json(s >= 1 ? jsonmsg : "Error :Booking Process fail.Please contact to administrator", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                jsonmsg = ex.InnerException.Message.ToString();
                return Json(jsonmsg, JsonRequestBehavior.AllowGet);

            }

        }


        /// <summary>
        /// Get Flight Booking Info
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetFlightBookingInfo()
        {
            var s = TempData["flightBookingInfo"];
            return Json(s, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Get Login User Info 
        /// </summary>
        /// <returns></returns>
        public void GetLoginUserInfo(string empId)
        {
            var s = empId.Equals("0") ? SOD.Services.ADO.SodCommonServices.GetEmployeeCommonDetailsSJSC(Session["SjsUserId"].ToString(), 1) : _userRepository.GetEmployeeList(int.Parse(empId));

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
        /// Sod Booking Type Manage Here
        /// </summary>
        public enum SodType
        {
            SodBookingType = 1,
            NonSodBookingType = 2
        }


        /// <summary>
        /// Initialized Email Credentials to Property
        /// </summary>
        /// <returns></returns>
        public EmailNotificationModel EmailCredentials(string subjectName, string emailTemplateName, List<TravelRequestMasterModels> sodRequestsList, List<FlightDetailModels> sodflightList, List<PassengerDetailModels> sodPassList, List<PassengerMealAllocationModels> sodPassMealsList, List<TravelRequestHotelDetailModels> hotelDetailList, string reqNo, string strHoldPNRMessage)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtpUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtpPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtpServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtpPort"].Trim(),
                TemplateFilePath = ReadFile(emailTemplateName, sodRequestsList, sodflightList, sodPassList, sodPassMealsList, hotelDetailList, reqNo, strHoldPNRMessage),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }


        /// <summary>
        /// Read Template File from Location
        /// </summary>
        /// <returns></returns>
        private string ReadFile(string emailTemplateName, List<TravelRequestMasterModels> sodRequestsList, List<FlightDetailModels> sodflightList, List<PassengerDetailModels> sodPassList, List<PassengerMealAllocationModels> sodPassMealsList, List<TravelRequestHotelDetailModels> hotelDetailList, string reqNo, string strHoldPNRMessage)
        {
            var strContent = new StringBuilder();
            string line;
            string path = Path.Combine(HttpRuntime.AppDomainAppPath, "Template/" + emailTemplateName);
            var file = new System.IO.StreamReader(path);
            //var file = new System.IO.StreamReader(
            //Server.MapPath("~/Template/" + emailTemplateName));
            while ((line = file.ReadLine()) != null)
            {
                strContent.Append(line);
            }
            file.Close();

            strContent = strContent.Replace("[RequesterName]", sodRequestsList[0].Title + " " + sodRequestsList[0].RequestedEmpName);
            var mltr = string.Empty;
            var c = 0;
            var bookingFor = sodRequestsList[0].BookingFor;
            var meal = sodRequestsList[0].Meals;
            var deg = sodRequestsList[0].RequestedEmpDesignation;
            var dep = sodRequestsList[0].RequestedEmpDept;
            var mtr = sodRequestsList[0].IsMandatoryTravel.Equals(1) ? "Yes" : "No";
            var btype = sodRequestsList[0].SodBookingTypeId.Equals(1) ? "SOD" : "NON-SOD";

            var tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Reason for Travel</td><td>Booking Type</td><td>No.of Passengers</td><td>Booking For</td>";
            if (mtr == "Yes")
            {
                tr = tr + "<td>Is Mandatory Travel</td>";
                //strContent = strContent.Replace("[mtaText]", "Reason for Mandatory Travel Request  : " + sodRequestsList[0].ReasonForMandatoryTravel); //Kiran Sir Request
                strContent = strContent.Replace("[mtaText]", "");
            }
            else
            {
                strContent = strContent.Replace("[mtaText]", "");
            }

            //Ok to Board
            if (sodRequestsList[0].IsOKtoBoard.Equals(true))
                tr = tr + "<td>Ok to Board</td>";

            if (strHoldPNRMessage != "")
                strContent = strContent.Replace("[hpnr]", strHoldPNRMessage);
            else
                strContent = strContent.Replace("[hpnr]", "");

            tr = tr + "</tr>";

            //Adding Booing Info
            tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td>" + sodRequestsList[0].ReasonForTravel + "</td><td>" + btype + "</td><td>" + sodRequestsList[0].Passengers + "</td><td>" + bookingFor + "</td>";
            if (mtr == "Yes")
                tr = tr + "<td> Yes - " + sodRequestsList[0].ReasonForMandatoryTravel + "</td>";
            if (sodRequestsList[0].IsOKtoBoard.Equals(true))
                tr = tr + "<td> Yes </td>";

            tr = tr + "</tr>";
            strContent = strContent.Replace("[tr]", tr);

            //Begin of Passengers List----------------------------------------------------------------------------------------------------------------
            var i = 0;
            var trp = string.Empty;

            if (btype == "SOD")
            {
                strContent = strContent.Replace("[pinfo]", "Passenger(s) Information");
                strContent = strContent.Replace("[finfo]", "Flight and Meals Information");
                strContent = strContent.Replace("[binfo]", "Booking Information (Request ID : SOD-" + reqNo + ")");
                trp = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>S. No.</td><td>Passenger Name</td><td>Designation</td><td>Department</td></tr>";
            }
            else
            {
                strContent = strContent.Replace("[pinfo]", "Passenger(s) and Meals Information");
                strContent = strContent.Replace("[finfo]", "Flight Information");
                strContent = strContent.Replace("[binfo]", "Booking Information (Request ID : NON-SOD-" + reqNo + ")");
                trp = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>S. No.</td><td>Passenger Name</td>";
                foreach (var f in sodflightList)
                {
                    var sector = f.OriginPlace + "-" + f.DestinationPlace;
                    trp = trp + "<td style='border-top:1px solid #c2c2c2'> Meal (" + sector + ")</td>";
                }
                trp = trp + "</tr>";
            }
            foreach (var p in sodPassList)
            {
                i++;
                if (btype == "SOD")
                    trp = trp + "<tr style='font-family:Arial; font-size:12px;'><td>" + i + "</td><td>" + p.TravelerFirstName + " " + p.TravelerLastName + "</td><td>" + deg + "</td><td>" + dep + "</td></tr>";
                else
                {
                    trp = trp + "<tr><td style='border-top:1px solid #c2c2c2'>" + i + "</td>";
                    int row = 0;
                    foreach (var fl in sodflightList)
                    {
                        var sector = fl.OriginPlace + "-" + fl.DestinationPlace;
                        foreach (var m in sodPassMealsList)
                        {
                            if (sector == m.Sector)
                            {
                                if (m.PassengerId == i && row == 0)
                                {
                                    trp = trp + "<td style='border-top:1px solid #c2c2c2'> " + p.Title + ". " + p.TravelerFirstName + " " + p.TravelerLastName + "</td>";
                                    trp = trp + "<td style='border-top:1px solid #c2c2c2'>" + m.MealType + "</td>";
                                    row++;
                                }
                                else if (m.PassengerId == i)
                                {
                                    trp = trp + "<td style='border-top:1px solid #c2c2c2'>" + m.MealType + "</td>";
                                }
                            }
                        }
                    }
                    trp = trp + "</tr>";
                }
            }
            strContent = strContent.Replace("[trp]", trp);
            //-EOD Passenger List--------------------------------------------------------------------------------------------------------------------

            //Begin of Flight List--------------------------------------------------------------------------------------------------------------------
            var trf = "";
            if (btype == "SOD")
            {
                trf = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Sector</td><td>Travel Date</td><td>Flight No.</td><td>Departure Time</td><td>Arrival Time</td><td>Meals/Beverages</td></tr>";
                foreach (var f in sodflightList)
                {
                    var sector = f.OriginPlace + "-" + f.DestinationPlace;
                    trf = trf + "<tr style='font-family:Arial; font-size:12px;'><td style='border-top:1px solid #c2c2c2'>" + sector + "</td><td style='border-top:1px solid #c2c2c2'>" + f.TravelDate.ToString("dd-MMM-yyyy") + "</td><td style='border-top:1px solid #c2c2c2'>" + f.FlightNo + "</td><td style='border-top:1px solid #c2c2c2'>" + f.DepartureTime + "</td><td style='border-top:1px solid #c2c2c2'>" + f.ArrivalTime + "</td>";
                    var meals = string.Empty;
                    foreach (var m in sodPassMealsList)
                    {
                        if (m.Sector == sector && m.MealType != "BVG")
                            meals = m.MealType;
                        if (m.Sector == sector && m.MealType == "BVG")
                            meals = meals + "," + m.MealType;
                    }
                    if (meals != string.Empty)
                        trf = trf + "<td style='border-top:1px solid #c2c2c2'>" + meals + "</td>";
                    trf = trf + "</tr>";
                }
            }
            else
            {
                trf = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Sector</td><td>Travel Date</td><td>Flight No.</td><td>Departure Time</td><td>Arrival Time</td></tr>";
                foreach (var f in sodflightList)
                {
                    var sector = f.OriginPlace + "-" + f.DestinationPlace;
                    trf = trf + "<tr style='font-family:Arial; font-size:12px;'><td style='border-top:1px solid #c2c2c2'>" + sector + "</td><td style='border-top:1px solid #c2c2c2'>" + f.TravelDate.ToString("dd-MMM-yyyy") + "</td><td style='border-top:1px solid #c2c2c2'>" + f.FlightNo + "</td><td style='border-top:1px solid #c2c2c2'>" + f.DepartureTime + "</td><td style='border-top:1px solid #c2c2c2'>" + f.ArrivalTime + "</td></tr>";
                }
            }
            //-EOD Flight List--------------------------------------------------------------------------------------------------------------------
            strContent = strContent.Replace("[trf]", trf);
            //-EOD Flight List--------------------------------------------------------------------------------------------------------------------

            //Begin of Hotel List--------------------------------------------------------------------------------------------------------------------

            if (sodRequestsList[0].IsHotelRequired == true)
            {
                var trh = "";
                if (btype == "SOD")
                {
                    trh = "<table cellpadding='0' cellspacing='0' style='width:100%; font-size:12px;border:solid 1px #c2c2c2;line-height:30px;'>";
                    trh = trh + "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>City</td><td>Check-In Date</td><td>Check-Out Date</td><td>No. Of Passenger</td></tr>";
                    foreach (var h in hotelDetailList)
                    {
                        //var sector = f.OriginPlace + "-" + f.DestinationPlace;
                        trh = trh + "<tr style='font-family:Arial; font-size:12px;'><td style='border-top:1px solid #c2c2c2'>" + h.City + "</td><td style='border-top:1px solid #c2c2c2'>" + h.CheckInDate.ToString("dd-MMM-yyyy") + "</td><td style='border-top:1px solid #c2c2c2'>" + h.CheckOutDate.ToString("dd-MMM-yyyy") + "</td><td style='border-top:1px solid #c2c2c2'>" + h.NoOfGuest + "</td>";
                        trh = trh + "</tr>";
                    }
                    trh = trh + "</table>";
                }
                //else
                //{
                //    trh = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Sector</td><td>Travel Date</td><td>Flight No.</td><td>Departure Time</td><td>Arrival Time</td></tr>";
                //    foreach (var f in sodflightList)
                //    {
                //        var sector = f.OriginPlace + "-" + f.DestinationPlace;
                //        trh = trh + "<tr style='font-family:Arial; font-size:12px;'><td style='border-top:1px solid #c2c2c2'>" + sector + "</td><td style='border-top:1px solid #c2c2c2'>" + f.TravelDate.ToString("dd-MMM-yyyy") + "</td><td style='border-top:1px solid #c2c2c2'>" + f.FlightNo + "</td><td style='border-top:1px solid #c2c2c2'>" + f.DepartureTime + "</td><td style='border-top:1px solid #c2c2c2'>" + f.ArrivalTime + "</td></tr>";
                //    }
                //}
                //-EOD Hotel List--------------------------------------------------------------------------------------------------------------------
                strContent = strContent.Replace("[trh]", trh);
                strContent = strContent.Replace("[hinfo]", "<table cellpadding='0' cellspacing='0' style='width:100%; border:0px;'><tr><td style='font-size: 16px;font-family:Arial;margin-top: 15px;margin-bottom:20px; border-right:solid 0px transparent;'><p>Hotel Information</p> </td></tr></table>");
            }
            else
            {
                strContent = strContent.Replace("[hinfo]", String.Empty);
                strContent = strContent.Replace("[trh]", String.Empty);
            }
            strContent = strContent.Replace("[trml]", mltr == "" ? "" : mltr);
            return strContent.ToString();
        }


        /// <summary>
        /// Successfull/Thanks Page
        /// </summary>
        /// <returns></returns>
        public ActionResult BookingResponse()
        {
            sendEmailNotification();
            return View("BookingResponse");
        }


        /// <summary>
        /// Send Email Notification to booking requester/user
        /// </summary>
        /// <returns></returns>
        public void sendEmailNotification()
        {
            if (TempData["emailData"] != null && TempData["emailId"] != null)
            {
                var emaildata = TempData["emailData"] as EmailNotificationModel;
                var emailid = TempData["emailId"].ToString();
                EmailNotifications.SendBookingRequestNotificationTo_Requester(emaildata, emailid);
            }

            if (TempData["emailData_Hod"] != null && TempData["emailId_Hod"] != null)
            {
                var emaildataHod = TempData["emailData_Hod"] as EmailNotificationModel;
                var emailidHod = TempData["emailId_Hod"].ToString();

                if (TempData["emailId_HodCC"] != null)
                    EmailNotifications.SendBookingRequestNotificationTo_HODCC(emaildataHod, emailidHod, TempData["emailId_HodCC"].ToString());
                else
                    EmailNotifications.SendBookingRequestNotificationTo_HOD(emaildataHod, emailidHod);
            }
        }


        /// <summary>
        /// Validate Cab Request Time
        /// Transport Request should be received by the (D-6) hours Minimum
        /// </summary>
        /// <returns></returns>
        public int ValidateCabRequest()
        {
            var s = 0;
            var CTime = DateTime.UtcNow.AddMinutes(330);
            CTime = CTime.AddMinutes(360);
            if (TempData["flightDateTimeInfo"] != null)
            {
                var fdate = TempData["flightDateTimeInfo"].ToString();
                var fdatetime = Convert.ToDateTime(fdate);
                if (CTime < fdatetime)
                    s = 1;
            }
            return s;
        }

        /// <summary>
        /// Validate Cab Request Time
        /// Hotel request should be reveived by the (D-12) hours minimum
        /// </summary>
        /// <returns></returns>
        public int ValidateHotelRequest()
        {
            var s = 0;
            var CTime = DateTime.UtcNow.AddMinutes(330);
            CTime = CTime.AddMinutes(1440);
            if (TempData.Peek("flightDateTimeInfo").ToString() != null)
            {
                //var fdate = TempData["flightDateTimeInfo"].ToString();
                var fdate = TempData.Peek("flightDateTimeInfo").ToString();
                var fdatetime = Convert.ToDateTime(fdate);
                if (CTime < fdatetime)
                    s = 1;
            }
            return s;
        }

        public EmailNotificationModel EmailCredentialsHotel(string subjectName, string emailTemplateName, List<TravelRequestHotelDetailModels> hoteldetails, List<TravelRequestMasterModels> bookingInfo, string reqNo, string approvalStatus)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtpHotelUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtptdPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtptdServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtptdPort"].Trim(),
                TemplateFilePath = ReadFileHotel(emailTemplateName, hoteldetails, bookingInfo, reqNo, approvalStatus),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }

        public EmailNotificationModel EmailCredentialsHotelHod(string subjectName, string emailTemplateName, List<TravelRequestHotelDetailModels> hoteldetails, List<TravelRequestMasterModels> bookingInfo, string reqNo, string hodemailID)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtpHotelUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtptdPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtptdServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtptdPort"].Trim(),
                TemplateFilePath = ReadFileHotelHod(emailTemplateName, hoteldetails, bookingInfo, reqNo, hodemailID),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }

        /// <summary>
        /// Read File Hotel
        /// </summary>
        /// <param name="emailTemplateName"></param>
        /// <param name="hoteldetails"></param>
        /// <param name="bookingInfo"></param>
        /// <param name="reqNo"></param>
        /// <param name="approvalStatus"></param>
        /// <returns></returns>
        private string ReadFileHotel(string emailTemplateName, List<TravelRequestHotelDetailModels> hoteldetails, List<TravelRequestMasterModels> bookingInfo, string reqNo, string approvalStatus)
        {
            var strContent = new StringBuilder();
            string line;
            var file = new System.IO.StreamReader(
                       Server.MapPath("~/Template/" + emailTemplateName));
            while ((line = file.ReadLine()) != null)
            {
                strContent.Append(line);
            }
            file.Close();

            var Title = bookingInfo[0].Title;
            var RequesterName = bookingInfo[0].RequestedEmpName;
            var HotelTitle = "Hotel Booking Details ";
            strContent = strContent.Replace("[RequesterName]", Title + " " + RequesterName);
            strContent = strContent.Replace("[binfo]", HotelTitle + "  (Booking Request Id : " + reqNo + ")");
            strContent = strContent.Replace("[hotelstatus]", approvalStatus);

            var tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>City</td><td>Check-In Date</td><td>Check-Out Date</td><td>Check-in time</td><td>Check-out time</td></tr>";

            foreach (var h in hoteldetails)
            {
                tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td>" + h.City + "</td><td>" + h.CheckInDate.ToString("dd/MM/yyyy") + "</td><td>" + h.CheckOutDate.ToString("dd/MM/yyyy") + "</td><td>" + h.CheckinTime + "</td><td>" + h.CheckoutTime + "</td></tr>";
            }
            strContent = strContent.Replace("[tr]", tr);
            return strContent.ToString();
        }

        /// <summary>
        /// Read File Hotel Hod
        /// </summary>
        /// <param name="emailTemplateName"></param>
        /// <param name="hoteldetails"></param>
        /// <param name="bookingInfo"></param>
        /// <param name="reqNo"></param>
        /// <param name="approvalStatus"></param>
        /// <returns></returns>
        private string ReadFileHotelHod(string emailTemplateName, List<TravelRequestHotelDetailModels> hoteldetails, List<TravelRequestMasterModels> bookingInfo, string reqNo, string approvalStatus)
        {
            var strContent = new StringBuilder();
            string line;
            var file = new System.IO.StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/Template/Hotels/" + emailTemplateName));
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

            var btype = bookingInfo[0].SodBookingTypeId.Equals(1) ? "SOD" : "NON-SOD";
            var tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Reason for Travel</td><td>Booking Type</td><td>No.of Passengers</td><td>Booking For</td></tr>";
            tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td>" + bookingInfo[0].ReasonForTravel + "</td><td>" + btype + "</td><td>" + bookingInfo[0].Passengers + "</td><td>" + bookingInfo[0].BookingFor + "</td></tr>";

            var trp = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Employee Id</td><td>Employee Name</td><td>Designation</td><td>Department</td></tr>";
            trp = trp + "<tr style='font-family:Arial; font-size:12px;'><td>" + bookingInfo[0].RequestedEmpId + "</td><td>" + bookingInfo[0].RequestedEmpName + "</td><td>" + bookingInfo[0].RequestedEmpDesignation + "</td><td>" + bookingInfo[0].RequestedEmpDept + "</td></tr>";

            var trh = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>City</td><td>Check-In Date</td><td>Check-Out Date</td><td>No. of Passenger</td></tr>";
            foreach (var h in hoteldetails)
            {
                trh = trh + "<tr style='font-family:Arial; font-size:12px;'><td>" + h.City + "</td><td>" + h.CheckInDate.ToString("dd-MMM-yyyy") + "</td><td>" + h.CheckOutDate.ToString("dd-MMM-yyyy") + "</td><td>" + h.NoOfGuest + "</td></tr>";
            }

            strContent = strContent.Replace("[tr]", tr);
            strContent = strContent.Replace("[trp]", trp);
            strContent = strContent.Replace("[trh]", trh);
            return strContent.ToString();
        }

        /// <summary>
        /// Cancel hotel request user
        /// </summary>
        /// <param name="hotelDetailList"></param>
        /// <returns></returns>
        [SessionTimeout]
        public JsonResult cancelHotelRequest(string TravelRequestId)
        {
            var trid = Convert.ToInt64(TravelRequestId.Split('|')[0]);
            var hid = TravelRequestId.Split('|')[1];
            var reason = TravelRequestId.Split('|')[2];
            var type = TravelRequestId.Split('|')[3];
            var isAllocated = 0;
            var HotelStatus = "";
            var usercheckinstatus = false;

            if (type == "SOD")
            {
                var hotelList = new List<TravelRequestHotelDetailModels>();
                var dicList = new Dictionary<string, object>();
                dicList = _transportRepository.GetSodHotelInfo(Convert.ToInt64(trid), Convert.ToInt32(hid));
                hotelList = dicList["hotelinfolist"] as List<TravelRequestHotelDetailModels>;
                isAllocated = hotelList[0].IsAllocated;
                HotelStatus = hotelList[0].HotelStatus;
                usercheckinstatus = hotelList[0].UserCheckinCheckoutUpdate;
            }
            if (isAllocated == 2)
            {
                return Json("Rejected", JsonRequestBehavior.AllowGet);
            }
            else if (usercheckinstatus == true)
            {
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
            else
            {
                var status = "";
                if (HotelStatus == "Approved by Hotel")
                {
                    status = "Allocated";
                }
                else
                {
                    status = "Not Allocated";
                }
                var s = _transportRepository.CancelHotelRequest(trid, Convert.ToInt32(hid), reason, status, type);
                if (s > 0)
                {
                    var c = hotelCancellationRequest(trid.ToString() + "|" + hid.ToString() + "|" + type);
                    return Json("CancellationInitiated", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Error in cancellation process. Please try again.", JsonRequestBehavior.AllowGet);
                }
            }
        }


        /// <summary>
        /// Set CXO Email ID in Temp Data for Email CC
        /// </summary>
        /// <param name="deptId"></param>
        public void setCXOEmailCC(int deptId, string hodEmailId)
        {
            //Get CXO Email ID for Confirm Booking CC Mail
            TempData["emailId_HodCC"] = null;
            string[] CXO_EmailCCName = ConfigurationManager.AppSettings["ConfirmBooking_CXO_EmailCC"].Trim().Split(',');
            string cxoEmails = _userRepository.GetCXO_EmailCC(deptId);//Dept ID
            if (cxoEmails != "")
            {
                int index = Array.IndexOf(CXO_EmailCCName, cxoEmails.Split('|')[0]);
                if (index > -1)
                {
                    if (cxoEmails.Split('|')[1] != hodEmailId)
                        TempData["emailId_HodCC"] = cxoEmails.Split('|')[1];
                }
            }
        }


        /// <summary>
        /// Action Filter Method
        /// </summary>
        /// <param name="filterContext"></param>
        //void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
        //{
        //If Required : Need to Implement
        // }


        /// <summary>
        /// On OnActionExecuting
        /// </summary>
        /// <param name="filterContext"></param>
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["EmpId"] == null)
            {
                Response.Clear();
                CloseBookingList();
            }
        }


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
        /// send cancellation request to hotel
        /// </summary>
        /// <param name="travelRqstId"></param>
        public int hotelCancellationRequest(string travelRqstId)
        {
            var travReqstId = travelRqstId.Split('|')[0];
            var hid = Convert.ToInt32(travelRqstId.Split('|')[1]);
            var sodOat = travelRqstId.Split('|')[2];
            string hotelname = "";
            var primaryEmail = "";

            if (sodOat == "SOD")
            {
                var dicListHotel = new Dictionary<string, object>();
                var hoteldetails = new List<HotelRequestApprovalModel>();
                dicListHotel = _transportRepository.getApprovalHotelDetails(travReqstId, hid, sodOat);
                hoteldetails = dicListHotel["approvalHotelDetails"] as List<HotelRequestApprovalModel>;
                if (hoteldetails.Count > 0)
                {
                    hotelname = hoteldetails[0].HotelName;
                    primaryEmail = hoteldetails[0].PrimaryEmail;
                }
            }
            else
            {
                var dicListHotel = new Dictionary<string, object>();
                var hoteldetails = new List<HotelRequestApprovalOatModels>();
                dicListHotel = _transportRepository.getApprovalHotelDetails(travReqstId, hid, sodOat);
                hoteldetails = dicListHotel["approvalHotelDetails"] as List<HotelRequestApprovalOatModels>;
                hotelname = hoteldetails[0].HotelName;
                primaryEmail = hoteldetails[0].PrimaryEmail;
            }

            var s = 0;
            List<String> listhotelInfo = new List<String>();
            listhotelInfo.Add(travReqstId);

            List<String> hidList = new List<String>();
            hidList.Add(hid.ToString());

            var dicList = new Dictionary<string, object>();
            dicList = _transportRepository.GetHotelandUserInfo(listhotelInfo, hidList, hotelname, sodOat);
            TempData["PassengerInfoForHotelRequest"] = dicList["PassengerInfoForHotelRequest"] as List<TravelRequestModels>;
            var hotelCancellationInfo = TempData["PassengerInfoForHotelRequest"] as List<TravelRequestModels>;

            var username = hotelCancellationInfo[0].RequestedEmpName;
            var useremail = hotelCancellationInfo[0].PassEmailId;

            if (primaryEmail.Length > 0 && useremail.Length > 0)
            {
                //send cancellation mail to user
                var emailSubject2 = "Hotel Cancellation Request Notification from Travel Desk:" + System.DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt");
                var emailTemplateName2 = "HotelBookingRequestNotificationTemplate_Cancellation.html";
                var emailCredentials2 = EmailCredentialsHotelRequest(emailSubject2, emailTemplateName2, listhotelInfo, hidList, hotelname, sodOat, "user");
                var templateData2 = emailCredentials2.TemplateFilePath;
                templateData2 = templateData2.Replace("[hotelName]", username);
                emailCredentials2.TemplateFilePath = templateData2;
                TempData["emailData_Hod"] = emailCredentials2;
                TempData["emailId_Hod"] = useremail;
                EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials2, useremail);


                var emailSubject = "Hotel Cancellation Request Notification from Spicejet:" + System.DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt");
                var emailTemplateName = "HotelBookingRequestNotificationTemplate_Cancellation.html";
                var emailCredentials = EmailCredentialsHotelRequest(emailSubject, emailTemplateName, listhotelInfo, hidList, hotelname, sodOat, "hotel");
                var templateData = emailCredentials.TemplateFilePath;
                templateData = templateData.Replace("[hotelName]", hotelname);
                emailCredentials.TemplateFilePath = templateData;
                TempData["emailData_Hod"] = emailCredentials;
                TempData["emailId_Hod"] = primaryEmail;
                EmailNotifications.SendBookingRequestNotificationTo_Requester_Traveldesk(emailCredentials, primaryEmail);
                TempData["msgResponse"] = "Hotel Request for Travel Request Id " + travReqstId.ToString() + " : Hotel cancellation request has been sent successfully. The request has been sent to the respected hotel at  " + primaryEmail;
            }
            return s;
        }


        public EmailNotificationModel EmailCredentialsHotelRequest(string subjectName, string emailTemplateName, List<String> requestList, List<String> hidList, string hotelname, string sodOat, string mailtype)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtpHotelUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtptdPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtptdServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtptdPort"].Trim(),
                TemplateFilePath = ReadFileHotelRequest(emailTemplateName, requestList, hidList, hotelname, sodOat, mailtype),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }


        private string ReadFileHotelRequest(string emailTemplateName, List<String> requestList, List<String> hidList, string hotelname, string sodOat, string mailtype)
        {
            var strContent = new StringBuilder();
            string line;
            var file = new System.IO.StreamReader(
                       Server.MapPath("~/Template/Hotels/" + emailTemplateName));
            while ((line = file.ReadLine()) != null)
            {
                strContent.Append(line);
            }
            file.Close();
            strContent = strContent.Replace("[binfo]", "Booking Details ");

            GetHotelRequestData(requestList, hidList, hotelname, sodOat);
            var passengerInfo = TempData["PassengerInfoForHotelRequest"] as List<TravelRequestModels>;
            var count = 1;
            var gender = "";
            var airtransport = "";
            var ETAString = "";
            var HotelPriceString = "";
            if (sodOat == "SOD")
            {
                ETAString = "<td style='height:20px; padding-bottom:8px;'>ETA</td>";
            }
            else
            {
                ETAString = "<td style='height:20px; padding-bottom:8px;'>Check-in time</td>";
            }
            if (passengerInfo.Count == 1)
            {
                strContent = strContent.Replace("[occupancy]", "Single Occupancy");
            }
            else
            {
                strContent = strContent.Replace("[occupancy]", "Double Occupancy/Sharing");
            }

            if (passengerInfo[0].HotelType == "Non-Contractual")
            {
                HotelPriceString = "<td style='height:20px; padding-bottom:8px;'>Price</td>";
            }

            var mobilenostring = "<td height:20px; padding-bottom:8px;'>Mobile No.</td></tr>";
            if (mailtype == "user")
            {
                HotelPriceString = "";
                mobilenostring = "";
            }

            var tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;font-weight:bold;'>" +
              "<td style='height:20px; padding-bottom:8px;'>S. No.</td>" +
              "<td style='height:20px; padding-bottom:8px;'>Gender</td>" +
              "<td style='height:20px; padding-bottom:8px;'>Employee Name</td>" +
              "<td style='height:20px; padding-bottom:8px;'>EmpId</td>" +
               "<td style='height:20px; padding-bottom:8px;'>Check-In Date</td>" +
               "<td style='height:20px; padding-bottom:8px;'>Check-Out Date</td>" +
               "<td style='height:20px; padding-bottom:8px;'>HeadOffice/Airport Transfers Required</td>" +
               "<td style='height:20px; padding-bottom:8px;'>Flight No.</td>" +
                ETAString +
                       HotelPriceString +
                       mobilenostring;


            foreach (var p in passengerInfo)
            {
                var hotelpriceNon_cont = "";
                if (p.HotelType == "Non-Contractual")
                {
                    hotelpriceNon_cont = "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.HotelPrice;
                }
                if (p.Title == "Ms.")
                {
                    gender = "F";
                }
                else
                {
                    gender = "M";
                }
                if (p.accomodationRequired == true)
                {
                    airtransport = "Yes";
                }
                else
                {
                    airtransport = "No";
                }
                var ETACheckin = "";
                if (sodOat == "SOD")
                {
                    if (p.IsCabRequiredAsPerETA == true)
                    {
                        ETACheckin = p.ArrivalTime;
                    }
                    else
                    {
                        ETACheckin = p.CabPickupTime;
                    }
                }
                else
                {
                    ETACheckin = p.CheckinTime;
                }

                var phonevalue = "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.Phone;
                if (mailtype == "user")
                {
                    hotelpriceNon_cont = "";
                    phonevalue = "";
                }

                tr = tr + "<tr style='font-family:Arial; font-size:12px;'>" +
                        "<td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + count +
                        "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + gender +
                        "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.RequestedEmpName +
                        "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.RequestedEmpId +
                        "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.CheckinDate.ToString("dd-MMM-yyyy") +
                        "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.CheckoutDate.ToString("dd-MMM-yyyy") +
                        "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + airtransport +
                        "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.FlightNo +
                        "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + ETACheckin +
                        hotelpriceNon_cont +
                        phonevalue +
                                "</td></tr>";
                count++;
            }
            strContent = strContent.Replace("[tr]", tr);


            return strContent.ToString();
        }


        public void GetHotelRequestData(List<String> requestList, List<String> hidList, string hotelname, string sodOat)
        {
            var dicList = new Dictionary<string, object>();
            dicList = _transportRepository.GetHotelandUserInfo(requestList, hidList, hotelname, sodOat);
            TempData["PassengerInfoForHotelRequest"] = dicList["PassengerInfoForHotelRequest"] as List<TravelRequestModels>;

        }

        /// <summary>
        /// check if city code exists or not
        /// </summary>
        /// <param name="citycode"></param>
        /// <returns></returns>
        public JsonResult CityCodeCheck(string citycode)
        {
            var citylist = new List<SodCityCodeMasterModels>();
            citylist = _transportRepository.checkCityCodeExist(citycode);
            if (citylist == null || citylist.Count < 1)
            {
                return Json("NotExist", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(citylist[0].CityCode, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Update hotel dates by user
        /// </summary>
        /// <param name="TravelRequestId"></param>
        /// <param name="hotelid"></param>
        /// <param name="checkin"></param>
        /// <param name="checkout"></param>
        /// <returns></returns>
        public JsonResult updateHotelDates(string TravelRequestId, string hotelid, string checkin, string checkout)
        {
            try
            {
                //var year1 = Convert.ToInt32(checkin.Split('-')[2].ToString());
                //var monthstr1 = checkin.Split('-')[1].ToString();
                //var day1 = Convert.ToInt32(checkin.Split('-')[0].ToString());

                //var year2 = Convert.ToInt32(checkout.Split('-')[2].ToString());
                //var monthstr2 = checkout.Split('-')[1].ToString();
                //var day2 = Convert.ToInt32(checkout.Split('-')[0].ToString());

                //var month1 = 0;
                //var month2 = 0;
                //String[] strMonthNames = new String[12] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
                //for (var i = 0; i < strMonthNames.Length;i++ )
                //{
                //    if (strMonthNames[i] == monthstr1)
                //    {
                //        month1 = i + 1;
                //    }
                //    if (strMonthNames[i] == monthstr2)
                //    {
                //        month2 = i + 1;
                //    }
                //}
                var checkinDate = Convert.ToDateTime(checkin);
                var checkoutDate = Convert.ToDateTime(checkout);
                var update = _transportRepository.updateHotelDatesByUser(TravelRequestId, hotelid, checkinDate, checkoutDate);
                if (update > 0)
                {
                    return Json("Done", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Warning: Dates cannot be updated as request has been already sent to hotel. Please contact TravelDesk to send cancellation request to existing hotel. Once this process has been completed, please try again to update the dates so that TravelDesk can Re-book the hotel for you.", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json("Error: Invalid request processing...", JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult UndoCancelledRequest(string TravelRequestId)
        {
            var trid = Convert.ToInt64(TravelRequestId.Split('|')[0]);
            var hid = TravelRequestId.Split('|')[1];
            var type = TravelRequestId.Split('|')[2];

            var hotelList = new List<TravelRequestHotelDetailModels>();
            var dicList = new Dictionary<string, object>();
            dicList = _transportRepository.GetSodHotelInfo(Convert.ToInt64(trid), Convert.ToInt32(hid));
            hotelList = dicList["hotelinfolist"] as List<TravelRequestHotelDetailModels>;
            var isAllocated = hotelList[0].IsAllocated;
            var HotelStatus = hotelList[0].HotelStatus;

            var status = "";
            if (HotelStatus == "Approved by Hotel")
            {
                status = "Allocated";
            }
            var s = _transportRepository.UndoCancelledRequest(trid, Convert.ToInt32(hid), status, type);
            if (s > 0)
            {
                return Json("Undo Successful. Contact TravelDesk to Send/Resend request to hotel.", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Error: Invalid request processing...", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Send SMS for Approval
        /// </summary>
        /// <param name="reqId"></param>
        /// <param name="paxName"></param>
        /// <param name="approvalURI"></param>
        public void SendSMSforApproval(string hoddetails, TravelRequestMasterModels sodRequest, List<FlightDetailModels> flightInfo, string approvalURI, string rejectionURI)
        {
            StringBuilder strFlight = new StringBuilder();
            var counter = 1;
            foreach (var flist in flightInfo)
            {
                strFlight.Append(flist.OriginPlace + "-" + flist.DestinationPlace);
                if (flightInfo.Count > 1 && flightInfo.Count != counter)
                    strFlight.Append(",");
                counter++;
            }
            var bookingfor = sodRequest.IsHotelRequired.Equals(true) ? "Flight (" + sodRequest.BookingFor + ") & Hotel booking " : "Only Flight booking (" + sodRequest.BookingFor + ")";
            var smsText = ConfigurationManager.AppSettings["smsApprovalFlight"].ToString().Replace("@Hodname", hoddetails.Split(',')[1]);
            smsText = smsText.Replace("@ReqId", sodRequest.TravelRequestId.ToString());
            smsText = smsText.Replace("@TravelDate", flightInfo[0].TravelDate.ToString("dd/MMM/yyyy") + " " + flightInfo[0].DepartureTime);
            smsText = smsText.Replace("@Sector", strFlight.ToString());
            smsText = smsText.Replace("@BookigFor", bookingfor);
            smsText = smsText.Replace("@PaxName", sodRequest.Title + " " + sodRequest.RequestedEmpName);
            smsText = smsText.Replace("[AppQueryString]", approvalURI);
            smsText = smsText.Replace("[RejQueryString]", rejectionURI);
            smsText = System.Uri.EscapeDataString(smsText);
            smsText = smsText.Replace("25", "");
            //Send SMS
            var smsLogModel = new SodApproverSMSLogModels();
            smsLogModel.TrRequestId = sodRequest.TravelRequestId;
            smsLogModel.EmpCode = hoddetails.Split(',')[4];
            smsLogModel.EmpName = hoddetails.Split(',')[1];
            smsLogModel.EmailID = hoddetails.Split(',')[0];
            smsLogModel.MobileNo = hoddetails.Split(',')[3];
            smsLogModel.Source = "SOD";
            smsLogModel.SMSText = smsText;
            smsLogModel.DeliveryDate = DateTime.Now;
            var status = SmsNotification.SmsNotifications.SendSmsViaApi(smsText, hoddetails.Split(',')[3]);
            smsLogModel.IsDelivered = status.Equals(true) ? true : false;
            _userRepository.SaveApproverSMSLog(smsLogModel);
        }

        [HttpPost]
        public ActionResult AddPassengers(List<VendorModels> addlist)
        {
            foreach (var data in addlist)
            {
                TempData["FirstName"] = data.FirstName;
                TempData["LastName"] = data.LastName;
                TempData["EmailId"] = data.EmailId;
                TempData["MobileNo"] = data.MobileNo;
            }
            return Redirect("EmpBookingDetail.cshtml");
        }
        /// <summary>
        /// Exception Handling
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            Logging.ErrorLog.Instance.AddDBLogging(ex, filterContext.Controller.ControllerContext.ToString(), "Controllers/EmployeeBookingDetailController.cs");
        }
    }
}