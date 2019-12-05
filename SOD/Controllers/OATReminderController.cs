using SOD.Logging;
using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;
using SOD.Services.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using RouteAttribute = System.Web.Http.RouteAttribute;

namespace SOD.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class OATReminderController : ApiController
    {
        private readonly IOATrepository _oaTRepository;
        private readonly IBulkUploadRepository _bulkUploadRepository;

        public OATReminderController()
        {
            _oaTRepository = new OATrepository(new SodEntities());
            _bulkUploadRepository = new BulkUploadRepository(new SodEntities());
        }

        [System.Web.Http.HttpGet]
        [Route("api/OATReminder/ReminderTriggerToUserForCancellation")]
        public String Get()
        {
            string msg = "";
            try
            {
                
                var controller = DependencyResolver.Current.GetService<HotelOnlyController>();
                var AllDetail = _oaTRepository.GetListOfAllBookedOATFlightDetail();
                foreach (var lst in AllDetail)
                {
                    //var dicList = new Dictionary<string, object>();
                    try
                    {
                        var skey = new StringBuilder();
                        var BookingDetail = lst.PassengerID + "," + lst.OriginPlace + "," + lst.DestinationPlace + "," + "Via_User" + "," + lst.OATRequestId;
                        skey.Append(lst.OATRequestId + ",");
                        skey.Append(0 + ",");
                        skey.Append(BookingDetail + ",");
                        var uri1 = ConfigurationManager.AppSettings["IthResponseFormPath"].Trim() + "?str=" + skey + "&type=CA";                        
                        SendSMSToCancel(lst, lst.EmpName, uri1.ToString());
                        //SendSMSforApproval(string (lst.OATRequestId).ToString(), string hoddetails, string requesterName, string approvalURI, string rejectionURI, string selectiveURI)
                        msg = "Mail Sent : Email has been sent successfully at " + "" + "  Booking Req. ID : " + "" + " at " + DateTime.Now.ToString();
                        ErrorLog.WriteLogg(msg, "HodApprovalReminderLogg.txt");

                    }
                    catch (Exception ex)
                    {
                        msg = "Email Error Hod Approval  : " + ex.InnerException.Message.ToString() + "\n Booking Req. ID :" + "" + "  at " + DateTime.Now.ToString();
                        ErrorLog.WriteLogg(msg, "ReminderErroLog.txt");
                    }
                }
                msg = "Success";
            }
            catch (Exception ex)
            {
                msg = "Exception Hod Approval : " + ex.InnerException.Message.ToString() + " at " + DateTime.Now.ToString();
                ErrorLog.WriteLogg(msg, "ReminderErroLog.txt");
            }
            return msg;
        }


        [System.Web.Http.HttpGet]
        [Route("api/OATReminder/ReminderTriggerToFinancialApprover")]
        public String GetReminder()
        {
            string msg = "";
            try
            {                
                var controller = DependencyResolver.Current.GetService<HotelOnlyController>();
                var GetAllList = _oaTRepository.GetListToSendFinancialApprover();
                foreach (var lst in GetAllList)
                {
                    //var dicList = new Dictionary<string, object>();
                    try
                    {                       
                        var skey = new StringBuilder();
                        skey.Append(lst.OATId.ToString() + ",");
                        skey.Append(0.ToString() + ",");
                        var uri1 = ConfigurationManager.AppSettings["IthResponseFormPath"].Trim() + "?str=" + skey + "&type=" + "FA";
                        var uri2 = ConfigurationManager.AppSettings["IthResponseFormPath"].Trim() + "?str=" + skey + "&type=" + "FR";
                        //SendSMSToCancel(lst, lst.EmpName, uri1.ToString());
                        SendSMSforFinApproval( lst, uri1, uri2);
                        msg = "Mail Sent : Email has been sent successfully at " + "" + "  Booking Req. ID : " + "" + " at " + DateTime.Now.ToString();
                        ErrorLog.WriteLogg(msg, "HodApprovalReminderLogg.txt");
                    }
                    catch (Exception ex)
                    {
                        msg = "Email Error Hod Approval  : " + ex.InnerException.Message.ToString() + "\n Booking Req. ID :" + "" + "  at " + DateTime.Now.ToString();
                        ErrorLog.WriteLogg(msg, "ReminderErroLog.txt");
                    }
                }
                msg = "Success";
            }
            catch (Exception ex)
            {
                msg = "Exception Hod Approval : " + ex.InnerException.Message.ToString() + " at " + DateTime.Now.ToString();
                ErrorLog.WriteLogg(msg, "ReminderErroLog.txt");
            }
            return msg;
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
        public void SendSMSToCancel(OATUploadItenaryModal OatRepository, string requesterName, string rqstQueryString)
        {
            var smsText = ConfigurationManager.AppSettings["sms_OATNotTravel"].ToString();
            smsText = smsText.Replace("@Passenger", OatRepository.EmpName);           
            smsText = smsText.Replace("[rqstQueryString]", rqstQueryString);            
            smsText = System.Uri.EscapeDataString(smsText);
            smsText = smsText.Replace("25", "");
            //Send SMS
            var smsLogModel = new SodApproverSMSLogModels();
            smsLogModel.TrRequestId = OatRepository.OATRequestId;
            smsLogModel.EmpCode = "";
            smsLogModel.EmpName = OatRepository.EmpName;
            smsLogModel.EmailID = OatRepository.EmpEmailId;
            smsLogModel.MobileNo = OatRepository.EmpPhoneNo;
            smsLogModel.Source = "OATCancel";
            smsLogModel.SMSText = smsText;
            smsLogModel.DeliveryDate = DateTime.Now;
            var status = SmsNotification.SmsNotifications.SendSmsViaApi(smsText, "8210172554");
            smsLogModel.IsDelivered = status.Equals(true) ? true : false;
            _bulkUploadRepository.SaveApproverSMSLog(smsLogModel);
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
        public void SendSMSforFinApproval(OATFinancialApprovalDetail_RoisteringModal lstDetail, string approvalURI, string rejectionURI)
        {
            var smsText = ConfigurationManager.AppSettings["sms_OATFinApproval_Roistering"].ToString().Replace("@Hodname", lstDetail.ApproverEmpName);
            smsText = smsText.Replace("@PaxName", lstDetail.PassengerName);
            smsText = smsText.Replace("@Sector", lstDetail.Sector);
            smsText = smsText.Replace("@Price", lstDetail.ApprovedAmount.ToString());
            smsText = smsText.Replace("@ReqId", lstDetail.OATId.ToString());
            
            smsText = smsText.Replace("@TravelDateDate", lstDetail.departureDate.ToString());
            
            smsText = smsText.Replace("[AppQueryString]", approvalURI);
            smsText = smsText.Replace("[RejQueryString]", rejectionURI);
            smsText = System.Uri.EscapeDataString(smsText);
            smsText = smsText.Replace("25", "");
            //Send SMS
            var smsLogModel = new SodApproverSMSLogModels();
            smsLogModel.TrRequestId = Convert.ToInt64(lstDetail.OATId);
            smsLogModel.EmpCode = lstDetail.ApproverEmpCode;
            smsLogModel.EmpName = lstDetail.ApproverEmpName;
            smsLogModel.EmailID = lstDetail.ApproverEmailID;
            smsLogModel.MobileNo =lstDetail.ApproverPhoneNo;
            smsLogModel.Source = "OATFin";
            smsLogModel.SMSText =  smsText;
            smsLogModel.DeliveryDate = DateTime.Now;
            var status = SmsNotification.SmsNotifications.SendSmsViaApi(smsText, "8210172554");
            smsLogModel.IsDelivered = status.Equals(true) ? true : false;
            _bulkUploadRepository.SaveApproverSMSLog(smsLogModel);
        }
    }
}
