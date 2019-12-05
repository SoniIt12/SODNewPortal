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
    public class SJSCFlightBookingController : Controller, IActionFilter, IExceptionFilter
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

        public SJSCFlightBookingController()
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
        public ActionResult SJSCFlightBooking()
        {
            if (TempData["Passengers"] != null)
            {
                ViewBag.Passengers = TempData["Passengers"].ToString();
                ViewBag.TravelRequestTypeId = TempData["TravelRequestTypeId"];
                ViewBag.SodBookingType = TempData["SodBookingType"].ToString();
                ViewBag.BookingFor = TempData["BookingFor"].ToString();
                ViewBag.Destination = TempData["DestinationName"].ToString();
                ViewBag.RequestListCount = TempData["RequestListCount"].ToString();
                ViewBag.bevDepRight = ConfigurationManager.AppSettings["BvgRight_SCSC"].ToString();
                ViewBag.EmpId = Session["EmpId"];
                ViewBag.EmpCode = Session["EmpCode"];
                ViewBag.Name = Session["FirstName"] + " " + Session["LastName"];
                ViewBag.Department = Session["DeptIdCR"];
                ViewBag.Email = Session["Email"];
                ViewBag.Phone = Session["Phone"];
                ViewBag.Designation = Session["Designation"];
                if (TempData["TravelRequestTypeId"].ToString() == "3")
                {
                    ViewBag.DestinationList = TempData["DestinationList"] as List<String>;
                }
                if (TempData["ReturnDateRoundTrip"] != null)
                {
                    ViewBag.ReturnDateRoundTrip = TempData["ReturnDateRoundTrip"].ToString();
                }
                ViewBag.CXOList = _userRepository.GetCXO_ApproverList(Convert.ToInt16(TempData["DeptId"]), Convert.ToInt16(TempData["SodBookingType"]));
                return View();
            }
            else
            {
                return RedirectToAction("../sjsisBooking/flight");
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
            ViewBag.bevDepRight = 1;
            if (sodRequestsList == null)
            {
                s = "Error : Flight information not available.";
                return Json(s, JsonRequestBehavior.AllowGet);
            }
            TempData["flightBookingInfo"] = sodRequestsList;
            TempData["flightDateTimeInfo"] = sodRequestsList[0].TravelDate + " " + sodRequestsList[0].DepartureTime;
            s = "Flight details received successfully.";
            return Json(s, JsonRequestBehavior.AllowGet);
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
                //Pass Traveller Info
                BookingType = "SOD";
                passengerList = new List<PassengerDetailModels>();
                var fullname = sodRequestsList[0].RequestedEmpName.Split(' ');
                passengerList.Add(new PassengerDetailModels
                {
                    Title = Session["Gender"].ToString() == "M" ? "Mr" : "Ms",
                    TravelerFirstName = fullname[0],
                    TravelerLastName = fullname[1],
                    TravelerGender = Session["Gender"].ToString()
                });
                sodRequestsList[0].IsVendorBooking = false;
                sodRequestsList[0].IsSJSC = true;
                sodRequestsList[0].RequestDate = DateTime.Now;
                sodRequestsList[0].BookingStatus = "Open";
                sodRequestsList[0].StatusDate = DateTime.Parse("01/01/1900");
                passengerMealsList = new List<PassengerMealAllocationModels>();
                foreach (var item in sodflightList)
                {
                    var passengerMealsListItem = new PassengerMealAllocationModels
                    {
                        TravelRequestId = item.TravelRequestId,
                        Sector = item.OriginPlace + "-" + item.DestinationPlace,
                        MealType = item.Meals,
                        PassengerId = 0
                    };
                    passengerMealsList.Add(passengerMealsListItem);
                }

                //Add Beverage Option for SOD Flights
                foreach (var item in sodflightList)
                {
                    if (item.Beverages != null)
                        if (item.Beverages.ToLower().Trim() != "not required")
                        {
                            var passengerMealsListItem = new PassengerMealAllocationModels
                            {
                                TravelRequestId = item.TravelRequestId,
                                Sector = item.OriginPlace + "-" + item.DestinationPlace,
                                MealType = item.Beverages,
                                PassengerId = 0
                            };
                            passengerMealsList.Add(passengerMealsListItem);
                        }
                }
                sodRequestsList[0].SJSCHodEmailId = Session["HodEmailId"].ToString();
                sodRequestsList[0].Title = Session["Gender"].ToString() == "M" ? "Mr" : "Ms";
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
                    bool b = Convert.ToBoolean(Session["IsApprover"]);
                    var hodEmailId = _sJsisConcernRepository.GetHodDetails(Session["HodEmailId"].ToString());
                    if (sodRequestsList[0].BookingFor.Trim().ToLower() == "confirm")
                    {
                        if (b == true)
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
                                    jsonmsg = BookingType + " " + "SJSC" + " " + "booking process has been completed successfully. Your PNR No. :" + pnr.Split('|')[0];
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
                            var emailSubject = BookingType + " " + "SJSC" + " " + "Confirm Booking Request Notification :" + System.DateTime.Now.ToString();
                            var emailTemplateName_User = "SodConfirmBookingRequestNotificationTemplate_User.html";
                            var emailCredentials = EmailCredentials(emailSubject, emailTemplateName_User, sodRequestsList, sodflightList, passengerList, passengerMealsList, hotelDetailList, s.ToString(), strHoldPNR);
                            TempData["emailData"] = emailCredentials;
                            TempData["emailId"] = sodRequestsList[0].EmailId.Trim();

                            //Email Notification to HOD Process
                            //Update Hod Email Template
                            var emailTemplateName_hod = "SodBookingRequestNotificationTemplate_HOD.html";
                            emailCredentials = EmailCredentials(emailSubject, emailTemplateName_hod, sodRequestsList, sodflightList, passengerList, passengerMealsList, hotelDetailList, s.ToString(), strHoldPNR);
                            //Wrap link in template
                            var templateData = emailCredentials.TemplateFilePath;
                            var appLink = string.Empty;
                            var approvaltype = string.Empty;
                            if (hodEmailId.Count > 0)
                            {
                                var skey = new StringBuilder();
                                skey.Append(sodRequestsList[0].TravelRequestId.ToString() + ",");
                                skey.Append(sodRequestsList[0].EmailId.Trim() + ",");
                                skey.Append(sodRequestsList[0].SodBookingTypeId.ToString() + ",");
                                skey.Append(sodRequestsList[0].BookingFor.Trim() + ",");
                               skey.Append("0" + ",");
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
                                templateData = templateData.Replace("[hodName]", hodEmailId[0].HoDTitle + " " + hodEmailId[0].HoDName);
                                emailCredentials.TemplateFilePath = templateData;                              
                             
                                //send email notification
                                TempData["emailData_Hod"] = emailCredentials;
                                TempData["emailId_Hod"] = hodEmailId[0].HodEmail;

                                //SMS Approval 
                                SendSMSforApproval(hodEmailId[0], sodRequestsList[0], sodflightList, uri1, uri2);
                            }
                            //Redirect to Thank You Page
                            jsonmsg = BookingType + "-" + "SJSC-" + s.ToString() + " " + "booking process has been completed successfully.Your booking request has been sent to your respected approver at  " + hodEmailId[0].HodEmail;
                            if (sodRequestsList[0].IsAmountPaidByTraveller.Equals(true))
                            {
                                jsonmsg = jsonmsg + ".\n" + strHoldPNR;
                            }
                        }
                    }
                    else
                    {
                        var r = Session["HodEmailId"].ToString().Trim().Equals(Session["SjsUserId"].ToString().Trim()) ? true : false;
                        if (r == true && b == true)
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
                            var emailSubject = BookingType + " " + "SJSC" + " " + "Standby Booking Request Notification :" + System.DateTime.Now.ToString();
                            var emailTemplateName = "SodStandbyBookingRequestNotificationTemplate_User.html";
                            var emailCredentials = EmailCredentials(emailSubject, emailTemplateName, sodRequestsList, sodflightList, passengerList, passengerMealsList, hotelDetailList, s.ToString(), strHoldPNR);
                            TempData["emailData"] = emailCredentials;
                            TempData["emailId"] = sodRequestsList[0].EmailId.Trim();                            

                            //Update Hod Email Template
                            var emailTemplateName_hod = "SodBookingRequestNotificationTemplate_HOD.html";
                            emailCredentials = EmailCredentials(emailSubject, emailTemplateName_hod, sodRequestsList, sodflightList, passengerList, passengerMealsList, hotelDetailList, s.ToString(), strHoldPNR);

                            var approvaltype = string.Empty;
                            if (hodEmailId.Count > 0)
                            {
                                var skey = new StringBuilder();
                                skey.Append(sodRequestsList[0].TravelRequestId.ToString() + ",");
                                skey.Append(sodRequestsList[0].EmailId.Trim() + ",");
                                skey.Append(sodRequestsList[0].SodBookingTypeId.ToString() + ",");
                                skey.Append(sodRequestsList[0].BookingFor.Trim() + ",");
                                skey.Append("0" + ",");
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
                                var hodName = hodEmailId[0].HoDTitle + " " + hodEmailId[0].HoDName;
                                templateData = templateData.Replace("[approvaltype]", approvaltype);
                                templateData = templateData.Replace("[appLink]", appLink);
                                templateData = templateData.Replace("[hodName]", hodName);
                                emailCredentials.TemplateFilePath = templateData;

                                //Redirect to Thank You Page
                                jsonmsg = BookingType + "-" + "SJSC-" + s.ToString() + " " + "Standby booking request process has been completed successfully.Your booking request has been sent to your respected approver at  " + hodEmailId[0].HodEmail;
                                if (sodRequestsList[0].IsAmountPaidByTraveller.Equals(true))
                                {
                                    jsonmsg = jsonmsg + ".\n" + strHoldPNR;
                                }
                                TempData["emailData_Hod"] = emailCredentials;
                                TempData["emailId_Hod"] = hodEmailId[0].HodEmail;

                                //SMS Approval 
                                SendSMSforApproval(hodEmailId[0], sodRequestsList[0], sodflightList, uri1, uri2);
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
            ViewBag.bevDepRight = 1;
            var s = TempData["flightBookingInfo"];
            return Json(s, JsonRequestBehavior.AllowGet);
        }
        public SJSCUserMasterModels GetUserData(string userId)
        {
            var userdata = new SJSCUserMasterModels();
            userdata = _sJsisConcernRepository.GetUserData(userId);
            return userdata;
        }        

        /// <summary>
        /// Sod Booking Type Manage Here
        /// </summary>
        public enum SodType
        {
            SodBookingType = 1,
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
        /// On OnActionExecuting
        /// </summary>
        /// <param name="filterContext"></param>
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["SjsUserId"] == null)
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
        /// Send SMS for Approval
        /// </summary>
        /// <param name="reqId"></param>
        /// <param name="paxName"></param>
        /// <param name="approvalURI"></param>
        public void SendSMSforApproval(HodDetails hoddetails, TravelRequestMasterModels sodRequest, List<FlightDetailModels> flightInfo, string approvalURI, string rejectionURI)
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
            var smsText = ConfigurationManager.AppSettings["smsApprovalFlight"].ToString().Replace("@Hodname", hoddetails.HoDName);
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
            var smsLogModel = new SodApproverSMSLogModels
            {
                TrRequestId = sodRequest.TravelRequestId,
                EmpCode = sodRequest.RequestedEmpCode,//Need to check as of now HOD Emp Code is not available
                EmpName = hoddetails.HoDName,
                EmailID = hoddetails.HodEmail,
                MobileNo = hoddetails.HodMobileNo,
                Source = "SOD-SJSC",
                SMSText = smsText,
                DeliveryDate = DateTime.Now
            };
            var status = SmsNotification.SmsNotifications.SendSmsViaApi(smsText, hoddetails.HodMobileNo);
            smsLogModel.IsDelivered = status.Equals(true) ? true : false;
            _userRepository.SaveApproverSMSLog(smsLogModel);
        }

        /// <summary>
        /// Exception Handling
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            Logging.ErrorLog.Instance.AddDBLogging(ex, filterContext.Controller.ControllerContext.ToString(), "Controllers/SJSCFlightBookingController.cs");
        }
    }
}