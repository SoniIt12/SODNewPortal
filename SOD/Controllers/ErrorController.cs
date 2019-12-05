using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SOD.CommonWebMethod;


namespace SOD.Controllers
{
    [SessionTimeout]
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Error(string message)
        {
            TempData["ErrorMessage"] = Request.QueryString[0].Trim();
            return View("Error");
        }

        // GET: /Error/HttpError404
        [HttpGet]
        public ActionResult HttpError404(string message)
        {
            return View("HttpError404", message);
        }

        // GET: /Error/HttpError500
        [HttpGet]
        public ActionResult HttpError500(string message)
        {
            return View("HttpError500", message);
        }
    }
}