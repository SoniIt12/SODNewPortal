using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;
using SOD.Services.Repository;
using SOD.CommonWebMethod;
using System.Configuration;
using SOD.Logging;
using System.Web.UI;
using System.Data.Entity.Infrastructure;
using SOD.EmailNotification;
using System.Text;
using System.IO;
using System.Drawing;
using System.Web.Script.Serialization;

namespace SOD.Controllers
{
    public class ChangeReqController : Controller, IExceptionFilter, IActionFilter
    {
        // GET: ChangeReq
        private readonly IUserRepository _userRepository;
        private readonly IUserChangeRequestHRRepository _changerequest;

        /// <summary>
        /// ChangeReqController Constructor
        /// </summary>
        public ChangeReqController()
        {
            _userRepository = new UserRepository(new SodEntities());
            _changerequest = new UserChangeRequestHRRepository(new SodEntities());
        }

        /// <summary>
        /// Change Request View
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangeRequest()
        {
            string verticalID = getverticalID();
            if (Session["cxo"] != "1")
            {
                var existinghoddetails = _userRepository.GetHODEmailIdDetails(verticalID, 5);
                if (existinghoddetails != "")
                {
                    var hoddetails = existinghoddetails.Split(',');
                    ViewBag.HodEmail = hoddetails[0];
                    ViewBag.HodName = hoddetails[1];
                    ViewBag.HodEmployeeCode = hoddetails[2];
                    ViewBag.HodDesignation = hoddetails[3];
                }
                else
                {
                    ViewBag.HodEmail = "NA";
                    ViewBag.HodName = "NA";
                    ViewBag.HodEmployeeCode = "NA";
                    ViewBag.HodDesignation = "NA";
                }
            }
            else
            {
                ViewBag.HodName = Session["FirstName"] + " " + Session["LastName"];
                ViewBag.HodEmail = Session["Email"];
                ViewBag.HodEmployeeCode = Session["EmpCode"];
                ViewBag.HodDesignation = Session["Designation"];
            }
            return View();
        }


        //Change Request HR Approval 
        public ActionResult ChangeRequestHistoryHR()
        {
            return View();
        }

        //Change Request Finance Approval
        public ActionResult ChangeRequestHistoryFinance()
        {
            return View();
        }

        /// <summary>
        /// To Save User Change Request data in Master and details Table
        /// </summary>
        /// <param name="sodChangeMaster"></param>
        /// <param name="sodChangeDetails"></param>
        /// <returns></returns>
        public JsonResult SaveChangeRequestMaster(SODEmployeeChangeRequestMaster sodChangeMaster, SODEmployeeChangeRequestDetails sodChangeDetails)
        {
            sodChangeMaster.RequestedDate = DateTime.Now;
            var savedReqID = _changerequest.SaveChangeRequest(sodChangeMaster, sodChangeDetails);
            if (savedReqID != -1)
            {
                var emailSubject = "SOD Employee Change Request Notification :" + System.DateTime.Now.ToString();
                var emailTemplateName_User = "SodUserChangeRequest.html";
                var emailTemplateName_HR = "SODHRChangeRequest_Approval_Rejection.html";
                var verticalID = getverticalID();
                TempData["hod"] = _userRepository.GetHODEmailIdDetails(verticalID, 5);//VerticalId
                TempData["emailId"] = sodChangeMaster.EmailID;
                var userEmailId = sodChangeMaster.EmailID;
                var hrEmailId = _changerequest.GetHREmail(Convert.ToInt32(Session["DeptCR"]));
                var financeEmailID = _changerequest.GetFinanceEmailId(Convert.ToInt32(Session["DeptIdCR"]));
                TempData["emailId_HR"] = hrEmailId;
                if (userEmailId.Trim() == hrEmailId.Trim())
                {
                    SendChangeRequestToFinance(savedReqID, "Approve");
                    TempData["successmsg"] = "Your Request has been sent successfully to Finance Team.";
                }
                else if (userEmailId.Trim() == financeEmailID.Trim())
                {
                    _changerequest.CRMailData(savedReqID, "Approve", "");
                    _changerequest.SaveHRApproval(savedReqID, "Approve");
                    ConfirmationMailtoUser(savedReqID, "Ok", sodChangeDetails.UserRemarks);
                    TempData["successmsg"] = "Request is initiated.";
                }
                else
                {
                    TempData["emailData_HR"] = EmailCredentials(emailSubject, emailTemplateName_HR, sodChangeMaster, sodChangeDetails, sodChangeDetails.RequestTypeId, 2);
                    TempData["successmsg"] = "Your Request has been sent successfully to HR Team.";
                }
                TempData["emailData"] = EmailCredentials(emailSubject, emailTemplateName_User, sodChangeMaster, sodChangeDetails, sodChangeDetails.RequestTypeId, 1);

            }
            return Json("", JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Success Page View
        /// </summary>
        /// <returns></returns>
        public ActionResult SuccessPage()
        {
            return View();
        }

        /// <summary>
        /// HOD Details
        /// </summary>
        /// <param name="idd"></param>
        /// <returns></returns>
        public JsonResult HodDetails(int idd)
        {
            var data = _userRepository.GetEmployeeList(idd);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Get Employee Vertical ID
        /// </summary>
        /// <returns></returns>
        public string getverticalID()
        {
            var data = _userRepository.GetEmployeeList(Convert.ToInt32(Session["EmpId"]));
            return data[0].EmployeeVertical;
        }

        /// <summary>
        /// Change Request History
        /// </summary>
        /// <returns></returns>
        public JsonResult changeRequestHistory()
        {
            var data = _changerequest.getchangeRequestData(Convert.ToString(Session["EmpCode"]));
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// HR Change Request Data
        /// </summary>
        /// <returns></returns>
        public JsonResult HRRightsData()
        {
            var data = _changerequest.ChangeRequestHRRights(Session["EmpCode"].ToString());
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// HR Change Request Data History
        /// </summary>
        /// <returns></returns>
        public JsonResult HRRightsDataHistory()
        {
            var data = _changerequest.ChangeRequestHRRightsHistory(Session["EmpCode"].ToString());
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Finance Change Request Data
        /// </summary>
        /// <returns></returns>
        public JsonResult FinanceRightsData()
        {
            var data = _changerequest.ChangeRequestFinanceRights();
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Finance Change Request Data HIstory
        /// </summary>
        /// <returns></returns>
        public JsonResult FinanceRightsDataHistory()
        {
            var data = _changerequest.ChangeRequestFinanceRightsHistory();
            return Json(data, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// Resend Mail to HR
        /// </summary>
        /// <param name="ReqId"></param>
        /// <returns></returns>
        public JsonResult ResendMailtoHRConfirmation(Int64 ReqId)
        {
            string msg = "";
            var resendData = _changerequest.ResendMailtoHRData(ReqId);
            if (resendData != null && resendData.ToString() != "")
            {
                SODEmployeeChangeRequestMaster sodmaster = new SODEmployeeChangeRequestMaster
                {
                    ReqId = resendData.ReqId,
                    RequestedEmpName = resendData.RequestedEmpName,
                    PhoneNo = resendData.Phone,
                    EmailID = resendData.EmailID,
                    RequestedEmpId = resendData.EmployeCode,
                    DesignationName = resendData.Designation
                };

                SODEmployeeChangeRequestDetails soddetails = new SODEmployeeChangeRequestDetails
                {
                    RequestTypeId = resendData.RequestTypeId,
                    UserRemarks = resendData.UserRemarks,
                    CRUpdatet1 = resendData.CR1Update,
                    CRUpdatet2 = resendData.CR2Update,
                    CRUpdatet3 = resendData.CR3Update
                };

                if (_changerequest.GetHREmail(Convert.ToInt32(Session["DeptCR"])) == "")
                    msg = "Sorry, HR right is not allocated for this Department.";

                else
                {
                    var emailSubject = "SOD Employee Change Request Notification :" + System.DateTime.Now.ToString();
                    var emailTemplateName_HR = "SODHRChangeRequest_Approval_Rejection.html";
                    TempData["emailId_HR"] = _changerequest.GetHREmail(Convert.ToInt32(Session["DeptCR"]));
                    TempData["emailData_HR"] = EmailCredentials(emailSubject, emailTemplateName_HR, sodmaster, soddetails, soddetails.RequestTypeId, 2);
                    sendEmailNotification();
                    msg = "Your request has been resent to HR for updation";
                }
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// Send Mail to Finance View
        /// </summary>
        /// <param name="ReqId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ActionResult SendMailToFinance(Int64 ReqId, string status)
        {
            //if (_changerequest.SendMAiltoFinanceCheck(ReqId))
            //{
            //    ViewBag.Na = "NA";
            //}
            //else
            //{
            //    ViewBag.Status = status;
            //}
            ViewBag.Status = status;
            ViewBag.ReqId = ReqId;
            return View();
        }


        /// <summary>
        /// Send Mail Of Change Request Data to Finance
        /// </summary>
        /// <param name="ReqId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public JsonResult SendChangeRequestToFinance(Int64 ReqId, string status)
        {
            Int64 flag = 0;
            if (status != null && status != "")
            {
                flag = _changerequest.SaveHRApproval(ReqId, status);
                if (flag != 0)
                {
                    var MailData = _changerequest.CRMailData(ReqId, status, "");
                    if (MailData != null)
                    {
                        SODEmployeeChangeRequestMaster sodmaster = new SODEmployeeChangeRequestMaster
                        {
                            ReqId = MailData.ReqId,
                            RequestedEmpName = MailData.RequestedEmpName,
                            PhoneNo = MailData.Phone,
                            EmailID = MailData.EmailID,
                            RequestedEmpId = MailData.EmployeCode,
                            DesignationName = MailData.Designation
                        };
                        SODEmployeeChangeRequestDetails soddetails = new SODEmployeeChangeRequestDetails
                        {
                            RequestTypeId = MailData.RequestTypeId,
                            UserRemarks = MailData.UserRemarks,
                            CRUpdatet1 = MailData.CR1Update,
                            CRUpdatet2 = MailData.CR2Update,
                            CRUpdatet3 = MailData.CR3Update
                        };
                        if (status == "Approve")
                        {
                            var emailSubject = "SOD Employee Change Request Notification :" + System.DateTime.Now.ToString();
                            var emailTemplateName_Finance = "SODHRChangeRequest_Approval_Rejection.html";
                            TempData["emailId_HR"] = _changerequest.GetFinanceEmailId(Convert.ToInt32(Session["DeptCR"]));
                            TempData["emailData_HR"] = EmailCredentials(emailSubject, emailTemplateName_Finance, sodmaster, soddetails, soddetails.RequestTypeId, 4);
                        }
                        else
                        {
                            var emailSubject = "SOD Employee Change Request Update Notification From HR :" + System.DateTime.Now.ToString();
                            var emailTemplateName_User = "SodChangeRequestConfirmationFromHR.html";
                            TempData["emailId"] = sodmaster.EmailID;
                            TempData["emailData"] = EmailCredentials(emailSubject, emailTemplateName_User, sodmaster, soddetails, soddetails.RequestTypeId, 6);
                        }
                        sendEmailNotification();
                    }
                }
            }
            //return Json("",JsonRequestBehavior.AllowGet);
            return Json(flag != 0 && status == "Approve" ? "1" : flag != 0 && status == "Reject" ? "2" : "0", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Confirmation Mail View
        /// </summary>
        /// <param name="ReqId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ActionResult confirmationMail(Int64 ReqId, string status)
        {
            ViewBag.ReqId = ReqId;
            ViewBag.Status = status;
            return View();
        }

        public JsonResult SendMailUserFromFinanceExists(Int64 ReqId)
        {
            return Json(_changerequest.SendMAiltoUserCheck(ReqId) ? 1 : 0, JsonRequestBehavior.AllowGet);
        }

        public JsonResult checkRole()
        {
            var role = _changerequest.CheckRoleOfUser(Convert.ToString(Session["EmpCode"]));
            return Json(role, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Confirmation Mail to User
        /// </summary>
        /// <param name="ReqId"></param>
        /// <param name="status"></param>
        /// <param name="remarks"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ConfirmationMailtoUser(Int64 ReqId, string status, string remarks)
        {

            var confirmdata = _changerequest.CRMailData(ReqId, status, remarks);
            if (confirmdata != null && confirmdata.ToString() != "")
            {
                SODEmployeeChangeRequestMaster sodmaster = new SODEmployeeChangeRequestMaster
                {
                    ReqId = confirmdata.ReqId,
                    RequestedEmpName = confirmdata.RequestedEmpName,
                    PhoneNo = confirmdata.Phone,
                    EmailID = confirmdata.EmailID,
                    RequestedEmpId = confirmdata.EmployeCode
                };
                SODEmployeeChangeRequestDetails soddetails = new SODEmployeeChangeRequestDetails
                {
                    RequestTypeId = confirmdata.RequestTypeId,
                    UserRemarks = confirmdata.UserRemarks,
                    CRUpdatet1 = confirmdata.CR1Update,
                    CRUpdatet2 = confirmdata.CR2Update,
                    CRUpdatet3 = confirmdata.CR3Update,
                    FinanceApprovalRemarks = confirmdata.RemarksFin

                };
                TempData["emailId"] = sodmaster.EmailID;
                var emailSubject = "SOD Employee Change Request Update Notification From Finance/SAP Team :" + System.DateTime.Now.ToString();
                var emailTemplateName_User = "SodChangeRequestConfirmationFromHR.html";
                if (status == "Ok")
                    TempData["emailData"] = EmailCredentials(emailSubject, emailTemplateName_User, sodmaster, soddetails, soddetails.RequestTypeId, 3);
                else
                    TempData["emailData"] = EmailCredentials(emailSubject, emailTemplateName_User, sodmaster, soddetails, soddetails.RequestTypeId, 5);

                sendEmailNotification();
            }
            return Json(status == "Ok" ? "Request has been approved successfully and confirmation email has sent to " + TempData["emailId"] + "" : "Request has not  been approved and  has sent " + TempData["emailId"] + "", JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Confirmation Mail Remarks
        /// </summary>
        /// <param name="HRApprovalRemarks"></param>
        /// <param name="ReqId"></param>
        /// <returns></returns>
        public JsonResult confirmationMailRemarks(string HRApprovalRemarks, Int64 ReqId)
        {
            _changerequest.updateconfirmationMailRemarks(HRApprovalRemarks, ReqId);
            return Json("Thanks for the Remarks", JsonRequestBehavior.AllowGet);
        }

        //Sod User Check Dp
        public JsonResult ExitsDp(string EmpCode)
        {
            bool check = _changerequest.checkdp(EmpCode);
            return Json(check, JsonRequestBehavior.AllowGet);
        }

        //Sod User Profile Dp
        public JsonResult SetDp(SodUserProfileDp model)
        {
            Int64 id = _changerequest.UserSetDp(model, Session["EmpCode"].ToString());
            return Json(id != 0 ? "Profile picture uploaded!!" : "Failed to uploaded!!", JsonRequestBehavior.AllowGet);
        }

        //Sod User Profile Dp Preview
        public JsonResult ShowDP(string EmpCode)
        {
            var userdata = _changerequest.DisplayDp(EmpCode);
            string base64String = "";
            byte[] imagedp = null;
            if (userdata != null && userdata.ToString() != "")
            {
                imagedp = userdata.DpImage;
                base64String = Convert.ToBase64String(imagedp, 0, imagedp.Length);
            }
            return Json(base64String != null ? base64String : "", JsonRequestBehavior.AllowGet);
        }


        //Sod User Profile Dp Remove
        public JsonResult DeleteDP(string EmpCode)
        {
            _changerequest.RemoveDP(EmpCode);
            return Json("", JsonRequestBehavior.AllowGet);
        }


        //Dynamic Menu bind as per user role 
        public JsonResult LoadMenu()
        {
            List<SodUserMenu> menulist = _changerequest.AllMenus(Convert.ToString(Session["Role"]));
            return Json(menulist, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Send Email to User as well as HOD
        /// </summary>
        public void sendEmailNotification()
        {
            if (TempData["emailData"] != null && TempData["emailId"] != null)
            {
                var emaildata = TempData["emailData"] as EmailNotificationModel;
                var emailid = TempData["emailId"].ToString();
                EmailNotifications.SendBookingRequestNotificationTo_Requester(emaildata, emailid);
            }
            if (TempData["emailData_HR"] != null && TempData["emailId_HR"] != null)
            {
                var emaildataHod = TempData["emailData_HR"] as EmailNotificationModel;
                var emailidHod = TempData["emailId_HR"].ToString();
                EmailNotifications.SendBookingRequestNotificationTo_HOD(emaildataHod, emailidHod);
            }
        }


        /// <summary>
        /// Email Credentails
        /// </summary>
        /// <param name="subjectName"></param>
        /// <param name="emailTemplateName"></param>
        /// <param name="sod_data"></param>
        /// <param name="sodChangeDetails"></param>
        /// <param name="reqtypeId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public EmailNotificationModel EmailCredentials(string subjectName, string emailTemplateName, SODEmployeeChangeRequestMaster sod_data, SODEmployeeChangeRequestDetails sodChangeDetails, int reqtypeId, Int16 criteria)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtpUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtptdPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtptdServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtptdPort"].Trim(),
                TemplateFilePath = ReadFile(emailTemplateName, sod_data, sodChangeDetails, reqtypeId, criteria),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }


        /// <summary>
        /// Read File
        /// </summary>
        /// <param name="emailTemplateName"></param>
        /// <param name="sod_data"></param>
        /// <param name="sodChangeDetails"></param>
        /// <param name="reqtypeId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        private string ReadFile(string emailTemplateName, SODEmployeeChangeRequestMaster sod_data, SODEmployeeChangeRequestDetails sodChangeDetails, int reqtypeId, Int16 criteria)
        {
            var strContent = new StringBuilder();
            string line;
            string detailname = reqtypeId == 1 ? "personal" : "HOD";
            var tr = "";
            var UserRole = checkRole();
            string UserType = UserRole.Data .ToString();
            string context = string.Empty;
            //Personal           
            if (criteria == 1)
            {
                var file = new System.IO.StreamReader(Server.MapPath("~/Template/HR" + "//" + emailTemplateName));
                while ((line = file.ReadLine()) != null)
                {
                    strContent.Append(line);
                }
                file.Close();             
               
                strContent = strContent.Replace("[user]", sod_data.RequestedEmpName);
                strContent = strContent.Replace("[DetailName]", detailname);
                strContent = strContent.Replace("[ReqId]", Convert.ToString(sod_data.ReqId));
                if(UserType == "user")
                {
                    context = "sent to HR team";
                }
                else if (UserType == "hr")
                {
                    context = "sent to Finance team";
                }
                else
                {
                    context = "initiated";
                }
                strContent = strContent.Replace("[Team]", context);
                if (reqtypeId == 1)
                {
                    tr = "<tr style='background-color:#EE1D23;color:white;text-align:left;'><th style='padding-left:10px;border-right:solid 1px #c2c2c2;'>Email ID</th><th style='padding-left:10px;border-right:solid 1px #c2c2c2;'>Phone Number</th><th style='padding-left:10px;border-right:solid 1px #c2c2c2;'>Remarks</th></tr>";
                    tr += "<tr style='font-family:Arial; font-size:12px;'><td>" + sod_data.EmailID + "</td><td>" + sod_data.PhoneNo + "</td><td>" + sodChangeDetails.UserRemarks + "</td></tr>";
                }
                else
                {
                    tr = "<tr style='background-color:#EE1D23;color:white;text-align:left;'><th style='padding-left:10px;border-right:solid 1px #c2c2c2;'>HOD Employee Code</th><th style='padding-left:10px;border-right:solid 1px #c2c2c2;'>HOD Name</th><th style='padding-left:10px;border-right:solid 1px #c2c2c2;'>Designation</th><th style='padding-left:10px;border-right:solid 1px #c2c2c2;'>Remarks</th></tr>";
                    tr += "<tr style='font-family:Arial; font-size:12px;'><td>" + sodChangeDetails.CRUpdatet1 + "</td><td>" + sodChangeDetails.CRUpdatet2 + "</td><td>" + sodChangeDetails.CRUpdatet3 + "</td><td>" + sodChangeDetails.UserRemarks + "</td></tr>";
                }
                strContent = strContent.Replace("[tr]", tr);
            }
            else if (criteria == 2 || criteria == 4)
            {
                var span = "";
                var file = new System.IO.StreamReader(Server.MapPath("~/Template/HR" + "//" + emailTemplateName));
                while ((line = file.ReadLine()) != null)
                {
                    strContent.Append(line);
                }
                file.Close();
                strContent = strContent.Replace("[team]", criteria == 2 ? "HR" : "Finance");
                strContent = strContent.Replace("[DetailName]", detailname);
                strContent = strContent.Replace("[RequesterName]", sod_data.RequestedEmpName + "(ECN-" + sod_data.RequestedEmpId + " " + "|" + sod_data.DesignationName + ")");
                strContent = strContent.Replace("[content]", criteria == 2 ? "as mentioned below" : "as approved by HR Team");
                strContent = strContent.Replace("[ReqId]", Convert.ToString(sod_data.ReqId));
                if (reqtypeId == 1)
                {
                    tr = "<tr style='background-color:#EE1D23;color:white;text-align:left;'><th style='padding-left:10px;border-right:solid 1px #c2c2c2;'>Email ID</th><th style='padding-left:10px;border-right:solid 1px #c2c2c2;'>Phone Number</th><th style='padding-left:10px;border-right:solid 1px #c2c2c2;'>Remarks</th></tr>";
                    tr += "<tr style='font-family:Arial; font-size:12px;'><td>" + sod_data.EmailID + "</td><td>" + sod_data.PhoneNo + "</td><td>" + sodChangeDetails.UserRemarks + "</td></tr>";
                }
                else
                {
                    tr = "<tr style='background-color:#EE1D23;color:white;text-align:left;'><th style='padding-left:10px;border-right:solid 1px #c2c2c2;'>HOD Employee Code</th><th style='padding-left:10px;border-right:solid 1px #c2c2c2;'>HOD Name</th><th style='padding-left:10px;border-right:solid 1px #c2c2c2;'>Designation</th><th style='padding-left:10px;border-right:solid 1px #c2c2c2;'>Remarks</th></tr>";
                    tr += "<tr style='font-family:Arial; font-size:12px;'><td>" + sodChangeDetails.CRUpdatet1 + "</td><td>" + sodChangeDetails.CRUpdatet2 + "</td><td>" + sodChangeDetails.CRUpdatet3 + "</td><td>" + sodChangeDetails.UserRemarks + "</td></tr>";
                }

                if (criteria == 2)
                {
                    span = "<span><span><a href='" + ConfigurationManager.AppSettings["FinanceMail"] + "?ReqId=[RID]&status=[Approve]'><input type='button' value='Approve' style='padding:8px 40px;border-radius:8px;background-color:#4CAF50;'></a></span>";
                    span += "<span><span><a href='" + ConfigurationManager.AppSettings["FinanceMail"] + "?ReqId=[RID]&status=[Reject]'><input type='button' value='Reject' style='padding: 8px 30px; border-radius:8px;background-color:#f44336'></a></span>";
                }
                else
                {
                    span = "<span><span><a href='" + ConfigurationManager.AppSettings["UserconfirmMail"] + "?ReqId=[RID]&status=[ok]'><input type='button' value='Approve' style='padding:8px 40px;border-radius:8px;background-color:#4CAF50;'></a></span>";
                    span += "<span><span><a href='" + ConfigurationManager.AppSettings["UserconfirmMail"] + "?ReqId=[RID]&status=[cancel]'><input type='button' value='Reject' style='padding: 8px 30px; border-radius:8px;background-color:#f44336'></a></span>";
                }
                strContent = strContent.Replace("[tr]", tr);
                strContent = strContent.Replace("[span]", span);
                strContent = criteria == 2 ? strContent.Replace("[Approve]", "Approve") : strContent.Replace("[ok]", "Ok");
                strContent = criteria == 2 ? strContent.Replace("[Reject]", "Reject") : strContent.Replace("[cancel]", "cancel");
                strContent = strContent.Replace("[RID]", Convert.ToString(sod_data.ReqId));
            }
            else
            {
                var file = new System.IO.StreamReader(
                     Server.MapPath("~/Template/HR" + "//" + emailTemplateName));
                while ((line = file.ReadLine()) != null)
                {
                    strContent.Append(line);
                }
                file.Close();

                string status = criteria == 3 ? "Your Request has been updated by Finance Team." : "Your Request has not been approved by Finance Team.";
                if (criteria == 6)
                    status = "Your Request has not been approved by HR Team.";

                strContent = strContent.Replace("[RequesterName]", sod_data.RequestedEmpName);
                strContent = strContent.Replace("[status]", status);
                strContent = strContent.Replace("[ReqId]", Convert.ToString(sod_data.ReqId));
                string approvalMessage = criteria == 3 ? "In SOD system, It will be reflected after <b> 2:00 </b> hours." : "Sorry, We are not able to process your request.";
                strContent = strContent.Replace("[approvalMessage]", approvalMessage);
                if (reqtypeId == 1)
                {
                    tr = "<tr style='background-color:#EE1D23;color:white;text-align:left;'><th style='padding-left:10px;border-right:solid 1px #c2c2c2;'>Email ID</th><th style='padding-left:10px;border-right:solid 1px #c2c2c2;'>Phone Number</th><th style='padding-left:10px;border-right:solid 1px #c2c2c2;'>Remarks</th></tr>";
                    tr += "<tr style='font-family:Arial; font-size:12px;'><td>" + sod_data.EmailID + "</td><td>" + sod_data.PhoneNo + "</td><td>" + sodChangeDetails.UserRemarks + "</td></tr>";
                }
                else
                {
                    tr = "<tr style='background-color:#EE1D23;color:white;text-align:left;'><th style='padding-left:10px;border-right:solid 1px #c2c2c2;'>HOD Employee Code</th><th style='padding-left:10px;border-right:solid 1px #c2c2c2;'>HOD Name</th><th style='padding-left:10px;border-right:solid 1px #c2c2c2;'>Designation</th><th style='padding-left:10px;border-right:solid 1px #c2c2c2;'>Remarks</th></tr>";
                    tr += "<tr style='font-family:Arial; font-size:12px;'><td>" + sodChangeDetails.CRUpdatet1 + "</td><td>" + sodChangeDetails.CRUpdatet2 + "</td><td>" + sodChangeDetails.CRUpdatet3 + "</td><td>" + sodChangeDetails.UserRemarks + "</td></tr>";
                }
                strContent = strContent.Replace("[tr]", tr);

                strContent = strContent.Replace("[FinRemarks]", sodChangeDetails.FinanceApprovalRemarks == null ? "" : "Finance Remarks: " + sodChangeDetails.FinanceApprovalRemarks + "");
            }
            return strContent.ToString();
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
            Logging.ErrorLog.Instance.AddDBLogging(ex, filterContext.Controller.ControllerContext.ToString(), "Controllers/ChangeRequestController.cs");
        }
    }
}