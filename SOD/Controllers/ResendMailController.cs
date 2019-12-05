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
    public class ResendMailController : Controller, IExceptionFilter
    {
        /// Constructor Initialization
        /// </summary>
        private readonly ISodApproverRepositorty _sodApproverRepositorty;
        private readonly IUserRepository _userRepository;
        private readonly ISjSisConcernRepository _sJsisConcernRepository;

        public ResendMailController()
        {
            _sodApproverRepositorty = new SodApproverRepositorty(new SodEntities());
            _userRepository = new UserRepository(new SodEntities());
            _sJsisConcernRepository = new SjSisConcernRepository(new SodEntities());
        }
        // GET: ResendMail
        public JsonResult ResendMailToHod()
        {
            string hodEmailId = string.Empty;
            IList<EmployeeModel> empInfo;
            var controller = DependencyResolver.Current.GetService<EmployeeBookingDetailController>();
            var trId = Request.QueryString["trId"].ToString();
            //Get Booking Detail
            var dicList = GetDetailOfBookingHistoryByTrId(trId);
            var sodRequestsList = dicList["bookingInfo"] as List<TravelRequestMasterModels>;
            var sodflightList = dicList["flightInfo"] as List<FlightDetailModels>;
            var passengerList = dicList["passInfo"] as List<PassengerDetailModels>;
            var passengerMealsList = dicList["mealsInfo"] as List<PassengerMealAllocationModels>;
            var cabList = dicList["cabInfo"] as List<TravelRequestCabDetailModels>;
            var hotelList = dicList["hotelInfo"] as List<TravelRequestHotelDetailModels>;

            //This is for SJSC-Hotel Booking
            if (sodRequestsList[0].TravelRequestTypeId == 5 || sodRequestsList[0].TravelRequestCode.Contains("SOD SJSC"))
            {
                //string UserDetails=""//will come from SJSCUserInfor table//Nee to check from SJSC Controller Link Gnereation
                var hodEmaildata = _sJsisConcernRepository.GetHodDetails(Session["HodEmailId"].ToString());
                hodEmailId = hodEmaildata[0].HodEmail + "," + hodEmaildata[0].HoDTitle + " " +hodEmaildata[0].HoDName + "," + "000" + "," + hodEmaildata[0].HodMobileNo + "," + "00113581";
            }
            else
            {
                empInfo = _userRepository.GetEmployeeList(int.Parse(Session["EmpId"].ToString()));
                hodEmailId = _userRepository.GetHODEmailId(empInfo[0].EmployeeVertical);
            }

            if (hodEmailId.Length > 0)
            {
                //Mail Template
                var strHoldPNR = "";
                var BookingFor = "";
                var emailSubject = "";
                var emailTemplateName = "";
                var BookingType = (sodRequestsList[0].SodBookingTypeId == (short)2) ? "NON-SOD" : "SOD";

                EmailNotificationModel emailCredentials = new EmailNotificationModel();
                if (sodRequestsList[0].BookingFor == "Only Hotel")
                {
                    BookingFor = sodRequestsList[0].BookingFor;
                    emailSubject = "SOD Only-Hotel Booking Request Notification :" + System.DateTime.Now.ToString();
                    emailTemplateName = "SodHotelBookingRequestFor_HodHotelApproval.html";
                    emailCredentials = controller.EmailCredentialsHotelHod(emailSubject, emailTemplateName, hotelList, sodRequestsList, trId, hodEmailId.Split(',')[0]);
                }
                else
                {
                    if (sodRequestsList[0].TravelRequestCode.Contains("SOD SJSC"))
                    {
                        BookingFor = (sodRequestsList[0].BookingFor.Trim().ToLower() == "standby") ? "Standby" : "Confirm";
                        emailSubject = BookingType + " " + "SJSC" + " " + BookingFor + " " + "Booking Request Notification :" + System.DateTime.Now.ToString();
                    }
                    else
                    {
                        BookingFor = (sodRequestsList[0].BookingFor.Trim().ToLower() == "standby") ? "Standby" : "Confirm";
                        emailSubject = BookingType + " " + BookingFor + " " + "Booking Request Notification :" + System.DateTime.Now.ToString();

                    }
                    emailTemplateName = "SodBookingRequestNotificationTemplate_HOD.html";
                    emailCredentials = controller.EmailCredentials(emailSubject, emailTemplateName, sodRequestsList, sodflightList, passengerList, passengerMealsList, hotelList, trId, strHoldPNR);
                }
                TempData["emailData_hod"] = emailCredentials;
                TempData["emailId_hod"] = hodEmailId.Split(',')[0].ToString().Trim();

                var templateData = emailCredentials.TemplateFilePath;
                var appLink = string.Empty;
                var approvaltype = string.Empty;
                var uri1 = string.Empty;
                var uri2 = string.Empty;
                if (hodEmailId.Length > 0)
                {
                    var skey = new StringBuilder();
                    skey.Append(sodRequestsList[0].TravelRequestId.ToString() + ",");
                    skey.Append(sodRequestsList[0].EmailId.Trim() + ",");
                    skey.Append(sodRequestsList[0].SodBookingTypeId.ToString() + ",");
                    skey.Append(sodRequestsList[0].BookingFor.Trim() + ",");
                    skey.Append(hodEmailId.Split(',')[2] + ",");
                    skey.Append(sodRequestsList[0].IsMandatoryTravel.ToString());
                    if (sodRequestsList[0].BookingFor == "Only Hotel" || sodRequestsList[0].TravelRequestTypeId == 5)
                    {
                        uri1 = ConfigurationManager.AppSettings["emailOnlyHotelApprovalPathHod"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=a");
                        uri2 = ConfigurationManager.AppSettings["emailOnlyHotelApprovalPathHod"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=r");
                    }
                    else
                    {
                        uri1 = ConfigurationManager.AppSettings["emailApprovalPath"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=a");
                        uri2 = ConfigurationManager.AppSettings["emailApprovalPath"].Trim() + "?str=" + CipherURL.Encrypt(skey + "&type=r");
                    }

                    if (sodRequestsList[0].IsMandatoryTravel.Equals(1))
                    {
                        approvaltype = "Please help to accord your Acceptance or Rejection or Mandatory Travel – Approval.";
                        appLink = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a name='app' style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Acceptance</a></td> <td>&nbsp;</td> <td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a name='rej'  style='color:#fff; text-decoration:none;' href='" + uri2 + "'>Rejection</a> </td><td>&nbsp;</td> <td style='width:275x; height:25px; background-color:#01DF3A;text-align:center;border-radius:5px'><a name='mapp'  style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Mandatory Travel – Approve</a></td></tr></table>";
                    }
                    else
                    {
                        approvaltype = "Please help to accord your Acceptance or Rejection.";
                        appLink = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a name='app' style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Acceptance</a></td> <td>&nbsp;</td> <td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a name='rej' style='color:#fff; text-decoration:none;' href='" + uri2 + "'>Rejection</a> </td></tr></table>";
                    }

                    templateData = templateData.Replace("[approvaltype]", approvaltype);
                    templateData = templateData.Replace("[appLink]", appLink);
                    templateData = templateData.Replace("[hodName]", hodEmailId.Split(',')[1]);
                    templateData = templateData.Replace("[RequesterName]", sodRequestsList[0].RequestedEmpName);
                    emailCredentials.TemplateFilePath = templateData;
                }
                var emaildata = TempData["emailData_hod"] as EmailNotificationModel;
                var emailid = TempData["emailId_hod"].ToString();
                EmailNotification.EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials, emailid);
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
                return Json("Fail", JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Get Booking History details by TrId
        /// </summary>
        /// <param name="trId"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetDetailOfBookingHistoryByTrId(String trId)
        {
            var sodRequestsList = new List<TravelRequestMasterModels>();
            var sodflightList = new List<FlightDetailModels>();
            var passengerMealsList = new List<PassengerMealAllocationModels>();

            var dicList = new Dictionary<string, object>();
            dicList = _sodApproverRepositorty.GetSodBookingInfoForPNR(Convert.ToInt64(trId));
            sodRequestsList = dicList["bookingInfo"] as List<TravelRequestMasterModels>;
            sodflightList = dicList["flightInfo"] as List<FlightDetailModels>;
            passengerMealsList = dicList["mealsInfo"] as List<PassengerMealAllocationModels>;

            //Check Meal Option from Old allocation
            if (passengerMealsList.Count == 0)
            {
                //Add Passenger meal Option for SOD Flights
                foreach (var item in sodflightList)
                {
                    var passengerMealsListItem = new PassengerMealAllocationModels();
                    passengerMealsListItem.TravelRequestId = item.TravelRequestId;
                    passengerMealsListItem.Sector = item.OriginPlace + "-" + item.DestinationPlace;
                    passengerMealsListItem.MealType = sodRequestsList[0].Meals;
                    passengerMealsListItem.PassengerId = 0;
                    passengerMealsList.Add(passengerMealsListItem);
                }
            }

            var fcount = 0;
            foreach (var item in sodflightList)
            {
                fcount++;
                var sector = item.OriginPlace + "-" + item.DestinationPlace;
                foreach (var m in passengerMealsList)
                {
                    if (m.Sector == sector && fcount == m.PassengerId)
                    {
                        item.Meals = m.MealType;
                    }
                    if (m.Sector == sector && m.PassengerId == 0)
                    {
                        if (item.Meals != null)
                            item.Meals = item.Meals + "," + m.MealType;
                        else
                            item.Meals = m.MealType;
                    }
                }
            }
            return dicList;
        }


        /// <summary>
        /// Open View
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Exception Handling
        /// </summary>
        /// <param name="filterContext"></param>
        void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            Logging.ErrorLog.Instance.AddDBLogging(ex, filterContext.Controller.ControllerContext.ToString(), "Controllers/ResendMailController.cs");
        }
    }
}