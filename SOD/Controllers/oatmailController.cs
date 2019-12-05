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
    public class oatmailController : Controller, IExceptionFilter
    {

        private readonly IOatApproverRepository _oatApproverRepositorty;

        public oatmailController()
        {
            _oatApproverRepositorty = new OatApproverRepository(new SodEntities());
        }

        // GET: oatmail
        public ActionResult Index()
        {
            var strQuery = string.Empty;
            strQuery = Request.QueryString[0].ToString();

            if (Request.QueryString[1].Trim().Equals("a"))
                TempData["jsonmsg"] = ApproveOatBookingRequest(strQuery);
            else if (Request.QueryString[1].Trim().Equals("r"))
                TempData["jsonmsg"] = RejectOatBookingRequest(strQuery);

            return View("Index");
        }



        /// <summary>
        /// Approve Booking Request for HOD
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        public string ApproveOatBookingRequest(string travelReqId)
        {
            var jsonmsg = string.Empty;
            var approvalList = new OatTravelRequestApprovalModel()
            {
                TravelRequestId = Convert.ToInt32(travelReqId.Split(',')[0]),//Travel Request
                ApprovedByEmpId = Convert.ToInt32(travelReqId.Split(',')[1]),//Employee Id of approver
                ApprovalStatus = 1,
                IsMandatoryTravel =0,// Convert.ToInt32(travelReqId.Split(',')[5]), //Is Mandatory Travel
                ApprovalDate = System.DateTime.Now,
                Comment = "Approved from HOD",
                ApprovedByEmpIdCLevel1 = 0,
                ApprovedByEmpIdCLevel2 = 0,
                ApprovalStatusCLevel1 = 0,
                ApprovalStatusCLevel2 = 0,
                CLevelComment1 = "",
                CLevelComment2 = "",
                CLevelAppDate1 = DateTime.Parse("01/01/1900"),
                CLevelAppDate2 = DateTime.Parse("01/01/1900"),
                RevenueApprovedStatus = 0,
                RevenueApprovedDate = DateTime.Parse("01/01/1900")
            };

            //Check Duplicate PNR  
            //var chkpnr = Services.ADO.SodCommonServices.CheckDuplicatePNR(Convert.ToInt64(travelReqId.Split(',')[0]), 1);
            //if (!chkpnr.Equals("0"))
            //{
            //    return "Sorry : PNR has already generated for this booking request. Existing PNR No. :" + chkpnr;
            //}

            var s = _oatApproverRepositorty.ApproveOatBookingRequest(approvalList);

            if (s >= 1)
            {
                //Close Booking Request
                var trRequestMaster = new OALTravelRequestMasterModel()
                {
                    TravelRequestId = Convert.ToInt64(travelReqId.Split(',')[0]),
                    BookingStatus = "Approved",
                    StatusDate = System.DateTime.Now
                    
                };
                var c = _oatApproverRepositorty.CloseOatBookingRequest_HOD(trRequestMaster);

                //Get Booking Request Data for Email Notification
                var sodRequestsList = new List<OALTravelRequestMasterModel>();
                var sodflightList = new List<OALModels>();
                var sodPassList = new List<OALPassengerModel>();
                var sodhotelList = new List<OALHotelModel>();
                var dicList = new Dictionary<string, object>();
                
                dicList = _oatApproverRepositorty.GetOatBookingInfoForPNR(Convert.ToInt32(travelReqId.Split(',')[0]));
                sodRequestsList = dicList["bookingInfo"] as List<OALTravelRequestMasterModel>;
                sodflightList = dicList["flightInfo"] as List<OALModels>;
                sodPassList = dicList["passInfo"] as List<OALPassengerModel>;
                sodhotelList = dicList["hotelInfo"] as List<OALHotelModel>;
                //sodPassMealsList = dicList["mealsInfo"] as List<PassengerMealAllocationModels>;

                //Send Approval Email Notification
                string emailSubject = "OAT Booking Request Approval Notification from HOD :" + System.DateTime.Now.ToString("dd-MMM-yyyy-hh:mm:ss");
                var emailTemplateName = "OatBookingRequest_Approval_User.html";

                //Send Email Notification Credentials
                var emailCredentials = EmailCredentials(emailSubject, emailTemplateName, sodRequestsList, sodflightList, sodPassList, sodhotelList, travelReqId);
                emailCredentials.TemplateFilePath = emailCredentials.TemplateFilePath.Replace("[Team]", "HOD");
                EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials, sodRequestsList[0].EmailId.Trim());

                jsonmsg = "Approval Alert for Request No. OAT-" + travelReqId.Split(',')[0] + ": \n Approval process has been completed successfully.";

            }
            return (s >= 1 ? jsonmsg : string.Empty);
        }



        /// <summary>
        /// Reject Booking Request
        /// </summary>
        /// <param name="travelReqId"></param>
        /// <returns></returns>
        public string RejectOatBookingRequest(string travelReqId)
        {
            var jsonmsg = string.Empty;
            

            //Initialize list for updating revenue status
            var approvalList = new OatTravelRequestApprovalModel()
            {
                TravelRequestId = Convert.ToInt32(travelReqId.Split(',')[0]),
                ApprovedByEmpId = Convert.ToInt32(travelReqId.Split(',')[1]),
                ApprovalStatus = 2,
                ApprovalDate = System.DateTime.Now,
                Comment = "Rejected from HOD",
                ApprovedByEmpIdCLevel1 = 0,
                ApprovedByEmpIdCLevel2 = 0,
                ApprovalStatusCLevel1 = 0,
                ApprovalStatusCLevel2 = 0,
                CLevelComment1 = "",
                CLevelComment2 = "",
                CLevelAppDate1 = DateTime.Parse("01/01/1900"),
                CLevelAppDate2 = DateTime.Parse("01/01/1900"),
                RevenueApprovedStatus = 0,
                RevenueApprovedDate = DateTime.Parse("01/01/1900")
            };
            var s = _oatApproverRepositorty.RejectOatBookingRequest(approvalList);
            if (s >= 1)
            {
                //Close Booking Request
                var trRequestMaster = new OALTravelRequestMasterModel()
                {
                    TravelRequestId = Convert.ToInt32(travelReqId.Split(',')[0]),
                    BookingStatus = "Rejected by HOD",
                    StatusDate = System.DateTime.Now
                };
                var c = _oatApproverRepositorty.CloseOatBookingRequest_HOD(trRequestMaster);

                //Get Booking Request Data for Email Notification
                var sodRequestsList = new List<OALTravelRequestMasterModel>();
                var sodflightList = new List<OALModels>();
                var sodPassList = new List<OALPassengerModel>();
                var sodhotelList = new List<OALHotelModel>();
                var dicList = new Dictionary<string, object>();

                dicList = _oatApproverRepositorty.GetOatBookingInfoForPNR(Convert.ToInt32(travelReqId.Split(',')[0]));
                sodRequestsList = dicList["bookingInfo"] as List<OALTravelRequestMasterModel>;
                sodflightList = dicList["flightInfo"] as List<OALModels>;
                sodPassList = dicList["passInfo"] as List<OALPassengerModel>;
                sodhotelList = dicList["hotelInfo"] as List<OALHotelModel>;
                //sodPassMealsList = dicList["mealsInfo"] as List<PassengerMealAllocationModels>;
                //string bookingType = sodRequestsList[0].SodBookingTypeId == 1 ? "SOD" : "NON-SOD";

                //Send Rejection Email Notification
                string emailSubject = "OAT Booking Request Rejection Notification from HOD :" + System.DateTime.Now.ToString("dd-MMM-yyyy-hh:mm:ss");
                var emailTemplateName = "OatBookingRequest_Rejection_User.html";

                //Send Email Notification Credentials
                var emailCredentials = EmailCredentials(emailSubject, emailTemplateName, sodRequestsList, sodflightList, sodPassList, sodhotelList, travelReqId);
                emailCredentials.TemplateFilePath = emailCredentials.TemplateFilePath.Replace("[Team]", "HOD");
                EmailNotifications.SendBookingRequestNotificationTo_Requester(emailCredentials, sodRequestsList[0].EmailId.Trim());

                jsonmsg = "Rejection Alert for Request No. OAT-" + travelReqId.Split(',')[0] + ": \n Rejection process has been completed successfully.";
            }
            return (s >= 1 ? jsonmsg : string.Empty);
        }



        /// <summary>
        /// Initialized Email Credentials to Property
        /// </summary>
        /// <returns></returns>
        public EmailNotificationModel EmailCredentials(string subjectName, string emailTemplateName, List<OALTravelRequestMasterModel> sodRequestsList, List<OALModels> sodflightList, List<OALPassengerModel> sodPassList, List<OALHotelModel> sodhotelList, string reqNo)
        {
            var objEmailServices = new EmailNotificationModel
            {
                SmtpUser = ConfigurationManager.AppSettings["smtpUser"].Trim(),
                SmtpPass = ConfigurationManager.AppSettings["smtpPass"].Trim(),
                SmtpServer = ConfigurationManager.AppSettings["smtpServer"].Trim(),
                SmtpPort = ConfigurationManager.AppSettings["smtpPort"].Trim(),
                TemplateFilePath = ReadFile(emailTemplateName, sodRequestsList, sodflightList, sodPassList, sodhotelList, reqNo),
                EmailSubjectName = subjectName
            };
            return objEmailServices;
        }



        /// <summary>
        /// Read Template File from Location
        /// </summary>
        /// <returns></returns>
        private string ReadFile(string emailTemplateName, List<OALTravelRequestMasterModel> sodRequestsList, List<OALModels> sodflightList, List<OALPassengerModel> sodPassList, List<OALHotelModel> sodhotelList, string reqNo)
        {
            var strContent = new StringBuilder();
            string line;
            var file = new System.IO.StreamReader(
                       Server.MapPath("~/Template/OAT/" + emailTemplateName));
            while ((line = file.ReadLine()) != null)
            {
                strContent.Append(line);
                //Replace code here..
            }
            file.Close();

            string title = "";
            if (sodRequestsList[0].Gender == "M") { title = "Mr."; } else { title = "Ms."; }

            strContent = strContent.Replace("[RequesterName]", title + " " + sodRequestsList[0].RequestedEmpName);
            var mltr = string.Empty;
            var c = 0;
            //var bookingFor = sodRequestsList[0].;
            var meal = sodRequestsList[0].Meals;
            var deg = sodRequestsList[0].RequestedEmpDesignation;
            var dep = sodRequestsList[0].RequestedEmpDept;
            //var mtr = sodRequestsList[0].IsMandatoryTravel.Equals(1) ? "Yes" : "No";
            var btype = "OAT";
            var tr = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Reason for Travel</td><td>Booking Type</td><td>No.of Passengers</td>";

            tr = tr + "</tr>";

            //Adding Booing Info
            tr = tr + "<tr style='font-family:Arial; font-size:12px;'><td>" + sodRequestsList[0].ReasonForTravel + "</td><td>" + btype + "</td><td>" + sodRequestsList[0].Passengers + "</td>";

            tr = tr + "</tr>";
            strContent = strContent.Replace("[tr]", tr);

            //Begin of Passengers List----------------------------------------------------------------------------------------------------------------
            var i = 0;
            var trp = string.Empty;


            strContent = strContent.Replace("[pinfo]", "Passenger(s) Information");
            strContent = strContent.Replace("[finfo]", "Flight Information");
            strContent = strContent.Replace("[binfo]", "Booking Information (Request ID : OAT-" + reqNo.Split(',')[0] + ")");
            trp = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>S. No.</td><td>First Name</td><td>Last Name</td><td>Gender</td>";

            trp = trp + "</tr>";

            foreach (var p in sodPassList)
            {
                i++;
                trp = trp + "<tr style='font-family:Arial; font-size:12px;'><td style='border-top:1px solid #c2c2c2'>" + i + "</td><td style='border-top:1px solid #c2c2c2'>" + p.FirstName + "</td><td style='border-top:1px solid #c2c2c2'> " + p.LastName + "</td><td style='border-top:1px solid #c2c2c2'>" + p.Gender + "</td></tr>";

            }
            strContent = strContent.Replace("[trp]", trp);
            //-EOD Passenger List--------------------------------------------------------------------------------------------------------------------

            //Begin of Flight List--------------------------------------------------------------------------------------------------------------------
            var trf = "";

            trf = "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>Sector</td><td>Travel Date</td><td>Flight Info</td><td>Flight Number</td><td>Departure Time</td></tr>";
            foreach (var f in sodflightList)
            {
                var sector = f.OriginPlace + "-" + f.DestinationPlace;
                trf = trf + "<tr style='font-family:Arial; font-size:12px;'><td style='border-top:1px solid #c2c2c2'>" + sector + "</td><td style='border-top:1px solid #c2c2c2'>" + f.DepartureDate.ToString("dd-MMM-yyyy") + "</td><td style='border-top:1px solid #c2c2c2'>" + f.DepFlightInfo + "</td><td style='border-top:1px solid #c2c2c2'>" + f.DepFlightNumber + "</td><td style='border-top:1px solid #c2c2c2'>" + f.DepartureTime + "</td></tr>";
            }

            //-EOD Flight List--------------------------------------------------------------------------------------------------------------------

            if (sodRequestsList[0].IsHotelRequired == true && sodhotelList[0].usercancellation != "Cancelled by User")
            {
                var trh = "";

                trh = "<table cellpadding='0' cellspacing='0' style='width:100%; font-size:12px;border:solid 1px #c2c2c2;line-height:30px;'>";
                trh = trh + "<tr style='background-color:#b33;color:#fff;font-family:Arial; font-size:12px;'><td>City</td><td>Check-In Date</td><td>Check-Out Date</td><td>Entitlement</td></tr>";
                foreach (var h in sodhotelList)
                {
                    //var sector = f.OriginPlace + "-" + f.DestinationPlace;
                    trh = trh + "<tr style='font-family:Arial; font-size:12px;'><td style='border-top:1px solid #c2c2c2'>" + h.City + "</td><td style='border-top:1px solid #c2c2c2'>" + h.CheckInDate.ToString("dd-MMM-yyyy") + "</td><td style='border-top:1px solid #c2c2c2'>" + h.CheckOutDate.ToString("dd-MMM-yyyy") + "</td><td style='border-top:1px solid #c2c2c2'>" + h.Entitlement + "</td>";
                    trh = trh + "</tr>";
                }
                trh = trh + "</table>";

                strContent = strContent.Replace("[trh]", trh);
                strContent = strContent.Replace("[hinfo]", "<table cellpadding='0' cellspacing='0' style='width:100%; border:0px;'><tr><td style='font-size: 16px;font-family:Arial;margin-top: 15px;margin-bottom:20px; border-right:solid 0px transparent;'><p>Hotel Information</p> </td></tr></table>");
            }
            else
            {
                strContent = strContent.Replace("[hinfo]", String.Empty);
                strContent = strContent.Replace("[trh]", String.Empty);
            }

            strContent = strContent.Replace("[trf]", trf);
            //strContent = strContent.Replace("[trml]", mltr == "" ? "" : mltr);
            return strContent.ToString();
        }

        /// <summary>
        /// Exception Handling
        /// </summary>
        /// <param name="filterContext"></param>
        void OnException(ExceptionContext filterContext)
        {
            Exception ex = filterContext.Exception;
            Logging.ErrorLog.Instance.AddDBLogging(ex, filterContext.Controller.ControllerContext.ToString(), "Controllers/oatmailController.cs");
        }


    }
}