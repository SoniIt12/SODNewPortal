using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SOD.Model;
using OfficeOpenXml;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;
using SOD.Services.Repository;
using System.Data.Entity.Infrastructure;
using System.Configuration;
using System.Text;
using SOD.CommonWebMethod;
using SOD.EmailNotification;
using System.Threading.Tasks;
using System.Diagnostics;


namespace SOD.Controllers
{

    [HandleError(ExceptionType = typeof(DbUpdateException), View = "Error")]
    [SessionTimeout]
    public class OatController : Controller, IActionFilter, IExceptionFilter
    {
        private readonly IUserRepository _userRepository;
        private readonly BulkUploadRepository _bulkUploadRepository;
        private readonly IOALRepository _oalRepository;
        private readonly IOATrepository _oaTRepository;
        private readonly ITransportRepository _transportRepository;
        //private readonly HotelOnlyController hotelOnlycontroller;

        public OatController()
        {
            _oalRepository = new OALRepository(new SodEntities());
            _userRepository = new UserRepository(new SodEntities());
            _bulkUploadRepository = new BulkUploadRepository(new SodEntities());
            _oaTRepository = new OATrepository(new SodEntities());
            _transportRepository = new TransportRepository(new SodEntities());
        }

        // GET: OAL
        public ActionResult oatbooking()
        {
            return View();
        }

        // GET: OAL
        public ActionResult oatTravelHistory()
        {
            return View();
        }

        // GET: OAL
        public ActionResult flist()
        {
            return View();
        }

        // GET: OAt hotel alignment
        public ActionResult OAThotelList()
        {
            return View();
        }

        //get all the details of User
        public JsonResult getUserDetail()
        {
            string empiD = Session["EmpId"].ToString();
            var s = _userRepository.GetEmployeeList(int.Parse(empiD));
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        //get all the details of User
        public JsonResult getPaxType()
        {
            var s = _oaTRepository.GetPaxType();
            return Json(s, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        //get bookingRight of Employee
        public JsonResult getOATBookingRight()
        {
            var EmpCode = Session["EmpCode"];
            var s = _oaTRepository.getOATBookingRight(EmpCode.ToString());
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        

        public JsonResult FetchEmployeeData(string empCode)
        {
            var empDetails = _bulkUploadRepository.fetchdetails(empCode, 0);
            return Json(empDetails, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FetchIOTAList(string IOTACode)
        {
            var LikeCode = IOTACode.Split(',')[0];
            var unlikeCode = IOTACode.Split(',')[1];
            var IsDomestic = IOTACode.Split(',')[2];
            var empDetails = _oaTRepository.fetchIOTAList(LikeCode, unlikeCode , IsDomestic);
            return Json(empDetails, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMasterOatList()
        {
            String empCode = Session["EmpCode"].ToString();
            var oatMasterList = _oaTRepository.getOatMasterlist(empCode);
            return Json(oatMasterList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult updateFlightDetail(OATTravelRequestFlightDetailModal Detail)
        {
            var jsonmsg = string.Empty; 
            var s = _oaTRepository.updateFlightDetail(Detail);
            if (s > 0) jsonmsg = "your request is successfully updated.";
            else jsonmsg = "Oops! something went wrong.";
            return Json(jsonmsg, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSelectedOatList(Int64 oatReqId, Int64 trreqId, string bookingfor)
        {
            var getallSelecteddetails = new List<viewOatDetailsModal>();

            getallSelecteddetails = _oaTRepository.getselecteddetails(oatReqId, trreqId, bookingfor);

            return Json(getallSelecteddetails, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetBookingHotelInfo(String trId)
        {
            var viewDetail = new List<viewOatHotelDetailsModal>();
            viewDetail = _oaTRepository.getViewOatHotelDetail(trId);
            return Json(viewDetail, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GetFlightOatListasPasID(Int64 oatReqID, Int64 ID)
        {
            var viewDetail = new List<OATTravelRequestFlightDetailModal>();
            viewDetail = _oaTRepository.getViewOatFlightDetail(oatReqID, ID);
            return Json(viewDetail, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult submitBookingFromHistory(List<OATTravelRequestFlightDetailModal> flightInfo, List<BulkUploadModels> hotelDetailList, Int64 passID, Int64 oatReqNo)
        {
            // var s = 0;
            var jsonmsg = string.Empty;
            string Result = string.Empty;
            var list = new Dictionary<string, object>();
            try
            {
                //Int64 oatReqNo = Convert.ToInt64(OatReqId);
                list = _oaTRepository.getOatDetailsPerReqId(oatReqNo);
                List<OATTravelRequestPassengerDetailModal> passangerInfo = _oaTRepository.getPassengerDetail(oatReqNo, passID);
                List<OATTravelRequestMasterModal> personalInfo = list["oatMasterList"] as List<OATTravelRequestMasterModal>;
                if (hotelDetailList != null)
                {
                    var addNo = _bulkUploadRepository.FindAddNo(hotelDetailList[0].TrnId);
                    hotelDetailList[0].Title = passangerInfo[0].Gender;
                    hotelDetailList[0].FirstName = passangerInfo[0].FirstName;
                    hotelDetailList[0].LastName = passangerInfo[0].LastName;
                    hotelDetailList[0].EmpCode = passangerInfo[0].EmployeeCode;
                    hotelDetailList[0].AddNo = addNo;

                    Result = OnlyhotelBulkBooking(personalInfo, passangerInfo, flightInfo, hotelDetailList);
                    if (Result == "AlreadyExist")
                    {
                        jsonmsg = "Warning: A similar hotel booking already exists. Kindly cancel the previous hotel booking and try again.";
                        return Json(jsonmsg, JsonRequestBehavior.AllowGet);
                    }
                    if (Result == "Fail" || Convert.ToInt64(Result) == 0)
                    {
                        jsonmsg = "Oops! Something went Wrong.";
                        return Json(jsonmsg, JsonRequestBehavior.AllowGet);
                    }

                    //update oatreqId 
                    if (personalInfo[0].TravelRequestId == 0)
                    {
                        _oaTRepository.UpdateTrnIdOnOatMaster(Convert.ToInt64(Result), oatReqNo);
                    }
                    _oaTRepository.UpdatePassengerModal(oatReqNo, passID, 2);
                    jsonmsg = "Your Request is submitted Successfully.";
                    if (flightInfo != null)
                    {
                        flightInfo[0].Empcode = passangerInfo[0].EmployeeCode;
                        flightInfo[0].EmpName = passangerInfo[0].Gender + " " + passangerInfo[0].FirstName + " " + passangerInfo[0].LastName;
                        var hotel_List = new List<BulkUploadModels>();
                        var s = _oaTRepository.submitDataForOnlyFlight(flightInfo);
                        _oaTRepository.UpdatePassengerModal(oatReqNo, passID, 2);
                        if (s > 0)
                        {
                            var emailSubject = "OAT Booking Request Notification :" + System.DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss tt");
                            var emailTemplateName = "OATRequestNotificationTemplate_User.html";
                            var emailCredentials = EmailCredentialsOatUser(emailSubject, emailTemplateName, personalInfo, passangerInfo, flightInfo, hotelDetailList, oatReqNo.ToString());
                            TempData["emailData"] = emailCredentials;
                            TempData["emailId"] = personalInfo[0].EmailId.Trim();
                            var emaildata = TempData["emailData"] as EmailNotificationModel;
                            jsonmsg = "Your Request has been submitted successfully.Your booking request has been sent to OAT traveldesk further processing";
                            TempData["jsonmsg"] = jsonmsg;
                        }
                        _oaTRepository.UpdatePassengerModal(oatReqNo, passID, 2);
                    }
                    sendEmailNotification();
                }
            }
            catch (Exception ae)
            {
                throw ae;
            }
            return Json(jsonmsg, JsonRequestBehavior.AllowGet);
        }

       
        [HttpPost]
        public JsonResult submitDataForBooking(List<OATTravelRequestMasterModal> personalInfo, List<OATTravelRequestPassengerDetailModal> passangerInfo, List<OATTravelRequestFlightDetailModal> flightInfo, string flightType, List<BulkUploadModels> hotelDetailList)
        {
            // var s = 0;
            var jsonmsg = string.Empty;
            Int64 trnID = 0;
            var Result = string.Empty;
            try
            {
                if (flightInfo != null)
                {
                    Result = CheckOnlyflightBooking(flightInfo, personalInfo);
                    if (Result == "AlreadyExist")
                    {
                        jsonmsg = "Warning: A similar flight booking already exists.";
                        return Json(jsonmsg, JsonRequestBehavior.AllowGet);
                    }
                }
                if (hotelDetailList != null)
                {
                    // trnID = Convert.ToInt64(OnlyhotelBulkBooking(personalInfo, passangerInfo, flightInfo, hotelDetailList));
                    Result = OnlyhotelBulkBooking(personalInfo, passangerInfo, flightInfo, hotelDetailList);
                    if (Result == "AlreadyExist")
                    {
                        jsonmsg = "Warning: A similar hotel booking already exists. Kindly cancel the previous hotel booking and try again.";
                        return Json(jsonmsg, JsonRequestBehavior.AllowGet);
                    }
                    if (Result == "Fail" || Convert.ToInt64(Result) == 0)
                    {
                        jsonmsg = "Oops! Something went Wrong.";
                        return Json(jsonmsg, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        trnID = Convert.ToInt64(Result);
                    }
                }
                personalInfo[0].TravelRequestId = Convert.ToInt64(trnID);
                //Dictionary<string, object> dicList = _bulkUploadRepository.GetBulkBookingInfoForPNRGeneration(Convert.ToInt64(Result.ToString()));
                //TempData["hotelList"] = dicList["hotelinfolist"] as List<TravelRequestHotelDetailModels>;
                //var hotel_List = dicList["bulkDetailInfo"] as List<BulkUploadModels>;
                var s = _oaTRepository.submitDataForsendingToIth(personalInfo, passangerInfo, flightInfo);
                if (s > 0)
                {
                    var emailSubject = "OAT Booking Request Notification :" + System.DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss tt");
                    var emailTemplateName = "OATRequestNotificationTemplate_User.html";
                    var emailCredentials = EmailCredentialsOatUser(emailSubject, emailTemplateName, personalInfo, passangerInfo, flightInfo, hotelDetailList, s.ToString());
                    TempData["emailData"] = emailCredentials;
                    TempData["emailId"] = personalInfo[0].EmailId.Trim();
                    var emaildata = TempData["emailData"] as EmailNotificationModel;
                    jsonmsg = "OAT Booking Request ID : " + s.ToString() + ". " + "Booking process has been completed successfully. Your booking request has been sent to OAT traveldesk for further processing.";

                    sendEmailNotification();
                }
            }
            catch (Exception ae)
            {
                jsonmsg = "Oops! Something went Wrong.";
                //throw ae;
            }
            TempData["jsonmsg"] = jsonmsg;
            return Json(jsonmsg, JsonRequestBehavior.AllowGet);
        }

        public string CheckOnlyflightBooking(List<OATTravelRequestFlightDetailModal> flightInfo, List<OATTravelRequestMasterModal> personalInfo)
        {
            var jsonmsg = string.Empty;
            var result = _oaTRepository.CheckOnlyflightBooking(flightInfo);
            if (result != "")
            {

                foreach (var lst in personalInfo)
                {
                    if (lst.BookingFor == "Roistering(Flight ops / In Flight)")
                    {
                        jsonmsg = "";
                        TempData["jsonmsg"] = "";
                    }
                    else
                    {
                        jsonmsg = "AlreadyExist";
                        TempData["jsonmsg"] = "Warning: A similar flight booking already exists for Employee " + result;
                    }
                }


            }
            return jsonmsg;
        }
        /// <summary>
       
        /// Send Email Notification to booking requester/user
        /// </summary>
        /// <returns></returns>
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

        ///bulk hotel booking
        public string OnlyhotelBulkBooking(List<OATTravelRequestMasterModal> personalInfo, List<OATTravelRequestPassengerDetailModal> passangerInfo, List<OATTravelRequestFlightDetailModal> flightInfo, List<BulkUploadModels> hotelDetailList)
        {
            var jsonmsg = string.Empty;
            var TrnId = 0;
            var bulkMaster = new BulkUploadMasterModels();
            var bulkList = new List<BulkUploadModels>();
            var empList = new List<string>();
            var bulkhotelList = new List<BulkTravelRequestHotelDetailModels>();
            var time = System.DateTime.Now.ToString("yyyyMMddHHss");
            string hodEmailId = "";
            try
            {
                foreach (var i in hotelDetailList)
                {
                    var empcode =  (i.EmpCode.Length == 6) ? ("00" + i.EmpCode) : i.EmpCode;
                    var checkin = i.CheckInDate;
                    var hotelcity = i.HotelCity;
                    var result = _transportRepository.FindDuplicateDataHotel(empcode, checkin, hotelcity);
                    if (result == true)
                    {
                        jsonmsg = "AlreadyExist";
                        TempData["jsonmsg"] = jsonmsg;
                        return jsonmsg;
                    }
                }

                //Bulk Upload Master 
                bulkMaster.FileName = "OAT View Detail_" + time;
                bulkMaster.FilePath = "../UploadFile/";
                bulkMaster.CreatedById = Session["EmpCode"].ToString().Trim();
                var UserName = Session["UserInfo"].ToString().Replace("Welcome : ", "").Split('|')[0].ToString();
                bulkMaster.CreatedByName = UserName;
                bulkMaster.CreatedDate = DateTime.Now;
                bulkMaster.TransactionDate = DateTime.Now;
                bulkMaster.DepartmentId = Convert.ToInt32(Session["DeptIdCR"]);
                bulkMaster.VerticalCode = Session["VerticalId"].ToString();
                bulkMaster.FileStatus = "Open";
                bulkMaster.BookingType = "OAT";
                Int32 n = 0;
                //Get HOD Details
                if (personalInfo[0].BookingFor != "Self" && personalInfo[0].BookingFor != "Others (On Behalf of Employee / Non-Employee)")
                {
                    hodEmailId = personalInfo[0].EmailId;
                }
                else
                {
                    hodEmailId = SOD.Services.ADO.SodCommonServices.GetHodEmailDetails(Session["EmpId"].ToString().Trim(), 2);
                }
                foreach (var lst in hotelDetailList)
                {
                    var list = passangerInfo.Where(p => p.EmployeeCode.Contains(lst.EmpCode)).ToList();
                    foreach (var l in list)
                    {                        
                        lst.SrNo = n + 1;
                        lst.EmpCode = (l.EmployeeCode.Length == 6) ? ("00" + l.EmployeeCode):l.EmployeeCode;
                        lst.Title = l.Gender;
                        lst.FirstName = l.FirstName;
                        lst.LastName = l.LastName;
                        lst.Designation = l.Designation;
                        lst.Department = l.Department;
                        lst.MobileNo = l.PhoneNo;
                        lst.EmailId = l.EmailId;
                        lst.CreatedDate = DateTime.Now;
                        lst.PNR = "";
                        if (personalInfo[0].BookingFor != "Self" && personalInfo[0].BookingFor != "Others (On Behalf of Employee / Non-Employee)")
                        {
                            lst.PNR = "ERR001";
                            lst.PNR_Status = 1;
                        }
                    }
                    n++;
                }

                if (hodEmailId.Length > 0)
                {
                    bulkMaster.ApproverEmailID = hodEmailId.Split(',')[0].ToString();
                    if (personalInfo[0].TravelRequestId != 0)
                    {
                        hotelDetailList[0].TrnId = personalInfo[0].TravelRequestId;
                        _bulkUploadRepository.saveBulk_newRow(hotelDetailList);
                        TrnId = Convert.ToInt32(personalInfo[0].TravelRequestId);
                    }
                    else
                    {
                        TrnId = _bulkUploadRepository.SaveBulkUploadTemp(bulkMaster, hotelDetailList);
                    }

                }
                else return TrnId.ToString();
                //Logged User Info
                if (personalInfo[0].BookingFor != "Self" && personalInfo[0].BookingFor != "Others (On Behalf of Employee / Non-Employee)")
                {
                    var approvalList = new List<BulkBookingRequestApprovalModels>();
                    BulkBookingRequestApprovalModels model = new BulkBookingRequestApprovalModels();
                    model.TrRequestId = TrnId;//Travel Request
                    model.ApprovedByEmpId = personalInfo[0].RequestedEmpId;
                    model.ApprovalStatus = 1;
                    model.IsMandatoryTravel = 0;//Is Mandatory Travel
                    model.ApprovalDate = System.DateTime.Now;
                    model.Comment = "SELF APPROVED";
                    model.ApprovedByEmpIdCLevel1 = 0;
                    model.ApprovedByEmpIdCLevel2 = 0;
                    model.ApprovalStatusCLevel1 = 0;
                    model.ApprovalStatusCLevel2 = 0;
                    model.CLevelComment1 = "";
                    model.CLevelComment2 = "";
                    model.CLevelAppDate1 = DateTime.Parse("01/01/1900");
                    model.CLevelAppDate2 = DateTime.Parse("01/01/1900");
                    model.RevenueApprovedStatus = 0;
                    model.RevenueApprovedDate = DateTime.Parse("01/01/1900");
                    model.AddNo = hotelDetailList[0].AddNo;
                    model.BReqId = 0;
                    approvalList.Add(model);
                    var s1 = _bulkUploadRepository.ApproveBulkSodBookingRequestSelective(approvalList);
                    if (s1 == 0) jsonmsg = "Fail";
                    else jsonmsg = TrnId.ToString();

                }
                else if (hodEmailId.Length > 0)
                {
                    var bulkController = DependencyResolver.Current.GetService<bulkController>();
                    var loggedUserInfo = bulkController.GetLoginUserInfo(Session["EmpId"].ToString().Trim());
                    var hodName = hodEmailId.Split(',')[1];

                    //var reqName = loggedUserInfo[2] == "M" ? "Mr." : "Ms." + " " + loggedUserInfo[1];
                    //Send Email Notification to  Hod
                    var emailSubject = "OAT Hotel Booking Approval Notification :" + System.DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss tt");
                    //List<TravelRequestHotelDetailModels> hoteldetails = new List<TravelRequestHotelDetailModels>();
                    List<TravelRequestMasterModels> bookingInfo = new List<TravelRequestMasterModels>();
                    List<FlightDetailModels> flightinfo = new List<FlightDetailModels>();
                    var emailTemplateName_hod = "OATRequestNotificationTemplate_HOD.html";
                    var emailCredentials = EmailCredentialsHotelHod(emailSubject, emailTemplateName_hod, personalInfo, bulkList, TrnId.ToString(), hodName, hotelDetailList, bookingInfo, flightinfo);


                    var skey = new StringBuilder();
                    skey.Append(TrnId.ToString() + ",");
                    skey.Append(hodEmailId.Split(',')[2] + ",");

                    var uri1 = ConfigurationManager.AppSettings["emailBulkApprovalPath"].Trim() + "?str=" + skey + "&type=Ha" + "&edit=" + hotelDetailList[0].AddNo;
                    var uri2 = ConfigurationManager.AppSettings["emailBulkApprovalPath"].Trim() + "?str=" + skey + "&type=Hr" + "&edit=" + hotelDetailList[0].AddNo;
                    var essurl = ConfigurationManager.AppSettings["essportalUrl"].Trim();
                    var appLink = string.Empty;
                    appLink = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a name='app' style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Acceptance</a></td>";
                    appLink = appLink + "<td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a name='rej' style='color:#fff; text-decoration:none;' href='" + uri2 + "'>Rejection</a></td> </tr></table>";

                    var templateData = emailCredentials.TemplateFilePath;
                    //var hodName = hodEmailId.Split(',')[1];

                    templateData = templateData.Replace("[appLink]", appLink);
                    //templateData = templateData.Replace("[hodName]", hodName);
                    emailCredentials.TemplateFilePath = templateData;

                    TempData["emailData_Hod"] = emailCredentials;
                    TempData["emailId_Hod"] = hodEmailId.Split(',')[0];
                    jsonmsg = TrnId.ToString();
                }

            }
            catch (Exception ex)
            {
                jsonmsg = "Fail";
            }
            return jsonmsg;
        }


        /// <summary>
        /// Initialized Email Credentials to Property
        /// </summary>
        /// <returns></returns>
        public EmailNotificationModel EmailCredentialsHotelHod(String subjectName, String emailTemplateName, List<OATTravelRequestMasterModal> personalInfo, List<BulkUploadModels> blist, String bbReqNo, String HodName, List<BulkUploadModels> hoteldetails, List<TravelRequestMasterModels> bookingInfo, List<FlightDetailModels> flightinfo)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtpUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtpPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtpServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtpPort"].Trim(),
                TemplateFilePath = ReadFileHotelHod(emailTemplateName, personalInfo, blist, bbReqNo, HodName, hoteldetails, bookingInfo, flightinfo),
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
        private string ReadFileHotelHod(string emailTemplateName, List<OATTravelRequestMasterModal> personalInfo, List<BulkUploadModels> blist, String reqNo, String HodName, List<BulkUploadModels> hoteldetails, List<TravelRequestMasterModels> bookingInfo, List<FlightDetailModels> flightinfo)
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

            //var Title = bookingInfo[0].Title;

            var HotelTitle = "Booking Details ";
            strContent = strContent.Replace("[RequesterName]", personalInfo[0].Gender + " " + personalInfo[0].RequestedEmpName);
            strContent = strContent.Replace("[hodName]", HodName);
            strContent = strContent.Replace("[binfo]", HotelTitle + "  (Booking Request Id : " + reqNo + ")");
            var tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td style='padding-left: 5px;'>Reason for Booking</td><td>Booking Type</td><td>No. of Passenger(s)</td><td>Booking For</td></tr>";


            tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td style='border: 1px solid #e3e3e3;padding: 5px;'>" + personalInfo[0].ReasonForTravel + " </td><td style='border: 1px solid #e3e3e3;padding: 5px;'>" + personalInfo[0].BookingFor + "</td><td style='border: 1px solid #e3e3e3;padding: 5px;'>" + personalInfo[0].PaxNo + "</td><td style='border: 1px solid #e3e3e3;padding: 5px;'>" + personalInfo[0].BookingType + "</td></tr>";

            var trp = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td style='padding-left: 5px;'>S. No</td><td>Employee Code</td><td>Employee Name</td><td>City</td><td>Check-In Date</td><td>Check-Out Date</td><td>Check-in Time</td><td>Check-Out Time</td></tr>";
            var count = 0;
            foreach (var i in hoteldetails)
            {
                count = count + 1;
                var name = i.Title + " " + i.FirstName + " " + i.LastName;
                trp = trp + "<tr style='font-family:Arial; font-size:12px;'><td style='border: 1px solid #e3e3e3;padding: 5px;'>" + count + "</td><td style='border: 1px solid #e3e3e3;padding: 5px;'>" + i.EmpCode + "</td><td style='border: 1px solid #e3e3e3;padding: 5px;'>" + name + "</td><td  style='border: 1px solid #e3e3e3;padding: 5px;'>" + i.HotelCity + "</td><td style='border: 1px solid #e3e3e3;padding: 5px;'>" + i.CheckInDate.ToString("dd/MM/yyyy") + "</td><td style='border: 1px solid #e3e3e3;padding: 5px;'>" + i.CheckOutDate.ToString("dd/MM/yyyy") + "</td> <td style='border: 1px solid #e3e3e3;padding: 5px;'>" + i.CheckinTime + "</td> <td style='border: 1px solid #e3e3e3;padding: 5px;'>" + i.CheckoutTime + "</td>";
            }



            strContent = strContent.Replace("[tr]", tr);
            strContent = strContent.Replace("[trp]", trp);
            strContent = strContent.Replace("[finfo]", "Hotel Detail");

            strContent = strContent.Replace("[hinfo]", "");
            strContent = strContent.Replace("[trh]", "");

            return strContent.ToString();
        }


        /// <summary>
        /// Initialized Email Credentials to Property
        /// </summary>
        /// <returns></returns>
        public EmailNotificationModel EmailCredentialsOatUser(string subjectName, string emailTemplateName, List<OATTravelRequestMasterModal> personalInfo, List<OATTravelRequestPassengerDetailModal> passangerInfo, List<OATTravelRequestFlightDetailModal> flightInfo, List<BulkUploadModels> hotelDetailList, string oatReqNo)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtpUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtpPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtpServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtpPort"].Trim(),
                TemplateFilePath = ReadFileForOatUser(emailTemplateName, personalInfo, passangerInfo, flightInfo, hotelDetailList, oatReqNo),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }
        /// Read File to write in Email
        /// </summary>
        /// <param name="emailTemplateName"></param>
        /// <param name="hoteldetails"></param>
        /// <param name="bookingInfo"></param>
        /// <param name="flightinfo"></param>
        /// <param name="reqNo"></param>
        /// <param name="approvalStatus"></param>
        /// <returns></returns>
        private string ReadFileForOatUser(string emailTemplateName, List<OATTravelRequestMasterModal> personalInfo, List<OATTravelRequestPassengerDetailModal> passangerInfo, List<OATTravelRequestFlightDetailModal> flightInfo, List<BulkUploadModels> hotelDetailList, string oatReqNo)
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
            var Title = personalInfo[0].Gender;

            strContent = strContent.Replace("[RequesterName]", Title + " " + personalInfo[0].RequestedEmpName);
            strContent = strContent.Replace("[binfo]", "OAT Booking Information   (Request Id : " + oatReqNo + ")");
            strContent = strContent.Replace("[pinfo]", "  Passenger Information(s)");



            var tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Reason for Booking</td><td>Booking For</td><td>No. of Guests</td><td>Booking Type</td></tr>";
            foreach (var b in personalInfo)
            {
                tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td>" + b.ReasonForTravel + "</td><td>" + b.BookingFor + "</td><td>" + b.PaxNo + "</td><td>" + b.BookingType + "</td></tr>";
            }

            var trp = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>S.No.</td><td>Name</td><td>Designation</td><td>Department</td><td>Email</td><td>Phone</td></tr>";
            var countP = 1;
            foreach (var b in passangerInfo)
            {
                //var PTitle = b.Gender == "F" ? "Ms." : "Mr.";
                trp = trp + "<tr style='font-family:Arial; font-size:12px;'><td>" + countP + "</td><td>" + b.Gender + " " + b.FirstName + " " + b.LastName + "</td><td>" + b.Designation + "</td><td>" + b.Department + "</td><td>" + b.EmailId + "</td ><td>" + b.PhoneNo + "</td></tr>";
                countP++;
            }
            var trh = "";
            if (hotelDetailList != null)
            {
                if (hotelDetailList.Count != 0)
                {
                    var countH = 1;
                    strContent = strContent.Replace("[hinfo]", "  Hotel Information(s)");
                    trh = "<table cellpadding='0' cellspacing='0' style='width:100%; font-size:12px;border:solid 1px #c2c2c2;line-height:30px;'><tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>S.No. </td><td>Passenger Name</td><td>City</td><td>Check-In Date</td><td>Check-Out Date</td><td>Check-in time</td><td>Check-out time</td></tr>";
                    foreach (var h in hotelDetailList)
                    {

                        trh = trh + "<tr style='font-family:Arial; font-size:12px;'><td>" + countH + "</td><td>" + h.Title + " " + h.FirstName + " " + h.LastName + "</td><td>" + h.HotelCity.ToUpper() + "</td><td>" + h.CheckInDate.ToString("dd/MMM/yyyy") + "</td><td>" + h.CheckOutDate.ToString("dd/MMM/yyyy") + "</td><td>" + h.CheckinTime + "</td><td>" + h.CheckoutTime + "</td></tr>";
                        countH++;
                    }
                    trh = trh + "</table >";
                }
            }
            else
            {
                strContent = strContent.Replace("[hinfo]", " ");
            }

            var trf = "";
            if (flightInfo != null)
            {
                var countf = 1;
                strContent = strContent.Replace("[finfo]", "  Flight Information(s)");
                trf = "<table cellpadding='0' cellspacing='0' style='width:100%; font-size:12px;border:solid 1px #c2c2c2;line-height:30px;'><tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td style='border-bottom: 1px solid #c2c2c2;'>S. No.</td><td style='border-bottom: 1px solid #c2c2c2;'>Passenger Name</td><td style='border-bottom: 1px solid #c2c2c2;'>Sector</td><td style='border-bottom: 1px solid #c2c2c2;'>Travel Date</td><td style='border-bottom: 1px solid #c2c2c2;'>Departure time</td><td style='border-bottom: 1px solid #c2c2c2;'>Airline Name</td><td style='border-bottom: 1px solid #c2c2c2;'>Flight No.</td></tr>";
                foreach (var h in flightInfo)
                {
                    var flightNo = h.FlightNumber == null ? "-" : h.FlightNumber;
                    var Aircraft = h.AirCraftName == null ? "-" : h.AirCraftName;
                    trf = trf + "<tr style='font-family:Arial; font-size:12px;'><td style='border-bottom: 1px solid #c2c2c2;'>" + countf + "</td><td style='border-bottom: 1px solid #c2c2c2;'>" + h.Gender + " " + h.EmpName + "</td><td style='border-bottom: 1px solid #c2c2c2;'>" + h.OriginPlace.ToUpper() + "-" + h.DestinationPlace.ToUpper() + "</td><td style='border-bottom: 1px solid #c2c2c2;'>" + h.DepartureDate.ToString("dd/MMM/yyyy") + "</td><td style='border-bottom: 1px solid #c2c2c2;'>" + h.DepartureTime + "</td><td style='border-bottom: 1px solid #c2c2c2;'>" + Aircraft + "</td><td style='border-bottom: 1px solid #c2c2c2;'>" + flightNo + "</td></tr>";
                    countf++;
                }
                trf = trf + "</table >";

            }
            else
            {
                strContent = strContent.Replace("[finfo]", " ");
            }
            strContent = strContent.Replace("[tr]", tr);
            strContent = strContent.Replace("[trh]", trh);
            strContent = strContent.Replace("[trp]", trp);
            strContent = strContent.Replace("[trf]", trf);
            return strContent.ToString();
        }
        [HttpPost]
        public JsonResult CheckAlreadyBookData(List<OATTravelRequestMasterModal> personalInfo, List<OATTravelRequestPassengerDetailModal> passangerInfo, List<OATTravelRequestFlightDetailModal> flightInfo)
        {
            var Result = new List<OATTravelRequestFlightDetailModal>();
            for (var i = 0; i < personalInfo.Count; i++)
            {
                if (personalInfo[i].BookingFor == "Roistering(Flight ops / In Flight)")
                {
                    Result = _oaTRepository.GetListOfRoisteringBooking(flightInfo);

                }
                else
                {
                    Result = null;
                }
            }
            return Json(Result, JsonRequestBehavior.AllowGet);
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