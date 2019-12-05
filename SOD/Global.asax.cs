using SOD.App_Start;
using SOD.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;



namespace SOD
{
    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Application start Event
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);            
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //AppScheduler.Start();            
            //SmsScheduler.Start();  
        }


        /// <summary>
        /// Application Close/End Event
        /// </summary>
        protected void Application_End()
        {
            if (Session["EmpId"] != null)
                Session.Abandon();
        }


        /// <summary>
        /// Session End Event
        /// </summary>
        protected void Session_End()
        {
            Session.Abandon();
        }

        /// <summary>
        /// Error Handling
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            Response.Clear();
            var httpException = exception as HttpException;
            if (httpException != null)
            {
                string action;
                switch (httpException.GetHttpCode())
                {
                    case 404:
                        // page not found
                        action = "HttpError404";
                        break;
                    case 500:
                        // server error
                        action = "HttpError500";
                        break;
                    default:
                        action = "Error";
                        break;
                }
                //Error Log
                ErrorLog objLogging = new ErrorLog();
                objLogging.AddDBLogging(exception, "Application_Error", "Global.asax.cs");
                HttpContext.Current.Server.ClearError();
                var path = "../Error/" + action + "?message=" + httpException.Message.ToString();
                HttpContext.Current.Response.Redirect(path, false);
            }
        }
    }
}
