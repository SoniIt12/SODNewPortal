using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SOD.CommonWebMethod
{
    public class SessionTimeoutAttribute : ActionFilterAttribute
    {
       
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                HttpContext ctx = HttpContext.Current;
                if (HttpContext.Current.Session["EmpId"] == null)
                {
                    filterContext.Result = new RedirectResult("~/Login/UserAccount");
                    return;
                }
                base.OnActionExecuting(filterContext);
            }
        
        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    HttpContext ctx = HttpContext.Current;
        //    if (HttpContext.Current.Session["EmpId"] == null)
        //    {
        //        filterContext.Result = new RedirectToRouteResult(
        //        new RouteValueDictionary
        //        {
        //            { "controller", "Login" },
        //            { "action", "UserAccount" }
        //        });
        //        return;
        //    }
        //    base.OnActionExecuting(filterContext);
        //}
    }
}