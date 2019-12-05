using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SOD.Model;
using SOD.Services;
using SOD.Services.Interface;
using SOD.Services.Repository;
using SOD.Services.EntityFramework;
using System.Configuration;
using SOD.EmailNotification;
using System.Data.Entity.Infrastructure;
using SOD.CommonWebMethod;

namespace SOD.Controllers
{
    [HandleError(ExceptionType = typeof(DbUpdateException), View = "Error")]
    public class nsvdappController : Controller, IExceptionFilter
    {
        private IVendorApprovalRepository _vendorApprovalRepository;
        private IVendorRepository _vendorRepository;
        private IUserRepository _userRepository;
        private readonly ISodApproverRepositorty _sodApproverRepositorty;
        public nsvdappController()
        {
            _vendorApprovalRepository = new VendorApprovalRepository(new SodEntities());
            _vendorRepository = new VendorRepository(new SodEntities());
            _userRepository = new UserRepository(new SodEntities());
            _sodApproverRepositorty = new SodApproverRepositorty(new SodEntities());
        }
        // GET: nsvdapp
        public ActionResult vendorlist()
        {
            return View();
        }       

        public ActionResult vdapplist()
        {
            if (Session["EmpId"] != null)
            {
                SodBookingList(Convert.ToInt16(Session["DeptIdCR"]), Convert.ToInt16(Session["DesigIdM"]));
                if (Session["SjsUserId"] != null && Session["HodEmailId"] != null)
                {
                    string SjscHod = Session["HodEmailId"].ToString();
                    SjscBookingList(SjscHod);
                }
            }
            return View();
        }

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
        /// Approval over the Email
        /// </summary>
        /// <returns></returns>
        public ActionResult appvendor()
        {
            var appController = DependencyResolver.Current.GetService<nsvdController>();
            var strquery = CipherURL.Decrypt(Request.QueryString[0].ToString());
            var BatchID = strquery.Split(',')[0];
            var ApproverID = strquery.Split(',')[1];
            var ApproverName = strquery.Split(',')[2];
            var type = strquery.Split('&')[1].Split('=')[1];
            //check if already approved or not ?
            var msg = string.Empty;
            if (type == "Approved")
            {
                msg = _vendorApprovalRepository.AppvendorList(Convert.ToInt32(BatchID), ApproverID, type);
            }
            else
            {
                msg = _vendorApprovalRepository.RejectVendorList(Convert.ToInt32(BatchID), ApproverID, type);
            }
            if (msg.Length < 32)
            {
                List<VendorModels> list = _vendorApprovalRepository.getlistasperBatchID(Convert.ToInt32(BatchID));
                SendEmailToUser(list, list[0].ReqEmpName, list[0].ReqEmpEmailID, ApproverName, type);
            }
            return View(TempData["jsonmsg"] = msg);
        }

        public EmailNotificationModel EmailCredentials(string subjectName, string VendoremailTemplateName, List<VendorModels> mailerlist)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtpUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtpPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtpServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtpPort"].Trim(),
                TemplateFilePath = ReadFile(subjectName, VendoremailTemplateName, mailerlist),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }
        public List<string> GetLoginUserInfo(string empId)
        {
            var s = SOD.Services.ADO.SodCommonServices.GetEmployeeCommonDetails(int.Parse(empId));
            var empInfo = new List<string> { s[0].EmpId.ToString().Trim(), s[0].EmpName.Trim(), s[0].Gender.Trim(), s[0].Email.Trim() };
            return empInfo;
        }
        private string ReadFile(string subjectName, string VendoremailTemplateName, List<VendorModels> mailerlist)
        {
            var strContent = new StringBuilder();
            var file = new System.IO.StreamReader(Server.MapPath("~/Template/Vendor/" + VendoremailTemplateName));
            string line;
            while ((line = file.ReadLine()) != null)
            {
                strContent.Append(line);
            }
            file.Close();
            var counter = 1;
            var tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;font-weight:bold;'><td style='height:20px; padding-bottom:8px;'>S. No.</td><td style='height:20px; padding-bottom:8px;'>Vender Code</td><td style='height:20px; padding-bottom:8px;'>First Name</td><td height:20px; padding-bottom:8px;'>Last Name</td><td style='height:20px; padding-bottom:8px;'>Gender</td><td style='height:20px; padding-bottom:8px;'>E-mail Id</td> <td style='height:20px; padding-bottom:8px;'>Mobile No.</td><td style='height:20px; padding-bottom:8px;'>Approval Status</td>";
            foreach (var lst in mailerlist)
            {
                var approvalstatus = string.Empty;
                if (lst.IsApproved == 0)
                {
                    approvalstatus = "Pending";
                }
                else if (lst.IsApproved == 1)
                {
                    approvalstatus = "Approved";
                }
                else
                    approvalstatus = "Rejected";
           
                tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + counter + "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + lst.VendorCode + "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + lst.FirstName + "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + lst.LastName + "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + lst.Gender + "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + lst.EmailId + "</td> <td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + lst.MobileNo + "</td> <td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + approvalstatus + "</td>";
                counter++;
            }
            strContent = strContent.Replace("[tr]", tr);
            return strContent.ToString();
        }
        public JsonResult GetVendorApprovalList()
        {
            return Json(_vendorApprovalRepository.GetVendorApprovalList(Session["Email"].ToString()), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ApproveAndRejectVendor(List<VendorModels> mailerlist, string Status)
        {
            //string EmpCode = Convert.ToString(Session["user"]).ToLower();
            var UserInfo = GetLoginUserInfo(Session["EmpId"].ToString().Trim());
            var data = _vendorRepository.Hoddetails(UserInfo[3].ToString());
            var apporrejlist = string.Empty;
            var appController = DependencyResolver.Current.GetService<nsvdController>();
            var jsonmsg = string.Empty;
            var subject = string.Empty;
            var VendoremailTemplateName = string.Empty;
            int BatchID = 0;
            int ID = 0;
            List<VendorModels> list = new List<VendorModels>();
            foreach (var i in mailerlist)
            {
                BatchID = i.BatchId;
                ID = (int)(i.ID);
                // BatchID = _vendorRepository.UpdateBatchId(mailerlist);
                if (Status == "approved")
                {
                    apporrejlist = _vendorApprovalRepository.AppvendorList(ID, data[0].ApproverEmailID, Status);
                }
                else
                {
                    apporrejlist = _vendorApprovalRepository.RejectVendorList(ID, data[0].ApproverEmailID, Status);
                }
                if (apporrejlist.Length < 40)
                {
                    list = _vendorApprovalRepository.getlistasperBatchID(BatchID);
                    SendEmailToUser(list, list[0].ReqEmpName, list[0].ReqEmpEmailID, data[0].ApproverName, Status);
                }
            }
            return Json(TempData["jsonmsg"] = apporrejlist, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// Send Email To User
        /// </summary>
        /// <param name="list"></param>
        /// <param name="reqName"></param>
        /// <param name="reqEmailId"></param>
        /// <param name="hodName"></param>
        /// <param name="appStatus"></param>
        private void SendEmailToUser(List<VendorModels> list, string reqName, string reqEmailId, string hodName, string appStatus)
        {
            var subject = "NON SOD Vendor Booking Response Notification  :" + System.DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss tt");
            var VendorResponseTemplateName = "VendorResponseEmailTemplate.html";
            var emailCredentials = EmailCredentials(subject, VendorResponseTemplateName, list);
            var requesttemplateData = emailCredentials.TemplateFilePath;
            requesttemplateData = requesttemplateData.Replace("[RequesterName]", reqName);
            requesttemplateData = requesttemplateData.Replace("[hodname]", hodName);
            requesttemplateData = requesttemplateData.Replace("[AppStatus]", appStatus);
            emailCredentials.TemplateFilePath = requesttemplateData;
            EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials, reqEmailId);
        }

        /// <summary>
        /// Exception Handling
        /// </summary>
        /// <param name="filterContext"></param>
        private void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            Logging.ErrorLog.Instance.AddDBLogging(ex, filterContext.Controller.ControllerContext.ToString(), "Controllers/nsvdController.cs");
        }
    }
}








