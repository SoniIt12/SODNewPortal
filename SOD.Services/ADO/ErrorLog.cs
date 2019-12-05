using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;
using SOD.Services.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SOD.Services.ADO
{
    public class ErrorLog : Exception
    {

        public readonly ILoggingRepository _loggingRepository;

        /// <summary>
        /// Initilize Constructor
        /// </summary>
        public ErrorLog()
        {
            this._loggingRepository = new LoggingRepository(new SodEntities());
        }


        /// <summary>
        /// Manage Database Logging
        /// </summary>
        /// <param name="ex"></param>
        public string AddDBLogging(Exception ex, string methosName, string filePath)
        {
            LoggingModels obj = new LoggingModels();
            obj.LogDate = System.DateTime.Now;
            obj.LogMessage = ex.Message.ToString();
            obj.LogData = ex.Data.ToString();
            obj.LogSource = ex.Source;
            obj.HelpLink = ex.HelpLink == null ? "" : ex.HelpLink;
            obj.HResult = ex.HResult == null ? "" : ex.HResult.ToString();
            obj.InnerException = ex.InnerException == null ? "" : ex.InnerException.Message;
            obj.EmpId = 0;
            obj.MethodName = methosName;
            obj.FilePath = filePath;
            return _loggingRepository.SaveDBLogg(obj);
        }
    }
}
