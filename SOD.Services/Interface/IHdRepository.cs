using SOD.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Services.Interface
{
    /// <summary>
    /// Help Desk 
    /// </summary>
    public interface IHdRepository : IDisposable
    {

        /// <summary>
        /// Get Sod Employee Booking History List
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="EmpId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        Dictionary<string, object> GetSodEmployeeBookingHistoryList_Helpdesk(int? departmentId, int? EmpId, int criteria);

       
        
        
       /// <summary>
        /// Get Sod Employee Booking History  for Export to Excel  Data
       /// </summary>
       /// <param name="fdate"></param>
       /// <param name="tdate"></param>
       /// <param name="type"></param>
       /// <param name="criteria"></param>
       /// <returns></returns>
        Dictionary<string, object> GetSodEmployeeBookingHistoryList_Helpdesk_ExcelExport(string fdate, string tdate, short type, string EmpId, string Dept, short criteria);



        /// <summary>
        /// Get All Employee Data
        /// </summary>
        /// <param name="EmpId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        Dictionary<string, object> GetSodEmployeeViewDetails(string EmpId, int criteria);




    }
}
