using System.Collections.Generic;
using SOD.Model;
using SOD.Services.EntityFramework;
using SOD.Services.Interface;

namespace SOD.Services.Repository
{
    public class RevenueRepository:IRevenueRepository
    {
        /// <summary>
        /// Initialized Constructor
        /// </summary>
        private readonly SodEntities _context;
        public RevenueRepository(SodEntities sodEntities)
        {
            this._context = sodEntities;
        }


        
        /// <summary>
        /// Get Sod Booking List For Approver :Standby Booking
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="designationId"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public IList<TravelRequestModels> GetSodBookingListForApproval(int departmentId, int designationId,int EmpId, int criteria)
        {
            return ADO.SodCommonServices.GetSodBookingListForApproval(departmentId, designationId,EmpId, criteria,null);
        }


        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
 
    }
}