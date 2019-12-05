using SOD.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Services.Interface
{
    
    /// <summary>
    /// Manage Logining
    /// </summary>
    public  interface ILoggingRepository :IDisposable
    {
        string SaveDBLogg(LoggingModels loggingModel);
    }
}
