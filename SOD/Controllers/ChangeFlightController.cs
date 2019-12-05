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
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SOD.Controllers
{
    public class ChangeFlightController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ISodApproverRepositorty _sodApproverRepositorty;

        public ChangeFlightController()
        {
            _sodApproverRepositorty = new SodApproverRepositorty(new SodEntities());
            _userRepository = new UserRepository(new SodEntities());
        }
        [HttpGet]
        public ActionResult ChangeFlight()
        {
            var trId = Request.QueryString["trId"].ToString();
            var sodRequestsList = new List<TravelRequestMasterModels>();
            var sodflightList = new List<FlightDetailModels>();
            var dicList = new Dictionary<string, object>();
            dicList = _sodApproverRepositorty.GetSodBookingInfoForPNR(Convert.ToInt64(trId));
            sodRequestsList = dicList["bookingInfo"] as List<TravelRequestMasterModels>;
            sodflightList = dicList["flightInfo"] as List<FlightDetailModels>;
            string TravelDate = sodflightList[0].TravelDate.ToString("dd/MM/yyyy");
            sodflightList[0].TravelDates = TravelDate;
            if (sodRequestsList[0].TravelRequestTypeId != 1)
            {
                string ReturnDate = sodflightList[1].TravelDate.ToString("dd/MM/yyyy");
                sodflightList[0].ReturnDates = ReturnDate;
                TempData["returnflight"] = sodflightList[1].FlightNo;
                ViewBag.Origin = sodflightList[0].OriginPlace;
                ViewBag.Destination = sodflightList[1].OriginPlace;
                ViewBag.TravelDate = sodflightList[0].TravelDates;
                ViewBag.ReturnDate = ReturnDate;
            }
            TempData["sodRequestsList"] = sodRequestsList;
            TempData["sodflightList"] = sodflightList;
            TempData["FlightNo"] = sodflightList[0].FlightNo;
            if (sodRequestsList[0].TravelRequestCode.Split('-')[0] == "SOD")
            {
                ViewBag.Sodtype = "SOD";
            }
            else if (sodRequestsList[0].TravelRequestCode.Split('-')[0] == "SOD SJSC")
            { ViewBag.Sodtype = "SOD SJSC"; }
            else
                ViewBag.Sodtype = "NON-SOD";
            ViewBag.TraveltypeID = sodRequestsList[0].TravelRequestTypeId;
            return View();
        }

        [HttpPost]
        public JsonResult SodRequestSubmit(List<TravelRequestModels> sodRequestsList)
        {
            TempData["SodbookingRequest"] = sodRequestsList;
            TempData["TravelRequestTypeId"] = sodRequestsList[0].TravelRequestTypeId;
            TempData["Passengers"] = sodRequestsList[0].Passengers;
            TempData["SodBookingType"] = sodRequestsList[0].SodBookingType;
            TempData["BookingFor"] = sodRequestsList[0].BookingFor;
            //spliting the destination place and getting the full destination name
            TempData["DestinationName"] = sodRequestsList[0].DestinationPlace.Split('-')[0];
            TempData["RequestListCount"] = sodRequestsList.Count;

            if (sodRequestsList[0].ReturnDate != null)
            {
                TempData["ReturnDateRoundTrip"] = sodRequestsList[0].ReturnDate.ToString();
            }
            var destList = new List<String>();
            //spliting the destination name and keeping the destination code in the sodRequestsList object.
            for (int i = 0; i < sodRequestsList.Count; i++)
            {
                destList.Add(sodRequestsList[i].DestinationPlace.Split('-')[0]);
            }
            TempData["DestinationList"] = destList;

            var s = "";
            switch (sodRequestsList[0].TravelRequestTypeId)
            {
                case 1:
                    s = GetTravelResposefromNavitaire();
                    ViewBag.OriginPlace = "Departure : " + sodRequestsList[0].OriginPlace + "-" + sodRequestsList[0].DestinationPlace;
                    break;
                case 2:
                    s = GetTravelResposefromNavitaire();
                    ViewBag.OriginPlace = "Departure : " + sodRequestsList[0].OriginPlace + "-" + sodRequestsList[0].DestinationPlace;
                    ViewBag.ReturnPlace = "Return :" + sodRequestsList[0].DestinationPlace + "-" + sodRequestsList[0].OriginPlace;
                    break;
                case 3:
                    s = GetTravelResposefromNavitaire();
                    break;
            }
            return Json(s, JsonRequestBehavior.AllowGet);
        }
        public string GetTravelResposefromNavitaire()
        {
            var lst = TempData["SodbookingRequest"] as List<TravelRequestModels>;
            var s = "";
            switch (lst[0].TravelRequestTypeId)
            {
                case 1:
                case 2:
                    s = NavitaireServices.GetAvailability1(lst[0].OriginPlace, lst[0].DestinationPlace, lst[0].TravelDate,
                   lst[0].ReturnDate, lst[0].TravelRequestTypeId.ToString(), lst[0].Passengers, lst[0].BookingFor);
                    break;
                case 3:
                    s = NavitaireServices.GetAvailabilityMulticity(lst);
                    break;
            }
            return s;
        }
    }
}