using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;
using SOD.Services.Repository;
using System.Text;
using SOD.Model;
using System.Configuration;
using SOD.EmailNotification;
using SOD.CommonWebMethod;
using System.Threading.Tasks;

namespace SOD.Controllers
{
    public class RevenueController : Controller, IActionFilter, IExceptionFilter
    {
        /// <summary>
        /// Constructor Initialization
        /// </summary>
        private readonly IRevenueRepository _revenueRepository;
        private readonly ISodApproverRepositorty _sodApproverRepositorty;
        private readonly IUserRepository _userRepository;

        public RevenueController()
        {
            _revenueRepository = new RevenueRepository(new SodEntities());
            _sodApproverRepositorty = new SodApproverRepositorty(new SodEntities());
            _userRepository = new UserRepository(new SodEntities());
        }


        /// <summary>
        /// Get Booking List View
        /// </summary>
        /// <returns></returns>
        public ActionResult BookingList()
        {
            if (Session["EmpId"] != null)
            {
                SodBookingList(Convert.ToInt32(Session["DepartmentId"]), Convert.ToInt32(Session["DesignationId"]));
            }
            return View();
        }


        /// <summary>
        /// Get Sod booking List 
        /// </summary>
        private void SodBookingList(int deptartmentId, int designationId)
        {
            var s = _revenueRepository.GetSodBookingListForApproval(deptartmentId, designationId, 0, 2);
            TempData["GetBookingList"] = s;
        }


        /// <summary>
        /// Get Sod Booking Data
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetBookingData()
        {
            return TempData["GetBookingList"] != null
                ? Json(TempData["GetBookingList"], JsonRequestBehavior.AllowGet)
                : Json("0", JsonRequestBehavior.AllowGet);
        }



        /// Get Employee booking details 
        /// Booking Request wise
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetBookingInfo()
        {
            var trId = Request.QueryString["trId"].ToString();
            //Display Request wise view details
            var sodRequestsList = new List<TravelRequestMasterModels>();
            var sodflightList = new List<FlightDetailModels>();
            var passengerList = new List<PassengerDetailModels>();
            var passengerMealsList = new List<PassengerMealAllocationModels>();
            var hotelList = new List<TravelRequestHotelDetailModels>();

            var dicList = new Dictionary<string, object>();
            dicList = _sodApproverRepositorty.GetSodBookingInfoForPNR(Convert.ToInt64(trId));
            sodRequestsList = dicList["bookingInfo"] as List<TravelRequestMasterModels>;
            sodflightList = dicList["flightInfo"] as List<FlightDetailModels>;
            passengerList = dicList["passInfo"] as List<PassengerDetailModels>;
            passengerMealsList = dicList["mealsInfo"] as List<PassengerMealAllocationModels>;
            hotelList = dicList["hotelInfo"] as List<TravelRequestHotelDetailModels>;
            //Check Meal Option from Old allocation
            if (passengerMealsList.Count == 0)
            {
                //Add Passenger meal Option for SOD Flights
                foreach (var item in sodflightList)
                {
                    var passengerMealsListItem = new PassengerMealAllocationModels();
                    passengerMealsListItem.TravelRequestId = item.TravelRequestId;
                    passengerMealsListItem.Sector = item.OriginPlace + "-" + item.DestinationPlace;
                    passengerMealsListItem.MealType = sodRequestsList[0].Meals;
                    passengerMealsListItem.PassengerId = 0;

                    passengerMealsList.Add(passengerMealsListItem);
                }
            }

            var fcount = 0;
            foreach (var item in sodflightList)
            {
                fcount++;
                var sector = item.OriginPlace + "-" + item.DestinationPlace;
                foreach (var m in passengerMealsList)
                {
                    if (m.Sector == sector && fcount == m.PassengerId)
                    {
                        item.Meals = m.MealType;
                    }
                    if (m.Sector == sector && m.PassengerId == 0)
                    {
                        if (item.Meals != null)
                            item.Meals = item.Meals + "," + m.MealType;
                        else
                            item.Meals = m.MealType;
                    }
                }
            }

            return Json(dicList, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Get Employee History
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetEmployeeBookingHistory(int EmpId)
        {
            var emId = Convert.ToInt32(Request.QueryString["EmpId"].ToString());
            var s = _sodApproverRepositorty.GetSodEmployeeBookingHistoryList(0, emId, 2);
            return Json(s, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Get Employee History Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetEmployeeBookingHistoryDetails(int bookingRequestId)
        {
            var trId = Convert.ToInt64(Request.QueryString["bookingRequestId"].ToString());
            var dicList = new Dictionary<string, object>();
            dicList = _sodApproverRepositorty.GetSodBookingInfoForPNR(trId);
            return Json(dicList, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Approve User Booking Request 
        /// </summary>
        [HttpPost]
        public JsonResult ApproveSodBookingRequest(string travelReqId)
        {
            var jsonmsg = string.Empty;
            var s = 0;
            var reqType = travelReqId.Split(',')[2].Trim().ToUpper();
            var bookingfor = travelReqId.Split(',')[3].Trim();
            TempData["requestData"] = travelReqId;

            //Check Duplicate PNR  
            var chkpnr = Services.ADO.SodCommonServices.CheckDuplicatePNR(Convert.ToInt64(travelReqId.Split(',')[0]), 1);
            if (!chkpnr.Equals("0"))
            {
                return Json("Sorry : PNR has been already generated for this Booking Request ID." + travelReqId.Split(',')[0] + "Existing PNR No. :" + chkpnr, JsonRequestBehavior.AllowGet);
            }
            //Initialize list for updating revenue status
            var approvalList = new TravelRequestApprovalModels()
            {
                TravelRequestId = Convert.ToInt64(travelReqId.Split(',')[0]),
                ApprovedByEmpId = Convert.ToInt32(Session["EmpId"].ToString()),
                RevenueApprovedStatus = 1,
                RevenueApprovedDate = System.DateTime.Now,
                Comment = "Approved from Revenue Dept."
            };

            ////Non-Sod Confirm booking
            if (reqType.Equals("NON-SOD") && bookingfor.Equals("Confirm"))
            {
                //Update Revenue Status & Send to C-Level
                s = _sodApproverRepositorty.UpdateSodBookingRequest_Revenue(approvalList);
                if (s >= 1)
                    jsonmsg = "SOD Booking Request ID :" + reqType + "-" + travelReqId.Split(',')[0] + "\n Booking Type :Confirm \n\n Approval Process has been completed successfully.\n For the final PNR approval Request has been sent to CXO panel.";
                else
                    jsonmsg = "Error : Error found in Request Processing.";
            }
            return Json(s >= 1 ? jsonmsg : string.Empty, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Send Email Notification & generate PNR for booking requester/user
        /// </summary>
        /// <returns></returns>
        public string BackgroundProcessNotification()
        {
            var jsonmsg = string.Empty;
            //Sod Confirm booking
            var travelReqId = TempData["requestData"].ToString();
            if (travelReqId.Split(',')[2].Equals("SOD") && travelReqId.Split(',')[3].Equals("Confirm"))
            {
                var pnrStatus = GeneratePNR(Convert.ToInt64(travelReqId.Split(',')[0]), travelReqId.Split(',')[1].Trim());
                if (pnrStatus.Length.Equals(6))
                    jsonmsg = "SOD Booking Request ID :" + travelReqId.Split(',')[2].ToUpper() + "-" + travelReqId.Split(',')[0] + "\n Booking Type :Confirm \n\n Approval Process has been completed successfully.\n PNR No :" + pnrStatus;
            }
            else
            {
                //Send Email Notification to One CXO  :23-Nov-2016
                //Get Data to show for rejection
                var strHoldPNR = "";
                var sodRequestsList = new List<TravelRequestMasterModels>();
                var sodflightList = new List<FlightDetailModels>();
                var passengerList = new List<PassengerDetailModels>();
                var approvalInfoList = new List<TravelRequestApprovalModels>();
                var passengerMealsList = new List<PassengerMealAllocationModels>();
                var hotelList = new List<TravelRequestHotelDetailModels>();
                var dicList = new Dictionary<string, object>();

                //Fill Value in List
                dicList = _sodApproverRepositorty.GetSodBookingInfoForPNR(Convert.ToInt64(travelReqId.Split(',')[0]));
                sodRequestsList = dicList["bookingInfo"] as List<TravelRequestMasterModels>;
                sodflightList = dicList["flightInfo"] as List<FlightDetailModels>;
                passengerList = dicList["passInfo"] as List<PassengerDetailModels>;
                approvalInfoList = dicList["approvalInfo"] as List<TravelRequestApprovalModels>;
                passengerMealsList = dicList["mealsInfo"] as List<PassengerMealAllocationModels>;
                hotelList = dicList["hotelInfo"] as List<TravelRequestHotelDetailModels>;
                var bookingType = sodRequestsList[0].SodBookingTypeId == 1 ? "SOD" : "NON-SOD";

                //Allocate Email Template
                var emailSubject = bookingType + " " + "Booking Request Notification Acceptance through Revenue and Itinerary :" + System.DateTime.Now.ToString("dd-MMM-yyyy-hh:mm:ss");
                var emailTemplateName = "SodBookingRequestNotificationTemplate_HOD.html";

                //Send Notification to one C-Level:  
                var strcxodetails = _sodApproverRepositorty.GetCXOMailId(sodRequestsList[0].CLevelApprover1, sodRequestsList[0].CLevelApprover2);
                var emailid = strcxodetails.Split('|')[1];
                var cxoEmpId = strcxodetails.Split('|')[0];

                var skey = new StringBuilder();
                skey.Append(sodRequestsList[0].TravelRequestId.ToString() + ",");
                skey.Append(sodRequestsList[0].EmailId.Trim() + ",");
                skey.Append(sodRequestsList[0].SodBookingTypeId.ToString() + ",");
                skey.Append(sodRequestsList[0].BookingFor.Trim() + ",");
                skey.Append(cxoEmpId + ",");
                skey.Append(sodRequestsList[0].IsMandatoryTravel.ToString() + ",RY");

                //passing querystring
                var uri1 = ConfigurationManager.AppSettings["emailApprovalPathcxo"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=a");
                var uri2 = ConfigurationManager.AppSettings["emailApprovalPathcxo"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=r");

                var approvaltype = "Please help to accord your Acceptance or Rejection.";
                var appLink = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a name='app' style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Acceptance</a></td> <td>&nbsp;</td> <td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a name='rej' style='color:#fff; text-decoration:none;' href='" + uri2 + "'>Rejection</a> </td></tr></table>";

                //Get Hold booking Status
                if (sodRequestsList[0].IsAmountPaidByTraveller.Equals(true))
                    strHoldPNR = ConfigurationManager.AppSettings["strHOLDPNRPassenger"].Trim();
                else
                    strHoldPNR = "";

                //Wrap link in template
                var emailCredentials = EmailCredentials(emailSubject, emailTemplateName, sodRequestsList, sodflightList, passengerList, passengerMealsList, hotelList, travelReqId.Split(',')[0].ToString(), strHoldPNR);
                var templateData = emailCredentials.TemplateFilePath;
                var hodName = "Sir/Madam";

                templateData = templateData.Replace("[approvaltype]", approvaltype);
                templateData = templateData.Replace("[appLink]", appLink);
                templateData = templateData.Replace("[hodName]", hodName);
                emailCredentials.TemplateFilePath = templateData;

                //Send email notification
                EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials, emailid);
                jsonmsg = "SOD Booking Request ID :" + travelReqId.Split(',')[2].ToUpper() + "-" + travelReqId.Split(',')[0] + "\n Booking Type :Confirm \n\n Approval Process has been completed successfully.\n The booking request has been sent to CXO Panel at -" + emailid;
            }
            return jsonmsg;
        }


        /// <summary>
        /// Reject Sod Booking Request
        /// </summary>
        /// <returns></returns>
        public JsonResult RejectSodBookingRequest(string travelReqId)
        {
            var jsonmsg = string.Empty;
            var bookingType = string.Empty;
            var strHoldPNR = string.Empty;
            if ((travelReqId.Split(',')[0] == "undefined") || (travelReqId.Split(',')[0] == ""))
            {
                return Json("Invalid record.", JsonRequestBehavior.AllowGet);
            }
            var approvalList = new TravelRequestApprovalModels()
            {
                TravelRequestId = Convert.ToInt64(travelReqId.Split(',')[0]),
                ApprovedByEmpId = Convert.ToInt32(Session["EmpId"].ToString()),
                RevenueApprovedStatus = 2,//for reject
                RevenueApprovedDate = System.DateTime.Now,
                Comment = travelReqId.Split(',')[4]
            };
            //Check Duplicate PNR  
            var chkpnr = Services.ADO.SodCommonServices.CheckDuplicatePNR(Convert.ToInt64(travelReqId.Split(',')[0]), 1);
            if (!chkpnr.Equals("0"))
            {
                return Json("Sorry : PNR has been already generated for this Booking Request ID-" + travelReqId.Split(',')[0] + ".Rejection is not allowed.\n Existing PNR No. :" + chkpnr, JsonRequestBehavior.AllowGet);
            }

            //Get Data to show for rejection
            var sodRequestsList = new List<TravelRequestMasterModels>();
            var sodflightList = new List<FlightDetailModels>();
            var passengerList = new List<PassengerDetailModels>();
            var approvalInfoList = new List<TravelRequestApprovalModels>();
            var passengerMealsList = new List<PassengerMealAllocationModels>();
            var hotelList = new List<TravelRequestHotelDetailModels>();
            var s = _sodApproverRepositorty.RejectSodBookingRequest_Revenue(approvalList);
            if (s >= 1)
            {
                var dicList = new Dictionary<string, object>();
                dicList = _sodApproverRepositorty.GetSodBookingInfoForPNR(Convert.ToInt64(travelReqId.Split(',')[0]));
                sodRequestsList = dicList["bookingInfo"] as List<TravelRequestMasterModels>;
                sodflightList = dicList["flightInfo"] as List<FlightDetailModels>;
                passengerList = dicList["passInfo"] as List<PassengerDetailModels>;
                approvalInfoList = dicList["approvalInfo"] as List<TravelRequestApprovalModels>;
                passengerMealsList = dicList["mealsInfo"] as List<PassengerMealAllocationModels>;
                hotelList = dicList["hotelInfo"] as List<TravelRequestHotelDetailModels>;
                bookingType = sodRequestsList[0].SodBookingTypeId == 1 ? "SOD" : "NON-SOD";

                //by lata (to gnerate standby PNR and send Email notification
                if (travelReqId.Split(',')[5] == "1")
                {
                    sodRequestsList[0].BookingFor = "standby";
                    var pnr = NavitaireServices.Generate_PNR(sodRequestsList, sodflightList, passengerList, passengerMealsList);
                    pnr = pnr + "|0001.11";
                    var pnrc = "Your Standby PNR is :" + pnr.Split('|')[0];
                    //insert value in sodtravelrequestmaster_reject with standbyPNR
                    SaveRejectwithStandByPNR(sodRequestsList[0], pnr);

                    sendNotification_RejectStandby(sodRequestsList, sodflightList, passengerList, passengerMealsList, hotelList, travelReqId.Split(',')[0], pnrc);
                }
                //Hold PNR 
                if (sodRequestsList[0].IsAmountPaidByTraveller.Equals(true))
                     strHoldPNR = ConfigurationManager.AppSettings["strHOLDPNRPassenger"].Trim();
                else
                    strHoldPNR = "";
                //Send Rejection Email Notification
                var emailSubject = bookingType + " " + "Booking Request Notification Rejection through Revenue and Itinerary :" + System.DateTime.Now.ToString("dd-MMM-yyyy-hh:mm:ss");
                var emailTemplateName = "SodBookingRequest_Rejection_User.html";
                var emailCredentials = EmailCredentials(emailSubject, emailTemplateName, sodRequestsList, sodflightList, passengerList, passengerMealsList, hotelList, travelReqId.Split(',')[0].ToString(), strHoldPNR);

                if (approvalInfoList[0].IsMandatoryTravel == 1)
                    emailCredentials.TemplateFilePath = emailCredentials.TemplateFilePath.Replace("[Team]", "Revenue Dept. and forwarded to C-Level Approval.");
                else if (approvalInfoList[0].IsMandatoryTravel == 0)
                    emailCredentials.TemplateFilePath = emailCredentials.TemplateFilePath.Replace("[Team]", "Revenue Dept. and the booking request has been closed.");

                TempData["emailData"] = emailCredentials;
                TempData["emailId"] = sodRequestsList[0].EmailId.Trim();

                //Check If Mandatory for send email
                if (approvalInfoList[0].IsMandatoryTravel == 1)
                {
                    emailSubject = bookingType + " " + "Booking Request Notification Rejection through Revenue and Itinerary :" + System.DateTime.Now.ToString("dd-MMM-yyyy-hh:mm:ss");
                    emailTemplateName = "SodBookingRequestNotificationTemplate_HOD.html";

                    //Send Notification to C-Level: Need C- Level Approval Email Template
                    var counter = 0;
                    var emailids = _sodApproverRepositorty.GetCXOMailId(sodRequestsList[0].CLevelApprover1, sodRequestsList[0].CLevelApprover2);
                    string[] arr = emailids.Split('|');

                    if (arr.Length == 2)
                        counter = 1;
                    else if (arr.Length == 4)
                        counter = 2;

                    for (int i = 0; i <= counter; i++)
                    {
                        var skey = new StringBuilder();
                        skey.Append(sodRequestsList[0].TravelRequestId.ToString() + ",");
                        skey.Append(sodRequestsList[0].EmailId.Trim() + ",");
                        skey.Append(sodRequestsList[0].SodBookingTypeId.ToString() + ",");
                        skey.Append(sodRequestsList[0].BookingFor.Trim() + ",");
                        skey.Append(arr[i] + ",");
                        skey.Append(sodRequestsList[0].IsMandatoryTravel.ToString() + ",RN");

                        var uri1 = ConfigurationManager.AppSettings["emailApprovalPathcxo"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=a");
                        var uri2 = ConfigurationManager.AppSettings["emailApprovalPathcxo"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=r");

                        var approvaltype = "Please help to accord your Acceptance or Rejection.";
                        var appLink = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a name='app' style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Acceptance</a></td> <td>&nbsp;</td> <td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a name='rej' style='color:#fff; text-decoration:none;' href='" + uri2 + "'>Rejection</a> </td></tr></table>";

                        //Wrap link in template
                        emailCredentials = EmailCredentials(emailSubject, emailTemplateName, sodRequestsList, sodflightList, passengerList, passengerMealsList, hotelList, travelReqId.Split(',')[0].ToString(), strHoldPNR);
                        var templateData = emailCredentials.TemplateFilePath;
                        var hodName = "Sir/Madam";

                        templateData = templateData.Replace("[approvaltype]", approvaltype);
                        templateData = templateData.Replace("[appLink]", appLink);
                        templateData = templateData.Replace("[hodName]", hodName);
                        emailCredentials.TemplateFilePath = templateData;
                        i = i + 1;
                        EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials, arr[i]);
                    }
                    jsonmsg = bookingType + " " + "booking request has been rejected and has been sent to CXO Panel.";
                }
                else if (approvalInfoList[0].IsMandatoryTravel == 0)
                {
                    //This methos also can be used by Revenue dept to close the Booking Request
                    var trRequestMaster = new TravelRequestMasterModels()
                    {
                        TravelRequestId = Convert.ToInt64(travelReqId.Split(',')[0]),
                        BookingStatus = "Close",
                        StatusDate = System.DateTime.Now
                    };
                    var c = _sodApproverRepositorty.CloseSodBookingRequest_HOD(trRequestMaster);
                    jsonmsg = bookingType + " " + "booking request has been rejected and the request has been closed.";
                }
            }
            return Json(s >= 1 ? jsonmsg : string.Empty, JsonRequestBehavior.AllowGet);
        }


        public string sendNotification_RejectStandby(List<TravelRequestMasterModels> sodRequestsList, List<FlightDetailModels> sodflightList, List<PassengerDetailModels> passengerList, List<PassengerMealAllocationModels> passengerMealsList, List<TravelRequestHotelDetailModels> hotelDetailList, string s, string pnrc)
        {
            var bookingType = sodRequestsList[0].SodBookingTypeId == 1 ? "SOD" : "NON-SOD";
            if (sodRequestsList[0].IsAmountPaidByTraveller.Equals(true))
            {
                var strHoldPNR = string.Empty;
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
            var emailSubject = bookingType + " " + "Standby Booking PNR Request Notification :" + System.DateTime.Now.ToString();
            var emailTemplateName = "SodBookingRequestNotificationTemplateFor_HOD_Booking.html";
            var emailCredentials = EmailCredentials(emailSubject, emailTemplateName, sodRequestsList, sodflightList, passengerList, passengerMealsList, hotelDetailList, s, pnrc);
            emailCredentials.TemplateFilePath = emailCredentials.TemplateFilePath.Replace("[PNR]", pnrc);
            EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials, sodRequestsList[0].EmailId);
            return "sucess";
        }
        /// <summary>
        /// Save Data in Sod ravel Request Mster-Standby NPR Model
        /// If Revenue say "No with SandBy"
        /// </summary>
        /// <param name="trMasterList"></param>
        /// <param name="strPNR"></param>
        /// <returns></returns>
        public bool SaveRejectwithStandByPNR(TravelRequestMasterModels sodRequestsList, string pnr)
        {
            var objRejectList = new TravelRequestMasterModels_RejectwithStandByPNR
            {
                TravelRequestId = sodRequestsList.TravelRequestId,
                BookingFor = sodRequestsList.BookingFor,
                RequestedEmpId = sodRequestsList.RequestedEmpId,
                RequestedEmpCode = sodRequestsList.RequestedEmpCode,
                Title = sodRequestsList.Title,
                RequestedEmpName = sodRequestsList.RequestedEmpName,
                Email = sodRequestsList.EmailId,
                MobileNo = sodRequestsList.Phno,
                Pnr = pnr.Split('|')[0],
                PnrAmount = Convert.ToDecimal(pnr.Split('|')[1]),
                PNRCreateddate = DateTime.Now,
                PNRCreatedby = "Revenue Dept.",
                IsAmountPaidByTraveller = sodRequestsList.IsAmountPaidByTraveller,
                IsHotelRequired = sodRequestsList.IsHotelRequired,
                IsOKtoBoard = sodRequestsList.IsOKtoBoard,
                HotelApproval = sodRequestsList.HotelApproval,
                SJSCHodEmailId = sodRequestsList.SJSCHodEmailId,
                IsVendorBooking = sodRequestsList.IsVendorBooking,
                IsSJSC = sodRequestsList.IsSJSC
            };
            var s = _sodApproverRepositorty.SaveRejectwithStandByPNR_Revenue(objRejectList);
            return true;
        }

        /// <summary>
        /// Send Email Notification to user :Rejection
        /// </summary>
        /// <returns></returns>
        public string sendRejectionNotification()
        {
            var jmsg = string.Empty;
            if (TempData["emailData"] != null && TempData["emailId"] != null)
            {
                var emaildata = TempData["emailData"] as EmailNotificationModel;
                var emailid = TempData["emailId"].ToString();
                EmailNotifications.SendBookingRequestNotificationTo_Requester(emaildata, emailid);
                jmsg = "Rejected";
            }
            return jmsg;
        }


        /// <summary>
        /// Generate PNR and Send Emal Notification
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <param name="emailId"></param>
        /// <returns></returns>
        private string GeneratePNR(Int64 travelReqId, string emailId)
        {
            //Generate PNR For Approver or HOD
            string strHoldPNR = ConfigurationManager.AppSettings["strHOLDPNR"].Trim();
            var sodRequestsList = new List<TravelRequestMasterModels>();
            var sodflightList = new List<FlightDetailModels>();
            var passengerList = new List<PassengerDetailModels>();
            var passengerMealsList = new List<PassengerMealAllocationModels>();
            var hotelList = new List<TravelRequestHotelDetailModels>();

            var dicList = new Dictionary<string, object>();
            dicList = _sodApproverRepositorty.GetSodBookingInfoForPNR(travelReqId);
            sodRequestsList = dicList["bookingInfo"] as List<TravelRequestMasterModels>;
            sodflightList = dicList["flightInfo"] as List<FlightDetailModels>;
            passengerList = dicList["passInfo"] as List<PassengerDetailModels>;
            passengerMealsList = dicList["mealsInfo"] as List<PassengerMealAllocationModels>;
            hotelList = dicList["hotelInfo"] as List<TravelRequestHotelDetailModels>;

            var reqType = sodRequestsList[0].SodBookingTypeId == 1 ? "SOD" : "NON-SOD";

            //Check Meal Option from Old allocation
            if (passengerMealsList.Count == 0)
            {
                //Add Passenger meal Option for SOD Flights
                foreach (var item in sodflightList)
                {
                    var passengerMealsListItem = new PassengerMealAllocationModels();
                    passengerMealsListItem.TravelRequestId = item.TravelRequestId;
                    passengerMealsListItem.Sector = item.OriginPlace + "-" + item.DestinationPlace;
                    passengerMealsListItem.MealType = sodRequestsList[0].Meals;
                    passengerMealsListItem.PassengerId = 0;
                    passengerMealsList.Add(passengerMealsListItem);
                }
            }
            var pnr = NavitaireServices.Generate_PNR(sodRequestsList, sodflightList, passengerList, passengerMealsList);
            pnr = pnr + "|Close";
            //Update in database
            var c = _sodApproverRepositorty.UpdatePnr(pnr, travelReqId);
            if (c > 0)
            {
                var pnrc = pnr.Split('|')[0];
                if (sodRequestsList[0].IsAmountPaidByTraveller.Equals(true))
                {
                    //Get PNR Booking Time
                    var pnrInfo = _sodApproverRepositorty.GetPNRAmountAndTime(sodRequestsList[0].TravelRequestId);
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

                //Send Email Notification
                var emailSubject = reqType + " " + "Booking Request Notification Acceptance through Revenue and Itinerary :" + System.DateTime.Now.ToString("dd-MMM-yyyy-hh:mm:ss");
                var emailTemplateName_User = "SodConfirm_Acceptance through Revenue and Itinerary.html";
                var emailCredentials = EmailCredentials(emailSubject, emailTemplateName_User, sodRequestsList, sodflightList, passengerList, passengerMealsList, hotelList, travelReqId.ToString(), "");
                emailCredentials.TemplateFilePath = emailCredentials.TemplateFilePath.Replace("[PNR]", pnrc);
                EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials, sodRequestsList[0].EmailId.Trim());
            }

            return pnr.Split('|')[0].Trim().ToString();
        }


        /// <summary>
        /// Initialized Email Credentials to Property
        /// </summary>
        /// <returns></returns>
        public EmailNotificationModel EmailCredentials(string subjectName, string emailTemplateName, List<TravelRequestMasterModels> sodRequestsList, List<FlightDetailModels> sodflightList, List<PassengerDetailModels> sodPassList, List<PassengerMealAllocationModels> sodPassMealsList, List<TravelRequestHotelDetailModels> hotelList, string reqNo, string strHoldPNRMessage)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtpUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtpPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtpServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtpPort"].Trim(),
                TemplateFilePath = ReadFile(emailTemplateName, sodRequestsList, sodflightList, sodPassList, sodPassMealsList, hotelList, reqNo, strHoldPNRMessage),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }


        /// <summary>
        /// Read Template File from Location
        /// </summary>
        /// <returns></returns>
        private string ReadFile(string emailTemplateName, List<TravelRequestMasterModels> sodRequestsList, List<FlightDetailModels> sodflightList, List<PassengerDetailModels> sodPassList, List<PassengerMealAllocationModels> sodPassMealsList, List<TravelRequestHotelDetailModels> hotelList, string reqNo, string strHoldPNRMessage)
        {
            var strContent = new StringBuilder();
            string line;
            var file = new System.IO.StreamReader(
                       Server.MapPath("~/Template/" + emailTemplateName));
            while ((line = file.ReadLine()) != null)
            {
                strContent.Append(line);
                //Replace code here..
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
                //strContent = strContent.Replace("[mtaText]", "Reason for Mandatory Travel Request  : " + sodRequestsList[0].ReasonForMandatoryTravel);//Kiran Sir
                strContent = strContent.Replace("[mtaText]", "");
            }
            else
            {
                strContent = strContent.Replace("[mtaText]", "");
            }

            //Ok to Board
            if (sodRequestsList[0].IsOKtoBoard.Equals(true))
                tr = tr + "<td>Ok to Board</td>";

            //For Hold PNR
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
                    trp = trp + "<td style='border-top:1px solid #c2c2c2'>" + sector + "</td>";
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

            if (sodRequestsList[0].IsHotelRequired == true)
            {
                var trh = "";
                if (btype == "SOD")
                {
                    trh = "<table cellpadding='0' cellspacing='0' style='width:100%; font-size:12px;border:solid 1px #c2c2c2;line-height:30px;'>";
                    trh = trh + "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>City</td><td>Check-In Date</td><td>Check-Out Date</td><td>No. Of Passenger</td></tr>";
                    foreach (var h in hotelList)
                    {
                        //var sector = f.OriginPlace + "-" + f.DestinationPlace;
                        trh = trh + "<tr style='font-family:Arial; font-size:12px;'><td style='border-top:1px solid #c2c2c2'>" + h.City + "</td><td style='border-top:1px solid #c2c2c2'>" + h.CheckInDate.ToString("dd-MMM-yyyy") + "</td><td style='border-top:1px solid #c2c2c2'>" + h.CheckOutDate.ToString("dd-MMM-yyyy") + "</td><td style='border-top:1px solid #c2c2c2'>" + h.NoOfGuest + "</td>";
                        trh = trh + "</tr>";
                    }
                    trh = trh + "</table>";
                }
                strContent = strContent.Replace("[trh]", trh);
                strContent = strContent.Replace("[hinfo]", "<table cellpadding='0' cellspacing='0' style='width:100%; border:0px;'><tr><td style='font-size: 16px;font-family:Arial;margin-top: 15px;margin-bottom:20px; border-right:solid 0px transparent;'><p>Hotel Information</p> </td></tr></table>");
            }
            else
            {
                strContent = strContent.Replace("[hinfo]", String.Empty);
                strContent = strContent.Replace("[trh]", String.Empty);
            }

            strContent = strContent.Replace("[trf]", trf);
            strContent = strContent.Replace("[trml]", mltr == "" ? "" : mltr);
            return strContent.ToString();
        }


        /// <summary>
        /// Action Filter Method
        /// </summary>
        /// <param name="filterContext"></param>
        void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
        {
            //If Required : Need to Implement
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
        private void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            Logging.ErrorLog.Instance.AddDBLogging(ex, filterContext.Controller.ControllerContext.ToString(), "Controllers/revenueController.cs");
        }
    }
}