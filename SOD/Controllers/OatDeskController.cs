using SOD.Services.EntityFramework;
using SOD.Services.Interface;
using SOD.Services.Repository;
using System.Data.Entity.Infrastructure;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using SOD.Model;
using SOD.EmailNotification;
using System.Text;
using System.IO;
using System.Linq;
using System.Diagnostics;
using SOD.CommonWebMethod;
using System.ComponentModel;

namespace SOD.Controllers
{
    [HandleError(ExceptionType = typeof(DbUpdateException), View = "Error")]
    [SessionTimeout]
    public class OatDeskController : Controller, IActionFilter, IExceptionFilter
    {
        private readonly IUserRepository _userRepository;
        private readonly IBulkUploadRepository _bulkUploadRepository;
        private readonly IOALRepository _oalRepository;
        private readonly IOATrepository _oaTRepository;
        //private readonly ITransportRepository _transportRepository;
     

        public OatDeskController()
        {
            _oalRepository = new OALRepository(new SodEntities());
            _userRepository = new UserRepository(new SodEntities());
            _bulkUploadRepository = new BulkUploadRepository(new SodEntities());
            _oaTRepository = new OATrepository(new SodEntities());
           // _transportRepository = new TransportRepository(new SodEntities());
        }
        public ActionResult IthResponseForm()
        {
            var status = string.Empty;
            var reqId = Request.QueryString[0].ToString().Split(',')[0];
            var TrnId = Request.QueryString[0].ToString().Split(',')[1];
            var types = Request.QueryString[1].ToString().Split(',')[0];
            try
            {
                status = _oaTRepository.checkstatusOfOthBookingResponse(types, Convert.ToInt64(reqId));
                if (status == " " && (types == "r" || types == "HR" || types == "UR" || types == "FR"))
                {
                    var response = _oaTRepository.RejectIthResponse(types, Convert.ToInt64(reqId));//rejectResponseByServiceProvidor(types, Convert.ToInt64(reqId));
                    TempData["Status"] = response;
                }
                else if (status == " " && (types == "UA" || types == "HA"))
                {
                    var response = "";
                    var getList = _oaTRepository.acceptReponseByHodOrUserAsPre(types, Convert.ToInt64(reqId));
                    if (getList != null && getList.Count > 0)
                    {
                        var msg = "";
                        if (types == "UA")
                        {
                            var FinancialApproval = new List<OATFinancialApprovalMaster_RoisteringModal>();
                            string bookingType = _oaTRepository.getBookingType(Convert.ToInt64(reqId));
                            var IsAmountGreater = _oaTRepository.getAmountForFinancialApproval(Convert.ToInt64(reqId));
                            if (bookingType == "Roistering(Flight ops / In Flight)" && IsAmountGreater == true)
                            {
                                ViewBag.IsAmountGreater = IsAmountGreater;
                                response = "You have to select your Financial Approval to Submit your choice Please click on the modify Link .";
                            }
                            else
                            {
                                msg = SubmitUserResponse(getList, FinancialApproval).ToString();
                                if (msg != "Fail") response = "Your response has been successfully submitted and forwarded to your HOD for final approval.";
                            }
                        }
                        else
                        {
                            msg = SubmitHodResponse(getList).ToString();
                            if (msg != "Fail") response = "Your Approval process has been completed successfully and sent to OAT TravelDesk for further processing.";
                        }
                    }
                    else
                    {
                        response = "Choice of OAT TravelDesk and User has Conflict, to Submit your choice Please click on the modify Link .";
                    }

                    // var response = AcceptResponseByServiceProvidor(types, Convert.ToInt64(reqId));
                    TempData["Status"] = response;
                    //TempData["Status"] = response;
                }
                else if (types == "FA" || types == "FR")
                {
                    var response = _oaTRepository.ApproveOrRejectFinancialApproval(types, Convert.ToInt64(reqId));
                    TempData["Status"] = response;
                }
                else if (types == "CA")
                {
                    var PassenegerId = Request.QueryString[0].ToString().Split(',')[2];
                    var OriginPlace = Request.QueryString[0].ToString().Split(',')[3];
                    var DestinationPlace = Request.QueryString[0].ToString().Split(',')[4];
                    var bookingDetail = PassenegerId + "|" + OriginPlace + "|" + DestinationPlace;
                    var response = _oaTRepository.SubmitCancelAcknwledgement(bookingDetail, Convert.ToInt64(reqId), 1);
                    TempData["Status"] = response;
                }
                else if (types == "Ca")
                {
                    var requestId = Request.QueryString[0].ToString().Split(',')[2];
                    //var OriginPlace = Request.QueryString[0].ToString().Split(',')[3];
                    //var DestinationPlace = Request.QueryString[0].ToString().Split(',')[4];
                    var bookingDetail = requestId + "|";// + OriginPlace + "|" + DestinationPlace;
                    var response = _oaTRepository.SubmitCancelAcknwledgement(bookingDetail, Convert.ToInt64(reqId), 2);
                    TempData["Status"] = response;
                }
                else if (types == "HOA")
                {
                    var response = _oaTRepository.SubmitHoldAcknwledgement(types, Convert.ToInt64(reqId));
                    TempData["Status"] = response;
                }
                else if (types == "CLA" || types == "CLR")
                {
                    var response = _oaTRepository.SubmitCLevelAcknowledgemnt(types, Convert.ToInt64(reqId));
                    TempData["Status"] = response;
                }
                else
                {
                    TempData["Status"] = status;
                }
            }
            catch (Exception ex)
            {
                TempData["Status"] = "Oops! Something Went Wrong.";
            }
            return View();
        }
        public ActionResult FinalBookingForm()
        {
            var status = string.Empty;
            var reqId = Request.QueryString[0].ToString().Split(',')[0];
            var TrnId = Request.QueryString[0].ToString().Split(',')[1];
            var types = Request.QueryString[1].ToString().Split(',')[0];
            try
            {
                status = _oaTRepository.checkstatusOfOthBookingResponse(types, Convert.ToInt64(reqId));
                if (status == " " && (types == "FBR"))
                {
                    var response = _oaTRepository.RejectIthResponse(types, Convert.ToInt64(reqId));//rejectResponseByServiceProvidor(types, Convert.ToInt64(reqId));
                    TempData["Status"] = response;
                }              
                else
                {
                    TempData["Type"] = types;
                    TempData["Status"] = status;
                }
            }
            catch (Exception ex)
            {
                TempData["Status"] = "Oops! Something Went Wrong.";
            }
            return View();
        }
        public ActionResult ViewAttachment()
        {
            //var djh = ViewBag.src;
            //var dfh = TempData["src"];
            string qString = Request.QueryString["str"];
            if (qString != null)
            {
                var reqId = Request.QueryString[0].ToString().Split(',')[0];
                var TrnId = Request.QueryString[0].ToString().Split(',')[1];
                var Id = Request.QueryString[0].ToString().Split(',')[2];
                viewIthAttachedSrc(Convert.ToInt64(Id), 3);
            }
            return View();
        }

        public ActionResult Cancellation()
        {           
            return View();
        }


        public ActionResult GetDataForFinalBookingForm(Int64 oatReqId)

        {
            var lst = new Dictionary<string, object>();
            lst = _oaTRepository.GetDetailForIthResponse(oatReqId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        //get all the list of Booking
        public JsonResult getListOfOatBooking()
        {
            var oatMasterList = _oaTRepository.getOatMasterlist();
            return Json(oatMasterList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult getOatDetailsPerReqId(long reqId)
        {
            var lst = new Dictionary<string, object>();
            lst = _oaTRepository.getOatDetailsPerReqId(reqId);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult getSaveManageNoShow(ITHTransactionDetailModal mangeShowList,DateTime date)
        {
            var str = _oaTRepository.GetManageNoShowDetails(mangeShowList,date);
            return Json(str, JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        //public ActionResult getNoShowDone(long res)
        //{
        //    if(res == 1)
        //   return Json(res, JsonRequestBehavior.AllowGet);
           
            
        //}
        [HttpGet]
        public JsonResult getITHlistName()
        {
            var lst = _oaTRepository.getIthListName();
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult getIthDetailPerName(String ithName)
        {
            var lst = _oaTRepository.getIthDetailPerName(ithName);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult sendReqToIth(ITHVenderModal ithDetaillst, List<Dictionary<String, String>> OATReqList)
        {
            var jsonmsg = "";
            List<ITHTransactionDetailModal> transactionDetailList = new List<ITHTransactionDetailModal>();
            var hotelOnlycontroller = DependencyResolver.Current.GetService<HotelOnlyController>();
            var IthMasterList = new List<ITHTransactionMasterModal>();
            foreach (var lst in OATReqList)
            {
                hotelOnlycontroller.GetLoginUserInfo(lst["EmpId"]);
                var empInfo = hotelOnlycontroller.TempData["EmpInfo"] as List<string>;
                var hodEmailId = _userRepository.GetHODEmailId(empInfo[8]);
                ITHTransactionMasterModal modal = new ITHTransactionMasterModal();
                modal.OATRequestID = Convert.ToInt64(lst["oatReqId"]);
                modal.RequestedDate = DateTime.Now;
                modal.RequestedBy = ithDetaillst.submittedBy;
                modal.Remarks = ithDetaillst.Remarks;
                modal.ITHVendorCode = ithDetaillst.ITHCode;
                modal.HODEmailID = hodEmailId.Split(',')[0];
                modal.IsReqSentToIth = true;

                var list = new Dictionary<string, object>();
                list = _oaTRepository.getOatDetailsPerReqId(Convert.ToInt64(lst["oatReqId"]));
                List<OATTravelRequestPassengerDetailModal> passengerList = list["oatPassangerList"] as List<OATTravelRequestPassengerDetailModal>;
                List<OATTravelRequestFlightDetailModal> flightList = list["oatFlightList"] as List<OATTravelRequestFlightDetailModal>;
                foreach (var Flst in flightList)
                {
                    ITHTransactionDetailModal fmodal = new ITHTransactionDetailModal();
                    fmodal.FlightType = Flst.FlightType;
                    fmodal.OriginPlace = Flst.OriginPlace;
                    fmodal.DestinationPlace = Flst.DestinationPlace;
                    fmodal.DepartureDate = Flst.DepartureDate;
                    fmodal.ArrivalDate = DateTime.Now;
                    fmodal.DepartureTime = Flst.DepartureTime;
                    fmodal.AirCraftName = Flst.AirCraftName;
                    fmodal.FlightNumber = Flst.FlightNumber;
                    fmodal.IsInternational = Flst.IsInternational;
                    fmodal.PassengerID = Flst.PassengerID;
                    fmodal.OATRequestID = Convert.ToInt64(lst["oatReqId"]);
                    transactionDetailList.Add(fmodal);
                }
                var trnId = _oaTRepository.saveIthReqDetail(modal, transactionDetailList);
                if (trnId != -1)
                {
                    string SubjectQuote = "This is with reference to the booking request as mentioned below:";
                    string bidingQuote = "<b>Please provide best Quote/Price for above passenger and Flight Details also Attach the flight rates herewith for Audit and Billing Purpose.</b> ";
                    String EmailSubjet = "OAT Booking Request Notification From SpiceJet:" + System.DateTime.Now.ToString(); 
                    var skey = new StringBuilder();
                    skey.Append(passengerList[0].OATTravelRequestId.ToString() + ",");
                    skey.Append(trnId.ToString() + ",");

                    var uri1 = ConfigurationManager.AppSettings["IthResponseFormPath"].Trim() + "?str=" + skey + "&type=a";
                    var uri2 = ConfigurationManager.AppSettings["IthResponseFormPath"].Trim() + "?str=" + skey + "&type=r";

                    var appLink = string.Empty;
                    appLink = "<table style='margin-top:10px;'><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a name='app' style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Acceptance</a></td> <td>&nbsp;</td> <td style='height:25px;text-align:center;border-radius:5px'><a name='rej' style='font-weight: bolder;margin-left: 15px;' href='" + uri2 + "'>If options are not available, kindly Revert/Reject the same</a> </td></tr></table>";
                    jsonmsg = SendMailToIth(Convert.ToInt64(lst["oatReqId"]), trnId, EmailSubjet, SubjectQuote, bidingQuote, appLink).ToString(); ;
                    TempData["jsonmsg"] = jsonmsg;
                }
            }
            return Json(jsonmsg, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ViewIthDetailAsPerReqID(String OatReqID)
        {
            //var dic = new Dictionary<String, object>();
            var dic = _oaTRepository.getIthDetailAsPerReqID(Convert.ToInt64(OatReqID));
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDetailForIthResponse(Int64 oatReqId, Int64 ithTransactionID, String Type)
        {
            var dic = new Dictionary<String, object>();
            dic = _oaTRepository.GetDetailForIthResponse(oatReqId, ithTransactionID, Type);
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string viewIthAttachedSrc(long Id, int criteria)
        {
            string base64String = _oaTRepository.viewIthAttachedSrc(Id, criteria);           
            TempData["src"] = base64String.Split('|')[0];
            TempData["Type"] = base64String.Split('|')[1];           
            return base64String;
        }

        public JsonResult RevertToITh(HttpPostedFileBase RejectionSrc)
        {
            string jsonmsg = String.Empty;
            try
            {
                string rejectionRemark = Request.QueryString["rejectionRemark"].ToString();
                string idDetail = Request.QueryString["idDetail"].ToString();
                string result = string.Empty;
                byte[] binData;

                if (RejectionSrc != null)
                {                    
                    using (BinaryReader b = new BinaryReader(RejectionSrc.InputStream))
                    {
                        binData = b.ReadBytes(RejectionSrc.ContentLength);
                        result = System.Text.Encoding.UTF8.GetString(binData);
                    }                   
                    var revertToIthSrc = new ITHTransactionDetailLogModal();
                    revertToIthSrc.ITHUploadRefFile = binData;
                    revertToIthSrc.OAtDeskUploadRefFileType = RejectionSrc.ContentType;
                    revertToIthSrc.ID = Convert.ToInt64(idDetail.Split('-')[2]);
                    revertToIthSrc.PassengerID = Convert.ToInt64(idDetail.Split('-')[0]);
                    revertToIthSrc.OATRequestID = Convert.ToInt64(idDetail.Split('-')[3]);
                    revertToIthSrc.TrnId = Convert.ToInt64(idDetail.Split('-')[1]);
                    revertToIthSrc.OriginPlace = idDetail.Split('-')[4];
                    revertToIthSrc.DestinationPlace = idDetail.Split('-')[5];
                    revertToIthSrc.OATDeskRemarks = rejectionRemark;
                    var s = _oaTRepository.RevertToITh(revertToIthSrc);
                    if (s >= 1)
                    {                       
                        var skey = new StringBuilder();
                        skey.Append(idDetail.Split('-')[3] + ",");
                        skey.Append(idDetail.Split('-')[1] + ",");
                        skey.Append(idDetail.Split('-')[2] + ",");
                        var uri3 = ConfigurationManager.AppSettings["ViewAttachment"].Trim() + "?str=" + skey;
                        string SubjectQuote = "This is with reference to modification the booking request as mentioned below:";
                        string bidingQuote = "<b>Your provided Quotes is not accepted by Traveldesk</b>, As per current availability of fares are on Higher side screen shot attached for your perusal.<br/><br/> <b> OAT Desk Remarks : </b>" + rejectionRemark + " " + "<a href='" + uri3 + "'> View Source </a>  <br/><br/> Please modify your best Quote/ Price for above passenger and Flight Details for the processing of the same."; 
                        String EmailSubjet = "Price Modification for OAT Booking Request Notification From SpiceJet Traveldesk:" + System.DateTime.Now.ToString(); 

                        var uri1 = ConfigurationManager.AppSettings["IthResponseFormPath"].Trim() + "?str=" + skey + "&type=ORa";
                        var uri2 = ConfigurationManager.AppSettings["IthResponseFormPath"].Trim() + "?str=" + skey + "&type=ORr";
                        var appLink = string.Empty;
                        appLink = "<br/><table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a name='app' style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Acceptance</a></td> <td>&nbsp;</td> <td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a name='rej' style='color:#fff; text-decoration:none;' href='" + uri2 + "'>Rejection</a> </td></tr></table>";
                        jsonmsg = SendMailToIth(Convert.ToInt64(idDetail.Split('-')[3]), Convert.ToInt64(idDetail.Split('-')[1]), EmailSubjet, SubjectQuote, bidingQuote, appLink).ToString(); ;
                        //TempData["jsonmsg"] = jsonmsg;
                    }
                }
                //}
            }
            catch (Exception ex)
            {
                jsonmsg = "Oops! Something went wrong";
                throw ex;
            }
            return Json(jsonmsg, JsonRequestBehavior.AllowGet);
        }
        public JsonResult cancellationRequestToIth(HttpPostedFileBase RejectionSrc)
        {
            string jsonmsg = String.Empty;
            string result = string.Empty;
            byte[] binData = { };
            try
            {
                string cancelReason = Request.QueryString["cancelReason"].ToString();
                var BookingDetail = Request.QueryString["BookingDetail"];
                var PassengerID = Convert.ToInt64(BookingDetail.Split(',')[0]);
                var OriginPlace = BookingDetail.Split(',')[1];
                var DestinationPlace = BookingDetail.Split(',')[2];
                string cancelType = Request.QueryString["cancelType"].ToString();
                var cancelBy = BookingDetail.Split(',')[3];
                var oatrequestID = BookingDetail.Split(',')[4];
                

                if (RejectionSrc != null || cancelType == "c")
                {                    
                    if (RejectionSrc != null)
                    {
                        using (BinaryReader b = new BinaryReader(RejectionSrc.InputStream))
                        {
                            binData = b.ReadBytes(RejectionSrc.ContentLength);
                            result = System.Text.Encoding.UTF8.GetString(binData);
                        }
                    }
                    var cancellationsrc = new OATTravelRequestFlightDetailModal();
                    cancellationsrc.CancellationAttachment = binData;
                    cancellationsrc.PassengerID = PassengerID;
                    cancellationsrc.OriginPlace = OriginPlace;
                    cancellationsrc.OATTravelRequestId = Convert.ToInt64(oatrequestID);
                    cancellationsrc.DestinationPlace = DestinationPlace;
                    cancellationsrc.CancellationReason = cancelReason;
                    cancellationsrc.IsFlightCancel = true;
                    cancellationsrc.FlightCancelBy = cancelBy;
                    cancellationsrc.CancelledTime = DateTime.Now;
                    cancellationsrc.CancelType = cancelType;
                    cancellationsrc.CancellationAttachmentType = RejectionSrc != null ? RejectionSrc.ContentType : "";
                    var oatReqId = _oaTRepository.cancelFlightReq(cancellationsrc, 1);
                    var trnID = 0;
                    if (oatReqId >= 1)
                    {
                        //check is reqbSet toIth Or not
                        var reqSent = _oaTRepository.ISReqSentToIth(oatReqId);
                        if (reqSent)
                        {
                            //Send mail to Ith
                            string SubjectQuote = "This is with reference to cancel the booking request as mentioned below:";
                            string bidingQuote = "<b style='color:red;'>This is to inform you that OAT Flight Booking has been cancelled.</b> <br/><br/>";
                            bidingQuote = bidingQuote + (cancelType == "c" ? "Please help to accord your acceptance and adjust the refundable amount. , The tentative cancellation charges can be provided, hence, you can change with final amount after 15 days on the auto triggered mail sent by Traveldesk" : "Please help to accord your acceptance and adjust the refundable amount");
                            String EmailSubjet = cancelType == "c" ? "OAT Flight Cancellation Request Notification from OAT Traveldesk :" + System.DateTime.Now.ToString() : "OAT Flight Full Refund Cancellation Request Notification from OAT Traveldesk:" + System.DateTime.Now.ToString();
                            var skey = new StringBuilder();
                            skey.Append(oatReqId + ",");
                            skey.Append(trnID + ",");
                            skey.Append(BookingDetail + ",");
                            var uri1 = ConfigurationManager.AppSettings["IthResponseFormPath"].Trim() + "?str=" + skey + "&type=CA";
                            // var uri2 = ConfigurationManager.AppSettings["IthResponseFormPath"].Trim() + "?str=" + skey + "&type=Cr";
                            var appLink = string.Empty;
                            appLink = "<br/><table><tr style='font-family:Arial;'><td style='padding:5px; background-color:#04B431;text-align:center;border-radius:5px'><a name='app' style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Please Acknowledge the same</a></td></tr></table>";
                            var dic = new Dictionary<String, object>();
                            
                            var emailTemplateName = "ITHResponsiveMailTemplateFromOatDesk.html";
                            try
                            {
                                dic = _oaTRepository.getIthDetailAsPerReqID(oatReqId);
                                var flightList = dic["FlightDetail"] as List<OATTravelRequestFlightDetailModal>;
                                flightList = flightList.Where(x => x.PassengerID == PassengerID && x.OriginPlace == OriginPlace && x.DestinationPlace == DestinationPlace).ToList();
                                var ithvendorlst = dic["IthVenderDetail"] as List<ITHVenderModal>;
                                var passengerList = dic["PassengerDetails"] as List<OATTravelRequestPassengerDetailModal>;
                                var IthMasterList = dic["ithMasterList"] as ITHTransactionMasterModal;
                                var emailCredentials = EmailCredentialsIthReq(EmailSubjet, emailTemplateName, passengerList, flightList, ithvendorlst[0]);
                                var templateData = emailCredentials.TemplateFilePath;
                                templateData = templateData.Replace("[appLink]", appLink);
                                templateData = templateData.Replace("[SubjectQuote]", SubjectQuote);
                                templateData = templateData.Replace("[bidingQuote]", bidingQuote);
                                emailCredentials.TemplateFilePath = templateData;
                                var emaildataHod = emailCredentials;
                                var emailidHod = ithvendorlst[0].PrimaryEmail.Trim();
                                var secondaryEmailId = ithvendorlst[0].SecondaryEmail.Trim();
                                string[] Sec_Email = new string[2];
                                Sec_Email[0] = ConfigurationManager.AppSettings["TravelDeskEmailId"].ToString();
                                if (secondaryEmailId != null)
                                {
                                    Sec_Email[1] = secondaryEmailId;
                                    EmailNotifications.SendBookingRequestNotificationTo_InCC(emaildataHod, emailidHod, Sec_Email);
                                }
                                else
                                    EmailNotifications.SendBookingRequestNotificationTo_HOD(emaildataHod, emailidHod);
                                //EmailNotifications.SendBookingRequestNotificationTo_HOD(emaildataHod, emailidHod);
                                jsonmsg = "Your request has been sent successfully";
                            }
                            catch (Exception ex)
                            {
                                jsonmsg = "Oops! Something went wrong";
                                throw ex;
                            }
                            //return jsonmsg;
                            jsonmsg = "Your cancellation request submitted successfully.";
                        }
                    }
                    else
                    {
                        jsonmsg = "Oops! Something went wrong, Your Cancellation Request is not Submiitted succesfully.";
                    }
                    //}
                    // }
                }
                else
                {
                    jsonmsg = "For Full Refund Cancellation, Please upload your supportive document.";
                }
            }
            catch (Exception ex)
            {
                jsonmsg = "Oops! Something went wrong";
                throw ex;
            }
            return Json(jsonmsg, JsonRequestBehavior.AllowGet);
        }

        public JsonResult cancellationRequestToWholeBookingRequest(HttpPostedFileBase RejectionSrc)
        {
            string jsonmsg = String.Empty;
            string result = string.Empty;
            byte[] binData = { };
            try
            {
                string cancelReason = Request.QueryString["cancelReason"].ToString();
                var BookingDetail = Request.QueryString["BookingDetail"];
                var oatrequestID = BookingDetail.Split(',')[0];
                //var OriginPlace = BookingDetail.Split(',')[1];
                // var DestinationPlace = BookingDetail.Split(',')[2];
                string cancelType = Request.QueryString["cancelType"].ToString();
                var cancelBy = BookingDetail.Split(',')[1];

                if (RejectionSrc != null || cancelType == "c")
                {                   
                    if (RejectionSrc != null)
                    {
                        using (BinaryReader b = new BinaryReader(RejectionSrc.InputStream))
                        {
                            binData = b.ReadBytes(RejectionSrc.ContentLength);
                            result = System.Text.Encoding.UTF8.GetString(binData);
                        }
                    }
                    var cancellationsrc = new OATTravelRequestFlightDetailModal();
                    cancellationsrc.CancellationAttachment = binData;
                    cancellationsrc.OATTravelRequestId = Convert.ToInt64(oatrequestID);
                    //cancellationsrc.OriginPlace = OriginPlace;
                    // cancellationsrc.DestinationPlace = DestinationPlace;
                    cancellationsrc.CancellationReason = cancelReason;
                    cancellationsrc.IsFlightCancel = true;
                    cancellationsrc.FlightCancelBy = cancelBy;
                    cancellationsrc.CancelledTime = DateTime.Now;
                    cancellationsrc.CancelType = cancelType;
                    cancellationsrc.CancellationAttachmentType = RejectionSrc != null ? RejectionSrc.ContentType : "";
                    var oatReqId = _oaTRepository.cancelFlightReq(cancellationsrc, 2);
                    var trnID = 0;
                    if (oatReqId >= 1)
                    {
                        //check is reqbSet toIth Or not
                        var reqSent = _oaTRepository.ISReqSentToIth(oatReqId);
                        if (reqSent)
                        {
                            //Send mail to Ith
                            string SubjectQuote = "This is with reference to cancel the booking request as mentioned below:";
                            string bidingQuote = "<b>This is to inform you that OAT Flight Booking has been cancelled.</b> <br/><br/>";
                             bidingQuote = bidingQuote + (cancelType == "c" ? "The tentative cancellation charges can be provided, you can change with final amount after 15 days on the auto triggered mail sent by Traveldesk.": "Please help to accord your acceptance and adjust the refundable amount");
                            String EmailSubjet = cancelType == "c" ? "OAT Flight Cancellation Request Notification From OAT Traveldesk :" : "OAT Flight Full Refund Cancellation Request Notification from OAT Traveldesk: " + System.DateTime.Now.ToString();
                            var skey = new StringBuilder();
                            skey.Append(oatReqId + ",");
                            skey.Append(trnID + ",");
                            var uri1 = ConfigurationManager.AppSettings["IthResponseFormPath"].Trim() + "?str=" + skey + "&type=Ca";
                            // var uri2 = ConfigurationManager.AppSettings["IthResponseFormPath"].Trim() + "?str=" + skey + "&type=Cr";
                            var appLink = string.Empty;
                            appLink = "<br/><table><tr style='font-family:Arial;'><td style='padding:5px; background-color:#04B431;text-align:center;border-radius:5px'><a name='app' style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Please Acknowledge</a></td></tr></table>";
                            jsonmsg = SendMailToIth(oatReqId, Convert.ToInt64(trnID), EmailSubjet, SubjectQuote, bidingQuote, appLink);
                        }
                        else
                        {
                            jsonmsg = "Your Cancellation request submiitted succesfully.";
                        }
                    }               
                }
                else
                {
                    jsonmsg = "For Full Refund Cancellation, Please upload your supportive document.";
                }
            }
            catch (Exception ex)
            {
                jsonmsg = "Oops! Something went wrong";
                // throw ex;
            }
            return Json(jsonmsg, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ConfirmIThResponse(List<ITHTransactionDetailModal> SelectedResponse)
        {
            var jsonmsg = string.Empty;
            try
            {
                var s = _oaTRepository.ConfirmIThResponse(SelectedResponse);
                if (s > 0)
                {
                    var oatReqID = SelectedResponse[0].OATRequestID;
                    var TrnId = SelectedResponse[0].TrnId;
                    var emailSubject = "User Acceptance for OAT Booking Request Notification :" + System.DateTime.Now.ToString();
                    var SubjectQuote = " ";
                    var uri1 = ConfigurationManager.AppSettings["IthResponseFormPath"].Trim() + "?str=" + "SKEY" + "&type=ua";
                    var bidingQuote = "<b>Note : </b> As per company SOP the L1 will be selected, If you have to override the Company SOP, please confirm to HOD that you have selected the higher fares and so that he approves the same, after your approval. <br/><br/> To Modify OAT Travel Desk Choice   <b><a  href='" + uri1 + "'>Please Click Here</a> </b> or Please help to accord your acceptance or rejection booking-approval via below button";
                    var msg = SendMailToHod(oatReqID, TrnId, emailSubject, SubjectQuote, bidingQuote, "User").ToString();
                    if (msg != "Fail") jsonmsg = "Your choice is saved successfully and send to the user for further processing request.";
                }
            }
            catch (Exception ex)
            {
                jsonmsg = "Oops! Something went Wrong.";
                throw ex;
            }
            return Json(jsonmsg, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetFinancialApproval(int ReqId)
        {
            var dic = new Dictionary<String, object>();
            var FinancialApproval = new List<OATFinancialApprovalMaster_RoisteringModal>();         
            string bookingType = _oaTRepository.getBookingType(Convert.ToInt64(ReqId));
            if (bookingType == "Roistering(Flight ops / In Flight)")
            {
                FinancialApproval = _oaTRepository.getFinacialMasterDetail(Convert.ToInt64(ReqId));
            }
            bool IsAmountGreater = _oaTRepository.getAmountForFinancialApproval(Convert.ToInt64(ReqId));          
            if (bookingType != "Roistering(Flight ops / In Flight)" || IsAmountGreater != true)
            {
                IsAmountGreater = false;
            }
            dic.Add("IsAmountGreater", IsAmountGreater);
            dic.Add("FinancialApproval", FinancialApproval);
            
            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SubmitUserResponse(List<ITHTransactionDetailModal> SelectedResponse, List<OATFinancialApprovalMaster_RoisteringModal> FinancialApproval)
        {
            var jsonmsg = string.Empty;
            var msg = string.Empty;
            try
            {
                var s = _oaTRepository.SubmitUserResponse(SelectedResponse);
                if (s > 0)
                {
                    var oatReqID = SelectedResponse[0].OATRequestID;
                    string bookingType = _oaTRepository.getBookingType(SelectedResponse[0].OATRequestID);

                    if (bookingType == "Roistering(Flight ops / In Flight)")
                    {
                        var listTosendFinancialApproval = new List<OATFinancialApprovalDetail_RoisteringModal>();
                        var getListForApproval = SelectedResponse.Where(p => p.Amount >= 8000).ToList();
                        foreach (var lst in getListForApproval)
                        {
                            var modal = new OATFinancialApprovalDetail_RoisteringModal();
                            modal.OATId = lst.OATRequestID;
                            modal.PassengerId = lst.PassengerID;
                            modal.FlightId = lst.ID;
                            modal.Sector = lst.OriginPlace + "-" + lst.DestinationPlace;
                            modal.ApproverEmpCode = FinancialApproval[0].EmpCode;
                            modal.ApproverEmpName = FinancialApproval[0].EmpName;
                            modal.ApprovedAmount = (int)lst.Amount;
                            modal.ApprovalDate = DateTime.Now;
                            modal.ApprovalStatus = 0;
                            modal.ApproverEmailID = FinancialApproval[0].EmailId;
                            listTosendFinancialApproval.Add(modal);
                        }
                        
                        var s1 = _oaTRepository.saveFinacialDetailForApproval(listTosendFinancialApproval);
                        if (s1 > 0)
                        {                           
                            var TrnId = SelectedResponse[0].TrnId;
                            var emailSubject = "Financial Approval Request for OAT Booking Request Notification :" + System.DateTime.Now.ToString();
                            var SubjectQuote = " ";
                            var uri1 = ConfigurationManager.AppSettings["IthResponseFormPath"].Trim() + "?str=" + "SKEY" + "&type=ha";
                            var bidingQuote = "";// "<b>Note : </b> Colored border is OatDesk Choice <br/> To Modify OATDESK Choice  <b><a  href='" + uri1 + "'>Please Click Here</a> </b> or Please help to accord your acceptance or rejection booking-approval via below button";
                            msg = SendMailToHod(oatReqID, TrnId, emailSubject, SubjectQuote, bidingQuote, "Finance");
                        }

                        //send Mail to Ith for Final booking
                        sendMailToIthForBooking(oatReqID);
                        if (msg != "Fail") jsonmsg = "Your response has been submitted sucessfully and sent to " + FinancialApproval[0].EmailId + " to get financial approval for further processing.";
                    }
                    else if (bookingType == "SPOC")
                    {
                        //send Mail to Ith for Final booking
                        sendMailToIthForBooking(oatReqID);
                        jsonmsg = "Your response has been submitted sucessfully and sent for further processing.";
                    }
                    else
                    {
                       //var oatReqID = SelectedResponse[0].OATRequestID;
                        var TrnId = SelectedResponse[0].TrnId;
                        var emailSubject = "HOD Approval Request for OAT Booking Request Notification :" + System.DateTime.Now.ToString();
                        var SubjectQuote = " ";
                        var uri1 = ConfigurationManager.AppSettings["IthResponseFormPath"].Trim() + "?str=" + "SKEY" + "&type=ha";
                        var bidingQuote = "<b>Note : Highlighted Colored border is OAT Travel Desk and User Choice,[Name], you have been prompted to approve twice due to choosing of higher fares.</b> <br/> To Modify OATDESK Choice  <b><a  href='" + uri1 + "'>Please Click Here</a> </b> or Please help to accord your acceptance or rejection booking-approval via below button";
                        msg = SendMailToHod(oatReqID, TrnId, emailSubject, SubjectQuote, bidingQuote, "Hod");
                        if (msg != "Fail") jsonmsg = "Your response has been submitted sucessfully and sent to " + msg + " to get HOD approval for further processing.";
                    }
                }
            }
            catch (Exception ex)
            {
                jsonmsg = "Fail";
                throw ex;
            }

            return Json(jsonmsg, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SubmitHodResponse(List<ITHTransactionDetailModal> SelectedResponse)
        {
            var jsonmsg = string.Empty;
            try
            {
                var s = _oaTRepository.SubmitHodResponse(SelectedResponse);
                if (s > 0)
                {
                    //send Mail to Ith for Final booking
                    sendMailToIthForBooking(SelectedResponse[0].OATRequestID);
                }
                jsonmsg = "Your response has been submitted sucessfully and sent to OAT for further processing.";
            }
            catch (Exception ex)
            {
                jsonmsg = "Oops! Something went wrong.";
                throw ex;
            }
            return Json(jsonmsg, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult sendHoldRequestToIth(Int64 oatReqId ,string holdBy)
        {
            var jsonmsg = string.Empty;
            try
            {
                var s = _oaTRepository.sendHoldRequestToIth(oatReqId, holdBy);
                if (s > 0)
                {
                    var trnID = 0;
                    //Revert mail to Ith
                    string SubjectQuote = "This is with reference to OAT booking request as mentioned below:";
                    string bidingQuote = "This is to inform you that OAT Flight booking request notification is on <b>HOLD </b>, so please do not book the ticket till the next confirmation comes to you. If not accepted in 10 minutes it will automatically be on hold";
                    String EmailSubjet = "OAT Hold Booking Request Notification from spicejet :" + System.DateTime.Now.ToString();
                    var skey = new StringBuilder();
                    skey.Append(oatReqId + ",");
                    skey.Append(trnID + ",");
                    var uri1 = ConfigurationManager.AppSettings["IthResponseFormPath"].Trim() + "?str=" + skey + "&type=HOA";
                    //var uri2 = ConfigurationManager.AppSettings["IthResponseFormPath"].Trim() + "?str=" + skey + "&type=Hr";
                    var appLink = string.Empty;
                    appLink = "<br/><table><tr style='font-family:Arial;'><td style='padding:5px;background-color:#04B431;text-align:center;border-radius:5px'><a name='app' style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Please acknowledge your Acceptance</a></td></tr></table>";
                    jsonmsg = SendMailToIth(Convert.ToInt64(oatReqId), Convert.ToInt64(trnID), EmailSubjet, SubjectQuote, bidingQuote, appLink);
                }
            }
            catch (Exception ex)
            {
                jsonmsg = ex.InnerException.Message;
            }
            return Json(jsonmsg, JsonRequestBehavior.AllowGet);
        }

       [HttpPost]
        public JsonResult undoHoldRequest(Int64 oatReqId, string holdBy)
        {
            var jsonmsg = string.Empty;
            try
            {
                var s = _oaTRepository.undoHoldRequestToIth(oatReqId, holdBy);
                if (s > 0)
                {
                    var trnID = 0;
                    //Revert mail to Ith
                    string SubjectQuote = "This is with reference to OAT booking request as mentioned below:";
                    string bidingQuote = "This is to inform you that OAT Flight booking request notification is  <b>UNHOLD </b>, so please book the ticket for this booking request.";
                    String EmailSubjet = "OAT UnHold Booking Request Notification from spicejet :" + System.DateTime.Now.ToString();
                    var skey = new StringBuilder();
                    skey.Append(oatReqId + ",");
                    skey.Append(trnID + ",");
                    var uri1 = ConfigurationManager.AppSettings["IthResponseFormPath"].Trim() + "?str=" + skey + "&type=HOA";
                    //var uri2 = ConfigurationManager.AppSettings["IthResponseFormPath"].Trim() + "?str=" + skey + "&type=Hr";
                    var appLink = string.Empty;
                    appLink = "<br/><table><tr style='font-family:Arial;'><td style='padding:5px;background-color:#04B431;text-align:center;border-radius:5px'><a name='app' style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Please acknowledge your Acceptance</a></td></tr></table>";
                    jsonmsg = SendMailToIth(Convert.ToInt64(oatReqId), Convert.ToInt64(trnID), EmailSubjet, SubjectQuote, bidingQuote, appLink);
                }
            }
            catch (Exception ex)
            {
                jsonmsg = ex.InnerException.Message;
            }
            return Json(jsonmsg, JsonRequestBehavior.AllowGet);
        }

        public String sendFinalConfirmationToUser(Int64 oatReqId)
        {
            var result = string.Empty;
            try
            {
                var trnID = 0;
                var skey = new StringBuilder();
                skey.Append(oatReqId + ",");
                skey.Append(trnID + ",");
                //Revert mail to Ith
                var uri1 = ConfigurationManager.AppSettings["IthFinalBookingForm"].Trim() + "?str=" + skey + "&type=SI";
                string SubjectQuote = "This is with reference to the booking, please find below flight booking details &amp; PNR against<b> OAT Booking Information (Request ID: 251)</b>";
                string bidingQuote = "<a href='"+ uri1 + "'>This is to inform you that your ticket is booked.";
                String EmailSubjet = "OAT Booking Confirmation from Service Provider/OAT Tarveldesk Notification:" + System.DateTime.Now.ToString();
                var jsonmsg = string.Empty;
                var dic = new Dictionary<String, object>();
             
                var emailTemplateName = "ITHResponsiveMailTemplateFromOatDesk.html";
                    dic = _oaTRepository.getIthDetailAsPerReqID(oatReqId);
                    var flightList = dic["FlightDetail"] as List<OATTravelRequestFlightDetailModal>;
                    var ithvendorlst = dic["IthVenderDetail"] as List<ITHVenderModal>;
                    var passengerList = dic["PassengerDetails"] as List<OATTravelRequestPassengerDetailModal>;
                    var OATMasterList = dic["RequestedEmployee"] as List<OATTravelRequestMasterModal>;
                     ithvendorlst[0].ITHName = OATMasterList[0].Gender + " " + OATMasterList[0].RequestedEmpName;
                    var emailCredentials = EmailCredentialsIthReq(EmailSubjet, emailTemplateName, passengerList, flightList, ithvendorlst[0]);
                    var templateData = emailCredentials.TemplateFilePath;
                    templateData = templateData.Replace("[appLink]", "");
                    templateData = templateData.Replace("[SubjectQuote]", SubjectQuote);
                    templateData = templateData.Replace("[bidingQuote]", bidingQuote);
                    emailCredentials.TemplateFilePath = templateData;
                    var emaildataHod = emailCredentials;
                    var emailidHod = OATMasterList[0].EmailId.Trim();
                    //var secondaryEmailId = ithvendorlst[0].SecondaryEmail.Trim();
                    string[] Sec_Email = new string[2];
                    Sec_Email[0] = ConfigurationManager.AppSettings["TravelDeskEmailId"].ToString();
                    //if (secondaryEmailId != null)
                    
                       // Sec_Email[1] = secondaryEmailId;
                        EmailNotifications.SendBookingRequestNotificationTo_InCC(emaildataHod, emailidHod, Sec_Email);
                // }
                //else
                //  EmailNotifications.SendBookingRequestNotificationTo_HOD(emaildataHod, emailidHod);
                SendSMSforUploadItenary(oatReqId.ToString(), OATMasterList, uri1.ToString());

                result = "Mail has been sent successfully to the user.";
            }
            catch (Exception ex)
            {
                result = "Oops! Something went wrong.";
            }
            return result;
        }

        

              /// <summary>
        /// Send SMS for Approval
        /// </summary>
        /// <param name="bookingID"></param>
        /// <param name="hoddetails"></param>
        /// <param name="requesterName"></param>
        /// <param name="approvalURI"></param>
        /// <param name="rejectionURI"></param>
        /// <param name="selectiveURI"></param>
        public void SendSMSforUploadItenary(String OatReqId, List<OATTravelRequestMasterModal> userDetail, string ItenaryURI)
        {
            var smsText = ConfigurationManager.AppSettings["smsOALUserDownloadItenary"].ToString().Replace("@Username", userDetail[0].Gender + userDetail[0].RequestedEmpName);

            smsText = smsText.Replace("[AppQueryString]", ItenaryURI);

            smsText = System.Uri.EscapeDataString(smsText);
            smsText = System.Uri.EscapeDataString(smsText);
            smsText = smsText.Replace("25", "");
            //Send SMS
            var smsLogModel = new SodApproverSMSLogModels();
            smsLogModel.TrRequestId = Convert.ToInt64(OatReqId);
            smsLogModel.EmpCode = userDetail[0].RequestedEmpCode;
            smsLogModel.EmpName = userDetail[0].Gender + userDetail[0].RequestedEmpName;
            smsLogModel.EmailID = userDetail[0].EmailId;
            smsLogModel.MobileNo = userDetail[0].PhoneNo;
            smsLogModel.Source = "OAT";
            smsLogModel.SMSText = smsText;
            smsLogModel.DeliveryDate = DateTime.Now;
            var status = SmsNotification.SmsNotifications.SendSmsViaApi(smsText, userDetail[0].PhoneNo);
            smsLogModel.IsDelivered = status.Equals(true) ? true : false;
            _bulkUploadRepository.SaveApproverSMSLog(smsLogModel);
        }

        [HttpPost]
        public JsonResult sendMailToIthForBooking(Int64 oatReqId)
        {
            var jsonmsg = string.Empty;
            try
            {
                var bookingType = _oaTRepository.getBookingType(oatReqId);              
                var trnID = 0;
                //Revert mail to Ith
                string SubjectQuote = "This is with reference to OAT booking request as mentioned below- Please book the ticket with suggested price by SpiceJet Traveldesk";
                string bidingQuote = "This is for final booking of below passenger.";
                String EmailSubjet = "OAT Booking Request Confirmation Notification from SpiceJet Traveldesk: :" + System.DateTime.Now.ToString();
                var skey = new StringBuilder();
                skey.Append(oatReqId + ",");
                skey.Append(trnID + ",");
                var uri1 = ConfigurationManager.AppSettings["IthFinalBookingForm"].Trim() + "?str=" + skey + "&type=FBA";
                var uri2 = ConfigurationManager.AppSettings["IthFinalBookingForm"].Trim() + "?str=" + skey + "&type=FBR";
                var appLink = string.Empty;
                appLink = "<br/><table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a name='app' style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Acceptance</a></td> <td>&nbsp;</td> <td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a name='rej' style='color:#fff; text-decoration:none;' href='" + uri2 + "'>Rejection</a> </td></tr></table>";
                jsonmsg = SendMailToIth(Convert.ToInt64(oatReqId), Convert.ToInt64(trnID), EmailSubjet, SubjectQuote, bidingQuote, appLink);

            }
            catch (Exception ex)
            {
                jsonmsg = ex.InnerException.Message;
            }
            return Json(jsonmsg, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// Send Mail To Hod
        /// </summary>
        /// <param name="oatReqId"></param>
        /// <param name="TrnId"></param>
        /// <param name="EmailSubject"></param>
        /// <param name="SubjectQuote"></param>
        /// <param name="bidingQuote"></param>
        /// <param name="responseTo"></param>
        /// <returns></returns>
        public String SendMailToHod(Int64 oatReqId, Int64 TrnId, String EmailSubject, string SubjectQuote, string bidingQuote, string responseTo)
        {
            var jsonmsg = "";
            string hodEmailId = "";
            var hodName = "";
            string senderEmpID = "";
            var type1 = "";
            var type2 = "Ur";
            string disclaimer = "";
            var emailTemplateName = "OatRequestNotificationTemplateHodApproval_ITH.html";
            try
            {
                //get detail as per oatreqID
                var dic = _oaTRepository.getIthDetailAsPerReqID(oatReqId);
                var flightInfo = dic["FlightDetail"] as List<OATTravelRequestFlightDetailModal>;
                var ithvendorlst = dic["IthVenderDetail"] as List<ITHVenderModal>;
                var passangerInfo = dic["PassengerDetails"] as List<OATTravelRequestPassengerDetailModal>;
                var personalInfo = dic["RequestedEmployee"] as List<OATTravelRequestMasterModal>;
                var IthDetails = dic["IthDetails"] as List<ITHTransactionDetailModal>;
                //get Detail for sending mail
                if (responseTo == "User")
                {
                    hodEmailId = personalInfo[0].EmailId;
                    hodName = personalInfo[0].Gender + " " + personalInfo[0].RequestedEmpName;
                    senderEmpID = personalInfo[0].RequestedEmpId.ToString();
                    type1 = "UA";
                    type2 = "UR";
                    disclaimer = "<b style='color:red'>Disclaimer</b>: Your HOD can modify the flight details if required. All Fares unless ticketed are not guaranteed.";

                }
                else if (responseTo == "Finance")
                {
                    var financeDetail = _oaTRepository.GetFinacialDetailForApproval(oatReqId);
                    hodEmailId = financeDetail.ApproverEmailID;
                    hodName = financeDetail.ApproverEmpName;
                    senderEmpID = personalInfo[0].RequestedEmpId.ToString();
                    type1 = "FA";
                    type2 = "FR";
                }
                else if (responseTo == "C-Level")
                {
                    var cxoDetail = _oaTRepository.getClevelApproverMail(oatReqId);
                    hodEmailId = cxoDetail.Split('|')[0];
                    hodName = cxoDetail.Split('|')[1];
                    senderEmpID = personalInfo[0].RequestedEmpId.ToString();
                    type1 = "CLA";
                    type2 = "CLR";
                }
                else
                {
                    // var reqEmpId = _oaTRepository.getEmpIdAsPerReqId(oatReqId);
                    var hodDetail = SOD.Services.ADO.SodCommonServices.GetHodEmailDetails(personalInfo[0].RequestedEmpId.ToString().Trim(), 2);
                    hodEmailId = hodDetail.Split(',')[0];
                    hodName = hodDetail.Split(',')[1];
                    senderEmpID = hodDetail.Split(',')[2];
                    type1 = "HA";
                    type2 = "HR";
                    bidingQuote = bidingQuote.Replace("[Name]", hodName.ToString());
                    disclaimer = "<b style='color:red'>Disclaimer</b>: This approval will also valid on changes in flight amount up to 1000 (INR) to 2000 (INR). All Fares unless ticketed are not guaranteed.";
                }
                if (hodEmailId.Length > 0)
                {
                    var skey = new StringBuilder();
                    skey.Append(oatReqId.ToString() + ",");
                    skey.Append(TrnId.ToString() + ",");
                    skey.Append(senderEmpID + ",");
                    var uri1 = ConfigurationManager.AppSettings["IthResponseFormPath"].Trim() + "?str=" + skey + "&type=" + type1;
                    var uri2 = ConfigurationManager.AppSettings["IthResponseFormPath"].Trim() + "?str=" + skey + "&type=" + type2;
                    var appLink = string.Empty;
                    appLink = "<table style='margin-top:10px;'><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a name='app' style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Acceptance</a></td>";
                    appLink = appLink + "<td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a name='rej' style='color:#fff; text-decoration:none;' href='" + uri2 + "'>Rejection</a></td> </tr></table>";
                    appLink = appLink + "<br/>" + disclaimer;
                    //Read Template to bind Data
                    var emailCredentials = EmailCredentialsHotelHod(EmailSubject, emailTemplateName, personalInfo, passangerInfo, flightInfo, ithvendorlst[0], IthDetails, oatReqId.ToString(), hodName, responseTo);
                    var templateData = emailCredentials.TemplateFilePath;
                    var stats = bidingQuote.Replace("SKEY", skey.ToString());
                    templateData = templateData.Replace("[mofificationstatement]", stats);
                    templateData = templateData.Replace("[appLink]", appLink);
                    templateData = templateData.Replace("[hodName]", hodName);
                    if (responseTo == "User")
                        templateData = templateData.Replace("This is with reference to the booking request received from <b>[RequesterName]</b> as mentioned below:", "This is with reference to the booking, <b>please find below flight & rate details for your information as mention below</b>:");
                    emailCredentials.TemplateFilePath = templateData;

                    var emaildataHod = emailCredentials;
                    var emailidHod = hodEmailId.Split(',')[0];
                    EmailNotifications.SendBookingRequestNotificationTo_HOD(emaildataHod, emailidHod);
                    jsonmsg = emailidHod;
                }
            }
            catch (Exception ex)
            {
                jsonmsg = "Fail";
                throw ex;
            }

            return jsonmsg;
        }

        public String SendMailToIth(Int64 oatReqId, Int64 TrnId, String EmailSubject, string SubjectQuote, string bidingQuote, string applink)
        {
            var jsonmsg = string.Empty;
            var dic = new Dictionary<String, object>();
            var emailSubject = EmailSubject;
            var emailTemplateName = "ITHResponsiveMailTemplateFromOatDesk.html";
            try
            {
                dic = _oaTRepository.getIthDetailAsPerReqID(oatReqId);
                var flightList = dic["FlightDetail"] as List<OATTravelRequestFlightDetailModal>;
                var ithvendorlst = dic["IthVenderDetail"] as List<ITHVenderModal>;
                var passengerList = dic["PassengerDetails"] as List<OATTravelRequestPassengerDetailModal>;
                var IthMasterList = dic["ithMasterList"] as ITHTransactionMasterModal;
                var emailCredentials = EmailCredentialsIthReq(emailSubject, emailTemplateName, passengerList, flightList, ithvendorlst[0]);
                var templateData = emailCredentials.TemplateFilePath;
                templateData = templateData.Replace("[appLink]", applink);
                templateData = templateData.Replace("[SubjectQuote]", SubjectQuote);
                templateData = templateData.Replace("[bidingQuote]", bidingQuote);
                emailCredentials.TemplateFilePath = templateData;
                var emaildataHod = emailCredentials;
                var emailidHod = ithvendorlst[0].PrimaryEmail.Trim();
                var secondaryEmailId = ithvendorlst[0].SecondaryEmail.Trim();
                string[] Sec_Email = new string[2];
                Sec_Email[0] = ConfigurationManager.AppSettings["TravelDeskEmailId"].ToString();
                if (secondaryEmailId != null)
                {
                    Sec_Email[1] = secondaryEmailId;
                    EmailNotifications.SendBookingRequestNotificationTo_InCC(emaildataHod, emailidHod, Sec_Email);
                }
                else
                    EmailNotifications.SendBookingRequestNotificationTo_HOD(emaildataHod, emailidHod);
                //EmailNotifications.SendBookingRequestNotificationTo_HOD(emaildataHod, emailidHod);
                jsonmsg = "Your request has been sent successfully";
            }
            catch (Exception ex)
            {
                jsonmsg = "Oops! Something went wrong";
                throw ex;
            }
            return jsonmsg;
        }
        


        public string SaveUploadSrc(HttpPostedFileBase src, String path)
        {
            var result = String.Empty;
            var Resources = "../OatUploadAttachments/" + path + "/";
            try
            {
                string fileName = src.FileName;
                string fileContentType = src.ContentType;
                //Save File
                var fileNames = Path.GetFileNameWithoutExtension(src.FileName).Trim();
                var fileExt = Path.GetExtension(src.FileName);
                var time = System.DateTime.Now.ToString();
                var DirectoryName = Server.MapPath(Resources);
                path = Path.Combine(DirectoryName, fileNames + "_" + time + fileExt);
                if (!Directory.Exists(DirectoryName))
                {
                    // Try to create the directory.
                    Directory.CreateDirectory(DirectoryName);
                }
                src.SaveAs(path);
                result = path;
            }
            catch (Exception ex)
            {
                result = "Fail";
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// Initialized Email Credentials to Property
        /// </summary>
        /// <returns></returns>
        public EmailNotificationModel EmailCredentialsHotelHod(String subjectName, String emailTemplateName, List<OATTravelRequestMasterModal> personalInfo, List<OATTravelRequestPassengerDetailModal> passangerInfo, List<OATTravelRequestFlightDetailModal> flightInfo, ITHVenderModal ithvendorlst, List<ITHTransactionDetailModal> IthDetails, string oatReqID, string hodname, string responseTO)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtpUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtpPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtpServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtpPort"].Trim(),
                TemplateFilePath = ReadFileHotelHod(emailTemplateName, personalInfo, passangerInfo, flightInfo, ithvendorlst, IthDetails, oatReqID, hodname, responseTO),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
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
        private string ReadFileHotelHod(String emailTemplateName, List<OATTravelRequestMasterModal> personalInfo, List<OATTravelRequestPassengerDetailModal> passangerInfo, List<OATTravelRequestFlightDetailModal> flightInfo, ITHVenderModal ithvendorlst, List<ITHTransactionDetailModal> IthDetails, string oatReqID, string HodName, string ResponseTO)
        {
            var strContent = new StringBuilder();
            string line;
            var file = new System.IO.StreamReader(
                       System.Web.HttpContext.Current.Server.MapPath("~/Template/OAT/" + emailTemplateName));
            while ((line = file.ReadLine()) != null)
            {
                strContent.Append(line);
            }
            file.Close();

            //var Title = personalInfo[0].Gender == "F" ? "Ms." : "Mr.";

            strContent = strContent.Replace("[RequesterName]", personalInfo[0].Gender + " " + personalInfo[0].RequestedEmpName);
            strContent = strContent.Replace("[binfo]", "OAT Booking Information   (Request ID : " + oatReqID + ")");
            strContent = strContent.Replace("[pinfo]", "  Passenger Information(s)");
            strContent = strContent.Replace("[finfo]", "  Flight Information(s)");
            strContent = strContent.Replace("[hinfo]", "  Option Provided By OAT Travel Desk");

            var tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Reason for Booking</td><td>Booking For</td><td>No. of Guests</td><td>Booking Type</td></tr>";
            foreach (var b in personalInfo)
            {
                tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td>" + b.ReasonForTravel + "</td><td>" + b.BookingFor + "</td><td>" + b.PaxNo + "</td><td>" + b.BookingType + "</td></tr>";
            }

            var trp = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>S.No.</td><td>Name</td><td>Designation</td><td>Department</td><td>Email</td><td>Phone No.</td></tr>";
            var countP = 1;
            foreach (var b in passangerInfo)
            {
                //var PTitle = b.Gender == "F" ? "Ms." : "Mr.";
                if (b.IsFlightRequired)
                {
                    trp = trp + "<tr style='font-family:Arial; font-size:12px;'><td>" + countP + "</td><td>" + b.Gender + b.FirstName + " " + b.LastName + "</td><td>" + b.Designation + "</td><td>" + b.Department + "</td><td>" + b.EmailId + "</td ><td>" + b.PhoneNo + "</td></tr>";
                }
                countP++;
            }
            var uniqueSectorFlight = flightInfo.GroupBy(d => new { d.OriginPlace, d.DestinationPlace, d.DepartureDate }).Select(group => group.First()).ToList();
            //flightInfo.Select(m => new { m.OriginPlace, m.DestinationPlace }).Distinct().ToList();
            var trh = "";
            foreach (var u in uniqueSectorFlight)
            {
                var counth = 1;
                trh = trh + "<h3>Sector: " + u.OriginPlace + " - " + u.DestinationPlace + "</h3>";
                if (u.IsFlightCancel) {
                    trh = trh + "<h4 style='padding: 5px; color:red '><b>This Sector is Cancelled. </b></h4>";
                }
                else
                {
                    trh = trh + " <table cellpadding='0' cellspacing='0' style='width:100%; font-size:12px;border:solid 1px #c2c2c2;line-height:30px;'><tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>S.No</td><td>Travel Date</td><td>Airline Name</td><td>Flight No.</td><td>Departure Time</td><td>Arrival Time</td><td>Price</td></tr>";
                    foreach (var h in IthDetails)
                    {
                        if (h.OriginPlace == u.OriginPlace && h.DestinationPlace == u.DestinationPlace)
                        {
                            if (ResponseTO == "Hod" || ResponseTO == "C-Level" || ResponseTO == "Finance")
                            {
                                if (h.UserApproval == 1)
                                {
                                    trh = trh + "<tr style='font-family:Arial; font-size:12px; background-color:lightyellow'><td style='border-top: 1px solid green; border-left: 1px solid green;border-bottom: 1px solid green; '>" + counth + "</td><td style='border-top: 1px solid green; border-bottom: 1px solid green;'>" + h.DepartureDate.ToString("dd/MMM/yyyy") + "</td><td style='border-top: 1px solid green; border-bottom: 1px solid green;'>" + h.AirCraftName + "</td><td style='border-top: 1px solid green; border-bottom: 1px solid green;'>" + h.FlightNumber + "</td><td style='border-top: 1px solid green; border-bottom: 1px solid green;'>" + h.DepartureTime + "</td><td style='border-top: 1px solid green; border-bottom: 1px solid green;'>" + h.ArrivalTime + "</td><td style='border-top: 1px solid green;border-Right: 1px solid green; border-bottom: 1px solid green;'>" + h.Amount + "</td></tr>";
                                }
                                else if (h.OATDeskApproval == 1)
                                {
                                    trh = trh + "<tr style='font-family:Arial; font-size:12px; background-color:#cefaca'><td style='border-top: 1px solid green; border-left: 1px solid green;border-bottom: 1px solid green; '>" + counth + "</td><td style='border-top: 1px solid green; border-bottom: 1px solid green;'>" + h.DepartureDate.ToString("dd/MMM/yyyy") + "</td><td style='border-top: 1px solid green; border-bottom: 1px solid green;'>" + h.AirCraftName + "</td><td style='border-top: 1px solid green; border-bottom: 1px solid green;'>" + h.FlightNumber + "</td><td style='border-top: 1px solid green; border-bottom: 1px solid green;'>" + h.DepartureTime + "</td><td style='border-top: 1px solid green; border-bottom: 1px solid green;'>" + h.ArrivalTime + "</td><td style='border-top: 1px solid green;border-Right: 1px solid green; border-bottom: 1px solid green;'>" + h.Amount + "</td></tr>";
                                }
                                else
                                {
                                    trh = trh + "<tr style='font-family:Arial; font-size:12px;'><td>" + counth + "</td><td>" + h.DepartureDate.ToString("dd/MMM/yyyy") + "</td><td>" + h.AirCraftName + "</td><td>" + h.FlightNumber + "</td><td>" + h.DepartureTime + "</td><td>" + h.ArrivalTime + "</td><td>" + h.Amount + "</td></tr>";
                                }
                            }
                            if (ResponseTO == "User")
                            {
                                if (h.OATDeskApproval == 1)
                                {
                                    trh = trh + "<tr style='font-family:Arial; font-size:12px; background-color:#cefaca'><td style='border-top: 1px solid green; border-left: 1px solid green;border-bottom: 1px solid green; '>" + counth + "</td><td style='border-top: 1px solid green; border-bottom: 1px solid green;'>" + h.DepartureDate.ToString("dd/MMM/yyyy") + "</td><td style='border-top: 1px solid green; border-bottom: 1px solid green;'>" + h.AirCraftName + "</td><td style='border-top: 1px solid green; border-bottom: 1px solid green;'>" + h.FlightNumber + "</td><td style='border-top: 1px solid green; border-bottom: 1px solid green;'>" + h.DepartureTime + "</td><td style='border-top: 1px solid green; border-bottom: 1px solid green;'>" + h.ArrivalTime + "</td><td style='border-top: 1px solid green;border-Right: 1px solid green; border-bottom: 1px solid green;'>" + h.Amount + "</td></tr>";
                                }
                                else
                                {
                                    trh = trh + "<tr style='font-family:Arial; font-size:12px;'><td>" + counth + "</td><td>" + h.DepartureDate.ToString("dd/MMM/yyyy") + "</td><td>" + h.AirCraftName + "</td><td>" + h.FlightNumber + "</td><td>" + h.DepartureTime + "</td><td>" + h.ArrivalTime + "</td><td>" + h.Amount + "</td></tr>";
                                }
                            }
                        }
                        counth++;
                    }
                    trh = trh + "</table>";
                }
               
            }
            var trf = "";
            var countf = 0;
            trf = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px'><td style='border-bottom: 1px solid #c2c2c2;'>S. No.</td><td style='border-bottom: 1px solid #c2c2c2;'>Emp Name</td><td style='border-bottom: 1px solid #c2c2c2;'>Sector</td><td style='border-bottom: 1px solid #c2c2c2;'>Travel Date</td><td style='border-bottom: 1px solid #c2c2c2;'>Departure time</td><tdstyle='border-bottom: 1px solid #c2c2c2;'>Airline Name</td><tdstyle='border-bottom: 1px solid #c2c2c2;'>Flight No.</td></tr>";
            foreach (var h in flightInfo)
            {
                countf++;
                var fltNo = h.FlightNumber == null ? "-" : h.FlightNumber;
                var aircraftName = h.AirCraftName == null ? "-" : h.AirCraftName;
                trf = trf + "<tr style='font-family:Arial; font-size:12px;'><td style='border-bottom: 1px solid #c2c2c2;'>" + countf + "</td><td style='border-bottom: 1px solid #c2c2c2;'>" + h.Gender + h.EmpName + "</td><td style='border-bottom: 1px solid #c2c2c2;'>" + h.OriginPlace.ToUpper() + "-" + h.DestinationPlace.ToUpper() + "</td><td style='border-bottom: 1px solid #c2c2c2;'>" + h.DepartureDate.ToString("dd/MM/yyyy") + "</td><td>" + h.DepartureTime + "</td><td>" + aircraftName + "</td><td>" + fltNo + "</td></tr>";
            }
            strContent = strContent.Replace("[tr]", tr);
            strContent = strContent.Replace("[trh]", trh);
            strContent = strContent.Replace("[trp]", trp);
            strContent = strContent.Replace("[trf]", trf);
            return strContent.ToString();
        }

        /// <summary>
        /// Initialized Email Credentials to Property
        /// </summary>
        /// <returns></returns>
        public EmailNotificationModel EmailCredentialsIthReq(String subjectName, String emailTemplateName, List<OATTravelRequestPassengerDetailModal> passengerList, List<OATTravelRequestFlightDetailModal> flightList, ITHVenderModal ithDetaillst)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtpUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtpPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtpServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtpPort"].Trim(),
                TemplateFilePath = ReadFileForIthReq(emailTemplateName, passengerList, flightList, ithDetaillst),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }

        private string ReadFileForIthReq(string emailTemplateName, List<OATTravelRequestPassengerDetailModal> passengerList, List<OATTravelRequestFlightDetailModal> flightList, ITHVenderModal ithDetaillst)
        {
            var strContent = new StringBuilder();
            string line;
            var file = new System.IO.StreamReader(
                       System.Web.HttpContext.Current.Server.MapPath("~/Template/OAT/" + emailTemplateName));
            while ((line = file.ReadLine()) != null)
            {
                strContent.Append(line);
            }
            file.Close();

            strContent = strContent.Replace("[IthName]", ithDetaillst.ITHName);
            strContent = strContent.Replace("[OatReqId]", passengerList[0].OATTravelRequestId.ToString());
            strContent = strContent.Replace("[pinfo]", "Passenger Information(s)");
            var count = 1;
            var tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>S. No</td><td>Passenger Name</td><td>Designation</td><td>Department</td><td>Phone No.</td><td>Email ID</td></tr>";
            foreach (var i in passengerList)
            {
                tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td style='border: 1px solid #c2c2c2;'>" + count++ + "</td><td style='border: 1px solid #c2c2c2;'>" + i.Gender + " " + i.FirstName + " " + i.LastName + "</td><td style='border: 1px solid #c2c2c2;'>" + i.Designation + "</td><td style='border: 1px solid #c2c2c2;'>" + i.Department + "</td><td style='border: 1px solid #c2c2c2;'>" + i.PhoneNo + "</td><td style='border: 1px solid #c2c2c2;'>" + i.EmailId + "</td></tr>";
            }

            var countP = 1;
            var trp = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>S. No</td><td>Emp Name</td><td>Sector</td><td>Travel Date</td><td>Flight No.</td><td>Airline Name </td><td>Departure Time</td></tr>";
            foreach (var j in flightList)
            {
                var fltNo = j.FlightNumber == null ? "-" : j.FlightNumber;
                var aircraftName = j.AirCraftName == null ? "-" : j.AirCraftName;
                trp = trp + "<tr style='font-family:Arial; font-size:12px;'><td style='border: 1px solid #c2c2c2;'>" + countP++ + "</td><td style='border: 1px solid #c2c2c2;'>" + j.EmpName + "</td><td style='border: 1px solid #c2c2c2;'>" + j.OriginPlace + "-" + j.DestinationPlace + "</td><td style='border: 1px solid #c2c2c2;'>" + j.DepartureDate.ToString("dd/MMM/yyyy") + "</td><td style='border: 1px solid #c2c2c2;'>" + fltNo + "</td><td style='border: 1px solid #c2c2c2;'>" + aircraftName + "</td><td style='border: 1px solid #c2c2c2;'>" + j.DepartureTime + "</td></tr>";
            }

            strContent = strContent.Replace("[tr]", tr);
            strContent = strContent.Replace("[trp]", trp);
            strContent = strContent.Replace("[finfo]", "Flight Information(s)");

            return strContent.ToString();
        }

        public void sendEmailNotification()
        {
            try
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
                    EmailNotifications.SendBookingRequestNotificationTo_HOD(emaildataHod, emailidHod);
                }
            }
            catch (Exception ex)
            {
                TempData["fun3"] = ex.InnerException;
            }
        }
        public void ExportListNoShow()
        {
            var data = _oaTRepository.getFlightNoShowList();
            var arr = data.ToArray();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=NoShowList.xls");
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            WriteTsv(arr, Response.Output);
            Response.End();
        }
        public void WriteTsv<T>(IEnumerable<T> data, TextWriter output)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));

            foreach (PropertyDescriptor prop in props)
            {
                output.Write(prop.DisplayName); // header
                output.Write("\t");
            }
            output.WriteLine();
            foreach (T item in data)
            {
                foreach (PropertyDescriptor prop in props)
                {
                    output.Write(prop.Converter.ConvertToString(
                         prop.GetValue(item)));
                    output.Write("\t");
                }
                output.WriteLine();
            }
        }

        /// <summary>
        /// Get FlightNo Show Report
        /// </summary>
        /// <returns></returns>
        public ActionResult  NoShowReport()
        {
            //Var FromDate toDate
            //var dic = new Dictionary<String, object>();
            //var dic = _oaTRepository.getIthDetailAsPerReqID(Convert.ToInt64(OatReqID));
            //return Json("", JsonRequestBehavior.AllowGet);
            return View();
        }

        public JsonResult getListOfOatFlightNoShow()
        {
            var oatflightNoShowList = _oaTRepository.getFlightNoShowList();
            return Json(oatflightNoShowList, JsonRequestBehavior.AllowGet);
        }
        protected override void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            string methodName = new StackTrace(ex).GetFrame(0).GetMethod().Name.ToString();
            int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
            string MathodNameWithLineNo = methodName;// + "in error on the line no" + line;
            Logging.ErrorLog.Instance.AddDBLogging(ex, methodName.ToString(), "Controllers/OATDeskControler.cs");
        }
       
        
    }
}