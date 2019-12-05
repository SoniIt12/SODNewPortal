using SOD.CommonWebMethod;
using SOD.EmailNotification;
using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;
using SOD.Services.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SOD.Controllers
{
    public class mailITHController : Controller, IExceptionFilter
    {

        private readonly IHotelApproverRepository _hotelApproverRepository;
        private readonly IOALRepository _oalRepository;
        
        public mailITHController()
        {
            _hotelApproverRepository = new HotelApproverRepository(new SodEntities());
            _oalRepository = new OALRepository(new SodEntities());
        }

        // GET: ithmaildata
        [HttpPost]
        public ActionResult Index(List<ITHResponseDetailModels> ithList)
        { 
            //save data in table
            foreach (var i in ithList)
            {
                i.ResponseDate = DateTime.Now;
            }            
            var s= _oalRepository.SaveITHResponseData(ithList);

            return View("Index");
            
        }

        public ActionResult IndexITHResponse()
        {
            return View("IndexITHResponse");
        }

        //Success message view
        public ActionResult Response()
        {
            return View("Response");
        }

        /// <summary>
        /// get details of flight and passenger for ith
        /// </summary>
        /// <param name="trid"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetDetailsOATList_forITH(string trid)
        {
            var dicList = new Dictionary<string, object>();
            dicList = _oalRepository.GetOATDetailsData(trid);
            return Json(dicList, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// get details for ith status
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult viewITHStatusList()
        {
            return Json(_oalRepository.viewITHStatusList(Request.QueryString["TrId"].Trim()), JsonRequestBehavior.AllowGet);
        }

        // GET: Hod approval data
        public ActionResult ApproveRejectHod()
        {
            return View("ApproveRejectHod");
        }

        /// <summary>
        /// send mail to hod for financial approval
        /// </summary>
        /// <param name="hlist"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult sendmailtoHOD(List<ITHResponseDetailModels> ithlist)
        {
            var dicList = new Dictionary<string, object>();
            var plist = new List<OALPassengerModel>();
            var mlist = new List<OALTravelRequestMasterModel>();

            dicList = _oalRepository.GetOATDetailsData("OAT-" + ithlist[0].TravelRequestId.ToString());
            plist = dicList["pasgList"] as List<OALPassengerModel>;
            mlist = dicList["masterList"] as List<OALTravelRequestMasterModel>;

            string hodEmailId = SOD.Services.ADO.SodCommonServices.GetHodEmailDetails(mlist[0].RequestedEmpId.ToString().Trim(), 2);
            var status= _oalRepository.saveHODStatus(ithlist[0].TravelRequestId, "Pending");

            if (status > 0)
            {
                //send mail to hod- pasg details, ith details with radio button
                var emailSubject = "OAT Request Notification from Travel Desk:" + System.DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt");
                var emailTemplateName = "OatRequestNotificationTemplateHodApproval_ITH.html";
                var emailCredentials = EmailCredentialsHODApproval(emailSubject, emailTemplateName, ithlist, plist);

                var skey = new StringBuilder();
                skey.Append(mlist[0].TravelRequestId.ToString() + ",");
                skey.Append(hodEmailId);

                var uri1 = "";
                var uri2 = "";

                uri1 = ConfigurationManager.AppSettings["emailApprovalPathHod_ITH"].Trim() + "?str=" + skey + "&type=a&trid=" + mlist[0].TravelRequestId;
                //uri2 = ConfigurationManager.AppSettings["emailRejectionPathHod_ITH"].Trim() + "?str=" + skey + "&type=r&trid=" + mlist[0].TravelRequestId;

                var appLink = string.Empty;
                appLink = "<table><tr style='font-family:Arial;'><td style='width:110px; height:25px; background-color:#04B431;text-align:center;border-radius:5px'><a style='color:#fff; text-decoration:none;' href='" + uri1 + "'>Select Option</a></td></tr></table>";
                //appLink = appLink + "<td style='width:110px; height:25px; background-color:#b33;text-align:center;border-radius:5px'><a style='color:#fff; text-decoration:none;' href='" + uri2 + "'>Rejection</a> </td></tr></table>";

                var templateData = emailCredentials.TemplateFilePath;
                var hodName = hodEmailId.Split(',')[1];

                templateData = templateData.Replace("[appLink]", appLink);
                templateData = templateData.Replace("[hodname]", hodName);

                emailCredentials.TemplateFilePath = templateData;

                TempData["emailData_Hod"] = emailCredentials;
                TempData["emailId_Hod"] = hodEmailId.Split(',')[0];
                EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials, hodEmailId.Split(',')[0].ToString());

                TempData["msgResponse"] = "OAT Response Notification : Flight details have been sent successfully to Travel desk.";
            }
            return Json("sent", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// email credentials for hod financial approval
        /// </summary>
        /// <param name="subjectName"></param>
        /// <param name="emailTemplateName"></param>
        /// <param name="ithlist"></param>
        /// <param name="plist"></param>
        /// <returns></returns>
        public EmailNotificationModel EmailCredentialsHODApproval(string subjectName, string emailTemplateName, List<ITHResponseDetailModels> ithlist, List<OALPassengerModel> plist)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtptdUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtptdPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtptdServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtptdPort"].Trim(),
                TemplateFilePath = ReadFileHODApproval(emailTemplateName, ithlist, plist),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }

        /// <summary>
        /// read file hod approval
        /// </summary>
        /// <param name="emailTemplateName"></param>
        /// <param name="hlist"></param>
        /// <param name="flist"></param>
        /// <param name="plist"></param>
        /// <returns></returns>
        private string ReadFileHODApproval(string emailTemplateName, List<ITHResponseDetailModels> ithlist, List<OALPassengerModel> plist)
        {
            var strContent = new StringBuilder();
            string line;
            var file = new System.IO.StreamReader(
                       Server.MapPath("~/Template/OAT/" + emailTemplateName));
            while ((line = file.ReadLine()) != null)
            {
                strContent.Append(line);
            }
            file.Close();
            strContent = strContent.Replace("[pinfo]", "Passenger Details ");
            strContent = strContent.Replace("[finfo]", "Flight Details provided by ITH");

            var tr = "";
            tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>First Name</td><td>Last Name</td><td>Gender</td></tr>";

            foreach (var item in plist)
            {
                tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td>" + item.FirstName +
                            "</td><td>" + item.LastName + "</td><td>" + item.Gender + "</td></tr>";
            }
            strContent = strContent.Replace("[tr]", tr);

            var trf = "";
            trf = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Source</td><td>Destination</td><td>Travel date</td><td>Flight No</td><td>Flight Name</td><td>Departure Time</td><td>Arrival Time</td><td>Amount</td><td>Flight type</td></tr>";

            var name = "";
            for (var i = 0; i < ithlist.Count; i++)
            {
                var segment = "";
                if ((i) % 3 == 0)
                {
                    name = "radio" + i;
                }
                if ((i + 1) % 3 == 0)
                {
                    segment = "<tr><td colspan='9'></td></tr>";
                }

                trf = trf + "<tr style='font-family:Arial; font-size:12px;'><td>" + ithlist[i].OriginPlace +
                    "</td><td>" + ithlist[i].DestinationPlace + "</td><td>" + ithlist[i].TravelDate.ToString("dd-MMM-yyyy") +
                    "</td><td>" + ithlist[i].FlightNo + "</td><td>" + ithlist[i].FlightName + "</td><td>" +
                    ithlist[i].DepartureTime + "</td><td>" + ithlist[i].ArrivalTime + "</td><td>" + ithlist[i].Amount +
                    "</td><td>" + ithlist[i].FlightType + "</td>" +
                    "</tr>" + segment;
            }
            strContent = strContent.Replace("[trf]", trf);

            return strContent.ToString();
        }


        /// <summary>
        /// save hod response
        /// </summary>
        /// <param name="IdList"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult saveHODResponseData(string IdList)
        {
            var s= _oalRepository.saveHODResponseData(IdList, 1);
            if (s > 0)
            {
                return Json("saved", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
            
        }


        /// <summary>
        /// Exception Handling
        /// </summary>
        /// <param name="filterContext"></param>
        void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            Logging.ErrorLog.Instance.AddDBLogging(ex, filterContext.Controller.ControllerContext.ToString(), "Controllers/mailHotelController.cs");
        }


    }
}