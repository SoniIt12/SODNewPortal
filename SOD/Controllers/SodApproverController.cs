using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;
using SOD.Services.Repository;
using SOD.Model;
using SOD.CommonWebMethod;
using SOD.EmailNotification;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;

namespace SOD.Controllers
{
    public class SodApproverController : Controller, IActionFilter, IExceptionFilter
    {

        /// <summary>
        /// Constructor Initialization
        /// </summary>
        private readonly ISodApproverRepositorty _sodApproverRepositorty;
        private readonly IUserRepository _userRepository;
        private readonly ITransportRepository _transportRepository;
        private readonly IHdRepository _hdRepositorty;

        public SodApproverController()
        {
            _sodApproverRepositorty = new SodApproverRepositorty(new SodEntities());
            _userRepository = new UserRepository(new SodEntities());
            _transportRepository = new TransportRepository(new SodEntities());
            _hdRepositorty = new HdRepository(new SodEntities());
        }


        // GET: SodApprover
        public ActionResult Approver()
        {
            var strQuery = new StringBuilder();
            Session["EmpId"] = Request.QueryString[0].ToString();
            ViewBag.strquery = Request.QueryString[0].ToString() + "," + Request.QueryString[1].ToString();
            return View();
        }

        // GET: Sod Blanket Booking
        public ActionResult BlanketBooking()
        {
            return View();
        }

        public ActionResult GetBookingReport()
        {
            ViewBag.Usertype = Session["usertype"];
            return View();
        }
        // GET: Sod Allocate Approver  
        public ActionResult AllocateApprover()
        {
            return View();
        }


        /// <summary>
        /// Get User Booking List
        /// </summary>
        /// <returns></returns>
        public ActionResult GetBookingList()
        {
            if (Session["EmpId"] != null)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["dep"]) && !string.IsNullOrEmpty(Request.QueryString["deg"]) && Session["SjsUserId"] == null)
                {
                    var deptartmentId = Convert.ToInt32(Request.QueryString["dep"].ToString().Trim());
                    var designationId = Convert.ToInt32(Request.QueryString["deg"].ToString().Trim());
                    SodBookingList(deptartmentId, designationId);
                }
                if (Session["SjsUserId"] != null && Session["HodEmailId"] != null)
                {
                    string SjscHod = Session["HodEmailId"].ToString();
                    SjscBookingList(SjscHod);
                }
            }
            return View();
        }


        /// <summary>
        /// Get User Travel History List
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewTravelList()
        {
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
                : Json("", JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Get Employee Booking Status  
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetEmployeeBookingStatus(int reqId)
        {
            var rqId = Convert.ToInt32(Request.QueryString["reqId"].ToString());
            var s = _sodApproverRepositorty.GetEmployeeBookingStatus(rqId, 1);
            return Json(s, JsonRequestBehavior.AllowGet);
        }
        //[HttpGet]
        //public JsonResult GetEmployeeBookingclosedStatus(int reqId)
        //{
        //    var rqId = Convert.ToInt32(Request.QueryString["reqId"].ToString());
        //    var s = _sodApproverRepositorty.GetEmployeeBookingclosedStatus(rqId, 1);
        //    return Json(s, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// Get Employee History
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetEmployeeBookingHistory(int empId)
        {
            var emId = 0;
            if (Request.QueryString["empId"].Equals("0"))
                emId = Convert.ToInt32(Session["EmpId"].ToString());
            else
                emId = Convert.ToInt32(Request.QueryString["empId"].ToString());

            var s = _sodApproverRepositorty.GetSodEmployeeBookingHistoryList(0, emId, 1);
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Employee History Employee Code wise
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetEmployeeBookingHistory_ByEmpCode()
        {
            var empcode = "0";
            if (Request.QueryString["EmpId"].Equals("0"))
                if (Request.QueryString["empCode"] != null || Request.QueryString["empCode"] == "0")
                    empcode = Request.QueryString["empCode"];
                else
                    empcode = Session["EmpCode"].ToString();
            var fdate = Request.QueryString["fdate"] != null ? Request.QueryString["fdate"] : "01/01/1900";
            var tdate = Request.QueryString["tdate"] != null ? Request.QueryString["tdate"] : "01/01/1900";
            var IsVendorBooking = Convert.ToBoolean(Request.QueryString["IsVendorBooking"]);

            if (Request.QueryString["prm"] == "1")
            {
                var data = _sodApproverRepositorty.GetSodEmployeeBookingHistoryList_ByEmployeeCode(empcode, 1, IsVendorBooking, fdate, tdate, null);
                Session["UserRole"] = Request.QueryString["prm"];
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var hoddata = _sodApproverRepositorty.GetSodApproverBookingHistoryList_ByEmployeeCode(empcode, 2, IsVendorBooking, fdate, tdate, Session["Department"].ToString());
                Session["UserRole"] = Request.QueryString["prm"];
                TempData["HODApprovalData"] = hoddata;
                return Json(hoddata, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Export Data
        /// </summary>
        public void ExportVendorData()
        {
            var fdate = Request.QueryString["fdate"];
            var tdate = Request.QueryString["tdate"];
            var empcode = Session["EmpId"].ToString();
            var type = Convert.ToBoolean(Request.QueryString["IsVendorBooking"]) == true ? "1" : "0";
            if (Session["UserRole"].ToString() == "1")
            {
                GetEmployeeHistoryData(fdate, tdate, type, empcode);
            }
            else
            {
                ExportApprovalData();
            }
        }

        /// <summary>
        /// Get Employee Booking History : Export Data
        /// </summary>
        /// <param name="fdate"></param>
        /// <param name="tdate"></param>
        /// <param name="type"></param>
        /// <param name="empcode"></param>
        public void GetEmployeeHistoryData(string fdate, string tdate, string type, string empcode)
        {
            IDictionary data = _hdRepositorty.GetSodEmployeeBookingHistoryList_Helpdesk_ExcelExport(fdate, tdate, Convert.ToInt16(type), empcode, null, 3);
            var sodRequestsList = data["bookingList"] as List<ExcelExportModel>;
            var arr = sodRequestsList.ToArray();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=PNRList.xls");
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            WriteTsv(arr, Response.Output);
            Response.End();
        }


        /// <summary>
        /// Export HOD Approval Data
        /// </summary>
        public void ExportApprovalData()
        {
            var data = TempData["HODApprovalData"] as List<TravelRequestMasterModels>;
            var exportData = new List<ExcelExportApprovalModel>();
            foreach (var i in data)
            {
               var lst = new ExcelExportApprovalModel
                {
                    TravelRequestId = i.TravelRequestId.ToString(),
                    TravelRequestCode = i.TravelRequestCode,
                    RequestDate = i.RequestDate.ToString(),
                    BookingFor = i.BookingFor,
                    IsMandatoryTravel = i.IsMandatoryTravel.ToString(),
                    ReasonForMandatoryTravel = i.ReasonForMandatoryTravel,
                    ReasonForTravel = i.ReasonForTravel,
                    RequestedEmpCode = i.RequestedEmpCode,
                    RequestedEmpName = i.RequestedEmpName,
                    RequestedEmpDesignation = i.RequestedEmpDesignation,
                    RequestedEmpDept = i.RequestedEmpDept,
                    EmailId = i.EmailId,
                    Phno = i.Phno,
                    Pnr = i.Pnr,
                    BookingStatus = i.BookingStatus,
                    IsHotelRequired = i.IsHotelRequired.ToString()
                };
                exportData.Add(lst);
            }
            var arr = exportData.ToArray();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=ApprovalList.xls");
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
        /// Get Employee Past History
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetEmployeeBookingPastHistory(int empId)
        {
            var emId = 0;
            if (Request.QueryString["empId"].Equals("0"))
                emId = Convert.ToInt32(Session["empId"]);
            else
                emId = Convert.ToInt32(Request.QueryString["empId"].ToString());

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
        /// Get Sod booking List 
        /// </summary>
        private void SodBookingList(int deptartmentId, int designationId)
        {
            var s = _sodApproverRepositorty.GetSodBookingListForApproval(deptartmentId, designationId, Convert.ToInt32(Session["EmpId"].ToString()), 1, null);
            if (s.Count() > 0)
                TempData["ApproverList"] = s;
            else
            {
                var s1 = _sodApproverRepositorty.GetSodBookingListForApproval(deptartmentId, designationId, Convert.ToInt32(Session["EmpId"].ToString()), 4, null);
                TempData["ApproverList"] = s1;
            }
        }

        /// <summary>
        /// Get SjSc booking List  
        /// </summary>
        private void SjscBookingList(string HodEmailId)
        {
            var s = _sodApproverRepositorty.GetSodBookingListForApproval(0, 0, 0, 5, HodEmailId);
            if (s.Count() > 0)
                TempData["ApproverList"] = s;
        }


        /// <summary>
        /// Get Employee booking details 
        /// </summary>
        [HttpGet]
        public JsonResult GetBookingInfo()
        {
            var trId = Request.QueryString["trId"].ToString();
            //Display Request wise view details
            var sodRequestsList = new List<TravelRequestMasterModels>();
            var sodflightList = new List<FlightDetailModels>();
            var passengerList = new List<PassengerDetailModels>();
            var passengerMealsList = new List<PassengerMealAllocationModels>();
            var cabList = new List<TravelRequestCabDetailModels>();
            var hotelList = new List<TravelRequestHotelDetailModels>();

            var dicList = new Dictionary<string, object>();
            dicList = _sodApproverRepositorty.GetSodBookingInfoForPNR(Convert.ToInt64(trId));

            sodRequestsList = dicList["bookingInfo"] as List<TravelRequestMasterModels>;
            sodflightList = dicList["flightInfo"] as List<FlightDetailModels>;
            passengerList = dicList["passInfo"] as List<PassengerDetailModels>;
            passengerMealsList = dicList["mealsInfo"] as List<PassengerMealAllocationModels>;
            cabList = dicList["cabInfo"] as List<TravelRequestCabDetailModels>;
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
                    return Json("Request has been already approved.", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var hreject = _sodApproverRepositorty.FindHotelRejectionStatus(Convert.ToInt64(travelReqId.Split(',')[0]));
                    if (hreject == true)
                    {
                        return Json("Request has been already Rejected.", JsonRequestBehavior.AllowGet);
                    }
                    var n = _sodApproverRepositorty.ApproveOnlyHotelSodBookingRequest(approvalList);
                    if (n > 0)
                    {
                        _sodApproverRepositorty.UpdateOnlyHotelApprovalStatus(Convert.ToInt64(travelReqId.Split(',')[0]));
                        return Json("Approved", JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else
            {   //Check Duplicate PNR  
                var chkpnr = Services.ADO.SodCommonServices.CheckDuplicatePNR(Convert.ToInt64(travelReqId.Split(',')[0]), 1);
                if (!chkpnr.Equals("0"))
                {
                    return Json("Sorry : PNR has already generated for this booking request. Existing PNR No. :" + chkpnr, JsonRequestBehavior.AllowGet);
                }
                var status = _sodApproverRepositorty.checkApprovalStatusOfHod(Convert.ToInt32(travelReqId.Split(',')[0]));
                if (status == 2)
                {
                    return Json("Sorry : Rejection Process has been already completed for this Booking Request ID :" + travelReqId.Split(',')[0], JsonRequestBehavior.AllowGet);
                }
            }
            var s = _sodApproverRepositorty.ApproveSodBookingRequest(approvalList);
            if (s >= 1)
            {
                TempData["requestData"] = travelReqId;
                var reqType = travelReqId.Split(',')[2].Trim();
                var bookingfor = travelReqId.Split(',')[3].Trim();
                TempData["reqType"] = reqType.ToUpper();
                if (bookingfor.Equals("Confirm"))
                {
                    //Check : Is approver from cxo level?
                    var r = _sodApproverRepositorty.IsCXORole(Convert.ToInt32(Session["EmpId"].ToString()));
                    if (r == true && reqType.ToUpper() == "SOD")
                    {
                        jsonmsg = "Approved";
                    }
                    else if (r == true && reqType.ToUpper() == "SOD-NONSOD")
                    {
                        jsonmsg = "Approved";
                    }
                    else
                    {
                        //update for revenue level..after approved from approver..
                        var msg = "\n Sod Type :" + reqType + "\n Booking Type :Confirm \n\n Approval process has been completed successfully.\n For the final PNR approval the request has been sent to Revenue Department.";
                        jsonmsg = "Request Id :" + "SOD-" + travelReqId.Split(',')[0] + msg;
                    }
                }
                else
                {
                    jsonmsg = "Approved";
                }
            }
            else
                jsonmsg = "Invalid Request.";
            return Json(jsonmsg.Length > 0 ? jsonmsg : string.Empty, JsonRequestBehavior.AllowGet);
        }

        public void sendRejectionMailToUser(string TrnId)
        {
            var hotelList = new List<TravelRequestHotelDetailModels>();
            var dicList = new Dictionary<string, object>();
            dicList = _userRepository.GetSodHotelInfo(Convert.ToInt64(TrnId));
            TempData["bookingInfo"] = dicList["bookingInfolist"] as List<TravelRequestMasterModels>;
            TempData["hotelList"] = dicList["hotelinfolist"] as List<TravelRequestHotelDetailModels>;
            TempData["flightinfo"] = dicList["flightInfolist"] as List<FlightDetailModels>;

            //Send Email Notification  
            if (TempData["hotelList"] != null && TempData["bookingInfo"] != null && TempData["flightinfo"] != null)
            {
                var hotel_List = TempData["hotelList"] as List<TravelRequestHotelDetailModels>;
                var bookingInfo = TempData["bookingInfo"] as List<TravelRequestMasterModels>;
                var flightinfo = TempData["flightinfo"] as List<FlightDetailModels>;
                var emailSubject = "SOD Only-Hotel Booking Request Rejection Notification :" + System.DateTime.Now.ToString();
                var emailTemplateName = "OnlyhotelBookingRejection_user.html";

                var controller = DependencyResolver.Current.GetService<HotelOnlyController>();
                var emailCredentials = controller.EmailCredentialsHotel(emailSubject, emailTemplateName, hotel_List, bookingInfo, flightinfo, TrnId.ToString(), "pending");

                TempData["emailData"] = emailCredentials;
                TempData["emailId"] = bookingInfo[0].EmailId.Trim();
                var emaildata = TempData["emailData"] as EmailNotificationModel;
                var emailid = TempData["emailId"].ToString();
                EmailNotifications.SendBookingRequestNotificationTo_Requester(emaildata, emailid);
            }
        }


        /// <summary>
        /// Reject Sod Booking Request
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RejectSodBookingRequest(string travelReqId)
        {
            var jsonmsg = string.Empty;
            //Validate for undefind data
            if ((travelReqId.Split(',')[0] == "undefined") || (travelReqId.Split(',')[0] == ""))
            {
                return Json("Invalid record.", JsonRequestBehavior.AllowGet);
            }
            //Initialize list for updating revenue status
            var approvalList = new TravelRequestApprovalModels()
            {
                TravelRequestId = Convert.ToInt64(travelReqId.Split(',')[0]),
                ApprovedByEmpId = Convert.ToInt32(Session["EmpId"].ToString()),
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

            var s = _sodApproverRepositorty.RejectSodBookingRequest(approvalList);
            if (travelReqId.Split(',')[3].ToString().ToLower() == "onlyhotel")
            {
                var hreq = _sodApproverRepositorty.FindHotelApprovalStatus(Convert.ToInt64(travelReqId.Split(',')[0]));
                if (hreq == true)
                {
                    return Json("Request has been already approved.", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var hreject = _sodApproverRepositorty.FindHotelRejectionStatus(Convert.ToInt64(travelReqId.Split(',')[0]));
                    if (hreject == true)
                    {
                        return Json("Request has been already Rejected.", JsonRequestBehavior.AllowGet);
                    }
                    _sodApproverRepositorty.UpdateOnlyHotelRejectionStatus(Convert.ToInt64(travelReqId.Split(',')[0]));
                    sendRejectionMailToUser(travelReqId.Split(',')[0]);
                    return Json("Rejected", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                //Check PNR if Rejected
                var chkpnr = Services.ADO.SodCommonServices.CheckDuplicatePNR(Convert.ToInt64(travelReqId.Split(',')[0]), 1);
                if (!chkpnr.Equals("0"))
                {
                    return Json("Sorry : PNR has already generated for this booking request. Existing PNR No. :" + chkpnr, JsonRequestBehavior.AllowGet);
                }
                var status = _sodApproverRepositorty.checkApprovalStatusOfHod(Convert.ToInt32(travelReqId.Split(',')[0]));
                if (status == 1)
                {
                    return Json("Sorry : Approval Process has been already completed for this Booking Request ID :" + travelReqId.Split(',')[0], JsonRequestBehavior.AllowGet);
                }
            }
            if (s >= 1)
            {
                //Close Booking Request
                var trRequestMaster = new TravelRequestMasterModels()
                {
                    TravelRequestId = Convert.ToInt64(travelReqId.Split(',')[0]),
                    BookingStatus = "Rejected",
                    StatusDate = System.DateTime.Now
                };
                var c = _sodApproverRepositorty.CloseSodBookingRequest_HOD(trRequestMaster);
                //Send Rejection Email Notification
                var bookingType = string.Empty;
                //Get Booking Request Data for Email Notification
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
                hotelList = dicList["hotelInfo"] as List<TravelRequestHotelDetailModels>;
                passengerMealsList = dicList["mealsInfo"] as List<PassengerMealAllocationModels>;

                bookingType = sodRequestsList[0].SodBookingTypeId == 1 ? "SOD" : "NON-SOD";
                string emailSubject = bookingType + " " + "Booking Request Rejection Notification from HOD :" + System.DateTime.Now.ToString("dd-MMM-yyyy-hh:mm:ss");
                var emailTemplateName = "SodBookingRequest_Rejection_User.html";
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

                //Send Email Notification Credentials
                var emailCredentials = EmailCredentials(emailSubject, emailTemplateName, sodRequestsList, sodflightList, passengerList, passengerMealsList, hotelList, travelReqId.Split(',')[0]);
                emailCredentials.TemplateFilePath = emailCredentials.TemplateFilePath.Replace("[Team]", "HOD");

                //Check Hold PNR Status
                if (sodRequestsList[0].IsAmountPaidByTraveller.Equals(true))
                {
                    var strHoldPNR = ConfigurationManager.AppSettings["strHOLDPNRPassenger"].Trim();
                    emailCredentials.TemplateFilePath = emailCredentials.TemplateFilePath.Replace("[hpnr]", strHoldPNR);
                }
                else
                {
                    emailCredentials.TemplateFilePath = emailCredentials.TemplateFilePath.Replace("[hpnr]", "");
                }

                TempData["emailData"] = emailCredentials;
                TempData["emailId"] = sodRequestsList[0].EmailId.Trim();
                jsonmsg = "Rejected";
            }
            else
                jsonmsg = "Invalid Request.";

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
            //Sod Confirm booking
            if (TempData["requestData"] != null)
            {
                var travelReqId = TempData["requestData"].ToString();
                if (travelReqId.Split(',')[3].Equals("Standby"))
                {
                    var pnrStatus = await GeneratePNR(Convert.ToInt64(travelReqId.Split(',')[0]), travelReqId.Split(',')[1].Trim());
                    if (pnrStatus.Length.Equals(6))
                    {
                        jsonmsg = "Request Id :" + travelReqId.Split(',')[2].ToUpper() + "-" + travelReqId.Split(',')[0] + "\n Sod Type :Sod \n Booking Type :Confirm \n\n Approval Process has been completed successfully.\n PNR No :" + pnrStatus;
                    }
                    else
                    {
                        jsonmsg = pnrStatus;
                    }
                }
                else
                {
                    jsonmsg = "Approved";
                }
            }
            else
                jsonmsg = "Invalid Request.";
            return jsonmsg;
        }


        /// <summary>
        /// Generate PNR and Send Emal Notification
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <param name="emailId"></param>
        /// <returns></returns>
        public async Task<string> GeneratePNR(Int64 travelReqId, string emailId)
        {
            //Generate PNR For Approver or HOD
            var strHoldPNR = ConfigurationManager.AppSettings["strHOLDPNR"].Trim();
            string msg = string.Empty;
            string bookingType = string.Empty;
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

            bookingType = sodRequestsList[0].SodBookingTypeId == 1 ? "SOD" : "NON-SOD";
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
                msg = "ER001";
                var approvalListrollback = new TravelRequestApprovalModels()
                {
                    TravelRequestId = travelReqId,
                    ApprovalStatus = 0,
                    Comment = ""
                };
                _sodApproverRepositorty.RollBackApprovalByHOD(approvalListrollback);
            }
            else
            {
                pnr = pnr + "|Close";
                //Update in database
                var c = _sodApproverRepositorty.UpdatePnr(pnr, travelReqId);
                if (c > 0)
                {
                    //Send Email Notification
                    string emailSubject = bookingType + " " + "Booking Request Approval Notification from HOD :" + System.DateTime.Now.ToString("dd-MMM-yyyy-hh:mm:ss");
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
                    var emailCredentials = EmailCredentials(emailSubject, emailTemplateName, sodRequestsList, sodflightList, passengerList, passengerMealsList, hotelList, travelReqId.ToString());
                    if (!pnr.Equals(string.Empty))
                    {
                        emailCredentials.TemplateFilePath = emailCredentials.TemplateFilePath.Replace("[PNR]", pnr.Split('|')[0].ToString());
                    }

                    //Send Email Notification
                    EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials, sodRequestsList[0].EmailId.Trim());
                    //msg = "Approved successfully.Your PNR No. :" + pnr.Split('|')[0];
                    msg = pnr.Split('|')[0];
                }
            }
            return msg;
        }

        /// <summary>
        /// Initialized Email Credentials to Property
        /// </summary>
        /// <returns></returns>
        public EmailNotificationModel EmailCredentials(string subjectName, string emailTemplateName, List<TravelRequestMasterModels> sodRequestsList, List<FlightDetailModels> sodflightList, List<PassengerDetailModels> sodPassList, List<PassengerMealAllocationModels> sodPassMealsList, List<TravelRequestHotelDetailModels> hotelList, string reqNo)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtpUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtpPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtpServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtpPort"].Trim(),
                TemplateFilePath = ReadFile(emailTemplateName, sodRequestsList, sodflightList, sodPassList, sodPassMealsList, hotelList, reqNo),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }

        /// <summary>
        /// Read Template File from Location
        /// </summary>
        /// <returns></returns>
        private string ReadFile(string emailTemplateName, List<TravelRequestMasterModels> sodRequestsList, List<FlightDetailModels> sodflightList, List<PassengerDetailModels> sodPassList, List<PassengerMealAllocationModels> sodPassMealsList, List<TravelRequestHotelDetailModels> hotelInfo, string reqNo)
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

            tr = tr + "</tr>";

            //Adding Booing Info
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

            if (sodRequestsList[0].IsHotelRequired == true && hotelInfo[0].usercancellation != "Cancelled by User")
            {
                var trh = "";
                if (btype == "SOD")
                {
                    trh = "<table cellpadding='0' cellspacing='0' style='width:100%; font-size:12px;border:solid 1px #c2c2c2;line-height:30px;'>";
                    trh = trh + "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>City</td><td>Check-In Date</td><td>Check-Out Date</td><td>No. Of Passenger</td></tr>";
                    foreach (var h in hotelInfo)
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
        //void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
        //{
        //    if (Session["EmpId"] == null)
        //    {
        //        Response.Clear();
        //        CloseBookingList();
        //    }
        //}

        /// <summary>
        /// Action Filter Method
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
        /// Exception Handling
        /// </summary>
        /// <param name="filterContext"></param>
        void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            Logging.ErrorLog.Instance.AddDBLogging(ex, filterContext.Controller.ControllerContext.ToString(), "Controllers/SodApproverController.cs");
        }
    }
}