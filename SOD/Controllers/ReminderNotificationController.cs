using SOD.EmailNotification;
using SOD.Logging;
using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;
using SOD.Services.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using SOD.CommonWebMethod;

namespace SOD.Controllers
{
    public class ReminderNotificationController : Controller
    {
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Reminder Notification Controller
        /// </summary>
        public ReminderNotificationController()
        {
            _userRepository = new UserRepository(new SodEntities());
        }

        /// <summary>
        /// Hod Approval Reminder Auto Job Function
        /// </summary>
        /// <returns></returns>
        public string HodApprovalReminder()
        {
            var msg = string.Empty;
            object emailData = new object();
            string emailId = string.Empty;
            var ListToSend = new List<String>();
            try
            {
                var controller = DependencyResolver.Current.GetService<HotelOnlyController>();
                var res = _userRepository.GetReminderListTosendApproval(1);
                foreach (var lst in res)
                {
                    var hotelList = new List<TravelRequestHotelDetailModels>();
                    var dicList = new Dictionary<string, object>();
                    try
                    {
                        dicList = _userRepository.GetSodHotelInfo(Convert.ToInt64(lst.TravelRequestId));
                        var bookingInfo = dicList["bookingInfolist"] as List<TravelRequestMasterModels>;
                        var hotel_List = dicList["hotelinfolist"] as List<TravelRequestHotelDetailModels>;
                        var flightinfo = dicList["flightInfolist"] as List<FlightDetailModels>;
                        string hodEmailId = _userRepository.GetHODEmailIdByTravelReqID(lst.TravelRequestId.ToString());
                        var emailSubject2 = "SOD Hotel Booking Request Notification :" + System.DateTime.Now.ToString();
                        if (hodEmailId.Length > 0)
                        {
                        var emailTemplateName2 = "SodHotelBookingRequestFor_HodHotelApproval.html";
                        var emailCredentials2 = controller.EmailCredentialsHotelHod(emailSubject2, emailTemplateName2, hotel_List, bookingInfo, flightinfo, lst.TravelRequestId.ToString(), hodEmailId);
                      
                        var templateData = emailCredentials2.TemplateFilePath;
                        var appLink = string.Empty;
                        var approvaltype = string.Empty;
                        var emailId_hod = hodEmailId.Split(',')[0].ToString().Trim();
                        var emailid2 = emailId_hod.ToString();
                       
                            var skey = new StringBuilder();
                            skey.Append(bookingInfo[0].TravelRequestId.ToString() + ",");
                            skey.Append(bookingInfo[0].EmailId.Trim() + ",");
                            skey.Append(bookingInfo[0].SodBookingTypeId.ToString() + ",");
                            skey.Append(bookingInfo[0].BookingFor.Trim() + ",");
                            skey.Append(hodEmailId.Split(',')[0].ToString().Trim() + ",");
                            skey.Append(0);

                            var uri1 = ConfigurationManager.AppSettings["emailOnlyHotelApprovalPathHod"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=a");
                            var uri2 = ConfigurationManager.AppSettings["emailOnlyHotelApprovalPathHod"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=r");

                            approvaltype = "Please help to accord your Acceptance or Rejection.";
                            appLink = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a name='app' style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Acceptance</a></td> <td>&nbsp;</td> <td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a name='rej' style='color:#fff; text-decoration:none;' href='" + uri2 + "'>Rejection</a> </td></tr></table>";

                            templateData = templateData.Replace("[approvaltype]", approvaltype);
                            templateData = templateData.Replace("[appLink]", appLink);
                            templateData = templateData.Replace("[hodName]", hodEmailId.Split(',')[1]);
                            templateData = templateData.Replace("[RequesterName]", bookingInfo[0].Title + " " + bookingInfo[0].RequestedEmpName);
                            emailCredentials2.TemplateFilePath = templateData;
                            
                            EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials2, emailid2);

                            msg = "Mail Sent : Email has been sent successfully at " + emailid2 + "  Booking Req. ID : " + lst.TravelRequestId.ToString() + " at " + DateTime.Now.ToString();
                            ErrorLog.WriteLogg(msg, "HodApprovalReminderLogg.txt");
                        }
                    }
                    catch (Exception ex)
                    {
                        msg = "Email Error Hod Approval  : " + ex.InnerException.Message.ToString() + "\n Booking Req. ID :" + lst.TravelRequestId.ToString() + "  at " + DateTime.Now.ToString();
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
        /// Hod Financial Reminder Auto Job Function
        /// </summary>
        /// <returns></returns>
        public string HodFinancialReminder()
        {
            var msg = string.Empty;
            var controller = DependencyResolver.Current.GetService<trnsController>();
            try
            {
                var list = _userRepository.GetReminderListTosendApproval(2);               
                foreach (var lst in list)
                {
                    try
                    {
                     var s=   controller.ResendApproverRequest(lst.TravelRequestId.ToString(), lst.HotelRequestId.ToString(), "SOD");
                     if (s.Data!="")
                     {
                         msg = "Mail Sent : EMail has been sent Successfully . Booking Request Id - " + lst.TravelRequestId.ToString() + " at " + DateTime.Now.ToString();
                         ErrorLog.WriteLogg(msg, "FinancialApprovalReminderLogg.txt");
                     }
                    }
                    catch (Exception ex)
                    {
                        msg = "Email Error Fin Approval : " + ex.InnerException.Message.ToString() + " Booking Request Id " + lst.TravelRequestId.ToString() + " at " + DateTime.Now.ToString();
                        ErrorLog.WriteLogg(msg, "ReminderErroLog.txt");
                    }
                }
                msg = "Success";
            }
            catch (Exception ex)
            {
                msg = "Exception Fin Approval :" + ex.InnerException.Message.ToString() + " at " + DateTime.Now.ToString();
                ErrorLog.WriteLogg(msg, "ReminderErroLog.txt");
                throw ex;
            }
            return msg;
        }

        // GET: ReminderNotification
        public ActionResult Index()
        {
            return View();
        }
    }
}