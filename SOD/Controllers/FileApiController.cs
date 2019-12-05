
using SOD.Model;
using System.Web.Http.Cors;
using RouteAttribute = System.Web.Mvc.RouteAttribute;
using System.Collections.Generic;
using SOD.Services.Interface;
using SOD.Services.Repository;
using SOD.Services.EntityFramework;
using System;
using System.Configuration;
using System.Text;
using SOD.EmailNotification;
using System.Web.Http;
using System.Web.Mvc;
using SOD.CommonWebMethod;

namespace SOD.Controllers
{
    [EnableCors(origins:"*",headers:"*",methods:"*")]
    public class FileApiController : ApiController
    {
        private readonly IUserRepository _userRepository;
        public FileApiController()
        {
            _userRepository = new UserRepository(new SodEntities());
        }

        [System.Web.Http.HttpGet]
        [Route("api/FileApi/SendToApprover")]
        public IHttpActionResult Get()
        {
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
                    dicList = _userRepository.GetSodHotelInfo(Convert.ToInt64(lst));
                    var bookingInfo = dicList["bookingInfolist"] as List<TravelRequestMasterModels>;
                    var hotel_List = dicList["hotelinfolist"] as List<TravelRequestHotelDetailModels>;
                    var flightinfo = dicList["flightInfolist"] as List<FlightDetailModels>;
                    String hodEmailId = _userRepository.GetHODEmailIdByTravelReqID(lst.TravelRequestId.ToString());
                    var emailSubject2 = "SOD-SJSC Hotel Booking Request Notification :" + System.DateTime.Now.ToString();
                    var emailTemplateName2 = "SodHotelBookingRequestFor_HodHotelApproval.html";

                    var emailCredentials2 = controller.EmailCredentialsHotelHod(emailSubject2, emailTemplateName2, hotel_List, bookingInfo, flightinfo, lst.TravelRequestId.ToString(), hodEmailId);
                    // var emailData_hod = emailCredentials2;


                    var templateData = emailCredentials2.TemplateFilePath;
                    var appLink = string.Empty;
                    var approvaltype = string.Empty;
                    var emailId_hod = hodEmailId.Split(',')[0].ToString().Trim();
                    if (hodEmailId.Length > 0)
                    {
                        var skey = new StringBuilder();
                        skey.Append(bookingInfo[0].TravelRequestId.ToString() + ",");
                        skey.Append(bookingInfo[0].EmailId.Trim() + ",");
                        skey.Append(bookingInfo[0].SodBookingTypeId.ToString() + ",");
                        skey.Append(bookingInfo[0].BookingFor.Trim() + ",");
                        skey.Append(hodEmailId.ToString().Trim() + ",");
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

                        //var emaildata2 = emailData_hod as EmailNotificationModel;
                        var emailid2 = emailId_hod.ToString();
                        EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials2, emailid2);
                        //SendBookingRequestNotificationTo_Requester_Traveldesk
                    }
                }
                return Json("su");
            }
            catch (Exception ex)
            {
                return Json(ex.InnerException.Message.ToString());
                throw ex;
            }
            return Json("su");
        }

        //[System.Web.Http.HttpGet]
        //[Route("api/FileApi/GetListToFinancialApproval")]
        //public IHttpActionResult GetListToFinancialApproval()
        //{
        //    //var controller = DependencyResolver.Current.GetService<trnsController>();
        //    //try
        //    //{
        //    //    var list = _userRepository.GetListTosendTriggerFinancalApproval();
        //    //    foreach (var lst in list)
        //    //    {
        //    //        controller.ResendApproverRequest(lst.TravelRequestId.ToString(), lst.HotelRequestId.ToString(), "SOD");
        //    //    }
        //    //    return Json( "success");
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    throw ex;
        //    //}
        //    return Json("success");
        //}

       

        /// <summary>
        /// Send Email Notification to booking requester/user
        /// </summary>
        /// <returns></returns>
        private void sendEmailNotification(object emailData, string emailId, object emailData_Hod, string emailId_Hod)
        {
            try
            {
                if (emailData != null && emailId != null)
                {
                    var emaildata = emailData as EmailNotificationModel;
                    var emailid = emailId;
                    EmailNotifications.SendBookingRequestNotificationTo_Requester(emaildata, emailid);
                }

                if (emailData_Hod != null && emailId_Hod != null)
                {
                    var emaildataHod = emailData_Hod as EmailNotificationModel;
                    var emailidHod = emailId_Hod;
                    EmailNotifications.SendBookingRequestNotificationTo_HOD(emaildataHod, emailidHod);
                }
            }
            catch (Exception ex)
            {
                var json = ex.InnerException;
            }
        }

    }
}
