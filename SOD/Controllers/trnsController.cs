using SOD.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SOD.Services.Interface;
using SOD.Services.Repository;
using SOD.Services.EntityFramework;
using SOD.EmailNotification;
using System.Configuration;
using System.Text;
using SOD.CommonWebMethod;

namespace SOD.Controllers
{

    [HandleError(ExceptionType = typeof(DbUpdateException), View = "Error")]
    public class trnsController : Controller, IActionFilter, IExceptionFilter
    {

        /// <summary>
        /// Constructor Initialization
        /// </summary>
        private readonly ITransportRepository _transportRepository;
        public trnsController()
        {
            _transportRepository = new TransportRepository(new SodEntities());
        }


        /// <summary>
        /// View Transport Module main List
        /// </summary>
        /// <returns></returns>
        public ActionResult trlist()
        {
            //Response.AddHeader("Refresh", "5");
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult hotelList()
        {
            return View();
        }

        /// <summary>
        /// View Vendor List
        /// </summary>
        /// <returns></returns>
        public ActionResult vdrlist()
        {
            return View();
        }

        /// <summary>
        /// View Hotel report
        /// </summary>
        /// <returns></returns>
        public ActionResult HotelDetailsData()
        {
            return View();
        }


        /// <summary>
        /// Get Employee Cab Request History
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetEmpCabBookingInfo(int empId)
        {
            var s = _transportRepository.GetSodEmployeeBookingHistoryList_TravelDesk(0, empId, 1);
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get hotel details of employees for traveldesk
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetEmpHotelBookingInfo(string type)
        {
            var empId = 0;
            //var type = Request.QueryString["type"].ToString();
            if (type == "SOD")
            {
                var s = _transportRepository.GetSodEmployeeHotelList_TravelDesk(0, empId, 1);
                return Json(s, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var s = _transportRepository.GetSodEmployeeHotelList_TravelDesk(0, empId, 2);
                return Json(s, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Get Employee Cab Request History
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetCabStatus(int reqId)
        {
            var rqId = Convert.ToInt32(Request.QueryString["reqId"].ToString());
            var s = _transportRepository.GetCabStatus(rqId, 1);
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Employee Hotel Request History
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetHotelStatus(int reqId)
        {
            var rqId = Convert.ToInt32(Request.QueryString["reqId"].ToString().Split('|')[0]);
            var hId = Convert.ToInt32(Request.QueryString["reqId"].ToString().Split('|')[1]);
            var s = _transportRepository.GetHotelStatus(rqId, hId, 1);
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Approve cab request
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AppCabRequest(string req)
        {
            var rqId = Request.QueryString["req"].ToString().Split('|')[0];
            var remark = Request.QueryString["req"].ToString().Split('|')[1];
            var s = _transportRepository.ApprovedCabRequest(Convert.ToInt64(rqId), remark);
            if (s > 0)
                GetCabApprovalNotificationData(rqId);
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// approve hotel request
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AppHotelRequest(List<TravelRequestHotelDetailModels> elist)
        {
            var rqId = elist[0].TravelRequestId.ToString();
            var hId = elist[0].HotelRequestId.ToString();
            var s = _transportRepository.ApprovedHotelRequest(elist);
            try
            {
                GetHotelApprovalNotificationData(rqId, hId);
            }
            catch (Exception e)
            {
                //Logging.ErrorLog.Instance.AddDBLogging(e, e.ToString(), "Controllers/trnsController.cs");
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Approve hotel request oat
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AppHotelRequestOat(List<OALHotelModel> elist)
        {
            var rqId = elist[0].TravelRequestId.ToString();
            var s = _transportRepository.ApprovedHotelRequestOat(elist);
            GetHotelApprovalNotificationDataOat(rqId);
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Reject Cab Request
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RejectCabRequest(string req)
        {
            var rqId = Request.QueryString["req"].ToString().Split('|')[0];
            var remark = Request.QueryString["req"].ToString().Split('|')[1];
            var s = _transportRepository.RejectCabRequest(Convert.ToInt64(rqId), remark);
            if (s > 0)
                GetCabRejectionNotificationData(rqId);
            return Json(s, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RejectHotelRequest(string req)
        {
            var rqId = Request.QueryString["req"].ToString().Split('|')[0];
            var remark = Request.QueryString["req"].ToString().Split('|')[1];
            var s = _transportRepository.RejectHotelRequest(Convert.ToInt64(rqId), remark);
            if (s > 0)
                GetHotelRejectionNotificationData(rqId);
            return Json(s, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Reject hotel request oat
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RejectHotelRequestOat(string req)
        {
            var rqId = Request.QueryString["req"].ToString().Split('|')[0];
            var remark = Request.QueryString["req"].ToString().Split('|')[1];
            var s = _transportRepository.RejectHotelRequestOat(Convert.ToInt64(rqId), remark);
            if (s > 0)
                GetHotelRejectionNotificationDataOat(rqId);
            return Json(s, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Get Employee cab & booking details 
        /// </summary>
        [HttpGet]
        public JsonResult GetBookingandCabInfo()
        {
            var trId = Request.QueryString["trId"].ToString();
            //Display Request wise view details
            var cabList = new List<TravelRequestCabDetailModels>();
            var sodflightList = new List<FlightDetailModels>();
            var passengerList = new List<PassengerDetailModels>();

            var dicList = new Dictionary<string, object>();
            dicList = _transportRepository.GetSodBookingandCabInfo(Convert.ToInt64(trId));

            cabList = dicList["cabinfo"] as List<TravelRequestCabDetailModels>;
            sodflightList = dicList["flightInfo"] as List<FlightDetailModels>;
            passengerList = dicList["passInfo"] as List<PassengerDetailModels>;
            TempData["bookingInfo"] = dicList["bookingInfo"] as List<TravelRequestMasterModels>;
            TempData["cabList"] = cabList;
            return Json(dicList, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetBookingandHotelInfo()
        {
            var trId = Request.QueryString["trId"].ToString();
            //Display Request wise view details
            var hotelList = new List<TravelRequestHotelDetailModels>();
            var sodflightList = new List<FlightDetailModels>();
            var passengerList = new List<PassengerDetailModels>();

            var dicList = new Dictionary<string, object>();
            dicList = _transportRepository.GetSodBookingandHotelInfo(Convert.ToInt64(trId));

            sodflightList = dicList["flightInfo"] as List<FlightDetailModels>;
            passengerList = dicList["passInfo"] as List<PassengerDetailModels>;
            TempData["bookingInfo"] = dicList["bookingInfo"] as List<TravelRequestMasterModels>;
            TempData["hotelList"] = dicList["hotelinfo"] as List<TravelRequestHotelDetailModels>;
            return Json(dicList, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetHotelDetailbyTrID()
        {
            var trId =Request.QueryString["trId"].ToString().Split('|')[0];
            var hId =Request.QueryString["trId"].ToString().Split('|')[1];
            var hotelList = new List<HotelRequestApprovalModel>();
            var hotelData = new List<TravelRequestHotelDetailModels>();
            var rejectedData = new List<HotelRequestRejectionModel>();
            var cancelledData = new List<HotelCancellationByTraveldeskModel>();

            var dicList = new Dictionary<string, object>();
            dicList = _transportRepository.GetHotelDetailbyTrID(Convert.ToInt64(trId), Convert.ToInt32(hId));
            hotelList = dicList["hotelinfobyTrid"] as List<HotelRequestApprovalModel>;
            hotelData = dicList["hotelData"] as List<TravelRequestHotelDetailModels>;
            rejectedData = dicList["rejectedData"] as List<HotelRequestRejectionModel>;
            cancelledData = dicList["cancelledData"] as List<HotelCancellationByTraveldeskModel>;

            return Json(dicList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetHotelDetailbyTrIDOat()
        {
            var trId = Request.QueryString["trId"].ToString();
            //Display Request wise view details
            var hotelList = new List<HotelRequestApprovalOatModels>();
            var hotelDataOat = new List<OALHotelModel>();

            var dicList = new Dictionary<string, object>();
            dicList = _transportRepository.GetHotelDetailbyTrIDOat(Convert.ToInt64(trId));
            hotelList = dicList["hotelinfobyTridOat"] as List<HotelRequestApprovalOatModels>;
            hotelDataOat = dicList["hotelDataOat"] as List<OALHotelModel>;

            return Json(dicList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Transport Vendor List
        /// </summary>
        /// <returns></returns>
        public JsonResult GetVendorList(string vCode)
        {
            return Json(_transportRepository.GetVendorList(vCode), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Get Cab Notification Data
        /// </summary>
        /// <returns></returns>
        public void GetCabApprovalNotificationData(string req)
        {
            //Send Email Notification  
            if (TempData["cabList"] != null && TempData["bookingInfo"] != null)
            {
                var cabList = TempData["cabList"] as List<TravelRequestCabDetailModels>;
                var bookingInfo = TempData["bookingInfo"] as List<TravelRequestMasterModels>;
                var emailSubject = "Cab Booking Approval Notification from Travel Desk :" + System.DateTime.Now.ToString();
                var emailTemplateName = "SodCabBookingRequestNotificationTemplateFor_TravelDesk.html";
                var emailCredentials = EmailCredentials(emailSubject, emailTemplateName, cabList, bookingInfo, req, "approved");
                TempData["emailData"] = emailCredentials;
                TempData["emailId"] = bookingInfo[0].EmailId.Trim();
            }
        }

        public void GetHotelApprovalNotificationData(string req, string hId)
        {
            var trId = req;
            //Display Request wise view details
            var hotelList = new List<TravelRequestHotelDetailModels>();
            var sodflightList = new List<FlightDetailModels>();
            var passengerList = new List<PassengerDetailModels>();

            var dicList = new Dictionary<string, object>();
            dicList = _transportRepository.GetSodHotelInfo(Convert.ToInt64(trId), Convert.ToInt32(hId));

            var dicListSharedUser = new Dictionary<string, object>();
            dicListSharedUser = _transportRepository.GetHotelDetailbyTrID(Convert.ToInt64(trId), Convert.ToInt32(hId));
            TempData["sharedUserList"] = dicListSharedUser["sharedUserdetails"] as List<TravelRequestMasterModels>;

            sodflightList = dicList["flightInfolist"] as List<FlightDetailModels>;
            passengerList = dicList["passInfolist"] as List<PassengerDetailModels>;
            TempData["bookingInfo"] = dicList["bookingInfolist"] as List<TravelRequestMasterModels>;
            TempData["hotelList"] = dicList["hotelinfolist"] as List<TravelRequestHotelDetailModels>;


            //Send Email Notification  
            if (TempData["hotelList"] != null && TempData["bookingInfo"] != null)
            {
                var hotel_List = TempData["hotelList"] as List<TravelRequestHotelDetailModels>;
                var bookingInfo = TempData["bookingInfo"] as List<TravelRequestMasterModels>;

                var emailSubject = "SOD Hotel Booking Approval Notification from Travel Desk :" + System.DateTime.Now.ToString();
                var emailTemplateName = "SodHotelBookingRequestNotificationTemplateFor_TravelDesk.html";
                var emailCredentials = EmailCredentialsHotel(emailSubject, emailTemplateName, hotel_List, bookingInfo, req, "approved");
                TempData["emailData"] = emailCredentials;
                TempData["emailId"] = bookingInfo[0].EmailId.Trim();
                sendEmailNotification();
            }
        }


        public void GetHotelApprovalNotificationDataOat(string req)
        {
            var trId = req;
            //Display Request wise view details
            var hotelList = new List<OALHotelModel>();
            var sodflightList = new List<OALModels>();
            var passengerList = new List<OALPassengerModel>();

            var dicList = new Dictionary<string, object>();
            dicList = _transportRepository.GetSodHotelInfoOat(Convert.ToInt64(trId));

            var dicListSharedUser = new Dictionary<string, object>();
            dicListSharedUser = _transportRepository.GetHotelDetailbyTrIDOat(Convert.ToInt64(trId));
            TempData["sharedUserListOat"] = dicListSharedUser["sharedUserdetailsOat"] as List<TravelRequestMasterModels>;

            sodflightList = dicList["flightInfolistOat"] as List<OALModels>;
            passengerList = dicList["passInfolistOat"] as List<OALPassengerModel>;
            TempData["bookingInfoOat"] = dicList["bookingInfolistOat"] as List<OALTravelRequestMasterModel>;
            TempData["hotelListOat"] = dicList["hotelinfolistOat"] as List<OALHotelModel>;

            //Send Email Notification  
            if (TempData["hotelListOat"] != null && TempData["bookingInfoOat"] != null)
            {
                var hotel_List = TempData["hotelListOat"] as List<OALHotelModel>;
                var bookingInfo = TempData["bookingInfoOat"] as List<OALTravelRequestMasterModel>;
                var emailSubject = "SOD Hotel Booking Approval Notification from Travel Desk :" + System.DateTime.Now.ToString();
                var emailTemplateName = "SodHotelBookingRequestNotificationTemplateFor_TravelDesk.html";
                var emailCredentials = EmailCredentialsHotelOat(emailSubject, emailTemplateName, hotel_List, bookingInfo, req, "approved");
                TempData["emailData"] = emailCredentials;
                TempData["emailId"] = bookingInfo[0].EmailId.Trim();
                sendEmailNotification();
            }
        }


        /// <summary>
        /// find inclusions of hotel by city and hotel name
        /// </summary>
        /// <param name="hotelcity"></param>
        /// <param name="hotelname"></param>
        /// <returns></returns>        
        public List<HotelInclusionMasterModels> findHotelInclusions(string hotelcity, string hotelname)
        {
            var inclusionList = new List<HotelInclusionMasterModels>();
            inclusionList = _transportRepository.findHotelInclusions(hotelcity, hotelname);
            return inclusionList;
        }


        /// <summary>
        /// Get Cab Notification Data
        /// </summary>
        /// <returns></returns>
        public void GetCabRejectionNotificationData(string req)
        {
            //Send Email Notification  
            if (TempData["cabList"] != null && TempData["bookingInfo"] != null)
            {
                var cabList = TempData["cabList"] as List<TravelRequestCabDetailModels>;
                var bookingInfo = TempData["bookingInfo"] as List<TravelRequestMasterModels>;
                var emailSubject = "Cab Booking Rejection Notification from Travel Desk :" + System.DateTime.Now.ToString();
                var emailTemplateName = "SodCabBookingRequestNotificationTemplateFor_TravelDesk.html";
                var emailCredentials = EmailCredentials(emailSubject, emailTemplateName, cabList, bookingInfo, req, "rejected");
                TempData["emailData"] = emailCredentials;
                TempData["emailId"] = bookingInfo[0].EmailId.Trim();
            }
        }



        /// <summary>
        /// Hotel Rejection Notifications
        /// </summary>
        /// <param name="req"></param>
        public void GetHotelRejectionNotificationData(string req)
        {
            //Send Email Notification  
            if (TempData["hotelList"] != null && TempData["bookingInfo"] != null)
            {
                var hotelList = TempData["hotelList"] as List<TravelRequestHotelDetailModels>;
                var bookingInfo = TempData["bookingInfo"] as List<TravelRequestMasterModels>;
                var emailSubject = "SOD Hotel Booking Rejection Notification from Travel Desk :" + System.DateTime.Now.ToString();
                var emailTemplateName = "SodHotelBookingRequestNotificationTemplateFor_TravelDesk.html";
                var emailCredentials = EmailCredentialsHotel(emailSubject, emailTemplateName, hotelList, bookingInfo, req, "rejected");
                TempData["emailData"] = emailCredentials;
                TempData["emailId"] = bookingInfo[0].EmailId.Trim();
            }
        }


        public void GetHotelRejectionNotificationDataOat(string req)
        {
            var dicList = new Dictionary<string, object>();
            dicList = _transportRepository.GetSodHotelInfoOat(Convert.ToInt64(req));
            TempData["bookingInfoOat"] = dicList["bookingInfolistOat"] as List<OALTravelRequestMasterModel>;
            TempData["hotelListOat"] = dicList["hotelinfolistOat"] as List<OALHotelModel>;

            //Send Email Notification  
            if (TempData["hotelListOat"] != null && TempData["bookingInfoOat"] != null)
            {
                var hotelList = TempData["hotelListOat"] as List<OALHotelModel>;
                var bookingInfo = TempData["bookingInfoOat"] as List<OALTravelRequestMasterModel>;
                var emailSubject = "SOD Hotel Booking Rejection Notification from Travel Desk :" + System.DateTime.Now.ToString();
                var emailTemplateName = "SodHotelBookingRequestNotificationTemplateFor_TravelDesk.html";
                var emailCredentials = EmailCredentialsHotelOat(emailSubject, emailTemplateName, hotelList, bookingInfo, req, "rejected");
                TempData["emailData"] = emailCredentials;
                TempData["emailId"] = bookingInfo[0].EmailId.Trim();

            }
        }

        /// <summary>
        /// get approver email id's for non contractual hotel approval
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetApproverIds()
        {
            var approverList = new List<NonContractualHotelApprovalMasterModels>();
            approverList = _transportRepository.GetApproverIds();

            return Json(approverList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// view detail for hod approval status
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult getdetailHODApproval()
        {
            var trId = Request.QueryString["trId"].ToString().Split('|')[0];
            var hId = Request.QueryString["trId"].ToString().Split('|')[1];
            var approverlist = new List<HotelRequestHODFinancialApprovalModels>();
            approverlist = _transportRepository.GetdetailHODApproval(Convert.ToInt64(trId), Convert.ToInt32(hId), "SOD");

            return Json(approverlist, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// view detail for hod approval status OAT
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult getdetailHODApprovalOat()
        {
            var trId = Request.QueryString["trId"].ToString().Split('|')[0];
            var hId = Request.QueryString["trId"].ToString().Split('|')[1];
            var approverlistOat = new List<HotelRequestHODFinancialApprovalModels>();
            approverlistOat = _transportRepository.GetdetailHODApproval(Convert.ToInt64(trId), Convert.ToInt32(hId), "OAT");

            return Json(approverlistOat, JsonRequestBehavior.AllowGet);
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
        }


        /// <summary>
        /// Initialized Email Credentials to Property
        /// </summary>
        /// <returns></returns>
        public EmailNotificationModel EmailCredentials(string subjectName, string emailTemplateName, List<TravelRequestCabDetailModels> cabdetails, List<TravelRequestMasterModels> bookingInfo, string reqNo, string approvalStatus)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtpHotelUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtptdPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtptdServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtptdPort"].Trim(),
                TemplateFilePath = ReadFile(emailTemplateName, cabdetails, bookingInfo, reqNo, approvalStatus),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
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

        public EmailNotificationModel EmailCredentialsHotelOat(string subjectName, string emailTemplateName, List<OALHotelModel> hoteldetails, List<OALTravelRequestMasterModel> bookingInfo, string reqNo, string approvalStatus)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtpHotelUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtptdPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtptdServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtptdPort"].Trim(),
                TemplateFilePath = ReadFileHotelOat(emailTemplateName, hoteldetails, bookingInfo, reqNo, approvalStatus),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
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

        /// <summary>
        /// Read Template File from Location
        /// </summary>
        /// <returns></returns>
        private string ReadFile(string emailTemplateName, List<TravelRequestCabDetailModels> cabdetails, List<TravelRequestMasterModels> bookingInfo, string reqNo, string approvalStatus)
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
            var CabTitle = "Cab Booking Details ";
            strContent = strContent.Replace("[RequesterName]", Title + " " + RequesterName);
            strContent = strContent.Replace("[binfo]", CabTitle + "  (Booking Request Id : " + reqNo + ")");
            strContent = strContent.Replace("[cabstatus]", approvalStatus);

            var tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Pick up Location (1)</td><td>Drop Location (1)</td><td>Pick up Location (2)</td><td>Drop Location (2)</td></tr>";
            //Adding Cab Info
            tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td>" + cabdetails[0].OneWay_From + "</td><td>" + cabdetails[0].OneWay_To + "</td><td>" + cabdetails[0].Return_From + "</td><td>" + cabdetails[0].Return_To + "</td></tr>";
            strContent = strContent.Replace("[tr]", tr);
            return strContent.ToString();
        }

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

            var hotelcityList = new List<SodCityCodeMasterModels>();
            hotelcityList = _transportRepository.FindCityName(hoteldetails[0].City);
            var hotelcity = hotelcityList[0].CityName;

            var tr = "";
            if (approvalStatus == "approved")
            {
                var uri1 = "";
                var uri2 = "";
                uri1 = ConfigurationManager.AppSettings["emailResponsivePathUser"].Trim() + "?trid=" + bookingInfo[0].TravelRequestId.ToString().Trim() + "&hid=" + hoteldetails[0].HotelRequestId.ToString();
                uri2 = ConfigurationManager.AppSettings["emailResponsivePathUserCancel"].Trim() + "?trid=" + bookingInfo[0].TravelRequestId.ToString().Trim() + "&hid=" + hoteldetails[0].HotelRequestId.ToString();
                var appLink = string.Empty;
                appLink = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Respond</a></td>";
                appLink = appLink + "<td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a style='color:#fff; text-decoration:none;' href='" + uri2 + "'>Cancel Hotel</a> </td></tr></table>";
                strContent = strContent.Replace("[appLink]", appLink);
                strContent = strContent.Replace("[appLinkText]", "Kindly respond with check-in & check-out time after your stay.");

                var shareduserlist = TempData["sharedUserList"] as List<TravelRequestMasterModels>;
                if (shareduserlist == null || shareduserlist.Count < 1)
                {
                    tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Hotel Confirmation No</td><td>City</td><td>Check-In Date</td><td>Check-Out Date</td><td>Hotel Information</td><td>No. of Guests</td></tr>";
                    tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td>" + hoteldetails[0].HotelConfirmationNo +
                        "</td><td>" + hotelcity + "</td><td>" + hoteldetails[0].CheckInDate.ToString("dd/MM/yyyy") + "</td><td>" + hoteldetails[0].CheckOutDate.ToString("dd/MM/yyyy") +
                        "</td><td>Name: " + hoteldetails[0].HotelName + "<br>Address: " + hoteldetails[0].HotelAddress + " <br>Phone No.: " + hoteldetails[0].HotelPhoneNo + "</td><td>"
                        + hoteldetails[0].NoOfGuest + "</td></tr>";
                }
                else
                {
                    tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Hotel Confirmation No</td><td>City</td><td>Check-In Date</td><td>Check-Out Date</td><td>Hotel Information</td><td>No. of Guests</td><td>Sharing With</td></tr>";
                    tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td>" + hoteldetails[0].HotelConfirmationNo +
                        "</td><td>" + hotelcity + "</td><td>" + hoteldetails[0].CheckInDate.ToString("dd/MM/yyyy") + "</td><td>" + hoteldetails[0].CheckOutDate.ToString("dd/MM/yyyy") +
                        "</td><td>Name: " + hoteldetails[0].HotelName + "<br>Address: " + hoteldetails[0].HotelAddress + " <br>Phone No.: " + hoteldetails[0].HotelPhoneNo + "</td>" +
                         "<td>" + "2" + "</td>" +
                        "<td>Employee Code: " + shareduserlist[0].RequestedEmpCode + "<br>Name: " + shareduserlist[0].RequestedEmpName + "<br>Phone No.: " + shareduserlist[0].Phno + "</td>" +
                        "</tr>";
                }

                if (hoteldetails[0].HotelType == "Contractual")
                {
                    var inclusionList = new List<HotelInclusionMasterModels>();
                    inclusionList = findHotelInclusions(hoteldetails[0].City, hoteldetails[0].HotelName);
                    if (inclusionList.Count > 0)
                    {
                        strContent = strContent.Replace("[hinfo]", "Hotel Inclusions Details");
                        var trI = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Accommodation</td><td>Food</td><td>Airport Transfers</td><td>Room service</td><td>Buffet Time</td><td>Laundry</td></tr>";

                        trI = trI + "<tr style='font-family:Arial; font-size:12px;'><td>" + inclusionList[0].Accomodation +
                                "</td><td>" + inclusionList[0].Food + "</td><td>" + inclusionList[0].AirportTransfers +
                                "</td><td>" + inclusionList[0].RoomService + "</td>" +
                                "<td>" + inclusionList[0].BuffetTime + "</td>" +
                                "<td>" + inclusionList[0].Laundry + "</td>" +
                                "</tr>";
                        strContent = strContent.Replace("[trI]", trI);
                    }
                    else
                    {
                        strContent = strContent.Replace("[hinfo]", "");
                        strContent = strContent.Replace("[trI]", "");
                    }
                }
                else
                {
                    var inclusionList = new List<HotelInclusionNonContractualMasterModels>();
                    inclusionList = _transportRepository.FindNonContractualHotelInclusions(hoteldetails[0].TravelRequestId, hoteldetails[0].HotelRequestId);
                    if (inclusionList.Count > 0)
                    {
                        strContent = strContent.Replace("[hinfo]", "Hotel Inclusions Details");
                        var trI = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Accommodation</td><td>Food</td><td>Airport Transfers</td><td>Room service</td><td>Buffet Time</td><td>Laundry</td></tr>";

                        trI = trI + "<tr style='font-family:Arial; font-size:12px;'><td>" + inclusionList[0].Accomodation +
                                "</td><td>" + inclusionList[0].Food + "</td><td>" + inclusionList[0].AirportTransfers +
                                "</td><td>" + inclusionList[0].RoomService + "</td>" +
                                "<td>" + inclusionList[0].BuffetTime + "</td>" +
                                "<td>" + inclusionList[0].Laundry + "</td>" +
                                "</tr>";
                        strContent = strContent.Replace("[trI]", trI);
                    }
                    else
                    {
                        strContent = strContent.Replace("[hinfo]", "");
                        strContent = strContent.Replace("[trI]", "");
                    }
                }
            }
            else
            {
                strContent = strContent.Replace("[hinfo]", "");
                strContent = strContent.Replace("[trI]", "");
                tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Hotel Reference Id</td><td>City</td><td>Check-In Date</td><td>Check-Out Date</td><td>No. of Guests</td></tr>";
                tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td>" + hoteldetails[0].HotelReferenceID + "</td><td>" + hotelcity + "</td><td>" + hoteldetails[0].CheckInDate.ToString("dd/MM/yyyy") + "</td><td>" + hoteldetails[0].CheckOutDate.ToString("dd/MM/yyyy") + "</td><td>" + hoteldetails[0].NoOfGuest + "</td></tr>";

                strContent = strContent.Replace("[appLink]", "");
                strContent = strContent.Replace("[appLinkText]", "");

            }
            strContent = strContent.Replace("[tr]", tr);
            return strContent.ToString();
        }

        private string ReadFileHotelOat(string emailTemplateName, List<OALHotelModel> hoteldetails, List<OALTravelRequestMasterModel> bookingInfo, string reqNo, string approvalStatus)
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

            var Title = "";
            if (bookingInfo[0].Gender == "M") { Title = "Mr."; }
            else { Title = "Ms."; }

            var RequesterName = bookingInfo[0].RequestedEmpName;
            var HotelTitle = "Hotel Booking Details ";
            strContent = strContent.Replace("[RequesterName]", Title + " " + RequesterName);
            strContent = strContent.Replace("[binfo]", HotelTitle + "  (Booking Request Id : " + reqNo + ")");
            strContent = strContent.Replace("[hotelstatus]", approvalStatus);

            var hotelcityList = new List<SodCityCodeMasterModels>();
            hotelcityList = _transportRepository.FindCityName(hoteldetails[0].City);
            var hotelcity = hotelcityList[0].CityName;

            var tr = "";
            if (approvalStatus == "approved")
            {
                var shareduserlist = TempData["sharedUserListOat"] as List<TravelRequestMasterModels>;
                if (shareduserlist == null || shareduserlist.Count < 1)
                {
                    tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Hotel Confirmation No.</td><td>City</td><td>Check-In Date</td><td>Check-Out Date</td><td>Hotel Information</td><td>No. of Guests</td></tr>";
                    tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td>" + hoteldetails[0].HotelConfirmationNo +
                        "</td><td>" + hoteldetails[0].City + "</td><td>" + hoteldetails[0].CheckInDate.ToString("dd/MM/yyyy") + "</td><td>" + hoteldetails[0].CheckOutDate.ToString("dd/MM/yyyy") +
                        "</td><td>Name: " + hotelcity + "<br>Address: " + hoteldetails[0].HotelAddress + " <br>Phone No.: " + hoteldetails[0].HotelPhoneNo + "</td><td>"
                        + hoteldetails[0].guests + "</td></tr>";
                }
                else
                {
                    tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Hotel Confirmation No</td><td>City</td><td>Check-In Date</td><td>Check-Out Date</td><td>Hotel Information</td><td>No. of Guests</td><td>Sharing With</td></tr>";
                    tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td>" + hoteldetails[0].HotelConfirmationNo +
                        "</td><td>" + hotelcity + "</td><td>" + hoteldetails[0].CheckInDate.ToString("dd/MM/yyyy") + "</td><td>" + hoteldetails[0].CheckOutDate.ToString("dd/MM/yyyy") +
                        "</td><td>Name: " + hoteldetails[0].HotelName + "<br>Address: " + hoteldetails[0].HotelAddress + " <br>Phone No.: " + hoteldetails[0].HotelPhoneNo + "</td>" +
                         "<td>" + "2" + "</td>" +
                        "<td>Employee Code: " + shareduserlist[0].RequestedEmpCode + "<br>Name: " + shareduserlist[0].RequestedEmpName + "<br>Phone No.: " + shareduserlist[0].Phno + "</td>" +
                        "</tr>";
                }
                if (hoteldetails[0].HotelType == "Contractual")
                {
                    var inclusionList = new List<HotelInclusionMasterModels>();
                    inclusionList = findHotelInclusions(hoteldetails[0].City, hoteldetails[0].HotelName);
                    if (inclusionList.Count > 0)
                    {
                        strContent = strContent.Replace("[hinfo]", "Hotel Inclusions Details");
                        var trI = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Accommodation</td><td>Food</td><td>Airport Transfers</td><td>Room service</td><td>Buffet Time</td><td>Laundry</td></tr>";

                        trI = trI + "<tr style='font-family:Arial; font-size:12px;'><td>" + inclusionList[0].Accomodation +
                                "</td><td>" + inclusionList[0].Food + "</td><td>" + inclusionList[0].AirportTransfers +
                                "</td><td>" + inclusionList[0].RoomService + "</td>" +
                                "<td>" + inclusionList[0].BuffetTime + "</td>" +
                                "<td>" + inclusionList[0].Laundry + "</td>" +
                                "</tr>";
                        strContent = strContent.Replace("[trI]", trI);
                    }
                    else
                    {
                        strContent = strContent.Replace("[hinfo]", "");
                        strContent = strContent.Replace("[trI]", "");
                    }
                }
                else
                {
                    var inclusionList = new List<HotelInclusionNonContractualMasterOatModels>();
                    inclusionList = _transportRepository.FindNonContractualHotelInclusionsOat(hoteldetails[0].TravelRequestId);
                    if (inclusionList.Count > 0)
                    {
                        strContent = strContent.Replace("[hinfo]", "Hotel Inclusions Details");
                        var trI = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Accommodation</td><td>Food</td><td>Airport Transfers</td><td>Room service</td><td>Buffet Time</td><td>Laundry</td></tr>";

                        trI = trI + "<tr style='font-family:Arial; font-size:12px;'><td>" + inclusionList[0].Accomodation +
                                "</td><td>" + inclusionList[0].Food + "</td><td>" + inclusionList[0].AirportTransfers +
                                "</td><td>" + inclusionList[0].RoomService + "</td>" +
                                "<td>" + inclusionList[0].BuffetTime + "</td>" +
                                "<td>" + inclusionList[0].Laundry + "</td>" +
                                "</tr>";
                        strContent = strContent.Replace("[trI]", trI);
                    }
                    else
                    {
                        strContent = strContent.Replace("[hinfo]", "");
                        strContent = strContent.Replace("[trI]", "");
                    }
                }
            }
            else
            {
                strContent = strContent.Replace("[hinfo]", "");
                strContent = strContent.Replace("[trI]", "");
                tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Hotel HotelReferenceID</td><td>City</td><td>Check-In Date</td><td>Check-Out Date</td><td>No. of Guests</td></tr>";
                tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td>" + hoteldetails[0].HotelReferenceId + "</td><td>" + hotelcity + "</td><td>" + hoteldetails[0].CheckInDate.ToString("dd/MM/yyyy") + "</td><td>" + hoteldetails[0].CheckOutDate.ToString("dd/MM/yyyy") + "</td><td>" + hoteldetails[0].guests + "</td></tr>";
            }
            strContent = strContent.Replace("[tr]", tr);
            return strContent.ToString();
        }


        /// <summary>
        /// Send Email to Hotel from Travel Desk to Hotel for Booking Request
        /// </summary>
        /// <param name="emailTemplateName"></param>
        /// <param name="requestList"></param>
        /// <param name="hidList"></param>
        /// <param name="hotelname"></param>
        /// <param name="sodOat"></param>
        /// <param name="mailtype"></param>
        /// <returns></returns>
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
            strContent = strContent.Replace("[binfo]", "Booking Details (Request ID :" + requestList[0].ToString() + ")");

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

            if (passengerInfo[0].HotelType.ToLower() == "non-contractual")
            {
                HotelPriceString = "<td style='height:20px; padding-bottom:8px;'>Price</td>";
                strContent = strContent.Replace("[retention]", "No Retention Charges Applicable on Cancellation.");
            }
            else
            {
                strContent = strContent.Replace("[retention]", "");
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
                if (p.HotelType.ToLower() == "non-contractual")
                {
                    hotelpriceNon_cont = "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.HotelPrice + " (" + p.HotelCurrencyCode + ") ";
                }
                if (p.Title == "Ms." || p.Title == "Ms")
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
                if (ETACheckin == "00:00 AM")
                {
                    ETACheckin = "NA";
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
                        "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.RequestedEmpName;
                if (p.RequestedEmpId == 0)
                {
                    tr = tr + "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.RequestedEmpCode;
                }
                else
                {
                    tr = tr + "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.RequestedEmpId;
                }

                tr = tr + "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.CheckinDate.ToString("dd-MMM-yyyy") +
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
            //Inclusion for NON-Contractual Hotel
            if (passengerInfo[0].HotelType.ToLower() == "non-contractual")
            {
                var inclusionList = new List<HotelInclusionNonContractualMasterModels>();
                inclusionList = _transportRepository.FindNonContractualHotelInclusions(passengerInfo[0].TravelRequestId, Convert.ToInt32(hidList[0]));
                if (inclusionList.Count > 0)
                {
                    strContent = strContent.Replace("[hinfo]", "Hotel Inclusions Details");
                    var trI = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Accommodation</td><td>Food</td><td>Airport Transfers</td><td>Room service</td><td>Buffet Time</td><td>Laundry</td></tr>";

                    trI = trI + "<tr style='font-family:Arial; font-size:12px;'><td>" + inclusionList[0].Accomodation +
                            "</td><td>" + inclusionList[0].Food + "</td><td>" + inclusionList[0].AirportTransfers +
                            "</td><td>" + inclusionList[0].RoomService + "</td>" +
                            "<td>" + inclusionList[0].BuffetTime + "</td>" +
                            "<td>" + inclusionList[0].Laundry + "</td>" +
                            "</tr>";
                    strContent = strContent.Replace("[trI]", trI);
                }
                else
                {
                    strContent = strContent.Replace("[hinfo]", "");
                    strContent = strContent.Replace("[trI]", "");
                }
            }

            return strContent.ToString();
        }


        /// <summary>
        /// fill hotel names from database
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult HotelListData()
        {
            List<SelectListItem> hotelItems = new List<SelectListItem>();
            SodEntities entities = new SodEntities();
            var count = entities.SodHotelListDataModels.Count();
            for (int i = 0; i < count; i++)
            {
                hotelItems.Add(new SelectListItem
                {
                    Value = entities.SodHotelListDataModels.ToList()[i].HotelCode,
                    Text = entities.SodHotelListDataModels.ToList()[i].HotelName + "-" + entities.SodHotelListDataModels.ToList()[i].StationCode
                });
            }

            return Json(hotelItems);
        }

        /// <summary>
        /// fill dropdown of hotel information
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult hotelfilldropdown(string name)
        {
            var dicList = new Dictionary<string, object>();
            var hoteldetails = new List<SodHotelListDataModels>();
            var pricedetails = new List<SodHotelPriceListMasterModels>();

            dicList = _transportRepository.hotelfilldropdown(name);

            hoteldetails = dicList["hoteldetails"] as List<SodHotelListDataModels>;
            pricedetails = dicList["pricedetails"] as List<SodHotelPriceListMasterModels>;
            return Json(dicList, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Send request to hotel for acceptance/rejection
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SendHotelRequest(List<HotelRequestApprovalModel> elist, string hotelprice, string occupancy, string sodOat)
        {
            var requestList = new List<String>();
            var hIdlist = new List<String>();
            var Exclubid = 0;
            var confirmNo = "";
            for (var i = 0; i < elist.Count; i++)
            {
                var rqId = elist[i].TravelRequestId;
                var hId = elist[i].HotelRequestId;
                var dichotel = new Dictionary<string, object>();
                dichotel = _transportRepository.GetHotelDetailbyTrID(rqId, hId);
                var hotelList = dichotel["hotelinfobyTrid"] as List<HotelRequestApprovalModel>;
                if (hotelList == null || hotelList.Count < 1)
                {

                }
                else
                {
                    Exclubid = hotelList[0].clubId;
                    confirmNo = hotelList[0].HotelConfirmationNo;
                }
                requestList.Add(rqId.ToString());
                hIdlist.Add(hId.ToString());
            }
            var primaryemail = elist[0].PrimaryEmail;
            var hotelname = elist[0].HotelName;
            var remarks = elist[0].Remarks;
            var _context = new SodEntities();

            var clubMaxId = 0;
            if (sodOat == "SOD")
            {
                clubMaxId = _context.HotelRequestApprovalModel.DefaultIfEmpty().Max(x => x.clubId == null ? 1 : x.clubId + 1);
            }
            else
            {
                clubMaxId = _context.HotelRequestApprovalOatModel.DefaultIfEmpty().Max(x => x.clubId == null ? 1 : x.clubId + 1);
            }

            if (Exclubid == 0 && confirmNo == "")
            {
                foreach (var e in elist)
                {
                    e.clubId = clubMaxId;
                }
            }
            else
            {
                foreach (var e in elist)
                {
                    e.clubId = Exclubid;
                    e.HotelConfirmationNo = confirmNo;
                }
            }

            var s1 = _transportRepository.SaveHotelRequest(elist, hotelprice, occupancy, sodOat);
            var s = elist[0].clubId;
            if (s1 > 0)
            {
                if (primaryemail.Length > 0)
                {
                    var emailSubject = "SOD Hotel Request Notification from Spicejet:" + System.DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt");
                    var emailTemplateName = "HotelBookingRequestNotificationTemplateFor_Contractual.html";
                    var emailCredentials = EmailCredentialsHotelRequest(emailSubject, emailTemplateName, requestList, hIdlist, hotelname, sodOat, "hotel");

                    var skey = new StringBuilder();
                    skey.Append(s.ToString() + ",");
                    skey.Append(primaryemail);

                    var uri1 = "";
                    var uri2 = "";
                    if (sodOat == "SOD")
                    {
                        uri1 = ConfigurationManager.AppSettings["emailApprovalPathHotelCode"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=a&trid=" + s.ToString() + "&hotelname=" + hotelname);
                        uri2 = ConfigurationManager.AppSettings["emailApprovalPathHotelCode"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=r&trid=" + s.ToString() + "&hotelname=" + hotelname);
                    }
                    else
                    {
                        uri1 = ConfigurationManager.AppSettings["emailApprovalPathHotelCode"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=b&trid=" + s.ToString() + "&hotelname=" + hotelname);
                        uri2 = ConfigurationManager.AppSettings["emailApprovalPathHotelCode"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=c&trid=" + s.ToString() + "&hotelname=" + hotelname);
                    }

                    var appLink = string.Empty;
                    appLink = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Acceptance</a></td>";
                    appLink = appLink + "<td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a style='color:#fff; text-decoration:none;' href='" + uri2 + "'>Rejection</a> </td></tr></table>";

                    var templateData = emailCredentials.TemplateFilePath;
                    templateData = templateData.Replace("[appLink]", appLink);
                    templateData = templateData.Replace("[hotelName]", "<b>" + hotelname + "</b>");
                    templateData = templateData.Replace("[remarksTravelDesk]", remarks);
                    if (elist[0].HotelType.ToLower() == "contractual")
                    {
                        //Add Inclusion Details to Hotel by Satyam
                        var inclusionList = new List<HotelInclusionMasterModels>();
                        var citycode = _transportRepository.GetCityCodeOfHotel(elist[0].HotelCode);
                        inclusionList = findHotelInclusions(citycode, elist[0].HotelName);
                        if (inclusionList.Count > 0)
                        {
                            templateData = templateData.Replace("[hinfo]", "Hotel Inclusions Details");
                            var trI = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Accommodation</td><td>Food</td><td>Airport Transfers</td><td>Room service</td><td>Buffet Time</td><td>Laundry</td></tr>";

                            trI = trI + "<tr style='font-family:Arial; font-size:12px;'><td>" + inclusionList[0].Accomodation +
                                    "</td><td>" + inclusionList[0].Food + "</td><td>" + inclusionList[0].AirportTransfers +
                                    "</td><td>" + inclusionList[0].RoomService + "</td>" +
                                    "<td>" + inclusionList[0].BuffetTime + "</td>" +
                                    "<td>" + inclusionList[0].Laundry + "</td>" +
                                    "</tr>";
                            templateData = templateData.Replace("[trI]", trI);
                        }
                        else
                        {
                            templateData = templateData.Replace("[hinfo]", "");
                            templateData = templateData.Replace("[trI]", "");
                        }
                        var gstdetails = _transportRepository.GetGstDetailsByCityCode(citycode) as List<HotelGstDetailModels>;
                        if (gstdetails.Count > 0)
                        {
                            templateData = templateData.Replace("[gstdetails]", "Please raise invoice against the following details: ");
                            templateData = templateData.Replace("[gstno]", "GST No : <b>" + gstdetails[0].GSTIN + "</b>");
                            templateData = templateData.Replace("[gstaddress]", "Address : <b>" + gstdetails[0].PlaceOfBusiness + "</b>");
                        }
                        else
                        {
                            templateData = templateData.Replace("[gstdetails]", "");
                            templateData = templateData.Replace("[gstno]", "");
                            templateData = templateData.Replace("[gstaddress]", "");
                        }
                    }
                    else
                    {
                        templateData = templateData.Replace("[gstdetails]", "");
                        templateData = templateData.Replace("[gstno]", "");
                        templateData = templateData.Replace("[gstaddress]", "");
                    }
                    emailCredentials.TemplateFilePath = templateData;
                    TempData["emailData_Hod"] = emailCredentials;
                    //by sony cc
                    TempData["emailId_Hod"] = primaryemail;
                    var sEmail = elist[0].SecondaryEmail.Split(',');
                    string[] Sec_Email = new string[sEmail.Count() + 1];
                    Sec_Email[0] = ConfigurationManager.AppSettings["TravelDeskEmailId"].ToString();
                    if (sEmail.Count() > 0)
                    {
                        var i = 1;
                        foreach (var se in sEmail)
                        {
                            Sec_Email[i] = se;
                            i++;
                        }
                    }
                    EmailNotifications.SendBookingRequestNotificationTo_InCC(emailCredentials, primaryemail, Sec_Email);
                    TempData["msgResponse"] = "Hotel Request Notification : Hotel details have been sent successfully. The request has been sent to the respected hotel at  " + primaryemail.ToString();
                }
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }


        public void GetHotelRequestData(List<String> requestList, List<String> hidList, string hotelname, string sodOat)
        {
            var dicList = new Dictionary<string, object>();
            dicList = _transportRepository.GetHotelandUserInfo(requestList, hidList, hotelname, sodOat);
            TempData["PassengerInfoForHotelRequest"] = dicList["PassengerInfoForHotelRequest"] as List<TravelRequestModels>;
        }


        /// <summary>
        /// resend reminder request to hotel for acceptance/rejection
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ResendHotelRequest(List<HotelRequestApprovalModel> elist, string sodOat)
        {
            var requestList = new List<String>();
            var hidList = new List<String>();
            for (var i = 0; i < elist.Count; i++)
            {
                var rqId = elist[i].TravelRequestId;
                requestList.Add(rqId.ToString());

                var hId = elist[i].HotelRequestId;
                hidList.Add(hId.ToString());
            }

            var primaryemail = "";
            var secondaryemail = "";
            var hotelname = "";
            var remarks = "";
            var s = 0;
            var dic = new Dictionary<string, object>();
            dic = _transportRepository.getApprovalHotelDetails(elist[0].TravelRequestId.ToString(), elist[0].HotelRequestId, sodOat);
            var hotelcode = "";
            var hoteltype = "";
            if (sodOat == "SOD")
            {
                var rlist = new List<HotelRequestApprovalModel>();
                rlist = dic["approvalHotelDetails"] as List<HotelRequestApprovalModel>;
                primaryemail = rlist[0].PrimaryEmail.Trim();
                if (rlist[0].SecondaryEmail!=null)
                secondaryemail = rlist[0].SecondaryEmail.Trim();
                hotelname = rlist[0].HotelName;
                remarks = rlist[0].Remarks;
                s = rlist[0].clubId;
                hotelcode = rlist[0].HotelCode;
                hoteltype = rlist[0].HotelType;
            }
            else
            {
                var rlist = new List<HotelRequestApprovalOatModels>();
                rlist = dic["approvalHotelDetails"] as List<HotelRequestApprovalOatModels>;
                primaryemail = rlist[0].PrimaryEmail;
                if (rlist[0].SecondaryEmail != null)
                secondaryemail = rlist[0].SecondaryEmail;
                hotelname = rlist[0].HotelName;
                remarks = rlist[0].Remarks;
                s = rlist[0].clubId;
                hotelcode = rlist[0].HotelCode;
                hoteltype = rlist[0].HotelType;
            }

            if (primaryemail !="")
            {
                var emailSubject = "SOD Hotel Request Notification from Spicejet:" + System.DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt");
                var emailTemplateName = "HotelBookingRequestNotificationTemplateFor_Contractual.html";
                var emailCredentials = EmailCredentialsHotelRequest(emailSubject, emailTemplateName, requestList, hidList, hotelname, sodOat, "hotel");

                var skey = new StringBuilder();
                skey.Append(s.ToString() + ",");
                skey.Append(primaryemail);

                var uri1 = "";
                var uri2 = "";
                if (sodOat == "SOD")
                {
                    uri1 = ConfigurationManager.AppSettings["emailApprovalPathHotelCode"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=a&trid=" + s.ToString() + "&hotelname=" + hotelname);
                    uri2 = ConfigurationManager.AppSettings["emailApprovalPathHotelCode"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=r&trid=" + s.ToString() + "&hotelname=" + hotelname);
                }
                else
                {
                    uri1 = ConfigurationManager.AppSettings["emailApprovalPathHotelCode"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=b&trid=" + s.ToString() + "&hotelname=" + hotelname);
                    uri2 = ConfigurationManager.AppSettings["emailApprovalPathHotelCode"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=c&trid=" + s.ToString() + "&hotelname=" + hotelname);
                }
                // var essurl = ConfigurationManager.AppSettings["essportalUrl"].Trim();
                var appLink = string.Empty;
                appLink = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Acceptance</a></td>";
                appLink = appLink + "<td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a style='color:#fff; text-decoration:none;' href='" + uri2 + "'>Rejection</a> </td></tr></table>";

                var templateData = emailCredentials.TemplateFilePath;
                templateData = templateData.Replace("[appLink]", appLink);
                templateData = templateData.Replace("[hotelName]", hotelname);
                templateData = templateData.Replace("[remarksTravelDesk]", remarks);

                if (hoteltype.ToLower() == "contractual")
                {
                    //Add Inclusion Details to Hotel by Satyam
                    var inclusionList = new List<HotelInclusionMasterModels>();
                    var citycode = _transportRepository.GetCityCodeOfHotel(hotelcode);
                    inclusionList = findHotelInclusions(citycode, hotelname);
                    if (inclusionList.Count > 0)
                    {
                        templateData = templateData.Replace("[hinfo]", "Hotel Inclusions Details");
                        var trI = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Accommodation</td><td>Food</td><td>Airport Transfers</td><td>Room service</td><td>Buffet Time</td><td>Laundry</td></tr>";

                        trI = trI + "<tr style='font-family:Arial; font-size:12px;'><td>" + inclusionList[0].Accomodation +
                                "</td><td>" + inclusionList[0].Food + "</td><td>" + inclusionList[0].AirportTransfers +
                                "</td><td>" + inclusionList[0].RoomService + "</td>" +
                                "<td>" + inclusionList[0].BuffetTime + "</td>" +
                                "<td>" + inclusionList[0].Laundry + "</td>" +
                                "</tr>";
                        templateData = templateData.Replace("[trI]", trI);
                    }
                    else
                    {
                        templateData = templateData.Replace("[hinfo]", "");
                        templateData = templateData.Replace("[trI]", "");
                    }

                    var gstdetails = _transportRepository.GetGstDetailsByCityCode(citycode) as List<HotelGstDetailModels>;
                    if (gstdetails.Count > 0)
                    {
                        templateData = templateData.Replace("[gstdetails]", "Please raise invoice against the following details: ");
                        templateData = templateData.Replace("[gstno]", "GST No : <b>" + gstdetails[0].GSTIN + "</b>");
                        templateData = templateData.Replace("[gstaddress]", "Address : <b>" + gstdetails[0].PlaceOfBusiness + "</b>");
                    }
                    else
                    {
                        templateData = templateData.Replace("[gstdetails]", "");
                        templateData = templateData.Replace("[gstno]", "");
                        templateData = templateData.Replace("[gstaddress]", "");
                    }
                }
                else
                {
                    templateData = templateData.Replace("[gstdetails]", "");
                    templateData = templateData.Replace("[gstno]", "");
                    templateData = templateData.Replace("[gstaddress]", "");

                }
                emailCredentials.TemplateFilePath = templateData;
                TempData["emailData_Hod"] = emailCredentials;
                TempData["emailId_Hod"] = primaryemail;
                if (secondaryemail !="")
                {
                    var sEmail = secondaryemail.Split(',');
                    string[] Sec_Email = new string[sEmail.Count() + 1];
                    Sec_Email[0] = ConfigurationManager.AppSettings["TravelDeskEmailId"].ToString();
                    if (sEmail.Count() > 0)
                    {
                        var i = 1;
                        foreach (var se in sEmail)
                        {
                            Sec_Email[i] = se;
                            i++;
                        }
                    }
                    EmailNotifications.SendBookingRequestNotificationTo_InCC(emailCredentials, primaryemail, Sec_Email);
                }
                else
                {
                    string[] Sec_Email = new string[1];
                    Sec_Email[0] = ConfigurationManager.AppSettings["TravelDeskEmailId"].ToString();
                    EmailNotifications.SendBookingRequestNotificationTo_InCC(emailCredentials, primaryemail, Sec_Email);
                }
                TempData["msgResponse"] = "Hotel Request Notification : Hotel details have been sent successfully. The request has been sent to the respected hotel at  " + primaryemail.ToString();
            }

            return Json(s, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// send cancellation request to hotel
        /// </summary>
        /// <param name="travelRqstId"></param>
        [HttpPost]
        public JsonResult hotelCancellationRequest(string travelRqstId)
        {
            var travReqstId = travelRqstId.Split('|')[0];
            var hid = travelRqstId.Split('|')[1];
            var sodOat = travelRqstId.Split('|')[2];
            string hotelname = "";
            string hotelnamedetails = "";
            var primaryEmail = "";

            if (sodOat == "SOD")
            {
                var dicListHotel = new Dictionary<string, object>();
                var hoteldetails = new List<HotelRequestApprovalModel>();
                dicListHotel = _transportRepository.getApprovalHotelDetails(travReqstId, Convert.ToInt32(hid), sodOat);
                hoteldetails = dicListHotel["approvalHotelDetails"] as List<HotelRequestApprovalModel>;
                hotelname = hoteldetails[0].HotelName;
                hotelnamedetails = hoteldetails[0].HotelName + " - " + hoteldetails[0].HotelAddress;
                primaryEmail = hoteldetails[0].PrimaryEmail;
            }
            else
            {
                var dicListHotel = new Dictionary<string, object>();
                var hoteldetails = new List<HotelRequestApprovalOatModels>();
                dicListHotel = _transportRepository.getApprovalHotelDetails(travReqstId, Convert.ToInt32(hid), sodOat);
                hoteldetails = dicListHotel["approvalHotelDetails"] as List<HotelRequestApprovalOatModels>;
                hotelname = hoteldetails[0].HotelName;
                hotelnamedetails = hoteldetails[0].HotelName + " - " + hoteldetails[0].HotelAddress;
                primaryEmail = hoteldetails[0].PrimaryEmail;
            }

            var s = _transportRepository.SaveCancellationRequest(travReqstId, Convert.ToInt32(hid), sodOat);

            try
            {
                List<String> listhotelInfo = new List<String>();
                listhotelInfo.Add(travReqstId);

                List<String> hidList = new List<String>();
                hidList.Add(hid);

                var dicList = new Dictionary<string, object>();
                dicList = _transportRepository.GetHotelandUserInfo(listhotelInfo, hidList, hotelname, sodOat);
                TempData["PassengerInfoForHotelRequest"] = dicList["PassengerInfoForHotelRequest"] as List<TravelRequestModels>;
                var hotelCancellationInfo = TempData["PassengerInfoForHotelRequest"] as List<TravelRequestModels>;

                var username = hotelCancellationInfo[0].RequestedEmpName;
                var useremail = hotelCancellationInfo[0].PassEmailId;

                if (primaryEmail.Length > 0 && useremail.Length > 0)
                {
                    //send cancellation request to hotel
                    var emailSubject = "SOD Hotel Cancellation Request Notification from Spicejet:" + System.DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt");
                    var emailTemplateName = "HotelBookingRequestNotificationTemplate_Cancellation.html";
                    var emailCredentials = EmailCredentialsHotelRequest(emailSubject, emailTemplateName, listhotelInfo, hidList, hotelnamedetails, sodOat, "hotel");
                    var templateData = emailCredentials.TemplateFilePath;
                    templateData = templateData.Replace("[hotelName]", "<b> " + hotelname + " </b><br/> " + hotelnamedetails);
                    emailCredentials.TemplateFilePath = templateData;
                    TempData["emailData_Hod"] = emailCredentials;
                    TempData["emailId_Hod"] = primaryEmail;
                    EmailNotifications.SendBookingRequestNotificationTo_Requester_Traveldesk(emailCredentials, primaryEmail);
                    TempData["msgResponse"] = "Hotel Request for Travel Request Id " + travReqstId.ToString() + " : Hotel cancellation request has been sent successfully. The request has been sent to the respected hotel at  " + primaryEmail;

                }
                return Json("RequestSent", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// send cancellation request to hotel by user
        /// </summary>
        /// <param name="travelRqstId"></param>
        [HttpPost]
        public JsonResult hotelCancellationRequestUser(string travelRqstId)
        {
            var travReqstId = travelRqstId.Split('|')[0];
            var hid = travelRqstId.Split('|')[1];
            var sodOat = travelRqstId.Split('|')[2];
            string hotelname = "";
            var primaryEmail = "";

            if (sodOat == "SOD")
            {
                var dicListHotel = new Dictionary<string, object>();
                var hoteldetails = new List<HotelRequestApprovalModel>();
                dicListHotel = _transportRepository.getApprovalHotelDetails(travReqstId, Convert.ToInt32(hid), sodOat);
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
                dicListHotel = _transportRepository.getApprovalHotelDetails(travReqstId, Convert.ToInt32(hid), sodOat);
                hoteldetails = dicListHotel["approvalHotelDetails"] as List<HotelRequestApprovalOatModels>;
                if (hoteldetails.Count > 0)
                {
                    hotelname = hoteldetails[0].HotelName;
                    primaryEmail = hoteldetails[0].PrimaryEmail;
                }
            }

            var s = _transportRepository.SaveCancellationRequestUser(travReqstId, Convert.ToInt32(hid), sodOat);

            try
            {
                List<String> listhotelInfo = new List<String>();
                listhotelInfo.Add(travReqstId);

                List<String> hidList = new List<String>();
                hidList.Add(hid);

                var dicList = new Dictionary<string, object>();
                dicList = _transportRepository.GetHotelandUserInfo(listhotelInfo, hidList, hotelname, sodOat);
                TempData["PassengerInfoForHotelRequest"] = dicList["PassengerInfoForHotelRequest"] as List<TravelRequestModels>;
                var hotelCancellationInfo = TempData["PassengerInfoForHotelRequest"] as List<TravelRequestModels>;

                var username = hotelCancellationInfo[0].RequestedEmpName;
                var useremail = hotelCancellationInfo[0].PassEmailId;

                if (primaryEmail.Length > 0 && useremail.Length > 0)
                {
                    //send cancellation request to hotel
                    var emailSubject = "SOD Hotel Cancellation Request Notification from Spicejet:" + System.DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt");
                    var emailTemplateName = "HotelBookingRequestNotificationTemplate_Cancellation.html";
                    var emailCredentials = EmailCredentialsHotelRequest(emailSubject, emailTemplateName, listhotelInfo, hidList, hotelname, sodOat, "hotel");
                    var templateData = emailCredentials.TemplateFilePath;
                    templateData = templateData.Replace("[hotelName]", hotelname);
                    emailCredentials.TemplateFilePath = templateData;
                    TempData["emailData_Hod"] = emailCredentials;
                    TempData["emailId_Hod"] = primaryEmail;
                    EmailNotifications.SendBookingRequestNotificationTo_Requester_Traveldesk(emailCredentials, primaryEmail);
                    TempData["msgResponse"] = "Hotel Request for Travel Request Id " + travReqstId.ToString() + " : Hotel cancellation request has been sent successfully. The request has been sent to the respected hotel at  " + primaryEmail;

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

                }
                return Json("RequestSent", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Send request to HOD for financial approval non-contractual
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult sendFinancialApprovalRequest(List<HotelRequestApprovalModel> elist, string hodemail1, string hodemail2, List<HotelInclusionNonContractualMasterModels> inclist, string sodOat)
        {
            var requestList = new List<String>();
            var hidList = new List<String>();
            var s = "";
            var s2 = "";
            for (var i = 0; i < elist.Count; i++)
            {
                var rqId = elist[i].TravelRequestId;
                requestList.Add(rqId.ToString());
                s = s + rqId.ToString() + ",";

                var hid = elist[i].HotelRequestId;
                hidList.Add(hid.ToString());
                s2 = s2 + hid.ToString() + ",";
            }
            var hotelname = elist[0].HotelName;
            var _context = new SodEntities();
            var clubMaxId = 0;
            var hotelInfo = new List<TravelRequestHotelDetailModels>();
            var travelInfo = new List<TravelRequestMasterModels>();
            if (sodOat == "SOD")
            {
                var trId = elist[0].TravelRequestId;
                clubMaxId = _context.HotelRequestApprovalModel.DefaultIfEmpty().Max(x => x.clubId == null ? 1 : x.clubId + 1);
                hotelInfo = _context.TravelRequestHotelDetailModel.Where(o => o.TravelRequestId == trId).ToList();
                travelInfo = _context.TravelRequestMasterModel.Where(o => o.TravelRequestId == trId).ToList();
            }
            else
            {
                clubMaxId = _context.HotelRequestApprovalOatModel.DefaultIfEmpty().Max(x => x.clubId == null ? 1 : x.clubId + 1);
            }
            foreach (var e in elist)
            {
                e.clubId = clubMaxId;
            }

            var s1 = _transportRepository.SaveHODFinancialApprovalRequest(elist, hodemail1, hodemail2, inclist, sodOat);
            if (s1 > 0)
            {
                if (hodemail1.Length > 0)
                {
                    var emailSubject = "Non-Contractual Hotel Request Notification from Travel desk:" + System.DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt");
                    var emailTemplateName = "HotelHODApprovalNotificationTemplateFor_NonContractual.html";
                    var emailCredentials = EmailCredentialsHODApprovalRequest(emailSubject, emailTemplateName, hotelname, requestList, hidList, elist, inclist, sodOat);
                    var emailCredentials2 = EmailCredentialsHODApprovalRequest(emailSubject, emailTemplateName, hotelname, requestList, hidList, elist, inclist, sodOat);

                    var skey = new StringBuilder();
                    skey.Append(s.ToString() + ",");
                    skey.Append(hodemail1);

                    var uri1 = "";
                    var uri2 = "";
                    var uri3 = "";
                    var uri4 = "";

                    var hodlist = _transportRepository.GetHodApproverNameHotels(hodemail1);
                    if (sodOat == "SOD")
                    {
                        uri1 = ConfigurationManager.AppSettings["emailApprovalPathHODCodeApp"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=a&trid=" + s.ToString() + "&empcode=" + hodlist[0].EmpCode.Trim() + "&hid=" + s2.ToString());
                        uri2 = ConfigurationManager.AppSettings["emailApprovalPathHODCodeApp"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=r&trid=" + s.ToString() + "&empcode=" + hodlist[0].EmpCode.Trim() + "&hid=" + s2.ToString());
                    }
                    else
                    {
                        uri1 = ConfigurationManager.AppSettings["emailApprovalPathHODCodeApp"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=b&trid=" + s.ToString() + "&empcode=" + hodlist[0].EmpCode.Trim());
                        uri2 = ConfigurationManager.AppSettings["emailApprovalPathHODCodeApp"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=c&trid=" + s.ToString() + "&empcode=" + hodlist[0].EmpCode.Trim());
                    }
                    var appLink = string.Empty;
                    appLink = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Acceptance</a></td>";
                    appLink = appLink + "<td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a style='color:#fff; text-decoration:none;' href='" + uri2 + "'>Rejection</a> </td></tr></table>";

                    var templateData = emailCredentials.TemplateFilePath;
                    templateData = templateData.Replace("[appLink]", appLink);
                    templateData = templateData.Replace("[HodName]", hodlist[0].EmpName.ToString());
                    emailCredentials.TemplateFilePath = templateData;
                    TempData["emailData_Hod"] = emailCredentials;
                    TempData["emailId_Hod"] = hodemail1;
                    EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials, hodemail1);
                    //SMS for Approval 1		
                    SendSMSforApproval(hodlist[0], travelInfo[0], hotelInfo, uri1, uri2);

                    if (hodemail2.Length > 0)
                    {
                        var hodlist2 = _transportRepository.GetHodApproverNameHotels(hodemail2);
                        if (sodOat == "SOD")
                        {
                            uri3 = ConfigurationManager.AppSettings["emailApprovalPathHODCodeApp"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=a&trid=" + s.ToString() + "&empcode=" + hodlist2[0].EmpCode.Trim() + "&hid=" + s2.ToString());
                            uri4 = ConfigurationManager.AppSettings["emailApprovalPathHODCodeApp"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=r&trid=" + s.ToString() + "&empcode=" + hodlist2[0].EmpCode.Trim() + "&hid=" + s2.ToString());
                        }
                        else
                        {
                            uri3 = ConfigurationManager.AppSettings["emailApprovalPathHODCodeApp"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=b&trid=" + s.ToString() + "&empcode=" + hodlist2[0].EmpCode.Trim() + "&hid=" + s2.ToString());
                            uri4 = ConfigurationManager.AppSettings["emailApprovalPathHODCodeApp"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=c&trid=" + s.ToString() + "&empcode=" + hodlist2[0].EmpCode.Trim() + "&hid=" + s2.ToString()); 
                        }

                        var appLink2 = string.Empty;
                        appLink2 = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a style='color:#fff; text-decoration:none;' href='" + uri3 + "'>Acceptance</a></td>";
                        appLink2 = appLink2 + "<td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a style='color:#fff; text-decoration:none;' href='" + uri4 + "'>Rejection</a> </td></tr></table>";

                        var templateData2 = emailCredentials2.TemplateFilePath;
                        templateData2 = templateData2.Replace("[appLink]", appLink2);
                        templateData2 = templateData2.Replace("[HodName]", hodlist2[0].EmpName.ToString());
                        emailCredentials2.TemplateFilePath = templateData2;
                        TempData["emailData_Hod"] = emailCredentials2;
                        TempData["emailId_Hod"] = hodemail2;
                        EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials2, hodemail2);
                        //SMS for Approval 2		
                        SendSMSforApproval(hodlist[1], travelInfo[0], hotelInfo, uri1, uri2);
                    }
                    TempData["msgResponse"] = "Hotel Approval Request Notification : Approval has been sent successfully to the Travel Desk.";
                }
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// request to hotel by button after financial approval
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RequestToHotlAfterApproval(List<HotelRequestApprovalModel> elist, string sodOat)
        {
            if (sodOat == "SOD")
            {
                var dicListSharedUser = new Dictionary<string, object>();
                dicListSharedUser = _transportRepository.GetHotelDetailbyTrID(Convert.ToInt64(elist[0].TravelRequestId), elist[0].HotelRequestId);
                var list = new List<HotelRequestApprovalModel>();

                if (dicListSharedUser["sharedtravID"] == null || dicListSharedUser["sharedtravID"] == "")
                {
                    list = elist;
                }
                else
                {
                    list.Add(new HotelRequestApprovalModel { TravelRequestId = elist[0].TravelRequestId, HotelRequestId = elist[0].HotelRequestId });
                    list.Add(new HotelRequestApprovalModel { TravelRequestId = Convert.ToInt64(dicListSharedUser["sharedtravID"]), HotelRequestId = Convert.ToInt32(dicListSharedUser["sharedhId"]) });
                }
                var i = _transportRepository.UpdateHotelStatus(list, sodOat);
                var s = ResendHotelRequest(list, sodOat);
                return s;
            }
            else
            {
                var dicListSharedUser = new Dictionary<string, object>();
                dicListSharedUser = _transportRepository.GetHotelDetailbyTrIDOat(Convert.ToInt64(elist[0].TravelRequestId));

                var list = new List<HotelRequestApprovalModel>();
                if (dicListSharedUser["sharedtravIDOat"] == null || dicListSharedUser["sharedtravIDOat"] == "")
                {
                    list = elist;
                }
                else
                {
                    list.Add(new HotelRequestApprovalModel { TravelRequestId = elist[0].TravelRequestId });
                    list.Add(new HotelRequestApprovalModel { TravelRequestId = Convert.ToInt64(dicListSharedUser["sharedtravID"]) });
                }
                var i = _transportRepository.UpdateHotelStatus(list, sodOat);
                var s = ResendHotelRequest(list, sodOat);
                return s;
            }
        }


        /// <summary>
        /// Resend request to respective financial approvers
        /// </summary>
        /// <param name="TravelRequestId"></param>
        /// <param name="sodOat"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ResendApproverRequest(string TravelRequestId, string HotelRequestId, string sodOat)
        {
            if (sodOat == "SOD")
            {
                var dicListSharedUser = new Dictionary<string, object>();
                dicListSharedUser = _transportRepository.GetHotelDetailbyTrID(Convert.ToInt64(TravelRequestId), Convert.ToInt32(HotelRequestId));
                var list = new List<HotelRequestApprovalModel>();

                var trid = Convert.ToInt64(TravelRequestId);
                var hid = Convert.ToInt32(HotelRequestId);
                if (dicListSharedUser["sharedtravID"] == null || dicListSharedUser["sharedtravID"] == "")
                {
                    list.Add(new HotelRequestApprovalModel { TravelRequestId = trid, HotelRequestId = hid });
                }
                else
                {
                    list.Add(new HotelRequestApprovalModel { TravelRequestId = trid, HotelRequestId = hid });
                    list.Add(new HotelRequestApprovalModel { TravelRequestId = Convert.ToInt64(dicListSharedUser["sharedtravID"]), HotelRequestId = Convert.ToInt32(dicListSharedUser["sharedhId"]) });
                }

                var s = ResendFinancialApproverRequest(list, sodOat);
                return s;
            }
            else
            {
                var dicListSharedUser = new Dictionary<string, object>();
                dicListSharedUser = _transportRepository.GetHotelDetailbyTrIDOat(Convert.ToInt64(TravelRequestId));

                var list = new List<HotelRequestApprovalModel>();
                var trid = Convert.ToInt64(TravelRequestId);
                if (dicListSharedUser["sharedtravIDOat"] == null || dicListSharedUser["sharedtravIDOat"] == "")
                {
                    list.Add(new HotelRequestApprovalModel { TravelRequestId = trid });
                }
                else
                {
                    list.Add(new HotelRequestApprovalModel { TravelRequestId = trid });
                    list.Add(new HotelRequestApprovalModel { TravelRequestId = Convert.ToInt64(dicListSharedUser["sharedtravID"]) });
                }

                var s = ResendFinancialApproverRequest(list, sodOat);
                return s;
            }
        }


        /// <summary>
        /// resend request to respective approver(s) for financial approval
        /// </summary>
        /// <param name="elist"></param>
        /// <param name="sodOat"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ResendFinancialApproverRequest(List<HotelRequestApprovalModel> elist, string sodOat)
        {
            var requestList = new List<String>();
            var hidList = new List<String>();
            var s = "";
            var s2 = "";
            for (var i = 0; i < elist.Count; i++)
            {
                var rqId = elist[i].TravelRequestId;
                requestList.Add(rqId.ToString());
                s = s + rqId.ToString() + ",";

                var hid = elist[i].HotelRequestId;
                hidList.Add(hid.ToString());
                s2 = s2 + hid.ToString() + ",";
            }

            var hodempcode1 = "";
            var hodempcode2 = "";
            var hodemail1 = "";
            var hodemail2 = "";
            var hotelname = "";

            var dic = new Dictionary<string, object>();
            var dichotel = new Dictionary<string, object>();
            var inclist = new List<HotelInclusionNonContractualMasterModels>();
            var rlist = new List<HotelRequestHODFinancialApprovalModels>();
            rlist = _transportRepository.GetdetailHODApproval(elist[0].TravelRequestId, elist[0].HotelRequestId, sodOat);
            if (rlist.Count > 0)
            {
                if (sodOat == "SOD")
                {
                    hodempcode1 = rlist[0].ApproverEmpCodeLevel1;
                    hodempcode2 = rlist[0].ApproverEmpCodeLevel2;
                    hodemail1 = _transportRepository.GetFinancialHodDetailsEmail(hodempcode1);
                    if (hodempcode2 == "" || hodempcode2 == null)
                    { }
                    else
                    {
                        hodemail2 = _transportRepository.GetFinancialHodDetailsEmail(hodempcode2);
                    }
                    dichotel = _transportRepository.GetHotelDetailbyTrID(elist[0].TravelRequestId, elist[0].HotelRequestId);
                    var hotelList = dichotel["hotelinfobyTrid"] as List<HotelRequestApprovalModel>;
                    elist = hotelList;
                    hotelname = hotelList[0].HotelName;
                    inclist = _transportRepository.FindNonContractualHotelInclusions(elist[0].TravelRequestId, elist[0].HotelRequestId);
                }
                else
                {
                    hodempcode1 = rlist[0].ApproverEmpCodeLevel1;
                    hodempcode2 = rlist[0].ApproverEmpCodeLevel2;
                    hodemail1 = _transportRepository.GetFinancialHodDetailsEmail(hodempcode1);
                    if (hodempcode2 == "" || hodempcode2 == null)
                    { }
                    else
                    {
                        hodemail2 = _transportRepository.GetFinancialHodDetailsEmail(hodempcode2);
                    }
                    dichotel = _transportRepository.GetHotelDetailbyTrIDOat(elist[0].TravelRequestId);
                    var hotelList = dichotel["hotelinfobyTridOat"] as List<HotelRequestApprovalOatModels>;
                    hotelname = hotelList[0].HotelName;
                    elist = new List<HotelRequestApprovalModel>();
                    elist.Add(new HotelRequestApprovalModel
                    {
                        HotelName = hotelList[0].HotelName,
                        HotelAddress = hotelList[0].HotelAddress,
                        HotelType = hotelList[0].HotelType,
                        PrimaryEmail = hotelList[0].HotelType,
                        HotelPrice = hotelList[0].HotelPrice,
                        FlightNo = hotelList[0].FlightNo,
                        ETA = hotelList[0].ETA
                    });
                    var inclistOat = new List<HotelInclusionNonContractualMasterOatModels>();
                    inclistOat = _transportRepository.FindNonContractualHotelInclusionsOat(elist[0].TravelRequestId);
                    inclist.Add(new HotelInclusionNonContractualMasterModels
                    {
                        TravelRequestId = inclistOat[0].TravelRequestId,
                        HotelName = inclistOat[0].HotelName,
                        Accomodation = inclist[0].Accomodation,
                        Food = inclist[0].Food,
                        AirportTransfers = inclist[0].AirportTransfers,
                        RoomService = inclist[0].RoomService,
                        BuffetTime = inclist[0].BuffetTime,
                        Laundry = inclist[0].Laundry
                    });
                }
                if (hodemail1.Length > 0)
                {
                    var emailSubject = "SOD Non-Contractual Hotel Request Notification from Travel desk:" + System.DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt");
                    var emailTemplateName = "HotelHODApprovalNotificationTemplateFor_NonContractual.html";
                    var emailCredentials = EmailCredentialsHODApprovalRequest(emailSubject, emailTemplateName, hotelname, requestList, hidList, elist, inclist, sodOat);
                    var emailCredentials2 = EmailCredentialsHODApprovalRequest(emailSubject, emailTemplateName, hotelname, requestList, hidList, elist, inclist, sodOat);

                    var skey = new StringBuilder();
                    skey.Append(s.ToString());
                    skey.Append(hodemail1);
                    var uri1 = "";
                    var uri2 = "";
                    var uri3 = "";
                    var uri4 = "";

                    if (rlist[0].ApprovalStatusLevel1 == 0)
                    {
                        var hodlist = _transportRepository.GetHodApproverNameHotels(hodemail1);
                        if (sodOat == "SOD")
                        {
                            uri1 = ConfigurationManager.AppSettings["emailApprovalPathHODCodeApp"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=a&trid=" + s.ToString() + "&empcode=" + hodlist[0].EmpCode.Trim() + "&hid=" + s2.ToString());
                            uri2 = ConfigurationManager.AppSettings["emailApprovalPathHODCodeApp"].Trim() + "?str=" + CipherURL.Encrypt(skey+ "&type=r&trid=" + s.ToString() + "&empcode=" + hodlist[0].EmpCode.Trim() + "&hid=" + s2.ToString());
                        }
                        else
                        {
                            uri1 = ConfigurationManager.AppSettings["emailApprovalPathHODCodeApp"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=b&trid=" + s.ToString() + "&empcode=" + hodlist[0].EmpCode.Trim());
                            uri2 = ConfigurationManager.AppSettings["emailApprovalPathHODCodeApp"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=c&trid=" + s.ToString() + "&empcode=" + hodlist[0].EmpCode.Trim());
                        }
                        var appLink = string.Empty;
                        appLink = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Acceptance</a></td>";
                        appLink = appLink + "<td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a style='color:#fff; text-decoration:none;' href='" + uri2 + "'>Rejection</a> </td></tr></table>";

                        var templateData = emailCredentials.TemplateFilePath;
                        templateData = templateData.Replace("[appLink]", appLink);
                        templateData = templateData.Replace("[HodName]", hodlist[0].EmpName.ToString());
                        emailCredentials.TemplateFilePath = templateData;
                        TempData["emailData_Hod"] = emailCredentials;
                        TempData["emailId_Hod"] = hodemail1;
                        EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials, hodemail1);
                    }

                    if (hodemail2.Length > 0 && rlist[0].ApprovalStatusLevel2 == 0)
                    {
                        var hodlist2 = _transportRepository.GetHodApproverNameHotels(hodemail2);
                        if (sodOat == "SOD")
                        {
                            uri3 = ConfigurationManager.AppSettings["emailApprovalPathHODCodeApp"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=a&trid=" + s.ToString() + "&empcode=" + hodlist2[0].EmpCode.Trim() + "&hid=" + s2.ToString());
                            uri4 = ConfigurationManager.AppSettings["emailApprovalPathHODCodeApp"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=r&trid=" + s.ToString() + "&empcode=" + hodlist2[0].EmpCode.Trim() + "&hid=" + s2.ToString());
                        }
                        else
                        {
                            uri3 = ConfigurationManager.AppSettings["emailApprovalPathHODCodeApp"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=b&trid=" + s.ToString() + "&empcode=" + hodlist2[0].EmpCode.Trim());
                            uri4 = ConfigurationManager.AppSettings["emailApprovalPathHODCodeApp"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=c&trid=" + s.ToString() + "&empcode=" + hodlist2[0].EmpCode.Trim());
                        }

                        var appLink2 = string.Empty;
                        appLink2 = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a style='color:#fff; text-decoration:none;' href='" + uri3 + "'>Acceptance</a></td>";
                        appLink2 = appLink2 + "<td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a style='color:#fff; text-decoration:none;' href='" + uri4 + "'>Rejection</a> </td></tr></table>";

                        var templateData2 = emailCredentials2.TemplateFilePath;
                        templateData2 = templateData2.Replace("[appLink]", appLink2);
                        templateData2 = templateData2.Replace("[HodName]", hodlist2[0].EmpName.ToString());
                        emailCredentials2.TemplateFilePath = templateData2;
                        TempData["emailData_Hod"] = emailCredentials2;
                        TempData["emailId_Hod"] = hodemail2;
                        EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials2, hodemail2);
                    }
                    TempData["msgResponse"] = "Hotel Approval Request Notification : Approval has been sent successfully to the Travel Desk.";
                }
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }



        public EmailNotificationModel EmailCredentialsHODApprovalRequest(string subjectName, string emailTemplateName, string hotelname, List<String> requestList, List<String> hidList, List<HotelRequestApprovalModel> elist, List<HotelInclusionNonContractualMasterModels> inclist, string sodOat)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtpHotelUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtptdPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtptdServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtptdPort"].Trim(),
                TemplateFilePath = ReadFileHODApprovalRequest(emailTemplateName, hotelname, requestList, hidList, elist, inclist, sodOat),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }


        private string ReadFileHODApprovalRequest(string emailTemplateName, string hotelname, List<String> requestList, List<String> hidList, List<HotelRequestApprovalModel> elist, List<HotelInclusionNonContractualMasterModels> inclist, string sodOat)
        {
            var strContent = new StringBuilder();
            string line;
            string path = System.IO.Path.Combine(HttpRuntime.AppDomainAppPath, "Template/Hotels/" + emailTemplateName);
            
            var file = new System.IO.StreamReader(path);
           
            while ((line = file.ReadLine()) != null)
            {
                strContent.Append(line);
            }
            file.Close();
            strContent = strContent.Replace("[binfo]", "Booking Details (Request ID :" + elist[0].TravelRequestId + ")");
            strContent = strContent.Replace("[hinfo]", "Hotel Details ");

            GetHotelRequestData(requestList, hidList, hotelname, sodOat);
            var passengerInfo = TempData["PassengerInfoForHotelRequest"] as List<TravelRequestModels>;
            var count = 1;
            var gender = "";
            var airtransport = "";
            var ETAString = "";
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

            var tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;font-weight:bold;'>" +
              "<td style='height:20px; padding-bottom:8px;'>S. No.</td>" +
              "<td style='height:20px; padding-bottom:8px;'>Gender</td>" +
              "<td style='height:20px; padding-bottom:8px;'>Employee Name</td>" +
              "<td style='height:20px; padding-bottom:8px;'>Emp Id</td>" +
               "<td style='height:20px; padding-bottom:8px;'>Check-In Date</td>" +
               "<td style='height:20px; padding-bottom:8px;'>Check-Out Date</td>" +
               "<td style='height:20px; padding-bottom:8px;'>HeadOffice/Airport Transfers Required</td>" +
               "<td style='height:20px; padding-bottom:8px;'>Flight No.</td>" +
               ETAString +
               "<td height:20px; padding-bottom:8px;'>Mobile No.</td></tr>";

            foreach (var p in passengerInfo)
            {
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
                tr = tr + "<tr style='font-family:Arial; font-size:12px;'>" +
                        "<td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + count +
                        "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + gender +
                        "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.RequestedEmpName;
                if (p.RequestedEmpId == 0)
                {
                    tr = tr + "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.RequestedEmpCode;
                }
                else
                {
                    tr = tr + "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.RequestedEmpId;
                }

                tr = tr + "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.CheckinDate.ToString("dd-MMM-yyyy") +
                        "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.CheckoutDate.ToString("dd-MMM-yyyy") +
                        "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + airtransport +
                        "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.FlightNo +
                        "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + ETACheckin +
                        "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.Phone +
                        "</td></tr>";
                count++;
            }
            strContent = strContent.Replace("[tr]", tr);

            var trh = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;font-weight:bold;'>" +
              "<td style='height:20px; padding-bottom:8px;'>Hotel Name</td>" +
              "<td style='height:20px; padding-bottom:8px;'>Address</td>" +
              "<td style='height:20px; padding-bottom:8px;'>Type</td>" +
              "<td style='height:20px; padding-bottom:8px;'>Email</td>" +
               "<td height:20px; padding-bottom:8px;'>Price</td></tr>";

            trh = trh + "<tr style='font-family:Arial; font-size:12px;'>" +
                    "<td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + elist[0].HotelName +
                    "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + elist[0].HotelAddress +
                    "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + elist[0].HotelType +
                    "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + elist[0].PrimaryEmail +
                    "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'><b>" + elist[0].HotelPrice + " (" + elist[0].HotelCurrencyCode + ") " + "</b>" +
                    "</td></tr>";

            strContent = strContent.Replace("[trh]", trh);

            if (sodOat == "SOD")
            {
                var inclusionList = new List<HotelInclusionNonContractualMasterModels>();
                inclusionList = _transportRepository.FindNonContractualHotelInclusions(elist[0].TravelRequestId, elist[0].HotelRequestId);
                if (inclusionList.Count > 0)
                {
                    strContent = strContent.Replace("[IncInfo]", "Hotel Inclusions Details");
                    var trI = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Accommodation</td><td>Food</td><td>Airport Transfers</td><td>Room service</td><td>Buffet Time</td><td>Laundry</td></tr>";

                    trI = trI + "<tr style='font-family:Arial; font-size:12px;'><td>" + inclusionList[0].Accomodation +
                            "</td><td>" + inclusionList[0].Food + "</td><td>" + inclusionList[0].AirportTransfers +
                            "</td><td>" + inclusionList[0].RoomService + "</td>" +
                            "<td>" + inclusionList[0].BuffetTime + "</td>" +
                            "<td>" + inclusionList[0].Laundry + "</td>" +
                            "</tr>";
                    strContent = strContent.Replace("[trI]", trI);
                }
                else
                {
                    strContent = strContent.Replace("[IncInfo]", "");
                    strContent = strContent.Replace("[trI]", "");
                }
            }
            else
            {
                var inclusionList = new List<HotelInclusionNonContractualMasterOatModels>();
                inclusionList = _transportRepository.FindNonContractualHotelInclusionsOat(elist[0].TravelRequestId);
                if (inclusionList.Count > 0)
                {
                    strContent = strContent.Replace("[IncInfo]", "Hotel Inclusions Details");
                    var trI = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Accommodation</td><td>Food</td><td>Airport Transfers</td><td>Room service</td><td>Buffet Time</td><td>Laundry</td></tr>";

                    trI = trI + "<tr style='font-family:Arial; font-size:12px;'><td>" + inclusionList[0].Accomodation +
                            "</td><td>" + inclusionList[0].Food + "</td><td>" + inclusionList[0].AirportTransfers +
                            "</td><td>" + inclusionList[0].RoomService + "</td>" +
                            "<td>" + inclusionList[0].BuffetTime + "</td>" +
                            "<td>" + inclusionList[0].Laundry + "</td>" +
                            "</tr>";
                    strContent = strContent.Replace("[trI]", trI);

                }
                else
                {
                    strContent = strContent.Replace("[IncInfo]", "");
                    strContent = strContent.Replace("[trI]", "");
                }
            }
            return strContent.ToString();
        }

        /// <summary>
        /// get requests not approved by hod- exceptional cases
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetHotelInfoExceptional(string trid)
        {
            var criteria = 1;
            if (!String.IsNullOrEmpty(trid))
            {
                var dichotel = new Dictionary<string, object>();
                dichotel = _transportRepository.GetHotelInfoApprovalbyTrID(Convert.ToInt64(trid));
                var hotelList = dichotel["hotelinfobyTrid"] as List<HotelRequestApprovalModel>;
                if (hotelList.Count == 0)
                {
                    criteria = 1;
                }
                else
                {
                    if (hotelList[0].HotelType == "Non-Contractual")
                    {
                        criteria = 2;
                    }
                }
            }
            else
                trid = "0";
            var s = _transportRepository.GetHotelInfoExceptional(trid, criteria);
            return Json(s, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// find hotel data which is similar to already allocated person
        /// </summary>
        /// <param name="TravelRequestId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult FindSimilarTravelData(string TravelRequestId, string hotelname, string hotelrqstid)
        {
            var dichotel = new Dictionary<string, object>();
            dichotel = _transportRepository.GetHotelDetailbyTrID(Convert.ToInt64(TravelRequestId), Convert.ToInt32(hotelrqstid));
            var hoteldata = dichotel["hotelData"] as List<TravelRequestHotelDetailModels>;

            var checkin = hoteldata[0].CheckInDate;
            var hotelcity = hoteldata[0].City;

            var existingHotelList = new List<TravelRequestHotelDetailModels>();
            existingHotelList = _transportRepository.FindSimilarHotelAllocationData(Convert.ToInt64(TravelRequestId), hotelname, checkin, hotelcity);

            //commented by soni
            //if (existingHotelList == null || existingHotelList.Count < 1)
            //{
            //    var returnString = "NotExist".ToString();
            //    return Json(returnString, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    //var returnitem = existingHotelList[0].TravelRequestId + "|" + existingHotelList[0].HotelRequestId;
            //    return Json(existingHotelList, JsonRequestBehavior.AllowGet);
            //}
            return Json(existingHotelList, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Get User Double Occupancy Details Sorted Traveldesk
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetUserDoubleOccupancyTraveldesk(string newTrid, string existingTrid, string hotelrqstid, string exishotelid)
        {
            var s = _transportRepository.GetSodEmployeeHotelList_TravelDesk_DoubleOccupancy(newTrid, existingTrid, hotelrqstid, exishotelid);
            return Json(s, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        /// resend reminder request to hotel for acceptance/rejection
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ResponsiveMailtoHotel(List<HotelRequestApprovalModel> elist, string sodOat)
        {
            var requestList = new List<String>();
            var hIdList = new List<String>();
            for (var i = 0; i < elist.Count; i++)
            {
                var rqId = elist[i].TravelRequestId;
                requestList.Add(rqId.ToString());

                var hId = elist[i].HotelRequestId;
                hIdList.Add(hId.ToString());
            }

            var primaryemail = "";
            var hotelname = "";
            var s = 0;
            var confirmNo = "";
            var dic = new Dictionary<string, object>();
            dic = _transportRepository.getApprovalHotelDetails(elist[0].TravelRequestId.ToString(), elist[0].HotelRequestId, sodOat);
            if (sodOat == "SOD")
            {
                var rlist = new List<HotelRequestApprovalModel>();
                rlist = dic["approvalHotelDetails"] as List<HotelRequestApprovalModel>;
                primaryemail = rlist[0].PrimaryEmail;
                hotelname = rlist[0].HotelName;
                s = rlist[0].clubId;
                confirmNo = rlist[0].HotelConfirmationNo;
            }
            else
            {
                var rlist = new List<HotelRequestApprovalOatModels>();
                rlist = dic["approvalHotelDetails"] as List<HotelRequestApprovalOatModels>;
                primaryemail = rlist[0].PrimaryEmail;
                hotelname = rlist[0].HotelName;
                s = rlist[0].clubId;
                confirmNo = rlist[0].HotelConfirmationNo;
            }

            if (primaryemail.Length > 0)
            {
                var emailSubject = "SOD Hotel Response Notification from Spicejet:" + System.DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt");
                var emailTemplateName = "HotelResponsiveMailTemplateFor_Checkin_CheckoutTime.html";
                var emailCredentials = EmailCredentialsResponsiveHotelMail(emailSubject, emailTemplateName, requestList, hIdList, hotelname, sodOat);

                var skey = new StringBuilder();
                skey.Append(s.ToString() + ",");
                skey.Append(primaryemail);

                var uri1 = "";
                var uri2 = "";
                if (sodOat == "SOD")
                {
                    uri1 = ConfigurationManager.AppSettings["emailResponsivePathHotel"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=a&trid=" + s.ToString() + "&hotelname=" + hotelname + "&confirmNo=" + confirmNo);
                    //uri2 = ConfigurationManager.AppSettings["emailApprovalPathHotelCode"].Trim() + "?str=" + skey + "&type=r&trid=" + s.ToString() + "&hotelname=" + hotelname;
                }
                else
                {
                    uri1 = ConfigurationManager.AppSettings["emailResponsivePathHotel"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=b&trid=" + s.ToString() + "&hotelname=" + hotelname + "&confirmNo=" + confirmNo);
                    //uri2 = ConfigurationManager.AppSettings["emailApprovalPathHotelCode"].Trim() + "?str=" + skey + "&type=c&trid=" + s.ToString() + "&hotelname=" + hotelname;
                }
                // var essurl = ConfigurationManager.AppSettings["essportalUrl"].Trim();
                var appLink = string.Empty;
                appLink = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Respond</a></td></tr></table>";
                //appLink = appLink + "<td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a style='color:#fff; text-decoration:none;' href='" + uri2 + "'>Rejection</a> </td></tr></table>";

                var templateData = emailCredentials.TemplateFilePath;

                templateData = templateData.Replace("[appLink]", appLink);
                templateData = templateData.Replace("[hotelName]", hotelname);
                emailCredentials.TemplateFilePath = templateData;

                TempData["emailData_Hod"] = emailCredentials;
                TempData["emailId_Hod"] = primaryemail;
                EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials, primaryemail);

                TempData["msgResponse"] = "Hotel Request Notification : Details have been sent successfully. The request has been sent to the respected hotel at  " + primaryemail.ToString();
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Email Credentials Responsive Hotel Mail
        /// </summary>
        /// <param name="subjectName"></param>
        /// <param name="emailTemplateName"></param>
        /// <param name="requestList"></param>
        /// <param name="hidList"></param>
        /// <param name="hotelname"></param>
        /// <param name="sodOat"></param>
        /// <returns></returns>
        public EmailNotificationModel EmailCredentialsResponsiveHotelMail(string subjectName, string emailTemplateName, List<String> requestList, List<String> hidList, string hotelname, string sodOat)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtpHotelUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtptdPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtptdServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtptdPort"].Trim(),
                TemplateFilePath = ReadFileResponsiveHotelMail(emailTemplateName, requestList, hidList, hotelname, sodOat),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }



        /// <summary>
        /// Read File for Responsive Mail to Hotel
        /// </summary>
        /// <param name="emailTemplateName"></param>
        /// <param name="requestList"></param>
        /// <param name="hidList"></param>
        /// <param name="hotelname"></param>
        /// <param name="sodOat"></param>
        /// <returns></returns>
        private string ReadFileResponsiveHotelMail(string emailTemplateName, List<String> requestList, List<String> hidList, string hotelname, string sodOat)
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

            var dicList = new Dictionary<string, object>();
            dicList = _transportRepository.GetHotelandUserInfoForResponsiveMail(requestList, hidList, hotelname, sodOat);
            var passengerInfo = dicList["PassengerInfoForHotelRequestResponsive"] as List<TravelRequestModels>;

            var count = 1;
            var gender = "";

            if (passengerInfo.Count == 1)
            {
                strContent = strContent.Replace("[occupancy]", "Single Occupancy");
            }
            else
            {
                strContent = strContent.Replace("[occupancy]", "Double Occupancy/Sharing");
            }

            var tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;font-weight:bold;'>" +
              "<td style='height:20px; padding-bottom:8px;'>S. No.</td>" +
              "<td style='height:20px; padding-bottom:8px;'>Gender</td>" +
              "<td style='height:20px; padding-bottom:8px;'>Employee Name</td>" +
              "<td style='height:20px; padding-bottom:8px;'>Emp Id</td>" +
               "<td style='height:20px; padding-bottom:8px;'>Check-In Date</td>" +
               "<td style='height:20px; padding-bottom:8px;'>Check-Out Date</td>" +
               "<td style='height:20px; padding-bottom:8px;'>Hotel Confirmation No.</td>" +
               "<td height:20px; padding-bottom:8px;'>Mobile No.</td></tr>";

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

                tr = tr + "<tr style='font-family:Arial; font-size:12px;'>" +
                        "<td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + count +
                        "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + gender +
                        "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.RequestedEmpName;
                if (p.RequestedEmpId == 0)
                {
                    tr = tr + "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.RequestedEmpCode;
                }
                else
                {
                    tr = tr + "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.RequestedEmpId;
                }

                tr = tr + "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.CheckinDate.ToString("dd-MMM-yyyy") +
                      "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.CheckoutDate.ToString("dd-MMM-yyyy") +
                      "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.HotelConfirmationNo +
                      "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.Phone +
                      "</td></tr>";
                count++;
            }
            strContent = strContent.Replace("[tr]", tr);
            return strContent.ToString();
        }


        /// <summary>
        /// get requests with for checkin-checkout time confirmation by user & hotel
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetDetailsCheckTimeConflict()
        {
            var s = _transportRepository.GetHotelDetailCheckTimeConflict();
            return Json(s, JsonRequestBehavior.AllowGet);
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
        /// Send EMail to Secondary Email ID's
        /// </summary>
        /// <param name="secondaryemail"></param>
        /// <param name="requestList"></param>
        /// <param name="hIdlist"></param>
        /// <param name="hotelname"></param>
        /// <param name="sodOat"></param>
        /// <param name="remarks"></param>
        /// <param name="elist"></param>
        public void SendMailToSecondaryIDs(string secondaryemail, List<String> requestList, List<String> hIdlist, string hotelname, string sodOat, string remarks, List<HotelRequestApprovalModel> elist)
        {
            var s = elist[0].clubId;
            string[] email = secondaryemail.Split(',');

            foreach (string i in email)
            {
                var primaryemail = i.ToString();
                var emailSubject = "SOD Hotel Request Notification from Spicejet:" + System.DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt");
                var emailTemplateName = "HotelBookingRequestNotificationTemplateFor_Contractual.html";
                var emailCredentials = EmailCredentialsHotelRequest(emailSubject, emailTemplateName, requestList, hIdlist, hotelname, sodOat, "hotel");

                var skey = new StringBuilder();
                skey.Append(s.ToString() + ",");
                skey.Append(primaryemail);

                var uri1 = "";
                var uri2 = "";
                if (sodOat == "SOD")
                {
                    uri1 = ConfigurationManager.AppSettings["emailApprovalPathHotelCode"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=a&trid=" + s.ToString() + "&hotelname=" + hotelname);
                    uri2 = ConfigurationManager.AppSettings["emailApprovalPathHotelCode"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=r&trid=" + s.ToString() + "&hotelname=" + hotelname);
                }
                else
                {
                    uri1 = ConfigurationManager.AppSettings["emailApprovalPathHotelCode"].Trim() + "?str=" + CipherURL.Encrypt(skey+ "&type=b&trid=" + s.ToString() + "&hotelname=" + hotelname);
                    uri2 = ConfigurationManager.AppSettings["emailApprovalPathHotelCode"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=c&trid=" + s.ToString() + "&hotelname=" + hotelname);
                }
                // var essurl = ConfigurationManager.AppSettings["essportalUrl"].Trim();
                var appLink = string.Empty;
                appLink = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Acceptance</a></td>";
                appLink = appLink + "<td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a style='color:#fff; text-decoration:none;' href='" + uri2 + "'>Rejection</a> </td></tr></table>";

                var templateData = emailCredentials.TemplateFilePath;
                templateData = templateData.Replace("[appLink]", appLink);
                templateData = templateData.Replace("[hotelName]", "<b>" + hotelname + "</b>");
                templateData = templateData.Replace("[remarksTravelDesk]", remarks);

                if (elist[0].HotelType.ToLower() == "contractual")
                {
                    //Add CityName
                    //templateData = templateData.Replace("[hotelName]", "<b>" + hotelname + "</b><br/>" + hotelname);

                    //Add Inclusion Details to Hotel by Satyam
                    var inclusionList = new List<HotelInclusionMasterModels>();
                    var citycode = _transportRepository.GetCityCodeOfHotel(elist[0].HotelCode);
                    inclusionList = findHotelInclusions(citycode, elist[0].HotelName);
                    if (inclusionList.Count > 0)
                    {
                        templateData = templateData.Replace("[hinfo]", "Hotel Inclusions Details");
                        var trI = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Accommodation</td><td>Food</td><td>Airport Transfers</td><td>Room service</td><td>Buffet Time</td><td>Laundry</td></tr>";

                        trI = trI + "<tr style='font-family:Arial; font-size:12px;'><td>" + inclusionList[0].Accomodation +
                                "</td><td>" + inclusionList[0].Food + "</td><td>" + inclusionList[0].AirportTransfers +
                                "</td><td>" + inclusionList[0].RoomService + "</td>" +
                                "<td>" + inclusionList[0].BuffetTime + "</td>" +
                                "<td>" + inclusionList[0].Laundry + "</td>" +
                                "</tr>";
                        templateData = templateData.Replace("[trI]", trI);
                    }
                    else
                    {
                        templateData = templateData.Replace("[hinfo]", "");
                        templateData = templateData.Replace("[trI]", "");
                    }

                    var gstdetails = _transportRepository.GetGstDetailsByCityCode(citycode) as List<HotelGstDetailModels>;
                    if (gstdetails.Count > 0)
                    {
                        templateData = templateData.Replace("[gstdetails]", "Please raise invoice against the following details: ");
                        templateData = templateData.Replace("[gstno]", "GST No : <b>" + gstdetails[0].GSTIN + "</b>");
                        templateData = templateData.Replace("[gstaddress]", "Address : <b>" + gstdetails[0].PlaceOfBusiness + "</b>");
                    }
                    else
                    {
                        templateData = templateData.Replace("[gstdetails]", "");
                        templateData = templateData.Replace("[gstno]", "");
                        templateData = templateData.Replace("[gstaddress]", "");
                    }
                }
                else
                {
                    templateData = templateData.Replace("[gstdetails]", "");
                    templateData = templateData.Replace("[gstno]", "");
                    templateData = templateData.Replace("[gstaddress]", "");
                }
                emailCredentials.TemplateFilePath = templateData;
                TempData["emailData_Hod"] = emailCredentials;
                TempData["emailId_Hod"] = primaryemail;
                EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials, primaryemail);
            }

            TempData["msgResponse"] = "Hotel Request Notification : Hotel details have been sent successfully. The request has been sent to the respected hotel at  " + secondaryemail.ToString();
        }


        /// <summary>		
        /// Send SMS for Approval		
        /// </summary>		
        /// <param name="reqId"></param>		
        /// <param name="paxName"></param>		
        /// <param name="approvalURI"></param>		
        public void SendSMSforApproval(NonContractualHotelApprovalMasterModels hoddetails, TravelRequestMasterModels sodRequest, List<TravelRequestHotelDetailModels> hotelInfo, string appURI, string rejURI)
        {
            var smsText = ConfigurationManager.AppSettings["smsFinApprovalOnlyHotel"].ToString().Replace("@Hodname", hoddetails.Title + ". " + hoddetails.EmpName);
            smsText = smsText.Replace("@PaxName", sodRequest.Title + " " + sodRequest.RequestedEmpName);
            smsText = smsText.Replace("@Sector", hotelInfo[0].City);
            smsText = smsText.Replace("@CheckInDate", hotelInfo[0].CheckInDate.ToString("dd/MMM/yyyy"));
            smsText = smsText.Replace("@CheckOutDate", hotelInfo[0].CheckOutDate.ToString("dd/MMM/yyyy"));
            smsText = smsText.Replace("@ReqId", sodRequest.TravelRequestId.ToString());
            smsText = smsText.Replace("@Price", hotelInfo[0].HotelPrice + " (" + hotelInfo[0].HotelCurrencyCode + ")");
            smsText = smsText.Replace("[AppQueryString]", appURI);
            smsText = smsText.Replace("[RejQueryString]", rejURI);
            smsText = System.Uri.EscapeDataString(smsText);
            smsText = smsText.Replace("25", "");
            //Send SMS		
            SmsNotification.SmsNotifications.SendSmsViaApi(smsText, hoddetails.MobileNo);
            {
                var smsLogModel = new SodApproverSMSLogModels();
                smsLogModel.TrRequestId = sodRequest.TravelRequestId;
                smsLogModel.EmpCode = hoddetails.EmpCode;
                smsLogModel.EmpName = hoddetails.EmpName;
                smsLogModel.EmailID = hoddetails.EmailId;
                smsLogModel.MobileNo = hoddetails.MobileNo;
                smsLogModel.IsDelivered = true;
                smsLogModel.Source = "SOD";
                smsLogModel.SMSText = smsText;
                smsLogModel.DeliveryDate = DateTime.Now;
                _transportRepository.SaveApproverSMSLog(smsLogModel);
            }
        }

        /// <summary>
        /// Exception Handling
        /// </summary>
        /// <param name="filterContext"></param>
        void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            Logging.ErrorLog.Instance.AddDBLogging(ex, filterContext.Controller.ControllerContext.ToString(), "Controllers/trnsController.cs");
        }
    }
}