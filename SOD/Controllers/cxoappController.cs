using SOD.CommonWebMethod;
using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;
using SOD.Services.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SOD.EmailNotification;

namespace SOD.Controllers
{
    public class cxoappController : Controller,IActionFilter,IExceptionFilter
    {

        /// <summary>
        /// Constructor Initialization : For Repository Design Pattern
        /// </summary>
        private readonly ISodApproverRepositorty _sodApproverRepositorty;
        public readonly SodApproverController _SodApproverController;
        public readonly UserRepository _userRepository;

        public cxoappController()
        {
            _sodApproverRepositorty = new SodApproverRepositorty(new SodEntities());
            _SodApproverController = new SodApproverController();
            _userRepository = new UserRepository(new SodEntities());
       }

        /// <summary>
        /// Get User Booking List for Approval
        /// </summary>
        /// <returns></returns>
        public ActionResult GetBookingList()
        {
            if (Request.QueryString["dep"] != null && Request.QueryString["deg"] != null)
            {
                var deptartmentId = Convert.ToInt32(Request.QueryString["dep"].ToString().Trim());
                var designationId = Convert.ToInt32(Request.QueryString["deg"].ToString().Trim());
                Session["DepartmentId"] = deptartmentId;
                Session["DesignationId"] = designationId;
                SodBookingList(deptartmentId, designationId);
            }
            return View();

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

       
        /// <summary>
        /// Get Sod booking List 
        /// </summary>
        private void SodBookingList(int deptartmentId, int designationId)
        {
            var s = _sodApproverRepositorty.GetSodBookingListForApproval(deptartmentId, designationId,Convert.ToInt32(Session["EmpId"].ToString()), 3,null);
            TempData["ApproverList"] = s;
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
        /// Approve User Booking Request 
        /// </summary>
        [HttpPost]
        public JsonResult ApproveSodBookingRequest(string travelReqId)
        {
            bool IsApp1=false;
            bool IsApp2=false;
            var jsonmsg = string.Empty;
            var deptartmentId = Convert.ToInt32(Session["DepartmentId"].ToString().Trim());
            var designationId = Convert.ToInt32( Session["DesignationId"].ToString().Trim());
            var verticalId = Session["VerticalId"].ToString().Trim();
            var pnrStatus = string.Empty;
            TempData["requestData"] = null;
            var bookingfor = travelReqId.Split(',')[3].Trim();
            var reqType = travelReqId.Split(',')[2].Trim();
            //Check Duplicate PNR  
            var chkpnr = Services.ADO.SodCommonServices.CheckDuplicatePNR(Convert.ToInt64(travelReqId.Split(',')[0]), 1);
            if (!chkpnr.Equals("0"))
            {
                return Json("PNR has been already generated for this Booking Request ID :" + travelReqId.Split(',')[0] + ".Existing PNR No. :" + chkpnr, JsonRequestBehavior.AllowGet);
            }
            //If CXO Having HOD right, It should initiate here....
            var r = _sodApproverRepositorty.IsHODRole(Convert.ToInt32(Session["EmpId"].ToString()),verticalId , deptartmentId);
            var Iscxo = _sodApproverRepositorty.IsCXORole(Convert.ToInt32(Session["EmpId"]));
            if (r == true || Iscxo==true)
            {
                jsonmsg = ApproveSodBookingRequestHODRole(travelReqId);
                if (jsonmsg == "HotelApproved")
                    return Json(jsonmsg, JsonRequestBehavior.AllowGet);
                if (jsonmsg == "Approved." && bookingfor.Equals("Standby"))
                {
                    TempData["requestData"] = travelReqId.Split(',')[0] + "," + travelReqId.Split(',')[1]+"NA";
                    return Json(jsonmsg, JsonRequestBehavior.AllowGet);
                }
                if (jsonmsg == "Approved." && bookingfor == "Confirm" && reqType == "SOD")
                {
                    TempData["requestData"] = travelReqId.Split(',')[0] + "," + travelReqId.Split(',')[1] + "NA";
                    return Json(jsonmsg, JsonRequestBehavior.AllowGet);
                }
                else if (reqType == "NON-SOD" && bookingfor == "Confirm")
                {
                    if (Iscxo)
                    {
                        //Check CXO1 Level 1 and Update
                        var cStatus = CheckCXOApprovalLevel_and_Update_the_Approval_Status(travelReqId.Split(',')[0], Session["EmpId"].ToString());
                        if (CheckCXOApprovalLevel(travelReqId.Split(',')[0]))
                        {
                            TempData["requestData"] = travelReqId.Split(',')[0] + "," + travelReqId.Split(',')[1] + "L1L2";
                            return Json("Approved.", JsonRequestBehavior.AllowGet);
                        }
                        //Manage revenue Rejection Status
                        ManageRevenueStatus_DefaultRejectionCase(travelReqId.Split(',')[0], Session["EmpId"].ToString());

                        //Fwd mail to CXO 2
                        var fwdCXOmailid = SendEmailtoAnotherCXO_for_NONSOD_Confirm_BookingApproval_andGenerate_PNR(travelReqId.Split(',')[0], cStatus);
                        jsonmsg = "Sod Booking Request ID: " + travelReqId.Split(',')[0] +
                         " - Approval process has been completed successfully.For final PNR approval the request has been forwarded to one more C-Level at " + fwdCXOmailid;
                    }
                }
            }
            else
            {
                if (reqType == "NON-SOD")
                {
                    //Check CXO1 Level 1 and Update
                    var cStatus = CheckCXOApprovalLevel_and_Update_the_Approval_Status(travelReqId.Split(',')[0], Session["EmpId"].ToString());
                    if (CheckCXOApprovalLevel(travelReqId.Split(',')[0]))
                    {
                        TempData["requestData"] = travelReqId.Split(',')[0] + "," + travelReqId.Split(',')[1] + "L1L2";
                        return Json("Approved.", JsonRequestBehavior.AllowGet);
                    }
                }
            }
          return Json(jsonmsg.Length > 1 ? jsonmsg : string.Empty, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Reject Sod Booking Request
        /// </summary>
        /// <returns></returns>
        public JsonResult RejectSodBookingRequest(string travelReqId)
        {
            var jsonmsg = string.Empty;
            var deptartmentId = Convert.ToInt32(Session["DepartmentId"].ToString().Trim());
            var designationId = Convert.ToInt32(Session["DesignationId"].ToString().Trim());
            var s = 0;
            if ((travelReqId.Split(',')[0] == "undefined") || (travelReqId.Split(',')[0] == ""))
            {
                return Json("Invalid record.", JsonRequestBehavior.AllowGet);
            }
            //Check Duplicate PNR  
            var chkpnr = Services.ADO.SodCommonServices.CheckDuplicatePNR(Convert.ToInt64(travelReqId.Split(',')[0]), 1);
            if (!chkpnr.Equals("0"))
            {
                return Json("PNR has been already generated for this Booking Request ID :" + travelReqId.Split(',')[0] + ". Existing PNR No. :" + chkpnr, JsonRequestBehavior.AllowGet);
            }
            //Get Employee Data to show in EMail
            var sodRequestsList = new List<TravelRequestMasterModels>();
            var sodflightList = new List<FlightDetailModels>();
            var passengerList = new List<PassengerDetailModels>();
            var passengerMealsList = new List<PassengerMealAllocationModels>();
            var hotelList = new List<TravelRequestHotelDetailModels>();
            var dicList = new Dictionary<string, object>();

            dicList = _sodApproverRepositorty.GetSodBookingInfoForPNR(Convert.ToInt64(travelReqId.Split(',')[0]));
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

            //Check Priority Level
            var cxoPriority = _sodApproverRepositorty.CheckCXOApprover_Priority(designationId, deptartmentId, Convert.ToInt64(travelReqId.Split(',')[0]));
            if (cxoPriority == 1)
            {
                var approvalList = new TravelRequestApprovalModels()
                {
                    TravelRequestId = Convert.ToInt64(travelReqId.Split(',')[0]),
                    ApprovedByEmpIdCLevel1 = Convert.ToInt32(Session["EmpId"].ToString()),
                    ApprovalStatusCLevel1 = 2,
                    CLevelAppDate1 = System.DateTime.Now,
                    CLevelComment1 = travelReqId.Split(',')[4].ToString()//  Rejected by CXO Priority 1"
                };
                s = _sodApproverRepositorty.UpdateSodBookingRequestCXO_Level1(approvalList);
            }
            else if (cxoPriority == 2)
            {
                var approvalList = new TravelRequestApprovalModels()
                {
                    TravelRequestId = Convert.ToInt64(travelReqId.Split(',')[0]),
                    ApprovedByEmpIdCLevel2 = Convert.ToInt32(Session["EmpId"].ToString()),
                    ApprovalStatusCLevel2 = 2,
                    CLevelAppDate2 = System.DateTime.Now,
                    CLevelComment2 =  travelReqId.Split(',')[4].ToString()//"Rejected by CXO Priority 2"
                };
                s = _sodApproverRepositorty.UpdateSodBookingRequestCXO_Level2(approvalList);
            }
            if (s>0)
            {
                //Close booking Request
                var trRequestMaster = new TravelRequestMasterModels()
                {
                    TravelRequestId = Convert.ToInt64(travelReqId.Split(',')[0]),
                    BookingStatus = "Close",
                    StatusDate = System.DateTime.Now
                };
                var c = _sodApproverRepositorty.CloseSodBookingRequest_HOD(trRequestMaster);

                //Get Hold booking Status
                var strHoldPNR = "";
                if (sodRequestsList[0].IsAmountPaidByTraveller.Equals(true))
                    strHoldPNR = ConfigurationManager.AppSettings["strHOLDPNRPassenger"].Trim();
                else
                    strHoldPNR = "";

                //Rejection Notification
                var emailSubject =reqType+" "+ "Booking Request Rejection Notification from CXO Level :" + System.DateTime.Now.ToString("dd-MMM-yyyy-hh:mm:ss");
                var emailTemplateName_User = "SodConfirm_Rejection through CXO-Level.html";
                var emailCredentials = EmailCredentials(emailSubject, emailTemplateName_User, sodRequestsList, sodflightList, passengerList, passengerMealsList, hotelList, travelReqId.Split(',')[0], strHoldPNR);

                TempData["emailData"] = emailCredentials;
                TempData["emailId"] = sodRequestsList[0].EmailId.Trim();
                jsonmsg = "Rejected.";
            }
            return Json(s >= 1 ? jsonmsg : string.Empty, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// Send Email Notification to user :Rejection
        /// </summary>
        /// <returns></returns>
        public async Task<string> sendRejectionNotification()
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
        /// Send Email Notification & generate PNR for booking requester/user
        /// </summary>
        /// <returns></returns>
        public async Task<string> BackgroundProcessNotification()
        {
            var jsonmsg = string.Empty;
            var pnrStatus = string.Empty;
            var travelReqId = string.Empty;
            if (TempData["requestData"] != null)
            {
                travelReqId = TempData["requestData"].ToString();
                pnrStatus =await GeneratePNR(Convert.ToInt64(travelReqId.Split(',')[0]), travelReqId.Split(',')[1]);
                if (pnrStatus == "ER001")
                {
                    jsonmsg = "Error Code :ER001.Please try after sometime.";
                    //Sod Confirm Booking L1 Priority
                    if (travelReqId.Split(',')[2] == "L1")
                    {
                        var approvalListrollback = new TravelRequestApprovalModels()
                        {
                            TravelRequestId = Convert.ToInt64(travelReqId.Split(',')[0]),
                            ApprovedByEmpIdCLevel1 = 0,
                            ApprovalStatusCLevel1 = 0,
                            CLevelComment1 = string.Empty,
                            CLevelAppDate1 = System.DateTime.Now
                        };
                        _sodApproverRepositorty.RollBackApprovalByCXO_Level1(approvalListrollback);
                    }
                    //Sod Confirm Booking L2 Priority
                    else if (travelReqId.Split(',')[2] == "L2")
                    {
                        var approvalListrollback = new TravelRequestApprovalModels()
                        {
                            TravelRequestId = Convert.ToInt64(travelReqId.Split(',')[0]),
                            ApprovedByEmpIdCLevel2 = 0,
                            ApprovalStatusCLevel2 = 0,
                            CLevelComment2 = string.Empty,
                            CLevelAppDate2 = System.DateTime.Now
                        };
                        _sodApproverRepositorty.RollBackApprovalByCXO_Level2(approvalListrollback);
                    }
                }
                else if (pnrStatus.Length.Equals(6))
                {
                    jsonmsg = "Approved.";
                }
            }
            return jsonmsg;
        }


        /// <summary>
        /// Generate PNR and Send Emal Notification
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <param name="emailId"></param>
        /// <returns></returns>
        private async Task<string> GeneratePNR(Int64 travelReqId, string emailId)
        {
            //Generate PNR For Approver or HOD
            string msg = string.Empty;
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
            if (passengerMealsList.Count() == 0)
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
            ////Validate PNR ERROR
            if (pnr == "ER001")
            {
                pnr = pnr + "|0.00|Error";
            }
            else
            {
                pnr = pnr + "|Close";
                //Update in database
                var c = _sodApproverRepositorty.UpdatePnr(pnr, travelReqId);
                if (c > 0)
                {
                    var pnrc = pnr.Split('|')[0].ToString();
                    var strHoldPNR = ConfigurationManager.AppSettings["strHOLDPNR"].Trim();
                    //Check Hold PNR
                    if (sodRequestsList[0].IsAmountPaidByTraveller.Equals(true))
                    {
                        //Get PNR Booking Time
                        var pnrInfo = _sodApproverRepositorty.GetPNRAmountAndTime(sodRequestsList[0].TravelRequestId);
                        var holdAmount = float.Parse(pnrInfo.Split('|')[1]);
                        var holdAmounts = string.Format("{0:0.00}", holdAmount).ToString();
                        var pnrGenerationTime = Convert.ToDateTime(pnrInfo.Split('|')[2]);

                        //Get Hold Time
                        var holdDateTime = CommonWebMethod.CommonWebMethods.GetHoldBookingDateTime(sodRequestsList[0].SodBookingTypeId, sodRequestsList[0].BookingFor,sodflightList[0].TravelDate, sodflightList[0].DepartureTime, pnrGenerationTime);
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
                    var emailSubject =reqType + " "+ "Booking Request Notification Acceptance through CXO-Level :" + System.DateTime.Now.ToString("dd-MMM-yyyy-hh:mm:ss");
                    var emailTemplateName_User = "SodConfirm_Acceptance through CXO-Level.html";
                    var emailCredentials = EmailCredentials(emailSubject, emailTemplateName_User, sodRequestsList, sodflightList, passengerList, passengerMealsList, hotelList, travelReqId.ToString(),"");
                    emailCredentials.TemplateFilePath = emailCredentials.TemplateFilePath.Replace("[PNR]", pnrc);
                    
                    //Replace [cxo] if required.
                    EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials, sodRequestsList[0].EmailId.Trim());
                }
            }
            return pnr.Split('|')[0].Trim().ToString();
        }


        /// <summary>
        /// Get Revenue comment
        /// </summary>
        /// <param name="reqId"></param>
        /// <returns></returns>
        public async Task<string> getRevenueComment(string reqId)
        {
            var str = _sodApproverRepositorty.GetRevenueComment(Convert.ToInt64(reqId));
            return str;
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
                tr = tr + "<td> Yes - "+sodRequestsList[0].ReasonForMandatoryTravel + "</td>";
            //Ok To Board
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
            if (Session["EmpId"] == null)
            {
                Response.Clear();
                CloseBookingList();
            }
        }



        /// <summary>
        /// Approve User Booking Request for HOD Right
        /// </summary>
        public string ApproveSodBookingRequestHODRole(string travelReqId)
        {
            var jsonmsg = string.Empty;
            var approvalList = new TravelRequestApprovalModels()
            {
                TravelRequestId = Convert.ToInt64(travelReqId.Split(',')[0]),
                ApprovedByEmpId = Convert.ToInt32(Session["EmpId"].ToString()),
                ApprovalStatus = 1,
                IsMandatoryTravel = Convert.ToInt16(travelReqId.Split(',')[4]),
                ApprovalDate = System.DateTime.Now,
                Comment = "Approved from HOD.",
                ApprovedByEmpIdCLevel1 = 0,
                ApprovedByEmpIdCLevel2 = 0,
                ApprovalStatusCLevel1 = 0,
                ApprovalStatusCLevel2 = 0,
                CLevelComment1 = "",
                CLevelComment2 = "",
                CLevelAppDate1 = DateTime.Parse("01/01/1900"),
                CLevelAppDate2 = DateTime.Parse("01/01/1900"),
                RevenueApprovedStatus = 0,
                RevenueApprovedDate = DateTime.Parse("01/01/1900")
            };

            //Check Duplicate Hotel  
            if (travelReqId.Split(',')[3].ToString().ToLower() == "onlyhotel")
            {
                var hreq = _sodApproverRepositorty.FindHotelApprovalStatus(Convert.ToInt64(travelReqId.Split(',')[0]));
                if (hreq == true)
                {
                    return "Request has been already approved.";
                }
                else
                {
                    _sodApproverRepositorty.UpdateOnlyHotelApprovalStatus(Convert.ToInt64(travelReqId.Split(',')[0]));
                    return "HotelApproved";
                }
            }
            else
            {   //Check Duplicate PNR  
                var chkpnr = Services.ADO.SodCommonServices.CheckDuplicatePNR(Convert.ToInt64(travelReqId.Split(',')[0]), 1);
                if (!chkpnr.Equals("0"))
                {
                    return "PNR has been already generated for this Booking Request ID :" + travelReqId.Split(',')[0] + ". Existing PNR No. :" + chkpnr;
                }
            }
            var s = _sodApproverRepositorty.ApproveSodBookingRequest(approvalList);
            if (s >= 1)
                jsonmsg = "Approved.";
            else
                jsonmsg = "Invalid Request.";

            return jsonmsg;
         }


        /// <summary>
        /// Default Rejection from Revenue Team if HOD Having CXO Right
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        private void ManageRevenueStatus_DefaultRejectionCase(string travelReqId, string ApprovedByEmpId)
        {
            var approvalList = new TravelRequestApprovalModels()
            {
                TravelRequestId = Convert.ToInt64(travelReqId.Split(',')[0]),
                ApprovedByEmpId = Convert.ToInt32(ApprovedByEmpId),
                RevenueApprovedStatus = 2,//for reject
                RevenueApprovedDate = System.DateTime.Now,
                Comment = "Rejected by Revenue Department."
            };
            var s = _sodApproverRepositorty.RejectSodBookingRequest_Revenue(approvalList);
        }


        /// <summary>
        /// Check CXO Level1 and CXO Level 2 for NON-SOD Confirm Booking
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        private bool CheckCXOApprovalLevel(string travelReqId)
        {
            bool IsApp1 = false;
            bool IsApp2 = false;
            IsApp1 = _sodApproverRepositorty.CheckSodApprovalStatusCXO_Level1(Convert.ToInt64(travelReqId));
            IsApp2 = _sodApproverRepositorty.CheckSodApprovalStatusCXO_Level2(Convert.ToInt64(travelReqId));
            if (!IsApp1 && !IsApp2)
                return true;
            else
                return false;

        }

        /// <summary>
        /// Check CXO Priority Level and Approve the request for generating the PNR
        /// </summary>
        /// <param name="travelReqId"></param>
        private string CheckCXOApprovalLevel_and_Update_the_Approval_Status(string travelReqId, string ApprovedByEmpId)
        {
            var approverInfo = _userRepository.GetEmployeeList(int.Parse(ApprovedByEmpId));
            bool IsApp1 = false;
            bool IsApp2 = false;
            var jsonmsg = string.Empty;
            //Check Priority Level
            var cxoPriority = _sodApproverRepositorty.CheckCXOApprover_Priority(approverInfo[0].DesignationId, approverInfo[0].DepartmentId, Convert.ToInt64(travelReqId));
            if (cxoPriority == 1)
            {
                IsApp1 = _sodApproverRepositorty.CheckSodApprovalStatusCXO_Level1(Convert.ToInt64(travelReqId));
                if (IsApp1)
                {
                    var appList1 = new TravelRequestApprovalModels()
                    {
                        TravelRequestId = Convert.ToInt64(travelReqId),
                        ApprovedByEmpIdCLevel1 = Convert.ToInt32(ApprovedByEmpId),
                        ApprovalStatusCLevel1 = 1,
                        CLevelComment1 = "Approved from CXO Priority 1",
                        CLevelAppDate1 = System.DateTime.Now
                    };
                    _sodApproverRepositorty.UpdateSodBookingRequestCXO_Level1(appList1);
                }
                jsonmsg = "CXO1";
            }
            else if (cxoPriority == 2)
            {
                IsApp2 = _sodApproverRepositorty.CheckSodApprovalStatusCXO_Level2(Convert.ToInt64(travelReqId));
                if (IsApp2)
                {
                    var appList2 = new TravelRequestApprovalModels()
                    {
                        TravelRequestId = Convert.ToInt64(travelReqId),
                        ApprovedByEmpIdCLevel2 = Convert.ToInt32(ApprovedByEmpId),
                        ApprovalStatusCLevel2 = 1,
                        CLevelComment2 = "Approved from CXO Priority 2",
                        CLevelAppDate2 = System.DateTime.Now
                    };
                    _sodApproverRepositorty.UpdateSodBookingRequestCXO_Level2(appList2);
                }
                jsonmsg = "CXO2";
            }
            return jsonmsg;
        }


        /// <summary>
        ///Send Email to another CXO for the NON-SOD Booking Approval and generate PNR
        /// </summary>
        /// <returns></returns>
        public string SendEmailtoAnotherCXO_for_NONSOD_Confirm_BookingApproval_andGenerate_PNR(string travelReqId, string CLevel)
        {
            var jsonmsg = string.Empty;
            var bookingType = string.Empty;
            var strHoldPNR = string.Empty;

            //Get Data to show for rejection
            var sodRequestsList = new List<TravelRequestMasterModels>();
            var sodflightList = new List<FlightDetailModels>();
            var passengerList = new List<PassengerDetailModels>();
            var approvalInfoList = new List<TravelRequestApprovalModels>();
            var passengerMealsList = new List<PassengerMealAllocationModels>();
            var hotelList = new List<TravelRequestHotelDetailModels>();
            var dicList = new Dictionary<string, object>();
            dicList = _sodApproverRepositorty.GetSodBookingInfoForPNR(Convert.ToInt64(travelReqId));
            sodRequestsList = dicList["bookingInfo"] as List<TravelRequestMasterModels>;
            sodflightList = dicList["flightInfo"] as List<FlightDetailModels>;
            passengerList = dicList["passInfo"] as List<PassengerDetailModels>;
            approvalInfoList = dicList["approvalInfo"] as List<TravelRequestApprovalModels>;
            passengerMealsList = dicList["mealsInfo"] as List<PassengerMealAllocationModels>;
            hotelList = dicList["hotelInfo"] as List<TravelRequestHotelDetailModels>;
            bookingType = sodRequestsList[0].SodBookingTypeId == 1 ? "SOD" : "NON-SOD";
            //Hold PNR 
            if (sodRequestsList[0].IsAmountPaidByTraveller.Equals(true))
                strHoldPNR = ConfigurationManager.AppSettings["strHOLDPNRPassenger"].Trim();
            else
                strHoldPNR = "";

            var emailSubject = bookingType + " " + "Booking Request Notification For Approval :" + System.DateTime.Now.ToString("dd-MMM-yyyy-hh:mm:ss");
            var emailTemplateName = "SodBookingRequestNotificationTemplate_HOD.html";
            var emailids = "";

            //Check C level to fwd email
            if (CLevel != "CXO1")
                emailids = _sodApproverRepositorty.GetCXOMailId(sodRequestsList[0].CLevelApprover1, "");
            else if (CLevel != "CXO2")
                emailids = _sodApproverRepositorty.GetCXOMailId(sodRequestsList[0].CLevelApprover2, "");

            string[] arr = emailids.Split('|');
            //Mail Body wrapper
            var skey = new StringBuilder();
            skey.Append(sodRequestsList[0].TravelRequestId.ToString() + ",");
            skey.Append(sodRequestsList[0].EmailId.Trim() + ",");
            skey.Append(sodRequestsList[0].SodBookingTypeId.ToString() + ",");
            skey.Append(sodRequestsList[0].BookingFor.Trim() + ",");
            skey.Append(arr[0] + ",");
            skey.Append(sodRequestsList[0].IsMandatoryTravel.ToString() + ",RN");

            var uri1 = ConfigurationManager.AppSettings["emailApprovalPathcxo"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=a");
            var uri2 = ConfigurationManager.AppSettings["emailApprovalPathcxo"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=r");

            var approvaltype = "Please help to accord your Acceptance or Rejection.";
            var appLink = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a name='app' style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Acceptance</a></td> <td>&nbsp;</td> <td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a name='rej' style='color:#fff; text-decoration:none;' href='" + uri2 + "'>Rejection</a> </td></tr></table>";

            //Wrap link in template
            var emailCredentials = EmailCredentials(emailSubject, emailTemplateName, sodRequestsList, sodflightList, passengerList, passengerMealsList, hotelList, travelReqId, strHoldPNR);
            var templateData = emailCredentials.TemplateFilePath;
            var hodName = "Sir/Madam";

            templateData = templateData.Replace("[approvaltype]", approvaltype);
            templateData = templateData.Replace("[appLink]", appLink);
            templateData = templateData.Replace("[hodName]", hodName);
            emailCredentials.TemplateFilePath = templateData;

            EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials, arr[1]);
            return arr[1].ToString();
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
        /// Exception Handling
        /// </summary>
        /// <param name="filterContext"></param>
        void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            Logging.ErrorLog.Instance.AddDBLogging(ex, filterContext.Controller.ControllerContext.ToString(), "Controllers/cxoappController.cs");
        }
    }
}