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


namespace SOD.Controllers
{
    [HandleError(ExceptionType = typeof(DbUpdateException), View = "Error")]
    public class VDBookingDetailController : Controller, IActionFilter, IExceptionFilter
    {

        /// <summary>
        /// Constructor Initialization
        /// </summary>
        private readonly IUserRepository _userRepository;
        private IVendorRepository _vendorRepository;
        public string strHoldPNR = ConfigurationManager.AppSettings["strHOLDPNR"].Trim();
        public static string strHoldPNRHOD = ConfigurationManager.AppSettings["strHOLDPNRRequest"].Trim();

        public VDBookingDetailController()
        {
            _vendorRepository = new VendorRepository(new SodEntities());
            _userRepository = new UserRepository(new SodEntities());
        }

        /// <summary>
        /// EmployeeBookingDetail View
        /// </summary>
        /// <returns></returns>
        public ActionResult VDBookingDetail(List<VendorModels> addlist)
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
                return RedirectToAction("../nsvdflightbooking/SearchFlight");
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
            TempData["Passengers"] = sodRequestsList[0].Passengers;
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
        public JsonResult SubmitBookingInfo(List<TravelRequestMasterModels> sodRequestsList, List<FlightDetailModels> sodflightList, List<PassengerDetailModels> passengerList, List<PassengerMealAllocationModels> passengerMealsList, List<TravelRequestCabDetailModels> cabDetailList, List<TravelRequestHotelDetailModels> hotelDetailList)
        {
            var jsonmsg = string.Empty;
            var BookingType = string.Empty;
            var empInfo = TempData["EmpInfo"] as List<string>;
            if (empInfo == null)
            {
                return Json("Error : Please follow again the same process to book the ticket.", JsonRequestBehavior.AllowGet);
            }
            //Update Existing Login Emp Info  
            BookingType = "NON-SOD";
            sodRequestsList[0].RequestedEmpName = empInfo[1];
            sodRequestsList[0].RequestedEmpId = Convert.ToInt32(empInfo[0]);
            sodRequestsList[0].RequestedEmpCode = empInfo[9];
            sodRequestsList[0].RequestedEmpDept = empInfo[2];
            sodRequestsList[0].RequestedEmpDesignation = empInfo[5];
            sodRequestsList[0].RequestDate = DateTime.Now;
            sodRequestsList[0].BookingStatus = "Open";
            sodRequestsList[0].StatusDate = DateTime.Parse("01/01/1900");
            sodRequestsList[0].EmailId = empInfo[3];
            //sodRequestsList[0].Phno = empInfo[4];
            sodRequestsList[0].Title = empInfo[8] == "M" ? "Mr." : "Ms.";
            sodRequestsList[0].IsOKtoBoard = CommonWebMethod.OKToBoard.CheckIsOTB(sodRequestsList[0].TravelRequestTypeId, sodflightList);
            sodRequestsList[0].IsVendorBooking = true;
            sodRequestsList[0].IsSJSC = false;
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
                if (sodRequestsList[0].BookingFor.Trim().ToLower() == "confirm" || sodRequestsList[0].BookingFor.Trim().ToLower() == "standby")
                {
                    //Generate PNR For Blanket Approver Booking
                    var pnr = NavitaireServices.Generate_PNR(sodRequestsList, sodflightList, passengerList, passengerMealsList);
                    if (pnr == "ER001")
                    {
                        pnr = pnr + "|0.00|Error";
                        _userRepository.UpdatePnr(pnr, s);
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
                            var pnrc = "Your PNR Number is : " + pnr.Split('|')[0];
                            jsonmsg = BookingType + " " + "Vendor booking process has been completed successfully. Your PNR No. : " + pnr.Split('|')[0];
                            if (sodRequestsList[0].IsAmountPaidByTraveller.Equals(true))
                            {
                                //Get PNR Booking Time
                                var pnrInfo = _userRepository.GetPNRAmountAndTime(sodRequestsList[0].TravelRequestId);
                                var holdAmount = float.Parse(pnrInfo.Split('|')[1]);
                                var holdAmounts = string.Format("{0:0.00}", holdAmount).ToString();
                                var pnrGenerationTime = Convert.ToDateTime(pnrInfo.Split('|')[2]);
                                //Get Hold Time
                                var holdDateTime = CommonWebMethods.GetHoldBookingDateTime(sodRequestsList[0].SodBookingTypeId, sodRequestsList[0].BookingFor, sodflightList[0].TravelDate, sodflightList[0].DepartureTime, pnrGenerationTime);
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
                            var emailSubject = BookingType + " " + "Vendor Booking Request Notification :" + System.DateTime.Now.ToString();
                            var emailTemplateName = "SodBookingRequestNotificationTemplateFor_HOD_Booking.html";
                            var emailCredentials = EmailCredentials(emailSubject, emailTemplateName, sodRequestsList, sodflightList, passengerList, passengerMealsList, hotelDetailList, s.ToString(), "");
                            emailCredentials.TemplateFilePath = emailCredentials.TemplateFilePath.Replace("[PNR]", pnrc);
                            //Email details for background notifications
                            TempData["emailData"] = emailCredentials;
                            TempData["emailId"] = sodRequestsList[0].EmailId.Trim();
                            //Redirect to Thank You Page
                            jsonmsg = BookingType + "-" + sodRequestsList[0].TravelRequestId + "  : Vendor Booking process has been completed successfully. Your PNR No. : " + pnr.Split('|')[0];
                            if (sodRequestsList[0].IsAmountPaidByTraveller.Equals(true))
                            {
                                jsonmsg = jsonmsg + ".\n" + ConfigurationManager.AppSettings["strHOLDPNRPassenger"].Trim();
                            }
                        }
                    }
                }
            }
            TempData["jsonmsg"] = jsonmsg;
            return Json(s >= 1 ? jsonmsg : "Error :Booking Process fail.Please contact to administrator", JsonRequestBehavior.AllowGet);
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
            var empInfo = new List<string> { s[0].EmpId.ToString().Trim(), s[0].EmpName.Trim(), s[0].Department.Trim(),s[0].Email.Trim(), s[0].Phone.Trim(),s[0].Designation.Trim(), s[0].DesignationId.ToString().Trim(),
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
            var file = new System.IO.StreamReader(
                       Server.MapPath("~/Template/" + emailTemplateName));
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
            var btype = sodRequestsList[0].SodBookingTypeId.Equals(1) ? "SOD" : "NON-SOD";
            var tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Reason for Travel</td><td>Booking Type</td><td>No.of Passengers</td><td>Booking For</td>";
            strContent = strContent.Replace("[mtaText]", "");

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
            if (sodRequestsList[0].IsOKtoBoard.Equals(true))
                tr = tr + "<td> Yes </td>";
            tr = tr + "</tr>";
            strContent = strContent.Replace("[tr]", tr);

            //Begin of Passengers List----------------------------------------------------------------------------------------------------------------
            var i = 0;
            var trp = string.Empty;
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
                                    trp = trp + "<td style='border-top:1px solid #c2c2c2'> " + p.Title + " " + p.TravelerFirstName + " " + p.TravelerLastName + "</td>";
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
            trf = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Sector</td><td>Travel Date</td><td>Flight No.</td><td>Departure Time</td><td>Arrival Time</td></tr>";
            foreach (var f in sodflightList)
            {
                var sector = f.OriginPlace + "-" + f.DestinationPlace;
                trf = trf + "<tr style='font-family:Arial; font-size:12px;'><td style='border-top:1px solid #c2c2c2'>" + sector + "</td><td style='border-top:1px solid #c2c2c2'>" + f.TravelDate.ToString("dd-MMM-yyyy") + "</td><td style='border-top:1px solid #c2c2c2'>" + f.FlightNo + "</td><td style='border-top:1px solid #c2c2c2'>" + f.DepartureTime + "</td><td style='border-top:1px solid #c2c2c2'>" + f.ArrivalTime + "</td></tr>";
            }

            //-EOD Flight List--------------------------------------------------------------------------------------------------------------------
            strContent = strContent.Replace("[trf]", trf);
            //-EOD Flight List--------------------------------------------------------------------------------------------------------------------                      

            strContent = strContent.Replace("[hinfo]", String.Empty);
            strContent = strContent.Replace("[trh]", String.Empty);

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
            ViewBag.Message = String.Format("Sorry ! Your session has been expired.Please click on the NON-SOD link again.");
            return View();
        }

        [HttpPost]
        public JsonResult AddPassengers(List<VendorModels> addlist)
        {
            TempData["passangerDetail"] = addlist;
            return Json(addlist, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult getPassengerList()
        {
            var passenegerList = TempData["passangerDetail"] as List<VendorModels>;
            return Json(passenegerList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Exception Handling
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            Logging.ErrorLog.Instance.AddDBLogging(ex, filterContext.Controller.ControllerContext.ToString(), "Controllers/VDBookingDetailController.cs");
        }
    }
}