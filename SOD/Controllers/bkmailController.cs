using SOD.CommonWebMethod;
using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;
using SOD.Services.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SOD.EmailNotification;

namespace SOD.Controllers
{

    [HandleError(ExceptionType = typeof(DbUpdateException), View = "Error")]
    public class bkmailController : Controller, IExceptionFilter
    {
        private readonly IBulkUploadRepository _bulkUploadRepository;

        /// <summary>
        /// Initialized Constructor
        /// </summary>
        public bkmailController()
        {
            _bulkUploadRepository = new BulkUploadRepository(new SodEntities());
        }



        /// <summary>
        /// Open View for HOD
        /// </summary>
        /// <returns></returns>
        public ActionResult bapp()
        {
            var strQuery = string.Empty;
            strQuery = Request.QueryString[0].ToString();
            strQuery = CipherURL.Decrypt(strQuery);
            if (strQuery.Split('&')[1].Contains("a"))
                TempData["jsonmsg"] = ApproveSod_BulkBookingRequest(strQuery);
            else if (Request.QueryString[1].Trim().Equals("r"))
                TempData["jsonmsg"] = RejectSod_BulkBookingRequest(strQuery);
            else if (Request.QueryString[1].Trim().Equals("s"))
            {
                var bbId = Request.QueryString[2].ToString();
                //TempData["bbkSelective"] = _bulkUploadRepository.GetBulkBookinDetailsData(bbId);
                TempData["EmpIdOfApproval"] = strQuery.Split(',')[1];
                return RedirectToAction("bbkSelective", "bulk", new { bbId = bbId });
            }else if (Request.QueryString[1].Trim().Equals("Ha"))
            {
                TempData["jsonmsg"] = ApproveOnlyHHotel_BulkBookingRequest(strQuery);
            }
            return View();
        }


        /// <summary>
        /// Approve Booking Request for HOD
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        public string ApproveSod_BulkBookingRequest(string travelReqId)
        {
            var jsonmsg = string.Empty;
            var s = 0;
            var approvalList = new List<BulkBookingRequestApprovalModels>();
            var IsRestList = new List<BulkUploadModels>();
           int AddNo = Convert.ToInt32(travelReqId.Split('&')[2].Split('=')[1]);
            //int AddNo = Convert.ToInt32(travelReqId.Split('&')[2]);
            IsRestList = _bulkUploadRepository.IsEmpRestForApproval(int.Parse(travelReqId.Split(',')[0]), AddNo);
            var vlist = new List<long>();
            if (IsRestList.Count > 0)
            {
                for (var i = 0; i < IsRestList.Count; i++)
                {
                    //Check Duplicate PNR 
                    var appStatus = Services.ADO.SodCommonServices.CheckDuplicatePNR_edit(Convert.ToInt64(travelReqId.Split(',')[0]), 2, AddNo, Convert.ToInt32(IsRestList[0].BReqId));

                    if (appStatus.Equals("1"))
                    {
                        return "Sorry : Approval Process has been already completed.";
                    }
                    if (appStatus.Equals("2"))
                    {
                        return "Sorry : Rejection Process has been already completed.";
                    }
                    BulkBookingRequestApprovalModels model = new BulkBookingRequestApprovalModels();
                    model.TrRequestId = Convert.ToInt64(travelReqId.Split(',')[0]);//Travel Request
                    var dfh = Session["EmpId"];
                    var dfjk = Convert.ToInt32(travelReqId.Split(',')[1].ToString().Trim());
                    //model.ApprovedByEmpId = (Session["EmpId"] != null && int.Parse(Session["EmpId"].ToString()) > 0  ) ? Convert.ToInt32(Session["EmpId"].ToString().Trim()) : Convert.ToInt32(travelReqId.Split(',')[1].ToString().Trim()); // added by soni 16 sep 2019

                    model.ApprovedByEmpId = Session["EmpId"] == null ? Convert.ToInt32(travelReqId.Split(',')[1].ToString().Trim()) : Convert.ToInt32(Session["EmpId"].ToString().Trim());
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
                    model.AddNo = IsRestList[i].AddNo;
                    model.BReqId = IsRestList[i].BReqId;
                    approvalList.Add(model);
                    vlist.Add(IsRestList[i].BReqId);                    
                };
            }
            else
            {
                return "Sorry : Approval Process has been already completed.";
            }
            s = _bulkUploadRepository.ApproveBulkSodBookingRequestSelective(approvalList);
            //Check Duplicate PNR  
            if (s >= 1)
            {
                var countflightRequired = IsRestList.Where(x => x.IsHotelRequired == true).ToList().Count();//added by soni 16 sep
              var pnrList = new List<BulkEmployeeList>();
                pnrList = GeneratePNR_BulkApproval_Selective(vlist, travelReqId.Split(',')[0].ToString().Trim());

                var counter = 0;
                var trr = "<table class='table' style='width:30%;'>";
                trr = trr + "<thead style='background-color:#EE1D23'><tr><th scope=col>Employee Code</th><th scope=col>PNR</th></tr></thead> <tbody>";

                foreach (var pnr in pnrList )
                {
                    if (pnr.PNRStatus.Split('|')[0].ToString() == "ERR001")
                        counter++;
                    trr = trr + "<tr><td>" + pnr.EmpCode + "</td><td>" + pnr.PNRStatus.Split('|')[0] + "</td></tr>";
                }

                //if (counter.Equals(pnrList.Count())) //commented by soni 16 sep 2019
                if (counter.Equals(countflightRequired))
                {
                    trr = trr + "<tr><td colspan='2'> PNR Error : ERR001 PNR generation fail for Bulk Booking Request No.- SOD BULK-" + travelReqId.Split(',')[0] + "&nbsp;<img src='../img/rejected.png' height='20px' width='20px'></td></tr>";
                    var approvalListrollback = new BulkBookingRequestApprovalModels()
                    {
                        TrRequestId = Convert.ToInt64(travelReqId.Split(',')[0]),
                        ApprovalStatus = 0,
                        Comment = ""
                    };
                    _bulkUploadRepository.RollBackApprovalByHOD(approvalListrollback);
                }
                else
                {
                    trr = trr + "<tr><td colspan='2'> PNR generation process has been completed successfully for Bulk Booking Request No.-SOD BULK-" + travelReqId.Split(',')[0] + "&nbsp;<img src='../img/right.png' height='20px' width='20px'></td></tr>";
                }
                trr = trr + "</tbody></table>";
                jsonmsg = trr;
                if (!counter.Equals(countflightRequired) && pnrList.Count > 0)
                {
                    SendMailToUSer_AfterApprovalfromHOD(vlist, travelReqId.Split(',')[0]);
                }

            }
            return (s >= 1 ? jsonmsg.ToString() : "PNR Error BK-ERR002: Please contect to helpdesk !");
        }

        /// <summary>
        /// Approveonly hotel Bulk Booking Request for HOD
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        public string ApproveOnlyHHotel_BulkBookingRequest(string travelReqId)
        {
            var jsonmsg = string.Empty;
            var s = 0;
            var approvalList = new List<BulkBookingRequestApprovalModels>();
            var IsRestList = new List<BulkUploadModels>();
            int AddNo = Int32.Parse(Request.QueryString[2]);
            IsRestList = _bulkUploadRepository.IsEmpRestForApproval(int.Parse(travelReqId.Split(',')[0]), AddNo);
            var vlist = new List<long>();
            if (IsRestList.Count > 0)
            {
                for (var i = 0; i < IsRestList.Count; i++)
                {
                    //Check Duplicate PNR 
                    var appStatus = Services.ADO.SodCommonServices.CheckDuplicatePNR_edit(Convert.ToInt64(travelReqId.Split(',')[0]), 2, AddNo, Convert.ToInt32(IsRestList[0].BReqId));

                    if (appStatus.Equals("1"))
                    {
                        return "Sorry : Approval Process has been already completed.";
                    }
                    if (appStatus.Equals("2"))
                    {
                        return "Sorry : Rejection Process has been already completed.";
                    }
                    BulkBookingRequestApprovalModels model = new BulkBookingRequestApprovalModels();
                    model.TrRequestId = Convert.ToInt64(travelReqId.Split(',')[0]);//Travel Request
                    model.ApprovedByEmpId = Session["EmpId"] == null ? Convert.ToInt32(travelReqId.Split(',')[1].ToString().Trim()) : Convert.ToInt32(Session["EmpId"].ToString().Trim());
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
                    model.AddNo = IsRestList[i].AddNo;
                    model.BReqId = IsRestList[i].BReqId;
                    approvalList.Add(model);
                    vlist.Add(IsRestList[i].BReqId);
                };
            }
            else
            {
                return "Sorry : Approval Process has been already completed.";
            }
            s = _bulkUploadRepository.ApproveBulkSodBookingRequestSelective(approvalList);
            //Check Duplicate PNR  
            if (s >= 1)
            {
               jsonmsg = "Approval Process has been completed successfully for OAT HOTEL Booking Request No.-OAT Request No-" + travelReqId.Split(',')[0] + "&nbsp;<img src='../img/right.png' height='20px' width='20px'>";
               SendMailToUSer_AfterApprovalfromHOD(vlist, travelReqId.Split(',')[0]);
            }
            return (s >= 1 ? jsonmsg.ToString() : "Some error occured, Please try again.");
        }


        /// <summary>
        /// Send mail to User after request approved by HOD
        /// </summary>
        public void SendMailToUSer_AfterApprovalfromHOD(List<long> vlist, string travelReqId)
        {
            //EMail Noitification
            var dicList = new Dictionary<string, object>();
            var bulkMasterModel = new List<BulkUploadMasterModels>();
            var bulkDetailModel = new List<BulkUploadModels>();
            //int AddNo = Int32.Parse(Request.QueryString[2]);
            dicList = _bulkUploadRepository.GetBulkBookingInfoForPNRGenerationSelective(vlist);
            //bulkMasterModel = dicList["bulkDetailInfoEdit"] as List<BulkUploadMasterModels>;
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
        /// Reject Bulk Booking
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        public string RejectSod_BulkBookingRequest(string travelReqId)
        {
            var jsonmsg = string.Empty;
            var s = 0;
            var approvalList = new List<BulkBookingRequestApprovalModels>();
            var IsRestList = new List<BulkUploadModels>();
            if ((travelReqId.Split(',')[0] == "undefined") || (travelReqId.Split(',')[0] == ""))
            {
                return "Invalid record.";
            }
            int AddNo = Int32.Parse(Request.QueryString[2]);
            IsRestList = _bulkUploadRepository.IsEmpRestForApproval(int.Parse(travelReqId.Split(',')[0]), AddNo);
            var vlist = new List<long>();
            if (IsRestList.Count > 0)
            {
                for (var i = 0; i < IsRestList.Count; i++)
                {
                    //Check Duplicate PNR 
                    var appStatus = Services.ADO.SodCommonServices.CheckDuplicatePNR_edit(Convert.ToInt64(travelReqId.Split(',')[0]), 2, AddNo, Convert.ToInt32(IsRestList[0].BReqId));

                    if (appStatus.Equals("1"))
                    {
                        return "Sorry : Approval Process has been already completed.";
                    }
                    if (appStatus.Equals("2"))
                    {
                        return "Sorry : Rejection Process has been already completed.";
                    }
                    BulkBookingRequestApprovalModels model = new BulkBookingRequestApprovalModels();
                    model.TrRequestId = Convert.ToInt64(travelReqId.Split(',')[0]);//Travel Request
                    //model.ApprovedByEmpId = Convert.ToInt32(Session["EmpId"].ToString().Trim());
                    model.ApprovedByEmpId = Session["EmpId"] == null ? Convert.ToInt32(travelReqId.Split(',')[1].ToString().Trim()) : Convert.ToInt32(Session["EmpId"].ToString().Trim());
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
                    model.AddNo = IsRestList[i].AddNo;
                    model.BReqId = IsRestList[i].BReqId;
                    approvalList.Add(model);
                    vlist.Add(IsRestList[i].BReqId);
                };
            }
            else
            {
                return "Sorry : Request Approval/Rejection Process has been already completed.Please go to selective option.";
            }
            s = _bulkUploadRepository.ApproveBulkSodBookingRequestSelective(approvalList);
            if (s >= 1)
            {
                var B = string.Empty;
                var dicList = new Dictionary<string, object>();
                dicList = _bulkUploadRepository.GetBulkBookingInfoForPNRGenerationSelective(vlist);
                //update rejection Pnr status in bulk booking 
                var c = _bulkUploadRepository.UpdateStatusOnRejection(vlist, travelReqId, B);
                //EMail Noitification
                var bulkDetailModel = new List<BulkUploadModels>();
                bulkDetailModel = dicList["bulkDetailInfoSelective"] as List<BulkUploadModels>;
                //var c = _bulkUploadRepository.RejectionCloseBulkBookingRequest(Convert.ToInt64(travelReqId.Split(',')[0]), 1);
                //Bulk Booking Requested User Info
                var bulkUserInfo = GetBulkBookingUserInfo(Convert.ToInt64(travelReqId.Split(',')[0]));
                var reqName = bulkUserInfo[2] == "M" ? "Mr." : "Ms." + " " + bulkUserInfo[1];
                //Send Email Notification to spoc
                var emailSubject = "SOD Bulk Booking Rejection Notification from HOD :" + System.DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss tt");
                var emailTemplateName = "SodBulkBookingRequest_Rejection_User.html";
                var emailCredentials = EmailCredentials(emailSubject, emailTemplateName, bulkDetailModel, travelReqId.Split(',')[0], reqName);
                TempData["emailData"] = emailCredentials;
                TempData["emailId"] = bulkUserInfo[3];
                jsonmsg = "Rejection process has been completed successfully for Bulk Booking Request No. -" + travelReqId.Split(',')[0];
                //sendEmailNotification();
            }
            else
            {
                jsonmsg = "Error :Rejection process has not been completed successfully.";
            }
            return (s >= 1 ? jsonmsg : string.Empty);
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
        /// Generate PNR and Send Emal Notification
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <param name="emailId"></param>
        /// <returns></returns>
        public List<BulkEmployeeList> GeneratePNR(string travelReqId)
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

            if (bulkDetailModel.Count > 0)
            {
                //Generate PNR
                pnrList = NavitaireServicesBulkBooking.Generate_PNR(bulkDetailModel, travelReqId.ToString());

                //Update PNR list
                var c = _bulkUploadRepository.UpdatePNRStatusList(int.Parse(travelReqId), pnrList);
            }
            return pnrList;
        }

        public List<BulkEmployeeList> GeneratePNR_edit(string travelReqId, Int32 addNo)
        {
            string msg = string.Empty;
            var pnrList = new List<BulkEmployeeList>();
            var bulkMasterModel = new List<BulkUploadMasterModels>();
            var bulkDetailModel = new List<BulkUploadModels>();

            var dicList = new Dictionary<string, object>();
            dicList = _bulkUploadRepository.GetBulkBookingInfoForPNRGeneration_edit(Convert.ToInt64(travelReqId), addNo);
            bulkDetailModel = dicList["bulkDetailInfoEdit"] as List<BulkUploadModels>;

            //As of now only stand by booking are allowed for HOD Approval 
            var sdAgencyCode = ConfigurationManager.AppSettings["Bulk_Booking_SOD_SDFINAgencyCode"].ToString();
            if (bulkDetailModel[0].AgencyCode == sdAgencyCode)
                bulkDetailModel = bulkDetailModel.Where(x => x.BookingType.ToLower() == "standby").ToList();

            if (bulkDetailModel.Count > 0)
            {
                //Generate PNR
                pnrList = NavitaireServicesBulkBooking.Generate_PNR(bulkDetailModel, travelReqId.ToString());

                //Update PNR list
                var c = _bulkUploadRepository.UpdatePNRStatusList(int.Parse(travelReqId), pnrList);
            }
            return pnrList;
        }


        /// <summary>
        /// Get Bulk Booking User Info
        /// </summary>
        /// <param name="EmpId"></param>
        /// <returns></returns>
        public List<string> GetBulkBookingUserInfo(Int64 TraId)
        {
            var s = SOD.Services.ADO.SodCommonServices.GetBulkBookingUserInfoByTransactionId(TraId, null, 1);
            var empInfo = new List<string> { s[0].EmpId.ToString().Trim(), s[0].EmpName.Trim(), s[0].Gender.Trim(), s[0].Email.Trim() };
            return empInfo;
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
        /// Read Template File from Location
        /// </summary>
        /// <returns></returns>
        private string ReadFile(string emailTemplateName, List<BulkUploadModels> blist, string bbReqNo, string ReqName)
        {
            var strContent = new System.Text.StringBuilder();
            string line;
            var file = new System.IO.StreamReader(
                       Server.MapPath("~/Template/BulkTemplate/" + emailTemplateName));
            while ((line = file.ReadLine()) != null)
            {
                strContent.Append(line);
            }
            file.Close();

            strContent = strContent.Replace("[RequesterName]", ReqName);

            strContent = strContent.Replace("[binfo]", "Traveller Info  (SOD-Bulk Req.No. -" + bbReqNo + ")");
            var trp = "";
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
                    trp = trp + "<tr style='font-family:Arial; font-size:12px;'><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + counter + "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + b.EmpCode + "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + name + "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + b.TravelDate + "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + b.Sector + "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + b.FlightNo + "</td> <td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + b.PNR + "</td> <td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + b.BookingType + "</td> <td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + ml + "</td> <td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + bvg + "</td>     <td style='border-top:1px solid #c2c2c2;border-right:solid 1px transparent;height:20px; padding-bottom:8px;'>" + hotelRequired + "</td></tr>";
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
                    trp = trp + "<tr style='font-family:Arial; font-size:12px;'><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + counter + "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + b.EmpCode + "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + name + "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + b.TravelDate + "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + b.Sector + "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + b.FlightNo + "</td> <td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + b.BookingType + "</td> <td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + ml + "</td> <td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + bvg + "</td>     <td style='border-top:1px solid #c2c2c2;border-right:solid 1px transparent;height:20px; padding-bottom:8px;'>" + hotelRequired + "</td></tr>";
                    counter++;
                }
            }
            strContent = strContent.Replace("[tr]", trp);
            return strContent.ToString();
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
                EmailNotifications.SendBookingRequestNotificationTo_HOD(emaildataHod, emailidHod);
            }
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