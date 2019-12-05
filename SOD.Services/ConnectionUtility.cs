using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Services
{
    class ConnectionUtility
    {
        public static string GetConnection()
        {
            return ConfigurationManager.ConnectionStrings["SodEntities"].ConnectionString;
        }
    }
}
