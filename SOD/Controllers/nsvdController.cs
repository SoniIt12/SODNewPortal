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
using System.IO;
using System.Configuration;
using System.ComponentModel;
using SOD.EmailNotification;
using SOD.CommonWebMethod;

namespace SOD.Controllers
{
    public class nsvdController : Controller, IExceptionFilter
    {
        private IVendorApprovalRepository _vendorApprovalRepository;
        private IVendorRepository _vendorRepository;
        private IUserRepository _userRepository;
        private readonly ISodApproverRepositorty _sodApproverRepositorty;
        public nsvdController()
        {
            _vendorRepository = new VendorRepository(new SodEntities());
            _userRepository = new UserRepository(new SodEntities());
            _vendorApprovalRepository = new VendorApprovalRepository(new SodEntities());
            _sodApproverRepositorty = new SodApproverRepositorty(new SodEntities());
        }
        /// <summary>
        /// For Help Desk
        /// </summary>
        /// <returns></returns>
        public ActionResult vendorhd()
        {
            return View();
        }

        /// <summary>
        /// vd booking history
        /// </summary>
        /// <returns></returns>
        public ActionResult vdbookinghistory()
        {
            return View();
        }
        /// <summary>
        /// For User
        /// </summary>
        /// <returns></returns>
        public ActionResult vendor()
        {
            return View();
        }
        /// <summary>
        /// For User And Approver
        /// </summary>
        /// <returns></returns>
        public ActionResult vendorlist()
        {
            return View();
        }

        /// <summary>
        /// Get VD Booking Report
        /// </summary>
        /// <returns></returns>
        public ActionResult GetVDBookingReport()
        {
            return View();
        }

        /// <summary>
        /// Get Vendor List
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetVendorList()
        {
            try
            {
                if (Convert.ToInt16(Session["EmpId"]) == -2)
                {
                    var VendorList = _vendorRepository.GetVendorList();
                    return Json(VendorList, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string Empcode = Session["EmpCode"].ToString();
                    var VendorList = _vendorRepository.GetVendorList().Where(n => n.ReqEmpCode == Empcode).ToList();
                    TempData["datewisedata"] = VendorList;
                    return Json(VendorList, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public JsonResult AddUpdateVendor(VendorModels Model)
        {
            var UseInfo = GetLoginUserInfo(Session["EmpId"].ToString());
            if (Model.ID > 0)
            {
                Model.ModifiedDate = DateTime.Now;
            }
            else
            {
                Model.ReqEmpCode = UseInfo[1];
                Model.ReqEmpName = UseInfo[2];
                Model.ReqEmpTitle = UseInfo[3].Equals("M") ? "Mr." : "Ms.";
                Model.ReqEmpEmailID = UseInfo[4];
                Model.ReqMobile = UseInfo[5];
                Model.AddDate = DateTime.Now;
            }
            Model.IsActive = true;
            return Json(_vendorRepository.AddUpdateVendor(Model), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteVendor(int Id)
        {
            return Json(_vendorRepository.DeleteVendor(Id), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult IsExistEmailOrMobile(string Email, string Mobile, int Id)
        {
            return Json(_vendorRepository.IsExistEmailOrMobile(Email, Mobile, Id), JsonRequestBehavior.AllowGet);
        }
        public JsonResult getdatewiseList()
        {
            var fromdate = Convert.ToDateTime(Request.QueryString["prm"].Split(',')[0]);
            var todate = Convert.ToDateTime(Request.QueryString["prm"].Split(',')[1]);
            string Empcode = Session["EmpCode"].ToString();
            var VendorList = _vendorRepository.GetVendorList().Where(n => n.ReqEmpCode == Empcode && n.AddDate >= fromdate && n.AddDate <= todate).ToList();
            TempData["datewisedata"] = VendorList;
            return Json(VendorList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Export Data
        /// </summary>
        public void ExportVendorData()
        {
            var data = TempData["datewisedata"] as List<VendorModels>;
            var exportData = new List<ExportVendorExcel>();
            var count = 0;
            foreach (var i in data)
            {
                count++;
                var lst = new ExportVendorExcel();
                lst.SNo = count;
                lst.VendorCode = i.VendorCode;
                lst.FirstName = i.FirstName;
                lst.LastName = i.LastName;
                lst.Gender = i.Gender;
                lst.EmailId = i.EmailId;
                lst.MobileNo = i.MobileNo;
                lst.ReqEmpCode = i.ReqEmpCode;
                lst.ReqEmpName = i.ReqEmpName;
                lst.AddDate = i.AddDate;
                lst.ModifiedDate = i.ModifiedDate;
                lst.IsActive = i.IsActive;
                if (i.IsApproved == 0)
                {
                    lst.IsApproved = "Pending";
                }
                else if (i.IsApproved == 1)
                {
                    lst.IsApproved = "Approved";
                }
                else
                    lst.IsApproved = "Rejected";
                lst.ApprovedbyEmpEmailID = i.ApprovedbyEmpEmailID;
                lst.Approvaldate = i.Approvaldate;
                lst.CompanyName = i.CompanyName;
                exportData.Add(lst);
            }
            var arr = exportData.ToArray();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment;filename=VendorMasterList.xls");
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            WriteTsv(arr, Response.Output);
            Response.End();
        }

        /// <summary>
        /// WriteTsv
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="output"></param>
        public void WriteTsv<T>(IEnumerable<T> data, TextWriter output)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            foreach (PropertyDescriptor prop in props)
            {
                output.Write(prop.DisplayName);  
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
        /// Send mail for Approval
        /// </summary>
        /// <param name="mailerlist"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult sendmailforApproval(List<VendorModels> mailerlist)
        {
            var jsonmsg = string.Empty;
            var subject = string.Empty;
            var VendoremailTemplateName = string.Empty;
            try
            {
                var batchId = _vendorRepository.UpdateBatchId(mailerlist);
                if (batchId > 0)
                {
                    var loggedUserInfo = GetLoginUserInfo(Session["EmpId"].ToString().Trim());
                    List<VendorHODDetails> approverDetails = Hoddetails(loggedUserInfo[4].ToString());
                    //***Wrire Email to User  
                    subject = "NONSOD Vendor Approval Request Notification :" + System.DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss tt");
                    VendoremailTemplateName = "VendorRequestEmailTemplate.html";
                    WriteEmailCredentialsForUser(subject, VendoremailTemplateName, mailerlist, approverDetails, loggedUserInfo);
                    //***Wrire Email to HOD/Approver
                    subject = "NONSOD Vendor Approval Request Notification  :" + System.DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss tt");
                    VendoremailTemplateName = "VendorApprovalEmailTemplate.html";
                    WriteEmailCredentialsForApprover(subject, VendoremailTemplateName, mailerlist, approverDetails, loggedUserInfo, batchId);
                    sendEmailNotification();
                    jsonmsg = "Email has been sent succesfully to your respected HOD -  " + approverDetails[0].ApproverEmailID;
                }
                else
                {
                    jsonmsg = "Internal server error";
                }
            }
            catch (Exception ex)
            {
                jsonmsg = ex.InnerException.Message;
            }
            return Json(jsonmsg, JsonRequestBehavior.AllowGet);
        }

        private void WriteEmailCredentialsForUser(string subject, string VendoremailTemplateName, List<VendorModels> mailerlist, List<VendorHODDetails> approverDetails, List<string> loggedUserInfo)
        {
            var emailCredentials = EmailCredentials(subject, VendoremailTemplateName, mailerlist);
            var templateData = emailCredentials.TemplateFilePath;
            templateData = templateData.Replace("[RequesterName]", loggedUserInfo[2]);
            templateData = templateData.Replace("[hodname]", approverDetails[0].ApproverTitle  + " " + approverDetails[0].ApproverName);
            emailCredentials.TemplateFilePath = templateData;
            TempData["emailData"] = emailCredentials;
            TempData["emailId"] = loggedUserInfo[4]; 
        }

        private void WriteEmailCredentialsForApprover(string subject, string VendoremailTemplateName, List<VendorModels> mailerlist, List<VendorHODDetails> approverDetails, List<string> loggedUserInfo, int batchId)
        {
            var emailCredentials = EmailCredentials(subject, VendoremailTemplateName, mailerlist);
            var SODtemplateData = emailCredentials.TemplateFilePath;
            SODtemplateData = SODtemplateData.Replace("[RequesterName]", approverDetails[0].ApproverTitle + " " + approverDetails[0].ApproverName);
            SODtemplateData = SODtemplateData.Replace("[hodname]", loggedUserInfo[3] + " " +loggedUserInfo[2]);
            var appLink = string.Empty;
            var approvaltype = string.Empty;
            var skey = new StringBuilder();
            skey.Append(batchId + ",");
            skey.Append(approverDetails[0].ApproverEmailID + ",");
            skey.Append(approverDetails[0].ApproverTitle +" "+ approverDetails[0].ApproverName + ",");
            var uri1 = ConfigurationManager.AppSettings["emailApprovalPathHodVendor"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=Approved");
            var uri2 = ConfigurationManager.AppSettings["emailApprovalPathHodVendor"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=Rejected");
            approvaltype = "Please help to accord your Approval or Rejection.";
            appLink = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a name='app' style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Approve</a></td> <td>&nbsp;</td> <td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a name='rej' style='color:#fff; text-decoration:none;' href='" + uri2 + "'>Reject</a> </td></tr></table>";
            SODtemplateData = SODtemplateData.Replace("[approvaltype]", approvaltype);
            SODtemplateData = SODtemplateData.Replace("[appLink]", appLink);
            emailCredentials.TemplateFilePath = SODtemplateData;
            TempData["emailData_Hod"] = emailCredentials;
            TempData["emailId_Hod"] = approverDetails[0].ApproverEmailID;
        }

        public List<VendorHODDetails> Hoddetails(string UserEmailID)
        {
            List<VendorHODDetails> data = new List<VendorHODDetails>();
            return data = _vendorRepository.Hoddetails(UserEmailID);
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
        /// Get logged User Info        
        /// <param name="empId"></param>
        public List<string> GetLoginUserInfo(string empId)
        {
            var s = SOD.Services.ADO.SodCommonServices.GetEmployeeCommonDetails(int.Parse(empId));
            var empInfo = new List<string> { s[0].EmpId.ToString().Trim(), s[0].EmpCode.Trim(), s[0].EmpName.Trim(), s[0].Gender.Trim(), s[0].Email.Trim(), s[0].Phone.Trim() };
            return empInfo;
        }

        public JsonResult NonSODUserRoleDetails()
        {
            var s = SOD.Services.ADO.SodCommonServices.NonSODUserRoleDetails();
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddUpdateVendorhd(VendorModels Model)
        {
            var userinfo = (NonSODUserRoleDetails().Data) as List<EmployeeModel>;
            var userDetail = userinfo.Where(x => x.Email == Model.ReqEmpEmailID).ToList();
            if (Model.ID > 0)
            {
                Model.ModifiedDate = DateTime.Now;
                Model.AddVendorOnBehalfof = "Helpdesk";
            }
            else
            {
                Model.ReqEmpCode = userDetail[0].EmpCode;
                Model.ReqEmpName = userDetail[0].EmpName;
                Model.ReqEmpEmailID = userDetail[0].Email;
                Model.ReqMobile = userDetail[0].Phone;
                Model.AddDate = DateTime.Now;
                Model.AddVendorOnBehalfof = "Helpdesk";
            }
            Model.IsActive = true;
            return Json(_vendorRepository.AddUpdateVendor(Model), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult NonSODHODdetails(string hodemailid)
        {
            var HODdetails = _vendorRepository.Hoddetails(hodemailid);
            return Json(HODdetails, JsonRequestBehavior.AllowGet);
        }
        public EmailNotificationModel EmailCredentials(string subjectName, string VendoremailTemplateName, List<VendorModels> mailerlist)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtpUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtpPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtpServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtpPort"].Trim(),
                TemplateFilePath = ReadFiletohodForvenderApproval(subjectName, VendoremailTemplateName, mailerlist),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }

        private string ReadFiletohodForvenderApproval(string subjectName, string VendoremailTemplateName, List<VendorModels> mailerlist)
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
            var tr = "<tr style='background-color:#ec1c24;color:#fff;font-family:Arial; font-size:12px;font-weight:bold;'><td style='height:20px; padding-bottom:8px;'>S. No.</td><td style='height:20px; padding-bottom:8px;'>Vender Code</td><td style='height:20px; padding-bottom:8px;'>First Name</td><td height:20px; padding-bottom:8px;'>Last Name</td><td style='height:20px; padding-bottom:8px;'>Gender</td><td style='height:20px; padding-bottom:8px;'>E-mail Id</td> <td style='height:20px; padding-bottom:8px;'>Mobile No.</td>";
            foreach (var lst in mailerlist)
            {
                tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + counter + "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + lst.VendorCode + "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + lst.FirstName + "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + lst.LastName + "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + lst.Gender + "</td><td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + lst.EmailId + "</td> <td style='border-top:1px solid #c2c2c2;height:20px; padding-bottom:8px;'>" + lst.MobileNo + "</td>";
                counter++;
            }
            strContent = strContent.Replace("[tr]", tr);
            return strContent.ToString();
        }
        [HttpGet]
        /// <summary>
        /// Exception Handling
        /// </summary>
        /// <param name="filterContext"></param>
        new void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            Logging.ErrorLog.Instance.AddDBLogging(ex, filterContext.Controller.ControllerContext.ToString(), "Controllers/nsvdController.cs");
        }
    }
}
