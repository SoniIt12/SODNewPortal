using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;
using SOD.Services.Repository;
using System;
using System.IO;
using System.Web;


namespace SOD.Logging
{
    public class ErrorLog
    {
        private static ErrorLog instance = null;
        public readonly ILoggingRepository _loggingRepository;
        public ErrorLog()
        {
            this._loggingRepository = new LoggingRepository(new SodEntities());
        }

        // Lock synchronization object
        private static object syncLock = new object();

        public static ErrorLog Instance
        {
            get
            {
                // Support multithreaded applications through
                // 'Double checked locking' pattern which
                lock (syncLock)
                {
                    if (ErrorLog.instance == null)
                        ErrorLog.instance = new ErrorLog();

                    return ErrorLog.instance;
                }
            }
        }


       /// <summary>
       /// Manage Error Log
       /// </summary>
       /// <param name="ex"></param>
        public static void AddEmailLogg(Exception ex )
        {
            string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            message += string.Format("Message: {0}", ex.Message);
            message += Environment.NewLine;
            message += string.Format("StackTrace: {0}", ex.Data);
            message += Environment.NewLine;
            message += string.Format("Source: {0}", ex.Source);
            message += Environment.NewLine;
            message += string.Format("TargetSite: {0}", ex.TargetSite.ToString());
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            string path = HttpContext.Current.Server.MapPath("~/ErrorLog//Log.txt");
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(message);
                writer.Close();
            }
        }


         /// <summary>
         /// Write Error Log
         /// </summary>
         /// <param name="strmsg"></param>
        public static void WriteLogg(String strmsg,String FileName)
        {
            string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
            message += Environment.NewLine;
            message += "-----------------------------------------------------------------------------------";
            message += Environment.NewLine;
            message += strmsg;
            //message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            string path = Path.Combine(HttpRuntime.AppDomainAppPath, "ErrorLog/"  +FileName);
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(message);
                writer.Close();
            }
        }


        /// <summary>
        /// Manage Database Logging
        /// </summary>
        /// <param name="ex"></param>
        public string AddDBLogging(Exception ex, string methosName, string filePath)
        {
            LoggingModels obj = new LoggingModels();
            try
            {               
                obj.LogDate = System.DateTime.Now;
                obj.LogMessage = ex.Message;
                obj.LogData = ex.Data.ToString();
                obj.LogSource = ex.Source;
                obj.HelpLink = ex.HelpLink == null ? "" : ex.HelpLink;
                obj.HResult = ex.HResult == 0 ? "" : ex.HResult.ToString();//by soni 5 nov 2019
                obj.InnerException = ex.InnerException == null ? "" : ex.InnerException.Message;
                obj.EmpId = HttpContext.Current.Session["EmpId"] == null ? 0 : Convert.ToInt32(HttpContext.Current.Session["EmpId"]);
                obj.MethodName = methosName;
                obj.FilePath = filePath;
            }
            catch(Exception)
            {

            }
            return _loggingRepository.SaveDBLogg(obj);
        }




        /// <summary>
        /// Manage Error Log
        /// </summary>
        /// <param name="ex"></param>
        public static void TestLogg(string msg)
        {
            string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            message += string.Format("Message: {0}", msg);
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            string path = HttpContext.Current.Server.MapPath("~/ErrorLog/Log.txt");
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(message);
                writer.Close();
            }
        }
    }
}