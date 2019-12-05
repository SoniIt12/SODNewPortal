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
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace SOD.Controllers
{
    [HandleError(ExceptionType = typeof(DbUpdateException), View = "Error")]
    public class bulkController : Controller, IActionFilter, IExceptionFilter
    {
        /// <summary>
        /// Constructor Initialization : For Repository Design Pattern
        /// </summary>
        private readonly IBulkUploadRepository _bulkUploadRepository;
        private readonly ITransportRepository _transportRepository;

        public bulkController()
        {
            _bulkUploadRepository = new BulkUploadRepository(new SodEntities());
            _transportRepository = new TransportRepository(new SodEntities());
        }

        // GET: bulk
        public ActionResult blkbooking()
        {
            ViewBag.bbbRight = getUserBlanketRight(Session["EmpId"].ToString());
            //Get HOD email ID :18-Feb-2019
            string hodEmailId = SOD.Services.ADO.SodCommonServices.GetHodEmailDetails(Session["EmpId"].ToString().Trim(), 2);
            ViewBag.alternateHODEmailId = hodEmailId.Split(',')[0];
            return View();
        }

        // GET: bulk
        public ActionResult bulkresponse()
        {
            return View("bulkresponse");
        }

        // GET: bulk booking list
        /// <summary>
        /// view bulk booking kList
        /// </summary>
        /// <returns></returns>
        public ActionResult viewbbkList()
        {
            ViewBag.bbbRight = getUserBlanketRight(Session["EmpId"].ToString());
            return View();
        }

        /// <summary>
        /// bbk Selective
        /// </summary>
        /// <returns></returns>
        public ActionResult bbkSelective()
        {
            TempData["BulkBookingRequestId"] = Request.QueryString["bbId"];
            return View();
        }


        /// <summary>
        /// Get Bulk Booking List
        /// </summary>
        /// <returns></returns>
        public ActionResult bulkappList()
        {
            return View("bulkappList");
        }

        /// <summary>
        /// Get bulk booking hotel list
        /// </summary>
        /// <returns></returns>
        public ActionResult bulkhotelList()
        {
            return View("bulkhotelList");
        }

        /// <summary>
        /// Upload File
        /// </summary>
        /// <returns></returns>
        //[HttpPost] 
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public JsonResult UploadFile()
        {
            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {
                var file = System.Web.HttpContext.Current.Request.Files["bulkfile"];
                HttpPostedFileBase filebase = new HttpPostedFileWrapper(file);
                var bulkMaster = new BulkUploadMasterModels();
                var agencyCode = Request.QueryString[0].Trim();
                var bulkList = new List<BulkUploadModels>();
                var empList = new List<string>();
                var bulkhotelList = new List<BulkTravelRequestHotelDetailModels>();
                //Read Excel File
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    //Save File
                    var fileNames = Path.GetFileNameWithoutExtension(filebase.FileName);
                    var fileExt = Path.GetExtension(filebase.FileName);
                    var time = System.DateTime.Now.ToString("yyyyMMddHHss");
                    var path = Path.Combine(Server.MapPath("~/UploadFile/"), fileNames + "_" + time + fileExt);
                    file.SaveAs(path);

                    //Bulk Upload Master 
                    bulkMaster.FileName = fileNames + "_" + time + fileExt;
                    bulkMaster.FilePath = "../UploadFile/";
                    bulkMaster.CreatedById = Session["EmpCode"].ToString().Trim();
                    var UserName = Session["UserInfo"].ToString().Replace("Welcome : ", "").Split('|')[0].ToString();
                    bulkMaster.CreatedByName = UserName;
                    bulkMaster.CreatedDate = System.DateTime.Now;
                    bulkMaster.TransactionDate = System.DateTime.Now;
                    bulkMaster.DepartmentId = Convert.ToInt32(Session["DeptIdCR"]);
                    bulkMaster.VerticalCode = Session["VerticalId"].ToString();
                    bulkMaster.FileStatus = "Open";
                    bulkMaster.BookingType = "bulk";
                    TempData["bulkListMaster"] = bulkMaster;

                    //Bulk Details
                    byte[] fileBytes = new byte[file.ContentLength];

                    if (file.ContentLength == 0)
                    {
                        return Json("Nul");
                    }
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();

                        try
                        {
                            var noOfCol = workSheet.Dimension.End.Column;
                            var noOfRow = workSheet.Dimension.End.Row;

                            for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                            {
                                var bulkUpload = new BulkUploadModels();
                                bulkUpload.SrNo = Convert.ToInt32(workSheet.Cells[rowIterator, 1].Value.ToString());
                                bulkUpload.EmpCode = workSheet.Cells[rowIterator, 2].Value.ToString();

                                bulkUpload.Title = "-";// workSheet.Cells[rowIterator, 3].Value.ToString();
                                bulkUpload.FirstName = "-";//workSheet.Cells[rowIterator, 4].Value.ToString();
                                bulkUpload.LastName = "-";// workSheet.Cells[rowIterator, 5].Value.ToString();
                                bulkUpload.Designation = "-";//// workSheet.Cells[rowIterator, 6].Value.ToString();
                                bulkUpload.Department = "-";//;//workSheet.Cells[rowIterator, 7].Value.ToString();
                                bulkUpload.MobileNo = "-";//workSheet.Cells[rowIterator, 6].Value.ToString();
                                bulkUpload.EmailId = "-";//workSheet.Cells[rowIterator, 7].Value.ToString();

                                bulkUpload.TravelDate = workSheet.Cells[rowIterator, 3].Value.ToString().Trim();
                                bulkUpload.FlightNo = workSheet.Cells[rowIterator, 4].Value.ToString().Trim();
                                bulkUpload.Sector = workSheet.Cells[rowIterator, 5].Value.ToString().Trim();
                                bulkUpload.Purpose = workSheet.Cells[rowIterator, 6].Value.ToString().Trim();

                                bulkUpload.Meal = (workSheet.Cells[rowIterator, 7].Value == "" || workSheet.Cells[rowIterator, 7].Value == null || workSheet.Cells[rowIterator, 7].Value.ToString().ToUpper().Trim() == "NA" || workSheet.Cells[rowIterator, 7].Value.ToString().ToUpper().Trim() == "NO") ? "NA" : workSheet.Cells[rowIterator, 7].Value.ToString();

                                var sdAgencyCode = ConfigurationManager.AppSettings["Bulk_Booking_SOD_SDFINAgencyCode"].ToString().Trim();
                                if (agencyCode == sdAgencyCode)
                                {
                                    bulkUpload.Beverage = (workSheet.Cells[rowIterator, 8].Value == "" || workSheet.Cells[rowIterator, 8].Value == null || workSheet.Cells[rowIterator, 8].Value.ToString().ToUpper().Trim() == "NA" || workSheet.Cells[rowIterator, 8].Value.ToString().ToUpper().Trim() == "NO") ? "NA" : workSheet.Cells[rowIterator, 8].Value.ToString();
                                }
                                else
                                {
                                    bulkUpload.Beverage = "NA";
                                }

                                if (bulkUpload.Meal.ToUpper() == "YES")
                                    bulkUpload.Meal = "VGML";
                                if (bulkUpload.Beverage.ToUpper() == "YES")
                                    bulkUpload.Beverage = "BVG";

                                bulkUpload.PNR = (workSheet.Cells[rowIterator, 9].Value == null || workSheet.Cells[rowIterator, 9].Value == "" ? "" : workSheet.Cells[rowIterator, 9].Value.ToString());
                                bulkUpload.BookingType = workSheet.Cells[rowIterator, 10].Value.ToString().Trim();

                                if (workSheet.Cells[rowIterator, 11].Value.ToString().ToLower().Trim() == "yes")
                                    bulkUpload.IsHotelRequired = true;
                                else
                                    bulkUpload.IsHotelRequired = false;

                                bulkUpload.HotelCity = workSheet.Cells[rowIterator, 12].Value.ToString();
                                if (workSheet.Cells[rowIterator, 13].Value.ToString().ToUpper().Trim() == "NA" || workSheet.Cells[rowIterator, 13].Value == null || workSheet.Cells[rowIterator, 13].Value == "")
                                {
                                    bulkUpload.CheckInDate = new DateTime(1900, 1, 1);
                                }
                                else
                                {
                                    var cInDate = Convert.ToInt32(workSheet.Cells[rowIterator, 13].Value.ToString().Trim().Split('/')[0]);
                                    var cInMonth = Convert.ToInt32(workSheet.Cells[rowIterator, 13].Value.ToString().Trim().Split('/')[1]);
                                    var cInYear = Convert.ToInt32(workSheet.Cells[rowIterator, 13].Value.ToString().Trim().Split('/')[2]);
                                    bulkUpload.CheckInDate = new DateTime(cInYear, cInMonth, cInDate);
                                }
                                if (workSheet.Cells[rowIterator, 14].Value.ToString().ToUpper().Trim() == "NA" || workSheet.Cells[rowIterator, 14].Value == null || workSheet.Cells[rowIterator, 14].Value == "")
                                {
                                    bulkUpload.CheckOutDate = new DateTime(1900, 01, 01);
                                }
                                else
                                {
                                    var cOutDate = Convert.ToInt32(workSheet.Cells[rowIterator, 14].Value.ToString().Trim().Split('/')[0]);
                                    var cOutMonth = Convert.ToInt32(workSheet.Cells[rowIterator, 14].Value.ToString().Trim().Split('/')[1]);
                                    var cOutYear = Convert.ToInt32(workSheet.Cells[rowIterator, 14].Value.ToString().Trim().Split('/')[2]);
                                    bulkUpload.CheckOutDate = new DateTime(cOutYear, cOutMonth, cOutDate);
                                }
                                bulkUpload.CheckinTime = workSheet.Cells[rowIterator, 15].Value.ToString().Trim();
                                bulkUpload.CheckoutTime = workSheet.Cells[rowIterator, 16].Value.ToString().Trim();
                                bulkUpload.AgencyCode = agencyCode;

                                if (workSheet.Cells[rowIterator, 17].Value.ToString().ToLower().Trim() == "yes")
                                {
                                    bulkUpload.AirportTransport = true;
                                }
                                else
                                {
                                    bulkUpload.AirportTransport = false;
                                }
                                bulkList.Add(bulkUpload);
                                empList.Add(bulkUpload.EmpCode);
                            }
                        }
                        catch (NullReferenceException exc)
                        {
                            return Json("empty");
                        }
                    }

                    //Remove Duplicate excel data
                    bulkList = bulkList.GroupBy(o => new { o.EmpCode, o.BookingType, o.Sector, o.TravelDate, o.FlightNo }).Select(o => o.FirstOrDefault()).ToList();
                    empList = empList.Distinct().ToList();

                    //Check Duplicate Booking
                    ValidateDuplicateBulkBooking(bulkList);

                    //Get Employee Details
                    var empDetailsList = new List<EmployeeCodewiseDetailModel>();
                    empDetailsList = _bulkUploadRepository.GetEmployeeCodewiseDetails(empList, 1);
                    if (empDetailsList.Count > 0)
                    {
                        foreach (var blist in bulkList)
                        {
                            foreach (var elist in empDetailsList)
                            {
                                if (blist.EmpCode == elist.EmpCode)
                                {
                                    blist.Title = elist.Gender.Equals("M") ? "Mr" : "Ms";
                                    blist.FirstName = elist.FirstName;
                                    blist.LastName = elist.LastName;
                                    blist.Designation = elist.Designation;
                                    blist.Department = elist.Department;
                                    blist.MobileNo = elist.PhoneNo;
                                    blist.EmailId = elist.EmailId;
                                }
                            }
                        }
                        TempData["bulkList"] = bulkList;
                        TempData["empList"] = empList;
                    }
                }
                else
                {
                    return Json("Nul");
                }
                return Json(bulkList, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("File upload fail...");
            }
        }


        /// <summary>
        /// Generate PNR
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GeneratePNR(List<BulkEmployeeList> elist, List<BulkEmployeeUpdatedList> BUpdateList)
        {
            var pnrList = new List<BulkEmployeeList>();
            if (TempData["bulkList"] != null)
            {
                var rlist = new List<int>();
                var vlist = new List<string>();
                foreach (var item in elist)
                {
                    vlist.Add(item.EmpCode);
                    rlist.Add(item.SrNo);
                }

                List<BulkUploadModels> blist = (List<BulkUploadModels>)TempData["bulkList"];
                BulkUploadMasterModels bmaster = (BulkUploadMasterModels)TempData["bulkListMaster"];
                blist = blist.Where(x => rlist.Contains(x.SrNo)).Distinct().ToList();
                foreach (var b in blist)
                {
                    if (b.EmailId == "" || b.EmailId == "Null")
                    {
                        foreach (var bu in BUpdateList)
                        {
                            if (bu.EmpCode == b.EmpCode)
                            {
                                b.EmailId = bu.EmailId;
                                break;
                            }
                        }
                    }
                    if (b.MobileNo == "" || b.MobileNo == "Null")
                    {
                        foreach (var bu in BUpdateList)
                        {
                            if (bu.EmpCode == b.EmpCode)
                            {
                                b.MobileNo = bu.MobileNo;
                                break;
                            }
                        }
                    }
                }
                //Save Record DB
                var agencyCode = Request.QueryString[0].Trim();
                bmaster.ApproverEmailID = Request.QueryString[1].Trim();
                var s = _bulkUploadRepository.SaveBulkUploadTemp(bmaster, blist);
                Session["Trid"] = s;
                var felist = Validate_VerticalwiseEmployeeList(vlist, agencyCode);
                blist = blist.Where(x => felist.Contains(x.EmpCode)).ToList();
                if (blist.Count > 0)
                {
                    pnrList = CommonWebMethod.NavitaireServicesBulkBooking.Generate_PNR(blist, s.ToString());
                    _bulkUploadRepository.UpdatePNRStatusList(s, pnrList);
                }
            }
            return pnrList != null ? Json(pnrList, JsonRequestBehavior.AllowGet) : Json(pnrList, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Approve Booking Request for HOD
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        public JsonResult ApproveToGeneratePNR_Selective(List<BulkEmployeeList> elist, string travelReqId)
        {
            var jsonmsg = string.Empty;
            var vlist = new List<long>();
            var pnrList = new List<BulkEmployeeList>();
            foreach (var item in elist)
            {
                vlist.Add(item.BTrId);
            }
            var hodId = TempData["EmpIdOfApproval"];
            //var travelReqId = travelReqId;
            var approvalList = new List<BulkBookingRequestApprovalModels>();
            for (var i = 0; i < elist.Count; i++)
            {
                BulkBookingRequestApprovalModels model = new BulkBookingRequestApprovalModels();
                model.TrRequestId = Convert.ToInt64(travelReqId);//Travel Request
                if (Session["EmpId"] != null)
                {
                    model.ApprovedByEmpId = Convert.ToInt32(Session["EmpId"].ToString().Trim());
                }
                else
                {
                    model.ApprovedByEmpId = Convert.ToInt32(hodId);
                }
                model.ApprovalStatus = 1;
                model.IsMandatoryTravel = 0;//Is Mandatory Travel
                model.ApprovalDate = System.DateTime.Now;
                model.Comment = "Approved from HOD";
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
                model.AddNo = elist[i].AddNo;
                model.BReqId = elist[i].BTrId;
                approvalList.Add(model);
            };
            var s = _bulkUploadRepository.ApproveBulkSodBookingRequestSelective(approvalList);
            if (s >= 1)
            {
                pnrList = GeneratePNR_BulkApproval_Selective(vlist, travelReqId);
                if (pnrList.Count > 0)
                    SendMailToUSer_AfterAppOrRejfromHOD(vlist, travelReqId);
            }
            return Json(pnrList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// SendMailToUSer After App Or Rej from HOD
        /// </summary>
        /// <param name="vlist"></param>
        /// <param name="travelReqId"></param>
        public void SendMailToUSer_AfterAppOrRejfromHOD(List<long> vlist, string travelReqId)
        {
            //EMail Noitification
            var dicList = new Dictionary<string, object>();
            var bulkMasterModel = new List<BulkUploadMasterModels>();
            var bulkDetailModel = new List<BulkUploadModels>();
            dicList = _bulkUploadRepository.GetBulkBookingInfoForPNRGenerationSelective(vlist);
            bulkDetailModel = dicList["bulkDetailInfoSelective"] as List<BulkUploadModels>;

            //Bulk Booking Requested User Info
            var bulkUserInfo = GetBulkBookingUserInfo(int.Parse(travelReqId));
            var reqName = bulkUserInfo[2] == "M" ? "Mr." : "Ms." + " " + bulkUserInfo[1];

            //Send Email Notification to user and Hod
            var emailSubject = "SOD Bulk Booking Approval Notification from HOD :" + System.DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss tt");
            var emailTemplateName = "SodBulkBookingRequestApprovalNotificationTemplate_User.html";
            var emailCredentials = EmailCredentials(emailSubject, emailTemplateName, bulkDetailModel, travelReqId.ToString(), reqName);
            TempData["emailData"] = emailCredentials;
            TempData["emailId"] = bulkUserInfo[3];
            sendEmailNotification();
        }

        /// <summary>
        /// Approve Booking Request for HOD Not Selective
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        public JsonResult ApproveToGeneratePNR(string trid)
        {
            var jsonmsg = "";
            var pnrList = new List<BulkEmployeeList>();
            var vlist = new List<string>();
            List<BulkUploadModels>  IsRestList = _bulkUploadRepository.IsEmpRestForApproval(int.Parse(trid));
            var travelReqId = Convert.ToInt64(Request.QueryString["trid"].ToString());            
            var approvalList = new BulkBookingRequestApprovalModels()
            {
                TrRequestId = travelReqId,//Travel Request
                ApprovedByEmpId = Convert.ToInt32(Session["EmpId"]),//Employee Id of approver
                ApprovalStatus = 1,
                IsMandatoryTravel = 0, //Is Mandatory Travel
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
            //Check Duplicate PNR  
            var appStatus = Services.ADO.SodCommonServices.CheckDuplicatePNR(travelReqId, 2);
            if (!appStatus.Equals("0"))
            {
                pnrList = null;
            }
            if (appStatus.Equals("3"))
            {
                var s = _bulkUploadRepository.ApproveBulkSodBookingRequest(approvalList);
                if (s >= 1)
                {
                    pnrList = GeneratePNR_BulkApproval(travelReqId.ToString());
                    var counter = 0;
                    if (pnrList.Count > 0)
                    {
                        foreach (var pnr in pnrList)
                        {
                            if (pnr.PNRStatus.Split('|')[0].ToString() == "ERR001")
                                counter++;
                        }

                        if (counter.Equals(pnrList.Count()))
                        {
                            var approvalListrollback = new BulkBookingRequestApprovalModels()
                            {
                                TrRequestId = Convert.ToInt64(s.ToString()),
                                ApprovalStatus = 0,
                                Comment = ""
                            };
                            _bulkUploadRepository.RollBackApprovalByHOD(approvalListrollback);
                        }
                        if (!counter.Equals(pnrList.Count()) && pnrList.Count > 0)
                        {
                            //SendMailToUSer_AfterApprovalfromHOD(vlist, travelReqId.Split(',')[0]);
                        }
                        SendMailToUSer_AfterApprovalfromHOD(travelReqId);
                    }
                }
            }
            return Json(pnrList, JsonRequestBehavior.AllowGet);
            }


            /// <summary>
            /// Generate PNR and Send Emal Notification
            /// </summary>
            /// <param name="travelReqId"></param>
            /// <param name="emailId"></param>
            /// <returns></returns>
            public List<BulkEmployeeList> GeneratePNR_BulkApproval(string travelReqId)
            {
                string msg = string.Empty;
                var pnrList = new List<BulkEmployeeList>();
                var bulkMasterModel = new List<BulkUploadMasterModels>();
                var bulkDetailModel = new List<BulkUploadModels>();

                var dicList = new Dictionary<string, object>();
                dicList = _bulkUploadRepository.GetBulkBookingInfoForPNRGeneration(Convert.ToInt64(travelReqId));

                bulkMasterModel = dicList["bulkMasterInfo"] as List<BulkUploadMasterModels>;
                bulkDetailModel = dicList["bulkDetailInfo"] as List<BulkUploadModels>;

                //As of now only stand by booking are allowed for HOD Approval 
                var sdAgencyCode = ConfigurationManager.AppSettings["Bulk_Booking_SOD_SDFINAgencyCode"].ToString();
                if (bulkDetailModel[0].AgencyCode == sdAgencyCode)
                    bulkDetailModel = bulkDetailModel.Where(x => x.BookingType.ToLower() == "standby").ToList();

                //Generate PNR
                if (bulkDetailModel.Count > 0)
                {
                    pnrList = NavitaireServicesBulkBooking.Generate_PNR(bulkDetailModel, travelReqId.ToString());
                    var c = _bulkUploadRepository.UpdatePNRStatusList(int.Parse(travelReqId), pnrList);
                }
                return pnrList;
            }



            /// <summary>
            /// Generate PNR and Send Emal Notification Selective Case
            /// </summary>
            /// <param name="travelReqId"></param>
            /// <param name="emailId"></param>
            /// <returns></returns>
            public List<BulkEmployeeList> GeneratePNR_BulkApproval_Selective(List<long> vlist, string travelReqId)
            {
                string msg = string.Empty;
                var pnrList = new List<BulkEmployeeList>();
                //var bulkMasterModel = new List<BulkUploadMasterModels>();
                var bulkDetailModel = new List<BulkUploadModels>();

                var dicList = new Dictionary<string, object>();
                dicList = _bulkUploadRepository.GetBulkBookingInfoForPNRGenerationSelective(vlist);

                //bulkMasterModel = dicList["bulkMasterInfo"] as List<BulkUploadMasterModels>;
                bulkDetailModel = dicList["bulkDetailInfoSelective"] as List<BulkUploadModels>;

                ////As of now only stand by booking are allowed for HOD Approval
                //bulkDetailModel = bulkDetailModel.Where(x => vlist.Contains(x.EmpCode)).ToList();

                ////As of now only stand by booking are allowed for HOD Approval 
                var sdAgencyCode = ConfigurationManager.AppSettings["Bulk_Booking_SOD_SDFINAgencyCode"].ToString();
                if (bulkDetailModel[0].AgencyCode == sdAgencyCode)
                    bulkDetailModel = bulkDetailModel.Where(x => x.BookingType.ToLower() == "standby").ToList();

                //Generate PNR
                if (bulkDetailModel.Count > 0)
                {
                    pnrList = NavitaireServicesBulkBooking.Generate_PNR(bulkDetailModel, travelReqId.ToString());
                    var c = _bulkUploadRepository.UpdatePNRStatusList(int.Parse(travelReqId), pnrList);
                }
                return pnrList;
            }

            /// <summary>
            /// Send mail to User after request approved by HOD
            /// </summary>
            public void SendMailToUSer_AfterApprovalfromHOD(Int64 travelReqId)
            {
                //EMail Noitification
                var dicList = new Dictionary<string, object>();
                dicList = _bulkUploadRepository.GetBulkBookingInfoForPNRGeneration(travelReqId);

                var bulkMasterModel = new List<BulkUploadMasterModels>();
                var bulkDetailModel = new List<BulkUploadModels>();

                bulkMasterModel = dicList["bulkMasterInfo"] as List<BulkUploadMasterModels>;
                bulkDetailModel = dicList["bulkDetailInfo"] as List<BulkUploadModels>;

                //Bulk Booking Requested User Info
                var bulkUserInfo = GetBulkBookingUserInfo(travelReqId);
                var reqName = bulkUserInfo[2] == "M" ? "Mr." : "Ms." + " " + bulkUserInfo[1];

                //Send Email Notification to user and Hod
                var emailSubject = "SOD Bulk Booking Approval Notification from HOD :" + System.DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss tt");
                var emailTemplateName = "SodBulkBookingRequestApprovalNotificationTemplate_User.html";
                var emailCredentials = EmailCredentials(emailSubject, emailTemplateName, bulkDetailModel, travelReqId.ToString(), reqName);
                TempData["emailData"] = emailCredentials;
                TempData["emailId"] = bulkUserInfo[3];
                sendEmailNotification();
            }


        [HttpPost]
        public JsonResult RejectBulkBookingSelective(List<BulkEmployeeList> elist, string travelReqId)
        {
            var jsonmsg = string.Empty;
            TempData["emailData"] = null;
            TempData["emailId"] = null;

            if ((travelReqId == "undefined") || (travelReqId == ""))
                return Json("Invalid record.", JsonRequestBehavior.AllowGet);
            var vlist = new List<long>();
            var pnrList = new List<BulkEmployeeList>();
            foreach (var item in elist)
            {
                vlist.Add(item.BTrId);
            }

            try
            {
                var hodId = TempData["EmpIdOfApproval"];
                var approvalList = new List<BulkBookingRequestApprovalModels>();
                for (var i = 0; i < elist.Count; i++)
                {
                    BulkBookingRequestApprovalModels model = new BulkBookingRequestApprovalModels();
                    model.TrRequestId = Convert.ToInt64(travelReqId);//Travel Request
                    if (Session["EmpId"] != null)
                    {
                        model.ApprovedByEmpId = Convert.ToInt32(Session["EmpId"].ToString().Trim());
                    }
                    else
                    {
                        model.ApprovedByEmpId = Convert.ToInt32(hodId);
                    }
                    model.ApprovalStatus = 2;
                    model.IsMandatoryTravel = 0;//Is Mandatory Travel
                    model.ApprovalDate = System.DateTime.Now;
                    model.Comment = "Rejected from HOD";
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
                    model.AddNo = elist[i].AddNo;
                    model.BReqId = elist[i].BTrId;
                    approvalList.Add(model);
                };
                var s = _bulkUploadRepository.ApproveBulkSodBookingRequestSelective(approvalList);

                if (s >= 1)
                {
                    //var c = _bulkUploadRepository.RejectionCloseBulkBookingRequest(Convert.ToInt64(travelReqId), 1);
                    var dicList = new Dictionary<string, object>();
                    dicList = _bulkUploadRepository.GetBulkBookingInfoForPNRGenerationSelective(vlist);
                    //update rejection Pnr status in bulk booking 
                    var c = _bulkUploadRepository.UpdateStatusOnRejection(vlist, travelReqId, "B");
                    //EMail Noitification
                    var bulkDetailModel = new List<BulkUploadModels>();

                    bulkDetailModel = dicList["bulkDetailInfoSelective"] as List<BulkUploadModels>;

                    //Bulk Booking Requested User Info
                    var bulkUserInfo = GetBulkBookingUserInfo(Convert.ToInt64(travelReqId));
                    var reqName = bulkUserInfo[2] == "M" ? "Mr." : "Ms." + " " + bulkUserInfo[1];

                    //Send Email Notification to user and Hod
                    var emailSubject = "SOD Bulk Booking Rejection Notification from HOD :" + System.DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss tt");
                    var emailTemplateName = "SodBulkBookingRequest_Rejection_User.html";
                    var emailCredentials = EmailCredentials(emailSubject, emailTemplateName, bulkDetailModel, travelReqId, reqName);
                    TempData["emailData"] = emailCredentials;
                    TempData["emailId"] = bulkUserInfo[3];
                }
                else
                {
                    jsonmsg = "Error :Rejection process has not been completed successfully.";
                }
            }
            catch (Exception ex)
            {
                jsonmsg = jsonmsg + "\n" + ex.Message.ToString();
            }
            return Json(jsonmsg, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Reject Bulk Booking Not Selective Option
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult RejectBulkBookingNotSelective(string trId)
        {
            var jsonmsg = string.Empty;
            try
            {
                TempData["emailData"] = null;
                TempData["emailId"] = null;

                if ((trId == "undefined") || (trId == ""))
                    return Json("Invalid record.", JsonRequestBehavior.AllowGet);

                var approvalList = new BulkBookingRequestApprovalModels()
                {
                    TrRequestId = Convert.ToInt64(trId),//Travel Request
                    ApprovedByEmpId = Convert.ToInt32(Session["EmpId"].ToString()),//Employee Id of Rejecter
                    ApprovalStatus = 2,
                    IsMandatoryTravel = 0,
                    ApprovalDate = System.DateTime.Now,
                    Comment = "Rejected by HOD",
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

                //Check Duplicate PNR  
                var appStatus = Services.ADO.SodCommonServices.CheckDuplicatePNR(Convert.ToInt64(trId), 2);
                if (appStatus.Equals("1"))
                    return Json("Sorry : Approval Process has been already completed.", JsonRequestBehavior.AllowGet);
                if (appStatus.Equals("2"))
                    return Json("Sorry : Rejection Process has been already completed.", JsonRequestBehavior.AllowGet);

                var s = _bulkUploadRepository.RejectSodBulkBookingRequest(approvalList);
                if (s >= 1)
                {
                    jsonmsg = "Rejection process has been completed successfully for Bulk Booking Request No. -" + trId;
                    var c = _bulkUploadRepository.RejectionCloseBulkBookingRequest(Convert.ToInt64(trId), 1);

                    //EMail Noitification
                    var dicList = new Dictionary<string, object>();
                    dicList = _bulkUploadRepository.GetBulkBookingInfoForPNRGeneration(Convert.ToInt64(trId));

                    var bulkMasterModel = new List<BulkUploadMasterModels>();
                    var bulkDetailModel = new List<BulkUploadModels>();

                    bulkMasterModel = dicList["bulkMasterInfo"] as List<BulkUploadMasterModels>;
                    bulkDetailModel = dicList["bulkDetailInfo"] as List<BulkUploadModels>;

                    //Bulk Booking Requested User Info
                    var bulkUserInfo = GetBulkBookingUserInfo(Convert.ToInt64(trId));
                    var reqName = bulkUserInfo[2] == "M" ? "Mr." : "Ms." + " " + bulkUserInfo[1];

                    //Send Email Notification to user and Hod
                    var emailSubject = "SOD Bulk Booking Rejection Notification from HOD :" + System.DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss tt");
                    var emailTemplateName = "SodBulkBookingRequest_Rejection_User.html";
                    var emailCredentials = EmailCredentials(emailSubject, emailTemplateName, bulkDetailModel, trId, reqName);
                    TempData["emailData"] = emailCredentials;
                    TempData["emailId"] = bulkUserInfo[3];
                }
                else
                {
                    jsonmsg = "Error :Rejection process has not been completed successfully.";
                }
            }
            catch (Exception ex)
            {
                jsonmsg = jsonmsg + "\n" + ex.Message.ToString();
            }
            return Json(jsonmsg, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Get Sod Booking Data
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetuploadedData()
        {
            return TempData["bulkList"] != null
                ? Json(TempData["bulkList"], JsonRequestBehavior.AllowGet)
                : Json("0", JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Validate Employee from DB
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ValidateEmployeeList()
        {
            var s = new List<string>();
            if (TempData["empList"] != null)
            {
                List<string> elist = (List<string>)TempData["empList"];
                s = _bulkUploadRepository.ValidateEmployeeCode(elist);
                TempData["vList"] = s;
            }
            return s != null ? Json(s, JsonRequestBehavior.AllowGet) : Json(s, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Get Master Bulk Booking List Data
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetMasterBulkList()
        {
            var fromdate = Convert.ToDateTime(Request.QueryString["prm"].Split(',')[0]);
            var todate = Convert.ToDateTime(Request.QueryString["prm"].Split(',')[1]);
            var DeptId = Convert.ToInt32(Session["DeptIdCR"]);
            var BookingType = Request.QueryString["BookingType"].ToString();
            var VerticalCode = Session["VerticalId"] == null ? "" : Session["VerticalId"].ToString();

            var flag = Request.QueryString["prm"].Split(',')[2];
            if (flag == "1")
                return Json(_bulkUploadRepository.GetBulkBookingMasterData(fromdate, todate, DeptId, VerticalCode, BookingType), JsonRequestBehavior.AllowGet);
            else
                return Json(_bulkUploadRepository.GetBulkBookingPNRWiseDetails(fromdate, todate, "", 1), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Get Bulk Upload Data detaikls
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetDetailsBulkList()
        {
            return Json(_bulkUploadRepository.GetBulkBookinDetailsData(Request.QueryString["TrId"].Trim()), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get hotel details for a travelRequestId
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetHotelListPopup()
        {
            return Json(_bulkUploadRepository.GetHotelListPopup(Request.QueryString["TrId"].Trim()), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Update Bulk Detail
        /// </summary>
        /// <param name="oldEcode"></param>
        /// <param name="upDatedrow"></param>
        /// <param name="trid"></param>
        /// <returns></returns>
        public JsonResult UpdateBulkDetail(string oldEcode, List<BulkUploadModels> upDatedrow, Int64 trid)
        {
            var HotelDetails = _bulkUploadRepository.GetHotelDetailForMail(oldEcode, trid.ToString());
            Int64 response = _bulkUploadRepository.UpdateBulkDetail(oldEcode, upDatedrow, trid);
            if (response != 0)
            {
                if (HotelDetails.Count > 0)
                {
                    var sharedUserList = new List<BulkUploadModels>();
                    sharedUserList = _bulkUploadRepository.GetSharedUserDetails(HotelDetails[0].sharingId, HotelDetails[0].clubId, oldEcode, trid.ToString());
                    // if  hotel status is approved send mail to hotel
                    var emailSubject = "SOD Hotel Accommodation Update from Spicejet:" + System.DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt");
                    var emailTemplateName = "HotelAccomodationUpdationTemplate.html";
                    var emailCredentials = EmailCredentialsHotelsOnUpdation(emailSubject, emailTemplateName, Int64.Parse(oldEcode), HotelDetails[0].clubId, HotelDetails[0].HotelName, sharedUserList, upDatedrow);
                    _bulkUploadRepository.UpdateHotelDetails(oldEcode, upDatedrow, trid);
                    var templateData = emailCredentials.TemplateFilePath;
                    templateData = templateData.Replace("[hotelName]", "<b>" + HotelDetails[0].HotelName + "</b>");
                    templateData = templateData.Replace("[remarksTravelDesk]", " ");
                    templateData = templateData.Replace("[hinfo]", "Replacement For");
                    var citycode = _transportRepository.GetCityCodeOfHotel(HotelDetails[0].HotelCode);
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
                    emailCredentials.TemplateFilePath = templateData;
                    TempData["emailData_Hod"] = emailCredentials;
                    TempData["emailId_Hod"] = HotelDetails[0].PrimaryEmail;
                    EmailNotifications.SendBookingRequestNotificationTo_Requester_Traveldesk(emailCredentials, HotelDetails[0].PrimaryEmail);
                }
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Initialized Email Credentials For updation
        /// </summary>
        /// <returns></returns>
        public EmailNotificationModel EmailCredentialsHotelsOnUpdation(string subjectName, string emailTemplateName, Int64 EmpCode, Int32 clubid, string hotelname, List<BulkUploadModels> sharedUserList, List<BulkUploadModels> upDatedrow)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtptdUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtptdPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtptdServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtptdPort"].Trim(),
                TemplateFilePath = ReadFileHotelRequestOnUpdation(emailTemplateName, EmpCode, clubid, hotelname, sharedUserList, upDatedrow),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }

        /// <summary>
        /// Read File Hotel Request On Updation
        /// </summary>
        /// <param name="emailTemplateName"></param>
        /// <param name="EmpCode"></param>
        /// <param name="clubid"></param>
        /// <param name="hotelname"></param>
        /// <param name="sharedUserList"></param>
        /// <param name="upDatedrow"></param>
        /// <returns></returns>
        private string ReadFileHotelRequestOnUpdation(string emailTemplateName, Int64 EmpCode, Int32 clubid, string hotelname, List<BulkUploadModels> sharedUserList, List<BulkUploadModels> upDatedrow)
        {
            var strContent = new StringBuilder();
            string line;
            var file = new System.IO.StreamReader(
                       Server.MapPath("~/Template/BulkTemplate/" + emailTemplateName));
            while ((line = file.ReadLine()) != null)
            {
                strContent.Append(line);
            }
            file.Close();
            strContent = strContent.Replace("[binfo]", "Booking Details - " + upDatedrow[0].TrnId + " To be Replaced");

            GetBulkHotelRequestData(EmpCode, clubid, hotelname, 2);
            var passengerInfo = TempData["InfoForHotelRequest"] as List<TravelRequestModels>;
            var count = 1;
            var gender = "";
            var airtransport = "";
            if (passengerInfo.Count == 1)
            {
                strContent = strContent.Replace("[occupancy]", "Single Occupancy");
            }
            else
            {
                strContent = strContent.Replace("[occupancy]", "Double Occupancy based on Sharing Id");
            }
            var tr1 = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;font-weight:bold;'>" +
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
                tr1 = tr1 + "<td height:20px; padding-bottom:8px;'>Sharing With</td></tr>";
            }
            else
            {
                tr1 = tr1 + "</tr>";
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

                tr1 = tr1 + "<tr style='font-family:Arial; font-size:12px;'>" +
                        "<td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + count +
                        "</td><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + gender +
                        "</td><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.RequestedEmpName +
                        "</td><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.RequestedEmpCode +
                        "</td><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.CheckinDate.ToString("dd-MMM-yyyy") +
                        "</td><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.CheckoutDate.ToString("dd-MMM-yyyy") +
                        "</td><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + airtransport +
                        "</td><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.FlightNo +
                        "</td><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.CheckinTime +
                        "</td><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.Phone;

                if (sharedUserList.Count > 0)
                {
                    tr1 = tr1 + "</td><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + "Employee Code: " + sharedUserList[0].EmpCode + "<br>Name: " + sharedUserList[0].FirstName + " " + sharedUserList[0].LastName + "<br>Phone No.: " + sharedUserList[0].MobileNo + "</td></tr>";
                }
                else
                {
                    tr1 = tr1 + "</td ></tr>";
                }
            }
            strContent = strContent.Replace("[trI]", tr1);
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
              "<td style='height:20px; padding-bottom:8px;'>Mobile No.</td>" + "</tr>";
            foreach (var p in upDatedrow)
            {
                if (p.Title == "Ms")
                {
                    gender = "F";
                }
                else
                {
                    gender = "M";
                }

                tr = tr + "<tr style='font-family:Arial; font-size:12px;'>" +
                        "<td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + count +
                        "</td><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + gender +
                        "</td><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.FirstName + "" + p.LastName +
                        "</td><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.EmpCode +
                        "</td><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.CheckInDate.ToString("dd-MMM-yyyy") +
                        "</td><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.CheckOutDate.ToString("dd-MMM-yyyy") +
                        "</td><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + airtransport +
                        "</td><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.FlightNo +
                        "</td><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.CheckinTime +
                        "</td><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.MobileNo + "</td ></tr>";
            }
            strContent = strContent.Replace("[tr]", tr);

            return strContent.ToString();
        }

        /// <summary>
        /// Cancel Booking Request
        /// </summary>
        /// <param name="empcode"></param>
        /// <param name="trid"></param>
        /// <returns></returns>
        public JsonResult CancelBookingRequest(String empcode, Int64 trid, String ReasonForCancellation)
        {
            int response = _bulkUploadRepository.CancelBookingRequest(empcode, trid, ReasonForCancellation);
            if (response != 0)
            {
                var HotelDetails = _bulkUploadRepository.GetHotelDetailForMail(empcode, trid.ToString());

                if (HotelDetails.Count > 0 && HotelDetails[0].HotelStatus == "Approved by Hotel")
                {
                    var sharedUserList = new List<BulkUploadModels>();
                    sharedUserList = _bulkUploadRepository.GetSharedUserDetails(HotelDetails[0].sharingId, HotelDetails[0].clubId, empcode, trid.ToString());
                    // if  hotel status is approved send mail to hotel
                    var emailSubject = "SOD Hotel Cancellation Request Notification from SpiceJet:" + System.DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt");
                    var emailTemplateName = "CancellationApprovalHotelTemplate.html";
                    var emailCredentials = EmailCredentialsHotelRequestOnCancel(emailSubject, emailTemplateName, Convert.ToInt64(empcode), HotelDetails[0].clubId, HotelDetails[0].HotelName, sharedUserList);
                    var uri1 = "";
                    var skey = new StringBuilder();
                    skey.Append(empcode.ToString());
                    var appLink = string.Empty;
                    var senderEmpCode = Session["EmpId"].ToString().Trim();
                    uri1 = ConfigurationManager.AppSettings["emailApprovalPathHotel"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=K&trid=" + trid.ToString() + "&hotelname=" + HotelDetails[0].HotelName + "&confirmNO =" + HotelDetails[0].HotelConfirmationNo + "&SenderEmp=" + senderEmpCode);

                    appLink = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Acceptance</a></td></tr></table>";

                    var templateData = emailCredentials.TemplateFilePath;
                    templateData = templateData.Replace("[appLink]", appLink);
                    templateData = templateData.Replace("[hotelName]", HotelDetails[0].HotelName);
                    emailCredentials.TemplateFilePath = templateData;
                    TempData["emailData_Hod"] = emailCredentials;
                    TempData["emailId_Hod"] = HotelDetails[0].PrimaryEmail;
                    EmailNotifications.SendBookingRequestNotificationTo_Requester_Traveldesk(emailCredentials, HotelDetails[0].PrimaryEmail);
                }

            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// fetch details
        /// </summary>
        /// <param name="newECode"></param>
        /// <param name="trid"></param>
        /// <returns></returns>
        public ActionResult fetchdetails(string newECode, Int64 trid)
        {
            //var deptartmentId = Convert.ToInt32(Session["DeptIdCR"]);
            var deptartmentId = 0; //to allow to all department
            var check_duplicacy = _bulkUploadRepository.check_duplicacy(newECode, trid);
            if (check_duplicacy == "Exist")
                return new ContentResult { Content = check_duplicacy, ContentType = "application/json" };
            else
                return Json(_bulkUploadRepository.fetchdetails(newECode, deptartmentId), JsonRequestBehavior.AllowGet);
        }


        //public JsonResult hotelCancellationRequestUser(string travelRqstId)
        //{
        //    var travReqstId = travelRqstId.Split('|')[0];
        //    var empCode = travelRqstId.Split('|')[1];
        //    var clubId = Convert.ToInt32( travelRqstId.Split('|')[2]);
        //    var trid = Convert.ToInt64(travReqstId);
        //    string hotelname = "";
        //    var primaryEmail = "";
        //    var dicListHotel = new Dictionary<string, object>();
        //    var hoteldetails = new List<BulkTravelRequestHotelDetailModels>();
        //    dicListHotel = _bulkUploadRepository.SaveCancellationRequestUser(travReqstId, empCode);
        //    hoteldetails = dicListHotel["approvalHotelDetails"] as List<BulkTravelRequestHotelDetailModels>;
        //    if (hoteldetails.Count > 0)
        //    {
        //        hotelname = hoteldetails[0].HotelName;
        //        primaryEmail = hoteldetails[0].PrimaryEmail;
        //    }
        //    try
        //    {
        //        List<String> listhotelInfo = new List<String>();
        //        //listhotelInfo.Add(travReqstId);                
        //        var dicList = new Dictionary<string, object>();
        //        dicList = _bulkUploadRepository.SaveCancellationRequestUser(travReqstId, empCode);
        //        var hotelCancellationInfo = dicList["approvalHotelDetails"] as List<BulkTravelRequestHotelDetailModels>;

        //        var username = hotelCancellationInfo[0].EmployeeCode;
        //        var useremail = hotelCancellationInfo[0].PrimaryEmail;

        //        if (primaryEmail.Length > 0 && useremail.Length > 0)
        //        {
        //            //send cancellation request to hotel
        //            var emailSubject = "SOD Hotel Cancellation Request Notification from Spicejet:" + System.DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt");
        //            var emailTemplateName = "HotelBookingRequestNotificationTemplate_Cancellation.html";
        //            var emailCredentials = EmailCredentialsHotelRequest(emailSubject, emailTemplateName, trid, clubId, hotelname,"hotel");
        //            var templateData = emailCredentials.TemplateFilePath;
        //            templateData = templateData.Replace("[hotelName]", hotelname);
        //            emailCredentials.TemplateFilePath = templateData;
        //            //TempData["emailData_Hod"] = emailCredentials;
        //            //TempData["emailId_Hod"] = primaryEmail;
        //            EmailNotifications.SendBookingRequestNotificationTo_Requester_Traveldesk(emailCredentials, primaryEmail);
        //            //TempData["msgResponse"] = "Hotel Request for Travel Request Id " + travReqstId.ToString() + " : Hotel cancellation request has been sent successfully. The request has been sent to the respected hotel at  " + primaryEmail;

        //            //send cancellation mail to user
        //            var emailSubject2 = "Hotel Cancellation Request Notification from Travel Desk:" + System.DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt");
        //            var emailTemplateName2 = "HotelBookingRequestNotificationTemplate_Cancellation.html";
        //            var emailCredentials2 = EmailCredentialsHotelRequest(emailSubject2, emailTemplateName2, trid, clubId, hotelname,"user");
        //            var templateData2 = emailCredentials2.TemplateFilePath;
        //            templateData2 = templateData2.Replace("[hotelName]", username);
        //            emailCredentials2.TemplateFilePath = templateData2;
        //            //TempData["emailData_Hod"] = emailCredentials2;
        //            //TempData["emailId_Hod"] = useremail;
        //            EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials2, useremail);

        //        }
        //        return Json("RequestSent", JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //      return Json("Failed", JsonRequestBehavior.AllowGet);
        //    }
        //}


        /// <summary>
        /// View traveldesk hotel status
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetViewstatushotel()
        {
            var s = _bulkUploadRepository.GetViewstatushotel(Request.QueryString["TrId"].Trim());
            if (s == 1)
            {
                return Json("pending", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("close", JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// approve hotel request
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AppHotelRequest(string empcode, string TravelRequestId)
        {
            var s = 0;
            var clubList = _bulkUploadRepository.FindClubid(empcode, TravelRequestId);
            for (var i = 0; i < clubList.Count; i++)
            {
                var employeecode = clubList[i].EmployeeCode.Trim();
                s = _bulkUploadRepository.ApprovedHotelRequest(employeecode, TravelRequestId);
                GetHotelApprovalNotificationData(employeecode, TravelRequestId);
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Resend mail to user
        /// </summary>
        /// <param name="empcode"></param>
        /// <param name="TravelRequestId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AppHotelRequestResend(string empcode, string TravelRequestId)
        {
            var s = _bulkUploadRepository.ApprovedHotelRequest(empcode, TravelRequestId);
            GetHotelApprovalNotificationData(empcode, TravelRequestId);
            return Json(s, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// get hotel info for user mail
        /// </summary>
        /// <param name="empcode"></param>
        /// <param name="travelrequestid"></param>
        public void GetHotelApprovalNotificationData(string empcode, string travelrequestid)
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
                var emailSubject = "SOD Hotel Booking Confirmation Notification from Travel Desk :" + System.DateTime.Now.ToString();
                var emailTemplateName = "SodHotelBookingRequestNotificationTemplateFor_TravelDesk.html";
                var emailCredentials = EmailCredentialsHotel(emailSubject, emailTemplateName, hotelList, userList, sharedUserList, "approved");
                TempData["emailData"] = emailCredentials;
                TempData["emailId"] = userList[0].EmailId.Trim();
                sendEmailNotification();
            }
        }


        /// <summary>
        /// Export data in an Excel Format
        /// </summary>
        public void Exportexcel()
        {
            if (Session["Trid"] != null)
            {
                var bulkData = _bulkUploadRepository.GetCurrentBulkBookingData_ExportToExcel(Convert.ToInt32(Session["Trid"]));
                System.Reflection.PropertyInfo[] _modelProperties = typeof(BulkUploadModels).GetProperties();
                OfficeOpenXml.ExcelPackage excel = new OfficeOpenXml.ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells[1, 1].LoadFromCollection(bulkData, true);
                for (var i = 0; i < _modelProperties.Length; i++)
                {
                    var canSetFormat = System.Globalization.DateTimeFormatInfo.CurrentInfo != null && (_modelProperties[i].PropertyType == typeof(DateTime));
                    if (canSetFormat)
                    {
                        workSheet.Column(i + 1).Style.Numberformat.Format = "dd-MMM-yyyy HH:mm AM/PM";
                    }
                }
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=SPJ_BulkBooking.xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
            else return;
        }


        /// <summary>
        /// Export data in an Excel Format : Transactionwise
        /// </summary>
        public void ExportexcelTrans()
        {
            if (Request.QueryString[0] != null || Request.QueryString[0] != "")
            {
                var bulkData = _bulkUploadRepository.GetCurrentBulkBookingData_ExportToExcel(Convert.ToInt32(Request.QueryString["TrId"]));
                System.Reflection.PropertyInfo[] _modelProperties = typeof(BulkUploadModels).GetProperties();
                OfficeOpenXml.ExcelPackage excel = new OfficeOpenXml.ExcelPackage();
                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells[1, 1].LoadFromCollection(bulkData, true);
                for (var i = 0; i < _modelProperties.Length; i++)
                {
                    var canSetFormat = System.Globalization.DateTimeFormatInfo.CurrentInfo != null && (_modelProperties[i].PropertyType == typeof(DateTime));
                    if (canSetFormat)
                    {
                        workSheet.Column(i + 1).Style.Numberformat.Format = "dd-MMM-yyyy HH:mm AM/PM";
                    }
                }
                using (var memoryStream = new MemoryStream())
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;  filename=SPJ_BulkBooking.xlsx");
                    excel.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
            else return;
        }


        /// <summary>
        /// Download excelsheet PNRwise
        /// </summary>
        public void ExportexcelPNRwise()
        {
            if (Request.QueryString[0] != null || Request.QueryString[0] != "")
            {
                var fromdate = Convert.ToDateTime(Request.QueryString["prm"].Split(',')[0]);
                var todate = Convert.ToDateTime(Request.QueryString["prm"].Split(',')[1]);
                var flag = Request.QueryString["prm"].Split(',')[2];
                if (flag == "2")
                {
                    var bulkData = _bulkUploadRepository.GetBulkBookingPNRWiseDetails_ExportToExcel(fromdate, todate, "", 1);
                    System.Reflection.PropertyInfo[] _modelProperties = typeof(BulkUpload_ExcelExportPNRWise).GetProperties();
                    OfficeOpenXml.ExcelPackage excel = new OfficeOpenXml.ExcelPackage();
                    var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                    workSheet.Cells[1, 1].LoadFromCollection(bulkData, true);
                    for (var i = 0; i < _modelProperties.Length; i++)
                    {
                        var canSetFormat = System.Globalization.DateTimeFormatInfo.CurrentInfo != null && (_modelProperties[i].PropertyType == typeof(DateTime));
                        if (canSetFormat)
                        {
                            workSheet.Column(i + 1).Style.Numberformat.Format = "dd-MMM-yyyy HH:mm AM/PM";
                        }
                    }
                    using (var memoryStream = new MemoryStream())
                    {
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;  filename=SPJ_BulkBookingPNRWise.xlsx");
                        excel.SaveAs(memoryStream);
                        memoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
            }
            else return;
        }


        /// <summary>
        /// Validate Employee Verticales List
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public List<string> Validate_VerticalwiseEmployeeList(List<string> vlist, string verticalCode)
        {
            var s = _bulkUploadRepository.ValidateEmployeeVerticals(vlist, verticalCode);
            return s.ToList<string>();
        }

        /// <summary>
        /// Get Agency Code rights
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetAgencyCode()
        {
            return Json(_bulkUploadRepository.GetEmployeeBookingAgencyRight(Session["EmpCode"].ToString().Trim(), 1), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Get Agency Code rights
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetDepartment()
        {
            return Json(_bulkUploadRepository.GetEmployeeDepartmentRight(Session["EmpCode"].ToString().Trim(), 1), JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// Get Bulk Booking Blanket Rights Approvels
        /// </summary>
        /// <param name="EmpId"></param>
        /// <returns></returns>
        private int getUserBlanketRight(string empId)
        {
            string[] bulkArray = ConfigurationManager.AppSettings["Bulk_Booking_Blanket_RightId"].Split(',');
            int index = Array.IndexOf(bulkArray, empId);
            return index > -1 ? 1 : 0;

        }

        [HttpPost]
        public JsonResult saveBulk_newRow(List<BulkUploadModels> DetalsList)
        {
            //Check Duplicate Booking
            // ValidateDuplicateBulkBooking(DetalsList);

            //Get Employee Details
            var empDetailsList = new List<EmployeeCodewiseDetailModel>();
            List<string> Empcode = new List<string>(DetalsList[0].EmpCode.Split(','));
            empDetailsList = _bulkUploadRepository.GetEmployeeCodewiseDetails(Empcode, 1);
            if (empDetailsList.Count != 0)
            {
                DetalsList[0].Title = empDetailsList[0].Gender.Equals("M") ? "Mr" : "Ms";
                DetalsList[0].FirstName = empDetailsList[0].FirstName;
                DetalsList[0].LastName = empDetailsList[0].LastName;
                DetalsList[0].Designation = empDetailsList[0].Designation;
                DetalsList[0].Department = empDetailsList[0].Department;
                DetalsList[0].MobileNo = empDetailsList[0].PhoneNo;
                DetalsList[0].EmailId = empDetailsList[0].EmailId;

            }

            int response = _bulkUploadRepository.saveBulk_newRow(DetalsList);
            return Json(response, JsonRequestBehavior.AllowGet);

        }

        public JsonResult sendReqtoHOD_edit(List<BulkEmployeeList> blist, List<BulkUploadModels> DetalsList)
        {
            var bulkMaster = new BulkUploadMasterModels();
            var empList = new List<string>();
            var bulkhotelList = new List<BulkTravelRequestHotelDetailModels>();
            var BulkBookingId = blist[0].BTrId;
            var addNo = _bulkUploadRepository.FindAddNo(blist[0].BTrId);
            var empDetailsList = new List<EmployeeCodewiseDetailModel>();
            for (var i = 0; i < DetalsList.Count; i++)
            {
                empList.Add(DetalsList[i].EmpCode);
            }
            empDetailsList = _bulkUploadRepository.GetEmployeeCodewiseDetails(empList, 1);
            if (empDetailsList.Count > 0)
            {
                foreach (var list in DetalsList)
                {
                    foreach (var elist in empDetailsList)
                    {
                        if (list.EmpCode == elist.EmpCode)
                        {
                            list.Title = elist.Gender.Equals("M") ? "Mr" : "Ms";
                            list.FirstName = elist.FirstName;
                            list.LastName = elist.LastName;
                            list.Designation = elist.Designation;
                            list.Department = elist.Department;
                            list.MobileNo = elist.PhoneNo;
                            list.EmailId = elist.EmailId;
                            list.AddNo = addNo + 1;
                        }
                    }
                }
                TempData["bulkList"] = DetalsList;
                TempData["empList"] = empList;
            }
            var s = 0;
            try
            {
                var vlist = new List<string>();
                foreach (var item in blist)
                {
                    vlist.Add(item.EmpCode);
                }
                s = _bulkUploadRepository.saveBulk_newRow(DetalsList);

                //Get HOD Details
                string hodEmailId = SOD.Services.ADO.SodCommonServices.GetHodEmailDetailsBulkBooking(DetalsList[0].TrnId, 1);

                //Logged User Info
                var loggedUserInfo = GetLoginUserInfo(Session["EmpId"].ToString().Trim());
                var reqName = loggedUserInfo[2] == "M" ? "Mr." : "Ms.";
                reqName = reqName + " " + loggedUserInfo[1];

                //Send Email Notification to user and Hod
                var emailSubject = "SOD Bulk Booking Request Notification :" + System.DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss tt");
                var emailTemplateName = "SodBulkBookingRequestNotificationTemplate_User.html";
                var emailCredentials = EmailCredentials(emailSubject, emailTemplateName, DetalsList, BulkBookingId.ToString(), reqName);

                TempData["emailData"] = emailCredentials;
                TempData["emailId"] = loggedUserInfo[3];

                //Update Hod Email Template
                var emailTemplateName_hod = "SodBulkBookingRequestNotificationTemplate_HOD.html";
                emailCredentials = EmailCredentials(emailSubject, emailTemplateName_hod, DetalsList, BulkBookingId.ToString(), reqName);

                if (hodEmailId.Length > 0)
                {
                    var skey = new StringBuilder();
                    skey.Append(BulkBookingId.ToString() + ",");
                    skey.Append(hodEmailId.Split(',')[2] + ",");

                    var uri1 = ConfigurationManager.AppSettings["emailBulkApprovalPath"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=a" + "&edit=" + (addNo + 1));
                    var uri2 = ConfigurationManager.AppSettings["emailBulkApprovalPath"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=r" + "&edit=" + (addNo + 1));
                    var uri3 = ConfigurationManager.AppSettings["emailBulkApprovalPath"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=s" + "&bbID=" + BulkBookingId);
                    //var essurl = ConfigurationManager.AppSettings["essportalUrl"].Trim();       
                    var appLink = string.Empty;
                    appLink = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a name='app' style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Acceptance</a></td>";
                    appLink = appLink + "<td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a name='mapp' style='color:#fff; text-decoration:none;' href='" + uri2 + "'>Rejection</a> </td>";
                    appLink = appLink + "<td style='width:110px; height:25px; background-color:#ffa500;text-align:center;border-radius:5px'><a name='rej' style='color:#fff; text-decoration:none;' href='" + uri3 + "'>Selective</a></td> </tr></table>";

                    var templateData = emailCredentials.TemplateFilePath;
                    var hodName = hodEmailId.Split(',')[1];

                    templateData = templateData.Replace("[appLink]", appLink);
                    templateData = templateData.Replace("[hodName]", hodName);
                    emailCredentials.TemplateFilePath = templateData;

                    TempData["emailData_Hod"] = emailCredentials;
                    TempData["emailId_Hod"] = hodEmailId.Split(',')[0];
                    sendEmailNotification();
                    TempData["msgResponse"] = "SOD-Bulk Booking Request No. " + BulkBookingId.ToString() + " : has been completed successfully.Your booking request has been sent to your concerned HOD for approval at  " + hodEmailId.Split(',')[0].ToString();

                    //SMS Approval
                    SendSMSforApproval(BulkBookingId.ToString(), hodEmailId, reqName, uri1, uri2, uri3);
                }
            }
            catch (Exception ex)
            {
                TempData["fun1"] = ex.Message + " ---" + ex.Source + "---- " + ex.InnerException.Message;
            }
            return Json(s.ToString(), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Send Request to HOD
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SendRequesttoHOD(List<BulkEmployeeList> blist, List<BulkEmployeeUpdatedList> BUpdateList)
                                       {
            var s = 0;
            try
            {
                if (TempData["bulkListMaster"] != null)
                {
                    var vlist = new List<int>();
                    foreach (var item in blist)
                    {
                        vlist.Add(item.SrNo);
                    }

                    List<BulkUploadModels> blists = (List<BulkUploadModels>)TempData["bulkList"];
                    BulkUploadMasterModels bmaster = (BulkUploadMasterModels)TempData["bulkListMaster"];
                    blists = blists.Where(x => vlist.Contains(x.SrNo)).ToList();
                    foreach (var b in blists)
                    {
                        if (b.EmailId == "" || b.EmailId == "Null")
                        {
                            foreach (var bu in BUpdateList)
                            {
                                if (bu.EmpCode == b.EmpCode)
                                {
                                    b.EmailId = bu.EmailId;
                                    break;
                                }
                            }
                        }
                        if (b.MobileNo == "" || b.MobileNo == "Null")
                        {
                            foreach (var bu in BUpdateList)
                            {
                                if (bu.EmpCode == b.EmpCode)
                                {
                                    b.MobileNo = bu.MobileNo;
                                    break;
                                }
                            }
                        }
                    }

                    //Get HOD Details
                    string hodEmailId = "";
                    if (Request.QueryString[1] == "")
                        hodEmailId = SOD.Services.ADO.SodCommonServices.GetHodEmailDetails(Session["EmpId"].ToString().Trim(), 2);
                    else
                        hodEmailId = SOD.Services.ADO.SodCommonServices.GetHodEmailDetails(Request.QueryString[1].Trim(), 4);
                    if (hodEmailId.Length > 0)
                    {
                        bmaster.ApproverEmailID = hodEmailId.Split(',')[0].ToString();
                        s = _bulkUploadRepository.SaveBulkUploadTemp(bmaster, blists);
                    }
                    else return Json(s.ToString(), JsonRequestBehavior.AllowGet);

                    //Logged User Info
                    var loggedUserInfo = GetLoginUserInfo(Session["EmpId"].ToString().Trim());
                    var reqName = loggedUserInfo[2] == "M" ? "Mr." : "Ms." + " " + loggedUserInfo[1];

                    //Send Email Notification to user and Hod
                    var emailSubject = "SOD Bulk Booking Request Notification :" + System.DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss tt");
                    var emailTemplateName = "SodBulkBookingRequestNotificationTemplate_User.html";
                    var emailCredentials = EmailCredentials(emailSubject, emailTemplateName, blists, s.ToString(), reqName);

                    TempData["emailData"] = emailCredentials;
                    TempData["emailId"] = loggedUserInfo[3];

                    //Update Hod Email Template
                    var emailTemplateName_hod = "SodBulkBookingRequestNotificationTemplate_HOD.html";
                    emailCredentials = EmailCredentials(emailSubject, emailTemplateName_hod, blists, s.ToString(), hodEmailId.Split(',')[0].ToString());

                    if (hodEmailId.Length > 0)
                    {
                        var skey = new StringBuilder();
                        skey.Append(s.ToString() + ",");
                        skey.Append(hodEmailId.Split(',')[2] + ",");

                        var uri1 = ConfigurationManager.AppSettings["emailBulkApprovalPath"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=a" + "&edit=0");
                        var uri2 = ConfigurationManager.AppSettings["emailBulkApprovalPath"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=r" + "&edit=0");
                        var uri3 = ConfigurationManager.AppSettings["emailBulkApprovalPath"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=s" + "&bbID=" + s);
                        var essurl = ConfigurationManager.AppSettings["essportalUrl"].Trim();
                        var appLink = string.Empty;
                        appLink = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a name='app' style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Acceptance</a></td>";
                        appLink = appLink + "<td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a name='mapp' style='color:#fff; text-decoration:none;' href='" + uri2 + "'>Rejection</a> </td>";
                        appLink = appLink + "<td style='width:110px; height:25px; background-color:#ffa500;text-align:center;border-radius:5px'><a name='rej' style='color:#fff; text-decoration:none;' href='" + uri3 + "'>Selective</a></td> </tr></table>";

                        var templateData = emailCredentials.TemplateFilePath;
                        var hodName = hodEmailId.Split(',')[1];

                        templateData = templateData.Replace("[appLink]", appLink);
                        templateData = templateData.Replace("[hodName]", hodName);
                        emailCredentials.TemplateFilePath = templateData;

                        TempData["emailData_Hod"] = emailCredentials;
                        TempData["emailId_Hod"] = hodEmailId.Split(',')[0];
                        TempData["msgResponse"] = "SOD-Bulk Booking Request No. " + s.ToString() + " : has been completed successfully.Your booking request has been sent to your concerned HOD for approval at  " + hodEmailId.Split(',')[0].ToString();

                        //SMS Approval
                        SendSMSforApproval(bmaster.TRId.ToString(), hodEmailId, reqName, uri1, uri2, uri3);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["fun1"] = ex.Message + " ---" + ex.Source + "---- " + ex.InnerException.Message;
            }
            return Json(s.ToString(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Generate Pnr Role Hod
        /// </summary>
        /// <param name="blist"></param>
        /// <param name="DetalsList"></param>
        /// <returns></returns>
        public JsonResult GeneratePnrRoleHod(List<BulkEmployeeList> blist, List<BulkUploadModels> DetalsList)
        {
            var bulkMaster = new BulkUploadMasterModels();
            var empList = new List<string>();
            var bulkhotelList = new List<BulkTravelRequestHotelDetailModels>();
            var BulkBookingId = blist[0].BTrId;
            var addNo = _bulkUploadRepository.FindAddNo(blist[0].BTrId);
            var empDetailsList = new List<EmployeeCodewiseDetailModel>();
            for (var i = 0; i < DetalsList.Count; i++)
            {
                empList.Add(DetalsList[i].EmpCode);
            }
            empDetailsList = _bulkUploadRepository.GetEmployeeCodewiseDetails(empList, 1);
            if (empDetailsList.Count > 0)
            {
                foreach (var list in DetalsList)
                {
                    foreach (var elist in empDetailsList)
                    {
                        if (list.EmpCode == elist.EmpCode)
                        {
                            list.Title = elist.Gender.Equals("M") ? "Mr" : "Ms";
                            list.FirstName = elist.FirstName;
                            list.LastName = elist.LastName;
                            list.Designation = elist.Designation;
                            list.Department = elist.Department;
                            list.MobileNo = elist.PhoneNo;
                            list.EmailId = elist.EmailId;
                            list.AddNo = addNo + 1;
                        }
                    }
                }
                TempData["bulkList"] = DetalsList;
                TempData["empList"] = empList;
            }
            var s = 0;
            var pnrList = new List<BulkEmployeeList>();
            try
            {
                s = _bulkUploadRepository.saveBulk_newRow(DetalsList);
                if (s >= 1)
                {
                    //generate Pnr
                    pnrList = NavitaireServicesBulkBooking.Generate_PNR(DetalsList, BulkBookingId.ToString());
                    var c = _bulkUploadRepository.UpdatePNRStatusList((int)(BulkBookingId), pnrList);
                    if (pnrList.Count > 0)
                    {
                        //Bulk Booking Requested User Info
                        var loggedUserInfo = GetLoginUserInfo(Session["EmpId"].ToString().Trim());
                        var reqName = loggedUserInfo[2] == "M" ? "Mr." : "Ms.";
                        reqName = reqName + " " + loggedUserInfo[1];

                        //Send Email Notification to user and Hod
                        var emailSubject = "SOD Bulk Booking Approval Notification from HOD :" + System.DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss tt");
                        var emailTemplateName = "SodBulkBookingRequestApprovalNotificationTemplate_User.html";
                        var emailCredentials = EmailCredentials(emailSubject, emailTemplateName, DetalsList, BulkBookingId.ToString(), reqName);
                        TempData["emailData"] = emailCredentials;
                        TempData["emailId"] = loggedUserInfo[3];
                        sendEmailNotification();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["fun1"] = ex.Message + " ---" + ex.Source + "---- " + ex.InnerException.Message;
            }
            return Json(pnrList, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Initialized Email Credentials to Property
        /// </summary>
        /// <returns></returns>
        public EmailNotificationModel EmailCredentials(string subjectName, string emailTemplateName, List<BulkUploadModels> blist, string bbReqNo, string ReqName)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtpUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtpPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtpServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtpPort"].Trim(),
                TemplateFilePath = ReadFile(emailTemplateName, blist, bbReqNo, ReqName),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }


        /// <summary>
        /// Initialized Email Credentials for hotel
        /// </summary>
        /// <param name="subjectName"></param>
        /// <param name="emailTemplateName"></param>
        /// <param name="hoteldetails"></param>
        /// <param name="approvalStatus"></param>
        /// <returns></returns>
        public EmailNotificationModel EmailCredentialsHotel(string subjectName, string emailTemplateName, List<BulkTravelRequestHotelDetailModels> hoteldetails, List<BulkUploadModels> bookingInfo, List<BulkUploadModels> shareduserlist, string approvalStatus)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtptdUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtptdPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtptdServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtptdPort"].Trim(),
                TemplateFilePath = ReadFileHotel(emailTemplateName, hoteldetails, bookingInfo, shareduserlist, approvalStatus),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }

        /// <summary>
        /// Read Template File from Location
        /// </summary>
        /// <returns></returns>
        private string ReadFile(string emailTemplateName, List<BulkUploadModels> blist, string bbReqNo, string ReqName)
        {
            var strContent = new StringBuilder();
            string line;
            var file = new System.IO.StreamReader(
                       System.Web.HttpContext.Current.Server.MapPath("~/Template/BulkTemplate/" + emailTemplateName));
            while ((line = file.ReadLine()) != null)
            {
                strContent.Append(line);
            }
            file.Close();

            strContent = strContent.Replace("[RequesterName]", ReqName);
            strContent = strContent.Replace("[binfo]", "Traveller Info  (SOD-Bulk Req.No. -" + bbReqNo + ")");
            var tr = "";
            var trp = string.Empty;
            var counter = 1;
            if (emailTemplateName == "SodBulkBookingRequestApprovalNotificationTemplate_User.html")
            {
                trp = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;font-weight:bold;'><td style='height:20px; padding-bottom:8px;'>S. No.</td><td style='height:20px; padding-bottom:8px;'>Emp. Code</td><td style='height:20px; padding-bottom:8px;'>Passenger Name</td><td height:20px; padding-bottom:8px;'>Travel Date</td><td height:20px; padding-bottom:8px;'>Sector</td> <td style='height:20px; padding-bottom:8px;'>Flight No.</td><td height:20px; padding-bottom:8px;'>Pnr</td><td style='height:20px; padding-bottom:8px;'>Booking Type</td> <td style='height:20px; padding-bottom:8px;'>Meal</td><td style='height:20px; padding-bottom:8px;'>Beverage</td><td style='height:20px; padding-bottom:8px;'>Hotel Required</td></tr>";
                foreach (var b in blist)
                {
                    var name = b.Title + " " + b.FirstName + " " + b.LastName;
                    var hotelRequired = "";
                    var bvg = "";
                    var ml = "";
                    hotelRequired = b.IsHotelRequired == true ? "Yes" : "No";
                    bvg = b.Beverage == "" ? "NA" : b.Beverage;
                    ml = b.Meal == "" ? "NA" : b.Meal;
                    trp = trp + "<tr style='font-family:Arial; font-size:12px;'><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + counter + "</td><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + b.EmpCode + "</td><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + name + "</td><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + b.TravelDate + "</td><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + b.Sector + "</td><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + b.FlightNo + "</td> <td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + b.PNR + "</td> <td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + b.BookingType + "</td> <td style='borde:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + ml + "</td> <td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + bvg + "</td>     <td style='border:1px solid #c2c2c2;border-right:solid 1px transparent;height:20px; padding-bottom:8px;'>" + hotelRequired + "</td></tr>";
                    counter++;
                }
            }
            else
            {
                trp = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;font-weight:bold;'><td style='height:20px; padding-bottom:8px;'>S. No.</td><td style='height:20px; padding-bottom:8px;'>Emp. Code</td><td style='height:20px; padding-bottom:8px;'>Passenger Name</td><td height:20px; padding-bottom:8px;'>Travel Date</td><td height:20px; padding-bottom:8px;'>Sector</td> <td style='height:20px; padding-bottom:8px;'>Flight No.</td><td style='height:20px; padding-bottom:8px;'>Booking Type</td> <td style='height:20px; padding-bottom:8px;'>Meal</td><td style='height:20px; padding-bottom:8px;'>Beverage</td><td style='height:20px; padding-bottom:8px;'>Hotel Required</td></tr>";
                foreach (var b in blist)
                {
                    var name = b.Title + " " + b.FirstName + " " + b.LastName;
                    var hotelRequired = "";
                    var bvg = "";
                    var ml = "";
                    hotelRequired = b.IsHotelRequired == true ? "Yes" : "No";
                    bvg = b.Beverage == "" ? "NA" : b.Beverage;
                    ml = b.Meal == "" ? "NA" : b.Meal;
                    trp = trp + "<tr style='font-family:Arial; font-size:12px;'><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + counter + "</td><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + b.EmpCode + "</td><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + name + "</td><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + b.TravelDate + "</td><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + b.Sector + "</td><td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + b.FlightNo + "</td> <td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + b.BookingType + "</td> <td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + ml + "</td> <td style='border:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + bvg + "</td>     <td style='border:1px solid #c2c2c2;border-right:solid 1px transparent;height:20px; padding-bottom:8px;'>" + hotelRequired + "</td></tr>";
                    counter++;
                }
            }

            strContent = strContent.Replace("[tr]", trp);

            return strContent.ToString();
        }


        /// <summary>
        /// Read Hotel File
        /// </summary>
        /// <param name="emailTemplateName"></param>
        /// <param name="hoteldetails"></param>
        /// <param name="bookingInfo"></param>
        /// <param name="shareduserlist"></param>
        /// <param name="approvalStatus"></param>
        /// <returns></returns>
        private string ReadFileHotel(string emailTemplateName, List<BulkTravelRequestHotelDetailModels> hoteldetails, List<BulkUploadModels> bookingInfo, List<BulkUploadModels> shareduserlist, string approvalStatus)
        {
            var strContent = new StringBuilder();
            string line;
            var file = new System.IO.StreamReader(Server.MapPath("~/Template/" + emailTemplateName));
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
                    tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Hotel Confirmation No</td><td>City</td><td>Check-In Date</td><td>Check-Out Date</td><td>Hotel Information</td><td>No. of Guests</td></tr>";
                    tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td>" + hoteldetails[0].HotelConfirmationNo +
                        "</td><td>" + hoteldetails[0].City + "</td><td>" + hoteldetails[0].CheckInDate.ToString("dd/MM/yyyy") + "</td><td>" + hoteldetails[0].CheckOutDate.ToString("dd/MM/yyyy") +
                        "</td><td>Name: " + hoteldetails[0].HotelName + "<br>Address: " + hoteldetails[0].HotelAddress + " <br>Phone No.: " + hoteldetails[0].HotelPhoneNo + "</td><td>"
                        + "1" + "</td></tr>";
                }
                else
                {
                    tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Hotel Confirmation No</td><td>City</td><td>Check-In Date</td><td>Check-Out Date</td><td>Hotel Information</td><td>No. of Guests</td><td>Sharing With</td></tr>";
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
                strContent = strContent.Replace("[appLinkText]", "");
                strContent = strContent.Replace("[appLink]", "");
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


        /// <summary>
        /// find inclusions of hotel by city and hotel name
        /// </summary>
        /// <param name="hotelcity"></param>
        /// <param name="hotelname"></param>
        /// <returns></returns>
        public List<HotelInclusionMasterModels> findHotelInclusions(string hotelcity, string hotelname)
        {
            var list = new List<HotelInclusionMasterModels>();
            list = _transportRepository.findHotelInclusions(hotelcity, hotelname);
            return list;
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



        /// <summary>
        /// Get logged User Info
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        public List<string> GetLoginUserInfo(string empId)
        {
            var s = SOD.Services.ADO.SodCommonServices.GetEmployeeCommonDetails(int.Parse(empId));
            var empInfo = new List<string> { s[0].EmpId.ToString().Trim(), s[0].EmpName.Trim(), s[0].Gender.Trim(), s[0].Email.Trim() };
            return empInfo;
        }



        /// <summary>
        /// Get Bulk Booking User Info
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        public List<string> GetBulkBookingUserInfo(Int64 TraId)
        {
            var s = SOD.Services.ADO.SodCommonServices.GetBulkBookingUserInfoByTransactionId(TraId, null, 1);
            var empInfo = new List<string> { s[0].EmpId.ToString().Trim(), s[0].EmpName.Trim(), s[0].Gender.Trim(), s[0].Email.Trim() };
            return empInfo;
        }



        /// <summary>
        /// Get Bulk Booking Approval List for HOD Approval
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetBulkApprovalMasterList()
        {
            if (TempData["BulkBookingRequestId"] != null)
                return Json('0', JsonRequestBehavior.AllowGet);
            else
                return Json(_bulkUploadRepository.GetBulkBookingHODApprovalList_MasterData(Session["EmpId"].ToString().Trim(), 1), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get bulk booking hotel list for traveldesk
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetBulkHotelList(String type)
        {
            var bookingType = type;
            return Json(_bulkUploadRepository.GetBulkBookingHotelListData(bookingType), JsonRequestBehavior.AllowGet);
        }


        /// Validate Duplicate PNR (Uploaded excel file)
        /// </summary>
        /// <returns></returns>
        public void ValidateDuplicateBulkBooking(List<BulkUploadModels> blist)
        {
            var dList = _bulkUploadRepository.ValidateDuplicateBulkBooking(blist);
            foreach (var d in dList)
            {
                foreach (var b in blist)
                {
                    if (d.Split('|')[0] == b.EmpCode && d.Split('|')[1] == b.FlightNo &&
                        d.Split('|')[2] == b.TravelDate && d.Split('|')[3] == b.Sector &&
                        d.Split('|')[4].ToLower() == b.BookingType.ToLower())
                        b.IsDuplicate = true;
                }
            }
        }


        /// <summary>
        /// Send bulk Request to hotel
        /// </summary>
        /// <param name="elist"></param>
        /// <returns></returns>
        public JsonResult SendBulkRequestToHotel(List<BulkUploadModels> elist, List<BulkTravelRequestHotelDetailModels> hlist)
        {
            var remarks = hlist[0].Remarks_Status;
            var jsonmsg = string.Empty;
            var _context = new SodEntities();
            var clubMaxId = _context.BulkTravelRequestHotelDetailModel.DefaultIfEmpty().Max(x => x.clubId == null ? 1 : x.clubId + 1);

            foreach (var item in hlist)
            {
                item.EntryDate = DateTime.Now;
                item.IsAllocated = 0;
                item.HotelReferenceID = "SH" + item.TravelRequestId;
                item.clubId = clubMaxId;
            }
            var saveData = _bulkUploadRepository.SaveBulkHotelUpload(hlist);
            if (saveData > 0)
            {
                try
                {
                    var s = clubMaxId;
                    var travelrequestId = hlist[0].TravelRequestId;
                    var primaryemail = hlist[0].PrimaryEmail;
                    var hotelname = hlist[0].HotelName;
                    var emailSubject = "Hotel Request Notification from Spicejet:" + System.DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt");
                    var emailTemplateName = "HotelBookingRequestNotificationTemplateFor_Contractual.html";
                    var emailCredentials = EmailCredentialsHotelRequest(emailSubject, emailTemplateName, travelrequestId, s, hotelname);

                    var skey = new StringBuilder();
                    skey.Append(s.ToString() + ",");
                    skey.Append(primaryemail);

                    var uri1 = "";
                    var uri2 = "";

                    uri1 = ConfigurationManager.AppSettings["emailApprovalPathHotelCode"].Trim() + "?str=" +CipherURL.Encrypt(skey + "&type=y&trid=" + s.ToString() + "&hotelname=" + hotelname);
                    uri2 = ConfigurationManager.AppSettings["emailApprovalPathHotelCode"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=n&trid=" + s.ToString() + "&hotelname=" + hotelname);

                    // var essurl = ConfigurationManager.AppSettings["essportalUrl"].Trim();
                    var appLink = string.Empty;
                    appLink = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Acceptance</a></td>";
                    appLink = appLink + "<td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a style='color:#fff; text-decoration:none;' href='" + uri2 + "'>Rejection</a> </td></tr></table>";

                    var templateData = emailCredentials.TemplateFilePath;

                    templateData = templateData.Replace("[appLink]", appLink);
                    templateData = templateData.Replace("[hotelName]", "<b>" + hotelname + "</b>");
                   // templateData = templateData.Replace("[remarksTravelDesk]", remarks);
                    //templateData = templateData.Replace("[hinfo]", "Hotel  Details");satyam
                    templateData = templateData.Replace("[hinfo]", " ");

                    var trI = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;font-weight:bold;'>" +
                              "<td style='height:20px; padding-bottom:8px;'>Hotel Name</td>" +
                              "<td style='height:20px; padding-bottom:8px;'>Address</td>" +
                              "<td style='height:20px; padding-bottom:8px;'>Type</td>" +
                              "<td style='height:20px; padding-bottom:8px;'>Email</td>" +
                               "<td height:20px; padding-bottom:8px;'>Price</td></tr>";

                    trI = trI + "<tr style='font-family:Arial; font-size:12px;'>" +
                                "<td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + hlist[0].HotelName +
                                "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + hlist[0].HotelAddress +
                                "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + hlist[0].HotelType +
                                "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + hlist[0].PrimaryEmail +
                                "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'><b>" + hlist[0].HotelPrice + "</b>" +
                                "</td></tr>";

                    templateData = templateData.Replace("[trI]", " ");//soni
                    //Need to hide
                    //templateData = templateData.Replace("[trI]", trI);
                    var citycode = _transportRepository.GetCityCodeOfHotel(hlist[0].HotelCode);
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
                    emailCredentials.TemplateFilePath = templateData;

                    if (hlist[0].SecondaryEmail != null)
                    {
                        var sEmail = hlist[0].SecondaryEmail.Split(',');
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
                        EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials, primaryemail);
                    }
                    //EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials, primaryemail);Satyam
                    TempData["msgResponse"] = "Hotel Request Notification : Hotel details have been sent successfully. The request has been sent to the respected hotel at  " + primaryemail.ToString();
                    return Json("RequestSent",JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json("OOps! Something went wrong", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("Save failed", JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Resend Bulk Request To Hotel
        /// </summary>
        /// <param name="travelreqId"></param>
        /// <param name="clubId"></param>
        /// <param name="hotelname"></param>
        /// <returns></returns>
        public JsonResult ResendRequestToHotel(string travelreqId, string clubId, string hotelname, string PrimaryEmail, string sec_mail, string HotelCity)
        {
            if (PrimaryEmail.Length > 0)
            {
                var hname = hotelname.Split('-')[0].ToString();
                var citycode = HotelCity;
                var s = Convert.ToInt32(clubId);
                var travelrequestId = Convert.ToInt64(travelreqId);
                var primaryemail = PrimaryEmail;
                var emailSubject = "Hotel Request Notification from Spicejet:" + System.DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt");
                var emailTemplateName = "HotelBookingRequestNotificationTemplateFor_Contractual.html";
                var emailCredentials = EmailCredentialsHotelRequest(emailSubject, emailTemplateName, travelrequestId, s, hotelname);

                var skey = new StringBuilder();
                skey.Append(s.ToString() + ",");
                skey.Append(primaryemail);

                var uri1 = "";
                var uri2 = "";

                uri1 = ConfigurationManager.AppSettings["emailApprovalPathHotelCode"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=y&trid=" + s.ToString() + "&hotelname=" + hotelname);
                uri2 = ConfigurationManager.AppSettings["emailApprovalPathHotelCode"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=n&trid=" + s.ToString() + "&hotelname=" + hotelname);

                // var essurl = ConfigurationManager.AppSettings["essportalUrl"].Trim();
                var appLink = string.Empty;
                appLink = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Acceptance</a></td>";
                appLink = appLink + "<td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a style='color:#fff; text-decoration:none;' href='" + uri2 + "'>Rejection</a> </td></tr></table>";

                var templateData = emailCredentials.TemplateFilePath;

                templateData = templateData.Replace("[appLink]", appLink);
                templateData = templateData.Replace("[hotelName]", "<b>" + hname + "</b>");

                emailCredentials.TemplateFilePath = templateData;
                //templateData = templateData.Replace("[remarksTravelDesk]", " ");
                templateData = templateData.Replace("[hinfo]", "");

                templateData = templateData.Replace("[trI]", " ");

                // var citycode = _transportRepository.GetCityCodeOfHotel(hotelname);
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
                emailCredentials.TemplateFilePath = templateData;
                TempData["emailData_Hod"] = emailCredentials;
                TempData["emailId_Hod"] = primaryemail;
                if (sec_mail != null)
                {
                    var sEmail = sec_mail.Split(',');
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
                    EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials, primaryemail);
                }
                TempData["msgResponse"] = "Hotel Request Notification : Hotel details have been sent successfully. The request has been sent to the respected hotel at  " + primaryemail.ToString();
                return Json("RequestSent", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Save failed", JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Send request to HOD for financial approval non-contractual
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult sendFinancialApprovalRequest(List<BulkTravelRequestHotelDetailModels> hlist, string hodemail1, string hodemail2, List<BulkHotelInclusionNonContractualMasterModels> inclist, string sodOat)
        {

            var s = hlist[0].TravelRequestId;
            var hotelname = hlist[0].HotelName;
            var _context = new SodEntities();
            var clubMaxId = 0;
            var hReqId = 0;
            if (sodOat == "SOD")
            {
                clubMaxId = _context.BulkTravelRequestHotelDetailModel.DefaultIfEmpty().Max(x => x.clubId == 0 ? 1 : x.clubId + 1);
                hReqId = _context.BulkHotelInclusionNonContractualMasterModel.DefaultIfEmpty().Max(x => x.HotelRequestId == null ? 1 : x.HotelRequestId + 1);
            }
            else
            {
                clubMaxId = _context.BulkTravelRequestHotelDetailModel.DefaultIfEmpty().Max(x => x.clubId == 0 ? 1 : x.clubId + 1);
            }
            var remarks = hlist[0].Remarks_Status;
            var jsonmsg = string.Empty;
            foreach (var item in hlist)
            {
                item.EntryDate = DateTime.Now;
                item.IsAllocated = 0;
                item.HotelReferenceID = "SH" + item.TravelRequestId;
                item.clubId = clubMaxId;
            }
            foreach (var item in inclist)
            {
                item.HotelRequestId = hReqId;
            }
            var s1 = _bulkUploadRepository.SaveHODFinancialApprovalRequest(hlist, hodemail1, hodemail2, inclist, sodOat);

            if (s1 > 0)
            {
                if (hodemail1.Length > 0)
                {
                    var emailSubject = "Non-Contractual Hotel Request Notification from Travel desk:" + System.DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt");
                    var emailTemplateName = "HotelHODApprovalNotificationTemplateFor_NonContractual.html";
                    var emailCredentials = EmailCredentialsFinancialApprovalRequest(emailSubject, emailTemplateName, inclist, sodOat, hlist);
                    var emailCredentials2 = EmailCredentialsFinancialApprovalRequest(emailSubject, emailTemplateName, inclist, sodOat, hlist);

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
                        uri1 = ConfigurationManager.AppSettings["emailApprovalPathHODCodeApp"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=Ba&trid=" + s.ToString() + "&empcode=" + hodlist[0].EmpCode.Trim() + "&hreqId=" + hReqId.ToString() + "&clubId=" + clubMaxId.ToString());
                        uri2 = ConfigurationManager.AppSettings["emailApprovalPathHODCodeApp"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=Br&trid=" + s.ToString() + "&empcode=" + hodlist[0].EmpCode.Trim() + "&hreqId=" + hReqId.ToString() + "&clubId=" + clubMaxId.ToString());
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
                    if (hodemail2.Length > 0)
                    {
                        var hodlist2 = _transportRepository.GetHodApproverNameHotels(hodemail2);
                        if (sodOat == "SOD")
                        {
                            uri3 = ConfigurationManager.AppSettings["emailApprovalPathHODCodeApp"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=Ba&trid=" + s.ToString() + "&empcode=" + hodlist2[0].EmpCode.Trim() + "&breqId=" + hReqId.ToString() + "&clubId=" + clubMaxId.ToString());
                            uri4 = ConfigurationManager.AppSettings["emailApprovalPathHODCodeApp"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=Br&trid=" + s.ToString() + "&empcode=" + hodlist2[0].EmpCode.Trim() + "&hid=" + hReqId.ToString() + "&clubId=" + clubMaxId.ToString());
                        }
                        else
                        {
                            uri3 = ConfigurationManager.AppSettings["emailApprovalPathHODCodeApp"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=b&trid=" + s.ToString() + "&empcode=" + hodlist2[0].EmpCode.Trim() + "&hid=" + hReqId.ToString());
                            uri4 = ConfigurationManager.AppSettings["emailApprovalPathHODCodeApp"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=c&trid=" + s.ToString() + "&empcode=" + hodlist2[0].EmpCode.Trim() + "&hid=" + hReqId.ToString()); 
                        }

                        var appLink2 = string.Empty;
                        appLink2 = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a style='color:#fff; text-decoration:none;' href='" + uri3 + "'>Acceptance</a></td>";
                        appLink2 = appLink2 + "<td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a style='color:#fff; text-decoration:none;' href='" + uri4 + "'>Rejection</a> </td></tr></table>";
                        var templateData1 = emailCredentials2.TemplateFilePath;
                        templateData1 = templateData1.Replace("[appLink]", appLink2);
                        templateData1 = templateData1.Replace("[HodName]", hodlist2[0].EmpName.ToString());
                        emailCredentials2.TemplateFilePath = templateData1;
                        TempData["emailData_Hod"] = emailCredentials2;
                        TempData["emailId_Hod"] = hodemail2;
                        EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials2, hodemail2);
                    }
                    EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials, hodemail1);
                    TempData["msgResponse"] = "Hotel Approval Request Notification : Approval has been sent successfully to the Travel Desk.";
                }
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// view detail for hod approval status
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult getdetailHODApproval()
        {
            var trnId = Request.QueryString["trId"].ToString().Split('|')[0];
            var breqId = Request.QueryString["trId"].ToString().Split('|')[1];
            var approverlist = new List<BulkBookingHODFinancialApprovalModels>();
            approverlist = _bulkUploadRepository.GetdetailHODApproval(Convert.ToInt64(trnId), Convert.ToInt32(breqId), "SOD");

            return Json(approverlist, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetHodOfDepartment(string dept)
        {
            var deptId = Int32.Parse(dept.Split('|')[0]);
            var verticalCode = dept.Split('|')[1];
            string hodEmail = _bulkUploadRepository.getHodEmployee(deptId, verticalCode);
            return Json(hodEmail, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Resend request to respective financial approvers
        /// </summary>
        /// <param name="TravelRequestId"></param>
        /// <param name="sodOat"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ResendApproverRequest(string TravelRequestId, string sodOat)
        {
            var msg = "";
            var dicListApprovalFinancial = new Dictionary<string, object>();
            dicListApprovalFinancial = _bulkUploadRepository.GetBulkHotelDetailbyTrID(Convert.ToInt64(TravelRequestId));
            var hotelDetails = dicListApprovalFinancial["hotelinfobyTrid"] as List<BulkTravelRequestHotelDetailModels>;
            var aprrovalList = dicListApprovalFinancial["listForFinancialApproval"] as List<BulkBookingHODFinancialApprovalModels>;
            var hotelReferenceType = dicListApprovalFinancial["ListOfHotelReferenceId"] as List<int>;
            var hodempcode1 = "";
            var hodempcode2 = "";
            var hodemail1 = "";
            var hodemail2 = "";
            var dicListofHotelAsperHotelReqId = new Dictionary<string, object>();
            List<int> distinctHotelRefId = new List<int>();

            if (sodOat == "SOD")
            {
                for (var i = 0; i < hotelReferenceType.Count; i++)
                {
                    List<BulkTravelRequestHotelDetailModels> elist1 = new List<BulkTravelRequestHotelDetailModels>();
                    List<BulkTravelRequestHotelDetailModels> elist2 = new List<BulkTravelRequestHotelDetailModels>();
                    List<Int64> breqID1 = new List<Int64>();
                    List<Int64> breqID2 = new List<Int64>();
                    foreach (var lst in aprrovalList)
                    {
                        if (hotelReferenceType[i] == lst.HotelRequestId)
                        {

                            if (lst.ApproverEmpCodeLevel2 != "" || lst.ApproverEmpCodeLevel2 != "NULL")
                            {
                                if (lst.ApprovalStatusLevel2 == 0)
                                {
                                    breqID2.Add(lst.BreqId);
                                    hodempcode2 = lst.ApproverEmpCodeLevel2;
                                }
                            }
                            if (lst.ApproverEmpCodeLevel1 != "" || lst.ApproverEmpCodeLevel1 != "NULL")
                            {
                                if (lst.ApprovalStatusLevel1 == 0)
                                {
                                    breqID1.Add(lst.BreqId);
                                    hodempcode1 = lst.ApproverEmpCodeLevel1;
                                }

                            }
                        }
                    }
                    foreach (var hlist in hotelDetails)
                    {
                        if (breqID1.Contains(hlist.BReqId))
                        {
                            elist1.Add(hlist);
                        }
                        if (breqID2.Contains(hlist.BReqId))
                        {
                            elist2.Add(hlist);
                        }
                    }

                    if (breqID1.Count > 0 || breqID2.Count > 0)
                    {

                        var emailCredentials = new SOD.Model.EmailNotificationModel();
                        var emailCredentials2 = new SOD.Model.EmailNotificationModel();
                        var inclist = _bulkUploadRepository.FindNonContractualHotelInclusions(Convert.ToInt64(TravelRequestId), hotelReferenceType[i]);
                        var emailSubject = "Non-Contractual Hotel Booking Request Notification from Travel desk:" + System.DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt");
                        var emailTemplateName = "HotelHODApprovalNotificationTemplateFor_NonContractual.html";

                        if (breqID1.Count > 0)
                        {
                            emailCredentials = EmailCredentialsFinancialApprovalRequest(emailSubject, emailTemplateName, inclist, sodOat, elist1);
                        }
                        if (breqID2.Count > 0)
                        {
                            emailCredentials2 = EmailCredentialsFinancialApprovalRequest(emailSubject, emailTemplateName, inclist, sodOat, elist2);
                        }

                        var skey = new StringBuilder();
                        skey.Append(TravelRequestId.ToString() + ",");


                        var uri1 = "";
                        var uri2 = "";
                        var uri3 = "";
                        var uri4 = "";
                        if (hodempcode1 != "")
                        {
                            hodemail1 = _transportRepository.GetFinancialHodDetailsEmail(hodempcode1);
                            skey.Append(hodemail1);
                            var hodlist = _transportRepository.GetHodApproverNameHotels(hodemail1);
                            if (sodOat == "SOD")
                            {
                                uri1 = ConfigurationManager.AppSettings["emailApprovalPathHODCodeApp"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=Ba&trid=" + TravelRequestId.ToString() + "&empcode=" + hodlist[0].EmpCode.Trim() + "&hreqId=" + hotelReferenceType[i].ToString() + "&clubId=" + elist1[0].clubId.ToString());
                                uri2 = ConfigurationManager.AppSettings["emailApprovalPathHODCodeApp"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=Br&trid=" + TravelRequestId.ToString() + "&empcode=" + hodlist[0].EmpCode.Trim() + "&hreqId=" + hotelReferenceType[i].ToString() + "&clubId=" + elist1[0].clubId.ToString());
                            }
                            else
                            {
                                uri1 = ConfigurationManager.AppSettings["emailApprovalPathHODCodeApp"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=b&trid=" + TravelRequestId.ToString() + "&empcode=" + hodlist[0].EmpCode.Trim());
                                uri2 = ConfigurationManager.AppSettings["emailApprovalPathHODCodeApp"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=c&trid=" + TravelRequestId.ToString() + "&empcode=" + hodlist[0].EmpCode.Trim());
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
                        if (hodempcode2 != "")
                        {
                            hodemail2 = _transportRepository.GetFinancialHodDetailsEmail(hodempcode2);
                            var hodlist2 = _transportRepository.GetHodApproverNameHotels(hodemail2);
                            skey.Append(hodemail2);
                            if (sodOat == "SOD")
                            {
                                uri3 = ConfigurationManager.AppSettings["emailApprovalPathHODCodeApp"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=Ba&trid=" + TravelRequestId.ToString() + "&empcode=" + hodlist2[0].EmpCode.Trim() + "&breqId=" + hotelReferenceType[i].ToString() + "&clubId=" + elist2[0].clubId.ToString());
                                uri4 = ConfigurationManager.AppSettings["emailApprovalPathHODCodeApp"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=Br&trid=" + TravelRequestId.ToString() + "&empcode=" + hodlist2[0].EmpCode.Trim() + "&hid=" + hotelReferenceType[i].ToString() + "&clubId=" + elist2[0].clubId.ToString());
                            }
                            else
                            {
                                uri3 = ConfigurationManager.AppSettings["emailApprovalPathHODCodeApp"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=b&trid=" + TravelRequestId.ToString() + "&empcode=" + hodlist2[0].EmpCode.Trim() + "&hid=" + elist2[0].clubId.ToString()) ;
                                uri4 = ConfigurationManager.AppSettings["emailApprovalPathHODCodeApp"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=c&trid=" + TravelRequestId.ToString() + "&empcode=" + hodlist2[0].EmpCode.Trim() + "&hid=" + elist2[0].clubId.ToString()); 
                            }

                            var appLink2 = string.Empty;
                            appLink2 = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a style='color:#fff; text-decoration:none;' href='" + uri3 + "'>Acceptance</a></td>";
                            appLink2 = appLink2 + "<td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a style='color:#fff; text-decoration:none;' href='" + uri4 + "'>Rejection</a> </td></tr></table>";
                            var templateData = emailCredentials2.TemplateFilePath;
                            templateData = templateData.Replace("[appLink]", appLink2);
                            templateData = templateData.Replace("[HodName]", hodlist2[0].EmpName.ToString());
                            emailCredentials2.TemplateFilePath = templateData;
                            TempData["emailData_Hod"] = emailCredentials2;
                            TempData["emailId_Hod"] = hodemail2;
                            EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials2, hodemail2);
                        }

                        TempData["msgResponse"] = "Hotel Approval Request Notification : Approval has been sent successfully to the following approvar.";
                        msg = "Request has been successfully resent to respected Approver(s).";
                    }
                    else
                    {
                        msg = "No record to send for approval !";
                    }

                }
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// request to hotel by button after financial approval
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RequestToHotlAfterApproval(string TravelRequestId, string sodOat)
        {
            var s = "";
            if (sodOat == "SOD")
            {
                var dicListApprovalFinancial = new Dictionary<string, object>();
                dicListApprovalFinancial = _bulkUploadRepository.GetBulkHotelDetailbyTrID(Convert.ToInt64(TravelRequestId));

                var clubIdList = dicListApprovalFinancial["ListOfclubId"] as List<int>;
                var hotelDetails = dicListApprovalFinancial["hotelinfobyTrid"] as List<BulkTravelRequestHotelDetailModels>;
                if (clubIdList.Count != 0)
                {
                    foreach (var lst in clubIdList)
                    {
                        string hotelname = "";
                        string HotelCity = "";
                        string PrimaryEmail = "";
                        string Sec_mail = "";
                        foreach (var hd in hotelDetails)
                        {
                            if (hd.clubId == lst)
                            {
                                hotelname = hd.HotelName;
                                HotelCity = hd.City;
                                PrimaryEmail = hd.PrimaryEmail;
                                Sec_mail = hd.SecondaryEmail;
                                break;
                            }
                        }
                        try
                        {
                            _bulkUploadRepository.UpdateHotelStatus(Convert.ToInt64(TravelRequestId), lst.ToString());
                            ResendRequestToHotel(TravelRequestId, lst.ToString(), hotelname, PrimaryEmail, Sec_mail, HotelCity);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                    }
                    s = TempData["msgResponse"].ToString();
                }
                else
                {
                    s = "No record for sending request to non-contractual hotel!";
                }
            }

            return Json(s, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Email credentials for sending mail to hotel
        /// </summary>
        /// <param name="subjectName"></param>
        /// <param name="emailTemplateName"></param>
        /// <param name="requestList"></param>
        /// <param name="hotelname"></param>
        /// <param name="sodOat"></param>
        /// <returns></returns>
        public EmailNotificationModel EmailCredentialsHotelRequest(string subjectName, string emailTemplateName, Int64 travelRequestID, Int32 clubid, string hotelname)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtpHotelUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtptdPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtptdServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtptdPort"].Trim(),
                TemplateFilePath = ReadFileHotelRequest(emailTemplateName, travelRequestID, clubid, hotelname),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }


        /// <summary>
        /// EmailCredential to send mail to hotel on cancellation
        /// in travelRequest Id we send employee id 
        /// </summary>
        /// <param name="subjectName"></param>
        /// <param name="emailTemplateName"></param>
        /// <param name="travelRequestID"></param>
        /// <param name="clubid"></param>
        /// <param name="hotelname"></param>
        /// <returns></returns>
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
        public EmailNotificationModel EmailCredentialsFinancialApprovalRequest(string subjectName, string emailTemplateName, List<BulkHotelInclusionNonContractualMasterModels> inclist, string sodOat, List<BulkTravelRequestHotelDetailModels> elist)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtptdUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtptdPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtptdServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtptdPort"].Trim(),
                TemplateFilePath = ReadFileFinancialApprovalRequest(emailTemplateName, inclist, sodOat, elist),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }

        /// <summary>
        /// Read FileFinancial Approval Request
        /// </summary>
        /// <param name="emailTemplateName"></param>
        /// <param name="inclist"></param>
        /// <param name="sodOat"></param>
        /// <param name="elist"></param>
        /// <returns></returns>
        private string ReadFileFinancialApprovalRequest(string emailTemplateName, List<BulkHotelInclusionNonContractualMasterModels> inclist, string sodOat, List<BulkTravelRequestHotelDetailModels> elist)
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
            strContent = strContent.Replace("[binfo]", "Booking Details (Request ID :" + elist[0].TravelRequestId + ")");
            strContent = strContent.Replace("[hinfo]", "Hotel Details ");

            GetBulkHotelRequestData(elist[0].TravelRequestId, elist[0].clubId, elist[0].HotelName, 1);
            var passengerInfo = TempData["InfoForHotelRequest"] as List<TravelRequestModels>;
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
                var inclusionList = new List<BulkHotelInclusionNonContractualMasterModels>();
                inclusionList = _bulkUploadRepository.FindNonContractualHotelInclusions(elist[0].TravelRequestId, inclist[0].HotelRequestId);
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
        /// Read File Hotel Request on Cancel
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
        /// Read file hotel for hotel request
        /// </summary>
        /// <param name="emailTemplateName"></param>
        /// <param name="requestList"></param>
        /// <param name="hotelname"></param>
        /// <param name="sodOat"></param>
        /// <returns></returns>
        private string ReadFileHotelRequest(string emailTemplateName, Int64 travelRequestID, Int32 clubid, string hotelname)
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

            GetBulkHotelRequestData(travelRequestID, clubid, hotelname, 1);
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
               "<td style='height:20px; padding-bottom:8px;'>Mobile No.</td>" +
               "<td height:20px; padding-bottom:8px;'>Sharing Id</td></tr>";

            if (passengerInfo.Count == 1)
            {
                strContent = strContent.Replace("[occupancy]", "Single Occupancy");
            }
            else
            {
                strContent = strContent.Replace("[occupancy]", "Double Occupancy based on Sharing Id");
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
                        "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.Phone +
                        "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + p.sharingId +
                        "</td></tr>";
                count++;
            }
            strContent = strContent.Replace("[tr]", tr);
            strContent = strContent.Replace("[remarksTravelDesk]", passengerInfo[0].Remarks_Status);
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

        /// <summary>
        /// get passenger details for hotel bulk booking
        /// </summary>
        /// <param name="requestList"></param>
        /// <param name="hotelname"></param>
        /// <param name="sodOat"></param>
        public JsonResult GetHotelDetail(string EmpCode, string TravelRequestId)
        {
            var Hlist = new List<BulkTravelRequestHotelDetailModels>();
            Hlist = _bulkUploadRepository.GetHotelDetailForMail(EmpCode, TravelRequestId);
            return Json(Hlist, JsonRequestBehavior.AllowGet);
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
        public void SendSMSforApproval(string bookingID, string hoddetails, string requesterName, string approvalURI, string rejectionURI, string selectiveURI)
        {
            var smsText = ConfigurationManager.AppSettings["smsApprovalbulk"].ToString().Replace("@Hodname", hoddetails.Split(',')[1]);
            smsText = smsText.Replace("@ReqId", bookingID);
            smsText = smsText.Replace("@PaxName", requesterName);
            smsText = smsText.Replace("[AppQueryString]", approvalURI);
            smsText = smsText.Replace("[RejQueryString]", rejectionURI);
            smsText = smsText.Replace("[SelQueryString]", selectiveURI);
            smsText = System.Uri.EscapeDataString(smsText);
            smsText = smsText.Replace("25", "");
            //Send SMS
            var smsLogModel = new SodApproverSMSLogModels();
            smsLogModel.TrRequestId = Convert.ToInt64(bookingID);
            smsLogModel.EmpCode = hoddetails.Split(',')[4];
            smsLogModel.EmpName = hoddetails.Split(',')[1];
            smsLogModel.EmailID = hoddetails.Split(',')[0];
            smsLogModel.MobileNo = hoddetails.Split(',')[3];
            smsLogModel.Source = "BULK";
            smsLogModel.SMSText = smsText;
            smsLogModel.DeliveryDate = DateTime.Now;
            var status = SmsNotification.SmsNotifications.SendSmsViaApi(smsText, hoddetails.Split(',')[3]);
            smsLogModel.IsDelivered = status.Equals(true) ? true : false;
            _bulkUploadRepository.SaveApproverSMSLog(smsLogModel);
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
            ViewBag.Message = String.Format("Sorry ! Your session has been expired. Please click on the sod link again.");
            return View();
        }

        /// <summary>
        /// Exception Handling
        /// </summary>
        /// <param name="filterContext"></param>
        void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            Logging.ErrorLog.Instance.AddDBLogging(ex, filterContext.Controller.ControllerContext.ToString(), "Controllers/bulkController.cs");
        }
    }
}
