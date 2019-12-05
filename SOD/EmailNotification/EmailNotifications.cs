using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using SOD.Model;
using System.Configuration;
using SOD.Logging;
using System.Web.Script.Serialization;

namespace SOD.EmailNotification
{
    /// <summary>
    /// This Class will be used for Send Email Notifications
    /// Author :Satyam
    /// Date :09, Aprl, 2016
    /// Mod Date :05, Jan, 2018
    /// NetCore API Integration
    /// </summary>
    public static class EmailNotifications 
    {
        /// <summary>
        /// Send Email Notification to HOD
        /// </summary>
        /// <param name="objEMailDac"></param>
        public static void SendBookingRequestNotificationTo_HOD(EmailNotificationModel objEMailDac, string senderEmailId)
        {
            string[] senderEmailIds = new string[] { senderEmailId };
            string MailApi = ConfigurationManager.AppSettings["MailApi"];
            var _jsonSerialiser = new JavaScriptSerializer();
            var _jsonRequest = _jsonSerialiser.Serialize
            (new Mail()
                {
                    applicationKey = ConfigurationManager.AppSettings["MailApiKey"],
                    //cc = arCC,
                    from = objEMailDac.SmtpUser,
                    plainTextContent = string.Empty,
                    htmlContent =objEMailDac.TemplateFilePath.Trim(),
                    subject = objEMailDac.EmailSubjectName,
                    to = senderEmailIds
                }
            );
            try
            {
                var s = SendEmailThroughAPI(MailApi, _jsonRequest);
            }
            catch (Exception ex)
            {
                ErrorLog.AddEmailLogg(ex);
                return;
            }
        }


        /// <summary>
        /// Send Email Notification to HOD with CC  
        /// </summary>
        /// <param name="objEMailDac"></param>
        public static void SendBookingRequestNotificationTo_HODCC(EmailNotificationModel objEMailDac, string senderEmailId, string senderEmailIdCC)
        {
            string[] senderEmailIds = new string[] { senderEmailId };
            string[] senderEmailIdCCs = new string[] { senderEmailIdCC };
            
            string MailApi = ConfigurationManager.AppSettings["MailApi"];
            var _jsonSerialiser = new JavaScriptSerializer();
            var _jsonRequest = _jsonSerialiser.Serialize
            (new Mail()
            {
                applicationKey = ConfigurationManager.AppSettings["MailApiKey"],
                cc = senderEmailIdCCs,
                from = objEMailDac.SmtpUser,
                plainTextContent = string.Empty,
                htmlContent = objEMailDac.TemplateFilePath.Trim(),
                subject = objEMailDac.EmailSubjectName,
                to = senderEmailIds
            }
            );
            try
            {
                var s = SendEmailThroughAPI(MailApi, _jsonRequest);
            }
            catch (Exception ex)
            {
                ErrorLog.AddEmailLogg(ex);
                return;
            }
        }

        public static void SendBookingRequestNotificationTo_InCC(EmailNotificationModel objEMailDac, string senderEmailId, string[] senderEmailIdCC)
        {
            string[] senderEmailIds = new string[] { senderEmailId };
            string[] senderEmailIdCCs = senderEmailIdCC ;

            string MailApi = ConfigurationManager.AppSettings["MailApi"];
            var _jsonSerialiser = new JavaScriptSerializer();
            var _jsonRequest = _jsonSerialiser.Serialize
            (new Mail()
            {
                applicationKey = ConfigurationManager.AppSettings["MailApiKey"],
                cc = senderEmailIdCCs,
                from = objEMailDac.SmtpUser,
                plainTextContent = string.Empty,
                htmlContent = objEMailDac.TemplateFilePath.Trim(),
                subject = objEMailDac.EmailSubjectName,
                to = senderEmailIds
            }
            );
            try
            {
                var s = SendEmailThroughAPI(MailApi, _jsonRequest);
            }
            catch (Exception ex)
            {
                ErrorLog.AddEmailLogg(ex);
                return;
            }
        }



        /// <summary>
        /// Send Email Notification to Requester
        /// </summary>
        /// <param name="objEMailDac"></param>
        public static void SendNotificationToRequester(EmailNotificationModel objEMailDac, string senderEmailId, string emailSubject)
        {
            string[] senderEmailIds = new string[] { senderEmailId };
            string MailApi = ConfigurationManager.AppSettings["MailApi"];
            var _jsonSerialiser = new JavaScriptSerializer();
            var _jsonRequest = _jsonSerialiser.Serialize
            (new Mail()
            {
                applicationKey = ConfigurationManager.AppSettings["MailApiKey"],
                //cc = arCC,
                from = objEMailDac.SmtpUser,
                plainTextContent = string.Empty,
                htmlContent = objEMailDac.TemplateFilePath.Trim(),
                subject = emailSubject,
                to = senderEmailIds
            }
            );
            try
            {
                var s = SendEmailThroughAPI(MailApi, _jsonRequest);
            }
            catch (Exception ex)
            {
                ErrorLog.AddEmailLogg(ex);
                return;
            }
        }

        /// <summary>
        /// Send PNR generation Notificationn
        /// </summary>
        /// <param name="objEMailDac"></param>
        public static void SendPnrGenerationNotification(EmailNotificationModel objEMailDac)
        {
            string[] senderEmailIds = new string[] { objEMailDac.SenderEmailId };
            string MailApi = ConfigurationManager.AppSettings["MailApi"];
            var _jsonSerialiser = new JavaScriptSerializer();
            var _jsonRequest = _jsonSerialiser.Serialize
            (new Mail()
            {
                applicationKey = ConfigurationManager.AppSettings["MailApiKey"],
                //cc = arCC,
                from = objEMailDac.SmtpUser,
                plainTextContent = string.Empty,
                htmlContent =objEMailDac.TemplateFilePath.Trim(),
                subject = objEMailDac.EmailSubjectName,
                to = senderEmailIds
            }
            );
            try
            {   
                var s = SendEmailThroughAPI(MailApi, _jsonRequest);
            }
            catch (Exception ex)
            {
                ErrorLog.AddEmailLogg(ex);
                return;
            }
        }


        /// <summary>
        /// Send Email Notification to Requester
        /// </summary>
        /// <param name="objEMailDac"></param>
        public static void SendBookingRequestNotificationTo_Requester(EmailNotificationModel objEMailDac, string senderEmailId)
        {
            string[] senderEmailIds = new string[] { senderEmailId };
            string MailApi = ConfigurationManager.AppSettings["MailApi"];
            var _jsonSerialiser = new JavaScriptSerializer();
            var _jsonRequest = _jsonSerialiser.Serialize
            (new Mail()
            {
                applicationKey = ConfigurationManager.AppSettings["MailApiKey"],
                //cc = arCC,
                from = objEMailDac.SmtpUser,
                plainTextContent = string.Empty,
                htmlContent = objEMailDac.TemplateFilePath.Trim(),
                subject = objEMailDac.EmailSubjectName,
                to = senderEmailIds
            }
            );
            try
            {
                var s = SendEmailThroughAPI(MailApi, _jsonRequest);
            }
            catch (Exception ex)
            {
                ErrorLog.AddEmailLogg(ex);
                return;
            }
        }

        /// <summary>
        /// Send Request Notification To hotel through Traveldesk
        /// </summary>
        /// <param name="objEMailDac"></param>
        /// <param name="senderEmailId"></param>
        public static void SendBookingRequestNotificationTo_Requester_Traveldesk(EmailNotificationModel objEMailDac, string senderEmailId)
        {
            string[] senderEmailIds = new string[] { senderEmailId };
            string MailApi = ConfigurationManager.AppSettings["MailApi"];

            string ccEmailId = ConfigurationManager.AppSettings["TravelDeskEmailId"].ToString();
            string[] ccEmailIds = new string[] { ccEmailId };
            
            var _jsonSerialiser = new JavaScriptSerializer();
            var _jsonRequest = _jsonSerialiser.Serialize
            (new Mail()
            {
                applicationKey = ConfigurationManager.AppSettings["MailApiKey"],
                cc = ccEmailIds,
                from = objEMailDac.SmtpUser,
                plainTextContent = string.Empty,
                htmlContent = objEMailDac.TemplateFilePath.Trim(),
                subject = objEMailDac.EmailSubjectName,
                to = senderEmailIds
            }
            );
            try
            {
                var s = SendEmailThroughAPI(MailApi, _jsonRequest);
            }
            catch (Exception ex)
            {
                ErrorLog.AddEmailLogg(ex);
                return;
            }
        }


        /// <summary>
        /// Send Email Through API Method
        /// </summary>
        /// <param name="MailApiUrl"></param>
        /// <param name="jRequest"></param>
        /// <returns></returns>
        public static bool SendEmailThroughAPI(string MailApiUrl, string jRequest)
        {
            bool result = false;
            try
            {
                using (WebClient _proxy = new WebClient())
                {
                    _proxy.Headers.Add("Content-Type", "application/json");
                    _proxy.Encoding = System.Text.Encoding.UTF8;
                    var _response = _proxy.UploadString(MailApiUrl, "POST", jRequest);
                    if (_response.Contains("200"))
                    {
                        result = true;
                    }
                }
            }
            catch(Exception ex)
            {
                ErrorLog.AddEmailLogg(ex);
                return result;
            }
            return result;
        }


        /// <summary>
        /// Send Email Notification to Requester with attachment
        /// </summary>
        /// <param name="objEMailDac"></param>
        public static void SendBookingRequestNotificationTo_Requester_Attachment(EmailNotificationModel objEMailDac, string senderEmailId, string attachment_path)
        {
            var client = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                //EnableSsl = true,//for local it should true
                EnableSsl = false,//For Production it shpuld false
                Host = objEMailDac.SmtpServer,
                Port = Convert.ToInt32(objEMailDac.SmtpPort),
            };
            var credentials = new NetworkCredential(objEMailDac.SmtpUser, objEMailDac.SmtpPass);
            client.UseDefaultCredentials = false;
            client.Credentials = credentials;
            var body = objEMailDac.TemplateFilePath;
            var msg = new MailMessage
            {
                From = new MailAddress(objEMailDac.SmtpUser),
                IsBodyHtml = true,
                Subject = objEMailDac.EmailSubjectName,
                Body = body,

            };
            msg.Attachments.Add(new Attachment(attachment_path));
            msg.To.Add(senderEmailId);
            try
            {
                client.Send(msg);
            }
            catch (Exception ex)
            {
                ErrorLog.AddEmailLogg(ex);
                return;
            }
        }
    

        /// <summary>
        /// Getter/Setter Property
        /// </summary>
        public class Mail
        {
            public string from { get; set; }
            public string subject { get; set; }
            public string plainTextContent { get; set; }
            public string htmlContent { get; set; }
            public string[] to { get; set; }
            public string[] bcc { get; set; }
            public string[] cc { get; set; }
            public string applicationKey { get; set; }
        }
    }
}