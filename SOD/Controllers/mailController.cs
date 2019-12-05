using SOD.CommonWebMethod;
using SOD.EmailNotification;
using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;
using SOD.Services.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SOD.Controllers
{
    public class mailController : Controller, IExceptionFilter
    {
        /// <summary>
        /// Constructor Initialization
        /// </summary>
        private readonly ISodApproverRepositorty _sodApproverRepositorty;
        private readonly IUserRepository _userRepository;
        private readonly ISjSisConcernRepository _sJsisConcernRepository;
        public mailController()
        {
            _sodApproverRepositorty = new SodApproverRepositorty(new SodEntities());
            _userRepository = new UserRepository(new SodEntities());
            _sJsisConcernRepository = new SjSisConcernRepository(new SodEntities());
        }


        /// <summary>
        /// Open View for HOD
        /// </summary>
        /// <returns></returns>
        public ActionResult app()
        {
            var strQuery = string.Empty;
            strQuery = Request.QueryString[0].ToString();
            var travelreqID = CipherURL.Decrypt(strQuery);
            if (travelreqID.Split('&')[1].Contains("a"))
                TempData["jsonmsg"] = ApproveSodBookingRequest(travelreqID.Split('&')[0]);
            else if (travelreqID.Split('&')[1].Contains("r"))
                TempData["jsonmsg"] = RejectSodBookingRequest(travelreqID.Split('&')[0]);

            return View();
        }


        /// <summary>
        /// Open View for CXO
        /// </summary>
        /// <returns></returns>
        public ActionResult appcxo()
        {
            var strQuery = string.Empty;
            strQuery = Request.QueryString[0].ToString();
            strQuery = CipherURL.Decrypt(strQuery);
            if (strQuery.Split('&')[1].Contains("a"))
                TempData["jsonmsg"] = ApproveSodBookingRequestToCXO(strQuery);
            else if (Request.QueryString[1].Trim().Equals("r"))
                TempData["jsonmsg"] = RejectSodBookingRequestToCXO(strQuery);

            return View("app");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult apphotel()
        {
            var strQuery = string.Empty;
            strQuery = Request.QueryString[0].ToString();
            if (strQuery.Split('&')[1].Contains("a"))
                TempData["jsonmsg"] = ApproveHotelBookingRequest(strQuery);
            else if (Request.QueryString[1].Trim().Equals("r"))
                TempData["jsonmsg"] = RejectHotelBookingRequest(strQuery);

            return View();
        }

        /// <summary>
        /// only Hotel approval
        /// </summary>
        /// <returns></returns>
        public ActionResult OnlyHotelApproval()
        {
            var strQuery = string.Empty;
            strQuery = Request.QueryString[0].ToString();
            var travelreqID = CipherURL.Decrypt(strQuery);
            if (travelreqID.Split('&')[1].Contains("a"))
                TempData["jsonmsg"] = ApproveOnlyHotelBookingRequest(travelreqID);
            else if (travelreqID.Split('&')[1].Contains("r"))
                TempData["jsonmsg"] = RejectHotelBookingRequest(travelreqID);

            return View();
        }

        /// <summary>
        /// Approve hotel booking request
        /// </summary>
        /// <param name="travelreqID"></param>
        /// <returns></returns>
        public string ApproveHotelBookingRequest(string travelreqID)
        {
            var controller = DependencyResolver.Current.GetService<HotelOnlyController>();
            string jsonmsg = "";
            var trid = Convert.ToInt64(travelreqID.Split(',')[0].ToString());
            var hreject = _sodApproverRepositorty.FindHotelRejectionStatus(trid);
            if (hreject == true)
            {
                return "Request cannot be approved. It has been already rejected.";
            }
            else
            {
                var s = _sodApproverRepositorty.FindHotelApprovalStatus(trid);
                if (s == true)
                {
                    return "Request has been already approved.";
                }
                else
                {
                    var n = ManageOnlyHotelBookingApprovalStatus(travelreqID, "A");
                    if (n > 0)
                    {
                        _sodApproverRepositorty.UpdateHotelApprovalStatus(trid);
                        var trRequestMaster = new TravelRequestMasterModels()
                        {
                            TravelRequestId = Convert.ToInt64(travelreqID.Split(',')[0]),
                            BookingStatus = "Close",
                            StatusDate = System.DateTime.Now
                        };
                        var c = _sodApproverRepositorty.CloseSodBookingRequest_HOD(trRequestMaster);

                        //Get Booking Request Data for Email Notification
                        var sodRequestsList = new List<TravelRequestMasterModels>();
                        var sodflightList = new List<FlightDetailModels>();
                        var sodPassList = new List<PassengerDetailModels>();
                        var sodPassMealsList = new List<PassengerMealAllocationModels>();
                        var hotelList = new List<TravelRequestHotelDetailModels>();
                        var dicList = new Dictionary<string, object>();

                        dicList = _sodApproverRepositorty.GetSodBookingInfoForPNR(trid);
                        sodRequestsList = dicList["bookingInfo"] as List<TravelRequestMasterModels>;
                        sodflightList = dicList["flightInfo"] as List<FlightDetailModels>;
                        sodPassList = dicList["passInfo"] as List<PassengerDetailModels>;
                        sodPassMealsList = dicList["mealsInfo"] as List<PassengerMealAllocationModels>;
                        hotelList = dicList["hotelInfo"] as List<TravelRequestHotelDetailModels>;
                        string bookingType = sodRequestsList[0].SodBookingTypeId == 1 ? "SOD" : "NON-SOD";


                        //Send Approval Email Notification
                        var emailSubject = bookingType + " " + "Booking Request Approval Notification from HOD :" + System.DateTime.Now.ToString("dd-MMM-yyyy-hh:mm:ss");
                        var emailTemplateName = "HotelBookingRequest_Approval_User.html";
                        var emailCredentials = controller.EmailCredentialsHotel(emailSubject, emailTemplateName, hotelList, sodRequestsList, sodflightList, trid.ToString(), "approved");
                        TempData["emailData"] = emailCredentials;
                        TempData["emailId"] = sodRequestsList[0].EmailId.Trim();
                        var emaildata = TempData["emailData"] as EmailNotificationModel;
                        var emailid = TempData["emailId"].ToString();
                        EmailNotifications.SendBookingRequestNotificationTo_Requester(emaildata, emailid);
                        jsonmsg = "Approval Alert for Booking Request : " + bookingType + "-" + travelreqID.Split(',')[0] + "\n Approval process has been completed successfully.";
                        return (n >= 1 ? jsonmsg : string.Empty);
                    }
                    else
                    {
                        return jsonmsg = "Oops! Something went wrong.";
                    }
                }
            }
        }


        /// <summary>
        /// Approve only hotel booking request
        /// </summary>
        /// <param name="travelreqID"></param>
        /// <returns></returns>
        public string ApproveOnlyHotelBookingRequest(string travelreqID)
        {
            //var travelreqID = CipherURL.Decrypt(ecryptedurl1);
            var controller = DependencyResolver.Current.GetService<HotelOnlyController>();
            string jsonmsg = "";
            string strHoldPNR = "";
            var trId = Convert.ToInt64(travelreqID.Split(',')[0]);
            var s = _sodApproverRepositorty.FindHotelApprovalStatus(trId);
            if (s == true)
            {
                jsonmsg = "Request has been already approved.";
            }
            else
            {
                var hreject = _sodApproverRepositorty.FindHotelRejectionStatus(trId);
                if (hreject == true)
                {
                    jsonmsg = "Request has been already Rejected.";
                }
                var n = ManageOnlyHotelBookingApprovalStatus(travelreqID, "A");
                if (n > 0)
                {
                    _sodApproverRepositorty.UpdateOnlyHotelApprovalStatus(trId);
                    //Close Booking Request
                    var trRequestMaster = new TravelRequestMasterModels()
                    {
                        TravelRequestId = Convert.ToInt64(travelreqID.Split(',')[0]),
                        BookingStatus = "Close",
                        StatusDate = System.DateTime.Now
                    };
                    var c = _sodApproverRepositorty.CloseSodBookingRequest_HOD(trRequestMaster);

                    //Get Booking Request Data for Email Notification
                    var sodRequestsList = new List<TravelRequestMasterModels>();
                    var sodflightList = new List<FlightDetailModels>();
                    var sodPassList = new List<PassengerDetailModels>();
                    var sodPassMealsList = new List<PassengerMealAllocationModels>();
                    var hotelList = new List<TravelRequestHotelDetailModels>();
                    var dicList = new Dictionary<string, object>();

                    dicList = _sodApproverRepositorty.GetSodBookingInfoForPNR(trId);
                    sodRequestsList = dicList["bookingInfo"] as List<TravelRequestMasterModels>;
                    sodflightList = dicList["flightInfo"] as List<FlightDetailModels>;
                    sodPassList = dicList["passInfo"] as List<PassengerDetailModels>;
                    sodPassMealsList = dicList["mealsInfo"] as List<PassengerMealAllocationModels>;
                    hotelList = dicList["hotelInfo"] as List<TravelRequestHotelDetailModels>;
                    string emailSubject = "";
                    string bookingType = "";
                    string emailTemplateName = "";
                    EmailNotificationModel emailCredentials = new EmailNotificationModel();
                    if (sodRequestsList[0].TravelRequestTypeId == 5)
                    {
                        bookingType = sodRequestsList[0].BookingFor;
                        emailSubject = bookingType + " " + "Booking Request Approval Notification from HOD :" + System.DateTime.Now.ToString("dd-MMM-yyyy-hh:mm:ss");
                        emailTemplateName = "HotelBookingRequest_Approval_User.html";
                        if (sodRequestsList[0].IsAmountPaidByTraveller.Equals(true))
                            strHoldPNR = ConfigurationManager.AppSettings["strHOLDPNRPassenger"].Trim();
                        else
                            strHoldPNR = "";

                        emailCredentials = controller.EmailCredentialsHotel(emailSubject, emailTemplateName, hotelList, sodRequestsList, sodflightList, trId.ToString(), "approved");
                    }
                    else
                    {
                        bookingType = sodRequestsList[0].SodBookingTypeId == 1 ? "SOD" : "NON-SOD";

                        //Send Rejection Email Notification
                        emailSubject = bookingType + " " + "Booking Request Approval Notification from HOD :" + System.DateTime.Now.ToString("dd-MMM-yyyy-hh:mm:ss");
                        emailTemplateName = "HotelBookingRequest_Approval_User.html";

                        //Check Hold PNR
                        if (sodRequestsList[0].IsAmountPaidByTraveller.Equals(true))
                            strHoldPNR = ConfigurationManager.AppSettings["strHOLDPNRPassenger"].Trim();
                        else
                            strHoldPNR = "";

                        //Send Email Notification Credentials
                        emailCredentials = controller.EmailCredentialsHotelHod(emailSubject, emailTemplateName, hotelList, sodRequestsList, sodflightList, trId.ToString(), strHoldPNR);
                    }
                    emailCredentials.TemplateFilePath = emailCredentials.TemplateFilePath.Replace("[Team]", "HOD");
                    EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials, sodRequestsList[0].EmailId.Trim());
                    TempData["emailData"] = emailCredentials;
                    TempData["emailId"] = sodRequestsList[0].EmailId.Trim();
                    var emaildata = TempData["emailData"] as EmailNotificationModel;
                    var emailid = TempData["emailId"].ToString();

                    jsonmsg = "Approval Alert for Booking Request : " + bookingType + "-" + travelreqID.Split(',')[0] + "\n Approval process has been completed successfully.";
                    return (n >= 1 ? jsonmsg : string.Empty);
                }
                else
                {
                    return jsonmsg = "Oops! Something went wrong.";
                }
            }
            return jsonmsg;
        }
        /// <summary>
        /// reject hotel booking request
        /// </summary>
        /// <param name="travelreqID"></param>
        /// <returns></returns>
        public string RejectHotelBookingRequest(string travelreqID)
        {
            var controller = DependencyResolver.Current.GetService<HotelOnlyController>();
            string jsonmsg = "";
            var trid = Convert.ToInt64(travelreqID.Split(',')[0].ToString());
            var s = _sodApproverRepositorty.FindHotelApprovalStatus(trid);
            if (s == true)
            {
                return "Request cannot be rejected. It has been already approved.";
            }
            else
            {
                var hreject = _sodApproverRepositorty.FindHotelRejectionStatus(trid);
                if (hreject == true)
                {
                    return "Request has been already Rejected.";
                }
                var n = ManageOnlyHotelBookingApprovalStatus(travelreqID, "R");
                if (n > 0)
                {
                    _sodApproverRepositorty.UpdateOnlyHotelApprovalStatus(trid);
                    //Close Booking Request
                    var trRequestMaster = new TravelRequestMasterModels()
                    {
                        TravelRequestId = Convert.ToInt64(travelreqID.Split(',')[0]),
                        BookingStatus = "Close",
                        StatusDate = System.DateTime.Now
                    };
                    var c = _sodApproverRepositorty.CloseSodBookingRequest_HOD(trRequestMaster);
                    //Get Booking Request Data for Email Notification
                    var sodRequestsList = new List<TravelRequestMasterModels>();
                    var sodflightList = new List<FlightDetailModels>();
                    var sodPassList = new List<PassengerDetailModels>();
                    var sodPassMealsList = new List<PassengerMealAllocationModels>();
                    var hotelList = new List<TravelRequestHotelDetailModels>();
                    var dicList = new Dictionary<string, object>();

                    dicList = _sodApproverRepositorty.GetSodBookingInfoForPNR(trid);
                    sodRequestsList = dicList["bookingInfo"] as List<TravelRequestMasterModels>;
                    sodflightList = dicList["flightInfo"] as List<FlightDetailModels>;
                    sodPassList = dicList["passInfo"] as List<PassengerDetailModels>;
                    sodPassMealsList = dicList["mealsInfo"] as List<PassengerMealAllocationModels>;
                    hotelList = dicList["hotelInfo"] as List<TravelRequestHotelDetailModels>;
                    string bookingType = sodRequestsList[0].SodBookingTypeId == 1 ? "SOD" : "NON-SOD";

                    //Send Rejection Email Notification

                    string emailSubject = bookingType + " " + "Booking Request Rejection Notification from HOD :" + System.DateTime.Now.ToString("dd-MMM-yyyy-hh:mm:ss");
                    var emailTemplateName = "OnlyhotelBookingRejection_user.html";
                    var emailCredentials = controller.EmailCredentialsHotel(emailSubject, emailTemplateName, hotelList, sodRequestsList, sodflightList, trid.ToString(), "Reject");
                    TempData["emailData"] = emailCredentials;
                    TempData["emailId"] = sodRequestsList[0].EmailId.Trim();
                    var emaildata = TempData["emailData"] as EmailNotificationModel;
                    var emailid = TempData["emailId"].ToString();
                    EmailNotifications.SendBookingRequestNotificationTo_Requester(emaildata, emailid);
                    jsonmsg = "Rejection Alert for Booking Request : " + bookingType + "-" + travelreqID.Split(',')[0] + "\n Rejection process has been completed successfully.";
                    return (n >= 1 ? jsonmsg : string.Empty);
                }
                else
                {
                    return jsonmsg = "Oops! Something went wrong.";
                }
            }
        }

        public int ManageOnlyHotelBookingApprovalStatus(string travelreqID, string type)
        {
            int n = 0;
            var trid = Convert.ToInt64(travelreqID.Split(',')[0].ToString());
            var approvalList = new TravelRequestApprovalModels()
            {
                TravelRequestId = trid,
                ApprovedByEmpId = 0,
                ApprovalStatus = (type == "A") ? (Int16)1 : (Int16)2,
                IsMandatoryTravel = 0,
                ApprovalDate = System.DateTime.Now,
                Comment = (type == "A" ? "Approved by" : "Rejected by") + " " + travelreqID.Split(',')[4].ToString(),
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
            try
            {
                n = _sodApproverRepositorty.ApproveOnlyHotelSodBookingRequest(approvalList);
            }
            catch (Exception ex)
            {
                n = -1;
            }
            return n;
        }

        /// <summary>
        /// Approve Booking Request for HOD
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        public string ApproveSodBookingRequest(string travelReqId)
        {
            var jsonmsg = string.Empty;
            var strHoldPNR = string.Empty;
            var approvalList = new TravelRequestApprovalModels()
            {
                TravelRequestId = Convert.ToInt64(travelReqId.Split(',')[0]),//Travel Request
                ApprovedByEmpId = Convert.ToInt32(travelReqId.Split(',')[4]),//Employee Id of approver
                ApprovalStatus = 1,
                IsMandatoryTravel = Convert.ToInt16(travelReqId.Split(',')[5]), //Is Mandatory Travel
                ApprovalDate = System.DateTime.Now,
                Comment = "Approved from HOD",
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
            if (travelReqId.Split(',')[3].ToString().ToLower() == "only hotel")
            {
                var hreq = _sodApproverRepositorty.FindHotelApprovalStatus(Convert.ToInt64(travelReqId.Split(',')[0]));
                if (hreq == true)
                {
                    return "Request has been already approved.";
                }
                else
                {
                    var hreject = _sodApproverRepositorty.FindHotelRejectionStatus(Convert.ToInt64(travelReqId.Split(',')[0]));
                    if (hreject == true)
                    {
                        return "Request has been already Rejected.";
                    }
                    var n = _sodApproverRepositorty.ApproveOnlyHotelSodBookingRequest(approvalList);
                    if (n > 0)
                    {
                        _sodApproverRepositorty.UpdateOnlyHotelApprovalStatus(Convert.ToInt64(travelReqId.Split(',')[0]));
                        var approval = _sodApproverRepositorty.ApproveSodBookingRequest(approvalList);
                        if (approval >= 1)
                        {
                            //Close Booking Request
                            var trRequestMaster = new TravelRequestMasterModels()
                            {
                                TravelRequestId = Convert.ToInt64(travelReqId.Split(',')[0]),
                                BookingStatus = "Close",
                                StatusDate = System.DateTime.Now
                            };
                            var c = _sodApproverRepositorty.CloseSodBookingRequest_HOD(trRequestMaster);

                            //Get Booking Request Data for Email Notification
                            var sodRequestsList = new List<TravelRequestMasterModels>();
                            var sodflightList = new List<FlightDetailModels>();
                            var sodPassList = new List<PassengerDetailModels>();
                            var sodPassMealsList = new List<PassengerMealAllocationModels>();
                            var hotelList = new List<TravelRequestHotelDetailModels>();
                            var dicList = new Dictionary<string, object>();

                            dicList = _sodApproverRepositorty.GetSodBookingInfoForPNR(Convert.ToInt64(travelReqId.Split(',')[0]));
                            sodRequestsList = dicList["bookingInfo"] as List<TravelRequestMasterModels>;
                            sodflightList = dicList["flightInfo"] as List<FlightDetailModels>;
                            sodPassList = dicList["passInfo"] as List<PassengerDetailModels>;
                            sodPassMealsList = dicList["mealsInfo"] as List<PassengerMealAllocationModels>;
                            hotelList = dicList["hotelInfo"] as List<TravelRequestHotelDetailModels>;
                            string bookingType = sodRequestsList[0].SodBookingTypeId == 1 ? "SOD" : "NON-SOD";

                            //Send Rejection Email Notification
                            string emailSubject = bookingType + " " + "Booking Request Approval Notification from HOD :" + System.DateTime.Now.ToString("dd-MMM-yyyy-hh:mm:ss");
                            var emailTemplateName = "SodBookingRequest_HotelApproval_User.html";

                            //Check Hold PNR
                            if (sodRequestsList[0].IsAmountPaidByTraveller.Equals(true))
                                strHoldPNR = ConfigurationManager.AppSettings["strHOLDPNRPassenger"].Trim();
                            else
                                strHoldPNR = "";

                            //Send Email Notification Credentials
                            var emailCredentials = EmailCredentials(emailSubject, emailTemplateName, sodRequestsList, sodflightList, sodPassList, sodPassMealsList, hotelList, travelReqId.Split(',')[0].ToString(), strHoldPNR);
                            emailCredentials.TemplateFilePath = emailCredentials.TemplateFilePath.Replace("[Team]", "HOD");
                            EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials, sodRequestsList[0].EmailId.Trim());

                            jsonmsg = "Approval Alert for Booking Request : " + bookingType + "-" + travelReqId.Split(',')[0] + "\n Approval process has been completed successfully.";
                            return (approval >= 1 ? jsonmsg : string.Empty);
                        }
                    }
                }
            }

            //Check Duplicate PNR  
            var chkpnr = Services.ADO.SodCommonServices.CheckDuplicatePNR(Convert.ToInt64(travelReqId.Split(',')[0]), 1);
            if (!chkpnr.Equals("0"))
            {
                return "PNR has been already generated for this booking request ID :" + travelReqId.Split(',')[0] + " . Existing PNR No. :" + chkpnr;
            }
            var status = _sodApproverRepositorty.checkApprovalStatusOfHod(Convert.ToInt32(travelReqId.Split(',')[0]));
            if (status == 2)
            {
                return "Sorry : Rejection Process has been already completed for this Booking Request ID :" + travelReqId.Split(',')[0];
            }
            var s = _sodApproverRepositorty.ApproveSodBookingRequest(approvalList);
            //SJSCHodEmailId = Session["HodEmailId"].ToString();
            var reqType = travelReqId.Split(',')[2].Trim().Equals("1") ? "SOD" : "NON-SOD";
            if (s >= 1)
            {
                /****Validate :Revenue Level Approval ************************/
                TempData["reqType"] = reqType;
                var bookingfor = travelReqId.Split(',')[3].Trim();
                if (bookingfor.Equals("Confirm"))
                {
                    //Check : Is approver from cxo level?
                    var r = _sodApproverRepositorty.IsCXORole(Convert.ToInt32(travelReqId.Split(',')[4]));
                    if (r == true && reqType == "SOD")
                    {
                        jsonmsg = GeneratePNRandGetMessage(Convert.ToInt64(travelReqId.Split(',')[0]), reqType);
                    }
                    else if (r == true && reqType == "NON-SOD")// && Convert.ToInt16(travelReqId.Split(',')[5]) == 1)
                    {
                        //Manage revenue Rejection Status
                        ManageRevenueStatus_DefaultRejectionCase(travelReqId.Split(',')[0], travelReqId.Split(',')[4]);
                        //Check CXO1 Level 1 and Update
                        var cStatus = CheckCXOApprovalLevel_and_Update_the_Approval_Status(travelReqId.Split(',')[0], travelReqId.Split(',')[4]);
                        //Fwd mail to CXO 2
                        var fwdCXOmailid = SendEmailtoAnotherCXO_for_NONSOD_Confirm_BookingApproval_andGenerate_PNR(travelReqId.Split(',')[0], cStatus);
                        //jsonmsg = GeneratePNRandGetMessage(Convert.ToInt64(travelReqId.Split(',')[0]), reqType);
                        jsonmsg = "Sod Booking Request ID: " + travelReqId.Split(',')[0] +
                         " - Approval process has been completed successfully.For final PNR approval the request has been forwarded to one more C-Level at " + fwdCXOmailid;
                    }
                    else
                    {
                        jsonmsg = "Sod Booking Request ID: " + travelReqId.Split(',')[0] +
                         " - Approval process has been completed successfully.For final PNR approval the request has been sent to Revenue Department.";
                    }
                }
                else
                {
                    jsonmsg = GeneratePNRandGetMessage(Convert.ToInt64(travelReqId.Split(',')[0]), reqType);
                }
            }
            return (s >= 1 ? jsonmsg : string.Empty);
        }


        /// <summary>
        /// PNRGenerationMessage
        /// </summary>
        /// <param name="trReqId"></param>
        /// <param name="reqType"></param>
        /// <returns></returns>
        public string GeneratePNRandGetMessage(Int64 trReqId, string reqType)
        {
            var jsonmsg = GeneratePNR(trReqId, reqType);
            if (jsonmsg.Length.Equals(6))
                jsonmsg = "Successfully PNR has been generated. PNR No. : " + jsonmsg;
            else if (jsonmsg == "ER001")
            {
                var approvalListrollback = new TravelRequestApprovalModels()
                {
                    TravelRequestId = trReqId,
                    ApprovalStatus = 0,
                    Comment = ""
                };
                _sodApproverRepositorty.RollBackApprovalByHOD(approvalListrollback);
                jsonmsg = "PNR Error : ER001";
            }
            else
                jsonmsg = "Sorry! Invalid PNR Error :" + jsonmsg;

            return jsonmsg;
        }

        /// <summary>
        /// Reject Booking Request
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        public string RejectSodBookingRequest(string travelReqId)
        {
            var jsonmsg = string.Empty;
            var strHoldPNR = string.Empty;

            //Initialize list for updating revenue status
            var approvalList = new TravelRequestApprovalModels()
            {
                TravelRequestId = Convert.ToInt64(travelReqId.Split(',')[0]),
                ApprovedByEmpId = Convert.ToInt32(travelReqId.Split(',')[4]),
                ApprovalStatus = 2,
                ApprovalDate = System.DateTime.Now,
                Comment = "Rejected from HOD",
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
            if (travelReqId.Split(',')[3].ToString().ToLower() == "only hotel")
            {
                var hreq = _sodApproverRepositorty.FindHotelApprovalStatus(Convert.ToInt64(travelReqId.Split(',')[0]));
                if (hreq == true)
                {
                    jsonmsg = "Request has been already approved.";
                }
                else
                {
                    var hreject = _sodApproverRepositorty.FindHotelRejectionStatus(Convert.ToInt64(travelReqId.Split(',')[0]));
                    if (hreject == true)
                    {
                        jsonmsg = "Request has been already Rejected.";
                    }
                    var n = _sodApproverRepositorty.ApproveOnlyHotelSodBookingRequest(approvalList);
                    if (n > 0)
                    {
                        _sodApproverRepositorty.UpdateOnlyHotelApprovalStatus(Convert.ToInt64(travelReqId.Split(',')[0]));
                    }
                }
            }
            var s = _sodApproverRepositorty.RejectSodBookingRequest(approvalList);
            if (s >= 1)
            {
                //Close Booking Request
                var trRequestMaster = new TravelRequestMasterModels()
                {
                    TravelRequestId = Convert.ToInt64(travelReqId.Split(',')[0]),
                    BookingStatus = "Close",
                    StatusDate = System.DateTime.Now
                };
                var c = _sodApproverRepositorty.CloseSodBookingRequest_HOD(trRequestMaster);

                //Get Booking Request Data for Email Notification
                var sodRequestsList = new List<TravelRequestMasterModels>();
                var sodflightList = new List<FlightDetailModels>();
                var sodPassList = new List<PassengerDetailModels>();
                var sodPassMealsList = new List<PassengerMealAllocationModels>();
                var hotelList = new List<TravelRequestHotelDetailModels>();
                var dicList = new Dictionary<string, object>();

                dicList = _sodApproverRepositorty.GetSodBookingInfoForPNR(Convert.ToInt64(travelReqId.Split(',')[0]));
                sodRequestsList = dicList["bookingInfo"] as List<TravelRequestMasterModels>;
                sodflightList = dicList["flightInfo"] as List<FlightDetailModels>;
                sodPassList = dicList["passInfo"] as List<PassengerDetailModels>;
                sodPassMealsList = dicList["mealsInfo"] as List<PassengerMealAllocationModels>;
                hotelList = dicList["hotelInfo"] as List<TravelRequestHotelDetailModels>;
                string bookingType = sodRequestsList[0].SodBookingTypeId == 1 ? "SOD" : "NON-SOD";

                //Send Rejection Email Notification
                string emailSubject = bookingType + " " + "Booking Request Rejection Notification from HOD :" + System.DateTime.Now.ToString("dd-MMM-yyyy-hh:mm:ss");
                var emailTemplateName = "SodBookingRequest_Rejection_User.html";

                //Check Hold PNR
                if (sodRequestsList[0].IsAmountPaidByTraveller.Equals(true))
                    strHoldPNR = ConfigurationManager.AppSettings["strHOLDPNRPassenger"].Trim();
                else
                    strHoldPNR = "";

                //Send Email Notification Credentials
                var emailCredentials = EmailCredentials(emailSubject, emailTemplateName, sodRequestsList, sodflightList, sodPassList, sodPassMealsList, hotelList, travelReqId.Split(',')[0].ToString(), strHoldPNR);
                emailCredentials.TemplateFilePath = emailCredentials.TemplateFilePath.Replace("[Team]", "HOD");
                EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials, sodRequestsList[0].EmailId.Trim());

                jsonmsg = "Rejection Alert for Booking Request : " + bookingType + "-" + travelReqId.Split(',')[0] + "\n Rejection process has been completed successfully.";
                return (s >= 1 ? jsonmsg : string.Empty);

            }
            return jsonmsg;
        }


        /// <summary>
        /// Approve Booking Request for CXO
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        public string ApproveSodBookingRequestToCXO(string travelReqId)
        {
            bool IsApp1 = false;
            bool IsApp2 = false;
            var jsonmsg = string.Empty;
            var pnrStatus = string.Empty;
            var s = 0;
            var l1 = 0;
            var l2 = 0;
            var reqType = travelReqId.Split(',')[2].Trim().Equals("1") ? "SOD" : "NON-SOD";
            TempData["reqType"] = reqType;

            //Check Duplicate PNR  
            var chkpnr = Services.ADO.SodCommonServices.CheckDuplicatePNR(Convert.ToInt64(travelReqId.Split(',')[0]), 1);
            if (!chkpnr.Equals("0"))
            {
                return "PNR has been already generated for this Booking Request ID :" + travelReqId.Split(',')[0] + ". Existing PNR No. :" + chkpnr;
            }

            //Check Already approved ?

            //Check Priority Level for CXO1 & CXO2
            var cxoPriority = _sodApproverRepositorty.CheckCXOApprover_PriorityByEmpId(Convert.ToInt32(travelReqId.Split(',')[4]), Convert.ToInt64(travelReqId.Split(',')[0]));
            if (cxoPriority == 1)
            {
                IsApp1 = _sodApproverRepositorty.CheckSodApprovalStatusCXO_Level1(Convert.ToInt64(travelReqId.Split(',')[0]));
                if (IsApp1)
                {
                    var appList1 = new TravelRequestApprovalModels()
                    {
                        TravelRequestId = Convert.ToInt64(travelReqId.Split(',')[0]),
                        ApprovedByEmpIdCLevel1 = Convert.ToInt32(travelReqId.Split(',')[4]),
                        ApprovalStatusCLevel1 = 1,
                        CLevelComment1 = "Approved from CXO Priority 1",
                        CLevelAppDate1 = System.DateTime.Now
                    };
                    l1 = _sodApproverRepositorty.UpdateSodBookingRequestCXO_Level1(appList1);
                    if (l1 > 0)
                    {
                        jsonmsg = "SOD Booking Request ID :" + reqType + "-" + travelReqId.Split(',')[0] + "\n, Booking Type :Confirm, \n\n Approval process has been completed successfully.";
                        //Generate PNR for SOD Booking Priority 1
                        if (travelReqId.Split(',')[2].Trim().Equals("1") && travelReqId.Split(',')[3].Trim().Equals("Confirm") && travelReqId.Split(',')[5].Trim().Equals("1"))
                        {
                            pnrStatus = GeneratePNR(Convert.ToInt64(travelReqId.Split(',')[0]), travelReqId.Split(',')[1]);
                            if (pnrStatus.Length.Equals(6))
                            {
                                s = 1;
                                jsonmsg = "SOD Booking Request ID :" + reqType + "-" + travelReqId.Split(',')[0] + "\n, PNR has been generated successfully.\n PNR No. :" + pnrStatus;
                            }
                            else
                            {
                                var approvalListrollback = new TravelRequestApprovalModels()
                                {
                                    TravelRequestId = Convert.ToInt64(travelReqId.Split(',')[0]),
                                    ApprovedByEmpIdCLevel1 = 0,
                                    ApprovalStatusCLevel1 = 0,
                                    CLevelComment1 = "PNR Error from CXO1",
                                    CLevelAppDate1 = System.DateTime.Now
                                };
                                _sodApproverRepositorty.RollBackApprovalByCXO_Level1(approvalListrollback);
                                jsonmsg = "Approval Status Error :" + pnrStatus;
                            }
                        }
                    }
                }
            }
            else if (cxoPriority == 2)
            {
                IsApp2 = _sodApproverRepositorty.CheckSodApprovalStatusCXO_Level2(Convert.ToInt64(travelReqId.Split(',')[0]));
                if (IsApp2)
                {
                    var appList2 = new TravelRequestApprovalModels()
                    {
                        TravelRequestId = Convert.ToInt64(travelReqId.Split(',')[0]),
                        ApprovedByEmpIdCLevel2 = Convert.ToInt32(travelReqId.Split(',')[4]),
                        ApprovalStatusCLevel2 = 1,
                        CLevelComment2 = "Approved from CXO Priority 2",
                        CLevelAppDate2 = System.DateTime.Now
                    };
                    l2 = _sodApproverRepositorty.UpdateSodBookingRequestCXO_Level2(appList2);
                    if (l2 > 0)
                    {
                        jsonmsg = "SOD Booking Request ID :" + reqType + "-" + travelReqId.Split(',')[0] + "\n, Booking Type :Confirm, \n\n Approval process has been completed successfully.";
                        //Generate PNR for SOD Booking Priority 2
                        if (travelReqId.Split(',')[2].Trim().Equals("1") && travelReqId.Split(',')[3].Trim().Equals("Confirm") && travelReqId.Split(',')[5].Trim().Equals("1"))
                        {
                            pnrStatus = GeneratePNR(Convert.ToInt64(travelReqId.Split(',')[0]), travelReqId.Split(',')[1]);
                            if (pnrStatus.Length.Equals(6))
                            {
                                s = 1;
                                jsonmsg = "SOD Booking Request ID :" + reqType + "-" + travelReqId.Split(',')[0] + "\n, Booking Type :Confirm, \n\n PNR has been generated successfully.\n PNR No. :" + pnrStatus;
                            }
                            else
                            {
                                var approvalListrollback = new TravelRequestApprovalModels()
                                {
                                    TravelRequestId = Convert.ToInt64(travelReqId.Split(',')[0]),
                                    ApprovedByEmpIdCLevel2 = 0,
                                    ApprovalStatusCLevel2 = 0,
                                    CLevelComment2 = "PNR Error from CXO2",
                                    CLevelAppDate2 = System.DateTime.Now
                                };
                                _sodApproverRepositorty.RollBackApprovalByCXO_Level2(approvalListrollback);
                                jsonmsg = "Approval Status Error :" + pnrStatus;
                            }
                        }
                    }
                }
            }

            //For NON-SOD PNR Generation : When Revenue says 'YES' 
            if (travelReqId.Split(',')[2].Trim().Equals("2") && travelReqId.Split(',')[3].Trim().Equals("Confirm") && travelReqId.Split(',')[6].Trim().Equals("RY"))
            {
                if (l1 > 0 || l2 > 0)
                {
                    pnrStatus = GeneratePNR(Convert.ToInt64(travelReqId.Split(',')[0]), travelReqId.Split(',')[1]);
                    if (pnrStatus.Length.Equals(6))
                    {
                        s = 1;
                        jsonmsg = "SOD Booking Request ID : " + reqType + "-" + travelReqId.Split(',')[0] + "\n, Booking Type :Confirm, \n\n PNR has been generated successfully.\n PNR No. :" + pnrStatus;
                    }
                    else
                    {
                        //L1 Roll Back
                        var approvalListrollback1 = new TravelRequestApprovalModels()
                        {
                            TravelRequestId = Convert.ToInt64(travelReqId.Split(',')[0]),
                            ApprovedByEmpIdCLevel1 = 0,
                            ApprovalStatusCLevel1 = 0,
                            CLevelComment1 = "PNR Error from CXO1",
                            CLevelAppDate1 = System.DateTime.Now
                        };
                        _sodApproverRepositorty.RollBackApprovalByCXO_Level1(approvalListrollback1);

                        //L2 RollBack
                        var approvalListrollback2 = new TravelRequestApprovalModels()
                        {
                            TravelRequestId = Convert.ToInt64(travelReqId.Split(',')[0]),
                            ApprovedByEmpIdCLevel2 = 0,
                            ApprovalStatusCLevel2 = 0,
                            CLevelComment2 = "PNR Error from CXO2",
                            CLevelAppDate2 = System.DateTime.Now
                        };
                        _sodApproverRepositorty.RollBackApprovalByCXO_Level2(approvalListrollback2);
                        jsonmsg = "Approval Status Error :" + pnrStatus;
                    }
                }
            }
            //For NON-SOD PNR Generation :When Revenue says 'NO' :
            else if (travelReqId.Split(',')[2].Trim().Equals("2") && travelReqId.Split(',')[3].Trim().Equals("Confirm") && travelReqId.Split(',')[6].Trim().Equals("RN"))
            {
                var IsApp = _sodApproverRepositorty.CheckSodApprovalStatusCXO_LevelL1L2(Convert.ToInt64(travelReqId.Split(',')[0]));
                //If C1 & C2 Say Yes
                if (IsApp)
                {
                    //Generate PNR
                    pnrStatus = GeneratePNR(Convert.ToInt64(travelReqId.Split(',')[0]), travelReqId.Split(',')[1]);
                    if (pnrStatus.Length.Equals(6))
                    {
                        s = 1;
                        jsonmsg = "SOD Booking Request ID : " + reqType + "-" + travelReqId.Split(',')[0] + "\n, Booking Type : Confirm, \n\n PNR has been generated successfully.\n PNR No. :" + pnrStatus;
                    }
                    else
                    {
                        //L1 Roll Back
                        var approvalListrollback1 = new TravelRequestApprovalModels()
                        {
                            TravelRequestId = Convert.ToInt64(travelReqId.Split(',')[0]),
                            ApprovedByEmpIdCLevel1 = 0,
                            ApprovalStatusCLevel1 = 0,
                            CLevelComment1 = "PNR Error from CXO1",
                            CLevelAppDate1 = System.DateTime.Now
                        };
                        _sodApproverRepositorty.RollBackApprovalByCXO_Level1(approvalListrollback1);

                        //L2 RollBack
                        var approvalListrollback2 = new TravelRequestApprovalModels()
                        {
                            TravelRequestId = Convert.ToInt64(travelReqId.Split(',')[0]),
                            ApprovedByEmpIdCLevel2 = 0,
                            ApprovalStatusCLevel2 = 0,
                            CLevelComment2 = "PNR Error from CXO2",
                            CLevelAppDate2 = System.DateTime.Now
                        };
                        _sodApproverRepositorty.RollBackApprovalByCXO_Level2(approvalListrollback2);
                        jsonmsg = "Approval Status Error :" + pnrStatus;
                    }
                }
            }
            return jsonmsg;
        }


        /// <summary>
        /// Reject Booking Request
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        public string RejectSodBookingRequestToCXO(string travelReqId)
        {
            var jsonmsg = string.Empty;
            var strHoldPNR = string.Empty;
            var s = 0;
            if ((travelReqId.Split(',')[0] == "undefined") || (travelReqId.Split(',')[0] == ""))
            {
                return "Invalid record.";
            }

            //Check Duplicate PNR  
            var chkpnr = Services.ADO.SodCommonServices.CheckDuplicatePNR(Convert.ToInt64(travelReqId.Split(',')[0]), 1);
            if (!chkpnr.Equals("0"))
            {
                return "PNR has been already generated for this Booking Request ID : " + travelReqId.Split(',')[0] + ". Existing PNR No. :" + chkpnr;
            }

            //Get Employee Data to show in EMail
            var sodRequestsList = new List<TravelRequestMasterModels>();
            var sodflightList = new List<FlightDetailModels>();
            var sodPassList = new List<PassengerDetailModels>();
            var sodPassMealsList = new List<PassengerMealAllocationModels>();
            var hotelList = new List<TravelRequestHotelDetailModels>();
            var dicList = new Dictionary<string, object>();

            dicList = _sodApproverRepositorty.GetSodBookingInfoForPNR(Convert.ToInt64(travelReqId.Split(',')[0]));
            sodRequestsList = dicList["bookingInfo"] as List<TravelRequestMasterModels>;
            sodflightList = dicList["flightInfo"] as List<FlightDetailModels>;
            sodPassList = dicList["passInfo"] as List<PassengerDetailModels>;
            sodPassMealsList = dicList["mealsInfo"] as List<PassengerMealAllocationModels>;
            hotelList = dicList["hotelInfo"] as List<TravelRequestHotelDetailModels>;
            string bookingType = sodRequestsList[0].SodBookingTypeId == 1 ? "SOD" : "NON-SOD";

            //Check Priority Level
            var cxoPriority = _sodApproverRepositorty.CheckCXOApprover_PriorityByEmpId(Convert.ToInt32(travelReqId.Split(',')[4]), Convert.ToInt64(travelReqId.Split(',')[0]));
            if (cxoPriority == 1)
            {
                var approvalList = new TravelRequestApprovalModels()
                {
                    TravelRequestId = Convert.ToInt64(travelReqId.Split(',')[0]),
                    ApprovedByEmpIdCLevel1 = Convert.ToInt32(Session["EmpId"].ToString()),
                    ApprovalStatusCLevel1 = 2,
                    CLevelAppDate1 = System.DateTime.Now,
                    CLevelComment1 = "Rejected by CXO Priority 1"
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
                    CLevelComment2 = "Rejected by CXO Priority 2"
                };
                s = _sodApproverRepositorty.UpdateSodBookingRequestCXO_Level2(approvalList);
            }
            if (s > 0)
            {
                //Close booking Request
                var trRequestMaster = new TravelRequestMasterModels()
                {
                    TravelRequestId = Convert.ToInt64(travelReqId.Split(',')[0]),
                    BookingStatus = "Close",
                    StatusDate = System.DateTime.Now
                };
                var c = _sodApproverRepositorty.CloseSodBookingRequest_HOD(trRequestMaster);

                //Hold PNR 
                if (sodRequestsList[0].IsAmountPaidByTraveller.Equals(true))
                    strHoldPNR = ConfigurationManager.AppSettings["strHOLDPNRPassenger"].Trim();
                else
                    strHoldPNR = "";

                //Rejection Notification
                var emailSubject = bookingType + " " + "Booking Request Rejection Notification from CXO Level :" + System.DateTime.Now.ToString("dd-MMM-yyyy-hh:mm:ss");
                var emailTemplateName_User = "SodConfirm_Rejection through CXO-Level.html";
                var emailCredentials = EmailCredentials(emailSubject, emailTemplateName_User, sodRequestsList, sodflightList, sodPassList, sodPassMealsList, hotelList, travelReqId.Split(',')[0], strHoldPNR);
                EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials, sodRequestsList[0].EmailId.Trim());
                jsonmsg = "Rejection Alert for Booking Request : " + bookingType + "-" + travelReqId.Split(',')[0] + "\n Rejection process has been completed successfully.";
            }
            return jsonmsg;
        }


        /// <summary>
        /// Generate PNR and Send Emal Notification
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <param name="emailId"></param>
        /// <returns></returns>
        public string GeneratePNR(Int64 travelReqId, string emailId)
        {
            string msg = string.Empty;
            var strHoldPNR = ConfigurationManager.AppSettings["strHOLDPNR"].Trim();
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
            //Generate PNR
            var pnr = NavitaireServices.Generate_PNR(sodRequestsList, sodflightList, passengerList, passengerMealsList);
            if (pnr == "ER001")
            {
                msg = pnr;
            }
            else
            {
                pnr = pnr + "|Close";
                var c = _sodApproverRepositorty.UpdatePnr(pnr, travelReqId);
                if (c > 0)
                {
                    //Send Email Notification
                    string emailSubject = reqType + " " + "Booking Request Approval Notification from HOD :" + System.DateTime.Now.ToString("dd-MMM-yyyy-hh:mm:ss");
                    string emailTemplateName = "SodBookingRequest_Approval_User.html";
                    var pnrc = pnr.Split('|')[0].ToString();

                    //Check Hold PNR
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

                    //Send Email Notification Credentials
                    var emailCredentials = EmailCredentials(emailSubject, emailTemplateName, sodRequestsList, sodflightList, passengerList, passengerMealsList, hotelList, travelReqId.ToString(), strHoldPNR);
                    if (!pnr.Equals(string.Empty))
                    {
                        emailCredentials.TemplateFilePath = emailCredentials.TemplateFilePath.Replace("[PNR]", pnrc);
                    }

                    //Send Email Notification
                    EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials, sodRequestsList[0].EmailId.Trim());
                    msg = pnr.Split('|')[0];
                }
            }
            return msg;
        }


        /// <summary>
        /// Initialized Email Credentials to Property
        /// </summary>
        /// <returns></returns>
        public EmailNotificationModel EmailCredentials(string subjectName, string emailTemplateName, List<TravelRequestMasterModels> sodRequestsList, List<FlightDetailModels> sodflightList, List<PassengerDetailModels> sodPassList, List<PassengerMealAllocationModels> sodPassMealsList, List<TravelRequestHotelDetailModels> hotelInfo, string reqNo, string strHoldPNRMessage)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtpUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtpPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtpServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtpPort"].Trim(),
                TemplateFilePath = ReadFile(emailTemplateName, sodRequestsList, sodflightList, sodPassList, sodPassMealsList, hotelInfo, reqNo, strHoldPNRMessage),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }


        /// <summary>
        /// Read Template File from Location
        /// </summary>
        /// <returns></returns>
        private string ReadFile(string emailTemplateName, List<TravelRequestMasterModels> sodRequestsList, List<FlightDetailModels> sodflightList, List<PassengerDetailModels> sodPassList, List<PassengerMealAllocationModels> sodPassMealsList, List<TravelRequestHotelDetailModels> hotelInfo, string reqNo, string strHoldPNRMessage)
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
                strContent = strContent.Replace("[mtaText]", "Reason for Mandatory Travel Request  : " + sodRequestsList[0].ReasonForMandatoryTravel);
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

            //Adding Booking Info
            tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td>" + sodRequestsList[0].ReasonForTravel + "</td><td>" + btype + "</td><td>" + sodRequestsList[0].Passengers + "</td><td>" + bookingFor + "</td>";
            if (mtr == "Yes")
                tr = tr + "<td>Yes</td>";
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

            //Hotel List
            if (sodRequestsList[0].IsHotelRequired == true)
            {
                var trh = "";
                if (btype == "SOD")
                {
                    trh = "<table cellpadding='0' cellspacing='0' style='width:100%; font-size:12px;border:solid 1px #c2c2c2;line-height:30px;'>";
                    trh = trh + "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>City</td><td>Check-In Date</td><td>Check-Out Date</td><td>No. Of Passenger</td></tr>";
                    foreach (var h in hotelInfo)
                    {
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

            var uri1 = ConfigurationManager.AppSettings["emailApprovalPathcxo"].Trim() + "?str=" + skey + "&type=a";
            var uri2 = ConfigurationManager.AppSettings["emailApprovalPathcxo"].Trim() + "?str=" + skey + "&type=r";

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
        /// Exception Handling
        /// </summary>
        /// <param name="filterContext"></param>
        void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            Logging.ErrorLog.Instance.AddDBLogging(ex, filterContext.Controller.ControllerContext.ToString(), "Controllers/mailController.cs");
        }

    }
}