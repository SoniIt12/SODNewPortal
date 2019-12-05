using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Web;
using SOD.Model;
using System.Configuration;
using SOD.Logging;
using System.Web.Script.Serialization;
using SOD.Services.EntityFramework;
using SOD.Services.ADO;

namespace SOD.SmsNotification
{ 
    /// <summary>
    /// This Class will be used for Sending Sms Notifications
    /// Author :Vineeta
    /// Date :25, Jun, 2018
    /// Mod Date :########    
    /// </summary>
    public static class SmsNotifications 
    {

        /// <summary>
        /// SMS Credentials,PAss SMS Text and Mobile No.
        /// </summary>
        /// <param name="smsText"></param>
        /// <param name="mobileNumber"></param>
        /// <returns></returns>
        public static bool SendSmsViaApi(string smsText, string mobileNumber)
        {
                string smsApi = ConfigurationManager.AppSettings["smsApi"];
                string smsFeedid = ConfigurationManager.AppSettings["smsFeedid"];
                string smsUsername = ConfigurationManager.AppSettings["smsUsername"];
                string smsPassword = ConfigurationManager.AppSettings["smsPassword"];
                string smsSenderid = ConfigurationManager.AppSettings["smsSenderid"];
                string message = HttpUtility.UrlEncode(smsText);
                try
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    WebRequest request = WebRequest.Create(smsApi + "?feedid=" + smsFeedid +
                        "&username=" + smsUsername + "&password=" + smsPassword + 
                        "&short=1" + "&To="+ mobileNumber + "&Text=" + smsText + "");
                    request.Method = "GET";
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    return true;
                }
                catch (SystemException ex)
                {
                    AddDBLogging(ex, "SendSmsViaApi", "SmsNotifications.cs");
                    return false;
                }
        }

        /// <summary>
        /// Add DB Logging for SMS
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="methosName"></param>
        /// <param name="filePath"></param>
        public static void AddDBLogging(Exception ex, string methosName, string filePath)
        {
            SodEntities _context = new SodEntities();
            SmsLoggingModels obj = new SmsLoggingModels();
            try
            {
                obj.LogDate = System.DateTime.Now;
                obj.LogMessage = ex.Message.ToString();
                obj.LogData = ex.Data.ToString();
                obj.LogSource = ex.Source;
                obj.HelpLink = ex.HelpLink == null ? "" : ex.HelpLink;
                obj.HResult = ex.HResult == null ? "" : ex.HResult.ToString();
                obj.InnerException = ex.InnerException == null ? "" : ex.InnerException.Message;
                obj.MethodName = methosName;
                obj.FilePath = filePath;
                _context.SmsLoggingModel.Add(obj);
                _context.SaveChanges();
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Send SMS and Save data in DB
        /// </summary>
        public static void getSmsData()
        {
            SodEntities _context = new SodEntities();
            DateTime currentDate = DateTime.Now;
            //DateTime currentDate = new DateTime(2018, 07, 04, 11, 8, 0);

            var time1 = currentDate.AddMinutes(30).ToShortTimeString();
            var time = currentDate.AddMinutes(35).ToShortTimeString();
            
            try
            {
                var data = SodCommonServices.GetSMSData(currentDate.Date, time1, time).ToList();
                if (data != null && data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        //modify link for cancellation in sms
                        string rqstQueryString = ConfigurationManager.AppSettings["smsLink"].ToString() + "?trid=" + item.TravelRequestId.ToString()
                            + "," + item.HotelRequestId.ToString() + ",sms";
                        string msgContent = ConfigurationManager.AppSettings["smsMsg"].ToString();
                        msgContent = msgContent.Replace("[rqstQueryString]", rqstQueryString);
                    
                        //save sms sent status in database
                        var flightData = _context.FlightDetailModel.SingleOrDefault(s => s.Id == item.Id);
                        if (flightData != null)
                        {
                            if (!flightData.IsHotelSendSms)
                            {
                                bool status = SendSmsViaApi(msgContent, item.Phone);
                                if (status)
                                    flightData.IsHotelSendSms = true;
                                _context.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AddDBLogging(ex, "getSmsData", "SmsNotifications.cs");
            }
        }
    }
}
