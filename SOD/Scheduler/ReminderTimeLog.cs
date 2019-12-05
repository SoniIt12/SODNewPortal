using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SOD.Scheduler
{
    public static class ReminderTimeLog
    {        

        /// <summary>
        /// Get Time to run auto job
        /// </summary>
        /// <returns></returns>
        public static int GetTimeLog()
        {
            int time = 0;
            try
            {                
                string path = Path.Combine(HttpRuntime.AppDomainAppPath, "TimeLog/Log.txt");
                using (StreamReader sr = new StreamReader(path, true))
                {
                    var line = sr.ReadLine();                    
                    if (line != null)
                    {
                        time = Int32.Parse(line);                                           
                    }                    
                    sr.Close();
                }
                using (StreamWriter writer = new StreamWriter(path, false))
                {
                    time = time + 5;
                    writer.WriteLine(time);                    
                    writer.Close();
                }               
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }           
            return time;
        }


        /// <summary>
        /// Manage Job timing/ Reset auto run job timing
        /// </summary>
        /// <returns></returns>
        public static int ResetTextTimeLog()
        {
            int time = 0;
            string path = Path.Combine(HttpRuntime.AppDomainAppPath, "TimeLog/Log.txt");
            using (StreamWriter writer = new StreamWriter(path, false))
            {
                writer.WriteLine(time);
                writer.Close();
            }           
            return time;
        }        
    }
}