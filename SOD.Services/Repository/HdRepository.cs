using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOD.Services.Repository
{
    public class HdRepository :IHdRepository
    {
        
        /// <summary>
        /// Initialized Constructor
        /// </summary>
        private readonly SodEntities _context;
        public HdRepository(SodEntities sodEntities)
        {
            this._context = sodEntities;
        }



        /// <summary>
        /// Get Sod Emloyee Booking History
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="EmpId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetSodEmployeeBookingHistoryList_Helpdesk(int? departmentId, int? EmpId, int criteria)
        {
            return ADO.SodCommonServices.GetSodEmployeeBookingHistoryList_Helpdesk(departmentId, EmpId, criteria);
        }


        
        /// <summary>
        /// Get Sod Employee Booking History  for Export to Excel  Data
        /// </summary>
        /// <param name="fdate"></param>
        /// <param name="tdate"></param>
        /// <param name="type"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetSodEmployeeBookingHistoryList_Helpdesk_ExcelExport(string fdate, string tdate, short type, string EmpId,string Dept,short criteria)
        {
            return ADO.SodCommonServices.GetSodEmployeeBookingHistoryList_Helpdesk_ExcelExport(fdate, tdate,type,EmpId,Dept, criteria);
        }


        /// <summary>
        /// Get Employee Data
        /// </summary>
        /// <param name="EmpId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetSodEmployeeViewDetails(string EmpId, int criteria)
        {
            return ADO.SodCommonServices.GetSodEmployeeViewDetails(EmpId, criteria);
        }




        /// <summary>
        /// Dispose Method
        /// </summary>
        public void Dispose()
        {
            throw new NotImplementedException();
        }

       
    }
}
