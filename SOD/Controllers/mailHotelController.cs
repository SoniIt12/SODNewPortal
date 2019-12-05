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
    public class mailHotelController : Controller, IExceptionFilter
    {

        private readonly IHotelApproverRepository _hotelApproverRepository;
        private readonly ISodApproverRepositorty _sodApproverRepository;
        private readonly ITransportRepository _transportRepository;
        private readonly IBulkUploadRepository _bulkUploadRepository;

        public mailHotelController()
        {
            _hotelApproverRepository = new HotelApproverRepository(new SodEntities());
            _sodApproverRepository = new SodApproverRepositorty(new SodEntities());
            _transportRepository = new TransportRepository(new SodEntities());
            _bulkUploadRepository = new BulkUploadRepository(new SodEntities());
        }

        // GET: To Manage query string data for various travel booking process
        public ActionResult Index()
        {
            var strQuery = string.Empty;
            var ghgj = Request.QueryString[0].ToString();
            var EncryptstrQuery = CipherURL.Decrypt(ghgj);            
            var type = (EncryptstrQuery  != "") ? EncryptstrQuery.Split('&')[1].Split('=')[1] :Request.QueryString[1].Trim();
            strQuery = Request.QueryString[0].ToString().Split(',')[0];
            var confirmNo = string.Empty;
            var hotelname = string.Empty;

            if (type.Equals("a"))
            {
                confirmNo = Request.QueryString[2].ToString();
                hotelname = Request.QueryString[3].ToString();
                Session["confirmNo"] = confirmNo;
                TempData["jsonmsg"] = ApproveHotelBookingRequest(strQuery, confirmNo, hotelname);
            }
            else if (type.Equals("r"))
            {
                hotelname = Request.QueryString[3].ToString();
                TempData["jsonmsg"] = RejectHotelBookingRequest(strQuery, hotelname);
            }
            else if (type.Equals("b"))
            {
                confirmNo = Request.QueryString[2].ToString();
                hotelname = Request.QueryString[3].ToString();
                Session["confirmNo"] = confirmNo;
                TempData["jsonmsg"] = ApproveHotelBookingRequestOat(strQuery, confirmNo, hotelname);
            }
            else if (type.Equals("c"))
            {
                hotelname = Request.QueryString[3].ToString();
                TempData["jsonmsg"] = RejectHotelBookingRequestOat(strQuery, hotelname);
            }
            else if (type.Equals("y"))
            {
                confirmNo = Request.QueryString[2].ToString();
                Session["confirmNo"] = confirmNo;
                TempData["jsonmsg"] = ApproveHotelBookingRequestBulk(strQuery, confirmNo);
            }
            else if (type.Equals("n"))
            {
                TempData["jsonmsg"] = RejectHotelBookingRequestBulk(strQuery);
            }
            else if (type.Equals("K"))
            {
                var EmpCode = string.Empty;
                EmpCode = EncryptstrQuery.Split('&')[0];
                TempData["EmpCode"] = EmpCode;
                var trid = EncryptstrQuery.Split('&')[2].Split('=')[1];
                confirmNo = EncryptstrQuery.Split('&')[4].Split('=')[1];
                Session["confirmNo"] = confirmNo;
                TempData["SenderEmpCode"] = EncryptstrQuery.Split('&')[5].Split('=')[1];
                TempData["jsonmsg"] = GetCancellationNotificationDataBulk(EmpCode, trid);
            }
            return View("Index");
        }

        public ActionResult IndexHotelCode()
        {
            var strQuery = string.Empty;
            strQuery = Request.QueryString[0].ToString();
            strQuery = CipherURL.Decrypt(strQuery);
            var dsjfh = strQuery.Split('&')[2].Split('=')[1];
            ViewBag.trid = dsjfh;
            ViewBag.clubId = dsjfh;
            ViewBag.type = strQuery.Split('&')[1].Split('=')[1];
            ViewBag.hotelname = strQuery.Split('&')[3].Split('=')[1];
            return View("IndexHotelCode");
        }

        public ActionResult hotelconfirmationresponse()
        {
            return View("hotelconfirmationresponse");
        }

        public ActionResult HotelResponsive()
        {
            var strQuery = string.Empty;
            strQuery = Request.QueryString[0].ToString();
            strQuery = CipherURL.Decrypt(strQuery);
            return View("HotelResponsive");
        }

        public ActionResult UserResponsive()
        {
            return View("UserResponsive");
        }

        public ActionResult UserCancellation()
        {
            return View("UserCancellation");
        }

        public ActionResult HodApproval()
        {
            var travelReqID = string.Empty;
            var hid = string.Empty;
            var approverEmpcCode = string.Empty;
            var strQuery = string.Empty;
            strQuery = Request.QueryString[0].ToString();
            strQuery = CipherURL.Decrypt(strQuery);
            if (strQuery.Split('&')[1].Split('=')[1].Equals("a"))
            {
                travelReqID = strQuery.Split('&')[2];
                approverEmpcCode = strQuery.Split('&')[3];
                hid = strQuery.Split('&')[4];
                TempData["jsonmsg"] = ApproveFinancialHotelBookingRequest(travelReqID, hid, approverEmpcCode, "a");
            }
            else if (strQuery.Split('&')[1].Split('=')[1].Equals("r"))
            {
                travelReqID = strQuery.Split('&')[2];
                approverEmpcCode = strQuery.Split('&')[3];
                hid = strQuery.Split('&')[4];
                TempData["jsonmsg"] = RejectFinancialHotelBookingRequest(travelReqID, hid, approverEmpcCode, "r");
            }
            else if (strQuery.Split('&')[1].Split('=')[1].Equals("b"))
            {
                travelReqID = strQuery.Split('&')[2];
                approverEmpcCode = strQuery.Split('&')[3];
                hid = strQuery.Split('&')[4];
                TempData["jsonmsg"] = ApproveFinancialHotelBookingRequest(travelReqID, hid, approverEmpcCode, "b");
            }
            else if (strQuery.Split('&')[1].Split('=')[1].Equals("c"))
            {
                travelReqID = strQuery.Split('&')[2];
                approverEmpcCode = strQuery.Split('&')[3];
                hid = strQuery.Split('&')[4];
                TempData["jsonmsg"] = RejectFinancialHotelBookingRequest(travelReqID, hid, approverEmpcCode, "c");
            }
            if (strQuery.Split('&')[1].Split('=')[1].Equals("Ba"))
            {
                travelReqID = strQuery.Split('&')[2];
                approverEmpcCode = strQuery.Split('&')[3];
                hid = strQuery.Split('&')[4];
                var clubid = Request.QueryString[5];
                TempData["jsonmsg"] = BulkApproveFinancialHotelBookingRequest(travelReqID, hid, Convert.ToInt32(clubid), approverEmpcCode, "a");
            }
            else if (strQuery.Split('&')[1].Split('=')[1].Equals("Br"))
            {
                travelReqID = strQuery.Split('&')[2];
                approverEmpcCode = strQuery.Split('&')[3];
                hid = strQuery.Split('&')[4];
                var clubid = strQuery.Split('&')[5];
                TempData["jsonmsg"] = BulkRejectFinancialHotelBookingRequest(travelReqID, hid, Convert.ToInt32(clubid), approverEmpcCode, "r");
            }
            return View("HodApproval");
        }


        /// <summary>
        /// Approve Booking Request for HOD
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        public string ApproveHotelBookingRequest(string clubid, string confirmationNo, string hotelname)
        {
            var jsonmsg = string.Empty;
            var smstext = string.Empty;
            var ClubbedEmpDetails = new List<TravelRequestMasterModels>();
            var s = _hotelApproverRepository.ApproveHotelBookingRequest(clubid, confirmationNo, hotelname, "SOD");
            var clubList = _transportRepository.SendMailToUsers(clubid);
            for (var i = 0; i < clubList.Count; i++)
            {
                var rqId = clubList[i].TravelRequestId.ToString();
                var hId = clubList[i].HotelRequestId;
                var dicList = new Dictionary<string, object>();
                dicList = _transportRepository.GetSodHotelInfo(Convert.ToInt64(rqId), hId);
                var empDetails = dicList["bookingInfolist"] as List<TravelRequestMasterModels>;

                for (var j = 0; j < clubList.Count; j++)
                {
                    if (i != j)
                    {
                        if (clubList[i].clubId == clubList[j].clubId)
                        {
                            var sharingReqId = clubList[j].TravelRequestId.ToString();
                            var sharingHid = clubList[j].HotelRequestId;
                            var sharedDicList = new Dictionary<string, object>();
                            sharedDicList = _transportRepository.GetSodHotelInfo(Convert.ToInt64(sharingReqId), sharingHid);
                            ClubbedEmpDetails = sharedDicList["bookingInfolist"] as List<TravelRequestMasterModels>;
                            break;
                        }
                    }
                }
                if (ClubbedEmpDetails.Count > 0)
                {
                    smstext = ConfigurationManager.AppSettings["smsBulkHotelConfirmation"].ToString().Replace("@TravellerName", empDetails[0].Title + " " + empDetails[0].RequestedEmpName)
                    .Replace("@HotelName", clubList[i].HotelName)
                    .Replace("@HotelAddress", clubList[i].HotelAddress)
                    .Replace("@Checkin", Convert.ToDateTime(clubList[i].CheckInDate).ToString("dd/MM/yyyy"))
                    .Replace("@Checkout", Convert.ToDateTime(clubList[i].CheckOutDate).ToString("dd/MM/yyyy"))
                    .Replace("@HotelConfirmationNo", clubList[i].HotelConfirmationNo)
                    .Replace("@ContactPersonMobileNo", clubList[i].HotelPhoneNo)
                    .Replace("@EmailID", clubList[i].PrimaryEmail)
                    .Replace("@ClubbingPersonName", ClubbedEmpDetails[0].Title + " " + ClubbedEmpDetails[0].RequestedEmpName);
                }
                else
                {
                    smstext = ConfigurationManager.AppSettings["smsBulkHotelConfirmation"].ToString().Replace("@TravellerName", empDetails[0].Title + " " + empDetails[0].RequestedEmpName)
                    .Replace("@HotelName", clubList[i].HotelName)
                    .Replace("@HotelAddress", clubList[i].HotelAddress)
                     .Replace("@Checkin", Convert.ToDateTime(clubList[i].CheckInDate).ToString("dd/MM/yyyy"))
                    .Replace("@Checkout", Convert.ToDateTime(clubList[i].CheckOutDate).ToString("dd/MM/yyyy"))
                    .Replace("@HotelConfirmationNo", clubList[i].HotelConfirmationNo)
                    .Replace("@ContactPersonMobileNo", clubList[i].HotelPhoneNo)
                    .Replace("@EmailID", clubList[i].PrimaryEmail)
                    .Replace("Clubbing with:@ClubbingPersonName", "");
                }
                // SMS Notifications
                SmsNotification.SmsNotifications.SendSmsViaApi(smstext, empDetails[0].Phno);
                //send mail to all users after approval
                GetHotelApprovalNotificationData(rqId, hId);
            }
            return (s >= 1 ? jsonmsg : string.Empty);
        }


        public string ApproveHotelBookingRequestOat(string clubid, string confirmationNo, string hotelname)
        {
            var jsonmsg = string.Empty;
            var s = _hotelApproverRepository.ApproveHotelBookingRequest(clubid, confirmationNo, hotelname, "OAT");
            return (s >= 1 ? jsonmsg : string.Empty);
        }

        /// <summary>
        /// Reject Booking Request
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        public string RejectHotelBookingRequest(string clubid, string hotelname)
        {
            var jsonmsg = string.Empty;
            var s = _hotelApproverRepository.RejectHotelBookingRequest(clubid, hotelname, "SOD");
            if (s >= 1)
                jsonmsg = "Rejection Alert : Request has been rejected successfully.";
            return (s >= 1 ? jsonmsg : string.Empty);
        }


        /// <summary>
        /// For OAT Rejection Request
        /// </summary>
        /// <param name="clubid"></param>
        /// <param name="hotelname"></param>
        /// <returns></returns>
        public string RejectHotelBookingRequestOat(string clubid, string hotelname)
        {
            var jsonmsg = string.Empty;
            var s = _hotelApproverRepository.RejectHotelBookingRequest(clubid, hotelname, "OAT");
            if (s >= 1)
            {
                jsonmsg = "Rejection Alert : Request has been rejected successfully.";
            }
            return (s >= 1 ? jsonmsg : string.Empty);
        }

        /// <summary>
        /// Approve Hotel Booking Request Bulk
        /// </summary>
        /// <param name="clubid"></param>
        /// <param name="confirmationNo"></param>
        /// <returns></returns>
        public string ApproveHotelBookingRequestBulk(string clubid, string confirmationNo)
        {
            var jsonmsg = string.Empty;
            var empList = new List<string>();
            var s = _hotelApproverRepository.ApproveHotelBookingRequestBulk(clubid, confirmationNo);
            //send mail to all users after approval
            var clubList = _bulkUploadRepository.FindListByClubId(clubid);
            for (var i = 0; i < clubList.Count; i++)
            {
                var clubListEmpCode = string.Empty;
                var clubListDetails = new List<EmdmAPIModels>();
                for (var j = 0; j < clubList.Count; j++)
                {
                    if (i != j)
                    {
                        if (clubList[i].sharingId == clubList[j].sharingId)
                        {
                            clubListEmpCode = clubList[j].EmployeeCode.Trim();
                            clubListDetails = _bulkUploadRepository.fetchEmpdetails(clubListEmpCode);
                            break;
                        }
                    }
                }
                var employeecode = clubList[i].EmployeeCode.Trim();
                var empDetails = _bulkUploadRepository.fetchEmpdetails(employeecode);
                var text = ConfigurationManager.AppSettings["smsBulkHotelConfirmation"].ToString();
                if (clubListDetails.Count > 0)
                {
                    text = ConfigurationManager.AppSettings["smsBulkHotelConfirmation"].ToString().Replace("@TravellerName", empDetails[0].FirstName + " " + empDetails[0].LastName)
                    .Replace("@HotelName", clubList[i].HotelName)
                    .Replace("@HotelAddress", clubList[i].HotelAddress)
                    .Replace("@Checkin", Convert.ToDateTime(clubList[i].CheckInDate).ToString("dd/MM/yyyy"))
                    .Replace("@Checkout", Convert.ToDateTime(clubList[i].CheckOutDate).ToString("dd/MM/yyyy"))
                    .Replace("@HotelConfirmationNo", clubList[i].HotelConfirmationNo)
                    .Replace("@ContactPersonMobileNo", clubList[i].HotelPhoneNo)
                    .Replace("@EmailID", clubList[i].PrimaryEmail)
                    .Replace("@ClubbingPersonName", clubListDetails[0].FirstName + " " + clubListDetails[0].LastName);
                }
                else
                {
                    text = ConfigurationManager.AppSettings["smsBulkHotelConfirmation"].ToString().Replace("@TravellerName", empDetails[0].FirstName + " " + empDetails[0].LastName)
                    .Replace("@HotelName", clubList[i].HotelName)
                    .Replace("@HotelAddress", clubList[i].HotelAddress)
                     .Replace("@Checkin", Convert.ToDateTime(clubList[i].CheckInDate).ToString("dd/MM/yyyy"))
                    .Replace("@Checkout", Convert.ToDateTime(clubList[i].CheckOutDate).ToString("dd/MM/yyyy"))
                    .Replace("@HotelConfirmationNo", clubList[i].HotelConfirmationNo)
                    .Replace("@ContactPersonMobileNo", clubList[i].HotelPhoneNo)
                    .Replace("@EmailID", clubList[i].PrimaryEmail)
                    .Replace("Clubbing with:@ClubbingPersonName", "");
                }
                //SMS Notifications
                // SmsNotification.SmsNotifications.SendSmsViaApi(text, empDetails[0].Phone);
                var TravelRequestId = clubList[i].TravelRequestId.ToString();
                s = _bulkUploadRepository.ApprovedHotelRequest(employeecode, TravelRequestId);
                GetHotelApprovalNotificationDataBulk(employeecode, TravelRequestId);
                empList.Add(employeecode);
            }
            consolidatedHotelApprovalNotificationDataBulk(clubid, clubList[0].TravelRequestId.ToString(), empList);
            return (s >= 1 ? jsonmsg : string.Empty);
        }

        /// <summary>		
        /// consolidated Hotel Approval Notification Data Bulk		
        /// </summary>		
        /// <param name="clubId"></param>		
        /// <param name="travelrequestid"></param>		
        /// <param name="empList">Emp List</param>		
        public void consolidatedHotelApprovalNotificationDataBulk(string clubId, string travelrequestid, List<string> empList)
        {
            var hotelList = new List<BulkTravelRequestHotelDetailModels>();
            var userList = new List<BulkUploadModels>();
            try
            {
                var spocEmpCode = new List<BulkUploadMasterModels>();
                hotelList = _bulkUploadRepository.FindListByClubId(clubId);
                userList = _bulkUploadRepository.GetSharedUserDetailsNew(Int32.Parse(clubId), Int64.Parse(travelrequestid));
                spocEmpCode = _bulkUploadRepository.FindSpocDetails(travelrequestid);
                var spocEmp = spocEmpCode[0].CreatedById.Substring(2);
                List<EmdmAPIModels> spocDetails = _bulkUploadRepository.fetchEmpdetails(spocEmp);
                //Send Email Notification		
                if (hotelList != null)
                {
                    var emailSubject = "SOD Hotel Booking Confirmation Notification :" + System.DateTime.Now.ToString();
                    var emailTemplateName = "SodHotelBookingRequestConfirmationNotificationTemplateFor_Hotel.html";
                    var emailCredentials = EmailCredentialsHotelBulkCCTravelDesk(emailSubject, emailTemplateName, hotelList, userList, "approved", spocDetails);
                    TempData["emailData"] = emailCredentials;
                    TempData["emailId"] = spocDetails[0].Email.Trim();
                    SendEmailNotificationCC_TravelDesk();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string RejectHotelBookingRequestBulk(string clubid)
        {
            var jsonmsg = string.Empty;
            var s = _hotelApproverRepository.RejectHotelBookingRequestBulk(clubid);
            if (s >= 1)
            {
                jsonmsg = "Rejection Alert : Request has been rejected successfully.";
            }
            return (s >= 1 ? jsonmsg : string.Empty);
        }

        /// <summary>
        /// Approve non contractual financial hotel request
        /// </summary>
        /// <param name="travelReqID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string ApproveFinancialHotelBookingRequest(string travelReqID, string hid, string approverEmpcCode, string type)
        {
            var jsonmsg = string.Empty;
            var s = 0;
            if (type == "a")
            {
                s = _hotelApproverRepository.ApproveNonContractualHotelRequest(travelReqID, hid, approverEmpcCode, type);
            }
            else
            {
                s = _hotelApproverRepository.ApproveNonContractualHotelRequestOat(travelReqID, approverEmpcCode, type);
            }
            if (s >= 1)
            {
                jsonmsg = "Approval Alert : \n Approval process has been completed successfully.";
            }
            if (s == -1)
            {
                jsonmsg = "Alert : The request has been already Approved.";
            }
            return (s == 0 ? "Alert: The request has been already Rejected." : jsonmsg);
        }

        /// <summary>
        /// Reject non contractual financial hotel request
        /// </summary>
        /// <param name="travelReqID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string RejectFinancialHotelBookingRequest(string travelReqID, string hid, string approverEmpcCode, string type)
        {
            var jsonmsg = string.Empty;
            var s = 0;
            if (type == "r")
                s = _hotelApproverRepository.RejectNonContractualHotelRequest(travelReqID, hid, approverEmpcCode, type);
            else
                s = _hotelApproverRepository.RejectNonContractualHotelRequestOat(travelReqID, approverEmpcCode, type);
            if (s >= 1)
                jsonmsg = "Rejection Alert : Request has been rejected successfully.";
            if (s == -1)
            {
                jsonmsg = "Alert : The request has been already Rejected.";
            }
            return (s == 0 ? "Alert: The request has been already Approved." : jsonmsg);
        }


        /// <summary>
        /// Initialized Email Credentials to Property
        /// </summary>
        /// <returns></returns>
        public EmailNotificationModel EmailCredentials(string subjectName, string emailTemplateName, List<HotelRequestApprovalModel> sodRequestsList, List<TravelRequestMasterModels> pasList, string reqNo)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtpUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtpPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtpServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtpPort"].Trim(),
                TemplateFilePath = ReadFile(emailTemplateName, sodRequestsList, pasList, reqNo),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }

        /// <summary>
        /// EmailCredentials for Oat
        /// </summary>
        /// <param name="subjectName"></param>
        /// <param name="emailTemplateName"></param>
        /// <param name="sodRequestsList"></param>
        /// <param name="pasList"></param>
        /// <param name="reqNo"></param>
        /// <returns></returns>
        public EmailNotificationModel EmailCredentialsOat(string subjectName, string emailTemplateName, List<HotelRequestApprovalOatModels> sodRequestsList, List<OALTravelRequestMasterModel> pasList, string reqNo)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtpUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtpPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtpServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtpPort"].Trim(),
                TemplateFilePath = ReadFileOat(emailTemplateName, sodRequestsList, pasList, reqNo),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }


        /// <summary>
        /// Read Template File from Location
        /// </summary>
        /// <returns></returns>
        private string ReadFile(string emailTemplateName, List<HotelRequestApprovalModel> sodRequestsList, List<TravelRequestMasterModels> pasList, string reqNo)
        {
            var strContent = new StringBuilder();
            string line;
            var file = new System.IO.StreamReader(
                       Server.MapPath("~/Template/Hotels/" + emailTemplateName));
            while ((line = file.ReadLine()) != null)
            {
                strContent.Append(line);
                //Replace code here..
            }
            file.Close();
            strContent = strContent.Replace("[RequesterName]", pasList[0].Title + " " + pasList[0].RequestedEmpName);
            var mltr = string.Empty;
            var c = 0;
            var tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Hotel Confirmation No.</td><td>Hotel Name</td><td>Hotel Address</td><td>Hotel Phone</td>";
            tr = tr + "</tr>";
            tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td>" + sodRequestsList[0].HotelConfirmationNo + "</td><td>" + sodRequestsList[0].HotelName + "</td><td>" + sodRequestsList[0].HotelAddress + "</td><td>" + sodRequestsList[0].HotelPhone + "</td>";
            tr = tr + "</tr>";
            strContent = strContent.Replace("[tr]", tr);
            strContent = strContent.Replace("[binfo]", "Hotel Information (Request ID : SOD-" + reqNo.Split(',')[0] + ")");
            return strContent.ToString();
        }


        private string ReadFileOat(string emailTemplateName, List<HotelRequestApprovalOatModels> sodRequestsList, List<OALTravelRequestMasterModel> pasList, string reqNo)
        {
            var strContent = new StringBuilder();
            string line;
            var file = new System.IO.StreamReader(
                       Server.MapPath("~/Template/Hotels/" + emailTemplateName));
            while ((line = file.ReadLine()) != null)
            {
                strContent.Append(line);
                //Replace code here..
            }
            file.Close();

            var title = "";
            var gender = pasList[0].Gender;
            if (gender == "M") { title = "Mr."; }
            else { title = "Ms."; }

            strContent = strContent.Replace("[RequesterName]", title + " " + pasList[0].RequestedEmpName);
            var mltr = string.Empty;
            var c = 0;

            var tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Hotel Confirmation No.</td><td>Hotel Name</td><td>Hotel Address</td><td>Hotel Phone</td>";
            tr = tr + "</tr>";
            tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td>" + sodRequestsList[0].HotelConfirmationNo + "</td><td>" + sodRequestsList[0].HotelName + "</td><td>" + sodRequestsList[0].HotelAddress + "</td><td>" + sodRequestsList[0].HotelPhone + "</td>";
            tr = tr + "</tr>";

            strContent = strContent.Replace("[tr]", tr);
            strContent = strContent.Replace("[binfo]", "Hotel Information (Request ID OAT: " + reqNo.Split(',')[0] + ")");

            return strContent.ToString();
        }


        /// <summary>
        /// To find confirmation no. of existing TravelRequestId
        /// </summary>
        /// <param name="TravelRequestId"></param>
        /// <returns></returns>
        public JsonResult FindExistingTrId(string TravelRequestId)
        {
            var trid = TravelRequestId.Split('|')[0];
            var type = TravelRequestId.Split('|')[1];
            var hotelname = TravelRequestId.Split('|')[2];
            var types = "";
            var dicList = new Dictionary<string, object>();

            if (type == "r" || type == "a")
            {
                types = "SOD";
                dicList = _hotelApproverRepository.FindExistingTrId(trid, types, hotelname);
                var rejectedlist = new List<HotelRequestRejectionModel>();
                rejectedlist = dicList["rejectedlist"] as List<HotelRequestRejectionModel>;
                if (rejectedlist.Count > 0)
                {
                    return Json("AlreadyRejected", JsonRequestBehavior.AllowGet);
                }

                var existingList = new List<TravelRequestHotelDetailModels>();
                existingList = dicList["existingList"] as List<TravelRequestHotelDetailModels>;
                if (existingList.Count > 0)
                {
                    if (type == "r")
                    {
                        return Json("AlreadyApproved", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ApproveHotelBookingRequest(trid, existingList[0].HotelConfirmationNo, hotelname);
                        //_hotelApproverRepository.ApproveHotelBookingRequest(trid, existingList[0].HotelConfirmationNo, hotelname, "SOD");
                        return Json(existingList[0].HotelConfirmationNo, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json("NotExists", JsonRequestBehavior.AllowGet);
                }
            }
            else if (type == "b" || type == "c")
            {
                types = "OAT";
                dicList = _hotelApproverRepository.FindExistingTrId(trid, types, hotelname);
                var existingList = new List<OALHotelModel>();
                existingList = dicList["existingList"] as List<OALHotelModel>;
                if (existingList.Count > 0)
                {
                    if (type == "c")
                    {
                        return Json("AlreadyApproved", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(existingList[0].HotelConfirmationNo, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json("NotExists", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                types = "BULK";
                dicList = _hotelApproverRepository.FindExistingTrId(trid, types, hotelname);
                var existingList = new List<BulkTravelRequestHotelDetailModels>();
                existingList = dicList["existingList"] as List<BulkTravelRequestHotelDetailModels>;
                if (existingList.Count == 0 || existingList[0].HotelConfirmationNo == null)
                {
                    return Json("NotExists", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if (type == "n")
                    {
                        return Json("AlreadyApproved", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(existingList[0].HotelConfirmationNo, JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }

        /// <summary>
        /// Update existing confirmation no.
        /// </summary>
        /// <param name="TravelRequestId"></param>
        /// <returns></returns>
        public JsonResult UpdateExistingCNo(string TravelRequestId)
        {
            var trid = TravelRequestId.Split('|')[0];
            var type = TravelRequestId.Split('|')[1];
            var newconfirmNo = TravelRequestId.Split('|')[2];
            var oldconfirmNo = TravelRequestId.Split('|')[3];
            var types = "";
            if (type == "r" || type == "a")
            {
                types = "SOD";
                var s = _hotelApproverRepository.UpdateExistingCNo(trid, types, newconfirmNo, oldconfirmNo);
                if (s >= 1)
                {

                    //send cc mail to traveldesk for updation in confirmation no.
                    var alist = _transportRepository.GetDetailsByClubId(trid);
                    var emailSubject2 = "Hotel Change in Confirmation No. Notification from Hotel :" + System.DateTime.Now.ToString();
                    var emailTemplateName2 = "HotelNotificationUpdateCNoFor_TravelDesk.html";
                    var emailCredentials2 = EmailCredentialsHotelUpdateConfNoTravelDesk(emailSubject2, emailTemplateName2, alist, newconfirmNo, oldconfirmNo);
                    TempData["emailData"] = emailCredentials2;
                    TempData["emailId"] = ConfigurationManager.AppSettings["TravelDeskEmailId"].Trim();
                    sendEmailNotification();
                    return Json("Updated", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Error", JsonRequestBehavior.AllowGet);
                }
            }
            else if (type == "b" || type == "c")
            {
                types = "OAT";
                var s = _hotelApproverRepository.UpdateExistingCNo(trid, types, newconfirmNo, oldconfirmNo);
                if (s >= 1)
                {
                    return Json("Updated", JsonRequestBehavior.AllowGet);
                    //send cc mail to traveldesk for updation in confirmation no.
                }
                else
                {
                    return Json("Error", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                types = "BULK";
                var s = _hotelApproverRepository.UpdateExistingCNo(trid, types, newconfirmNo, oldconfirmNo);
                if (s >= 1)
                {

                    //send cc mail to traveldesk for updation in confirmation no.
                    var blist = _bulkUploadRepository.FindListByClubId(trid);
                    var elist = new List<TravelRequestHotelDetailModels>();
                    foreach (var e in blist)
                    {
                        elist.Add(new TravelRequestHotelDetailModels
                        {
                            TravelRequestId = e.TravelRequestId,
                            EmployeeCode = e.EmployeeCode,
                            HotelName = e.HotelName,
                            City = e.City
                        });
                    }
                    var emailSubject2 = "Hotel Change in Confirmation No. Notification from Hotel :" + System.DateTime.Now.ToString();
                    var emailTemplateName2 = "HotelNotificationUpdateCNoFor_TravelDesk.html";
                    var emailCredentials2 = EmailCredentialsHotelUpdateConfNoTravelDesk(emailSubject2, emailTemplateName2, elist, newconfirmNo, oldconfirmNo);
                    TempData["emailData"] = emailCredentials2;
                    TempData["emailId"] = ConfigurationManager.AppSettings["TravelDeskEmailId"].Trim();
                    sendEmailNotification();
                    return Json("Updated", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Error", JsonRequestBehavior.AllowGet);
                }
            }
        }


        /// <summary>
        /// Update check-in and check-out time by hotel
        /// </summary>
        /// <param name="TravelRequestId"></param>
        /// <returns></returns>
        public JsonResult UpdateCheckinCheckout()
        {
            var clubid = Request.QueryString[0].ToString().Split(',')[0];
            var type = Request.QueryString[1].ToString();
            var checkin = Request.QueryString[2].ToString();
            var checkout = Request.QueryString[3].ToString();
            var hotelname = Request.QueryString[4].ToString();
            var types = "";
            var s = 0;
            if (type == "a")
            {
                s = _hotelApproverRepository.UpdateCheckinCheckout(clubid, "SOD", checkin, checkout, hotelname);
            }
            else
            {
                s = _hotelApproverRepository.UpdateCheckinCheckout(clubid, "OAT", checkin, checkout, hotelname);
            }
            if (s > 0)
            {
                return Json("Saved", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Update check-in and check-out time by user
        /// </summary>
        /// <param name="TravelRequestId"></param>
        /// <returns></returns>
        public JsonResult UpdateUserCheckinCheckout()
        {
            var trid = Request.QueryString[0].ToString();
            var checkin = Request.QueryString[1].ToString();
            var checkout = Request.QueryString[2].ToString();
            var hotelId = Request.QueryString[3].ToString();
            var s = "";

            s = _hotelApproverRepository.UpdateUserCheckinCheckout(trid, "SOD", checkin, checkout, hotelId);

            if (s == "Saved")
            {
                return Json("Saved", JsonRequestBehavior.AllowGet);
            }
            else if (s == "AlreadyUpdated")
            {
                return Json("AlreadyUpdated", JsonRequestBehavior.AllowGet);
            }
            else if (s == "Cancelled")
            {
                return Json("Cancelled", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// Get Hotel Approval Notification Data
        /// </summary>
        /// <param name="req"></param>
        /// <param name="hid"></param>
        public void GetHotelApprovalNotificationData(string req, int hid)
        {
            var trId = req;
            //Display Request wise view details
            var hotelList = new List<TravelRequestHotelDetailModels>();
            var sodflightList = new List<FlightDetailModels>();
            var passengerList = new List<PassengerDetailModels>();

            var dicList = new Dictionary<string, object>();
            dicList = _transportRepository.GetSodHotelInfo(Convert.ToInt64(trId), hid);

            var dicListSharedUser = new Dictionary<string, object>();
            dicListSharedUser = _transportRepository.GetHotelDetailbyTrID(Convert.ToInt64(trId), hid);
            TempData["sharedUserList"] = dicListSharedUser["sharedUserdetails"] as List<TravelRequestMasterModels>;

            sodflightList = dicList["flightInfolist"] as List<FlightDetailModels>;
            passengerList = dicList["passInfolist"] as List<PassengerDetailModels>;
            TempData["bookingInfo"] = dicList["bookingInfolist"] as List<TravelRequestMasterModels>;
            TempData["hotelList"] = dicList["hotelinfolist"] as List<TravelRequestHotelDetailModels>;

            //Send Email Notification to user and traveldesk 
            if (TempData["hotelList"] != null && TempData["bookingInfo"] != null)
            {
                var hotel_List = TempData["hotelList"] as List<TravelRequestHotelDetailModels>;
                var bookingInfo = TempData["bookingInfo"] as List<TravelRequestMasterModels>;

                var emailSubject = "SOD Hotel Booking Confirmation Notification :" + System.DateTime.Now.ToString();
                var emailTemplateName = "SodHotelBookingRequestNotificationTemplateFor_TravelDesk.html";
                var emailCredentials = EmailCredentialsHotel(emailSubject, emailTemplateName, hotel_List, bookingInfo, req, "approved");
                TempData["emailData"] = emailCredentials;
                TempData["emailId"] = bookingInfo[0].EmailId.Trim();
                SendEmailNotificationCC_TravelDesk();
            }
        }

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
        /// Send CC Email 
        /// </summary>
        public void SendEmailNotificationCC_TravelDesk()
        {
            if (TempData["emailData"] != null && TempData["emailId"] != null)
            {
                var emaildata = TempData["emailData"] as EmailNotificationModel;
                var emailid = TempData["emailId"].ToString();
                EmailNotifications.SendBookingRequestNotificationTo_Requester_Traveldesk(emaildata, emailid);
            }
        }

        /// <summary>
        /// Email Credentials Hotel
        /// </summary>
        /// <param name="subjectName"></param>
        /// <param name="emailTemplateName"></param>
        /// <param name="hoteldetails"></param>
        /// <param name="bookingInfo"></param>
        /// <param name="reqNo"></param>
        /// <param name="approvalStatus"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Email Credentials Hotel CC and TravelDesk Email 
        /// </summary>
        /// <param name="subjectName"></param>
        /// <param name="emailTemplateName"></param>
        /// <param name="hoteldetails"></param>
        /// <param name="bookingInfo"></param>
        /// <param name="reqNo"></param>
        /// <param name="approvalStatus"></param>
        /// <returns></returns>
        public EmailNotificationModel EmailCredentialsHotelCCTravelDesk(string subjectName, string emailTemplateName, List<TravelRequestHotelDetailModels> hoteldetails, List<TravelRequestMasterModels> bookingInfo, string reqNo, string approvalStatus)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtpHotelUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtptdPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtptdServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtptdPort"].Trim(),
                TemplateFilePath = ReadFileHotelCCTravelDesk(emailTemplateName, hoteldetails, bookingInfo, reqNo, approvalStatus),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }

        public EmailNotificationModel EmailCredentialsHotelUpdateConfNoTravelDesk(string subjectName, string emailTemplateName, List<TravelRequestHotelDetailModels> hoteldetails, string newConfirmationNo, string oldConfirmationNo)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtpHotelUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtptdPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtptdServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtptdPort"].Trim(),
                TemplateFilePath = ReadFileHotelConfNoTravelDesk(emailTemplateName, hoteldetails, newConfirmationNo, oldConfirmationNo),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }

        /// <summary>
        /// Read TEmplate Hotel File
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
                    tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Hotel Confirmation No.</td><td>City</td><td>Check-In Date</td><td>Check-Out Date</td><td>Hotel Information</td><td>No. of Guests</td></tr>";
                    tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td>" + hoteldetails[0].HotelConfirmationNo +
                        "</td><td>" + hotelcity + "</td><td>" + hoteldetails[0].CheckInDate.ToString("dd/MM/yyyy") + "</td><td>" + hoteldetails[0].CheckOutDate.ToString("dd/MM/yyyy") +
                        "</td><td>Name: " + hoteldetails[0].HotelName + "<br>Address: " + hoteldetails[0].HotelAddress + " <br>Phone No.: " + hoteldetails[0].HotelPhoneNo + "</td><td>"
                        + hoteldetails[0].NoOfGuest + "</td></tr>";
                }
                else
                {
                    tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Hotel Confirmation No.</td><td>City</td><td>Check-In Date</td><td>Check-Out Date</td><td>Hotel Information</td><td>No. of Guests</td><td>Sharing With</td></tr>";
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

        private string ReadFileHotelCCTravelDesk(string emailTemplateName, List<TravelRequestHotelDetailModels> hoteldetails, List<TravelRequestMasterModels> bookingInfo, string reqNo, string approvalStatus)
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

                var shareduserlist = TempData["sharedUserList"] as List<TravelRequestMasterModels>;
                if (shareduserlist == null || shareduserlist.Count < 1)
                {
                    tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Hotel Confirmation No.</td><td>City</td><td>Check-In Date</td><td>Check-Out Date</td><td>Hotel Information</td><td>No. of Guests</td></tr>";
                    tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td>" + hoteldetails[0].HotelConfirmationNo +
                        "</td><td>" + hotelcity + "</td><td>" + hoteldetails[0].CheckInDate.ToString("dd/MM/yyyy") + "</td><td>" + hoteldetails[0].CheckOutDate.ToString("dd/MM/yyyy") +
                        "</td><td>Name: " + hoteldetails[0].HotelName + "<br>Address: " + hoteldetails[0].HotelAddress + " <br>Phone No.: " + hoteldetails[0].HotelPhoneNo + "</td><td>"
                        + hoteldetails[0].NoOfGuest + "</td></tr>";
                }
                else
                {
                    tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Hotel Confirmation No.</td><td>City</td><td>Check-In Date</td><td>Check-Out Date</td><td>Hotel Information</td><td>No. of Guests</td><td>Sharing With</td></tr>";
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

        private string ReadFileHotelConfNoTravelDesk(string emailTemplateName, List<TravelRequestHotelDetailModels> hoteldetails, string newConfirmationNo, string oldConfirmationNo)
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

            strContent = strContent.Replace("[binfo]", " Hotel Details ");

            var tr = "";
            tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Hotel Name</td><td>City Code</td><td>Old Confirmation No.</td><td>New Confirmation No.</td></tr>";
            tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td>" + hoteldetails[0].HotelName +
                "</td><td>" + hoteldetails[0].City + "</td><td>" + oldConfirmationNo + "</td><td>" + newConfirmationNo + "</td></tr>";

            strContent = strContent.Replace("[hinfo]", "This booking was against following Travel Request Id(s):");

            var trI = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Travel Request Id</td><td>EmpCode</td></tr>";

            foreach (var i in hoteldetails)
            {
                trI = trI + "<tr style='font-family:Arial; font-size:12px;'><td>" + i.TravelRequestId + "</td><td>" + i.EmployeeCode + "</td>" + "</tr>";
            }
            strContent = strContent.Replace("[tr]", tr);
            strContent = strContent.Replace("[trI]", trI);
            return strContent.ToString();
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
        /// Get sender Bulk User Info
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        public List<string> GetSenderUserInfo(string empId)
        {
            var s = SOD.Services.ADO.SodCommonServices.GetEmployeeCommonDetails(int.Parse(empId));
            var empInfo = new List<string> { s[0].EmpId.ToString().Trim(), s[0].EmpName.Trim(), s[0].Gender.Trim(), s[0].Email.Trim() };
            return empInfo;
        }


        /// <summary>
        /// get hotel info for user mail
        /// </summary>
        /// <param name="empcode"></param>
        /// <param name="travelrequestid"></param>
        public void GetHotelApprovalNotificationDataBulk(string empcode, string travelrequestid)
        {
            var hotelList = new List<BulkTravelRequestHotelDetailModels>();
            var userList = new List<BulkUploadModels>();
            hotelList = _bulkUploadRepository.GetHotelDetailForMail(empcode, travelrequestid);
            userList = _bulkUploadRepository.GetUserDetailForMail(empcode, travelrequestid);

            var sharedUserList = new List<BulkUploadModels>();
            sharedUserList = _bulkUploadRepository.GetSharedUserDetails(hotelList[0].sharingId, hotelList[0].clubId, empcode, travelrequestid);

            //Send Email Notification
            if (hotelList != null)
            {
                var emailSubject = "SOD Hotel Booking Confirmation Notification :" + System.DateTime.Now.ToString();
                var emailTemplateName = "SodHotelBookingRequestConfirmationNotificationTemplateFor_Hotel.html";
                var emailCredentials = EmailCredentialsHotelBulk(emailSubject, emailTemplateName, hotelList, userList, sharedUserList, "approved");
                TempData["emailData"] = emailCredentials;
                TempData["emailId"] = userList[0].EmailId.Trim();
                sendEmailNotification();
            }
        }




        /// <summary>
        /// Get Cancellation Notification Data Bulk
        /// </summary>
        /// <param name="empcode"></param>
        /// <param name="trid"></param>
        /// <returns></returns>
        public string GetCancellationNotificationDataBulk(string empcode, string trid)
        {
            var jsonmsg = string.Empty;
            var hotelList = new List<BulkTravelRequestHotelDetailModels>();
            var userList = new List<BulkUploadModels>();
            hotelList = _bulkUploadRepository.GetHotelDetailForMail(empcode, trid);
            userList = _bulkUploadRepository.GetUserDetailForMail(empcode, trid);
            var sharedUserList = new List<BulkUploadModels>();
            sharedUserList = _bulkUploadRepository.GetSharedUserDetails(hotelList[0].sharingId, hotelList[0].clubId, empcode, trid);
            if (hotelList != null)
            {
                if (hotelList[0].HotelStatus == "Cancellation Confirm")
                {
                    jsonmsg = "cancellation Confirmation mail has been already sent to spicejet TravelDesk.";
                }
                else
                {
                    var emailSubject = "SOD Hotel Cancellation Request Notification from Hotel/TravelDsk:" + System.DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt");
                    var emailTemplateName = "HotelBookingRequestNotificationTemplate_Cancellation.html";
                    var emailCredentials = EmailCredentialsHotelRequestOnCancel(emailSubject, emailTemplateName, Convert.ToInt64(empcode), hotelList[0].clubId, hotelList[0].HotelName, sharedUserList);
                    var templateData = emailCredentials.TemplateFilePath;
                    //sender User Info
                    var SenderUserInfo = GetSenderUserInfo(TempData["SenderEmpCode"].ToString().Trim());
                    var reqName = SenderUserInfo[2] == "M" ? "Mr." : "Ms.";
                    reqName = reqName + " " + SenderUserInfo[1];

                    templateData = templateData.Replace("[hotelName]", reqName);
                    emailCredentials.TemplateFilePath = templateData;
                    TempData["emailData_Hod"] = emailCredentials;

                    TempData["emailId_Hod"] = SenderUserInfo[3];
                    EmailNotifications.SendBookingRequestNotificationTo_Requester_Traveldesk(emailCredentials, SenderUserInfo[3]);
                    //change hotelstatus approved by Hod to Cancellation Approved
                    _bulkUploadRepository.ChangeHotelStatus(empcode, trid);
                    jsonmsg = "Hotel Cancellaion Confirmation for Employee ID :-" + empcode + "Employee Name: -" + userList[0].FirstName + userList[0].LastName + "." + "Hotel Booking cancellation request has been sent successfully. The request has been sent to spicejet TravelDesk";

                }
            }
            else
            {
                jsonmsg = "Alert : There is something error in sending mail to user or TravelDesk.";
            }
            return jsonmsg;
        }
        public EmailNotificationModel EmailCredentialsHotelRequestOnCancel(string subjectName, string emailTemplateName, Int64 EmpCode, Int32 clubid, string hotelname, List<BulkUploadModels> sharedUserList)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtptdUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtptdPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtptdServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtptdPort"].Trim(),
                TemplateFilePath = ReadFileHotelRequestOnCancel(emailTemplateName, EmpCode, clubid, hotelname, sharedUserList),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }
        /// <summary>
        /// Template to send After cancellation approval from hotel
        /// </summary>
        /// <param name="emailTemplateName"></param>
        /// <param name="EmpCode"></param>
        /// <param name="clubid"></param>
        /// <param name="hotelname"></param>
        /// <param name="sharedUserList"></param>
        /// <returns></returns>
        private string ReadFileHotelRequestOnCancel(string emailTemplateName, Int64 EmpCode, Int32 clubid, string hotelname, List<BulkUploadModels> sharedUserList)
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

            GetBulkHotelRequestData(EmpCode, clubid, hotelname, 2);
            var passengerInfo = TempData["InfoForHotelRequest"] as List<TravelRequestModels>;
            var count = 1;
            var gender = "";
            var airtransport = "";

            var tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;font-weight:bold;'>" +
              "<td style='height:20px; padding-bottom:8px;'>S. No.</td>" +
              "<td style='height:20px; padding-bottom:8px;'>Gender</td>" +
              "<td style='height:20px; padding-bottom:8px;'>Employee Name</td>" +
              "<td style='height:20px; padding-bottom:8px;'>EmpId</td>" +
               "<td style='height:20px; padding-bottom:8px;'>Check-In Date</td>" +
               "<td style='height:20px; padding-bottom:8px;'>Check-Out Date</td>" +
               "<td style='height:20px; padding-bottom:8px;'>HeadOffice/Airport Transfers Required</td>" +
               "<td style='height:20px; padding-bottom:8px;'>Flight No.</td>" +
               "<td style='height:20px; padding-bottom:8px;'>Check-In Time</td>" +
               "<td style='height:20px; padding-bottom:8px;'>Mobile No.</td>";
            if (sharedUserList.Count > 0)
            {
                tr = tr + "<td height:20px; padding-bottom:8px;'>Sharing With</td></tr>";
            }
            else
            {
                tr = tr + "</tr>";
            }
            foreach (var p in passengerInfo)
            {
                if (p.Title == "Ms")
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

                tr = tr + "<tr style='font-family:Arial; font-size:12px;'>" +
                        "<td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + count +
                        "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + gender +
                        "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.RequestedEmpName +
                        "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.RequestedEmpCode +
                        "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.CheckinDate.ToString("dd-MMM-yyyy") +
                        "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.CheckoutDate.ToString("dd-MMM-yyyy") +
                        "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + airtransport +
                        "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.FlightNo +
                        "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.CheckinTime +
                        "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.Phone;

                if (sharedUserList.Count > 0)
                {
                    tr = tr + "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + "Employee Code: " + sharedUserList[0].EmpCode + "<br>Name: " + sharedUserList[0].FirstName + " " + sharedUserList[0].LastName + "<br>Phone No.: " + sharedUserList[0].MobileNo + "</td></tr>";
                }
                else
                {
                    tr = tr + "</td ></tr>";
                }

                count++;
            }

            strContent = strContent.Replace("[tr]", tr);

            return strContent.ToString();
        }
        /// <summary>
        /// get passenger details for hotel bulk booking
        /// </summary>
        /// <param name="requestList"></param>
        /// <param name="hotelname"></param>
        /// <param name="sodOat"></param>
        public void GetBulkHotelRequestData(Int64 travelRequestID, Int32 clubid, string hotelname, int criteria)
        {
            var dicList = new Dictionary<string, object>();
            dicList = _bulkUploadRepository.GetBulkHotelandUserInfo(travelRequestID, clubid, hotelname, criteria);
            TempData["InfoForHotelRequest"] = dicList["InfoForHotelRequest"] as List<TravelRequestModels>;
        }


        public EmailNotificationModel EmailCredentialsHotelBulk(string subjectName, string emailTemplateName, List<BulkTravelRequestHotelDetailModels> hoteldetails, List<BulkUploadModels> bookingInfo, List<BulkUploadModels> shareduserlist, string approvalStatus)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtpHotelUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtptdPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtptdServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtptdPort"].Trim(),
                TemplateFilePath = ReadFileHotelBulk(emailTemplateName, hoteldetails, bookingInfo, shareduserlist, approvalStatus),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }

        public EmailNotificationModel EmailCredentialsHotelBulkCCTravelDesk(string subjectName, string emailTemplateName, List<BulkTravelRequestHotelDetailModels> hoteldetails, List<BulkUploadModels> bookingInfo, string approvalStatus, List<EmdmAPIModels> spocDetails)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtpHotelUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtptdPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtptdServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtptdPort"].Trim(),
                TemplateFilePath = ReadFileHotelBulkCCTravelDesk(emailTemplateName, hoteldetails, bookingInfo, approvalStatus, spocDetails),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }

        /// <summary>
        /// Read File Hotel Bulk
        /// </summary>
        /// <param name="emailTemplateName"></param>
        /// <param name="hoteldetails"></param>
        /// <param name="bookingInfo"></param>
        /// <param name="shareduserlist"></param>
        /// <param name="approvalStatus"></param>
        /// <returns></returns>
        private string ReadFileHotelBulk(string emailTemplateName, List<BulkTravelRequestHotelDetailModels> hoteldetails, List<BulkUploadModels> bookingInfo, List<BulkUploadModels> shareduserlist, string approvalStatus)
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

            var shareid = hoteldetails[0].sharingId;
            var Title = bookingInfo[0].Title;
            var RequesterName = bookingInfo[0].FirstName + " " + bookingInfo[0].LastName;
            var HotelTitle = "Hotel Booking Details ";
            strContent = strContent.Replace("[RequesterName]", Title + " " + RequesterName);
            strContent = strContent.Replace("[binfo]", HotelTitle + "  (Booking Request Id : " + bookingInfo[0].TrnId + ")");
            strContent = strContent.Replace("[hotelstatus]", approvalStatus);

            var tr = "";
            if (approvalStatus == "approved")
            {

                if (shareduserlist.Count < 1)
                {
                    tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Hotel Confirmation No.</td><td>City</td><td>Check-In Date</td><td>Check-Out Date</td><td>Hotel Information</td><td>No. of Guests</td></tr>";
                    tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td>" + hoteldetails[0].HotelConfirmationNo +
                        "</td><td>" + hoteldetails[0].City + "</td><td>" + hoteldetails[0].CheckInDate.ToString("dd/MM/yyyy") + "</td><td>" + hoteldetails[0].CheckOutDate.ToString("dd/MM/yyyy") +
                        "</td><td>Name: " + hoteldetails[0].HotelName + "<br>Address: " + hoteldetails[0].HotelAddress + " <br>Phone No.: " + hoteldetails[0].HotelPhoneNo + "</td><td>"
                        + "1" + "</td></tr>";
                }
                else
                {
                    tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Hotel Confirmation No.</td><td>City</td><td>Check-In Date</td><td>Check-Out Date</td><td>Hotel Information</td><td>No. of Guests</td><td>Sharing With</td></tr>";
                    tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td>" + hoteldetails[0].HotelConfirmationNo +
                        "</td><td>" + hoteldetails[0].City + "</td><td>" + hoteldetails[0].CheckInDate.ToString("dd/MM/yyyy") + "</td><td>" + hoteldetails[0].CheckOutDate.ToString("dd/MM/yyyy") +
                        "</td><td>Name: " + hoteldetails[0].HotelName + "<br>Address: " + hoteldetails[0].HotelAddress + " <br>Phone No.: " + hoteldetails[0].HotelPhoneNo + "</td>" +
                        "<td>" + "2" + "</td>" +
                        "<td>Employee Code: " + shareduserlist[0].EmpCode + "<br>Name: " + shareduserlist[0].FirstName + " " + shareduserlist[0].LastName + "<br>Phone No.: " + shareduserlist[0].MobileNo + "</td>" +
                        "</tr>";
                }

                var cityCodeList = new List<SodCityCodeMasterModels>();
                cityCodeList = _transportRepository.FindCityCode(hoteldetails[0].City);

                var inclusionList = new List<HotelInclusionMasterModels>();
                inclusionList = findHotelInclusions(cityCodeList[0].CityCode, hoteldetails[0].HotelName);

                if (inclusionList.Count > 0)
                {
                    strContent = strContent.Replace("[hinfo]", "The hotel inclusions are as mentioned below:");
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
                strContent = strContent.Replace("[hinfo]", "");
                strContent = strContent.Replace("[trI]", "");
                tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Hotel Reference Id</td><td>City</td><td>Check-In Date</td><td>Check-Out Date</td><td>No. of Guests</td></tr>";
                tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td>" + hoteldetails[0].HotelReferenceID + "</td><td>" + hoteldetails[0].City + "</td><td>" + hoteldetails[0].CheckInDate.ToString("dd/MM/yyyy") + "</td><td>" + hoteldetails[0].CheckOutDate.ToString("dd/MM/yyyy") + "</td><td>" + hoteldetails[0].NoOfGuest + "</td></tr>";
            }
            strContent = strContent.Replace("[tr]", tr);
            return strContent.ToString();
        }

        private string ReadFileHotelBulkCCTravelDesk(string emailTemplateName, List<BulkTravelRequestHotelDetailModels> hoteldetails, List<BulkUploadModels> bookingInfo, string approvalStatus, List<EmdmAPIModels> spocDetails)
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
            if (spocDetails[0].Gender == "F")
            {
                Title = "Ms.";
            }
            else
            {
                Title = "Mr.";
            }
            var RequesterName = spocDetails[0].FirstName + " " + spocDetails[0].LastName;
            var HotelTitle = "Booking Details ";
            strContent = strContent.Replace("[RequesterName]", Title + " " + RequesterName);
            strContent = strContent.Replace("[binfo]", HotelTitle + "  (Employee Booking Request Id : " + bookingInfo[0].TrnId + ")");
            strContent = strContent.Replace("[hotelstatus]", approvalStatus);
            var tr = "";
            if (approvalStatus == "approved")
            {
                var count = 0;
                tr = tr + "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>S. No.</td><td>Title </td><td>EmpCode</td><td>Emp Name</td><td>Mobile No.</td><td>Email Id</td><td>Sharing Id</td></tr>";
                foreach (var list in bookingInfo)
                {
                    count++;
                    tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td>" + count +
                          "</td><td>" + list.Title + "</td><td>" + list.EmpCode + "</td><td>" + list.FirstName + " " + list.LastName +
                          "</td><td>" + list.MobileNo + "</td><td>"
                          + list.EmailId + "</td><td>" + list.sharingId + "</td></tr>";
                }
                strContent = strContent.Replace("[hinfo]", "Hotel Details: ");
                var trI = " ";
                trI = trI + "<table cellpadding='0' cellspacing='0' style='width:100%; font-size:12px;border:solid 1px #c2c2c2;line-height:30px;'><tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Hotel Confirmation No.</td><td>City</td><td>Check-In Date</td><td>Check-Out Date</td><td>Hotel Information</td><td>No. of Guests</td></tr>";
                trI = trI + "<tr style='font-family:Arial; font-size:12px;'><td>" + hoteldetails[0].HotelConfirmationNo +
                        "</td><td>" + hoteldetails[0].City + "</td><td>" + hoteldetails[0].CheckInDate.ToString("dd/MM/yyyy") + "</td><td>" + hoteldetails[0].CheckOutDate.ToString("dd/MM/yyyy") +
                        "</td><td>Name: " + hoteldetails[0].HotelName + "<br>Address: " + hoteldetails[0].HotelAddress + " <br>Phone No.: " + hoteldetails[0].HotelPhoneNo + "</td><td>"
                        + count + "</td></tr></table>";

                var cityCodeList = new List<SodCityCodeMasterModels>();
                cityCodeList = _transportRepository.FindCityCode(hoteldetails[0].City);
                var inclusionList = new List<HotelInclusionMasterModels>();
                inclusionList = findHotelInclusions(cityCodeList[0].CityCode, hoteldetails[0].HotelName);
                if (inclusionList.Count > 0)
                {

                    trI = trI + "<p style='font-size: 16px; font-family:Arial; margin-top: 10px;padding-left: 20px;color:fff;align-items:center'>Hotel inclusions are as mentioned below: </P>";
                    trI = trI + "<table cellpadding='0' cellspacing='0' style='width:100%; font-size:12px;border:solid 1px #c2c2c2;line-height:30px;'><tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Accommodation</td><td>Food</td><td>Airport Transfers</td><td>Room service</td><td>Buffet Time</td><td>Laundry</td></tr>";
                    trI = trI + "<tr style='font-family:Arial; font-size:12px;'><td>" + inclusionList[0].Accomodation +
                                "</td><td>" + inclusionList[0].Food + "</td><td>" + inclusionList[0].AirportTransfers +
                                "</td><td>" + inclusionList[0].RoomService + "</td>" +
                                "<td>" + inclusionList[0].BuffetTime + "</td>" +
                                "<td>" + inclusionList[0].Laundry + "</td>" +
                                "</tr></table>";
                }
                strContent = strContent.Replace("[trI]", trI);

            }
            else
            {
                strContent = strContent.Replace("[hinfo]", "");
                strContent = strContent.Replace("[trI]", "");
                tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Hotel Reference Id</td><td>City</td><td>Check-In Date</td><td>Check-Out Date</td><td>No. of Guests</td></tr>";
                tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td>" + hoteldetails[0].HotelReferenceID + "</td><td>" + hoteldetails[0].City + "</td><td>" + hoteldetails[0].CheckInDate.ToString("dd/MM/yyyy") + "</td><td>" + hoteldetails[0].CheckOutDate.ToString("dd/MM/yyyy") + "</td><td>" + hoteldetails[0].NoOfGuest + "</td></tr>";
            }
            strContent = strContent.Replace("[tr]", tr);
            return strContent.ToString();
        }


        /// <summary>
        /// send cancellation request to hotel
        /// </summary>
        /// <param name="travelRqstId"></param>
        [HttpPost]
        public JsonResult hotelCancellationRequest(string travelRqstId)
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
                if (hoteldetails.Count > 0)
                {
                    hotelname = hoteldetails[0].HotelName;
                    primaryEmail = hoteldetails[0].PrimaryEmail;
                }
            }

            var s = _transportRepository.SaveCancellationRequest(travReqstId, hid, sodOat);

            if (s > 0)
            {

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

                if (primaryEmail.Length > 0)
                {
                    //send cancellation request to hotel
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
                if (useremail.Length > 0)
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
                }
                return Json("RequestSent", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
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
        /// Approve non contractual financial hotel request
        /// </summary>
        /// <param name="travelReqID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string BulkApproveFinancialHotelBookingRequest(string travelReqID, string hid, int clubid, string approverEmpcCode, string type)
        {
            var jsonmsg = string.Empty;
            var s = 0;
            if (type == "a")
            {
                s = _bulkUploadRepository.BulkApproveNonContractualHotelRequest(travelReqID, hid, clubid, approverEmpcCode, type);
            }
            if (s >= 1)
            {
                jsonmsg = "Approval Alert : \n Approval process has been completed successfully.";
            }
            if (s == -1)
            {
                jsonmsg = "Alert : The request has been already Approved.";
            }
            return (s == 0 ? "Alert: The request has been already Rejected." : jsonmsg);
        }

        /// <summary>
        /// Reject non contractual financial hotel request
        /// </summary>
        /// <param name="travelReqID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string BulkRejectFinancialHotelBookingRequest(string travelReqID, string hid, int clubId, string approverEmpcCode, string type)
        {
            var jsonmsg = string.Empty;
            var s = 0;
            if (type == "r")
                s = _bulkUploadRepository.BulkRejectNonContractualHotelRequest(travelReqID, hid, clubId, approverEmpcCode, type);

            if (s >= 1)
                jsonmsg = "Rejection Alert : Request has been rejected successfully.";
            if (s == -1)
            {
                jsonmsg = "Alert : The request has been already Rejected.";
            }
            return (s == 0 ? "Alert: The request has been already Approved." : jsonmsg);
        }

        /// <summary>
        /// Exception Handling
        /// </summary>
        /// <param name="filterContext"></param>
        void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            Logging.ErrorLog.Instance.AddDBLogging(ex, filterContext.Controller.ControllerContext.ToString(), "Controllers/mailHotelController.cs");
        }
    }
}