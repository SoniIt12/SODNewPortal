using System;
using System.Collections.Generic;
using System.Web.Mvc;
using SOD.CommonWebMethod;
using SOD.Model;

namespace SOD.Controllers
{
    public class TravelSelectionController : Controller,IActionFilter,IExceptionFilter
    {
        // GET: TravelSelection
        public ActionResult TrSelection()
        {
              
            var lst =TempData["SodbookingRequest"] as List<TravelRequestModels>;

            if (lst != null)
            {
                ViewBag.TravelRequestTypeId = lst[0].TravelRequestTypeId;
                TempData["TravelRequestTypeId"] = lst[0].TravelRequestTypeId;

                TempData["Passengers"] = lst[0].Passengers;
                switch (lst[0].TravelRequestTypeId)
                {
                    case 1:
                        TempData["NavitaireData"] = GetTravelResposefromNavitaire();
                        ViewBag.OriginPlace = "Departure : " + lst[0].OriginPlace + "-" + lst[0].DestinationPlace;
                        ViewBag.TravelRequestTypeId = lst[0].TravelRequestTypeId;
                        break;
                    case 2:
                        TempData["NavitaireData"] = GetTravelResposefromNavitaire();
                        ViewBag.OriginPlace = "Departure : " + lst[0].OriginPlace + "-" + lst[0].DestinationPlace;
                        ViewBag.ReturnPlace = "Return :" + lst[0].DestinationPlace + "-" + lst[0].OriginPlace;
                        break;
                    case 3:
                        TempData["NavitaireData"] = GetTravelResposefromNavitaire();
                        break;
                }
            }
            return View();
        }

        /// <summary>
        /// Get Travel request
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetTravelRequest()
        { 
                var s = TempData["NavitaireData"];
                return Json(s, JsonRequestBehavior.AllowGet);
        }

       
        
       /// <summary>
       /// One way Booking Response
       /// </summary>
       /// <returns></returns>
       public string GetTravelResposefromNavitaire()
        {
            var lst = TempData["SodbookingRequest"] as List<TravelRequestModels>;
            var s="";
            switch (lst[0].TravelRequestTypeId)
            {
                case 1  :
                case 2:
                         s= NavitaireServices.GetAvailability1(lst[0].OriginPlace, lst[0].DestinationPlace, lst[0].TravelDate,
                        lst[0].ReturnDate, lst[0].TravelRequestTypeId.ToString(), lst[0].Passengers,lst[0].BookingFor);
                    break;
                case 3:
                         s= NavitaireServices.GetAvailabilityMulticity(lst);
                    break;
            }
            return s;
        }

       /// <summary>
       /// Action Filter Method
       /// </summary>
       /// <param name="filterContext"></param>
       void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
       {
           //If Required : Need to Implement
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
               Response.Redirect("../Login/UserAccount");
           }
       }


       /// <summary>
       /// Exception Handling
       /// </summary>
       /// <param name="filterContext"></param>
       void OnException(ExceptionContext filterContext)
       {
           Exception ex = filterContext.Exception;
           Logging.ErrorLog.Instance.AddDBLogging(ex, filterContext.Controller.ControllerContext.ToString(), "Controllers/TravelSelectionController.cs");
       }
    }
}